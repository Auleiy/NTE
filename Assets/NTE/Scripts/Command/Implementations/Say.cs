using NTE.Scenario;

namespace NTE.Command.Implementations
{
    public class Say : Command
    {


        public override CommandReturn Main(string[] args)
        {
            base.Main(args);
            ScenarioPlayer.Current.SetText(args[0], args[1]);
            return CommandReturn.BlockClick;
        }
    }
}