using NTE.Engine.Scenario;

namespace NTE.Engine.Command.Implementations
{
    public class Say : Command
    {
        public override string EditorName => "角色说话";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.String(); // 角色名
            builder.String(); // 文本内容
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            ScenarioPlayer.Instance.SetText(args[0], args[1]);
            return CommandReturn.BlockClick;
        }
    }
}