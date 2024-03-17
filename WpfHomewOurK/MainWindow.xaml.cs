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
		public readonly string path = "..\\..\\..\\Authorization\\User.json";
		public readonly string url = "https://localhost:7228/";
		public int userId;

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
			using (ApplicationContext context = new ApplicationContext())
			{
				HttpHelper<User> httpHelper = new HttpHelper<User>(this, _getUserUrl);
				var userTask = httpHelper.GetReqAsync();
				User? user = await userTask;

				if (user != null)
				{
					userId = user.Id;

					var httpHelper2 = new HttpHelper<List<Group>>(this, _getGroupsUrl + userId.ToString());
					var groupsTask = httpHelper2.GetReqAsync();
					List<Group>? groups = await groupsTask;

					if (groups != null)
					{
						context.Groups.AddRange(groups);
						context.SaveChanges();
					}
				}
			}
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