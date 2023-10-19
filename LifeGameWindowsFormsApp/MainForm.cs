using System;
using System.Drawing;
using System.Windows.Forms;

namespace LifeGameWindowsFormsApp
{
    public partial class MainForm : Form
    {
        const int mapSize = 30;
        const int cellSize = 30;

        int[,] currentState = new int[mapSize, mapSize]; //массив  текущих поколений
        int[,] nextState = new int[mapSize, mapSize];    //будущих

        Button[,] cells = new Button[mapSize, mapSize];
        bool isPlaying = false;
        Timer mainTimer;
        int offset = 25;

        public MainForm()
        {
            InitializeComponent();
            SetFormSize();
            BildMenu();
            Init();
        }

        private void SetFormSize()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize + 40;
        }

        public void Init()
        {
            isPlaying = false;
            mainTimer = new Timer();
            mainTimer.Interval = 100;
            mainTimer.Tick += new EventHandler(UpdateStates);

            //заполняем оба массива нулями
            currentState = InitMap();
            nextState = InitMap();
            InitCells();
        }

        public void ClearGame()
        {
            isPlaying = false;
            mainTimer = new Timer();
            mainTimer.Interval = 100;
            mainTimer.Tick += new EventHandler(UpdateStates);

            //заполняем оба массива нулями
            currentState = InitMap();
            nextState = InitMap();
            ResetCells();

        }

        void ResetCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    cells[i, j].BackColor = Color.White;
                }
            }
        }

        void BildMenu()
        {
            var menu = new MenuStrip();
            var restart = new ToolStripMenuItem("Restart");
            restart.Click += new EventHandler(Restart);

            var play = new ToolStripMenuItem("Play");
            play.Click += new EventHandler(Play);

            menu.Items.Add(restart);
            menu.Items.Add(play);
            this.Controls.Add(menu);
        }

        private void Play(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mainTimer.Start();
            }
        }

        bool CheckGenerationDead()
        {           
            for(int i = 0; i < mapSize;i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        return false;
                }
            }
            return true;
        }

        private void Restart(object sender, EventArgs e)
        {
            mainTimer.Stop();
            ClearGame();
        }

        private void UpdateStates(object sender, EventArgs e)
        {
            CalculateNextState();
            DisplayMap();
            if(CheckGenerationDead())
            {
                mainTimer.Stop();
                MessageBox.Show("Generation was dead");
            }
        }

        void CalculateNextState() //подсчет слудующих состояний
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var countNeighboors = CountNeighboors(i, j);
                    if (currentState[i, j] == 0 && countNeighboors == 3)
                    {
                        nextState[i, j] = 1;
                    }
                    else if (currentState[i, j] == 1 && (countNeighboors < 2 && countNeighboors > 3))
                    {
                        nextState[i, j] = 0;
                    }
                    else if (currentState[i, j] == 1 && (countNeighboors >= 2 && countNeighboors <= 3))
                    {
                        nextState[i, j] = 1;
                    }
                    else
                    {
                        nextState[i, j] = 0;
                    }
                }
            }
            currentState = nextState;
            nextState = InitMap();
        }

        //отобразить на карте после подсчета состояний
        void DisplayMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        cells[i, j].BackColor = Color.Black;
                    else cells[i, j].BackColor = Color.White;
                }
            }
        }

        int CountNeighboors(int i, int j)  //подсчет соседеей (вокруг восемь клеток)
        {
            var count = 0;
            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (!IsInsideMap(k, l))
                    {
                        continue;
                    }
                    if (k == i && l == j)
                    {
                        continue;
                    }
                    if (currentState[k, l] == 1)
                    {
                        count++;
                    }
                }
            }
            return count;
        }


        //понадобится узнать, что мы вышли за пределы
        bool IsInsideMap(int i, int j)
        {
            if (i < 0 || i >= mapSize || j < 0 || j >= mapSize)
            {
                return false;
            }
            return true;
        }

        int[,] InitMap()
        {
            int[,] arr = new int[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    arr[i, j] = 0;
                }
            }
            return arr;
        }

        void InitCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    button.Location = new Point(j * cellSize, (i * cellSize) + offset);
                    //добавим обработчик нажатия на кнопку
                    button.Click += new EventHandler(OnCellClick);
                    this.Controls.Add(button);  //добавили кнопку на форму и включили в массив
                    cells[i, j] = button;
                }
            }
        }

        private void OnCellClick(object sender, EventArgs e)
        {
            /*Если сейчас состояние не игровое, мы
             * имеем возможность задать положение
             * для начальной симуляции
             * **/
            var pressButton = sender as Button;
            if (!isPlaying)
            {
                //получаем необходимые индексы
                var i = (pressButton.Location.Y - offset) / cellSize;
                var j = pressButton.Location.X / cellSize;
                //проверяем, что текущие состояния по этим индексам равны 0
                if (currentState[i, j] == 0)
                {
                    currentState[i, j] = 1;
                    cells[i, j].BackColor = Color.Black;
                }
                else
                {
                    currentState[i, j] = 0;
                    cells[i, j].BackColor = Color.White;
                }
            }
        }
    }
}
