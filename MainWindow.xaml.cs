using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Rossvyaz2
{

    public partial class MainWindow : Window
    {
        public string ProgressTitle
        {
            set => Dispatcher.Invoke(() => ProgressText.Content = value);
        }

        public MainWindow()
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                OptionsWorker.Load();
            }
            catch
            {
                OptionsWorker.Options = new Options();
            }
            InitializeComponent();
        }

        OpenFileDialog FileDialog = new OpenFileDialog() { Multiselect = true, Filter = "*.csv|*.csv|*.*|*.*", CheckFileExists = true };
        public string[] Files 
        {
            get => OptionsWorker.Options.Files;
            set 
            {
                OptionsWorker.Options.Files = value;
                TextFiles.ItemsSource = value;
                DoWork.IsEnabled = value.Length != 0;
            }
        }

        private bool FormEnabled
        {
            get => MainFrame.IsEnabled;
            set
            {
                MainBlur.Radius = value ? 0 : Config.blurLevel;
                MainFrame.IsEnabled = value; 
            }
        }

        private bool ProgressVisible
        {
            set
            {
                if (value)
                {
                    FormEnabled = false;
                    int To = 120;
                    DoubleAnimation anim = new DoubleAnimation()
                    {
                        IsAdditive = true,
                        To = To,
                        Duration = TimeSpan.FromMilliseconds(Config.animDuration)
                    };
                    ProgressInfo.BeginAnimation(HeightProperty, anim);
                }
                else
                {
                    int To = 0;
                    DoubleAnimation anim = new DoubleAnimation()
                    {
                        IsAdditive = true,
                        To = To,
                        Duration = TimeSpan.FromMilliseconds(Config.animDuration)
                    };
                    anim.Completed += CloseAnimation_Completed;
                    ProgressInfo.BeginAnimation(HeightProperty, anim);
                }
            }
        }

        public static void Error(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProgressVisible = true;
            try
            {
                foreach (string url in OptionsWorker.Options.Urls)
                {
                    ProgressTitle = $"Скачиваю\n{url}";
                    if (!await DownloadCSV(url)) throw new Exception($"Ошибка скачивания файла\n{url}");
                }
                Files = OptionsWorker.Options.Urls.Select(x => GetFileNameFromUrl(x)).ToArray();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            finally
            {
                ProgressVisible = false;
            }
        }

        private string GetFileNameFromUrl(string str)
            => OptionsWorker.DataPath + System.IO.Path.GetFileName(str);

        private async Task<bool> DownloadCSV(string url)
        {
            return await DownloadCSV(new string[] { url });
        }

        private async Task<bool> DownloadCSV(string[] url_array)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(OptionsWorker.DataPath)) 
                        Directory.CreateDirectory(OptionsWorker.DataPath);
                    using (var wc = new WebClient() { })
                        foreach(string url in url_array)
                            wc.DownloadFile(url, GetFileNameFromUrl(url));
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormEnabled = false;
            if (FileDialog.ShowDialog().Value) Files = FileDialog.FileNames;
            FormEnabled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var WW = new WorkWindow(Files);
            try
            {
                this.Hide();
                WW.ShowDialog();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            finally
            {
                WW.Close();
                this.Show();
                GC.Collect(GC.MaxGeneration);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            TextFiles.ItemsSource = OptionsWorker.Options.Files;
            LabelVersion.Content = $"Версия: " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FormEnabled = this.Visibility == Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OptionsWorker.Save();
        }

        private bool AboutVisible
        {
            get => About.Visibility == Visibility.Visible;
            set
            {
                if (value)
                {
                    FormEnabled = false;
                    DoubleAnimation anim = new DoubleAnimation()
                    {
                        IsAdditive = true,
                        To = 250,
                        Duration = TimeSpan.FromMilliseconds(Config.animDuration),
                        AccelerationRatio = 0.5
                    };
                    anim.Completed += OpenAbout_Completed;
                    About.BeginAnimation(HeightProperty, anim);
                }
                else
                {
                    About.IsEnabled = false;
                    DoubleAnimation anim = new DoubleAnimation()
                    {
                        IsAdditive = true,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(Config.animDuration),
                        AccelerationRatio = 0.5
                    };
                    anim.Completed += CloseAnimation_Completed;
                    About.BeginAnimation(HeightProperty, anim);
                }
            }
        }

        private void CloseAnimation_Completed(object sender, EventArgs e)
        {
            try
            {
                FormEnabled = true;
            }
            catch { }
        }

        private void OpenAbout_Completed(object sender, EventArgs e)
        {
            try
            { 
            About.IsEnabled = true;
            }
            catch { }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AboutVisible = true;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AboutVisible = false;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://money.yandex.ru/to/410018346573481");
            AboutVisible = false;
        }

        private void WindowsFalls_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Margin = new Thickness(0);
        }

        private void WindowsFalls_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Margin = new Thickness(2);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
