using Microsoft.AspNetCore.Identity;

namespace ToBee.API.Models
{

	public class ApplicationUser : IdentityUser
	{
		public DateTime? JoinDate { get; set; }
		public string? UserImage { get; set; } 
	}
}
