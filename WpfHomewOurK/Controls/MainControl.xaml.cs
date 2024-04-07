using HomewOurK.Domain.Entities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfHomewOurK.Authorization;
using WpfHomewOurK.Controls;
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

			LoadMainPage();
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

		private void Main_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadMainPage();
		}

		public void LoadMainPage()
		{
			MainPage mainPage = new MainPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				var contextHomeworks = context.Homeworks
					.Where(h => !h.Done).OrderByDescending(h => h.CreationDate);
				var groupingHomeworks = contextHomeworks
					.GroupBy(h => h.SubjectId)
					.ToList() // Выполняем запрос и получаем список групп
					.OrderByDescending(group => group.Max(h => h.CreationDate));

				foreach (var ghomeworks in groupingHomeworks)
				{
					if (ghomeworks.Any())
					{
						var homeworks = ghomeworks.OrderByDescending(h => h.CreationDate);
						var subject = context.Subjects
							.FirstOrDefault(s => s.Id == homeworks.First().SubjectId);

						mainPage.HomeworksStackPanel.Children.Add(new TextBlock()
						{
							Text = subject != null ? subject.Name : "",
							FontSize = 25
						});

						foreach (Homework homework in homeworks)
						{
							HomeworkControl homeworkControl = new HomeworkControl(homework, this);
							homeworkControl.Category = 0;
							mainPage.HomeworksStackPanel.Children.Add(homeworkControl);
						}
					}
				}
			}

			MainFrame.Navigate(mainPage);
		}

		private void AddHomework_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Group selectedObject = (Group)Groups.SelectedItem;

			MainFrame.Navigate(new EditAddHomeworkPage(_mainWindow, this, selectedObject.Id));
		}

		public void EditHomework(Homework homework, int category)
		{
			Group selectedObject = (Group)Groups.SelectedItem;
			var editPage = new EditAddHomeworkPage(_mainWindow, this, selectedObject.Id);
			editPage.Category = category;
			editPage.Edit(homework);
			MainFrame.Navigate(editPage);
		}

		private void Profile_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MainFrame.Navigate(new ProfilePage(_mainWindow));
		}

		private void Urgent_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadUrgentPage();
		}

		public void LoadUrgentPage()
		{
			UrgentPage urgentPage = new UrgentPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Deadline != null)
					.OrderBy(h => h.Deadline).ToList();

				foreach (Homework homework in homeworks)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homework, this);
					homeworkControl.Category = 1;
					urgentPage.HomeworksStackPanel.Children.Add(homeworkControl);
				}
			}

			MainFrame.Navigate(urgentPage);
		}

		private void Important_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadImportantPage();
		}

		public void LoadImportantPage()
		{
			ImportantPage importantPage = new ImportantPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Important).ToList();

				foreach (Homework homework in homeworks)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homework, this);
					homeworkControl.Category = 2;
					importantPage.HomeworksStackPanel.Children.Add(homeworkControl);
				}
			}

			MainFrame.Navigate(importantPage);
		}

		private void Written_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadWrittenPage();
		}

		public void LoadWrittenPage()
		{
			WrittenPage writtenPage = new WrittenPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Written).ToList();

				foreach (Homework homework in homeworks)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homework, this);
					homeworkControl.Category = 3;
					writtenPage.HomeworksStackPanel.Children.Add(homeworkControl);
				}
			}

			MainFrame.Navigate(writtenPage);
		}

		private void Oral_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadOralPage();
		}

		public void LoadOralPage()
		{
			OralPage oralPage = new OralPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Oral).ToList();

				foreach (Homework homework in homeworks)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homework, this);
					homeworkControl.Category = 4;
					oralPage.HomeworksStackPanel.Children.Add(homeworkControl);
				}
			}

			MainFrame.Navigate(oralPage);
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
	}
}