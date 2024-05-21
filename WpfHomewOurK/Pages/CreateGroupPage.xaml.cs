using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfHomewOurK.Pages
{
	/// <summary>
	/// Логика взаимодействия для CreateGroupPage.xaml
	/// </summary>
	public partial class CreateGroupPage : Page
	{
		MainWindow _mainWindow;

		public CreateGroupPage(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}

		private void CreateGroup_Click(object sender, RoutedEventArgs e)
		{
			CreateGroupAsync();
		}

		private async void CreateGroupAsync()
		{
			HttpHelper<Group> httpGroupHelper = new(_mainWindow, "api/Groups");
			/*var groupResponse =*/ await httpGroupHelper.PostReqAuthAsync(new Group
			{
				Grade = int.Parse(GroupGrade.Text),
				GroupType = GroupType.Text,
				Name = GroupName.Text,
				UniqGroupName = UniqGroupName.Text
			});

			//if (groupResponse != null && groupResponse.IsSuccessStatusCode)
			//{
			//	var content = await groupResponse.Content.ReadAsStringAsync().ConfigureAwait(true);
			//	var group = JsonConvert.DeserializeObject<Group>(content);

			//	if (group != null)
			//	{
			//		var httpUser = new HttpHelper<User>(_mainWindow, "api/Users/GetUser");
			//		var user = await httpUser.GetReqAsync();
			//		if (user != null)
			//		{
			//			HttpHelper<GroupsUsers> httpGroupsUsersHelper = new(_mainWindow, "api/UsersGroups");
			//			await httpGroupsUsersHelper.PostReqAuthAsync(new GroupsUsers
			//			{
			//				GroupId = group.Id,
			//				UserId = user.Id,
			//				Role = Role.GroupCreator
			//			});
			//		}
			//	}
			//}
		}
	}
}
