using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
   public class AlarmEntity : IModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
    }
}
