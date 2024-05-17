using HomewOurK.Domain.Entities;
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
using WpfHomewOurK.Controls;

namespace WpfHomewOurK.Pages
{
	/// <summary>
	/// Логика взаимодействия для SearchGroupPage.xaml
	/// </summary>
	public partial class SearchGroupPage : Page
	{
		List<Group> groups = [];
		List<GroupControl> controls = [];
		MainWindow _mainWindow;

		public SearchGroupPage(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			LoadGroups();
		}

		private async void LoadGroups()
		{
			var groups = new List<Group>();

			var httpUser = new HttpHelper<User>(_mainWindow, "api/Users/GetUser");
			var user = await httpUser.GetReqAsync();
			if (user != null)
			{
				var httpGroups = new HttpHelper<List<Group>>(_mainWindow, $"api/UsersGroups/GetGroupsWithoutUser?userId={user.Id}");
				groups = await httpGroups.GetReqAsync() ?? [];
			}

			foreach (var group in groups)
			{
				controls.Add(new GroupControl(group));
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			GroupsPanel.Children.Clear();
			foreach (var groupCtrl in controls)
			{
				if (groupCtrl.Group.UniqGroupName != null && 
					!string.IsNullOrEmpty(GroupUniqName.Text) &&
					groupCtrl.Group.UniqGroupName.ToLower().Contains(GroupUniqName.Text.ToLower()))
					GroupsPanel.Children.Add(groupCtrl);
			}
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}
	}
}
