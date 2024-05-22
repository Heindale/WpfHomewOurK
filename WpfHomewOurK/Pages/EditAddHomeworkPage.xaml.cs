using HomewOurK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Логика взаимодействия для EditAddHomeworkPage.xaml
	/// </summary>
	public partial class EditAddHomeworkPage : Page
	{
		public List<Subject> subjects;
		public Dictionary<string, Importance> importances;

		private MainWindow _mainWindow;
		private MainControl _mainControl;
		private int _groupId;
		private int _subjectId;
		private int _homeworkId;
		private Importance _importance;
		private DateTime? _deadline;
		public int Category { get; set; }

		public EditAddHomeworkPage(MainWindow mainWindow, MainControl mainControl, int groupId)
		{
			InitializeComponent();
			_groupId = groupId;
			_mainWindow = mainWindow;
			_mainControl = mainControl;

			using (var context = new ApplicationContext())
			{
				subjects = context.Subjects.Where(s => s.GroupId == groupId).ToList();
			}
			Subjects.ItemsSource = subjects;

			importances = new Dictionary<string, Importance>
			{
				{"Без категории", Importance.Undefined},
				{"Устное", Importance.Oral },
				{"Письменное", Importance.Written },
				{"Важное", Importance.Important }
			};

			Importances.ItemsSource = importances.Keys;
			_groupId = groupId;
		}

		public void Edit(Homework homework)
		{
			Description.Text = homework.Description;
			_homeworkId = homework.Id;
			_groupId = homework.GroupId;
			_subjectId = homework.SubjectId;
			_importance = homework.Importance;
			_deadline = homework.Deadline;
			var comboSubjects = Subjects.Items;
			var subjects = new List<Subject>();

			foreach (var subject in comboSubjects)
			{
				subjects.Add(subject as Subject);
			}

			string selectedKey = importances.FirstOrDefault(x => x.Value == _importance).Key;
			Importances.SelectedItem = selectedKey;
			Subjects.SelectedItem = subjects.FirstOrDefault(s => s.Id == _subjectId);
			DeadlineDatePicker.Text = _deadline != null ? _deadline.Value.ToShortDateString() : "";
		}

		private void Importances_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Получаем выбранный ключ
			string selectedKey = Importances.SelectedItem as string;

			// Получаем соответствующее значение из словаря, если ключ был выбран
			if (selectedKey != null && importances.TryGetValue(selectedKey, out Importance importance))
			{
				_importance = importance;
			}
		}

		private void Create_Click(object sender, RoutedEventArgs e)
		{
			var homework = new Homework
			{
				Description = Description.Text,
				GroupId = _groupId,
				SubjectId = _subjectId,
				Importance = _importance,
				Deadline = _deadline != null ? DateTime.SpecifyKind((DateTime)_deadline, DateTimeKind.Utc) : null
			};

			AddHomework(homework);
		}

		private async void AddHomework(Homework homework)
		{
			var httpHelper = new HttpHelper<Homework>(_mainWindow, "api/Homeworks");
			await httpHelper.PostReqAuthAsync(homework);
			await _mainWindow.UpdateDataFromLocalDb(_mainWindow);
			_mainControl.LoadMainPage(_groupId);
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			var homework = new Homework
			{
				Id = _homeworkId,
				Description = Description.Text,
				GroupId = _groupId,
				SubjectId = _subjectId,
				Importance = _importance,
				Deadline = _deadline != null ? DateTime.SpecifyKind((DateTime)_deadline, DateTimeKind.Utc) : null
			};
			UpdateHomework(homework);
		}

		private async void UpdateHomework(Homework homework)
		{
			var httpHelper = new HttpHelper<Homework>(_mainWindow, "api/Homeworks");
			await httpHelper.PatchReqAsync(homework);
			using (var context = new ApplicationContext())
			{
				context.Homeworks.Remove(homework);
				context.SaveChanges();
			}

			LoadDataFromDb loadDataFromDb = new LoadDataFromDb(_mainWindow);
			await loadDataFromDb.LoadDataAsync(_mainWindow.paths);
			Gobck();
		}

		private void Subjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Subjects.SelectedItem != null)
			{
				Subject selectedSubject = (Subject)Subjects.SelectedItem;
				_subjectId = selectedSubject.Id;
			}
		}

		private void DeadlineDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			_deadline = DeadlineDatePicker.SelectedDate;
		}

		private void Goback_Click(object sender, RoutedEventArgs e)
		{
			Gobck();
		}

		private void Gobck()
		{
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}

		private void Refrsh()
		{
			// Проверяем, доступен ли стек навигации
			if (NavigationService.CanGoBack)
			{
				// Переходим на предыдущий экран
				NavigationService.Refresh();
			}
		}
	}
}
