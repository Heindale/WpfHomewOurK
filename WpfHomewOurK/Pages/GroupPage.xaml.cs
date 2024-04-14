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
	/// Логика взаимодействия для GroupPage.xaml
	/// </summary>
	public partial class GroupPage : Page
	{
		private MainWindow _mainWindow;
		private MainControl _mainControl;
		private const string _getUserUrl = "api/Users/GetUsers?groupId=";
		private const string _getGroupUrl = "api/Groups/GetGroup?groupId=";

		public GroupPage(MainControl mainControl, MainWindow mainWindow)
		{
			InitializeComponent();
			_mainControl = mainControl;
			_mainWindow = mainWindow;
			LoadMembersAsync();
			LoadGroupDataAsync();
		}

		private async void LoadGroupDataAsync()
		{
			var selectedObject = (Group)_mainControl.Groups.SelectedItem;
			if (selectedObject != null)
			{
				HttpHelper<Group> httpHelper = new HttpHelper<Group>(_mainWindow, _getGroupUrl + selectedObject.Id);
				var groupTask = httpHelper.GetReqAsync();

				var group = await groupTask;
				if (group != null)
				{
					GroupName.Text = group.Name;
					var grade = group.Grade;
					var type = group.GroupType;
					if (grade != null)
						GroupGrade.Text = grade.ToString();
					if (type != null)
						GroupType.Text = type;
				}
			}
		}

		private async void LoadMembersAsync()
		{
			var selectedObject = (Group)_mainControl.Groups.SelectedItem;
			if (selectedObject != null)
			{
				HttpHelper<List<User>> httpHelper = new HttpHelper<List<User>>(_mainWindow, _getUserUrl + selectedObject.Id);
				var userTask = httpHelper.GetReqAsync();

				var preordUsers = await userTask;

				if (preordUsers != null)
				{
					var users = preordUsers.OrderBy(u => u.Surname).ToList();
					MembersCount.Content = "Всего участников: " + users.Count.ToString();
					for (int i = 0; i < users.Count; i++)
					{
						var member = new MemberControl(users.ElementAt(i));
						if (i == users.Count - 1)
							member.ButtomLine.Visibility = Visibility.Collapsed;
						Members.Children.Add(member);
					}
				}
			}
		}
	}
}
