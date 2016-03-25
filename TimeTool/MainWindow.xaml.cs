using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon  notifyIcon;
        System.Windows.Forms.ContextMenu contextMenu;

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
            notifyIcon.BalloonTipText = "Hello, " + Environment.UserName;
            notifyIcon.Text = "Time Tool";

            Uri iconUri = new Uri("pack://application:,,,/clock.ico");
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
                    //notifyIcon.ContextMenu.Show((Button)s, e.Location);
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
    }
}
