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

		public async Task<HttpResponseMessage?> PostReqAsync(T entity)
		{
			try
			{
				_entity = entity;
				using var client = new HttpClient();

				var url = $"{_mainWindow.url}{_urlPath}";
				var data = JsonConvert.SerializeObject(_entity); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					return response;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
					return response;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				return null;
			}
		}

		public async Task<HttpResponseMessage?> PostReqAuthAsync(T entity)
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

				_entity = entity;

				var url = $"{_mainWindow.url}{_urlPath}";
				var data = JsonConvert.SerializeObject(_entity); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					return response;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
					return response;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				return null;
			}
		}

		public async Task<HttpResponseMessage?> PatchReqAsync(T entity)
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

				_entity = entity;

				var url = $"{_mainWindow.url}{_urlPath}";
				var data = JsonConvert.SerializeObject(_entity); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PatchAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					return response;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
					return response;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				return null;
			}
		}

		public async Task<HttpResponseMessage?> DeleteReqAsync()
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

				var url = $"{_mainWindow.url}{_urlPath}";
				var data = JsonConvert.SerializeObject(_entity); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.DeleteAsync(url).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					return response;
				}
				else
				{
					MessageBox.Show($"Error: {response}");
					return response;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				return null;
			}
		}

		public async Task<T?> GetReqAsync()
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