using System;
using System.Collections.Generic;
using System.Globalization;
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
        private string _userName;
        private string _userRole;
        private List<Tovar> _allProducts;
        private List<Tovar> _currentProducts;

        public ProductsPage(bool enableSearch, bool enableSort, bool enableFilter, bool isAdminMode = false, string userName = "", string userRole = "")
        {
            InitializeComponent();
            _enableSearch = enableSearch;
            _enableSort = enableSort;
            _enableFilter = enableFilter;
            _isAdminMode = isAdminMode;
            _userName = userName;
            _userRole = userRole;

            InitializeControls();
            LoadProducts();
            DisplayUserInfo();

            Loaded += ProductsPage_Loaded;
        }

        private void DisplayUserInfo()
        {
            // Для администратора скрываем панель пользователя
            if (_isAdminMode)
            {
                UserInfoPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserNameText.Text = _userName;
                UserRoleText.Text = _userRole;
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void InitializeControls()
        {
            if (!_enableSearch && !_enableSort && !_enableFilter)
            {
                SearchTextBox.IsEnabled = false;
                SortComboBox.IsEnabled = false;
                FilterComboBox.IsEnabled = false;
                DiscountFilterComboBox.IsEnabled = false;


                SearchTextBox.Opacity = 0.5;
                SortComboBox.Opacity = 0.5;
                FilterComboBox.Opacity = 0.5;
                DiscountFilterComboBox.Opacity = 0.5;

                SearchTextBox.ToolTip = "Функция поиска недоступна для вашей роли";
                SortComboBox.ToolTip = "Функция сортировки недоступна для вашей роли";
                FilterComboBox.ToolTip = "Функция фильтрации недоступна для вашей роли";
                DiscountFilterComboBox.ToolTip = "Функция фильтрации по скидке недоступна для вашей роли";

                return;
            }

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
                DiscountFilterComboBox.IsEnabled = false;
                DiscountFilterComboBox.Opacity = 0.5;
                DiscountFilterComboBox.ToolTip = "Функция фильтрации по скидке недоступна";
            }

            if (_enableFilter)
            {
                var categories = Entities.GetContext().Category.ToList();
                FilterComboBox.Items.Clear();
                FilterComboBox.Items.Add("Все категории");
                foreach (var category in categories)
                {
                    FilterComboBox.Items.Add(category.CategoryName);
                }
                FilterComboBox.SelectedIndex = 0;
            }
            else
            {
                FilterComboBox.Items.Clear();
                FilterComboBox.Items.Add("Все категории");
                FilterComboBox.SelectedIndex = 0;
            }
        }
        private void ProductsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            _allProducts = Entities.GetContext().Tovar.ToList();
            _currentProducts = _allProducts.ToList();
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
                List<Tovar> products = _allProducts.ToList();

                if (_enableSearch && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    products = products.Where(x => x.Title1.TitleName.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }

                if (_enableFilter && FilterComboBox.SelectedIndex > 0)
                {
                    string selectedCategory = FilterComboBox.SelectedItem.ToString();
                    products = products.Where(x => x.Category1.CategoryName == selectedCategory).ToList();
                }

                if (_enableFilter && DiscountFilterComboBox.SelectedIndex > 0)
                {
                    if (DiscountFilterComboBox.SelectedItem.ToString() == "Скидка > 15%")
                    {
                        products = products.Where(x => x.Sale != null && x.Sale > 14).ToList();
                    }
                }

                _currentProducts = products;

                if (_enableSort)
                {
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            products = products.OrderBy(x => x.CountOnSklad ?? 0).ToList();
                            break;
                        case 2:
                            products = products.OrderByDescending(x => x.CountOnSklad ?? 0).ToList();
                            break;
                    }
                }

                var productsWithColor = products.Select(p => new
                {
                    ID_Tovar = p.ID_Tovar,
                    Title1 = p.Title1,
                    Category1 = p.Category1,
                    Manufacturer1 = p.Manufacturer1,
                    Postavchick = p.Postavchick,
                    Price = p.Price,
                    Sale = p.Sale,
                    Edinica = p.Edinica,
                    CountOnSklad = p.CountOnSklad,
                    Description = p.Description,
                    Photo = p.Photo,
                    OriginalProduct = p,

                    OriginalPriceValue = p.OriginalPriceValue,
                    HasDiscount = p.HasDiscount,
                    DiscountedPriceValue = p.DiscountedPriceValue,

                    BackgroundColor = GetProductBackgroundColor(p)
                }).ToList();

                ProductsListView.ItemsSource = productsWithColor;
            }
            catch (Exception)
            {
            }
        }
        private Brush GetProductBackgroundColor(Tovar product)
        {
            if (product.CountOnSklad == 0 || product.CountOnSklad == null)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADD8E6"));
            }

            if (product.Sale != null && product.Sale > 14)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));
            }

            return new SolidColorBrush(Colors.White);
        }

        public Tovar GetSelectedProduct()
        {
            if (ProductsListView.SelectedItem != null)
            {
                dynamic selectedItem = ProductsListView.SelectedItem;
                return selectedItem.OriginalProduct;
            }
            return null;
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

        private void DiscountFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableFilter) UpdateProducts();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SortComboBox.SelectedIndex = 0;
            FilterComboBox.SelectedIndex = 0;
            DiscountFilterComboBox.SelectedIndex = 0;
            UpdateProducts();
        }

        private void ProductsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_isAdminMode)
            {
                MessageBox.Show("Недостаточно прав для редактирования");
                return;
            }

            Tovar selectedProduct = GetSelectedProduct();
            if (selectedProduct != null)
            {
                var editWindow = new AddEditProductWindow(selectedProduct);
                editWindow.Owner = Window.GetWindow(this);

                bool? result = editWindow.ShowDialog();

                if (result == true && editWindow.IsSaved)
                {
                    LoadProducts();
                    MessageBox.Show("Изменения сохранены успешно!");
                }
            }
        }

    }
}