using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Shared shape of every subcommand that takes one target argument, some extras and a precondition.
/// </summary>
internal static class TargetCommandBuilder
{
	/// <summary>
	/// Builds the command. The precondition runs before anything is resolved, so bad input is
	/// reported as such rather than as a missing token or an empty discovery.
	/// </summary>
	internal static Command Build(
		string name,
		string description,
		Argument<string> targetArgument,
		Argument[] extraArguments,
		Option[] options,
		Func<ParseResult, bool> precondition,
		Func<ParseResult, string, CancellationToken, Task> run)
	{
		var command = new Command(name, description);

		command.Arguments.Add(targetArgument);
		foreach (var argument in extraArguments)
		{
			command.Arguments.Add(argument);
		}

		foreach (var option in options)
		{
			command.Options.Add(option);
		}

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			if (precondition(parseResult))
			{
				await run(parseResult, parseResult.GetValue(targetArgument)!, cancellationToken);
			}
		});

		return command;
	}

	internal static Option<double> CreateDurationOption()
		=> new("--duration", "-d")
		{
			Description = "Transition duration in seconds",
			DefaultValueFactory = _ => 1.0
		};
}
