using HomewOurK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfHomewOurK.Authorization;

namespace WpfHomewOurK
{
	public static class CurrentUser
	{
		public static Role GetRole(MainWindow mainWindow)
		{
			var authHelper = new AuthHelper(mainWindow);
			string jsonText = File.ReadAllText(mainWindow.path);
			var desUser = authHelper.DeserializeAuthUser(jsonText);
			if (desUser != null)
			{
				return desUser.Role;
			}
			return Role.None;
		}

		public static int? GetGroupId(MainWindow mainWindow)
		{
			var authHelper = new AuthHelper(mainWindow);
			string jsonText = File.ReadAllText(mainWindow.path);
			var desUser = authHelper.DeserializeAuthUser(jsonText);
			if (desUser != null)
			{
				return desUser.LastGroupId;
			}
			return null;
		}
	}
}
