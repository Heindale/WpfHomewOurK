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
	/// Логика взаимодействия для GroupControl.xaml
	/// </summary>
	public partial class GroupControl : UserControl
	{
		public Group Group { get; set; }
		MainWindow _mainWindow;

		public GroupControl(Group group, MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			Group = group;
			UniqGroupName.Text = group.UniqGroupName;
			GroupName.Text = group.Name;
		}

		private void Apply_Click(object sender, RoutedEventArgs e)
		{
			ApplyAsync();
		}

		private async void ApplyAsync()
		{
			var httpUser = new HttpHelper<User>(_mainWindow, "api/Users/GetUser");
			var user = await httpUser.GetReqAsync();
			if (user != null)
			{
				var httpHelper = new HttpHelper<Proposal>(_mainWindow, "api/Proposals");
				await httpHelper.PostReqAuthAsync(new Proposal
				{
					GroupId = Group.Id,
					UserId = user.Id
				});
			}
			Visibility = Visibility.Collapsed;
		}
	}
}
