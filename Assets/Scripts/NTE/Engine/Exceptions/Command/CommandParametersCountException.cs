namespace NTE.Engine.Exceptions.Command
{
    public class CommandParametersCountException : CommandException
    {
        public int Required, Provided;

        public CommandParametersCountException(string cmdName, int required, int provided) : base(cmdName)
        {
            Required = required;
            Provided = provided;
        }

        public override string Message
        {
            get
            {
                if (Required > Provided)
                    return $"指令{CommandName}需要{Required}个参数，但提供了{Provided}个参数。";
                if (Required < Provided)
                    return $"指令{CommandName}最多能接受{Required}个参数，但提供了{Provided}个参数。";
                return $"指令{CommandName}在判断中进入了绝对不会进入的地方，这是系统错误，重新运行就没问题了。"; // 我是不是闲的
            }
        }
    }
}
