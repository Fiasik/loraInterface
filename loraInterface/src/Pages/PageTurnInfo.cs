
using loraInterface.src.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace loraInterface.src.Admin
{
    public partial class PageTurnInfo : UserControl
    {
        private System.Windows.Forms.Timer timer;

        public PageTurnInfo()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000; // Обновление каждые 5 секунд (5000 миллисекунд)
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DisplayLastData(); // Обновляем данные при срабатывании таймера
        }
        private void DisplayLastData()
        {
            try
            {
                DataTurn dataTurnInstance = new DataTurn(false, 0, 0, "");
                List<DataProcessing> turnDataList = dataTurnInstance.ReadFromFile();

                if (turnDataList.Count > 0)
                {
                    // Получаем последние данные
                    DataTurn lastTurnData = turnDataList[turnDataList.Count - 1] as DataTurn;

                    // Обновляем свойства элементов CellParamsInfo
                    cellParamsInfoState.labelParamText = "Состояние:";
                    cellParamsInfoState.labelValueText = lastTurnData.RotationStatus ? "ON" : "OFF";

                    cellParamsInfoTurn.labelParamText = "Заданные обороты:";
                    cellParamsInfoTurn.labelValueText = lastTurnData.TurnValue.ToString();

                    cellParamsInfoActualTurn.labelParamText = "Актуальные обороты:";
                    cellParamsInfoActualTurn.labelValueText = lastTurnData.ActualTurn.ToString();
                }
                else
                {
                    // Если данных нет, показываем пустые значения
                    //MessageBox.Show("Данных нет, файл пустой.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cellParamsInfoState.labelParamText = "Состояние:";
                    cellParamsInfoState.labelValueText = "null";

                    cellParamsInfoTurn.labelParamText = "Заданные обороты:";
                    cellParamsInfoTurn.labelValueText = "null";

                    cellParamsInfoActualTurn.labelParamText = "Актуальные обороты:";
                    cellParamsInfoActualTurn.labelValueText = "null";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data from JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PageTurnInfo_Load(object sender, EventArgs e)
        {
            DisplayLastData();
        }
    }
}