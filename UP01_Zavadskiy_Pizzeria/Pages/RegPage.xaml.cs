using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP01_Zavadskiy_Pizzeria.Pages;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
            => lblLogHint.Visibility = txtbxLog.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
            => lblPassHint.Visibility = passBxFrst.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
            => lblPassSecHint.Visibility = passBxScnd.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
            => lblFioHint.Visibility = txtbxFIO.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void lblLogHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxLog.Focus();
        private void lblPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxFrst.Focus();
        private void lblPassSecHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxScnd.Focus();
        private void lblFioHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxFIO.Focus();

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbxLog.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password) ||
                string.IsNullOrWhiteSpace(txtbxFIO.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (var db = new uppizzariaEntities())
            {
                if (db.app_users.Any(u => u.login == txtbxLog.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                if (passBxFrst.Password.Length < 6)
                {
                    MessageBox.Show("Пароль слишком короткий, минимум 6 символов!");
                    return;
                }

                bool en = true, number = false;
                foreach (char c in passBxFrst.Password)
                {
                    if (c >= '0' && c <= '9') number = true;
                    else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                        en = false;
                }
                if (!en) { MessageBox.Show("Используйте только английскую раскладку!"); return; }
                if (!number) { MessageBox.Show("Добавьте хотя бы одну цифру!"); return; }
                if (passBxFrst.Password != passBxScnd.Password)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                var newUser = new app_users
                {
                    login = txtbxLog.Text,
                    password = AuthPage.GetHash(passBxFrst.Password),
                    role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User",
                    fio = txtbxFIO.Text
                };
                db.app_users.Add(newUser);
                db.SaveChanges();
            }

            MessageBox.Show("Пользователь успешно зарегистрирован!");
            NavigationService?.Navigate(new AuthPage());
        }
    }
}