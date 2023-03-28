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

                    for (int j = 0; j < forecastNode.AsArray().Count-1; j++)
                    {
                        
                        int RankNode = forecastNode![j]["Rank"]!.GetValue<int>();
                        string UserNode = forecastNode![j]["User"]!.GetValue<string>();
                        string StatusNode = forecastNode![j]["Status"]!.GetValue<string>();
                        int StepsNode = forecastNode![j]["Steps"]!.GetValue<int>();

                        if (!_users.ContainsKey(UserNode))
                        {
                            _users.Add(UserNode, new Users());
                            _users[UserNode].User = UserNode;
                        }
                        _users[UserNode].AddDay(i, RankNode, StatusNode, StepsNode);
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


        //private int _number1;
        //public int Number1
        //{
        //    get { return _number1; }
        //    set
        //    {
        //        _number1 = value;
        //        OnPropertyChanged("Number3"); // уведомление View о том, что изменилась сумма
        //    }
        //}

        //private int _number2;
        //public int Number2
        //{
        //    get { return _number2; }
        //    set { _number2 = value; OnPropertyChanged("Number3"); }
        //}

        ////свойство только для чтения, оно считывается View каждый раз, когда обновляется Number1 или Number2
        //public int Number3 => Number1 + Number2;
    }
}
