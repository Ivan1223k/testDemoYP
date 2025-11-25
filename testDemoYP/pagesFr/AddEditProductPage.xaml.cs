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
    /// Логика взаимодействия для AddEditProductPage.xaml
    /// </summary>
    public partial class AddEditProductPage : Page
    {
        private Tovar _currentProduct;

        public AddEditProductPage(Tovar selectedProduct)
        {
            InitializeComponent();
            _currentProduct = selectedProduct ?? new Tovar();
            LoadComboBoxData();
            LoadProductData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                TitleComboBox.ItemsSource = Entities.GetContext().Title.ToList();
                SupplierComboBox.ItemsSource = Entities.GetContext().Postavchick.ToList();
                ManufacturerComboBox.ItemsSource = Entities.GetContext().Manufacturer.ToList();
                CategoryComboBox.ItemsSource = Entities.GetContext().Category.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            if (_currentProduct.ID_Tovar != 0)
            {
                TitleComboBox.SelectedValue = _currentProduct.Title;
                UnitTextBox.Text = _currentProduct.Edinica;
                PriceTextBox.Text = _currentProduct.Price.ToString();
                SupplierComboBox.SelectedValue = _currentProduct.Supplier;
                ManufacturerComboBox.SelectedValue = _currentProduct.Manufacturer;
                CategoryComboBox.SelectedValue = _currentProduct.Category;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TitleComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(UnitTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text) || SupplierComboBox.SelectedItem == null ||
                    ManufacturerComboBox.SelectedItem == null || CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                _currentProduct.Title = ((Title)TitleComboBox.SelectedItem).ID_Title;
                _currentProduct.Edinica = UnitTextBox.Text;
                _currentProduct.Price = double.Parse(PriceTextBox.Text);
                _currentProduct.Supplier = ((Postavchick)SupplierComboBox.SelectedItem).ID_Supplier;
                _currentProduct.Manufacturer = ((Manufacturer)ManufacturerComboBox.SelectedItem).ID_Manufacturer;
                _currentProduct.Category = ((Category)CategoryComboBox.SelectedItem).ID_Category;

                if (_currentProduct.ID_Tovar == 0)
                {
                    Entities.GetContext().Tovar.Add(_currentProduct);
                }

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены успешно");

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}