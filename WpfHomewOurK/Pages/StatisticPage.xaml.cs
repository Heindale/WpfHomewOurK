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
        public StatisticPage()
		{
			InitializeComponent();

			using (var context = new ApplicationContext())
			{
				AllHomeworksCount = context.Homeworks.Count(h => h.Done == false);
				ImportantHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Important && h.Done == false);
				WrittenHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Written && h.Done == false);
				OralHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Oral && h.Done == false);
				UndefindHomeworksCount = context.Homeworks
					.Count(h => h.Importance == Importance.Undefined && h.Done == false);
				AllDoneHomeworksCount = context.Homeworks
					.Count(h => h.Done == true);
			}
		}
	}
}
