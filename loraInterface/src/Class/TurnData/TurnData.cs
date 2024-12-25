    using System.Text.RegularExpressions;

    public class TurnData
    {
        public bool RotationStatus { get; set; }
        public int TurnValue { get; set; }
        public int ActualTurn { get; set; }
        public string Data { get; set; }
        public TurnData(bool rotationStatus, int turnValue, int actualTurn, string data)
        {
            RotationStatus = rotationStatus;
            TurnValue = turnValue;
            ActualTurn = actualTurn;
            Data = data;
        }

        public TurnData(Dictionary<string, dynamic> json)
        {
            RotationStatus = json["rotationStatus"];
            TurnValue = (int)(long)json["turnValue"]; // Явное преобразование из long в int
            ActualTurn = (int)(long)json["actualTurn"]; // Явное преобразование из long в int
            Data = json["data"];
        }

        public Dictionary<string, dynamic> ToJson()
        {
            return new Dictionary<string, dynamic>
            {
                { "rotationStatus", RotationStatus },
                { "turnValue", TurnValue },
                { "actualTurn", ActualTurn },
                { "data", Data }
            };
        }

        public void ProcessTurnData(string message)
        {
            try
            {
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

                if (statusMatch.Success)
                {
                    rotationStatus = statusMatch.Groups[1].Value == "ON";
                }

                if (turnMatch.Success)
                {
                    turnValue = int.TryParse(turnMatch.Groups[1].Value, out int result) ? result : 0;
                }

                if (actualTurnMatch.Success)
                {
                    actualTurn = int.TryParse(actualTurnMatch.Groups[1].Value, out int result) ? result : 0;
                }

                // Передаем аргументы в конструктор TurnData
                TurnData newTurnData = new TurnData(rotationStatus, turnValue, actualTurn, timestamp);

                List<TurnData> existingTurnDataList = TurnData.ReadTurnDataFromFile();
                existingTurnDataList.Add(newTurnData);

                WriteTurnDataToFile(existingTurnDataList);        
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при обработке данных о повороте: {e}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static List<TurnData> ReadTurnDataFromFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            // const string pathFile = "D:\\Tree\\I\\library\\Git\\loraInterface\\loraInterface\\src\\Class\\TurnData\\turn_data.json";
            const string pathFile = "D:\\library\\git_hub\\loraInterface\\loraInterface\\src\\Class\\TurnData\\turn_data.json";
            try
            {
                if (!File.Exists(pathFile))
                    return new List<TurnData>();

                string contents = File.ReadAllText(pathFile);
                if (string.IsNullOrEmpty(contents))
                    return new List<TurnData>();

                List<Dictionary<string, dynamic>> jsonDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(contents);
                List<TurnData> turnDataList = new List<TurnData>();
                foreach (var json in jsonDataList)
                {
                    turnDataList.Add(new TurnData(json));
                }
                return turnDataList;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading file: {e}");
                return new List<TurnData>();
            }
        }
        public static void WriteTurnDataToFile(List<TurnData> turnDataList)
        {
            // string pathFile = Path.Combine("D:\\Tree\\I\\library\\Git\\loraInterface\\loraInterface\\src\\Class\\TurnData\\turn_data.json");
            const string pathFile = "D:\\library\\git_hub\\loraInterface\\loraInterface\\src\\Class\\TurnData\\turn_data.json";

            try
            {
                List<Dictionary<string, dynamic>> jsonDataList = new List<Dictionary<string, dynamic>>();
                foreach (var turnData in turnDataList)
                {
                    jsonDataList.Add(turnData.ToJson());
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonDataList, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(pathFile, jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writing file: {e}");
            }
        }
    }
