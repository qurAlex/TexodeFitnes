using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TexodeFitnes.Model;

namespace TexodeFitnes.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Dictionary<string,Users> _users = new Dictionary<string,Users>();

        public Dictionary<string, Users> Users
        {
            get { return _users; }
        }
        public MainViewModel()
        {
            JSONread();
        }
        
        public void JSONread()
        {
            try
            {
                for (int i = 0; i < 30; i++)
                {
                    string fileName = $"../../../TestData/day{i + 1}.json";
                    string jsonString = File.ReadAllText(fileName);
                    JsonNode forecastNode = JsonNode.Parse(jsonString)!;

                    for (int j = 0; j < forecastNode.AsArray().Count; j++)
                    {
                        
                        int rankNode = forecastNode![j]["Rank"]!.GetValue<int>();
                        string userNode = forecastNode![j]["User"]!.GetValue<string>();
                        string statusNode = forecastNode![j]["Status"]!.GetValue<string>();
                        int stepsNode = forecastNode![j]["Steps"]!.GetValue<int>();

                        if (!_users.ContainsKey(userNode))
                        {
                            _users.Add(userNode, new Users());
                            _users[userNode].User = userNode;
                        }
                        _users[userNode].AddDay(i, rankNode, statusNode, stepsNode);
                    }
                }
            }
            catch
            {
                _messageError = "произошла ошибка чтения JSON файла";
                OnPropertyChanged("MessageEror");
            }
        }



        string _messageError;
        public string MessageError
        {
            get { return _messageError; }
        }

        bool _stepsChart;

        public bool StepsChart
        {
            get { return _stepsChart; }
            set 
            {
                _stepsChart = value;
                OnPropertyChanged("StepsChart");
            }
        }
        public void Chart()
        {

            this.MyModel = new PlotModel { Title = "Шаги" };


            if (_stepsChart)
            {

                var line1 = new OxyPlot.Series.LineSeries()
                {
                    Color = OxyPlot.OxyColors.Blue,
                    StrokeThickness = 1,
                    InterpolationAlgorithm = InterpolationAlgorithms.UniformCatmullRomSpline,
                    MarkerType = MarkerType.Circle,
                };
                var upperStepsLine= new OxyPlot.Series.LineSeries()
                {
                    Color = OxyColors.Transparent,
                    MarkerFill = OxyPlot.OxyColors.Green,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 5
                };
                var lowerStepsLine = new OxyPlot.Series.LineSeries()
                {
                    Color = OxyColors.Transparent,
                    MarkerFill = OxyPlot.OxyColors.Red,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 5
                };

                for (int i = 0; i < _selectedUser.Steps.Length; i++)
                {
                    line1.Points.Add(new OxyPlot.DataPoint(i + 1, _selectedUser.Steps[i]));
                    if (_selectedUser.UpperSteps == _selectedUser.Steps[i])  upperStepsLine.Points.Add(new OxyPlot.DataPoint(i + 1, _selectedUser.Steps[i]));;
                    if (_selectedUser.LowerSteps == _selectedUser.Steps[i])  lowerStepsLine.Points.Add(new OxyPlot.DataPoint(i + 1, _selectedUser.Steps[i]));;
                }
                
                this.MyModel.Series.Add(line1);
                this.MyModel.Series.Add(upperStepsLine);
                this.MyModel.Series.Add(lowerStepsLine);

            }
            else
            {
                this.MyModel.Axes.Add(new LinearAxis());
                double x = 0.0;
                for (int i = 0; i < _selectedUser.Steps.Length; i++)
                {
                    var histogramSeries = new HistogramSeries()
                    {
                        StrokeThickness = 1,
                        FillColor = OxyColors.LightBlue,
                        StrokeColor = OxyColors.LightCyan
                    };
                    if (_selectedUser.UpperSteps == _selectedUser.Steps[i]) histogramSeries.FillColor = OxyColors.LightGreen;
                    if (_selectedUser.LowerSteps == _selectedUser.Steps[i]) histogramSeries.FillColor = OxyColors.Orange;
                    histogramSeries.Items.Add(new HistogramItem(x + 0.0, x + 1, _selectedUser.Steps[i], 1));
                    MyModel.Series.Add(histogramSeries);

                    x += 1;
                }
            }
            OnPropertyChanged("MyModel");
        }
        public PlotModel MyModel { get; private set; }


        private Users _selectedUser;
        public Users SelectedUser
        {
            get
            {
                return this._selectedUser;
            }

            set
            {
                this._selectedUser = value;
                OnPropertyChanged("SelectedUser");
                Chart();
            }
        }

    }
}
