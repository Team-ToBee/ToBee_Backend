using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToBee.API.Models
{
	public class ProgressReport
	{
		[Key]
		public Guid ReportId { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		[Range(0, double.MaxValue)]
		public double TotalFocusTime { get; set; }

		[Range(0, int.MaxValue)]
		public int TotalTasksCompleted { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
	}
}
