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
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand SignInCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OpenSignUpPageCommand { get; }

        private void SignIn()
        {
            try
            {
                _userDomain.Login(Username, Password);
                //MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow?.Close();

                User user = _userDomain.GetUserByUsername(Username);
                UserProfile userProfile = new UserProfile(user);
                userProfile.Show();
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
    }
}
