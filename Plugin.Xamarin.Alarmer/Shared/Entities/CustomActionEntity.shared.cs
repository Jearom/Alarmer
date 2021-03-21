using SQLite;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class CustomActionEntity : IModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int AlarmId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
