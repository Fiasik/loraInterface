interface IDataProcessing {
    public abstract Dictionary<string, dynamic> ToJson();
    public abstract List<DataProcessing> ReadFromFile();
    public abstract void WriteToFile(List<DataProcessing> dataList);
    public void ProcessData(string message);
}