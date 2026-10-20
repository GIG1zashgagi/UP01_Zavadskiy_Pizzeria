using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = new uppizzariaEntities().app_users.ToList();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                using (var db = new uppizzariaEntities())
                {
                    DataGridUser.ItemsSource = db.app_users.ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new AddUserPage(null));

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new AddUserPage((sender as Button)?.DataContext as app_users));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridUser.SelectedItems.Cast<app_users>().ToList();
            if (selected.Count == 0) { MessageBox.Show("Выберите записи для удаления"); return; }

            if (MessageBox.Show($"Удалить {selected.Count} записей?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new uppizzariaEntities())
                    {
                        foreach (var item in selected)
                        {
                            var toDelete = db.app_users.Find(item.user_id);
                            if (toDelete != null) db.app_users.Remove(toDelete);
                        }
                        db.SaveChanges();
                    }
                    MessageBox.Show("Данные удалены!");
                    Page_IsVisibleChanged(null, default);
                }
                catch (System.Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}