using AircraftApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AircraftApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private string _runwayLengthInput = "1500";
        public string RunwayLengthInput
        {
            get => _runwayLengthInput;
            set => SetProperty(ref _runwayLengthInput, value);
        }
        private Aircraft? _selectedAircraft;
        private string _status = "Ready";

        public ObservableCollection<Aircraft> Aircrafts { get; } = new ObservableCollection<Aircraft>();

        public Aircraft? SelectedAircraft
        {
            get => _selectedAircraft;
            set => SetProperty(ref _selectedAircraft, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public ICommand AddAirplaneCommand { get; }
        public ICommand TakeOffCommand { get; }
        public ICommand LandCommand { get; }

        public MainViewModel()
        {
            // Инициализация коллекции летательных аппаратов
            Aircrafts.Add(new Helicopter());   // Вертолет

            // Инициализация команд
            TakeOffCommand = new RelayCommand(ExecuteTakeOff);
            LandCommand = new RelayCommand(ExecuteLand);
            AddAirplaneCommand = new RelayCommand(AddAirplane);

            // Подписка на события изменения статуса
            foreach (var aircraft in Aircrafts)
            {
                aircraft.StatusChanged += (sender, message) => 
                    Status = $"{sender?.GetType().Name}: {message}";
            }
        }

            private void AddAirplane()
            {
                if (double.TryParse(RunwayLengthInput, out double length))
                {
                    var airplane = new Airplane(length);
                    airplane.StatusChanged += (sender, msg) => Status = msg;
                    Aircrafts.Add(airplane);
                }
            }

            private void ExecuteTakeOff()
            {
                SelectedAircraft?.TakeOff();
            }

            private void ExecuteLand()
            {
                SelectedAircraft?.Land();
            }
    }
}