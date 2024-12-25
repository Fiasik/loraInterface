using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
class DataTurn : DataProcessing {
    public bool RotationStatus { get; private set; }
    public int TurnValue { get; private set; }
    public int ActualTurn { get; private set; }
    public string Data { get; private set; }
    public DataTurn(bool rotationStatus, int turnValue, int actualTurn, string data) {
        pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "turn_data.json");
        RotationStatus = rotationStatus;
        TurnValue = turnValue;
        ActualTurn = actualTurn;
        Data = data;
    }
    public DataTurn(Dictionary<string, dynamic> json) {
        pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "turn_data.json");

        RotationStatus = json["rotationStatus"];
        TurnValue = (int)(long)json["turnValue"];
        ActualTurn = (int)(long)json["actualTurn"];
        Data = json["data"];
    }
    public override Dictionary<string, dynamic> ToJson() {
        return new Dictionary<string, dynamic>
        {
            { "rotationStatus", RotationStatus },
            { "turnValue", TurnValue },
            { "actualTurn", ActualTurn },
            { "data", Data }
        };
    }
    public override void ProcessData(string message) {
    try {
            Regex statusRegex = new Regex(@"RATATION - (ON|OFF)");
            Regex turnRegex = new Regex(@"SET TURN: (\d+)");
            Regex actualTurnRegex = new Regex(@"TURN ACTUAL: (\d+)");

            bool rotationStatus = false;
            int turnValue = 0;
            int actualTurn = 0;
            string timestamp = DateTime.Now.ToString();

            Match statusMatch = statusRegex.Match(message);
            Match turnMatch = turnRegex.Match(message);
            Match actualTurnMatch = actualTurnRegex.Match(message);

            if (statusMatch.Success) {
                rotationStatus = statusMatch.Groups[1].Value == "ON";
            }

            if (turnMatch.Success) {
                turnValue = int.TryParse(turnMatch.Groups[1].Value, out int result) ? result : 0;
            }

            if (actualTurnMatch.Success) {
                actualTurn = int.TryParse(actualTurnMatch.Groups[1].Value, out int result) ? result : 0;
            }

            DataTurn newDataTurn = new DataTurn(rotationStatus, turnValue, actualTurn, timestamp);

            List<DataProcessing> existingList = ReadFromFile();
            existingList.Add(newDataTurn);
            WriteToFile(existingList);        
        } catch (Exception e) {
            MessageBox.Show($"Ошибка при обработке данных о повороте: {e}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}