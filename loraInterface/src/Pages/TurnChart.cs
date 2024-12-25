using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace loraInterface.src.Pages
{
    public partial class TurnChart : UserControl
    {
        private Chart chart; // График для отображения данных
        private System.Windows.Forms.Timer timer; // Таймер для обновления данных
        private Button changeIntervalButton; // Кнопка для смены интервала
        private DateTimeIntervalType currentIntervalType; // Текущий тип интервала

        public TurnChart()
        {
            InitializeChart(); // Инициализация компонентов графика
            LoadData(); // Загрузка начальных данных
            InitializeTimer(); // Инициализация таймера для периодической загрузки данных
            InitializeButton(); // Инициализация кнопки для смены интервала
        }

        // Метод для инициализации компонентов графика
        private void InitializeChart()
        {
            chart = new Chart
            {
                Dock = DockStyle.Fill, // Растягиваем график на всю панель управления
                BackColor = Color.WhiteSmoke // Белый фон графика
            };

            ChartArea chartArea = new ChartArea
            {
                Name = "График Дата/Актуальные вращения",
                AxisX =
                {
                    Title = "Дата", // Название оси X
                    IntervalType = DateTimeIntervalType.Minutes, // Автоматический интервал по дате
                    Interval = 1, // Интервал
                    LabelStyle = { Format = "HH:mm" } // Формат отображения даты
                },
                AxisY = { Title = "Актуальные вращения" } // Название оси Y
            };

            chart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                Name = "Актуальные вращения",
                ChartType = SeriesChartType.Line, // Линейный тип графика
                XValueType = ChartValueType.DateTime, // Тип данных по оси X - дата и время
                YValueType = ChartValueType.Int32 // Тип данных по оси Y - целые числа
            };

            chart.Series.Add(series); // Добавление серии данных на график
            chart.MouseWheel += Chart_MouseWheel; // Подписка на событие MouseWheel для графика
            this.Controls.Add(chart); // Добавление графика на панель управления

            currentIntervalType = DateTimeIntervalType.Minutes; // Изначально устанавливаем интервал на минуты
        }

        // Метод для загрузки данных на график из файла
        private void LoadData()
        {
            DataTurn dataTurnInstance = new DataTurn(false, 0, 0, "");
            List<DataProcessing> processingList = dataTurnInstance.ReadFromFile(); // Чтение данных о вращениях из файла
            List<DataTurn> turnDataList = processingList.OfType<DataTurn>().ToList();

            chart.Series["Актуальные вращения"].Points.Clear(); // Очистка текущих данных на графике перед загрузкой новых

            foreach (var turnData in turnDataList)
            {
                DateTime date;
                int actualTurn = turnData.ActualTurn;

                // Пытаемся распарсить дату и время с учетом нового формата "d.MM.yyyy HH:mm:ss"
                if (DateTime.TryParseExact(turnData.Data, "d.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    chart.Series["Актуальные вращения"].Points.AddXY(date, actualTurn); // Добавляем точку данных на график
                }
                else
                {
                    MessageBox.Show($"Error parsing date: {turnData.Data}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Выводим сообщение об ошибке при парсинге даты
                }
            }

            chart.Invalidate(); // Перерисовываем график для отображения новых данных
        }

        // Метод для инициализации таймера для периодической загрузки данных
        private void InitializeTimer()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // Интервал в миллисекундах (5000 мс = 5 секунд)
            };
            timer.Tick += Timer_Tick; // Устанавливаем обработчик события Tick для таймера
            timer.Start(); // Запускаем таймер
        }

        // Обработчик события Tick таймера для периодической загрузки данных
        private void Timer_Tick(object sender, EventArgs e)
        {
            LoadData(); // Вызываем метод загрузки данных
        }

        // Метод для инициализации кнопки для смены интервала
        private void InitializeButton()
        {
            changeIntervalButton = new Button
            {
                Size = new Size(633, 50),
                Text = "Изменить маштабирование",
                Font = new Font("Montserrat", 10, FontStyle.Regular),
                Dock = DockStyle.Top // Размещаем кнопку в верхней части панели управления
            };
            changeIntervalButton.Click += ChangeIntervalButton_Click; // Устанавливаем обработчик события Click для кнопки
            this.Controls.Add(changeIntervalButton); // Добавление кнопки на панель управления
        }

        // Обработчик события Click кнопки для смены интервала
        private void ChangeIntervalButton_Click(object sender, EventArgs e)
        {
            if (currentIntervalType == DateTimeIntervalType.Minutes)
            {
                currentIntervalType = DateTimeIntervalType.Hours;
                chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours; // Меняем интервал на часы
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm"; // Изменяем формат отображения на часы и минуты
            }
            else
            {
                currentIntervalType = DateTimeIntervalType.Minutes;
                chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes; // Меняем интервал на минуты
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm"; // Изменяем формат отображения на часы и минуты
            }

            chart.Invalidate(); // Перерисовываем график для применения изменений
        }

        // Обработчик события MouseWheel для масштабирования графика
        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                var chartArea = chart.ChartAreas[0];

                if (e.Delta < 0) // Zoom out
                {
                    chartArea.AxisX.ScaleView.ZoomReset();
                    chartArea.AxisY.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Zoom in
                {
                    double xMin = chartArea.AxisX.ScaleView.ViewMinimum;
                    double xMax = chartArea.AxisX.ScaleView.ViewMaximum;
                    double yMin = chartArea.AxisY.ScaleView.ViewMinimum;
                    double yMax = chartArea.AxisY.ScaleView.ViewMaximum;

                    double posXStart = chartArea.AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 3;
                    double posXFinish = chartArea.AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 3;
                    double posYStart = chartArea.AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 3;
                    double posYFinish = chartArea.AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 3;

                    chartArea.AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chartArea.AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
    }
}
