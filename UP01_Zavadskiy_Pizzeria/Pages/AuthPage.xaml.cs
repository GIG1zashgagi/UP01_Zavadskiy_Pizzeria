using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP01_Zavadskiy_Pizzeria;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(x => x.ToString("X2")));
            }
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = TextBoxLogin.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassHint.Visibility = PasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин или пароль");
                return;
            }

            string hashed = GetHash(PasswordBox.Password);
            using (var db = new uppizzariaEntities())
            {
                var user = db.app_users.AsNoTracking()
                    .FirstOrDefault(u => u.login == TextBoxLogin.Text && u.password == hashed);

                if (user == null)
                {
                    failedAttempts++;
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    if (failedAttempts >= 3 && captcha.Visibility != Visibility.Visible)
                    {
                        CaptchaSwitch();
                        CaptchaChange();
                    }
                    else if (captcha.Visibility == Visibility.Visible)
                    {
                        CaptchaChange();
                    }
                    return;
                }

                failedAttempts = 0;
                MessageBox.Show($"Добро пожаловать, {user.fio}!");

                var mw = (MainWindow)Application.Current.MainWindow;
                if (user.role == "Admin")
                {
                    mw.MainFrame.Navigate(new AdminPage());
                    mw.TbPageTitle.Text = "Панель администратора";
                }
                else
                {
                    mw.MainFrame.Navigate(new UserPage(user));
                    mw.TbPageTitle.Text = "Личный кабинет";
                }
            }
        }

        public void CaptchaSwitch()
        {
            bool showCaptcha = captcha.Visibility != Visibility.Visible;
            captcha.Visibility = showCaptcha ? Visibility.Visible : Visibility.Hidden;
            captchaInput.Visibility = showCaptcha ? Visibility.Visible : Visibility.Hidden;
            submitCaptcha.Visibility = showCaptcha ? Visibility.Visible : Visibility.Hidden;

            TextBoxLogin.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            lblLoginHint.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            PasswordBox.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            lblPassHint.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            ButtonEnter.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            ButtonReg.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
            ButtonChangePassword.Visibility = showCaptcha ? Visibility.Hidden : Visibility.Visible;
        }

        public void CaptchaChange()
        {
            string allow = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z," +
                           "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z,1,2,3,4,5,6,7,8,9,0";
            var ar = allow.Split(',');
            var rnd = new Random();
            var pwd = new StringBuilder();
            for (int i = 0; i < 6; i++) pwd.Append(ar[rnd.Next(ar.Length)]);
            captcha.Text = pwd.ToString();
            captchaInput.Clear();
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                CaptchaChange();
            }
            else
            {
                MessageBox.Show("Капча введена успешно, продолжите авторизацию", "Успех");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new RegPage());

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new ChangePassPage());
    }
}