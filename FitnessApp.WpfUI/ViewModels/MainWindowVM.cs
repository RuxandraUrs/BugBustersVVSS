using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessApp.Models;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.WpfUI.Services;
using FitnessApp.WpfUI.Views;

namespace FitnessApp.WpfUI.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        private readonly IUserDomain _userDomain;

        public MainWindowVM(IUserDomain userDomain)
        {
            _userDomain = userDomain;

            SignInCommand = new RelayCommand(SignIn);
            ExitCommand = new RelayCommand(Exit);
            OpenSignUpPageCommand = new RelayCommand(OpenSignUpPage);
            AboutCommand = new RelayCommand(About);
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand SignInCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OpenSignUpPageCommand { get; }
        public ICommand AboutCommand { get; }

        private void SignIn()
        {
            try
            {
                var user = _userDomain.Login(Username, Password);

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UserProfile userProfile = new UserProfile(user, _userDomain);
                userProfile.Show();

                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = userProfile;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message;
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit()
        {
            MainWindow mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow?.Close();
        }

        private void OpenSignUpPage()
        {
            CreateAccount signUpPage = new CreateAccount();
            signUpPage.ShowDialog();
        }

        private void About()
        {
            MessageBox.Show("Developed by Bug Busters\n" +
                "Collaborators:\nHuruba Adriana\nUrs Ruxandra\n" +
                "Vilcu Andreea\nEnache Nicolae", "About FitnessApp", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
