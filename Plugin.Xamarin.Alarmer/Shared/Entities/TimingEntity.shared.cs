﻿using SQLite;
using System;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class TimingEntity : IModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int AlarmId { get; set; }

        public TimeSpan Time { get; set; }
    }
}
