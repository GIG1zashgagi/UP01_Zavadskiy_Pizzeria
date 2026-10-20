using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage() { InitializeComponent(); }

        private void BtnTab1_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());
        private void BtnTab2_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new CategoryTabPage());
        private void BtnTab3_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new OrdersTabPage());
        private void BtnTab4_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagrammPage());
    }
}