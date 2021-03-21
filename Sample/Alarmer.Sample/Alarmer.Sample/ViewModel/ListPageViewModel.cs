using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Alarmer.Sample.ViewModel
{
    public class ListPageViewModel : INotifyPropertyChanged
    {

        readonly IAlarmer _alarmer;

        public ListPageViewModel()
        {
            _alarmer = DependencyService.Get<IAlarmer>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var list = await _alarmer.GetAlarmList();

                Alarms = new ObservableCollection<AlarmModel>(list);

            });
        }

        private ObservableCollection<AlarmModel> alarms;
        public ObservableCollection<AlarmModel> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;

                OnPropertyChanged();
            }
        }



        public ICommand GotoMainPage => new Command(() =>
        {
            Xamarin.Forms.Application.Current.MainPage = new MainPage();

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
