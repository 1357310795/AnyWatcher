using AnyWatcher.Helpers;
using AnyWatcher.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AnyWatcher.Converters
{
    public class WatchStateToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WatcherState state)
            {
                switch (state)
                {
                    case WatcherState.Running:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#4CAF50"));//绿色
                    case WatcherState.Error:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#F44336"));//红色
                    case WatcherState.Disabled:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#607D8B"));//灰色
                    case WatcherState.Wait:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#2196F3"));//蓝色
                    case WatcherState.Done:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#673AB7"));//紫色
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
