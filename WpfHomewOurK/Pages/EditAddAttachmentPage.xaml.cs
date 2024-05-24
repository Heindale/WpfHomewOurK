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
    /// Логика взаимодействия для EditAddAttachmentPage.xaml
    /// </summary>
    public partial class EditAddAttachmentPage : Page
    {
		private MainWindow _mainWindow;
		private Subject _subject;
		MainControl _mainControl;
		Attachment _attachment;
		private int _groupId;
		private int _subjectId;

		public EditAddAttachmentPage(Attachment attachment, MainWindow mainWindow, MainControl mainControl, Subject subject, int groupId, bool isEdit = false)
        {
            InitializeComponent(); 
			_groupId = groupId;
			_mainWindow = mainWindow;
			_subject = subject;
			_attachment = attachment;
			_mainControl = mainControl;

			if (isEdit)
			{ 
				Create.Visibility = Visibility.Collapsed;
				Update.Visibility = Visibility.Visible;
			}
			else
			{
				Create.Visibility = Visibility.Visible;
				Update.Visibility = Visibility.Collapsed;
			}

			_groupId = groupId;

			Description.Text = _attachment.Name;
			Link.Text = _attachment.Link;
		}

		private void Create_Click(object sender, RoutedEventArgs e)
		{
			CreateAsync();
		}

		private async void CreateAsync()
		{
			_attachment.Subject = _subject;
			_attachment.SubjectId = _subject.Id;
			_attachment.Name = Description.Text;
			_attachment.Link = Link.Text;
			_attachment.GroupId = _subject.GroupId;

			var httpHelper = new HttpHelper<Attachment>(_mainWindow, "api/Attachments");
			await httpHelper.PostReqAuthAsync(_attachment);
			_mainControl.LoadSubjectsPage(_subject.GroupId);
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			UpdateAsync();
		}

		private async void UpdateAsync()
		{
			_attachment.Subject = _subject;
			_attachment.SubjectId = _subject.Id;
			_attachment.Name = Description.Text;
			_attachment.Link = Link.Text;
			_attachment.GroupId = _subject.GroupId;

			var httpHelper = new HttpHelper<Attachment>(_mainWindow, "api/Attachments");
			await httpHelper.PatchReqAsync(_attachment);
			_mainControl.LoadSubjectsPage(_subject.GroupId);
		}

		private void Goback_Click(object sender, RoutedEventArgs e)
		{
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}
	}
}
