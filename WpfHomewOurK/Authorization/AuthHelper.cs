﻿using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace WpfHomewOurK.Authorization
{
	internal class AuthHelper
	{
		private readonly MainWindow _mainWindow;
		private string _cookie;

		public bool IsAuthorize { get; set; }

		public AuthHelper(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			string jsonText = File.ReadAllText(_mainWindow.path);
			DeserializeUser(jsonText);
		}

		public AuthUser? DeserializeAuthUser(string authUser)
		{
			AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(authUser);
			if (desUser == null)
			{
				IsAuthorize = false;
				return null;
			}
			return desUser;
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

		public async Task<bool> AuthUserAsync(User user)
		{
			try
			{
				var cookieContainer = new CookieContainer();
				using var handler = new HttpClientHandler { CookieContainer = cookieContainer };

				using var client = new HttpClient(handler);

				var url = "https://localhost:7228/api/Users/Login";
				var data = JsonConvert.SerializeObject(user); // JSON строка
				var reqContent = new StringContent(data, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url, reqContent).ConfigureAwait(true);

				if (response.IsSuccessStatusCode)
				{
					IsAuthorize = true;

					// Получение cookie из ответа и сохранение их в CookieContainer
					foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://localhost:7228")))
					{
						if (cookie.Name == ".AspNetCore.Cookies")
						{
							string cookieValue = cookie.Value;
							// Используйте значение cookie по вашему усмотрению
							//Console.WriteLine($"Value of .AspNetCore.Cookies: {cookieValue}");
							_cookie = cookieValue;
						}
					}

					//_mainWindow.MainContent.Content = new MainControl(_mainWindow);
				}
				else
				{
					MessageBox.Show($"Error: {response} \n  {data}");
					IsAuthorize = false;
				}
				return IsAuthorize;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Exception: {ex.Message}");
				IsAuthorize = false;
				return IsAuthorize;
			}
			finally
			{
				_mainWindow.isAuthorize = IsAuthorize;

				string jsonText1 = File.ReadAllText(_mainWindow.path);
				AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(jsonText1);

				int lastGroupId = desUser != null ? desUser.LastGroupId : 0;

				var httpHelper = new HttpHelper<GroupsUsers>(_mainWindow, $"api/UsersGroups/GetGroupsUsers?groupId={lastGroupId}&userId={user.Id}");
				var groupsUsers = await httpHelper.GetReqAsync();

				var authUser = new AuthUser
				{
					Id = user.Id,
					Role = groupsUsers != null ? groupsUsers.Role : Role.None,
					Authorize = IsAuthorize,
					Email = user.Email,
					Password = user.Password,
					Cookie = _cookie,
					LastGroupId = lastGroupId
				};

				var jsonUser = JsonConvert.SerializeObject(authUser);

				using var sw = new StreamWriter(_mainWindow.path);
				sw.Write(jsonUser);

			}
		}
	}

	public class AuthUser
	{
		public int Id { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? Cookie { get; set; }
		public bool Authorize { get; set; }
		public int LastGroupId { get; set; }
		public Role Role { get; set; }
	}
}