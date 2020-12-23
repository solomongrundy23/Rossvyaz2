using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rossvyaz2
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        public WorkWindow(string[] files)
        {
            InitializeComponent();
            LoadWindow(files);
        }

        public SaveFileDialog SaverDialog = new SaveFileDialog()
        {
            Filter = "*.txt|*.txt|*.*|*.*",
        };

        public bool MainFrameEnabled
        {
            get => MainFrame.IsEnabled;
            set
            {
                ProgressVisible = !value;
                int To = value ? 0 : 20;
                DoubleAnimation blur = new DoubleAnimation()
                {
                    To = To,
                    Duration = TimeSpan.FromMilliseconds(500),
                    AccelerationRatio = 0.5
                };
                MainFrameBlur.BeginAnimation(BlurEffect.RadiusProperty, blur);
                MainFrame.IsEnabled = value;
            }
        }

        private bool ProgressVisible
        {
            get => ProgressInfo.Visibility == Visibility.Visible;
            set
            {
                int To = value ? 200 : 0;
                var anim = new DoubleAnimation()
                {
                    To = To,
                    Duration = TimeSpan.FromMilliseconds(500)
                    //AccelerationRatio = 0.5
                };
                ProgressInfo.BeginAnimation(HeightProperty, anim);
            }
        }

        public static void Error(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void UpdateElement(ListBox list, List<string> items, string filter)
        {
            Dispatcher.Invoke(() =>
            {
                list.ItemsSource = items.Where(x => x.ToLower().Contains(filter)).OrderBy(x => x);
            });
        }

        public async void LoadWindow(string[] files)
        {
            try
            {
                MainFrameEnabled = false;
                await rossRecords.LoadAsync(files);
                if (rossRecords.Items.Count() == 0) throw new Exception("При парсинге не удалость заполнить таблицу данных");
                string[] regions = rossRecords.GetRegionsList();
                Regions.NoSelect.AddRange(regions);
                UpdateElement(RegionsUnselect, Regions.NoSelect, RegionsFilterUnselect.Text);
                string[] providers = rossRecords.GetOperatorsList();
                Operators.NoSelect.AddRange(providers);
                UpdateElement(OperatorsUnselect, Operators.NoSelect, OperatorsFilterUnselect.Text);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                Close();
            }
            finally
            {
                MainFrameEnabled = true;
            }
        }

        private RossRecords rossRecords = new RossRecords();
        private Lists Regions = new Lists();
        private Lists Operators = new Lists();
        private string _OutText = string.Empty;
        public string OutText
        {
            get => _OutText;
            set
            {
                _OutText = value;
                Dispatcher.Invoke(() =>
                {
                    if (_OutText.IsEmpty()) OutTextBox.Text = "Нет данных или пустой результат";
                    else
                    if (_OutText.Length > 5000)
                    {
                        OutTextBox.Text = "Внимание! Результат большой, отображаются первые 10000 символов, " +
                        "скопируйте или сохраните результат по соответствующим кнопкам" 
                        + Environment.NewLine + Environment.NewLine + _OutText.Cut(10000);
                    }
                    else
                        OutTextBox.Text = _OutText;
                    ButtonSave.IsEnabled = ButtonCopy.IsEnabled = !_OutText.IsEmpty();
                });
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in RegionsUnselect.SelectedItems) Regions.Select(item);
            UpdateElement(RegionsUnselect, Regions.NoSelect, RegionsFilterUnselect.Text);
            UpdateElement(RegionsSelected, Regions.Selected, RegionsFilterSelect.Text);
        }

        private void RegionsFilterUnselect_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateElement(RegionsUnselect, Regions.NoSelect, RegionsFilterUnselect.Text.ToLower());
        }

        private void RegionsFilterSelect_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateElement(RegionsSelected, Regions.Selected, RegionsFilterSelect.Text.ToLower());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (string item in RegionsSelected.SelectedItems) Regions.UnSelect(item);
            UpdateElement(RegionsUnselect, Regions.NoSelect, RegionsFilterUnselect.Text.ToLower());
            UpdateElement(RegionsSelected, Regions.Selected, RegionsFilterSelect.Text.ToLower());
        }

        private void OperatorsFilterUnselect_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateElement(OperatorsUnselect, Operators.NoSelect, OperatorsFilterUnselect.Text.ToLower());
        }

        private void OperatorsFilterSelect_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateElement(OperatorsSelected, Operators.Selected, OperatorsFilterSelect.Text.ToLower());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (string item in OperatorsUnselect.SelectedItems) Operators.Select(item);
            UpdateElement(OperatorsUnselect, Operators.NoSelect, OperatorsFilterUnselect.Text);
            UpdateElement(OperatorsSelected, Operators.Selected, OperatorsFilterSelect.Text);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (string item in OperatorsSelected.SelectedItems) Operators.UnSelect(item);
            UpdateElement(OperatorsUnselect, Operators.NoSelect, OperatorsFilterUnselect.Text);
            UpdateElement(OperatorsSelected, Operators.Selected, OperatorsFilterSelect.Text);
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrameEnabled = false;
                string splitter = SplitterSelector.Text == "ENTER" ? Environment.NewLine : SplitterSelector.Text;
                int style = ModeSelector.SelectedIndex;
                bool checkOperatorNo = CheckOperatorNo.IsChecked.Value;
                bool checkRegionNo = CheckRegionNo.IsChecked.Value;
                await Task.Run(() =>
                {
                    var phoneRanger = new PhoneRanger();
                    phoneRanger.Ranges.AddRange(
                        rossRecords.GetRecords(
                            Operators.Selected.ToArray(), Regions.Selected.ToArray(), checkOperatorNo, checkRegionNo)
                        .Select(x => new Range(x.Min, x.Max)));
                    phoneRanger.Merge();
                    if (phoneRanger.Ranges.Count() == 0) OutText = string.Empty;
                    switch (style)
                    {
                        case 0:
                            OutText = string.Join(splitter, phoneRanger.BreakingRange(true));
                            break;
                        case 1:
                            OutText = string.Join(splitter, phoneRanger.BreakingRange(false));
                            break;
                        case 2:
                            OutText = string.Join(splitter, phoneRanger.Ranges.Select(x => $"{x.Min}-{x.Max}"));
                            break;
                        default:
                            throw new Exception("Обработчик для данного стиля не найден");
                    }
                });
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            finally
            {
                MainFrameEnabled = true;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (OutText != string.Empty) Clipboard.SetText(OutText);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SplitterSelector.SelectedIndex =
                OptionsWorker.Options.Splitter > -1 && OptionsWorker.Options.Splitter < SplitterSelector.Items.Count ?
                OptionsWorker.Options.Splitter : 0;
            ModeSelector.SelectedIndex =
                OptionsWorker.Options.WorkStyle > -1 && OptionsWorker.Options.WorkStyle < ModeSelector.Items.Count ?
                OptionsWorker.Options.WorkStyle : 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OptionsWorker.Options.Splitter = SplitterSelector.SelectedIndex;
            OptionsWorker.Options.WorkStyle = ModeSelector.SelectedIndex;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            SaverDialog.FileName = string.Empty;
            if (SaverDialog.ShowDialog().Value)
            {
                try
                {
                    File.WriteAllText(SaverDialog.FileName, _OutText);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
            }
        }
    }
}