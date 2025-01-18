using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations
{
    public class Think : Command
    {
        public override string EditorName => "内心想";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.String(); // 文本内容
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            ScenarioPlayer.Instance.SetThinkText(args[0]);
            return CommandReturn.BlockClick;
        }
    }
}