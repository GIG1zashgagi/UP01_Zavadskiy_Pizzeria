using System;
using System.Windows;
using System.Windows.Threading;

namespace UP01_Zavadskiy_Pizzeria
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private bool _isDark = false;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.AuthPage());

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => TbDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            _timer.Start();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDark = !_isDark;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            try
            {
                var uri = new Uri(_isDark ? "DictionaryDark.xaml" : "Dictionary.xaml", UriKind.Relative);
                var dict = (ResourceDictionary)Application.LoadComponent(uri);

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);

                var width = this.Width;
                this.Width = width + 1;
                this.Width = width;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка смены темы: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?",
                "Message", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}