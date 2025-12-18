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
using System.Windows.Shapes;

namespace testDemoYP.pagesFr
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private Tovar _currentProduct;
        public bool IsSaved { get; private set; }

        public AddEditProductWindow(Tovar selectedProduct = null)
        {
            InitializeComponent();

            _currentProduct = selectedProduct ?? new Tovar();

            LoadComboBoxData();
            LoadProductData();

            if (_currentProduct.ID_Tovar == 0)
                this.Title = "Добавление нового товара";
            else
                this.Title = "Редактирование товара";
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
                SaleTextBox.Text = _currentProduct.Sale?.ToString("F2");
                CountTextBox.Text = _currentProduct.CountOnSklad?.ToString();
                DescriptionTextBox.Text = _currentProduct.Description;
                PhotoTextBox.Text = _currentProduct.Photo;
            }
            else
            {
                PriceTextBox.Text = "0.00";
                CountTextBox.Text = "0";
                UnitTextBox.Text = "шт.";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TitleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите название товара", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleComboBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
                {
                    MessageBox.Show("Введите единицу измерения", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    UnitTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                    !double.TryParse(PriceTextBox.Text, out double price) || price < 0)
                {
                    MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    PriceTextBox.Focus();
                    return;
                }

                if (SupplierComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите поставщика", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    SupplierComboBox.Focus();
                    return;
                }

                if (ManufacturerComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите производителя", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    ManufacturerComboBox.Focus();
                    return;
                }

                if (CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    CategoryComboBox.Focus();
                    return;
                }

                _currentProduct.Title = (int)TitleComboBox.SelectedValue;
                _currentProduct.Edinica = UnitTextBox.Text.Trim();
                _currentProduct.Price = price;
                _currentProduct.Supplier = (int)SupplierComboBox.SelectedValue;
                _currentProduct.Manufacturer = (int)ManufacturerComboBox.SelectedValue;
                _currentProduct.Category = (int)CategoryComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(SaleTextBox.Text) &&
                    double.TryParse(SaleTextBox.Text, out double sale) && sale >= 0 && sale <= 100)
                {
                    _currentProduct.Sale = sale;
                }
                else if (!string.IsNullOrWhiteSpace(SaleTextBox.Text))
                {
                    MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100%", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    SaleTextBox.Focus();
                    return;
                }
                else
                {
                    _currentProduct.Sale = null;
                }

                // Обработка количества
                if (!string.IsNullOrWhiteSpace(CountTextBox.Text) &&
                    int.TryParse(CountTextBox.Text, out int count) && count >= 0)
                {
                    _currentProduct.CountOnSklad = count;
                }
                else if (!string.IsNullOrWhiteSpace(CountTextBox.Text))
                {
                    MessageBox.Show("Количество должно быть положительным числом", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    CountTextBox.Focus();
                    return;
                }
                else
                {
                    _currentProduct.CountOnSklad = 0;
                }

                // Дополнительные поля
                _currentProduct.Description = DescriptionTextBox.Text?.Trim();
                _currentProduct.Photo = PhotoTextBox.Text?.Trim();

                // Сохранение в базу данных
                if (_currentProduct.ID_Tovar == 0)
                {
                    Entities.GetContext().Tovar.Add(_currentProduct);
                }

                Entities.GetContext().SaveChanges();
                IsSaved = true;

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}