﻿using HomewOurK.Domain.Entities;
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
	/// Логика взаимодействия для HomeworkControl.xaml
	/// </summary>
	public partial class HomeworkControl : UserControl
	{
		public string SubjName { get; set; }
		public string Title { get; set; }
		public string DeadlineTitle { get; set; }
		public Homework Homework { get; set; }
		private MainControl _mainControl;
		private bool expired = false;
		public int Category { get; set; }

		public HomeworkControl(Homework homework, MainControl mainControl, bool isMainPage = false)
		{
			InitializeComponent();

			_mainControl = mainControl;
			Homework = homework;
			Title = homework.Description;
			HomeworkDescription.Text = Title;

			if (homework.Deadline + TimeSpan.FromDays(1) < DateTime.UtcNow)
				expired = true;

			DeadlineTitle = homework.Deadline != null ? " до " + homework.Deadline.Value.ToShortDateString() : "";
			Deadline.Text = DeadlineTitle;

			if (expired)
				Deadline.Foreground = new SolidColorBrush(Color.FromRgb(155, 0, 0));

			if (isMainPage)
				switch (Homework.Importance)
				{
					case Importance.Undefined:
						break;
					case Importance.Oral:
						oralImage.Visibility = Visibility.Visible;
						break;
					case Importance.Written:
						writtenImage.Visibility = Visibility.Visible;
						break;
					case Importance.Important:
						importantImage.Visibility = Visibility.Visible;
						break;
					default:
						break;
				}

			var subjName = GetSubjName(homework.SubjectId, homework.GroupId);
			if (subjName != null)
				SubjName = subjName;
			SubjectName.Text = SubjName;
			RoleVerification();
		}

		private void RoleVerification()
		{
			if (CurrentUser.GetRole(_mainControl._mainWindow) == Role.None)
				ChangeButton.Visibility = Visibility.Collapsed;
			else
				ChangeButton.Visibility = Visibility.Visible;
		}

		private string? GetSubjName(int id, int groupId)
		{
			using (var context = new ApplicationContext())
			{
				var subject = context.Subjects.FirstOrDefault(s => s.Id == id && s.GroupId == groupId);
				if (subject != null)
					return subject.Name;
				return null;
			}
		}

		private void Complete()
		{
			using (var context = new ApplicationContext())
			{
				var homework = context.Homeworks.FirstOrDefault(h => h.Id == Homework.Id
				&& h.GroupId == Homework.GroupId && h.SubjectId == Homework.SubjectId);

				if (homework != null)
				{
					homework.Done = true;
					homework.CompletedDate = DateTime.UtcNow;
					context.Homeworks.Update(homework);
					context.SaveChanges();
				}
			}
		}

		private void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			Complete();
			switch (Category)
			{
				case 0:
					_mainControl.LoadMainPage();
					break;
				case 1:
					_mainControl.LoadUrgentPage();
					break;
				case 2:
					_mainControl.LoadImportantPage();
					break;
				case 3:
					_mainControl.LoadWrittenPage();
					break;
				case 4:
					_mainControl.LoadOralPage();
					break;
			}
		}

		private void ChangeButton_Click(object sender, RoutedEventArgs e)
		{
			_mainControl.EditHomework(Homework, Category);
		}
	}
}
