using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class CustomActionEntity : IModel
    {
        public int AlarmId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
