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
		public string Title { get; set; }
		public string DeadlineTitle { get; set; }
		private bool expired = false;

		public HomeworkControl(string title, DateTime deadline)
		{
			InitializeComponent();
			Title = title;
			HomeworkDescription.Text = Title;
			if (deadline + TimeSpan.FromDays(1) < DateTime.UtcNow)
				expired = true;
			DeadlineTitle = deadline != new DateTime() ? " до " + deadline.ToShortDateString() : "";
			Deadline.Text = DeadlineTitle;
			if (expired)
				Deadline.Foreground = Brushes.Red;
		}
	}
}
