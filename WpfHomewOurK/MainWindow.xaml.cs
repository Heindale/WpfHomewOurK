using HomewOurK.Domain.Entities;
using System.IO;
using System.Text;
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
				//var httpGroupHelper = new HttpHelper<List<Group>>(this, _getGroupsUrl + userId.ToString());
				//var groupsTask = httpGroupHelper.GetReqAsync();
				//List<Group>? groups = await groupsTask;

				//if (groups != null)
				//{
				//	var localGroups = context.Groups.ToList();

				//	foreach (var group in groups)
				//	{
				//		if (localGroups.FirstOrDefault(g => g.Id == group.Id) == null)
				//		{
				//			context.Groups.Add(group);
				//			addedNewPost = true;
				//		}
				//	}
				//}

				//var httpTeacherHelper = new HttpHelper<List<Teacher>>(this, _getTeachersUrl + userId.ToString());
				//var teachersTask = httpTeacherHelper.GetReqAsync();
				//List<Teacher>? teachers = await teachersTask;

				//if (teachers != null)
				//{
				//	var localTeachers = context.Teachers.ToList();

				//	foreach (var teacher in teachers)
				//	{
				//		if (localTeachers.FirstOrDefault(t => t.Id == teacher.Id && t.GroupId == teacher.GroupId) == null)
				//		{
				//			context.Teachers.Add(teacher);
				//			addedNewPost = true;
				//		}
				//	}
				//}

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
			//if (addedNewPost)
			//	context.SaveChanges();

			//addedNewPost = false;
		}

		//Тестовый метод получения списка групп
		//private  async void LoadGroupsAsync()
		//{
		//	try
		//	{
		//		using var client = new HttpClient();
		//		var response = await client.GetAsync("https://localhost:7228/api/Groups").ConfigureAwait(true);

		//		if (response.IsSuccessStatusCode)
		//		{
		//			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

		//			// Десериализация JSON в объект Groups
		//			var groups = JsonConvert.DeserializeObject<List<Group>>(content);

		//			// Добавьте свой код для работы с полученными данными (groups)
		//			string gs = "";
		//			if (groups != null)
		//			{
		//				foreach (var group in groups)
		//				{
		//					gs += "\n" + group.Name + "\n" + group.Id + "\n" + group.GroupType + "\n" + group.Grade + "\n----------------------------------\n";
		//				}
		//			}
		//			//TestTextBlock.Text = gs;
		//		}
		//		else
		//		{
		//			MessageBox.Show($"Error: {response.StatusCode}");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show($"Exception: {ex.Message}");
		//	}
		//}
	}
}