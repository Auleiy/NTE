using System;

namespace NTE.Command
{
    public abstract class Command
    {
        public readonly CommandParameters paras; // 前面加个@有点太……

        static Command()
        {

        }

        public virtual CommandReturn Main(string[] args)
        {
            if (args.Length < paras.RequiredParametersCount)
                throw new CommandArgumentInsufficientException(CommandRegistry.GetKey(this), paras.RequiredParametersCount, args.Length);
            if (args.Length > paras.Parameters.Count)
                throw new CommandArgumentInsufficientException(CommandRegistry.GetKey(this), paras.Parameters.Count, args.Length);
            for (int i = 0; i < args.Length; i++)
            {

            }
            return null;
        }
    }

    public class CommandException : Exception
    { }

    public class CommandArgumentInsufficientException : CommandException
    {
        public string CommandName;
        public int Required, Provided;

        public CommandArgumentInsufficientException(string cmdName, int required, int provided)
        {
            CommandName = cmdName;
            Required = required;
            Provided = provided;
        }

        public override string Message
        {
            get
            {
                if (Required == Provided)
                    return $"指令{CommandName}在需求参数和提供参数相等的情况下抛出了此异常，属于引擎错误，请报告给 NTE 开发者。";
                if (Required < Provided)
                    return $"指令{CommandName}需要{Required}个参数，但只提供了{Provided}个参数。";
                if (Required > Provided)
                    return $"指令{CommandName}最多能接受{Required}个参数，但提供了{Provided}个参数。";
                return $"指令{CommandName}在判断中进入了绝对不会进入的地方，这是系统错误，重新运行就没问题了。";
            }
        }
    }
}