using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using SyncPaintBoard.Annotations;

namespace SyncPaintBoard
{
    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly Random _random = new Random();
        private string _connectionError;
        private string _ipAddress = GetLocalIpAddress();
        private string _port = "5049";

        public MainViewModel()
        {
            Objects = new MTObservableCollection<INotifyPropertyChanged>();
        }

        public MTObservableCollection<INotifyPropertyChanged> Objects { get; set; }

        public void CreateNewSimpleObject(Point location)
        {
            var brush = string.Format("#{0:X}{1:X}{2:X}{3:X}", _random.Next(125, 255), _random.Next(255), _random.Next(255), _random.Next(255));
            var width = _random.Next(20, 40);

            Objects.Add(new SimpleObject
            {
                Top = (int)location.Y - width/2,
                Left = (int)location.X - width/2,
                Brush = brush,
                BorderBrush = brush,
                Width = width
            });
        }

        public void RemoveSimpleObject(SimpleObject simpleObject)
        {
            Objects.Remove(simpleObject);
        }

        protected static string GetLocalIpAddress()
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork)
                    continue;
                return ip.ToString();
            }

            return "127.0.0.1";
        }

        public string ConnectionError
        {
            get
            {
                return _connectionError;
            }
            set
            {
                if (value == _connectionError) return;
                _connectionError = value;
                OnPropertyChanged();
            }
        }

        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                if (value == _ipAddress) return;
                _ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (value == _port) return;
                _port = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "IpAddress":
                        if (!Regex.IsMatch(IpAddress ?? "", @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"))
                            return "Ip-адрес должен иметь вид 123.123.123.123";
                        break;
                    case "Port":
                        if (!Regex.IsMatch(Port ?? "", @"^\d{1,5}$"))
                            return "Port должен иметь вид 12345";
                        break;
                }
                return string.Empty;
            }
        }

        public string Error
        {
            get;
        }
    }
}
    