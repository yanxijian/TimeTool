using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimeTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon  notifyIcon;
        System.Windows.Forms.ContextMenu contextMenu;
        readonly string configFileName = "config.conf";
        readonly string confLogTime    = "Log_Time";
        string settingFilePath         = null;

        public MainWindow()
        {
            InitializeComponent();
            InitIcon();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
        }

        private void InitIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipText = "Hello, " + (Environment.UserName ?? "guy") + ".";
            notifyIcon.Text = "Time Tool";

            // Set the icon
            Uri iconUri = new Uri("pack://application:,,,/Images/clock.ico");
            Stream iconStream = Application.GetResourceStream(iconUri).Stream;
            notifyIcon.Icon = new Icon(iconStream);
            iconStream.Dispose();

            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000);

            SetIconEvent();
        }

        private void SetIconEvent()
        {
            notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (this.Visibility == Visibility.Visible)
                    {
                        this.Hide();
                    }
                    else
                    {
                        this.Show();
                    }
                }
                else
                {
                    ContextMenu contexMenu = this.FindResource("contexMenu") as ContextMenu;
                    contexMenu.PlacementTarget = s as Button;
                    foreach (MenuItem item in contexMenu.Items)
                    {
                        if (item.Name.Equals("logTime"))
                        {
                            item.IsChecked = bool.Parse(GetConfig(confLogTime) ?? "False");
                        }
                    }
                    contexMenu.IsOpen = true;
                }
            };
        }

        private void InitContexMenu()
        {
            contextMenu = new System.Windows.Forms.ContextMenu();

            // Exit Menu Item
            var exitMenu = new System.Windows.Forms.MenuItem();
            exitMenu.Index = 0;
            exitMenu.Text = "E&xit";
            exitMenu.Click += (s, e) =>
                {
                    base.Close();
                };

            contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {exitMenu});
            notifyIcon.ContextMenu = contextMenu;
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void logTime_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            item.IsChecked = !item.IsChecked;
            SetConfig(confLogTime, item.IsChecked.ToString());
        }

        private string SettingFilePath()
        {
            if (!String.IsNullOrEmpty(settingFilePath))
            {
                return settingFilePath;
            }

            settingFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TimeTool";
            if (!Directory.Exists(settingFilePath))
            {
                Directory.CreateDirectory(settingFilePath);
            }
            settingFilePath += "\\" + configFileName;
            if (!File.Exists(settingFilePath))
            {
                File.CreateText(settingFilePath).Close();
            }
            return settingFilePath;
        }

        private string GetConfig(string confName)
        {
            Dictionary<string, string> configs = LoadConfig();
            string value;
            if (configs.TryGetValue(confName, out value))
            {
                return value;
            }
            return null;
        }

        private void SetConfig(string confName, string confValue)
        {
            Dictionary<string, string> configs = LoadConfig();
            string value;
            if (configs.TryGetValue(confName, out value))
            {
                configs[confName] = confValue;
            }
            else
            {
                configs.Add(confName, confValue);
            }

            SaveConfig(configs);
        }

        private Dictionary<string, string> LoadConfig()
        {
            Dictionary<string, string> configs = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader(SettingFilePath(), Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    configs.Add(line.Split('=')[0], line.Split('=')[1]);
                }
            }
            return configs;
        }

        private void SaveConfig(Dictionary<string, string> configs)
        {
            FileStream fs = new FileStream(SettingFilePath(), FileMode.Truncate);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var config in configs)
            {
                string line = config.Key + "=" + config.Value + Environment.NewLine;
                sw.Write(line);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
