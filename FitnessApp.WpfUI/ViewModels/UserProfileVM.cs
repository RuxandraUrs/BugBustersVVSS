using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.BusinessLogic.Services;
using FitnessApp.Models;
using FitnessApp.WpfUI.Views;
using FitnessApp.WpfUI.Services;
using Microsoft.Win32;

namespace FitnessApp.WpfUI.ViewModels
{
    public class UserProfileVM : BaseVM
    {
        private readonly IUserDomain _userDomain;
        private FitnessCalculator _fitnessCalculator;
        private User currentUser;

        public UserProfileVM(User user, IUserDomain userDomain)
        {
            _userDomain = userDomain;
            _fitnessCalculator = new FitnessCalculator(_userDomain);
            currentUser = user;

            UpdateAccountCommand = new RelayCommand(UpdateAccount);
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            LogOutCommand = new RelayCommand(LogOut);
        }

        public string Username => currentUser?.Username ?? string.Empty;
        public double Height => currentUser?.HeightCm ?? 0;
        public double CurrentWeight => currentUser?.WeightKg ?? 0;
        public double TargetWeight => currentUser?.TargetWeightKg ?? 0;
        public int Age         {
            get
            {
                if (currentUser == null)
                    return 0;
                var today = DateTime.Today;
                var age = today.Year - currentUser.BirthDate.Year;
                if (currentUser.BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string WelcomeMessage => $"Welcome, {Username}!";
        public string ErrorMessage { get; set; }
        public double UserBMI => _userDomain.CalculateBMI(Username);
        public double UserBMR => _userDomain.CalculateBMR(Username);
        public string BMICategory => _fitnessCalculator.GetBMICategory(UserBMI);

        public void UpdateAccount()
        {
            UpdateAccount updateAccount = new UpdateAccount(currentUser, _userDomain);
            updateAccount.ShowDialog();
        }

        public void DeleteAccount()
        {
            //_userDomain.DeleteUser(Username);

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    //_userDomain.DeleteUser(currentUser);
                    MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = mainWindow;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public void LogOut()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = mainWindow;
        }

        public ICommand UpdateAccountCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand LogOutCommand { get; }
    }
}
