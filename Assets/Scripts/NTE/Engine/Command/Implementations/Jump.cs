using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations
{
    public class Jump : Command
    {
        public override string EditorName => "Ìø×ªµ½";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.Int();
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            env.Script.Jump(Params.Get<int>(args, 0));
            return CommandReturn.ImmediatlyEvent;
        }
    }
}