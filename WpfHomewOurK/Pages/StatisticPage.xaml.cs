using System.Linq;
using System.Windows.Controls;
using HomewOurK.Domain.Entities;
using ScottPlot;
using ScottPlot.WPF;
using SkiaSharp;

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

			List<double> _values = [];

			List<Tick> _ticks = [];

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

				var subjects = context.Subjects.Where(s => s.GroupId == groupId).ToArray();
				var homeworks = context.Homeworks.Where(s => s.GroupId == groupId);

				for (int i = 0; i < subjects.Length; i++)
				{
					var hmCounts = homeworks.Count(h => h.SubjectId == subjects[i].Id && h.Done == false);
					_values.Add(hmCounts);
					_ticks.Add(new(i, subjects[i].Name));
				}
			}

			AllHomeworks.Text = $"Выполнено домашних заданий: {AllDoneHomeworksCount}" +
				$"\nНужно выполнить: {AllHomeworksCount}" +
				$"\nИз них важных: {ImportantHomeworksCount}" +
				$"\nПисьменных: {WrittenHomeworksCount}" +
				$"\nУстных {OralHomeworksCount}" +
				$"\nБез категории: {UndefindHomeworksCount}" +
				$"\n\nКоличество предметов: {SubjectCount}" +
				$"\nИз них с домашними заданиями: {SubjectWithHomeworksCount}" +
				$"\nБез домашних заданий: {SubjectWithoutHomeworksCount}";


			List<PieSlice> slices = new()
			{
				new PieSlice() { Value = ImportantHomeworksCount, FillColor = Color.FromHex("#339444"), Label = "Важное" },
				new PieSlice() { Value = WrittenHomeworksCount, FillColor = Color.FromHex("#05818b"), Label = "Письменное" },
				new PieSlice() { Value = OralHomeworksCount, FillColor = Color.FromHex("#c6d963"), Label = "Устное" },
				new PieSlice() { Value = UndefindHomeworksCount, FillColor = Color.FromHex("#565656"), Label = "Без категории" },
			}; 

			var pie = WpfPlot.Plot.Add.Pie(slices);
			pie.ExplodeFraction = .1;
			WpfPlot.Plot.ShowLegend(Alignment.MiddleLeft);
			WpfPlot.Plot.FigureBackground.Color = Color.FromHex("#00000000");
			WpfPlot.Plot.Layout.Frameless();
			WpfPlot.Plot.HideGrid();
			WpfPlot1.Refresh();



			// create a bar plot
			double[] values = _values.ToArray();
			WpfPlot1.Plot.Add.Bars(values);
			WpfPlot1.Plot.Axes.Margins(bottom: 0);

			// create a tick for each bar
			Tick[] ticks = _ticks.ToArray();
			WpfPlot1.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
			WpfPlot1.Plot.Axes.Bottom.TickLabelStyle.Rotation = 30;
			WpfPlot1.Plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;

			// determine the width of the largest tick label
			float largestLabelWidth = 0;
			using SKPaint paint = new();
			foreach (Tick tick in ticks)
			{
				PixelSize size = WpfPlot1.Plot.Axes.Bottom.TickLabelStyle.Measure(tick.Label, paint).Size;
				largestLabelWidth = Math.Max(largestLabelWidth, size.Width);
			}

			// ensure axis panels do not get smaller than the largest label
			WpfPlot1.Plot.Axes.Bottom.MinimumSize = largestLabelWidth - largestLabelWidth * 0.25f;
			WpfPlot1.Plot.Axes.Right.MinimumSize = largestLabelWidth;
			WpfPlot1.Plot.FigureBackground.Color = Color.FromHex("#00000000");

			WpfPlot1.Refresh();
		}
	}
}
