using Microsoft.AspNetCore.Identity;

namespace UserService.API.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Role { get; set; } = string.Empty; // "Professional", "Student", "Parent"
		public bool FocusModeStatus { get; set; }
		public DateTime JoinDate { get; set; }
	}
}
