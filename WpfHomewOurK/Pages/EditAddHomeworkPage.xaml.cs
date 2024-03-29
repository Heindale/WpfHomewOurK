﻿using HomewOurK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private int _groupId;
		private int _subjectId;
		private int _homeworkId;
		private Importance _importance;

		public EditAddHomeworkPage(MainWindow mainWindow, int groupId)
		{
			InitializeComponent();
			_groupId = groupId;
			_mainWindow = mainWindow;

			using (var context = new ApplicationContext())
			{
				subjects = context.Subjects.ToList();
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
				Importance = _importance
			};

			AddHomework(homework);
		}

		private void Subjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Subjects.SelectedItem != null)
			{
				Subject selectedSubject = (Subject)Subjects.SelectedItem;
				_subjectId = selectedSubject.Id;
			}
		}

		private async void AddHomework(Homework homework)
		{
			var httpHelper = new HttpHelper<Homework>(_mainWindow, "api/Homeworks");
			await httpHelper.PostReqAuthAsync(homework);
		}
	}
}
