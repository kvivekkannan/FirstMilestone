namespace FirstMilestone.Api.Entities;

public class DashboardModule
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "▦";
    public bool IsInstalled { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
