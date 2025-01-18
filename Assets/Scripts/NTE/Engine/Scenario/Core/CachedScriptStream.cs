using System.IO;

using NTE.Engine.Command;

namespace NTE.Engine.Scenario.Core
{
    public class CachedScriptStream
    {
        public readonly string[] Content;
        public int Position { get; private set; } = 0;
        public bool EndOfScript => Position >= Content.Length;
        public readonly string ScriptPath;

        private ScenarioPlayer Player;

        public CachedScriptStream(string path, ScenarioPlayer player)
        {
            Content = File.ReadAllLines(path);
            Player = player;
            ScriptPath = path;
        }

        public CommandReturn Execute(string cmd)
        {
            return CommandRegistry.ParseAndExecute(cmd, Player);
        }

        public CommandReturn ExecuteNext()
        {
            if (EndOfScript)
                throw new EndOfStreamException();
            return Execute(Content[Position++]);
        }

        public void Jump(int lineNum)
        {
            if (lineNum < 0 || lineNum > Content.Length)
            {
                Player.ErrorText("跳转位置应该属于(0, 文件行数]区间。");
                throw new EndOfStreamException("跳转位置应该属于(0, 文件行数]区间。");
            }
            Position = lineNum - 1;
        }
    }
}
