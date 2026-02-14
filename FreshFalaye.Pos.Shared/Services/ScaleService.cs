using System.IO.Ports;
using System.Text.RegularExpressions;

namespace FreshFalaye.Pos.Shared.Services
{

    public class ScaleService : IDisposable
    {
        private SerialPort? _port;
        private readonly object _lock = new();
        private bool _isRunning;

        public event Action<decimal>? WeightReceived;

        public bool IsRunning => _isRunning;

        public void Start(string portName, int baudRate = 9600)
        {
            try
            {
                lock (_lock)
                {
                    if (_isRunning)
                        return; // already running

                    _port = new SerialPort(portName, baudRate)
                    {
                        NewLine = "\r\n",
                        ReadTimeout = 1000
                    };

                    _port.DataReceived += Port_DataReceived;
                    _port.Open();

                    _isRunning = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_port == null || !_port.IsOpen)
                    return;

                string raw = _port.ReadLine();

                var match = Regex.Match(raw, @"\d+(\.\d+)?");

                if (match.Success && decimal.TryParse(match.Value, out var weight))
                {
                    WeightReceived?.Invoke(weight);
                }
            }
            catch (TimeoutException)
            {
                // Ignore read timeout
            }
            catch (Exception ex)
            {
               // Console.WriteLine($"Scale error: {ex.Message}");
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning || _port == null)
                    return;

                try
                {
                    _port.DataReceived -= Port_DataReceived;

                    if (_port.IsOpen)
                        _port.Close();

                    _port.Dispose();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Scale stop error: {ex.Message}");
                }

                _port = null;
                _isRunning = false;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    //public class ScaleService
    //{
    //    private SerialPort _port;
    //    public event Action<decimal>? WeightReceived;
    //    public void Start(string portName, int baudRate = 9600)
    //    {
    //        Stop();
    //        _port = new SerialPort(portName, baudRate); // <- change if needed
    //        _port.NewLine = "\r\n";
    //        _port.DataReceived += Port_DataReceived;
    //        _port.Open();
    //    }
    //    private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    //    {
    //        try
    //        {
    //            string raw = _port.ReadLine();

    //            // extract number like 1.250
    //            var match = Regex.Match(raw, @"\d+(\.\d+)?");

    //            if (match.Success)
    //            {
    //                decimal weight = decimal.Parse(match.Value);
    //                WeightReceived?.Invoke(weight);
    //            }
    //        }
    //        catch { }
    //    }
    //    public void Stop()
    //    {
    //        if (_port?.IsOpen == true)
    //            _port.Close();
    //    }
    //}
}
