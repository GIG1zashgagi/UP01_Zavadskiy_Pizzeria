using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class AddCategoryPage : Page
    {
        private menu_categories _current;

        public AddCategoryPage(menu_categories selected)
        {
            InitializeComponent();
            _current = selected ?? new menu_categories();
            DataContext = _current;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.name)) errors.AppendLine("Укажите название!");
            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            using (var db = new uppizzariaEntities())
            {
                if (_current.category_id == 0) db.menu_categories.Add(_current);
                else
                {
                    var ex = db.menu_categories.Find(_current.category_id);
                    ex.name = _current.name;
                    ex.display_order = _current.display_order;
                }
                db.SaveChanges();
            }
            MessageBox.Show("Данные сохранены!");
            NavigationService?.GoBack();
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBCategoryName.Clear();
            TBOrder.Clear();
        }
    }
}