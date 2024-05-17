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
		private const string _GroupsUrl = "api/Groups";
		private const string _getProposalsUrl = "api/Proposals?groupId=";

		public GroupPage(MainControl mainControl, MainWindow mainWindow)
		{
			InitializeComponent();
			_mainControl = mainControl;
			_mainWindow = mainWindow;
			LoadProposalsAsync();
			LoadMembersAsync();
			LoadGroupDataAsync();
		}

		private async void LoadProposalsAsync()
		{
			var selectedObject = (Group)_mainControl.Groups.SelectedItem;
			if (selectedObject != null)
			{
				HttpHelper<List<Proposal>> httpHelper = new HttpHelper<List<Proposal>>(_mainWindow, _getProposalsUrl + selectedObject.Id);
				var propsalsTask = httpHelper.GetReqAsync();

				var proposals = await propsalsTask;
				if (proposals != null)
				{
					foreach (Proposal proposal in proposals)
					{
						Proposals.Children.Add(new ProposalControl(proposal, _mainWindow));
					}
				}
			}
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
					UniqGroupName.Text = group.UniqGroupName;
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

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteGroupAsync(sender, e);
		}



		private async void DeleteGroupAsync(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить группу?\nОтменить данное действие будет невозможно.", "Удаление группы", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
			//if (result == MessageBoxResult.Yes)
			//{
			//	HttpHelper<User> httpHelper = new HttpHelper<User>(_mainWindow, _baseUsersUrl);
			//	var response = await httpHelper.DeleteReqAsync();
			//	if (response != null)
			//	{
			//		MessageBox.Show("Аккаунт удален", "Удаление аккаунта", MessageBoxButton.OK, MessageBoxImage.Information);
			//	}
			//	else
			//		MessageBox.Show("Аккаунт не был удален", "Удаление аккаунта", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			//}
		}

		private void SaveChanges_Click(object sender, RoutedEventArgs e)
		{
			UpdateGroupAsync();
		}

		private async void UpdateGroupAsync()
		{
			var selectedObject = (Group)_mainControl.Groups.SelectedItem;
			if (selectedObject != null)
			{
				HttpHelper<Group> httpHelper = new HttpHelper<Group>(_mainWindow, _GroupsUrl);
				await httpHelper.PatchReqAsync(new Group
				{
					Id = selectedObject.Id,
					Grade = int.Parse(GroupGrade.Text),
					GroupType = GroupType.Text,
					Name = GroupName.Text,
					UniqGroupName = UniqGroupName.Text
				});
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_mainControl.MainFrame.Navigate(new SearchGroupPage(_mainWindow));
		}
	}
}
