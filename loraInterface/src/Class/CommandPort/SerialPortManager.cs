using System;
using System.Collections.Generic;
using System.IO.Ports;

public class SerialPortManager
{
    private List<SerialPort> _ports;

    public SerialPortManager()
    {
        _ports = new List<SerialPort>();
    }

    public void AddPort(string portName)
    {
        var port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        _ports.Add(port);
    }

    public void OpenPorts()
    {
        foreach (var port in _ports)
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                    Console.WriteLine($"Порт {port.PortName} подключен.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Не удалось открыть порт {port.PortName}.");
            }
        }
    }

    public void ClosePorts()
    {
        foreach (var port in _ports)
        {
            if (port.IsOpen)
            {
                port.Close();
                Console.WriteLine($"Порт {port.PortName} отключен.");
            }
        }
    }

    public string ReadFromPort(int index)
    {
        if (index >= 0 && index < _ports.Count)
        {
            return ReadFromPort(_ports[index]);
            
        }
        return string.Empty;
    }

    public void WriteToPort(int index, string data)
    {
        if (index >= 0 && index < _ports.Count)
        {
            WriteToPort(_ports[index], data);
        }
    }

    private string ReadFromPort(SerialPort port)
    {
        if (port.IsOpen)
        {
            try
            {
                return port.ReadLine().Trim();
            }
            catch (TimeoutException)
            {
                return string.Empty;
            }
        }
        return string.Empty;
    }

    private void WriteToPort(SerialPort port, string data)
    {
        if (port.IsOpen)
        {
            try
            {
                port.WriteLine(data);
            }
            catch (Exception)
            {
                // Ошибка отправки данных
            }
        }
    }
}
