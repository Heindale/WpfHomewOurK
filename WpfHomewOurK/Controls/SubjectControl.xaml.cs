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
	/// Логика взаимодействия для SubjectControl.xaml
	/// </summary>
	public partial class SubjectControl : UserControl
	{
		Subject Subject { get; set; }
		List<Attachment> Attachments { get; set; }
		Attachment CurrentAttachment { get; set; }
		private MainControl _mainControl;
		private MainWindow _mainWindow;

		public SubjectControl(Subject subject, MainControl mainControl, MainWindow mainWindow, List<Attachment> attachments)
		{
			InitializeComponent();

			Subject = subject;
			Attachments = attachments;
			_mainControl = mainControl;
			_mainWindow = mainWindow;
			AttachmentsComboBox.ItemsSource = Attachments;
			if (Attachments.Count != 0)
			{
				AttachmentsComboBox.SelectedItem = Attachments.First();
				AddTitle.Visibility = Visibility.Collapsed;
			}
			else
			{
				AttachmentsComboBox.Visibility = Visibility.Collapsed;
				AttachmentsLink.Visibility = Visibility.Collapsed;
			}

			SubjectName.Text = Subject.Name;
			ChangeButton.Visibility = Visibility.Collapsed;
			RoleVerification();
		}

		private void RoleVerification()
		{
			if (CurrentUser.GetRole(_mainWindow) == Role.None)
			{
				SubjectName.IsReadOnly = true;
				DeleteButton.Visibility = Visibility.Collapsed;
			}
		}

		private void ChangeButton_Click(object sender, RoutedEventArgs e)
		{
			ChangeButton.Visibility = Visibility.Collapsed;
			Subject.Name = SubjectName.Text;
			UpdateSubject(Subject);
		}

		private async void UpdateSubject(Subject subject)
		{
			var httpHelper = new HttpHelper<Subject>(_mainWindow, "api/Subjects");
			await httpHelper.PatchReqAsync(subject);

			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(_mainWindow);
			await loadDataFromDb.LoadDataAsync(_mainWindow.paths);
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			DeleteSubject(Subject);
		}


		private async void DeleteSubject(Subject subject)
		{
			var httpHelper = new HttpHelper<Subject>(_mainWindow, $"api/Subjects?subjectId={subject.Id}&groupId={subject.GroupId}");
			await httpHelper.DeleteReqAsync();

			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(_mainWindow);
			await loadDataFromDb.LoadDataAsync(_mainWindow.paths);

			Visibility = Visibility.Collapsed;
		}


		private void SubjectName_SelectionChanged(object sender, RoutedEventArgs e)
		{
			ChangeButton.Visibility = Visibility.Visible;
		}

		private void SubjectName_TextChanged(object sender, TextChangedEventArgs e)
		{
			ChangeButton.Visibility = Visibility.Visible;
		}

		private void AttachmentsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Edit.Visibility = Visibility.Visible;
			AddTitle.Visibility = Visibility.Collapsed;
			CurrentAttachment = (Attachment)AttachmentsComboBox.SelectedItem;
			AttachmentsLink.Text = CurrentAttachment.Link;
		}

		private void AttachmentsLink_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Clipboard.SetText(AttachmentsLink.Text);
			MessageBox.Show("Ссылка скопирована в буфер обмена");
		}
	}
}
