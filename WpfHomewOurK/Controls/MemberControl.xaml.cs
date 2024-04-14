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
    /// Логика взаимодействия для MemberControl.xaml
    /// </summary>
    public partial class MemberControl : UserControl
    {
        public User Member { get; set; }

        public MemberControl(User member)
        {
            InitializeComponent();
            Member = member;
            Info.Content = "@" + member.Username;
            Name.Text = member.Surname + " " + member.Firstname;
        }
    }
}
