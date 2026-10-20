using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class OrdersTabPage : Page
    {
        public OrdersTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            try
            {
                using (var db = new uppizzariaEntities())
                {
                    DataGridPayment.ItemsSource = db.orders.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) LoadData();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new AddOrderPage(null));

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new AddOrderPage((sender as Button)?.DataContext as orders));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridPayment.SelectedItems.Cast<orders>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите записи");
                return;
            }

            if (MessageBox.Show($"Удалить {selected.Count} заказов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new uppizzariaEntities())
                    {
                        foreach (var item in selected)
                        {
                            var toDelete = db.orders.Find(item.order_id);
                            if (toDelete != null) db.orders.Remove(toDelete);
                        }
                        db.SaveChanges();
                    }
                    MessageBox.Show("Данные удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message);
                }
            }
        }
    }
}