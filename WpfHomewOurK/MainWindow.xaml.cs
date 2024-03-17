using HomewOurK.Domain.Entities;
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
		public readonly string path = "..\\..\\..\\Authorization\\User.json";
		public readonly string url = "https://localhost:7228/";
		public int userId;
		private bool addedNewPost = false;

		public MainWindow()
		{
			InitializeComponent();

			var authHelper = new AuthHelper(this);
			isAuthorize = authHelper.IsAuthorize;

			if (!isAuthorize)
				MainContent.Content = new AuthControl(this);
			else
			{
				string jsonText = File.ReadAllText(path);

				User? user = authHelper.DeserializeUser(jsonText);

				if (user == null || !authHelper.IsAuthorize)
					MainContent.Content = new AuthControl(this);
				else
				{
					authHelper.AuthUserAsync(user);
					MainContent.Content = new MainControl(this);
					LoadGroupsAsync();
				}
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

				loadDataFromDb.LoadGroupElementEntityAsync<Teacher>(_getTeachersUrl, 1.ToString());

				loadDataFromDb.LoadGroupElementEntityAsync<Subject>(_getSubjectsUrl, 1.ToString());

				//var httpSubjectHelper = new HttpHelper<List<Subject>>(this, _getSubjectsUrl + userId.ToString());
				//var subjectsTask = httpSubjectHelper.GetReqAsync();
				//List<Subject>? subjects = await subjectsTask;

				//if (subjects != null)
				//{
				//	var localSubjects = context.Subjects.ToList();

				//	foreach (var subject in subjects)
				//	{
				//		if (localSubjects.FirstOrDefault(t => t.Id == subject.Id && t.GroupId == subject.GroupId) == null)
				//		{
				//			context.Subjects.Add(subject);
				//			addedNewPost = true;
				//		}
				//	}
				//}
			}
		}
	}
}