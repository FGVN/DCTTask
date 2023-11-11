using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using DCTTask.View;

namespace DCTTask
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string themeCacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheDirectory", "UICache.txt");
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Removing the default resource
            var dictionariesToKeep = Current.Resources.MergedDictionaries
    .Where(dictionary => dictionary.Source == null || dictionary.Source == new Uri("View/DarkTheme.xaml", UriKind.Relative))
    .ToList();

            Current.Resources.MergedDictionaries.Clear();

            foreach (var dictionary in dictionariesToKeep)
            {
                Current.Resources.MergedDictionaries.Add(dictionary);
            }


            // Searching cache to find which one should be used
            if (!File.Exists(themeCacheDirectory) || File.ReadAllText(themeCacheDirectory) == "0")
            {
                Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("View/LightTheme.xaml", UriKind.Relative)
                });
            }
            else
            {
                Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("View/DarkTheme.xaml", UriKind.Relative)
                });
            }


            MainWindow mainWindow = new MainWindow();
        }
    }
}
