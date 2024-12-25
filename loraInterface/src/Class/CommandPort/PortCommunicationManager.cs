public class PortCommunicationManager
{
    private SerialPortManager _portManager;
    //private DataHandler _dataHandler;

    public PortCommunicationManager()
    {
        _portManager = new SerialPortManager();
        //_dataHandler = new DataHandler();
    }

    public void AddPort(string portName)
    {
        _portManager.AddPort(portName);
    }

    public void OpenPorts()
    {
        _portManager.OpenPorts();
    }

    public void ClosePorts()
    {
        _portManager.ClosePorts();
    }

    public void ReadAndProcessDataFromPort(int portIndex)
    {
        string data = _portManager.ReadFromPort(portIndex);
        if (!string.IsNullOrEmpty(data))
        {
            //_dataHandler.ProcessReceivedData(data);
            //_dataHandler.LogData(data);

        }
    }

    public void SendDataToPort(int portIndex, string data)
    {
        _portManager.WriteToPort(portIndex, data);
    }
}
