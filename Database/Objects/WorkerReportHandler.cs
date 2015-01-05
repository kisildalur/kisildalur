using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Database
{
    public class WorkerReportHandler : INotifyPropertyChanged
    {
        public WorkerReportHandler(string status, int value, int maxValue)
        {
            _status = status;
            _value = value;
            _maxValue = maxValue;
        }

        string _status;
        int _value;
        int _maxValue;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Get or set the status in the status bar
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Status"));
            }
        }

        /// <summary>
        /// Get or set the value of progress bar in status bar
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// Get or set the max value for the progress bar in status window
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxValue"));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

    }
}
