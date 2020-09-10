using Plugin.Xamarin.Alarmer.Shared;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;


namespace Alarmer.Sample.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan selectedTime;

        public TimeSpan SelectedTime
        {
            get { return selectedTime; }
            set
            {
                selectedTime = value;
                OnPropertyChanged();
            }
        }



        public ICommand StartAlarm => new Command(() =>
        {
            var alrm = DependencyService.Get<IAlarmer>();
            try
            {
               
                Plugin.Xamarin.Alarmer.Alarmer.Current.Notify("Test Title", "Test Message", true, true);

            }
            catch (Exception e)
            {
                var asd = e;
            }
            
        });



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
