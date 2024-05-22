using System.Linq;
using System.Windows.Controls;
using HomewOurK.Domain.Entities;

namespace WpfHomewOurK.Pages
{
	/// <summary>
	/// Логика взаимодействия для StatisticPage.xaml
	/// </summary>
	public partial class StatisticPage : Page
	{
        public int AllHomeworksCount { get; set; }	
        public int ImportantHomeworksCount { get; set; }	
        public int WrittenHomeworksCount { get; set; }	
        public int OralHomeworksCount { get; set; }	
        public int UndefindHomeworksCount { get; set; }	
        public int AllDoneHomeworksCount { get; set; }	
		public int SubjectCount { get; set; }
		public int SubjectWithHomeworksCount { get; set; }
		public int SubjectWithoutHomeworksCount { get; set; }
        public StatisticPage(int? groupId)
		{
			InitializeComponent();

			if (groupId == null)
				return;

			using (var context = new ApplicationContext())
			{
				AllHomeworksCount = context.Homeworks.Count(h => h.Done == false && h.GroupId == groupId);
				ImportantHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Important && h.Done == false && h.GroupId == groupId);
				WrittenHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Written && h.Done == false && h.GroupId == groupId);
				OralHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Oral && h.Done == false && h.GroupId == groupId);
				UndefindHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Undefined && h.Done == false && h.GroupId == groupId);
				AllDoneHomeworksCount = context.Homeworks
					.Count(h => h.Done == true && h.GroupId == groupId);
				SubjectCount = context.Subjects.Count(s => s.GroupId == groupId);
				SubjectWithHomeworksCount = context.Homeworks
					.Where(h => h.Done == false && h.GroupId == groupId)
					.Select(h => h.SubjectId).Distinct().Count();
				SubjectWithoutHomeworksCount = SubjectCount - SubjectWithHomeworksCount;
			}

			AllHomeworks.Text = $"Всего домашних заданий: {AllHomeworksCount}\nИз них важных: {ImportantHomeworksCount}" +
				$"\nПисьменных: {WrittenHomeworksCount}" +
				$"\nУстных {OralHomeworksCount}" +
				$"\nБез категории: {UndefindHomeworksCount}" +
				$"\nВыполнено: {AllDoneHomeworksCount}" +
				$"\n\nКоличество предметов: {SubjectCount}" +
				$"\nИз них с домашними заданиями: {SubjectWithHomeworksCount}" +
				$"\nБез домашних заданий: {SubjectWithoutHomeworksCount}";
		}
	}
}
