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

namespace WpfHomewOurK.Controls
{
	/// <summary>
	/// Логика взаимодействия для MemberControl.xaml
	/// </summary>
	public partial class MemberControl : UserControl
	{
		public User Member { get; set; }
		private MainWindow _mainWindow;

		public MemberControl(User member, MainWindow mainWindow, Role? role)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			Member = member;
			Info.Content = "@" + member.Username;
			Name.Text = member.Surname + " " + member.Firstname;
			RoleVerification(role);
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
			var memberWindow = new MemberWindow(Member);
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
