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

		public HomeworkControl(string title)
		{
			InitializeComponent();
			Title = title;
			HomeworkDescription.Text = Title;
		}
	}
}
