using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class AddOrderPage : Page
    {
        private orders _current;

        public AddOrderPage(orders selected)
        {
            InitializeComponent();
            var db = new uppizzariaEntities();
            CBCustomer.ItemsSource = db.customers.ToList();
            CBPizzeria.ItemsSource = db.pizzerias.ToList();
            CBType.ItemsSource = db.order_types.ToList();
            CBStatus.ItemsSource = db.order_statuses.ToList();

            _current = selected ?? new orders { order_date = DateTime.Now };
            DataContext = _current;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();
            if (_current.customer_id == 0) errors.AppendLine("Выберите клиента!");
            if (_current.pizzeria_id == 0) errors.AppendLine("Выберите пиццерию!");
            if (_current.order_type_id == 0) errors.AppendLine("Выберите тип заказа!");
            if (_current.status_id == 0) errors.AppendLine("Выберите статус!");
            if (_current.final_amount < 0) errors.AppendLine("Сумма не может быть отрицательной!");
            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            using (var db = new uppizzariaEntities())
            {
                if (_current.order_id == 0)
                {
                    _current.order_date = DateTime.Now;
                    db.orders.Add(_current);
                }
                else
                {
                    var ex = db.orders.Find(_current.order_id);
                    ex.customer_id = _current.customer_id;
                    ex.pizzeria_id = _current.pizzeria_id;
                    ex.order_type_id = _current.order_type_id;
                    ex.status_id = _current.status_id;
                    ex.final_amount = _current.final_amount;
                    ex.comment = _current.comment;
                }
                db.SaveChanges();
            }
            MessageBox.Show("Заказ сохранён!");
            NavigationService?.GoBack();
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            CBCustomer.SelectedIndex = -1;
            CBPizzeria.SelectedIndex = -1;
            CBType.SelectedIndex = -1;
            CBStatus.SelectedIndex = -1;
            TBFinal.Clear();
            TBComment.Clear();
        }
    }
}