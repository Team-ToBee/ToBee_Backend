namespace ToBee.API.Dtos.RewardDtos
{
	public class RewardDto
	{
		public Guid RewardId { get; set; }
		public string UserId { get; set; }
		public int Points { get; set; }
		public string BadgeEarned { get; set; }
		public DateTime DateEarned { get; set; }
	}

}
