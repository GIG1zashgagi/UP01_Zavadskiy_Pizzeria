using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Zavadskiy_Pizzeria.Pages 
{
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
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
                    DataGridCategory.ItemsSource = db.menu_categories.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) 
            {
                LoadData();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddCategoryPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var category = button?.DataContext as menu_categories;

            if (category != null)
            {
                NavigationService?.Navigate(new AddCategoryPage(category));
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = DataGridCategory.SelectedItems.Cast<menu_categories>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите записи для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {selectedItems.Count} записей?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new uppizzariaEntities())
                    {
                        foreach (var item in selectedItems)
                        {
                            var entityToDelete = db.menu_categories.Find(item.category_id);
                            if (entityToDelete != null)
                            {
                                db.menu_categories.Remove(entityToDelete);
                            }
                        }

                        db.SaveChanges();
                    }

                    MessageBox.Show("Данные успешно удалены!");
                    LoadData(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}