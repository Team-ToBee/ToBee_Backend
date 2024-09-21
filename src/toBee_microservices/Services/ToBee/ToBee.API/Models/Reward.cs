using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToBee.API.Models
{
	public class Reward
	{
		[Key]
		public Guid RewardId { get; set; }

		[Required]
		public string UserId { get; set; }

		[Range(0, int.MaxValue)]
		public int Points { get; set; }

		[StringLength(100)]
		public string BadgeEarned { get; set; }

		public DateTime DateEarned { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
	}
}
