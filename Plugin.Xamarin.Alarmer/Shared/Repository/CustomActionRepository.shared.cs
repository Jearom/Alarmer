﻿using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Entities;
using SQLite;
using System;
using System.IO;
using UniCore.Mobile.Extension.SQLite;

namespace Plugin.Xamarin.Alarmer.Shared.Repository
{
    public class CustomActionRepository : SqlRepository<CustomActionEntity>
    {
        public const SQLite.SQLiteOpenFlags Flags =
    // open the database in read/write mode
    SQLite.SQLiteOpenFlags.ReadWrite |
    // create the database if it doesn't exist
    SQLite.SQLiteOpenFlags.Create |
    // enable multi-threaded database access
    SQLite.SQLiteOpenFlags.SharedCache;

        private static string _path
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, Consts.DatabaseName);
            }
        }

        public CustomActionRepository() : base(_path, Flags)
        {
          
        }
    }
}
