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
using System.Windows.Shapes;
using WpfHomewOurK.Controls;

namespace WpfHomewOurK
{
	/// <summary>
	/// Логика взаимодействия для MemberWindow.xaml
	/// </summary>
	public partial class MemberWindow : Window
	{
		public bool isOpen = true;
		public bool isExclude = false;
		User _user;

		public MemberWindow(User user)
		{
			InitializeComponent();
			_user = user;
			Username.Text = user.Username;
			Name.Content = "Имя: " + user.Firstname;
			Surname.Content = "Фамилия: " + user.Surname;
		}

		public void Exclude()
		{
			isExclude = true;
		}
		 
		private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
			isOpen = false;	
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			Exclude();
			Close();
		}

		private void DeleteRole_Click(object sender, RoutedEventArgs e)
		{

		}

		private void AddRole_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
