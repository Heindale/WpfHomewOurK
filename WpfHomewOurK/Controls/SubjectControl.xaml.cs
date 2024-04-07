using HomewOurK.Domain.Entities;
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
	/// Логика взаимодействия для SubjectControl.xaml
	/// </summary>
	public partial class SubjectControl : UserControl
	{
		Subject Subject { get; set; }
		private MainControl _mainControl;

		public SubjectControl(Subject subject, MainControl mainControl)
		{
			InitializeComponent();

			Subject = subject;
			_mainControl = mainControl;

			SubjectName.Text = Subject.Name;
			ChangeButton.Visibility = Visibility.Collapsed;
		}

		private void ChangeButton_Click(object sender, RoutedEventArgs e)
		{
			ChangeButton.Visibility = Visibility.Collapsed;

		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SubjectName_SelectionChanged(object sender, RoutedEventArgs e)
		{
			ChangeButton.Visibility = Visibility.Visible;
		}
	}
}
