using System;
using System.ComponentModel.DataAnnotations;

namespace ToBee.API.Models.DTOs
{
	public class PomodoroSessionCreateDto
	{
		[Required]
		public Guid TaskId { get; set; }

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		[Range(0, int.MaxValue)]
		public int BreakDuration { get; set; }

		[Required]
		public string Status { get; set; }
	}
}
