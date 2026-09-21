namespace FirstMilestone.Api.DTOs;

public record ModuleResponse(int Id, string Key, string Name, string Description, string Icon, bool IsInstalled);
