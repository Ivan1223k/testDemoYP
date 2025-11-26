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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var user = Entities.GetContext().Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                var userRole = Entities.GetContext().User_Roles
                    .FirstOrDefault(ur => ur.ID_User == user.ID_User)?.Roles;

                if (userRole != null)
                {
                    string userName = $"{user.LastName} {user.FirstName} {user.Patronymic}";
                    string roleName = userRole.Role_Name;

                    switch (userRole.Role_Name)
                    {
                        case "Авторизированный клиент":
                            NavigationService.Navigate(new ProductsPage(false, false, false, false, userName, roleName));
                            break;
                        case "Менеджер":
                            NavigationService.Navigate(new ProductsPage(true, true, true, false, userName, roleName));
                            break;
                        case "Администратор":
                            NavigationService.Navigate(new AdminPage(userName, roleName));
                            break;
                        default:
                            MessageBox.Show($"Неизвестная роль пользователя: {userRole.Role_Name}");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("У пользователя не назначена роль");
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductPage(false, false, false, false, "Гость", "Гость"));
        }
    }
}