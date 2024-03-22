using HomewOurK.Domain.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfHomewOurK.Authorization;
using WpfHomewOurK.Pages;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для MainControl.xaml
	/// </summary>
	public partial class MainControl : UserControl
	{
		private readonly MainWindow _mainWindow;

		public List<Group> _groups;

		public MainControl(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;

			using (var context = new ApplicationContext())
			{
				_groups = context.Groups.ToList();
			}
			Groups.ItemsSource = _groups;

			string jsonText = File.ReadAllText(_mainWindow.path);
			AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(jsonText);
			if (desUser != null && desUser.LastGroupId > 0)
			{
				Groups.SelectedItem = _groups.First(g => g.Id == desUser.LastGroupId);
			}

			MainFrame.Navigate(new MainPage());
		}

		private void Profile_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new ProfilePage(_mainWindow));
		}

		private void Main_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new MainPage());
		}

		private void Urgent_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new UrgentPage());
		}

		private void Important_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new ImportantPage());
		}

		private void Written_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new WrittenPage());
		}

		private void Oral_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new OralPage());
		}

		private void Statistic_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new StatisticPage());
		}

		private void Info_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new InfoPage());
		}

		private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new SettingsPage());
		}

		private void AddHomework_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new AddHomeworkPage());
		}

		private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Groups.SelectedItem != null)
			{
				string jsonText = File.ReadAllText(_mainWindow.path);
				AuthUser? desUser = JsonConvert.DeserializeObject<AuthUser>(jsonText);

				if (desUser != null)
				{
					Group selectedObject = (Group)Groups.SelectedItem;

					desUser.LastGroupId = selectedObject.Id;
				}

				var jsonUser = JsonConvert.SerializeObject(desUser);
				using var sw = new StreamWriter(_mainWindow.path);
				sw.Write(jsonUser);
			}
		}
	}
}