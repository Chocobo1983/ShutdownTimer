using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShutdownTimer
{
    internal class ViewModel : VMbase
    {
        #region PrivateFields
        int _hours;
        int _minutes;
        int _seconds;
        bool _active = true;
        string _title = "بداية";
        
        RelayCommand _secondsDecrement;
        RelayCommand _secondsIncrement;
        RelayCommand _minutesDecrement;
        RelayCommand _minutesIncrement;
        RelayCommand _hoursDecrement;
        RelayCommand _hoursIncrement;
        RelayCommand _start;

        Task task;
        CancellationTokenSource cancelTokenSource;
        CancellationToken token;
        #endregion
        #region PublicProperties
        public bool Active { get { return _active; } set { _active = value; OnPropertyChanged(nameof(Active)); } }
        public int Hours { get { return _hours; } set { _hours = value; OnPropertyChanged(nameof(Hours)); } }
        public int Minutes { get { return _minutes; } set { _minutes = value; OnPropertyChanged(nameof(Minutes)); } }
        public int Seconds { get { return _seconds; } set { _seconds = value; OnPropertyChanged(nameof(Seconds)); } }
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        #endregion
        public ViewModel()
        {
            _secondsDecrement = new RelayCommand(SecondsDecrement);
            _secondsIncrement = new RelayCommand(SecondsIncrement);
            _minutesDecrement = new RelayCommand(MinutesDecrement);
            _minutesIncrement = new RelayCommand(MinutesIncrement);
            _hoursDecrement = new RelayCommand(HoursDecrement);
            _hoursIncrement = new RelayCommand(HoursIncrement);
            _start = new RelayCommand(Start);
            
        }
        #region PrivateMethods
        void SecondsDecrement(object o)
        {
            if (Seconds > 0) --Seconds;
        }
        void SecondsIncrement(object o)
        {
            if (Seconds > 58)
            {
                Seconds = 0;
                MinutesIncrement(o);
                return;
            }
            ++Seconds;
        }
        void MinutesDecrement(object o)
        {
            if (Minutes > 0) --Minutes;
        }
        void MinutesIncrement(object o)
        {
            if (Minutes > 58)
            {
                Minutes = 0;
                HoursIncrement(o);
                return;
            }
            ++Minutes;
        }
        void HoursDecrement(object o)
        {
            if (Hours > 0) --Hours;
        }
        void HoursIncrement(object o)
        {
            if (Hours < 99) ++Hours;
        }

        void Start(object o)
        {
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            task = new Task(() => Countdown(), token);
            if (_active)
            {
                _active = false;
                Title = "قف";
                task.Start();
            }
            else
            {
                _active = true;
                Title = "بداية";
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
            }

        }
        void Countdown()
        {
            while (Hours > 0 || Minutes > 0 || Seconds > 0)
            {
                if (token.IsCancellationRequested) return;
                if (Seconds > 0) Seconds--;
                else if (Seconds == 0 && Minutes > 0)
                {
                    Minutes--;
                    Seconds = 59;
                }
                else if (Seconds == 0 && Minutes == 0 && Hours > 0)
                {
                    Hours--;
                    Minutes = 59;
                    Seconds = 59;
                }
                Thread.Sleep(1000);
            }
            Process.Start("shutdown", "/f /s /t 0");
        }
        #endregion
        #region Commands
        public ICommand SecondsIncrementCommand { get { return _secondsIncrement; } }
        public ICommand SecondsDecrementCommand { get { return _secondsDecrement; } }
        public ICommand MinutesIncrementCommand { get { return _minutesIncrement; } }
        public ICommand MinutesDecrementCommand { get { return _minutesDecrement; } }
        public ICommand HoursIncrementCommand { get { return _hoursIncrement; } }
        public ICommand HoursDecrementCommand { get { return _hoursDecrement; } }
        public ICommand StartCommand { get { return _start; } }
        #endregion
    }
}
