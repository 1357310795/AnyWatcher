using AnyWatcher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyWatcher.Models
{
    public class WatchStorage
    {
        public string Type { get; set; } 
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<WatchArgs> Args { get; set; }
    }
}
