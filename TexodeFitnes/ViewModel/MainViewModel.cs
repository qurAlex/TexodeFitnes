using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TexodeFitnes.Model;
using static System.Net.Mime.MediaTypeNames;

namespace TexodeFitnes.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Dictionary<string,UserClass> _users = new Dictionary<string,UserClass>();

        public Dictionary<string, UserClass> Users
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
                            _users.Add(userNode, new UserClass());
                            _users[userNode].User = userNode;
                        }
                        _users[userNode].AddDay(i, rankNode, statusNode, stepsNode);
                        if (_users[userNode].MiddleSteps * 0.2 > Math.Abs(_users[userNode].MiddleSteps - _users[userNode].UpperSteps))
                        {
                            OnPropertyChanged("DifSteps");
                        }

                    }
                }
            }
            catch
            {
                _messageError = "произошла ошибка чтения JSON файла";
                OnPropertyChanged("MessageEror");
            }
        }

        public void JSONread(string[] fileNames)
        {
            try
            {
                _users.Clear();
                SelectedUser = null;
                OnPropertyChanged("Users.Value");

                for (int i = 0; i < fileNames.Length; i++)
                {
                    string jsonString = File.ReadAllText(fileNames[i]);
                    JsonNode forecastNode = JsonNode.Parse(jsonString)!;
                    for (int j = 0; j < forecastNode.AsArray().Count; j++)
                    {

                        int rankNode = forecastNode![j]["Rank"]!.GetValue<int>();
                        string userNode = forecastNode![j]["User"]!.GetValue<string>();
                        string statusNode = forecastNode![j]["Status"]!.GetValue<string>();
                        int stepsNode = forecastNode![j]["Steps"]!.GetValue<int>();

                        if (!_users.ContainsKey(userNode))
                        {
                            _users.Add(userNode, new UserClass(fileNames.Length));
                            _users[userNode].User = userNode;
                        }
                        _users[userNode].AddDay(i, rankNode, statusNode, stepsNode);
                        if (_users[userNode].MiddleSteps * 0.2 > Math.Abs(_users[userNode].MiddleSteps - _users[userNode].UpperSteps))
                        {
                            OnPropertyChanged("DifSteps");
                        }

                    }
                    
                }
            }
            catch
            {
                _messageError = "произошла ошибка чтения JSON файла";
                OnPropertyChanged("MessageEror");
            }
        }

        ICommand _saveFileCommand;
        public ICommand SaveFileCommand 
        {
            get
            {
                if (_saveFileCommand == null)
                    _saveFileCommand = new RelayCommand(o => SaveFile());
                return _saveFileCommand;
            }
        }

        void SaveFile() 
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON файлы(.json) | *.json";
            dialog.FileName = SelectedUser.User;
            dialog.DefaultExt = ".json";
            var result = dialog.ShowDialog();
            if (result == true)
            {
                string file = dialog.FileName;
                JSONwrite(file);
            }
        }

        public bool SaveEnabled { get; private set; }

        void JSONwrite(string file)
        {
            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic), WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(SelectedUser, options);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(jsonString);
            writer.Close();
        }
        
        ICommand _openFileCommand;
        public ICommand OpenFileCommand 
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new RelayCommand(o => OpenFile());
                return _openFileCommand;
            }
        }

        void OpenFile() 
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON(.json)|*.json";
            dialog.DefaultExt = ".json";
            dialog.Multiselect = true;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                string[] file = dialog.FileNames;
                JSONread(file);
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
                Chart();
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
                    MarkerType = MarkerType.Circle
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


        private UserClass _selectedUser;
        public UserClass SelectedUser
        {
            get
            {
                return this._selectedUser;
            }

            set
            {
                this._selectedUser = value;
                OnPropertyChanged("SelectedUser");
                SaveEnabled = true;
                OnPropertyChanged("SaveEnabled");
                if (_selectedUser != null) Chart();
                else SaveEnabled = false;
            }
        }

    }
}
