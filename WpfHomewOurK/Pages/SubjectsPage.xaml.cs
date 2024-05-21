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

namespace WpfHomewOurK.Pages
{
	/// <summary>
	/// Логика взаимодействия для SubjectsPage.xaml
	/// </summary>
	public partial class SubjectsPage : Page
	{
		private MainControl _mainControl;
		private MainWindow _mainWindow;
		private int _groupId;

		public SubjectsPage(MainWindow mainWindow, MainControl mainControl, int groupId)
		{
			InitializeComponent();
			_groupId = groupId;
			_mainWindow = mainWindow;
			_mainControl = mainControl;
			RoleVerification();
		}

		private void RoleVerification()
		{
			if (CurrentUser.GetRole(_mainWindow) == Role.None)
			{
				newSubject.Visibility = Visibility.Collapsed;
			}
		}

		private void NewSubjectButton_Click(object sender, RoutedEventArgs e)
		{
			if (SubjectName.Text == "")
			{
				MessageBox.Show("Заполните название предмета");
				return;
			}

			AddSubject(new Subject
			{
				GroupId = _groupId,
				Name = SubjectName.Text
			});
			SubjectName.Text = "";
		}


		private async void AddSubject(Subject subject)
		{
			var httpHelper = new HttpHelper<Subject>(_mainWindow, "api/Subjects");
			await httpHelper.PostReqAuthAsync(subject);

			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(_mainWindow);
			await loadDataFromDb.LoadDataAsync(_mainWindow.paths);
		}

		private void Gobck()
		{
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}
	}
}
