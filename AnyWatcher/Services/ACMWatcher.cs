using AnyWatcher.Models;
using AnyWatcher.Service;
using AnyWatcher.Services.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnyWatcher.Services
{
    public partial class ACMWatcher : ObservableObject, IWatcher
    {
        public ACMWatcher()
        {
            State = WatcherState.Wait;
            Enabled = true;
            Args = new List<WatchArgs>
            {
                new WatchArgs() { Id = "cookie", Desc = "Cookie" },
                new WatchArgs() { Id = "regex", Desc = "比赛名称正则" },
            };
        }
        public string Type { get; } = nameof(ACMWatcher);
        public string Name { get; } = "ACM 比赛监测";
        [ObservableProperty]
        private bool enabled;
        [ObservableProperty]
        private string message = "等待运行";
        [ObservableProperty]
        private WatcherState state;
        public List<WatchArgs> Args { get; set; }

        public CommonResult Check()
        {
            try
            {
                var headers = new Dictionary<string, string>();
                headers.Add("Cookie", Args.First(x => x.Id == "cookie").Input);
                var res = Web.Get("https://acm.sjtu.edu.cn/OnlineJudge/contest", headers);

                if (res.code == System.Net.HttpStatusCode.OK)
                {
                    var html = res.result;
                    var reg = new Regex(Args.First(x => x.Id == "regex").Input);
                    var regres = reg.Match(html);
                    if (regres.Success)
                    {
                        State = WatcherState.Done;
                        Message = "比赛存在";
                        return new CommonResult(true, Message);
                    }
                    else
                    {
                        State = WatcherState.Running;
                        Message = "不存在比赛";
                        return new CommonResult(true, Message);
                    }

                }
                else
                {
                    State = WatcherState.Error;
                    Message = "Cookie 无效";
                    return new CommonResult(false, Message);
                }
            }
            catch(Exception ex)
            {
                State = WatcherState.Error;
                Message = ex.Message;
                return new CommonResult(false, Message);
            }
        }
    }
}
