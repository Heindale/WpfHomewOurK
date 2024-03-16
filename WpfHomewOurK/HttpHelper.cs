using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHomewOurK.Authorization;
using System.IO;

namespace WpfHomewOurK
{
	public class HttpHelper<T>
	{
		private readonly MainWindow _mainWindow;

		private T? _entity;

		private string _urlPath;

		public HttpHelper(MainWindow mainWindow, string urlPath)
		{
			_mainWindow = mainWindow;
			_urlPath = urlPath;
		}

		public async Task<T?> GetContentAsync()
		{
			try
			{
				var cookieContainer = new CookieContainer();
				string jsonText = File.ReadAllText(_mainWindow.path);
				var authHelper = new AuthHelper(_mainWindow);
				var authUser = authHelper.DeserializeAuthUser(jsonText);

				var cookieValue = authUser != null ? authUser.Cookie : "";

				cookieContainer.Add(new Uri(_mainWindow.url), new Cookie(".AspNetCore.Cookies", cookieValue));
				var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				using var client = new HttpClient(handler);

				var response = await client.GetAsync($"{_mainWindow.url}{_urlPath}").ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

					// Десериализация JSON в объект Groups
					var user = JsonConvert.DeserializeObject<T>(content);

					// Добавьте свой код для работы с полученными данными (groups)
					_entity = user;

					return _entity;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
					return _entity;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				return _entity;
			}
		}
	}
}