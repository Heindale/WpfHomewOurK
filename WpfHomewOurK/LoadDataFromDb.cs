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
		public List<int> GroupsId { get; private set; }

		public LoadDataFromDb(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;

		}

		public async Task LoadDataAsync(string[] paths)
		{
			User? user;

			HttpHelper<User> httpUserHelper = new HttpHelper<User>(_mainWindow, paths[0]);
			var userTask = httpUserHelper.GetReqAsync();
			user = await userTask;

			if (user != null)
			{
				await LoadBaseEntityAsync<Group>(paths[1], user.Id.ToString());

				if (GroupsId != null && GroupsId.Count > 0)
					foreach (var groupId in GroupsId)
					{
						LoadGroupElementEntityAsync<Teacher>(paths[2], groupId.ToString());
						LoadGroupElementEntityAsync<Subject>(paths[3], groupId.ToString());
						LoadGroupElementEntityAsync<Attachment>(paths[4], groupId.ToString());
						using (ApplicationContext context = new ApplicationContext())
						{
							foreach (var subject in context.Subjects)
							{
								LoadSubjectElementEntityAsync<Homework>(paths[5], groupId.ToString(), subject.Id.ToString());
							}
						}
					};
			}
		}

		public async Task LoadBaseEntityAsync<T>(string getEntityUrl, string userId) where T : BaseEntity
		{
			using (var context = new ApplicationContext())
			{
				var httpHelper = new HttpHelper<List<T>>(_mainWindow, getEntityUrl + userId);
				var entitiesTask = httpHelper.GetReqAsync();
				List<T>? entities = await entitiesTask;
				if (entities != null)
					GroupsId = entities.Select(x => x.Id).ToList();

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

		public async void LoadSubjectElementEntityAsync<T>(string getEntityUrl, string groupId, string subjectId) where T : SubjectElementEntity
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
						if (localGroups.FirstOrDefault(g => g.Id == entity.Id && g.GroupId == entity.GroupId && g.SubjectId == entity.SubjectId) == null)
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