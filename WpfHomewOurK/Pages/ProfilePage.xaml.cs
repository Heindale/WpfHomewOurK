using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
using WpfHomewOurK.Authorization;

namespace WpfHomewOurK.Pages
{
	/// <summary>
	/// Логика взаимодействия для ProfilePage.xaml
	/// </summary>
	public partial class ProfilePage : Page
	{
		private readonly MainWindow _mainWindow;
		private const string _getUserUrl = "api/Users/GetUser";
		private const string _baseUsersUrl = "api/Users";

		public ProfilePage(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			LoadProfileInfoAsync();
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			File.WriteAllText(_mainWindow.path, string.Empty);
			_mainWindow.MainContent.Content = new AuthControl(_mainWindow);
		}

		private async void LoadProfileInfoAsync()
		{
			HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _getUserUrl);
			var userTask = httpHelper.GetReqAsync();

			User? user = await userTask;

			if (user != null)
			{
				Firstname.Text = user.Firstname;
				Surname.Text = user.Surname;
				Username.Text = user.Username;
				Email.Text = user.Email;
			}
		}

		private async void EditUserAsync()
		{
			HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _baseUsersUrl);
			var response = await httpHelper.PatchReqAsync(new User
			{
				Email = Email.Text,
				Firstname = Firstname.Text,
				Surname = Surname.Text,
				Username = Username.Text,
			});

			if (response != null)
			{
				LoadProfileInfoAsync();
			}

			MessageBox.Show("Данные были изменены", "Изменение данных", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			EditUserAsync();
		}

		private async void DeleteUserAsync(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить аккаунт?", "Удаление аккаунта", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
			if (result == MessageBoxResult.Yes)
			{
				HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _baseUsersUrl);
				var response = await httpHelper.DeleteReqAsync();
				if (response != null)
				{
					MessageBox.Show("Аккаунт удален", "Удаление аккаунта", MessageBoxButton.OK, MessageBoxImage.Information);
					Logout_Click(sender, e);
				}
				else
					MessageBox.Show("Аккаунт не был удален", "Удаление аккаунта", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteUserAsync(sender, e);
		}
	}
}