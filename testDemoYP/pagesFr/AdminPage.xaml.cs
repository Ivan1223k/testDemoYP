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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            ProductsFrame.Navigate(new ProductsPage(true, true, true));
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = Entities.GetContext().Order
                .Include("Status1")
                .Include("Address")
                .ToList();
            OrdersDataGrid.ItemsSource = orders;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductsFrame.Navigate(new AddEditProductPage(null));
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var productsPage = ProductsFrame.Content as ProductsPage;
            if (productsPage != null && productsPage.ProductsListView.SelectedItem != null)
            {
                Tovar selectedProduct = productsPage.ProductsListView.SelectedItem as Tovar;
                if (selectedProduct != null)
                {
                    ProductsFrame.Navigate(new AddEditProductPage(selectedProduct));
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования");
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var productsPage = ProductsFrame.Content as ProductsPage;
            if (productsPage != null && productsPage.ProductsListView.SelectedItem != null)
            {
                Tovar selectedProduct = productsPage.ProductsListView.SelectedItem as Tovar;
                if (selectedProduct != null)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedProduct.Title1.TitleName}'?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Tovar.Remove(selectedProduct);
                            Entities.GetContext().SaveChanges();
                            ProductsFrame.Navigate(new ProductsPage(true, true, true));
                            MessageBox.Show("Товар успешно удален");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении товара");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления");
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditOrderPage(null));
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem != null)
            {
                Order selectedOrder = OrdersDataGrid.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    NavigationService.Navigate(new AddEditOrderPage(selectedOrder));
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem != null)
            {
                Order selectedOrder = OrdersDataGrid.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ №{selectedOrder.ID_order}?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Order.Remove(selectedOrder);
                            Entities.GetContext().SaveChanges();
                            LoadOrders();
                            MessageBox.Show("Заказ успешно удален");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении заказа");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления");
            }
        }
    }
}