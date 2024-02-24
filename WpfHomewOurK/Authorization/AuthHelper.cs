using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace WpfHomewOurK.Authorization
{
	internal class AuthHelper
	{
		private readonly MainWindow _mainWindow;

		public bool IsAuthorize { get; set; }

		public AuthHelper(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			string jsonText = File.ReadAllText(_mainWindow.path);
			DeserializeUser(jsonText);
		}

		public User? DeserializeUser(string authUser)
		{
			AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(authUser);
			if (desUser == null)
			{
				IsAuthorize = false;
				return null;
			}

			IsAuthorize = desUser.Authorize;
			var user = new User
			{
				Email = desUser.Email ?? "",
				Password = desUser.Password,
				Username = "qwerty",
				Firstname = "qwerty",
				Surname = "qwerty"
			};
			return user;
		}

		public async void AuthUserAsync(User user)
		{
			try
			{
				using var client = new HttpClient();

				var url = "https://localhost:7228/api/Users/Login";
				var data = JsonConvert.SerializeObject(user); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					IsAuthorize = true;
					_mainWindow.MainContent.Content = new MainControl(_mainWindow);
				}
				else
				{
					MessageBox.Show($"Error: {response} \n  {data}");
					IsAuthorize = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				IsAuthorize = false;
			}
			finally
			{
				_mainWindow.isAuthorize = IsAuthorize;
				var authUser = new AuthUser
				{
					Authorize = IsAuthorize,
					Email = user.Email,
					Password = user.Password
				};

				var jsonUser = JsonConvert.SerializeObject(authUser);

				using var sw = new StreamWriter(_mainWindow.path);
				sw.Write(jsonUser);
			}
		}
	}

	public class AuthUser
	{
		public string? Email { get; set; }
		public string? Password { get; set; }
		public bool Authorize { get; set; }
	}
}