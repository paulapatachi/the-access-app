using System.Text.RegularExpressions;
using MyApp.Domain.Commands;

namespace MyApp.Infrastructure.Helpers;

public static class CommandHelper
{
    // Define a regex pattern to match the command format: CommandName(param1, param2, ...)
    private static readonly Regex CommandRegex = new(
        @"^(?<name>\w+)\((?<params>.*)\)$",
        RegexOptions.Compiled);

    public static GeneralCommand? Parse(string? input)
    {
        if (string.IsNullOrEmpty(input)) return null;
        
        var match = CommandRegex.Match(input.Trim());

        if (!match.Success) return null;

        var name = match.Groups["name"].Value;
        var rawParams = match.Groups["params"].Value;

        var parameters = rawParams.Split(',', StringSplitOptions.TrimEntries).ToArray();

        return new GeneralCommand
        {
            Name = name,
            Parameters = parameters
        };
    }
}