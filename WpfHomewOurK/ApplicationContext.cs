using HomewOurK.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomewOurK
{
	internal class ApplicationContext : DbContext
	{
		public DbSet<Group> Groups { get; set; }
		public DbSet<Homework> Homeworks { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		public DbSet<Teacher> Teachers { get; set; }
		public DbSet<Attachment> Attachments { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string filePath = Path.GetFullPath("../../../homewourk.db");

			optionsBuilder.UseSqlite($"Data Source={filePath}");
		}
	}
}