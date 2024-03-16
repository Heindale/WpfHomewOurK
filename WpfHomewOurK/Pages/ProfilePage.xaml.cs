using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private const string _urlPath = "api/Users/GetUser";

		public ProfilePage(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			LoadProfileInfoAsync();
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.MainContent.Content = new AuthControl(_mainWindow);
		}

		private async void LoadProfileInfoAsync()
		{
			HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _urlPath);
			var userTask = httpHelper.GetContentAsync();

			User? user = await userTask;

			if (user != null)
			{
				Firstname.Text = user.Firstname;
				Surname.Text = user.Surname;
				Username.Text = user.Username;
				Email.Text = user.Email;
			}
		}
	}
}