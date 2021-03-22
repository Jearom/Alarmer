# Alarm Plugin for Xamarin Forms

Alarmer is a helper plugin to show notifications with exact timing. I Create this plugin to use it on my own project. ios side not implemented yet but it will be available soon.

## Installation

You can initialize Alarmer like below;

```bash
Plugin.Xamarin.Alarmer.Alarmer.Current;

OR

var _alarmer = DependencyService.Get<IAlarmer>();
```


## Notify

```C#
 _alarmer.Notify("Test Title", "Test Message", Guid.NewGuid().ToString(), new Plugin.Xamarin.Alarmer.Shared.Models.NotificationOptions
                {
                    EnableSound = true,
                    EnableVibration = true,
                    SmallIcon = "ic_alarm",
                    CustomActions = new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction[] { new Plugin.Xamarin.Alarmer.Shared.Models.CustomAction {
                    Icon = "ic_alarm",
                    Name = "Snooze"
                    } }
                });
```

## Schedule
```
_alarmer.Schedule("Test Title", "Test Message", dateTime, new Plugin.Xamarin.Alarmer.Shared.Models.AlarmOptions
                {
                    AlarmSequence = (Enums.AlarmSequence)SelectedSequnce.Value,
                    Interval = Interval,
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
````
## Others

EnableAlarm and DisableAlarm functions added.


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
