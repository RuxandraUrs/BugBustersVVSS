using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.Models;
using FitnessApp.WpfUI.Services;
using FitnessApp.WpfUI.Views;

namespace FitnessApp.WpfUI.ViewModels
{
    public class CreateAccountVM : BaseVM
    {
        private readonly IUserDomain _userDomain;

        public CreateAccountVM(IUserDomain userDomain)
        {
            _userDomain = userDomain;

            CreateAccountCommand = new RelayCommand(CreateAccount);
            CancelCommand = new RelayCommand(Cancel);
        }

        //Bindable Properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Today;
        public string Gender { get; set; }
        private double? _heightCm;
        public double? HeightCm
        {
            get => _heightCm;
            set
            {
                _heightCm = value;
                OnPropertyChanged();
            }
        }
        private double? _weightKg;
        public double? WeightKg
        {
            get => _weightKg;
            set
            {
                _weightKg = value;
                OnPropertyChanged();
            }
        }

        private double? _targetWeightKg;
        public double? TargetWeightKg
        {
            get => _targetWeightKg;
            set
            {
                _targetWeightKg = value;
                OnPropertyChanged();
            }
        }

        private int? _dailyCalorieGoal;
        public int? DailyCalorieGoal
        {
            get => _dailyCalorieGoal;
            set
            {
                _dailyCalorieGoal = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage { get; private set; }

        public ICommand CreateAccountCommand { get; }
        public ICommand CancelCommand { get; }

        private bool CanCreateAccount()
        {
            return !string.IsNullOrWhiteSpace(Username)
                   && !string.IsNullOrWhiteSpace(Password)
                   && !string.IsNullOrWhiteSpace(Email);
        }

        private void CreateAccount()
        {
            try
            {
                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    BirthDate = BirthDate,
                    Gender = Gender,
                    HeightCm = HeightCm ?? 0,
                    WeightKg = WeightKg ?? 0,
                    TargetWeightKg = TargetWeightKg ?? 0,
                    DailyCalorieGoal = DailyCalorieGoal ?? 0,
                    PasswordHash = Password
                };

                _userDomain.RegisterUser(user);
                MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CreateAccount createAccount = Application.Current.Windows.OfType<CreateAccount>().FirstOrDefault();
                createAccount?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CreateAccount createAccount = Application.Current.Windows.OfType<CreateAccount>().FirstOrDefault();
            createAccount?.Close();

        }
    }
}
