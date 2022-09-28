using AnyWatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyWatcher.Services.Contracts
{
    public interface IWatcher
    {
        public string Name { get; }
        public string Type { get; }
        public string Message { get; }
        public bool Enabled { get; set; }
        public WatcherState State { get; set; }
        public List<WatchArgs> Args { get; set; }
        public CommonResult Check();
    }
}
