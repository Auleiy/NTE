namespace NTE.Command
{
    public class CommandReturn
    {
        public float WaitTime;
        public bool WaitForClick;

        public CommandReturn(float waitTime, bool waitForClick = false)
        {
            WaitTime = waitTime;
            WaitForClick = waitForClick;
        }

        public static CommandReturn ImmediatlyEvent = new(0, false);
        public static CommandReturn BlockClick = new(0, true);
    }
}
