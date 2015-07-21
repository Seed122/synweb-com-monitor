using System;
using System.Configuration;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace SynWeb.PhobiaInfrastructure.COMPort
{
    public class COMPort:IDisposable
    {
        private string _portName;
        private bool _working;
        private bool _heartbeatEnabled;
        private string _heartbeatSign;
        private int _heartbeatTimeout;
        private DateTime _lastHeartbeat;

        private System.IO.Ports.SerialPort rs_port;

        public string PortName
        {
            get { return _portName; }
            private set { _portName = value; }
        }

        public COMPort(string portName)
        {
            _portName = portName;
            InitRs();
            
        }

        private void InitRs()
        {
            if (rs_port != null)
            {
                rs_port.DataReceived -= rs_port_DataReceived;
                rs_port.ErrorReceived -= rs_port_ErrorReceived;

                if (rs_port.IsOpen)
                {
                    rs_port.Close();
                }
                rs_port.Dispose();
            }

            rs_port = new System.IO.Ports.SerialPort(_portName, 9600,
                System.IO.Ports.Parity.None,
                8,
                System.IO.Ports.StopBits.One);



            rs_port.Open();

            _lastHeartbeat = DateTime.Now;
            
        }

        public void StopListening()
        {
            _working = false;
        }

        public void StartListening()
        {
            rs_port.DataReceived += rs_port_DataReceived;
            //rs_port.ErrorReceived += rs_port_ErrorReceived;
        }


        void rs_port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }

        private void rs_port_DataReceived(object sender, SerialDataReceivedEventArgs ea)
        {
            if (rs_port.BytesToRead > 0)
            {
                //byte[] answer = new byte[(int) rs_port.BytesToRead];
                string msg = String.Empty;
                //try
                //{
                //    //  Читаем буфер для анализа ответа на команду управления
                //    rs_port.Read(answer, 0, rs_port.BytesToRead);
                //    msg = System.Text.Encoding.ASCII.GetString(answer);
                //}
                try
                {
                    msg = rs_port.ReadLine();
                }
                catch (Exception e)
                {
                    ReInit();
                }
                if (!String.IsNullOrEmpty(msg))
                {
                    MessageReceived(msg);
                }
            }
        }

        private void ReInit()
        {
            bool reinited = false;
            try
            {
                InitRs();
                StartListening();
                reinited = true;
            }
            catch(Exception e)
            {
            }

            if (!reinited)
            {
                Thread.Sleep(5000);
                ReInit();
            }
        }
        

        public void SendCom(string msg)
        {
            try
            {
                rs_port.Write(msg);
            }
            catch (Exception e)
            {
                ReInit();
            }
        }

        public event Action<string> MessageReceived = (msg) => { };
        public void Dispose()
        {
            rs_port.Dispose();
        }
    }
}
