using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations.CG
{
    public class SetCGStartValue : Command
    {
        public override string EditorName => "CG:设置初始变种";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.String(); // CG层ID
            builder.String(); // CG变种ID
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            env.CGManager.AddStartValue(args[0], args[1]);
            return CommandReturn.ImmediatlyEvent;
        }
    }
}