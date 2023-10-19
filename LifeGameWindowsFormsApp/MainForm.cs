using System;
using System.Drawing;
using System.Windows.Forms;

namespace LifeGameWindowsFormsApp
{
    public partial class MainForm : Form
    {
        const int mapSize = 10;
        const int cellSize = 30;

        int[,] currentState = new int[mapSize, mapSize]; //массив  текущих поколений
        int[,] nextState = new int[mapSize, mapSize];    //будущих

        Button[,] cells = new Button[mapSize, mapSize];
        bool isPlaying = false;
        Timer mainTimer;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            mainTimer = new Timer();
            mainTimer.Interval = 100;
            mainTimer.Tick += new EventHandler(UpdateStates);

            //заполняем оба массива нулями
            currentState = InitMap();
            nextState = InitMap();
            InitCells();
        }

        private void UpdateStates(object sender, EventArgs e)
        {
            CalculateNextState();
            DisplayMap();
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
            for(int i = 0; i < mapSize;i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        cells[i,j].BackColor = Color.Black;
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
                    if (k == i && l ==j)
                    {
                        continue;
                    }
                    if (currentState[k,l] == 1)
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
                    button.Location = new Point(j * cellSize, i * cellSize);
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
                var i = pressButton.Location.Y / cellSize;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mainTimer.Start();
            }
        }
    }
}
