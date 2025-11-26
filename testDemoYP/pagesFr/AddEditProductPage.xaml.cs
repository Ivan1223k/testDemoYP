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
                SaleTextBox.Text = _currentProduct.Sale?.ToString();
                CountTextBox.Text = _currentProduct.CountOnSklad?.ToString();
                DescriptionTextBox.Text = _currentProduct.Description;
                PhotoTextBox.Text = _currentProduct.Photo;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TitleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите название");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
                {
                    MessageBox.Show("Введите единицу измерения");
                    return;
                }
                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !double.TryParse(PriceTextBox.Text, out double price))
                {
                    MessageBox.Show("Введите корректную цену");
                    return;
                }
                if (SupplierComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите поставщика");
                    return;
                }
                if (ManufacturerComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите производителя");
                    return;
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию");
                    return;
                }

                _currentProduct.Title = (int)TitleComboBox.SelectedValue;
                _currentProduct.Edinica = UnitTextBox.Text;
                _currentProduct.Price = price;
                _currentProduct.Supplier = (int)SupplierComboBox.SelectedValue;
                _currentProduct.Manufacturer = (int)ManufacturerComboBox.SelectedValue;
                _currentProduct.Category = (int)CategoryComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(SaleTextBox.Text) && double.TryParse(SaleTextBox.Text, out double sale))
                {
                    _currentProduct.Sale = sale;
                }
                else
                {
                    _currentProduct.Sale = null;
                }

                if (!string.IsNullOrWhiteSpace(CountTextBox.Text) && int.TryParse(CountTextBox.Text, out int count))
                {
                    _currentProduct.CountOnSklad = count;
                }
                else
                {
                    _currentProduct.CountOnSklad = null;
                }

                _currentProduct.Description = DescriptionTextBox.Text;
                _currentProduct.Photo = PhotoTextBox.Text;

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