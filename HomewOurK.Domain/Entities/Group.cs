using HomewOurK.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HomewOurK.Domain.Entities
{
	public class Group : BaseEntity
	{
		[Required]
		[StringLength(25)]
		public string Name { get; set; } = "";

		[Required]
		[StringLength(50)]
		public string? UniqGroupName { get; set; }

		[Range(0, 99)]
		public int? Grade { get; set; }

		[StringLength(25)]
		public string? GroupType { get; set; }
	}
}