using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations
{
    public class Wait : Command
    {
        public override string EditorName => "等待";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.Float(); // 时长
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            return new(Params.Get<float>(args, 0), false);
        }
    }
}