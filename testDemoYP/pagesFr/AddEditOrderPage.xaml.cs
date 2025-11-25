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
    /// Логика взаимодействия для AddEditOrderPage.xaml
    /// </summary>
    public partial class AddEditOrderPage : Page
    {
        private Order _currentOrder;

        public AddEditOrderPage(Order selectedOrder)
        {
            InitializeComponent();
            _currentOrder = selectedOrder ?? new Order();
            LoadComboBoxData();
            LoadOrderData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                StatusComboBox.ItemsSource = Entities.GetContext().Status.ToList();
                AddressComboBox.ItemsSource = Entities.GetContext().Address.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadOrderData()
        {
            if (_currentOrder.ID_order != 0)
            {
                OrderDatePicker.SelectedDate = _currentOrder.DateOrder;
                DeliveryDatePicker.SelectedDate = _currentOrder.DateDel;
                StatusComboBox.SelectedValue = _currentOrder.Status;
                AddressComboBox.SelectedValue = _currentOrder.AddressPVZ;
            }
            else
            {
                OrderDatePicker.SelectedDate = DateTime.Now;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrderDatePicker.SelectedDate == null || StatusComboBox.SelectedItem == null || AddressComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните обязательные поля");
                    return;
                }

                _currentOrder.DateOrder = OrderDatePicker.SelectedDate;
                _currentOrder.DateDel = DeliveryDatePicker.SelectedDate;
                _currentOrder.Status = ((Status)StatusComboBox.SelectedItem).ID_Status;
                _currentOrder.AddressPVZ = ((Address)AddressComboBox.SelectedItem).ID;

                if (_currentOrder.ID_order == 0)
                {
                    Entities.GetContext().Order.Add(_currentOrder);
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