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

		public LoadDataFromDb(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
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

		public async void LoadGroupElementEntityAsync<T>(string getEntityUrl, string groupId) where T : GroupElementEntity
		{
			using (var context = new ApplicationContext())
			{
				var httpHelper = new HttpHelper<List<T>>(_mainWindow, getEntityUrl + groupId);
				var entitiesTask = httpHelper.GetReqAsync();
				List<T>? entities = await entitiesTask;

				if (entities != null)
				{
					var localGroups = context.Set<T>().ToList();

					foreach (var entity in entities)
					{
						if (localGroups.FirstOrDefault(g => g.Id == entity.Id && g.GroupId == entity.GroupId) == null)
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