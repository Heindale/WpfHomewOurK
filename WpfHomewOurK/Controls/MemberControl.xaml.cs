using HomewOurK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace WpfHomewOurK.Controls
{
	/// <summary>
	/// Логика взаимодействия для MemberControl.xaml
	/// </summary>
	public partial class MemberControl : UserControl
	{
		public User Member { get; set; }
		private Role _role = Role.None;
		private MainWindow _mainWindow;

		public MemberControl(User member, MainWindow mainWindow, Role? role)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			Member = member;
			Init();
			RoleVerification(role);
		}

		private async void Init()
		{
			HttpHelper<GroupsUsers> httpHelper = new(_mainWindow,
				$"api/UsersGroups/GetGroupsUsers?groupId={CurrentUser.GetGroupId(_mainWindow)}&userId={Member.Id}");
			var groupsUsers = await httpHelper.GetReqAsync();
			if (groupsUsers != null)
				_role = groupsUsers.Role;

			switch (_role)
			{
				case Role.None:
					break;
				case Role.HomeworkCreator:
					RoleName.Text = "Создатель домашних заданий - ";
					RoleName.Foreground = new SolidColorBrush(Color.FromRgb(50, 150, 50));
					break;
				case Role.GroupCreator:
					RoleName.Text = "Создатель группы - "; 
					RoleName.Foreground = new SolidColorBrush(Color.FromRgb(10, 100, 125));
					break;
				default:
					break;
			}

			Info.Content = "@" + Member.Username;
			Name.Text = Member.Surname + " " + Member.Firstname;
		}

		private void RoleVerification(Role? role)
		{
			var currentRole = role ?? CurrentUser.GetRole(_mainWindow);
			if (currentRole != Role.GroupCreator)
			{
				Info.IsEnabled = false;
			}
		}

		private void Info_Click(object sender, RoutedEventArgs e)
		{
			var memberWindow = new MemberWindow(Member, _mainWindow);
			memberWindow.Show();
			WaitingAsync(memberWindow);
		}

		private async void WaitingAsync(MemberWindow memberWindow)
		{
			while (memberWindow.isOpen)
			{
				if (memberWindow.isExclude)
				{
					Visibility = Visibility.Collapsed;
					return;
				}
				await Task.Delay(100);
			}
		}
	}
}
