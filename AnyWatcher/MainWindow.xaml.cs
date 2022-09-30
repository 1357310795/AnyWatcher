using AnyWatcher.Models;
using AnyWatcher.Services;
using AnyWatcher.Services.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AnyWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        [ObservableProperty]
        private ObservableCollection<IWatcher> watchers;

        [ObservableProperty]
        private int interval = 5000;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Watchers = new ObservableCollection<IWatcher>();
            Watchers.Add(new ACMWatcher());
            OnGoAnimation += TaskModel_OnGoAnimation;

            var s = StorageService.Read();
            if (s != null)
            {
                foreach (var tmp in s)
                {
                    var watch = Watchers.FirstOrDefault(x => x.Type == tmp.Type);
                    if (watch != null)
                    {
                        watch.Args = tmp.Args;
                        watch.Enabled = tmp.Enabled;
                    }
                }
            }
            
        }

        private void TaskModel_OnGoAnimation()
        {
            this.Dispatcher.Invoke(startani);
        }

        private void startani()
        {
            var da = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(Interval));
            ProgressBar1.BeginAnimation(ProgressBar.ValueProperty, da);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleRun();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            List<WatchStorage> watches = new List<WatchStorage>();
            foreach(var watch in Watchers)
            {
                watches.Add(new WatchStorage() { Args = watch.Args, Enabled = watch.Enabled, Name = watch.Name, Type = watch.Type });
            }
            StorageService.Save(watches);
        }


        #region Task Work
        private TaskState State;

        private BackgroundWorker bgw;

        public delegate void OnGoAnimationHandler();

        public event OnGoAnimationHandler OnGoAnimation;
        private bool Go()
        {
            bool onesuccess = false;
            //----Check Condition----//
            foreach (var watcher in watchers) 
            {
                if (watcher.State == Models.WatcherState.Running ||
                    watcher.State == Models.WatcherState.Wait)
                {
                    var res = watcher.Check();
                    onesuccess |= res.Success;
                    if (!res.Success)
                        return false;
                    if (watcher.State == WatcherState.Done)
                    {
                        try
                        {

                            new ToastContentBuilder()
                            .AddText($"发现更新：{watcher.Name}")
                            .Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
            return onesuccess;
        }

        public void StartRun()
        {
            if (State == TaskState.Started) return;
            if (!CanRun())
            {
                //
                return;
            }
            foreach (var watcher in watchers)
                //if (watcher.State == Models.WatcherState.Error)
                    watcher.State = Models.WatcherState.Wait;
            State = TaskState.Started;
            initbgw();
            bgw.RunWorkerAsync();
            OnGoAnimation.Invoke();
        }

        public void StopRun()
        {
            State = TaskState.None;
        }

        public bool CanRun()
        {
            return true;
        }

        public void ToggleRun()
        {
            if (State == TaskState.None)
            {
                StartRun();
            }
            else
            {
                StopRun();
            }
        }

        private void initbgw()
        {
            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            }
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Monitor.Enter(this);
            if ((bool)e.Result)
            {
                bgw.RunWorkerAsync();
            }
            Monitor.Exit(this);
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var res = Go();//true继续
            e.Result = res;
            if (!res)
            {
                State = TaskState.None;
                return;
            }
            if (State != TaskState.Started)
            {
                StopRun();
                e.Result = false;
                return;
            }
            OnGoAnimation.Invoke();
            System.Threading.Thread.Sleep(Interval);
            if (State != TaskState.Started)
            {
                StopRun();
                e.Result = false;
                return;
            }
        }
        #endregion

    }

    public enum TaskState
    {
        None, Started
    }
}
