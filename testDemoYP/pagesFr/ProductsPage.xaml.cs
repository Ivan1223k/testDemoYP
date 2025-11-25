using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace testDemoYP.pagesFr
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private bool _enableSearch;
        private bool _enableSort;
        private bool _enableFilter;
        private bool _isAdminMode;
        private List<Tovar> _currentProducts;

        public ProductsPage(bool enableSearch, bool enableSort, bool enableFilter, bool isAdminMode = false)
        {
            InitializeComponent();
            _enableSearch = enableSearch;
            _enableSort = enableSort;
            _enableFilter = enableFilter;
            _isAdminMode = isAdminMode;

            InitializeControls();
            LoadProducts();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void InitializeControls()
        {
            // Скрываем кнопку выхода в Border для администратора
            if (_isAdminMode)
            {
                LogoutBtn.Visibility = Visibility.Collapsed;
            }

            // Для клиента и гостя блокируем доступ к фильтрам, но оставляем панель видимой
            if (!_enableSearch && !_enableSort && !_enableFilter)
            {
                SearchTextBox.IsEnabled = false;
                SortComboBox.IsEnabled = false;
                FilterComboBox.IsEnabled = false;
                ClearFiltersButton.IsEnabled = false;

                // Делаем элементы полупрозрачными для визуального обозначения блокировки
                SearchTextBox.Opacity = 0.5;
                SortComboBox.Opacity = 0.5;
                FilterComboBox.Opacity = 0.5;
                ClearFiltersButton.Opacity = 0.5;

                // Добавляем подсказку
                SearchTextBox.ToolTip = "Функция поиска недоступна для вашей роли";
                SortComboBox.ToolTip = "Функция сортировки недоступна для вашей роли";
                FilterComboBox.ToolTip = "Функция фильтрации недоступна для вашей роли";
                ClearFiltersButton.ToolTip = "Функция очистки фильтров недоступна для вашей роли";

                return;
            }

            // Для менеджера/админа настраиваем доступные функции
            if (!_enableSearch)
            {
                SearchTextBox.IsEnabled = false;
                SearchTextBox.Opacity = 0.5;
                SearchTextBox.ToolTip = "Функция поиска недоступна";
            }

            if (!_enableSort)
            {
                SortComboBox.IsEnabled = false;
                SortComboBox.Opacity = 0.5;
                SortComboBox.ToolTip = "Функция сортировки недоступна";
            }

            if (!_enableFilter)
            {
                FilterComboBox.IsEnabled = false;
                FilterComboBox.Opacity = 0.5;
                FilterComboBox.ToolTip = "Функция фильтрации недоступна";
            }

            // Заполняем фильтры категорий (только если разрешено)
            if (_enableFilter)
            {
                var categories = Entities.GetContext().Category.ToList();
                FilterComboBox.Items.Add("Все категории");
                foreach (var category in categories)
                {
                    FilterComboBox.Items.Add(category.CategoryName);
                }
                FilterComboBox.SelectedIndex = 0;
            }
            else
            {
                FilterComboBox.Items.Add("Все категории");
                FilterComboBox.SelectedIndex = 0;
            }

            // Устанавливаем сортировку по умолчанию (только если разрешено)
            if (_enableSort)
            {
                SortComboBox.SelectedIndex = 0;
            }
        }

        // Остальные методы без изменений...
        private void LoadProducts()
        {
            _currentProducts = Entities.GetContext().Tovar.ToList();
            UpdateProducts();
        }

        private void UpdateProducts()
        {
            if (!IsInitialized)
            {
                return;
            }

            try
            {
                List<Tovar> products = _currentProducts.ToList();

                // Поиск по названию (только если разрешено)
                if (_enableSearch && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    products = products.Where(x => x.Title1.TitleName.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }

                // Фильтрация по категории (только если разрешено)
                if (_enableFilter && FilterComboBox.SelectedIndex > 0)
                {
                    string selectedCategory = FilterComboBox.SelectedItem.ToString();
                    products = products.Where(x => x.Category1.CategoryName == selectedCategory).ToList();
                }

                // Сортировка (только если разрешено)
                if (_enableSort)
                {
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            products = products.OrderBy(x => x.Title1.TitleName).ToList();
                            break;
                        case 2:
                            products = products.OrderByDescending(x => x.Title1.TitleName).ToList();
                            break;
                        case 3:
                            products = products.OrderBy(x => x.Price).ToList();
                            break;
                        case 4:
                            products = products.OrderByDescending(x => x.Price).ToList();
                            break;
                    }
                }

                ProductsListView.ItemsSource = products;
            }
            catch (Exception)
            {
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_enableSearch) UpdateProducts();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableSort) UpdateProducts();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableFilter) UpdateProducts();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enableSearch) SearchTextBox.Text = "";
            if (_enableSort) SortComboBox.SelectedIndex = 0;
            if (_enableFilter) FilterComboBox.SelectedIndex = 0;
        }
    }
}