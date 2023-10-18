using System;
using System.Drawing;
using System.Windows.Forms;

namespace LifeGameWindowsFormsApp
{
    public partial class MainForm : Form
    {
        const int mapSize = 10;
        const int cellSize = 30;

        int[,] currentState = new int[mapSize,mapSize]; //массив  текущих поколений
        int[,] nextState = new int[mapSize,mapSize];    //будущих

        Button[,] cells = new Button[mapSize, mapSize];
        bool isPlaying = false;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }       

        public void Init()
        {
            //заполняем оба массива нулями
            currentState = InitMap(currentState);
            nextState = InitMap(nextState);
            InitCells();
        }

        int[,] InitMap(int[,] arr)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for(int j = 0; j < mapSize; j++)
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
                    button.Size = new Size(cellSize,cellSize);
                    button.BackColor = Color.White;
                    button.Location = new Point(j*cellSize, i*cellSize);
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
                var i = pressButton.Location.Y/cellSize;
                var j = pressButton.Location.X / cellSize;
                //проверяем, что текущие состояния по этим индексам равны 0
                if (currentState[i,j] == 0)
                {
                    currentState[i,j] = 1;
                    cells[i,j].BackColor = Color.Black;
                }
                else
                {
                    currentState[i,j] = 0;
                    cells[i,j].BackColor= Color.White;
                }
            }


        }
    }
}
