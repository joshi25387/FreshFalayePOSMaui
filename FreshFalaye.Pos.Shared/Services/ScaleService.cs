using System.IO.Ports;
using System.Text.RegularExpressions;

namespace FreshFalaye.Pos.Shared.Services
{
    public class ScaleService
    {
        private SerialPort _port;
        public event Action<decimal>? WeightReceived;
        public void Start(string portName, int baudRate = 9600)
        {
            Stop();
            _port = new SerialPort(portName, baudRate); // <- change if needed
            _port.NewLine = "\r\n";
            _port.DataReceived += Port_DataReceived;
            _port.Open();
        }
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string raw = _port.ReadLine();

                // extract number like 1.250
                var match = Regex.Match(raw, @"\d+(\.\d+)?");

                if (match.Success)
                {
                    decimal weight = decimal.Parse(match.Value);
                    WeightReceived?.Invoke(weight);
                }
            }
            catch { }
        }
        public void Stop()
        {
            if (_port?.IsOpen == true)
                _port.Close();
        }
    }
}
