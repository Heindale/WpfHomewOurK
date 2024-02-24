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
			var authHelper = new AuthHelper(_mainWindow);
			authHelper.AuthUserAsync(new User
			{
				Email = Login.Text,
				Password = Password.Password,
				Username = "qwerty",
				Firstname = "qwerty",
				Surname = "qwerty"
			});
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.MainContent.Content = new RegControl(_mainWindow);
		}
	}
}