using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using WpfHomewOurK.Authorization;

namespace WpfHomewOurK
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public bool isAuthorize = false;
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
		public readonly string path = "..\\..\\..\\Authorization\\User.json";
		public readonly string url = "https://localhost:7228/";
		public int userId;

		public MainWindow()
		{
			InitializeComponent();

			UpdateDataFromLocalDb(this);
		}

		public void UpdateDataFromLocalDb(MainWindow mainWindow)
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
					UserIsAuthorized(authHelper, user);
				}
			}
		}

		private async void UserIsAuthorized(AuthHelper authHelper, User user)
		{

			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(this);
			await loadDataFromDb.LoadDataAsync(paths);
			string jsonText1 = File.ReadAllText(path);
			AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(jsonText1);
			await authHelper.AuthUserAsync(user);
			var mainControl = new MainControl(this);
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