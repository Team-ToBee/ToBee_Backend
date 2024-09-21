public class PomodoroSessionResponseDto
{
	public Guid SessionId { get; set; }
	public string UserId { get; set; }
	public Guid TaskId { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public int BreakDuration { get; set; }
	public string Status { get; set; }
}
