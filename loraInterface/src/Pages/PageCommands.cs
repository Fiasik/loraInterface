using Timer = System.Windows.Forms.Timer;

namespace loraInterface.src.Admin
{
    public partial class PageCommands : UserControl
    {
        private PortManagement portManagement;
        public PageCommands(PortManagement portManagement)
        {
            InitializeComponent();
            this.portManagement = portManagement;
            LoadData(); // Загрузка данных при инициализации
        }

        private void LoadData()
        {
            DataTurn dataTurnInstance = new DataTurn(false, 0, 0, "");  
            List<DataProcessing> dataTurnList = dataTurnInstance.ReadFromFile();
            if (dataTurnList.Count > 0)
            {
                DataTurn lastTurnData = dataTurnList[dataTurnList.Count - 1] as DataTurn;
                turnTrackBar1.Value = lastTurnData.TurnValue; // Установка начального значения
            }
        }


        private void buttonComandOff_Click(object sender, EventArgs e)
        {
            portManagement.SetCommand("/CMD/SET/RATATION/OFF/\r");
        }

        private void buttonComandOn_Click(object sender, EventArgs e)
        {
            portManagement.SetCommand("/CMD/SET/RATATION/ON/\r");
        }

        private void buttonComandTurn_Click(object sender, EventArgs e)
        {
            portManagement.SetCommand($"/CMD/SET/TURN/{turnTrackBar1.Value}/\r");
        }

        private void buttonComandReset_Click(object sender, EventArgs e)
        {
            portManagement.SetCommand("/CMD/SET/RESET/\r");
        }

        private void buttonComand5_Click(object sender, EventArgs e)
        {
            portManagement.SetCommand("/CMD/SET/REPORTRERUN/\r");
        }
    }
}
