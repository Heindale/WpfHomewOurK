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
using System.Windows.Shapes;
using WpfHomewOurK.Controls;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для MemberWindow.xaml
	/// </summary>
	public partial class MemberWindow : Window
	{
		public bool isOpen = true;
		public bool isExclude = false;
		Role _role;
		User _user;
		MainWindow _mainWindow;

		public MemberWindow(User user, MainWindow mainWindow)
		{
			InitializeComponent();
			_user = user;
			_mainWindow = mainWindow;
			Username.Text = user.Username;
			Name.Content = "Имя: " + user.Firstname;
			Surname.Content = "Фамилия: " + user.Surname;
			Init();
		}

		private async void Init()
		{
			HttpHelper<GroupsUsers> httpHelper = new(_mainWindow,
				$"api/UsersGroups/GetGroupsUsers?groupId={CurrentUser.GetGroupId(_mainWindow)}&userId={_user.Id}");
			var groupsUsers = await httpHelper.GetReqAsync();
			if (groupsUsers != null)
			{
				_role = groupsUsers.Role;

				if (_role == Role.HomeworkCreator)
					DeleteRole.Visibility = Visibility.Visible;
				else if (_role == Role.GroupCreator)
					Edit.Visibility = Visibility.Collapsed;
				else
					AddRole.Visibility = Visibility.Visible;
			}
		}

		public void Exclude()
		{
			isExclude = true;
		}

		private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
			isOpen = false;
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			Exclude();
			Close();
		}

		private void DeleteRole_Click(object sender, RoutedEventArgs e)
		{

		}

		private void AddRole_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
