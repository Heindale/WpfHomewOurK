using HomewOurK.Domain.Entities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using WpfHomewOurK.Authorization;
using WpfHomewOurK.Controls;

namespace WpfHomewOurK
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public bool isAuthorize = false;
		private const string _connectionUrl = "api/Connection";
		private const string _getUserUrl = "api/Users/GetUser";
		private const string _getGroupsUrl = "api/Groups/GetGroups?userId=";
		private const string _getTeachersUrl = "api/Teachers/GetTeachers?groupId=";
		private const string _getSubjectsUrl = "api/Subjects/GetSubjects?groupId=";
		private const string _getAttachmentsUrl = "api/Attachments/GetAttachments?groupId=";
		private const string _getHomeworksUrl = "api/Homeworks/GetHomeworks?groupId=";
		public string[] paths =
					{
						_getUserUrl,
						_getGroupsUrl,
						_getTeachersUrl,
						_getSubjectsUrl,
						_getAttachmentsUrl,
						_getHomeworksUrl
					};
		//public readonly string path = "..\\..\\..\\Authorization\\User.json";
		public readonly string path = "User.json";
		public readonly string url = "https://localhost:7228/";
		public int userId;
		private bool isLoading = true;

		public MainWindow()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			Height = 375;
			Width = 375;
			InitializeAsync();
		}

		private async void InitializeAsync()
		{
			if (!await CheckServerConnectionAsync())
			{
				MessageBox.Show("Соединение с сервером не было установлено :( \nПроверьте подключение к сети и повторите попытку.");
				Close(); // закрыть приложение или выполнить другие действия
				return;
			}

			await UpdateDataFromLocalDb(this);
			Background = Brushes.Transparent;
			loadingControl.Visibility = Visibility.Collapsed; // Скрываем контрол загрузки после завершения загрузки данных
			Height = 500;
			Width = 800;
			MinWidth = 600;
			Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
			Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;

		}

		private async Task<bool> CheckServerConnectionAsync()
		{
			try
			{
				using (var client = new HttpClient())
				{
					var response = await client.GetAsync(url + _connectionUrl);
					return response.IsSuccessStatusCode;
				}
			}
			catch
			{
				return false;
			}
		}


		public async Task UpdateDataFromLocalDb(MainWindow mainWindow)
		{
			var authHelper = new AuthHelper(mainWindow);
			isAuthorize = authHelper.IsAuthorize;

			if (!isAuthorize)
				MainContent.Content = new AuthControl(mainWindow);
			else
			{
				string jsonText = File.ReadAllText(path);

				User? user = authHelper.DeserializeUser(jsonText);

				if (user == null || !authHelper.IsAuthorize)
					MainContent.Content = new AuthControl(mainWindow);
				else
				{
					await UserIsAuthorized(authHelper, user);
				}
			}
			isLoading = false;
		}

		private async Task UserIsAuthorized(AuthHelper authHelper, User user)
		{
			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(this);
			await loadDataFromDb.LoadDataAsync(paths);
			string jsonText1 = File.ReadAllText(path);
			AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(jsonText1);
			await authHelper.AuthUserAsync(user);
			var mainControl = new MainControl(this, desUser != null ? desUser.Role : Role.None);
			MainContent.Content = mainControl;
			if (desUser != null && desUser.LastGroupId > 0)
			{
				mainControl.Groups.SelectedItem = mainControl._groups.First(g => g.Id == desUser.LastGroupId);
			}
		}

		private async void LoadGroupsAsync()
		{
			User? user;

			using (ApplicationContext context = new ApplicationContext())
			{
				HttpHelper<User> httpUserHelper = new HttpHelper<User>(this, _getUserUrl);
				var userTask = httpUserHelper.GetReqAsync();
				user = await userTask;
			}

			if (user != null)
			{
				userId = user.Id;

				LoadDataFromDb loadDataFromDb = new LoadDataFromDb(this);
				loadDataFromDb.LoadBaseEntityAsync<Group>(_getGroupsUrl, userId.ToString());

				var groupsIds = loadDataFromDb.GroupsId;
				for (int i = 0; i < groupsIds.Count; i++)
				{
					var groupId = groupsIds[i];
					loadDataFromDb.LoadGroupElementEntityAsync<Teacher>(_getTeachersUrl, groupId.ToString());
					loadDataFromDb.LoadGroupElementEntityAsync<Subject>(_getSubjectsUrl, groupId.ToString());
					loadDataFromDb.LoadGroupElementEntityAsync<Attachment>(_getAttachmentsUrl, groupId.ToString());
					loadDataFromDb.LoadGroupElementEntityAsync<Homework>(_getHomeworksUrl, groupId.ToString());
				}
			}
		}
	}
}
