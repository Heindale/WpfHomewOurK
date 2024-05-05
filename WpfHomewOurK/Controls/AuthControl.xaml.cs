using HomewOurK.Domain.Entities;
using System.Windows;
using System.Windows.Controls;
using WpfHomewOurK.Authorization;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для AuthControl.xaml
	/// </summary>
	public partial class AuthControl : UserControl
	{
		private readonly MainWindow _mainWindow;

		public AuthControl(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			InitializeComponent();
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			LoginAsync();
		}

		private async void LoginAsync()
		{
			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(_mainWindow);
			await loadDataFromDb.LoadDataAsync(_mainWindow.paths);

			var authHelper = new AuthHelper(_mainWindow);
			bool isAuth = await authHelper.AuthUserAsync(new User
			{
				Email = Login.Text,
				Password = Password.Password,
				Username = "qwerty",
				Firstname = "qwerty",
				Surname = "qwerty"
			});
			if (isAuth)
			{
				MainWindow mainWindow = new MainWindow();
				mainWindow.Top = _mainWindow.Top;
				mainWindow.Left = _mainWindow.Left;
				mainWindow.Width = _mainWindow.Width;
				mainWindow.Height = _mainWindow.Height;
				mainWindow.WindowState = _mainWindow.WindowState;
				mainWindow.MainContent.Content = new MainControl(_mainWindow);
				mainWindow.Show();
				_mainWindow.Close();
			}
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.MainContent.Content = new RegControl(_mainWindow);
		}

		private void GoOut_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.Close();
		}
	}
}