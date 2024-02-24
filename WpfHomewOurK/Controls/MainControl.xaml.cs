using System.IO;
using System.Windows.Controls;
using WpfHomewOurK.Pages;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для MainControl.xaml
	/// </summary>
	public partial class MainControl : UserControl
	{
		private readonly MainWindow _mainWindow;

		public MainControl(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;

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
		}

		private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new SettingsPage());
		}
	}
}