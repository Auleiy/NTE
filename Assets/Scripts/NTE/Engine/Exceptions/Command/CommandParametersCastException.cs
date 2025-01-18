namespace NTE.Engine.Exceptions.Command
{
    public class CommandParametersTypeException : CommandException
    {
        public string RequiredType, Information;
        public int Index;

        public CommandParametersTypeException(string cmdName, string requiredType, string info, int index) : base(cmdName)
        {
            RequiredType = requiredType;
            Information = info;
            Index = index;
        }

        public override string Message => $"指令{CommandName}的第{Index}个参数不符合{RequiredType}类型。详细信息：{Information}";
    }
}