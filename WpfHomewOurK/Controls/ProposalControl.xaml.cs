using HomewOurK.Domain.Common.Interfaces;
using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
	/// Логика взаимодействия для ProposalControl.xaml
	/// </summary>
	public partial class ProposalControl : UserControl
	{
		private Proposal _proposal;
		private MainWindow _mainWindow;
		private const string _getUsersUrl = "api/Users";

		public ProposalControl(Proposal proposal, MainWindow mainWindow)
		{
			InitializeComponent();

			_proposal = proposal;
			_mainWindow = mainWindow;

			LoadUserDataAsync();
			ShimmeringBackground();
		}

		private async void LoadUserDataAsync()
		{
			HttpHelper<List<User>> httpHelper = new HttpHelper<List<User>>(_mainWindow, _getUsersUrl);
			var usersTask = httpHelper.GetReqAsync();
			var users = await usersTask;
			if (users != null)
			{
				var user = users.FirstOrDefault(u => u.Id == _proposal.UserId);
				if (user != null)
				{
					if (user.Surname != null || user.Firstname != null)
						Description.Text = $"{user.Surname} {user.Firstname} хочет вступить в группу";
					else
						Description.Text = $"{user.Username} хочет вступить в группу";
				}
				else
					this.Visibility = Visibility.Collapsed;
			}
		}

		private async void ShimmeringBackground()
		{
			var rnd = new Random();

			byte red = (byte)rnd.Next(1, 254);
			byte blue = (byte)rnd.Next(2, 253);

			byte bStep = (byte)rnd.Next(1, 5);
			byte rStep = (byte)rnd.Next(1, 5);

			byte green = 255;
			var bRevers = true;
			var rRevers = true;
			while (true)
			{
				CornerBorder.Background = new SolidColorBrush(Color.FromArgb(150, red, green, blue));
				await Task.Delay(10);

				if (bRevers)
				{
					blue -= bStep;
					if (blue <= 100)
						bRevers = false;
				}
				else
				{
					blue += bStep;
					if (blue >= 250)
						bRevers = true;
				}

				if (rRevers)
				{
					red -= rStep;
					if (red <= 5)
						rRevers = false;
				}
				else
				{
					red += rStep;
					if (red >= 200)
						rRevers = true;
				}
			}
		}

		private void Disagree_Click(object sender, RoutedEventArgs e)
		{
			DisagreeAsync();
		}

		private async void DisagreeAsync()
		{
			HttpHelper<Proposal> proposalHttpHelper = new HttpHelper<Proposal>(_mainWindow, $"api/Proposals?id={_proposal.Id}");
			var deleteResponse = await proposalHttpHelper.DeleteReqAsync();

			if (deleteResponse != null && deleteResponse.IsSuccessStatusCode)
				Visibility = Visibility.Collapsed;
		}

		private void Agree_Click(object sender, RoutedEventArgs e)
		{
			AgreeAsync();
		}

		private async void AgreeAsync()
		{
			HttpHelper<GroupsUsers> httpHelper = new HttpHelper<GroupsUsers>(_mainWindow, $"api/UsersGroups");
			var postResponse = await httpHelper.PostReqAuthAsync(new GroupsUsers
			{
				GroupId = _proposal.GroupId,
				UserId = _proposal.UserId
			});

			HttpHelper<Proposal> proposalHttpHelper = new HttpHelper<Proposal>(_mainWindow, $"api/Proposals?id={_proposal.Id}");
			var deleteResponse = await proposalHttpHelper.DeleteReqAsync();

			if (postResponse != null && deleteResponse != null &&
				postResponse.IsSuccessStatusCode && deleteResponse.IsSuccessStatusCode)
				Visibility = Visibility.Collapsed;
		}
	}
}
