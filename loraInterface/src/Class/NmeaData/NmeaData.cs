using Newtonsoft.Json.Linq;

public class NmeaData
{
    public string Type { get; set; }
    public List<string> Data { get; set; }

    public NmeaData(string type,List<string> data)
    {
        Type = type;
        Data = data;
    }

    public NmeaData(Dictionary<string, dynamic> json)
    {
        Type = json["type"];
        Data = ((JArray)json["data"]).ToObject<List<string>>();
    }

    public Dictionary<string, dynamic> ToJson()
    {
        return new Dictionary<string, dynamic>
        {
            { "type", Type },
            { "data", Data }
        };
    }
    public void ProcessNmeaData(string message)
    {
        try
        {
            List<string> parts = new List<string>(message.Split(','));
            if (parts.Count == 0 || parts[0].Length < 6)
            {
                return;
            }

            string typeCode = parts[0].Substring(1, 5);
            NmeaData newNmeaData = new NmeaData(typeCode, parts);

            List<NmeaData> existingNmeaDataList = NmeaData.ReadNmeaDataFromFile();
            existingNmeaDataList.Add(newNmeaData);
            NmeaData.WriteNmeaDataToFile(ref existingNmeaDataList);
            //MessageBox.Show(newNmeaData.ToString(), "Новые данные NMEA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка при обработке данных NMEA: {e}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    public static List<NmeaData> ReadNmeaDataFromFile()
    {
        // string currentDirectory = Directory.GetCurrentDirectory();
        //const string pathFile = "D:\\Tree\\I\\library\\Git\\loraInterface\\loraInterface\\src\\Class\\NmeaData\\nmea_data.json";
        const string pathFile = "D:\\library\\git_hub\\loraInterface\\loraInterface\\src\\Class\\NmeaData\\nmea_data.json";
        try
        {
            if (!File.Exists(pathFile))
                return new List<NmeaData>();

            string contents = File.ReadAllText(pathFile);
            if (string.IsNullOrEmpty(contents))
                return new List<NmeaData>();

            List<Dictionary<string, dynamic>> jsonDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(contents);
            List<NmeaData> nmeaDataList = new List<NmeaData>();
            foreach (var json in jsonDataList)
            {
                nmeaDataList.Add(new NmeaData(json));
            }
            return nmeaDataList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file: {e}");
            return new List<NmeaData>();
        }
    }

    public static void WriteNmeaDataToFile(ref List<NmeaData> nmeaDataList)
    {
        // string currentDirectory = Directory.GetCurrentDirectory();
        // const string pathFile = "D:\\Tree\\I\\library\\Git\\loraInterface\\loraInterface\\src\\Class\\NmeaData\\nmea_data.json";
        const string pathFile = "D:\\library\\git_hub\\loraInterface\\loraInterface\\src\\Class\\NmeaData\\nmea_data.json";

        try
        {
            List<Dictionary<string, dynamic>> jsonDataList = new List<Dictionary<string, dynamic>>();
            foreach (var nmeaData in nmeaDataList)
            {
                jsonDataList.Add(nmeaData.ToJson());
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