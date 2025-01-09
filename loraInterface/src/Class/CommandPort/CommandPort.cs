using System.IO.Ports;
using System.Text.RegularExpressions;

public class PortManagement
{
    private SerialPort serialPortFirst;
    private SerialPort serialPortSecond;
    private string port_first;
    private string port_second; // ffffffff
    public string port_first_open = "Не подключен";
    public string port_second_open = "Не подключен";
    private object lockObject = new object();
    private bool sendData = false;
    private string command = "";
    public string sendDataState = "";

    public PortManagement()
    {
        // Порты
        port_first = "COM10";
        port_second = "COM12";

        //Настройки портов
        int baudRate = 115200;
        Parity parity = Parity.None;
        int dataBits = 8;
        StopBits stopBits = StopBits.One;

        serialPortFirst = new SerialPort(port_first, baudRate, parity, dataBits, stopBits);
        serialPortSecond = new SerialPort(port_second, baudRate, parity, dataBits, stopBits);
        
    }
    public void OpenPorts()
    {
        try
        {
            if (serialPortFirst != null && !serialPortFirst.IsOpen)
            {
                serialPortFirst.Open();
                port_first_open = "Подключен";
            }
        }
        catch (Exception)
        {
            port_first_open = "Не подключен";
        }
    }
    public void ClosePorts()
    {
        if (serialPortFirst.IsOpen)
        {
            serialPortFirst.Close();
            port_first_open = "Не подключен";
        }

        if (serialPortSecond.IsOpen)
        {
            serialPortSecond.Close();
            port_second_open = "Не подключен";
        }
    }
    public void ReadData()
    {
        try
        {
            if (serialPortFirst.IsOpen)
            {
                lock (lockObject)
                {
                    if (sendData)
                    {
                        Thread.Sleep(1000);
                        SendCommand(command);
                        sendData = false;
                    }
                }

                string dataFirst = serialPortFirst.ReadLine().Trim();
                DataProcess(dataFirst);
                LogDataToFile(dataFirst);
            }
        }
        catch (TimeoutException)
        {
            MessageBox.Show("Тайм-аут при чтении из порта.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка при чтении из порта: {e.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void DataProcess(string data)
    {
        if (data == "NOT SEND")
        {
            sendDataState = "Данные не получены! Повторная отправка...";
            Thread.Sleep(5000);
            SendCommand(command);
        }
        else if (Regex.IsMatch(data, @"^Receive .+ rssi dbm : -\d+ signal rssi dbm : -\d+ snr db : \d+$"))
        {
            sendDataState = "Данные получены!";
            Thread.Sleep(5000);
            sendDataState = "";
        }

        UpdateFromMessage(data);
    }
    private void LogDataToFile(string data)
    {
        try
        {
            //const string pathFile = "C:\\Tree\\programming\\GitHub\\loraInterface\\loraInterface\\src\\Class\\CommandPort\\received_data.txt";
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "received_data.txt");
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {data}");
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка при записи данных в файл: {e.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    public void SendCommand(string command)
    {
        try
        {
            if (serialPortFirst.IsOpen)
            {
                sendDataState = "Отправка...";
                serialPortFirst.WriteLine(command);
                sendDataState = "Обработка...";
            }
            else
            {
                MessageBox.Show("Порт не открыт.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка при отправке команды '{command}': {e.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void SetCommand(string newCommand)
    {
        lock (lockObject)
        {
            command = newCommand;
            sendData = true;
        }
    }

    public void UpdateFromMessage(string message)
    {
        try
        {
            List<string> messages = new List<string>(message.Split('\n'));

            foreach (string msg in messages)
            {
                // Tuple<bool, int, int, string> DataParams;
                if (msg.Contains("//RATATION"))
                {
                    DataTurn dataTurn = new DataTurn(false, 0, 0, "");
                    dataTurn.ProcessData(msg);
                }
                else if (msg.StartsWith("$"))
                {
                    DataNmea dataNmea = new DataNmea("", new List<string>());
                    dataNmea.ProcessData(msg);
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка при обновлении данных из сообщения: {e}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public bool IsPortFirstOpen()
    {
        return serialPortFirst != null && serialPortFirst.IsOpen;
    }

    public bool IsPortSecondOpen()
    {
        return serialPortSecond != null && serialPortSecond.IsOpen;
    }

    public void TryReconnectPorts()
    {
        if (!IsPortFirstOpen())
        {
            try
            {
                serialPortFirst.Open();
                port_first_open = "Подключен";
            }
            catch (Exception)
            {
                port_first_open = "Не подключен";
            }
        }

        if (!IsPortSecondOpen())
        {
            try
            {
                serialPortSecond.Open();
                port_second_open = "Подключен";
            }
            catch (Exception)
            {
                port_second_open = "Не подключен";
            }
        }
    }
}
