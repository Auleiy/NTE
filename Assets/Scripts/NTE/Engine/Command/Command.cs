using NTE.Engine.Scenario;

namespace NTE.Engine.Command
{
    public abstract class Command
    {
        public virtual string EditorName => CommandRegistry.GetKey(this);

        protected readonly CommandParameters Params; // 前面加个@有点太……

        public Command()
        {
            CommandParameters.Builder builder = new();
            BuildParameter(builder);
            Params = builder.Build(this);
        }

        public virtual void BuildParameter(CommandParameters.Builder builder) { }

        public virtual CommandReturn Execute(string[] args, ScenarioPlayer env)
        {
            return Main(Params.Validate(args), env);
        }

        public abstract CommandReturn Main(string[] args, ScenarioPlayer env);
    }
}