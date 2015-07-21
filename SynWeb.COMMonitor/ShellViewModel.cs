using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SynWeb.COMMonitor
{
    public class ShellViewModel : Caliburn.Micro.ViewAware, IShell
    {
        public ShellViewModel()
        {
            ((App)Application.Current).COMMessageReceived += COMMessageReceived;
            ((App)Application.Current).COMMessageSent += COMMessageSent;
            ((App)Application.Current).COMConnected += COMConnected;
            ((App)Application.Current).COMDisconnected += COMDisconnected;
        }

        private void COMDisconnected(string name)
        {
            WriteToHistory("Disconnected from " + name);
        }

        private void COMConnected(string name)
        {
            WriteToHistory("Connected to " + name);
        }

        private void COMMessageReceived(string msg)
        {
            var msgTrimmed = msg.Replace("\r", "").Replace("\n", "");
            WriteToHistory("→ " + msgTrimmed);
            if (!string.IsNullOrEmpty(ExpectedString))
            {
                if (msgTrimmed.Equals(ExpectedString))
                {
                    if (!string.IsNullOrEmpty(AnswerIfEquals))
                    {
                        ((App) Application.Current).SendToCOMPort(AnswerIfEquals);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AnswerIfDiffers))
                    {
                        ((App) Application.Current).SendToCOMPort(AnswerIfDiffers);
                    }
                }
            }
        }

        private void COMMessageSent(string msg)
        {
            WriteToHistory("← " + msg);
        }

        private void WriteToHistory(string msg)
        {
            History += "\r\n" + msg;
        }

        private string _selectedPort;
        private string _history;
        private string _messageToSend;
        private string _expectedString;
        private string _answerIfEquals;
        private string _answerIfDiffers;

        public IEnumerable<string> PortsList
        {
            get { return Enumerable.Range(1, 18).Select(x => "COM" + x); }
        }

        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                if (value == _selectedPort) return;
                _selectedPort = value;
                NotifyOfPropertyChange(() => SelectedPort);
            }
        }

        public string History
        {
            get { return _history; }
            set
            {
                if (value == _history) return;
                _history = value;
                NotifyOfPropertyChange(() => History);
            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                if (value == _messageToSend) return;
                _messageToSend = value;
                NotifyOfPropertyChange(() => MessageToSend);
            }
        }

        public void ConnectToCOMPort()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedPort))
                {
                    ((App) Application.Current).ConnectToCOMPort(SelectedPort);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при подключении");
            }
        }

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(MessageToSend))
            {
                ((App) Application.Current).SendToCOMPort(MessageToSend);
                MessageToSend = String.Empty;
            }
        }

        public void HistoryTextChanged(TextBox item)
        {
            item.ScrollToEnd();
        }

        public void MsgKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SendMessage();
                    break;
            }
        }


        public string ExpectedString
        {
            get { return _expectedString; }
            set
            {
                if (value == _expectedString) return;
                _expectedString = value;
                NotifyOfPropertyChange(() => ExpectedString);
            }
        }

        public string AnswerIfEquals
        {
            get { return _answerIfEquals; }
            set
            {
                if (value == _answerIfEquals) return;
                _answerIfEquals = value;
                NotifyOfPropertyChange(() => AnswerIfEquals);
            }
        }

        public string AnswerIfDiffers
        {
            get { return _answerIfDiffers; }
            set
            {
                if (value == _answerIfDiffers) return;
                _answerIfDiffers = value;
                NotifyOfPropertyChange(() => AnswerIfDiffers);
            }
        }
    }
}