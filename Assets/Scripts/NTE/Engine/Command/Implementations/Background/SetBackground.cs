using NTE.Core.Project.Resource;
using NTE.Engine.Scenario;

using UnityEngine;

namespace NTE.Engine.Command.Implementations.Background
{
    public class SetBackground : Command
    {
        public override string EditorName => "背景:设置";

        public override void BuildParameter(CommandParameters.Builder builder)
        {
            builder.String(); // 背景名
        }

        public override CommandReturn Main(string[] args, ScenarioPlayer env)
        {
            Texture2D tex = ResourceManager.GetSprite(args[0]);
            return CommandReturn.BlockClick;
        }
    }
}