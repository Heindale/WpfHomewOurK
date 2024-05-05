using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для RegControl.xaml
	/// </summary>
	public partial class RegControl : UserControl
	{
		private readonly MainWindow _mainWindow;
		private const string _urlPath = "api/Users/Register";

		public RegControl(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.MainContent.Content = new AuthControl(_mainWindow);
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			AddUserAsync(new User
			{
				Email = Login.Text,
				Username = Username.Text,
				Password = Password.Password,
				Firstname = Firstname.Text,
				Surname = Surname.Text
			});
		}

		private async void AddUserAsync(User user)
		{
			HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _urlPath);

			var response = await httpHelper.PostReqAsync(user);

			if (response != null)
			{
				var authControl = new AuthControl(_mainWindow);
				authControl.Login.Text = Login.Text;
				authControl.Password.Password = Password.Password;
				_mainWindow.MainContent.Content = authControl;
			}
		}

		private void GoOut_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.Close();
		}
	}
}