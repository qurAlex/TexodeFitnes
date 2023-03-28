using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexodeFitnes.Model
{
    internal class User : INotifyPropertyChanged
    {
        string _name;
        string [] _status;
        int [] _rank;
        int [] _steps;
        int _upperSteps;
        int _lowerSteps;
        int _middleSteps;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
