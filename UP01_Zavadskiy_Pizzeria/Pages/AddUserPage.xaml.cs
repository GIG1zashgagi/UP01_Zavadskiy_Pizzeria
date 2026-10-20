using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UP01_Zavadskiy_Pizzeria.Pages;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class AddUserPage : Page
    {
        private app_users _current;

        public AddUserPage(app_users selected)
        {
            InitializeComponent();
            _current = selected ?? new app_users();
            DataContext = _current;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Password) && _current.user_id == 0)
                errors.AppendLine("Укажите пароль!");
            if (string.IsNullOrWhiteSpace(_current.role)) errors.AppendLine("Выберите роль!");
            if (string.IsNullOrWhiteSpace(_current.fio)) errors.AppendLine("Укажите ФИО!");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            using (var db = new uppizzariaEntities())
            {
                if (_current.user_id == 0)
                {
                    _current.password = AuthPage.GetHash(TBPass.Password);
                    db.app_users.Add(_current);
                }
                else
                {
                    var existing = db.app_users.Find(_current.user_id);
                    existing.login = _current.login;
                    existing.role = _current.role;
                    existing.fio = _current.fio;
                    if (!string.IsNullOrEmpty(TBPass.Password))
                        existing.password = AuthPage.GetHash(TBPass.Password);
                }
                db.SaveChanges();
            }
            MessageBox.Show("Данные сохранены!");
            NavigationService?.GoBack();
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Clear();
            TBPass.Clear();
            cmbRole.SelectedIndex = -1;
            TBFio.Clear();
        }
    }
}