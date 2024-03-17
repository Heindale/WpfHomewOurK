using HomewOurK.Domain.Common;
using HomewOurK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomewOurK
{
	internal class LoadDataFromDb
	{
		private MainWindow _mainWindow;
		//private ApplicationContext _context;

		public LoadDataFromDb(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			//_context = context;
		}

		public async void LoadBaseEntityAsync<T>(string getEntityUrl, string userId) where T : BaseEntity
		{
			using (var context = new ApplicationContext())
			{
				var httpHelper = new HttpHelper<List<T>>(_mainWindow, getEntityUrl + userId);
				var entitiesTask = httpHelper.GetReqAsync();
				List<T>? entities = await entitiesTask;

				if (entities != null)
				{
					var localGroups = context.Set<T>().ToList();

					foreach (var entity in entities)
					{
						if (localGroups.FirstOrDefault(g => g.Id == entity.Id) == null)
						{
							context.Set<T>().Add(entity);
						}
					}

					context.SaveChanges();
				}
			}
		}
	}
}