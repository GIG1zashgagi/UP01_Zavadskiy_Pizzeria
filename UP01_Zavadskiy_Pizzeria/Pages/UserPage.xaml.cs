using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class UserPage : Page
    {
        private app_users _currentUser;
        private bool _initialized = false;

        public UserPage(app_users user)
        {
            InitializeComponent();
            _currentUser = user;
            _initialized = true;
            UpdateUsers();
        }

        private void fioFilterTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateUsers();
        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs e) => UpdateUsers();

        private void clearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            fioFilterTextBox.Text = "";
            sortComboBox.SelectedIndex = 0;
            onlyAdminCheckBox.IsChecked = false;
        }

        private void UpdateUsers()
        {
            if (!_initialized) return;
            try
            {
                using (var db = new uppizzariaEntities())
                {
                    var users = db.app_users.AsNoTracking().ToList();

                    if (!string.IsNullOrWhiteSpace(fioFilterTextBox.Text))
                        users = users.Where(x => x.fio.ToLower().Contains(fioFilterTextBox.Text.ToLower())).ToList();

                    if (onlyAdminCheckBox.IsChecked == true)
                        users = users.Where(x => x.role == "Admin").ToList();

                    ListUser.ItemsSource = sortComboBox.SelectedIndex == 0
                        ? users.OrderBy(x => x.fio).ToList()
                        : users.OrderByDescending(x => x.fio).ToList();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}