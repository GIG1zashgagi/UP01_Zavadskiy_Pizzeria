using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Zavadskiy_Pizzeria.Pages;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage() { InitializeComponent(); }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) ||
                string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            using (var db = new uppizzariaEntities())
            {
                string hashedCurrent = AuthPage.GetHash(CurrentPasswordBox.Password);
                var user = db.app_users.FirstOrDefault(u => u.login == TbLogin.Text && u.password == hashedCurrent);
                if (user == null)
                {
                    MessageBox.Show("Текущий пароль/логин неверный!");
                    return;
                }

                if (NewPasswordBox.Password.Length < 6)
                {
                    MessageBox.Show("Новый пароль слишком короткий, минимум 6 символов!");
                    return;
                }

                bool en = true, number = false;
                foreach (char c in NewPasswordBox.Password)
                {
                    if (c >= '0' && c <= '9') number = true;
                    else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                        en = false;
                }
                if (!en) { MessageBox.Show("Используйте только английскую раскладку!"); return; }
                if (!number) { MessageBox.Show("Добавьте хотя бы одну цифру!"); return; }
                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                user.password = AuthPage.GetHash(NewPasswordBox.Password);
                db.SaveChanges();
            }

            MessageBox.Show("Пароль успешно изменён!");
            NavigationService?.Navigate(new AuthPage());
        }
    }
}