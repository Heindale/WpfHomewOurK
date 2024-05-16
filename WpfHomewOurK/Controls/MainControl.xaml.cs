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
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

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
			RestyleLeftMenu();
		}

		private void RestyleLeftMenu()
		{
			bool firstBtn = true;
			foreach (var menuBtn in LeftMenu.Children )
			{
				var btn = menuBtn as Button;
				btn.Margin = new Thickness(5, 0, 5, 0);
				if (firstBtn)
					btn.Margin = new Thickness(5, 5, 5, 0);
				firstBtn = false;
			}
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
			PaintActiveButton(Main);
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

						var subjText = new TextBlock
						{
							Text = subject != null ? subject.Name : "",
							FontSize = 25,
							Padding = new Thickness(5)
						};

						// Создаем стиль для TextBlock
						Style textBlockStyle = new Style(typeof(TextBlock));

						// Устанавливаем BasedOn для стиля
						textBlockStyle.BasedOn = (Style)Application.Current.FindResource("MaterialDesignTextBlock"); // Используйте ресурс из вашего XAML

						// Устанавливаем остальные свойства
						textBlockStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily("Verdana")));
						textBlockStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromArgb(127, 255, 255, 255))));

						// Применяем стиль к TextBlock
						subjText.Style = textBlockStyle;

						mainPage.HomeworksStackPanel.Children.Add(subjText);

						var groupSubjBorder = new Border
						{
							Background = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)),
							CornerRadius = new CornerRadius(10),
							Effect = new DropShadowEffect()
							{
								BlurRadius = 4,
								ShadowDepth = 3,
								Direction = 315,
								Opacity = 0.3,
								Color = Color.FromRgb(0, 0, 0)
							}
						};

						var groupSubjStack = new StackPanel();

						for (int i = 0; i < homeworks.Count(); i++)
						{
							HomeworkControl homeworkControl = new HomeworkControl(homeworks.ElementAt(i), this);
							if (i == homeworks.Count() - 1)
							{
								homeworkControl.BorderButtom.BorderThickness = new Thickness(0);
							}
							homeworkControl.SubjectName.Visibility = Visibility.Collapsed;
							homeworkControl.Category = 0;
							groupSubjStack.Children.Add(homeworkControl);
							groupSubjBorder.Child = groupSubjStack;
						}
						mainPage.HomeworksStackPanel.Children.Add(groupSubjBorder);
					}
				}
			}

			MainFrame.Navigate(mainPage);
		}

		private void AddHomework_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintActiveButton(AddHomework);
			Group selectedObject = (Group)Groups.SelectedItem;
			var addHomeworkPage = new EditAddHomeworkPage(_mainWindow, this, selectedObject.Id);
			addHomeworkPage.newHomeworkTitle.Visibility = Visibility.Visible;
			addHomeworkPage.Update.Visibility = Visibility.Collapsed;
			addHomeworkPage.Goback.Visibility = Visibility.Collapsed;
			MainFrame.Navigate(addHomeworkPage);
		}

		public void EditHomework(Homework homework, int category)
		{
			Group selectedObject = (Group)Groups.SelectedItem;
			var editPage = new EditAddHomeworkPage(_mainWindow, this, selectedObject.Id);
			editPage.newHomeworkTitle.Text = "Редактирование";
			editPage.Create.Visibility = Visibility.Collapsed;
			editPage.Category = category;
			editPage.Edit(homework);
			MainFrame.Navigate(editPage);
		}

		private void Profile_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintAllButtonsToWhite();
			MainFrame.Navigate(new ProfilePage(_mainWindow));
		}

		private void Urgent_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadUrgentPage();
		}

		public void LoadUrgentPage()
		{
			PaintActiveButton(Urgent);
			UrgentPage urgentPage = new UrgentPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Deadline != null)
					.OrderBy(h => h.Deadline).ToList();

				var groupSubjBorder = new Border
				{
					Background = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)),
					CornerRadius = new CornerRadius(10),
					Effect = new DropShadowEffect()
					{
						BlurRadius = 4,
						ShadowDepth = 3,
						Direction = 315,
						Opacity = 0.3,
						Color = Color.FromRgb(0, 0, 0)
					}
				};

				var subjStack = new StackPanel();

				for (int i = 0; i < homeworks.Count(); i++)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homeworks.ElementAt(i), this);
					if (i == homeworks.Count() - 1)
					{
						homeworkControl.BorderButtom.BorderThickness = new Thickness(0);
					}
					homeworkControl.Category = 1;
					subjStack.Children.Add(homeworkControl);
				}

				groupSubjBorder.Child = subjStack;
				urgentPage.HomeworksStackPanel.Children.Add(groupSubjBorder);
			}

			MainFrame.Navigate(urgentPage);
		}

		private void Important_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadImportantPage();
		}

		public void LoadImportantPage()
		{
			PaintActiveButton(Important);
			ImportantPage importantPage = new ImportantPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Important).ToList();

				var groupSubjBorder = new Border
				{
					Background = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)),
					CornerRadius = new CornerRadius(10),
					Effect = new DropShadowEffect()
					{
						BlurRadius = 4,
						ShadowDepth = 3,
						Direction = 315,
						Opacity = 0.3,
						Color = Color.FromRgb(0, 0, 0)
					}
				};

				var subjStack = new StackPanel();

				for (int i = 0; i < homeworks.Count(); i++)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homeworks.ElementAt(i), this);
					if (i == homeworks.Count() - 1)
					{
						homeworkControl.BorderButtom.BorderThickness = new Thickness(0);
					}
					homeworkControl.Category = 2;
					subjStack.Children.Add(homeworkControl);
				}

				groupSubjBorder.Child = subjStack;
				importantPage.HomeworksStackPanel.Children.Add(groupSubjBorder);
			}

			MainFrame.Navigate(importantPage);
		}

		private void Written_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadWrittenPage();
		}

		public void LoadWrittenPage()
		{
			PaintActiveButton(Written);
			WrittenPage writtenPage = new WrittenPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Written).ToList();
				var groupSubjBorder = new Border
				{
					Background = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)),
					CornerRadius = new CornerRadius(10),
					Effect = new DropShadowEffect()
					{
						BlurRadius = 4,
						ShadowDepth = 3,
						Direction = 315,
						Opacity = 0.3,
						Color = Color.FromRgb(0, 0, 0)
					}
				};

				var subjStack = new StackPanel();

				for (int i = 0; i < homeworks.Count(); i++)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homeworks.ElementAt(i), this);
					if (i == homeworks.Count() - 1)
					{
						homeworkControl.BorderButtom.BorderThickness = new Thickness(0);
					}
					homeworkControl.Category = 3;
					subjStack.Children.Add(homeworkControl);
				}

				groupSubjBorder.Child = subjStack;
				writtenPage.HomeworksStackPanel.Children.Add(groupSubjBorder);
			}

			MainFrame.Navigate(writtenPage);
		}

		private void Oral_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LoadOralPage();
		}

		public void LoadOralPage()
		{
			PaintActiveButton(Oral);
			OralPage oralPage = new OralPage();

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Homework> homeworks = context.Homeworks
					.Where(h => !h.Done && h.Importance == Importance.Oral).ToList();
				var groupSubjBorder = new Border
				{
					Background = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)),
					CornerRadius = new CornerRadius(10),
					Effect = new DropShadowEffect()
					{
						BlurRadius = 4,
						ShadowDepth = 3,
						Direction = 315,
						Opacity = 0.3,
						Color = Color.FromRgb(0, 0, 0)
					}
				};

				var subjStack = new StackPanel();

				for (int i = 0; i < homeworks.Count(); i++)
				{
					HomeworkControl homeworkControl = new HomeworkControl(homeworks.ElementAt(i), this);
					if (i == homeworks.Count() - 1)
					{
						homeworkControl.BorderButtom.BorderThickness = new Thickness(0);
					}
					homeworkControl.Category = 4;
					subjStack.Children.Add(homeworkControl);
				}

				groupSubjBorder.Child = subjStack;
				oralPage.HomeworksStackPanel.Children.Add(groupSubjBorder);
			}

			MainFrame.Navigate(oralPage);
		}

		private void Statistic_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintActiveButton(Statistic);
			MainFrame.Navigate(new StatisticPage());
		}

		private void Info_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintAllButtonsToWhite();
			MainFrame.Navigate(new InfoPage());
		}

		private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintAllButtonsToWhite();
			MainFrame.Navigate(new SettingsPage());
		}

		private void Subjects_Click(object sender, RoutedEventArgs e)
		{
			LoadSubjectsPage();
		}
		public async void LoadSubjectsPage()
		{
			PaintActiveButton(Subjects);
			Group selectedObject = (Group)Groups.SelectedItem;
			SubjectsPage subjectsPage = new SubjectsPage(_mainWindow, this, selectedObject.Id);

			using (ApplicationContext context = new ApplicationContext())
			{
				List<Subject> subjects = context.Subjects.ToList();
				context.Subjects.RemoveRange(subjects);
				HttpHelper<List<Subject>> httpHelper = new HttpHelper<List<Subject>>
					(_mainWindow, "api/Subjects/GetSubjects?groupId=" + selectedObject.Id);
				var subjectsHttp = await httpHelper.GetReqAsync();
				if (subjectsHttp != null)
					context.AddRange(subjectsHttp);
				subjects = context.Subjects.ToList();
				foreach (Subject subject in subjects)
				{
					SubjectControl subjectControl = new SubjectControl(subject, this, _mainWindow);
					subjectsPage.SubjectsStackPanel.Children.Add(subjectControl);
				}
				
				context.SaveChanges();
			}

			MainFrame.Navigate(subjectsPage);
		}

		public void PaintActiveButton(Button button)
		{
			PaintAllButtonsToWhite();
			//button.Background = new SolidColorBrush(Color.FromRgb(0, 150, 136));
			button.Background = new SolidColorBrush(Color.FromArgb(75, 255, 255, 255));
		}

		public void PaintAllButtonsToWhite()
		{
			var btnbckgrnds = Brushes.Transparent;
			Main.Background = btnbckgrnds;
			Urgent.Background = btnbckgrnds;
			Important.Background = btnbckgrnds;
			Written.Background = btnbckgrnds;
			Oral.Background = btnbckgrnds;
			Subjects.Background = btnbckgrnds;
			Teachers.Background = btnbckgrnds;
			Statistic.Background = btnbckgrnds;
			AddHomework.Background = btnbckgrnds;
			Group.Background = btnbckgrnds;
		}

		private void Group_Click(object sender, RoutedEventArgs e)
		{
			PaintActiveButton(Group);
			MainFrame.Navigate(new GroupPage(this, _mainWindow));
		}

		private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			_mainWindow.Left += e.HorizontalChange;
			_mainWindow.Top += e.VerticalChange;
		}

		private void Hide_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.WindowState = WindowState.Minimized;
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.Close();
		}

		private void Maximize_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.WindowState = WindowState.Maximized;
			Minimize.Visibility = Visibility.Visible;
			Maximize.Visibility = Visibility.Collapsed;
		}

		private void Minimize_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.WindowState = WindowState.Normal;
			Maximize.Visibility = Visibility.Visible;
			Minimize.Visibility = Visibility.Collapsed;
		}
	}
}