using DevSup.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DevSup
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private string _currentThemeFileName = "ColorsDark.xaml";
        XmlLoad xmlLoad = XmlLoad.make();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _currentThemeFileName = xmlLoad.GetTheme();
            // 초기 테마 설정
            ApplyTheme(_currentThemeFileName);
        }

        public void ChangeTheme(string newThemeFileName)
        {
            // 현재 테마를 제거
            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(dict => dict.Source != null &&
                                        dict.Source.OriginalString.EndsWith(_currentThemeFileName, StringComparison.OrdinalIgnoreCase));

            if (oldTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
            }

            // 새로운 테마를 추가
            var newTheme = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Theme/{newThemeFileName}", UriKind.Absolute)
            };

            Application.Current.Resources.MergedDictionaries.Add(newTheme);
            _currentThemeFileName = newThemeFileName;
        }

        private void ApplyTheme(string themeFileName)
        {
            var theme = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Theme/{themeFileName}", UriKind.Absolute)
            };

            Application.Current.Resources.MergedDictionaries.Add(theme);
            _currentThemeFileName = themeFileName;
        }
    }
}
