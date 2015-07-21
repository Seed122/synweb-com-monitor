using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SynWeb.PhobiaInfrastructure.COMPort;

namespace SynWeb.COMMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        private SynWeb.PhobiaInfrastructure.COMPort.COMPort _comPort;

        public void ConnectToCOMPort(string port)
        {
            if (_comPort != null)
            {
                var name = _comPort.PortName;
                _comPort.Dispose();
                _comPort = null;
                COMDisconnected(name);
            }
            _comPort = new COMPort(port);
            _comPort.MessageReceived += COMMessageReceived;
            _comPort.StartListening();
            COMConnected(port);
        }

        public event Action<string> COMMessageReceived = x => { };
        public event Action<string> COMMessageSent = x => { };
        public event Action<string> COMDisconnected = x => { };
        public event Action<string> COMConnected = x => { };
        public void SendToCOMPort(string msg)
        {
            if (_comPort != null && !string.IsNullOrEmpty(msg))
            {
                if (!msg.EndsWith("\r\n"))
                {
                    msg = msg + "\r\n";
                }
                _comPort.SendCom(msg);
                COMMessageSent(msg.Trim());
            }
        }
    }
}
