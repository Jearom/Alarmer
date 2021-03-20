using Plugin.Xamarin.Alarmer.Shared.Models;
using SQLite;
using System;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class AlarmEntity : IModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime StartDate { get; set; }
    }
}
