using AnyWatcher.Models;
using AnyWatcher.Services.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyWatcher.Services
{
    public static class StorageService
    {
        public static List<WatchStorage> Read()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AnyWatcher\data.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var s = JsonConvert.DeserializeObject<List<WatchStorage>>(json);
                return s;
            }
            else 
                return null;
        }

        public static void Save(List<WatchStorage> watches)
        {
            var json = JsonConvert.SerializeObject(watches);
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AnyWatcher"));
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AnyWatcher\data.json"), json);
        }
    }
}
