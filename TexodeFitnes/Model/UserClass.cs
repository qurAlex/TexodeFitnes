using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TexodeFitnes.Model
{
    internal class UserClass : INotifyPropertyChanged
    {
        string _user;
        string[] _status;
        int[] _rank;
        int[] _steps;
        int _upperSteps;
        int _lowerSteps;
        int _middleSteps;

        [JsonIgnore]
        public bool DifSteps
        {
            get
            {
                if (_middleSteps * 0.2 < Math.Abs(_middleSteps - _upperSteps) || _middleSteps * 0.2 < Math.Abs(_middleSteps - _lowerSteps))
                {
                    return true;
                }
                else return false;
            }
        }

        public UserClass()
        {
            _status = new string[30];
            _rank = new int[30];
            _steps = new int[30];
        }

        public UserClass(int count)
        {
            _status = new string[count];
            _rank = new int[count];
            _steps = new int[count];
        }
        public string User
        {
            get { return _user; }
            set { _user = value; OnPropertyChanged("User"); }
        }

        public int[] Rank
        {
            get { return _rank; }
        }

        public string[] Status
        {
            get { return _status; }
        }

        public int[] Steps
        {
            get { return _steps; }
        }

        public int UpperSteps
        {
            get { return _upperSteps; }
        }

        public int LowerSteps
        {
            get { return _lowerSteps; }
        }

        public int MiddleSteps
        {
            get { return _middleSteps; }
        }

        public void AddDay(int day, int rank, string status, int steps)
        {
            _rank[day] = rank;
            _status[day] = status;
            _steps[day] = steps;
            _upperSteps = _steps.Max();
            OnPropertyChanged("UpperSteps");
            _lowerSteps = _steps.Min();
            OnPropertyChanged("LowerSteps");
            _middleSteps = _steps.Sum() / _steps.Count();
            OnPropertyChanged("MiddleSteps");
            OnPropertyChanged("DifSteps");
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
