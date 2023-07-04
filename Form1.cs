using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MosaicApp
{
    public partial class Form1 : Form
    {
        private const int SquareSize = 30;
        private const int GridSize = 6;

        private Color[,] mosaicGrid;
        private Color[,] userGrid;
        private Color selectedColor;

        public Form1()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            
            InitializeComponent();

            // Создаем сетку мозаики и пользовательскую сетку
            mosaicGrid = new Color[GridSize, GridSize];
            userGrid = new Color[GridSize, GridSize];
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    mosaicGrid[i, j] = Color.White;
                    userGrid[i, j] = Color.Gray;
                }
            }

            // Устанавливаем начальный выбранный цвет
            selectedColor = Color.Red;

            // Создаем элементы управления и размещаем их на форме
            CreateMosaicGrid();
            CreateUserGrid();

            CreateColorPalette(); // Добавлено создание палитры

            CreateNewImageButton();
            CreateClearButton();

            ResetAllElements();
        }


        private void CreateMosaicGrid()
        {
            const int offsetX = 20;
            const int offsetY = 20;

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Panel panel = new Panel();
                    panel.BackColor = mosaicGrid[i, j];
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.Location = new Point(offsetX + i * SquareSize, offsetY + j * SquareSize);
                    panel.Size = new Size(SquareSize, SquareSize);

                    panel.AllowDrop = true;
                    panel.DragEnter += MosaicPanel_DragEnter;
                    panel.DragDrop += MosaicPanel_DragDrop;

                    Controls.Add(panel);
                }
            }
        }

        private void CreateUserGrid()
        {
            const int offsetX = 320;
            const int offsetY = 20;

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Panel panel = new Panel();
                    panel.BackColor = userGrid[i, j];
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.Location = new Point(offsetX + i * SquareSize, offsetY + j * SquareSize);
                    panel.Size = new Size(SquareSize, SquareSize);

                    panel.AllowDrop = true;
                    panel.DragEnter += UserPanel_DragEnter;
                    panel.DragDrop += UserPanel_DragDrop;

                    Controls.Add(panel);
                }
            }
        }

        //private void CreateColorPalette()
        //{
        //    const int offsetX = 680;
        //    const int offsetY = 30;

        //    Color[] colors = { Color.White, Color.Green, Color.Blue, Color.Yellow, Color.Red };

        //    for (int i = 0; i < colors.Length; i++)
        //    {
        //        Panel panel = new Panel();
        //        panel.BackColor = colors[i];
        //        panel.BorderStyle = BorderStyle.FixedSingle;
        //        panel.Location = new Point(offsetX, offsetY + i * SquareSize);
        //        panel.Size = new Size(SquareSize, SquareSize);

        //        panel.MouseDown += ColorPanel_MouseDown;

        //        Controls.Add(panel);
        //    }
        //}

        private void CreateColorPalette()
        {
            const int offsetX = 680;
            const int offsetY = 30;

            Color[] colors = { Color.White, Color.Green, Color.Blue, Color.Red, Color.Yellow };

            for (int i = 0; i < colors.Length; i++)
            {
                Panel panel = new Panel();
                panel.BackColor = colors[i];
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Location = new Point(offsetX, offsetY + i * SquareSize);
                panel.Size = new Size(SquareSize, SquareSize);

                panel.MouseDown += ColorPanel_MouseDown;

                Controls.Add(panel);
            }
        }







        private void ResetAllElements()
        {
            ResetMosaicGrid();
           
        }

        private void CreateNewImageButton()
        {
            const int offsetX = 30;
            const int offsetY = 225;

            Button button = new Button();
            button.Text = "Новый рисунок";
            button.Location = new Point(offsetX, offsetY);
            button.Size = new Size(100, 30);

            button.Click += NewImageButton_Click;

            Controls.Add(button);
        }

        private void MosaicPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Panel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MosaicPanel_DragDrop(object sender, DragEventArgs e)
        {
            Panel sourcePanel = (Panel)e.Data.GetData(typeof(Panel));
            Panel targetPanel = (Panel)sender;

            int sourceIndexX = (sourcePanel.Location.X - 20) / SquareSize;
            int sourceIndexY = (sourcePanel.Location.Y - 20) / SquareSize;
            int targetIndexX = (targetPanel.Location.X - 20) / SquareSize;
            int targetIndexY = (targetPanel.Location.Y - 20) / SquareSize;

            if (sourceIndexX == targetIndexX && sourceIndexY == targetIndexY)
            {
                if (mosaicGrid[targetIndexX, targetIndexY] == selectedColor || mosaicGrid[targetIndexX, targetIndexY] == Color.White)
                {
                    targetPanel.BackColor = sourcePanel.BackColor;
                    mosaicGrid[targetIndexX, targetIndexY] = sourcePanel.BackColor;

                    bool isMosaicComplete = CheckMosaicComplete();
                    if (isMosaicComplete)
                    {
                        MessageBox.Show("Поздравляю! Вы собрали рисунок.", "Успех");
                        ResetMosaicGrid();
                    }
                }
                else
                {
                    MessageBox.Show("Этот цвет не подходит для данного квадрата.", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Этот квадрат не подходит.", "Ошибка");
            }
        }


        private void UserPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Panel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void UserPanel_DragDrop(object sender, DragEventArgs e)
        {
            Panel sourcePanel = (Panel)e.Data.GetData(typeof(Panel));
            Panel targetPanel = (Panel)sender;

            int targetIndexX = (targetPanel.Location.X - 320) / SquareSize;
            int targetIndexY = (targetPanel.Location.Y - 20) / SquareSize;

            Color expectedColor = mosaicGrid[targetIndexX, targetIndexY]; // Цвет, ожидаемый для ячейки мозаичной сетки

            if (targetPanel.BackColor == Color.Gray && selectedColor == expectedColor) // Проверка, является ли целевая панель серым квадратом и совпадает ли выбранный цвет с ожидаемым цветом
            {
                targetPanel.BackColor = selectedColor;
                userGrid[targetIndexX, targetIndexY] = selectedColor;

                bool isMosaicComplete = CheckMosaicComplete();
                if (isMosaicComplete)
                {
                    MessageBox.Show("Поздравляю! Вы собрали рисунок.", "Успех");
                    ResetMosaicGrid();
                }
            }
            else
            {
                MessageBox.Show("Этот квадрат не подходит.", "Ошибка");
            }
        }



        private void ColorPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            selectedColor = panel.BackColor;
            panel.DoDragDrop(panel, DragDropEffects.Copy);
        }



        private void NewImageButton_Click(object sender, EventArgs e)
        {
            Color[] allowedColors = { Color.White, Color.Green, Color.Blue, Color.Yellow, Color.Red };
            Random random = new Random();

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    int randomIndex = random.Next(allowedColors.Length);
                    mosaicGrid[i, j] = allowedColors[randomIndex];
                }
            }

            // Очищаем существующие элементы палитры
            List<Control> controlsToRemove = new List<Control>();
            foreach (Control control in Controls)
            {
                if (control is Panel panel && panel.Location.X == 240)
                {
                    controlsToRemove.Add(panel);
                }
            }
            foreach (Control controlToRemove in controlsToRemove)
            {
                Controls.Remove(controlToRemove);
                controlToRemove.Dispose();
            }

            // Создаем новую палитру с цветами, соответствующими новому рисунку
            CreateColorPalette();

            // Сбрасываем все элементы
            ResetAllElements();
        }




        private void CreateClearButton()
        {
            const int offsetX = 30;
            const int offsetY = 260;

            Button button = new Button();
            button.Text = "Очистить";
            button.Location = new Point(offsetX, offsetY);
            button.Size = new Size(100, 30);

            button.Click += ClearButton_Click;

            Controls.Add(button);
        }
        
        private void ClearButton_Click(object sender, EventArgs e)
        {
            // Очистка палитры
            foreach (Control control in Controls)
            {
                if (control is Panel panel && panel.Location.X == 320)
                {
                    panel.BackColor = Color.Gray;
                }
            }

            // Очистка сгенерированной мозайки
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    mosaicGrid[i, j] = Color.White;
                }
            }
            // Очистка серого квадрата в пользовательской сетке
            foreach (Control control in Controls)
            {
                if (control is Panel panel && panel.Location.X >= 320 && panel.Location.X <= 500)
                {
                    panel.BackColor = Color.Gray;
                }
            }

            ResetMosaicGrid();
        }


        private bool CheckMosaicComplete()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (mosaicGrid[i, j] == Color.White)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ResetMosaicGrid()
        {
            // Обновление цветов на мозайке
            foreach (Control control in Controls)
            {
                if (control is Panel panel && panel.Location.X >= 20 && panel.Location.X <= 200)
                {
                    int indexX = (panel.Location.X - 20) / SquareSize;
                    int indexY = (panel.Location.Y - 20) / SquareSize;

                    panel.BackColor = mosaicGrid[indexX, indexY];
                }
            }

            //// Обновление цветов на палитре
            //foreach (Control control in Controls)
            //{
            //    if (control is Panel panel && panel.Location.X == 680)
            //    {
            //        int indexY = (panel.Location.Y - 30) / SquareSize;
            //        panel.BackColor = mosaicGrid[0, indexY];
            //    }
            //}
        }

    }
}
