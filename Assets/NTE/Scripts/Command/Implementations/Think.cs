using NTE.Scenario;

namespace NTE.Command.Implementations
{
    public class Think : Command
    {
        public override CommandReturn Main(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentInsufficientException("think", 1, args.Length);
            ScenarioPlayer.Current.SetThinkText(args[0]);
            return CommandReturn.BlockClick;
        }
    }
}