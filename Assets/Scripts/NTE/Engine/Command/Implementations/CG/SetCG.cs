using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations.CG
{
    public class SetCG : Command
    {
        public override string EditorName => "CG:…Ë÷√";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.String(string.Empty); // CG ID
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            env.CGManager.Set(args[0]);
            return CommandReturn.ImmediatlyEvent;
        }
    }
}