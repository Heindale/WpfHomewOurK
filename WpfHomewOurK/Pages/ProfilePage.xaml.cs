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
			try
			{
				var cookieContainer = new CookieContainer();
				string jsonText = File.ReadAllText(_mainWindow.path);
				var authHelper = new AuthHelper(_mainWindow);
				var authUser = authHelper.DeserializeAuthUser(jsonText);

				var cookieValue = authUser != null ? authUser.Cookie : "";

				cookieContainer.Add(new Uri("https://localhost:7228/"), new Cookie(".AspNetCore.Cookies", cookieValue));
				var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				using var client = new HttpClient(handler);

				var response = await client.GetAsync("https://localhost:7228/api/Groups").ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

					// Десериализация JSON в объект Groups
					var groups = JsonConvert.DeserializeObject<List<Group>>(content);

					// Добавьте свой код для работы с полученными данными (groups)
					string gs = "";
					if (groups != null)
					{
						foreach (var group in groups)
						{
							gs += "\n" + group.Name + "\n" + group.Id + "\n" + group.GroupType + "\n" + group.Grade + "\n----------------------------------\n";
						}
					}
					TestTextBlock.Text = gs;
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