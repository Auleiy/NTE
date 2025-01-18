namespace NTE.Command.Implementations
{
    public class Wait : Command
    {
        public override CommandReturn Main(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentInsufficientException("wait", 1, args.Length);
            return new(float.Parse(args[0]), false);
        }
    }
}