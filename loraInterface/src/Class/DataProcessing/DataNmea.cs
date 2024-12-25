using Newtonsoft.Json.Linq;
class DataNmea : DataProcessing {
    public string Type { get; set; }
    public List<string> Data { get; set; }
    public DataNmea(string type,List<string> data) {
        pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "nmea_data.json");
        Type = type;
        Data = data;
    }
    public DataNmea(Dictionary<string, dynamic> json) {
        pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "turn_data.json");
        Type = json["type"];
        Data = ((JArray)json["data"])?.ToObject<List<string>>() ?? new List<string>();
    }
    public override Dictionary<string, dynamic> ToJson() {
        return new Dictionary<string, dynamic>
        {
            { "type", Type },
            { "data", Data }
        };
    }
    public override void ProcessData(string message) {
        try {
            List<string> parts = new List<string>(message.Split(','));
            if (parts.Count == 0 || parts[0].Length < 6)
            {
                return;
            }
            string typeCode = parts[0].Substring(1, 5);
            DataNmea newNmeaData = new DataNmea(typeCode, parts);
            List<DataProcessing> existingList = ReadFromFile();
            existingList.Add(newNmeaData);
            WriteToFile(existingList);
        } catch (Exception e) {
            MessageBox.Show($"Ошибка при обработке данных NMEA: {e}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}