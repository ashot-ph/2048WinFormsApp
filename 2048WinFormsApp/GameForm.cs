namespace _2048WinFormsApp;

public partial class GameForm : Form
{
    public int mapSize { get; set; }
    public int bestScore;
    public int score = 0;
    public Users user { get; set; }

    private int numbers { get; set; }
    private const int labelSize = 80;
    private const int padding = 6;
    private const int start_x = 10;
    private const int start_y = 80;
    private Label[,] labelsMap;

    public GameForm()
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void GameForm_Load(object sender, EventArgs e)
    {
        InitMap();
        GenerateNumber();
        ShowScore();
        CalculateBestScore();
    }

    public void CalculateMapSize(int size)
    {
        mapSize = size;
    }

    private void CalculateBestScore()
    {
        var users = UserStorage.GetUserResults();
        if (users.Count == 0)
        {
            return;
        }
        bestScore = users[0].Score;
        foreach (var user in users)
        {
            if (user.Score > bestScore)
            {
                bestScore = user.Score;
            }
        }
        ShowBestScore();
    }

    private void ShowBestScore()
    {
        if (score > bestScore)
        {
            bestScore = score;
        }
        bestScoreLabel.Text = bestScore.ToString();
    }

    private void ShowScore()
    {
        scoreLabel.Text = score.ToString();
    }

    private void InitMap()
    {
        ClientSize = new Size(start_x + (labelSize + padding) * mapSize, start_y + (labelSize + padding) * mapSize);
        labelsMap = new Label[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                var newLabel = CreateLabel(i, j);
                Controls.Add(newLabel);
                labelsMap[i, j] = newLabel;
            }
        }
    }

    private void GenerateNumber()
    {
        var random = new Random();
        var randomNumberLabel = random.Next(mapSize * mapSize);
        var indexRow = randomNumberLabel / mapSize;
        var indexColumn = randomNumberLabel % mapSize;
        for (int j = 0; j < mapSize; j++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (labelsMap[indexRow, indexColumn].Text == string.Empty)
                {
                    labelsMap[indexRow, indexColumn].Text = GenerateValue();
                }
                break;
            }
        }
    }

    private string GenerateValue()
    {
        var random = new Random();
        var randomNumbers = random.Next(4);
        if (randomNumbers == 0)
        {
            numbers = 4;
        }
        else
        {
            numbers = 2;
        }
        return Convert.ToString(numbers);
    }

    private Label CreateLabel(int indexRow, int indexColumn)
    {
        var label = new Label();
        label.BackColor = SystemColors.ButtonShadow;
        label.Font = new Font("Cambria", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label.Size = new Size(labelSize, labelSize);
        label.TextAlign = ContentAlignment.MiddleCenter;
        var x = start_x + indexColumn * (labelSize + padding);
        var y = start_y + indexRow * (labelSize + padding);
        label.Location = new Point(x, y);
        label.TextChanged += Label_TextChanged;
        return label;
    }

    private void Label_TextChanged(object? sender, EventArgs e)
    {
        var label = (Label)sender;
        switch (label.Text)
        {
            case "": label.BackColor = SystemColors.ButtonShadow; break;
            case "2": label.BackColor = Color.FromArgb(238, 228, 218); break;
            case "4": label.BackColor = Color.FromArgb(231, 224, 200); break;
            case "8": label.BackColor = Color.FromArgb(242, 177, 121); break;
            case "16": label.BackColor = Color.FromArgb(245, 149, 99); break;
            case "32": label.BackColor = Color.FromArgb(255, 116, 85); break;
            case "64": label.BackColor = Color.FromArgb(245, 95, 58); break;
            case "128": label.BackColor = Color.FromArgb(247, 207, 95); break;
            case "256": label.BackColor = Color.FromArgb(153, 131, 60); break;
            case "512": label.BackColor = Color.FromArgb(207, 111, 41); break;
            case "1024": label.BackColor = Color.FromArgb(189, 15, 165); break;
            case "2048": label.BackColor = Color.FromArgb(212, 11, 57); break;
        }
    }

    private void GameForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Right && e.KeyCode != Keys.Left && e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
        {
            return;
        }
        if (e.KeyCode == Keys.Right)
        {
            MoveRight();
            GenerateNumber();
        }
        if (e.KeyCode == Keys.Left)
        {
            MoveLeft();
            GenerateNumber();
        }
        if (e.KeyCode == Keys.Up)
        {
            MoveUp();
            GenerateNumber();
        }
        if (e.KeyCode == Keys.Down)
        {
            MoveDown();
            GenerateNumber();
        }

        GenerateNumber();
        ShowScore();
        ShowBestScore();

        if (Win())
        {
            user.Score = score;
            MessageBox.Show($"УРА! Вы выйграли! {user.Name}, Ваше количество очков : {user.Score}");
            var userStorage = new UserStorage();
            userStorage.Add(user);

            ChoiceForm choiceForm = new ChoiceForm();
            choiceForm.ShowDialog();
            this.Close();
        }
        if (EndGame())
        {
            user.Score = score;
            MessageBox.Show($"Увы..., но Вы проиграли!  {user.Name}, Ваше количество очков :{user.Score}");
            var userStorage = new UserStorage();
            userStorage.Add(user);

            ChoiceForm choiceForm = new ChoiceForm();
            choiceForm.ShowDialog();
            Close();
        }
    }

    private void MoveDown()
    {
        for (int j = 0; j < mapSize; j++)
        {
            for (int i = mapSize - 1; i >= 0; i--)
            {
                if (labelsMap[i, j].Text != string.Empty)
                {
                    for (int k = i - 1; k >= 0; k--)
                    {
                        if (labelsMap[k, j].Text != string.Empty)
                        {
                            if (labelsMap[i, j].Text == labelsMap[k, j].Text)
                            {
                                var number = int.Parse(labelsMap[i, j].Text);
                                score += number * 2;
                                labelsMap[i, j].Text = (number * 2).ToString();
                                labelsMap[k, j].Text = string.Empty;
                            }
                            break;
                        }
                    }
                }
            }
        }

        for (int j = 0; j < mapSize; j++)
        {
            for (int i = mapSize - 1; i >= 0; i--)
            {
                if (labelsMap[i, j].Text == string.Empty)
                {
                    for (int k = i - 1; k >= 0; k--)
                    {
                        if (labelsMap[k, j].Text != string.Empty)
                        {
                            labelsMap[i, j].Text = labelsMap[k, j].Text;
                            labelsMap[k, j].Text = string.Empty;
                            break;
                        }
                    }

                }
            }
        }
    }

    private void MoveUp()
    {
        for (int j = 0; j < mapSize; j++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (labelsMap[i, j].Text != string.Empty)
                {
                    for (int k = i + 1; k < mapSize; k++)
                    {
                        if (labelsMap[k, j].Text != string.Empty)
                        {
                            if (labelsMap[i, j].Text == labelsMap[k, j].Text)
                            {
                                var number = int.Parse(labelsMap[i, j].Text);
                                score += number * 2;
                                labelsMap[i, j].Text = (number * 2).ToString();
                                labelsMap[k, j].Text = string.Empty;
                            }
                            break;
                        }
                    }
                }
            }
        }

        for (int j = 0; j < mapSize; j++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (labelsMap[i, j].Text == string.Empty)
                {
                    for (int k = i + 1; k < mapSize; k++)
                    {
                        if (labelsMap[k, j].Text != string.Empty)
                        {
                            labelsMap[i, j].Text = labelsMap[k, j].Text;
                            labelsMap[k, j].Text = string.Empty;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveLeft()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (labelsMap[i, j].Text != string.Empty)
                {
                    for (int k = j + 1; k < mapSize; k++)
                    {
                        if (labelsMap[i, k].Text != string.Empty)
                        {
                            if (labelsMap[i, j].Text == labelsMap[i, k].Text)
                            {
                                var number = int.Parse(labelsMap[i, j].Text);
                                score += number * 2;
                                labelsMap[i, j].Text = (number * 2).ToString();
                                labelsMap[i, k].Text = string.Empty;
                            }
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (labelsMap[i, j].Text == string.Empty)
                {
                    for (int k = j + 1; k < mapSize; k++)
                    {
                        if (labelsMap[i, k].Text != string.Empty)
                        {
                            labelsMap[i, j].Text = labelsMap[i, k].Text;
                            labelsMap[i, k].Text = string.Empty;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveRight()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = mapSize - 1; j >= 0; j--)
            {
                if (labelsMap[i, j].Text != string.Empty)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (labelsMap[i, k].Text != string.Empty)
                        {
                            if (labelsMap[i, j].Text == labelsMap[i, k].Text)
                            {
                                var number = int.Parse(labelsMap[i, j].Text);
                                score += number * 2;
                                labelsMap[i, j].Text = (number * 2).ToString();
                                labelsMap[i, k].Text = string.Empty;
                            }
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = mapSize - 1; j >= 0; j--)
            {
                if (labelsMap[i, j].Text == string.Empty)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (labelsMap[i, k].Text != string.Empty)
                        {
                            labelsMap[i, j].Text = labelsMap[i, k].Text;
                            labelsMap[i, k].Text = string.Empty;
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool Win()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (labelsMap[i, j].Text == "ура победа")
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool EndGame()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (labelsMap[i, j].Text == "")
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < mapSize - 1; i++)
        {
            for (int j = 0; j < mapSize - 1; j++)
            {
                if (labelsMap[i, j].Text == labelsMap[i, j + 1].Text || labelsMap[i, j].Text == labelsMap[i + 1, j].Text)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
