using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
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
			try
			{
				using var client = new HttpClient();

				var url = "https://localhost:7228/api/Users/Register";
				var data = JsonConvert.SerializeObject(user); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					var authControl = new AuthControl(_mainWindow);
					authControl.Login.Text = Login.Text;
					authControl.Password.Password = Password.Password;
					_mainWindow.MainContent.Content = authControl;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
			}
		}
	}
}