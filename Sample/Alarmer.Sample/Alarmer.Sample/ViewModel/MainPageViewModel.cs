﻿using Alarmer.Sample.Models;
using Plugin.Xamarin.Alarmer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;


namespace Alarmer.Sample.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        private DateTime selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan selectedTime = DateTime.Now.TimeOfDay;
        IAlarmer alrm;
        public MainPageViewModel()
        {
            alrm = DependencyService.Get<IAlarmer>();
            alrm.NotificationReceived += notificationreceived;
            alrm.NotificationSelectionReceived += notificationSelectionReceived;
        }

        public TimeSpan SelectedTime
        {
            get { return selectedTime; }
            set
            {
                selectedTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan selectedEndTime = DateTime.Now.TimeOfDay;

        public TimeSpan SelectedEndTime
        {
            get { return selectedEndTime; }
            set { selectedEndTime = value; }
        }


        private DateTime endDate = DateTime.Now.AddDays(1);

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        private bool isEndDate;

        public bool IsEndDate
        {
            get { return isEndDate; }
            set { isEndDate = value; }
        }




        private List<EnumModel> sequences = new List<EnumModel>
        {
            new EnumModel{ Text=Enums.AlarmSequence.OneTime.ToString(), Value = (int)Enums.AlarmSequence.OneTime},
            new EnumModel{ Text=Enums.AlarmSequence.Minute.ToString(), Value = (int)Enums.AlarmSequence.Minute},
            new EnumModel{ Text=Enums.AlarmSequence.Hourly.ToString(), Value = (int)Enums.AlarmSequence.Hourly},
            new EnumModel{ Text=Enums.AlarmSequence.Daily.ToString(), Value = (int)Enums.AlarmSequence.Daily},
            new EnumModel{ Text=Enums.AlarmSequence.Weekly.ToString(), Value = (int)Enums.AlarmSequence.Weekly},
            new EnumModel{ Text=Enums.AlarmSequence.Monthly.ToString(), Value = (int)Enums.AlarmSequence.Monthly},
            new EnumModel{ Text=Enums.AlarmSequence.Yearly.ToString(), Value = (int)Enums.AlarmSequence.Yearly},
        };

        public List<EnumModel> Sequences
        {
            get { return sequences; }
            set { sequences = value; }
        }

        private Enums.AlarmSequence selectedSequence;

        public Enums.AlarmSequence SelectedSequnce
        {
            get { return selectedSequence; }
            set { selectedSequence = value; }
        }


        private int interval;

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        private int maxCount = 0;

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }


        public ICommand StartAlarm => new Command(() =>
        {

            try
            {

                alrm.Notify("Test Title", "Test Message", Guid.NewGuid().ToString(), new Plugin.Xamarin.Alarmer.Shared.Models.NotificationOptions
                {
                    EnableSound = true,
                    EnableVibration = true,
                    SmallIcon = "ic_alarm",
                    CustomActions = new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction[] { new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction {
                    Icon = "ic_alarm",
                    Name = "Snooze"
                    } }
                });

            }
            catch (Exception e)
            {
                var asd = e;
            }

        });

        public ICommand ScheduleAlarm => new Command(() =>
        {
            DateTime dateTime = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, selectedTime.Hours, selectedTime.Minutes, selectedTime.Seconds);

            try
            {
                DateTime? _endDate = null;

                if (IsEndDate)
                    _endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, SelectedEndTime.Hours, SelectedEndTime.Minutes, SelectedEndTime.Seconds);

                alrm.Schedule("Test Title", "Test Message", dateTime, new Plugin.Xamarin.Alarmer.Shared.Models.AlarmOptions
                {
                    AlarmSequence = Enums.AlarmSequence.OneTime,
                    Interval = 1,
                    EndDate = _endDate,
                    TotalAlarmCount = MaxCount
                },
                    new Plugin.Xamarin.Alarmer.Shared.Models.NotificationOptions
                    {
                        EnableSound = true,
                        EnableVibration = true,
                        SmallIcon = "ic_alarm",
                        CustomActions = new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction[] { new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction {
                    Icon = "ic_alarm",
                    Name = "Snooze"
                    } }
                    });

            }
            catch (Exception e)
            {
                var asd = e;
            }

        });

        private void notificationSelectionReceived(object sender, EventArgs e)
        {
            var asd = e;
        }

        private void notificationreceived(object sender, EventArgs e)
        {
            var asd = e;
        }

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
