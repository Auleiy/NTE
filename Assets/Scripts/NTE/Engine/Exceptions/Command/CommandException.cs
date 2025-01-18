namespace NTE.Engine.Exceptions.Command
{
    public class CommandException : ProjectException
    {
        public string CommandName;

        public CommandException(string cmdName)
        {
            CommandName = cmdName;
        }

        public override string Message => $"指令{CommandName}抛出未知异常";
    }
}