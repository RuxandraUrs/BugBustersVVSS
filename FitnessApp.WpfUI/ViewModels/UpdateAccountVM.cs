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
using FitnessApp.WpfUI.Services;
using FitnessApp.WpfUI.Views;

namespace FitnessApp.WpfUI.ViewModels
{
    public class UpdateAccountVM : BaseVM
    {
        private readonly IUserDomain _userDomain;
        private User currentUser;

        public UpdateAccountVM(User user, IUserDomain userDomain)
        {
            _userDomain = userDomain;
            currentUser = user;

            Username = user.Username;
            Email = user.Email;
            HeightCm = user.HeightCm;
            WeightKg = user.WeightKg;
            TargetWeightKg = user.TargetWeightKg;
            DailyCalorieGoal = user.DailyCalorieGoal;

            UpdateAccountCommand = new RelayCommand(UpdateAccount);
            CancelCommand = new RelayCommand(Cancel);
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> GenderOptions { get; } = new()
        {
            "Male",
            "Female",
            "Other"
        };

        private string _gender;
        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        private double? _heightCm;
        public double? HeightCm
        {
            get => _heightCm;
            set { _heightCm = value; OnPropertyChanged(); }
        }

        private double? _weightKg;
        public double? WeightKg
        {
            get => _weightKg;
            set { _weightKg = value; OnPropertyChanged(); }
        }

        private double? _targetWeightKg;
        public double? TargetWeightKg
        {
            get => _targetWeightKg;
            set { _targetWeightKg = value; OnPropertyChanged(); }
        }

        private int? _dailyCalorieGoal;
        public int? DailyCalorieGoal
        {
            get => _dailyCalorieGoal;
            set { _dailyCalorieGoal = value; OnPropertyChanged(); }
        }
        public string ErrorMessage { get; set; }

        public ICommand UpdateAccountCommand { get; }
        public ICommand CancelCommand { get; }

        private void UpdateAccount()
        {
            try
            {
                currentUser.Email = Email;
                currentUser.HeightCm = HeightCm ?? currentUser.HeightCm;
                currentUser.WeightKg = WeightKg ?? currentUser.WeightKg;
                currentUser.TargetWeightKg = TargetWeightKg ?? currentUser.TargetWeightKg;
                currentUser.DailyCalorieGoal = DailyCalorieGoal ?? currentUser.DailyCalorieGoal;

                _userDomain.UpdateUser(currentUser);
                MessageBox.Show("User updated succesfully!!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                UpdateAccount updateAccount = Application.Current.Windows.OfType<UpdateAccount>().FirstOrDefault();
                updateAccount?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            UpdateAccount updateAccount = Application.Current.Windows.OfType<UpdateAccount>().FirstOrDefault();
            updateAccount?.Close();

        }
    }
}
