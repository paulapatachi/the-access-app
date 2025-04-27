namespace MyApp.Domain.Commands;

public class GeneralCommand
{
    public string Name { get; set; } = string.Empty;
    public string[] Parameters { get; set; } = [];
}