using System;
using System.Collections.Generic;
using System.Linq;
using NTE.Command.Implementations;
using NTE.Scenario;
using NTE.Utils;

namespace NTE.Command
{
    public static class CommandRegistry
    {
        public static Dictionary<string, Command> Commands = new();

        public static void Register(string name, Command command) => Commands.Add(name, command);
        public static CommandReturn Invoke(string name, string[] args)
        {
            try
            {
                return Commands[name].Main(args);
            }
            catch (CommandException ex)
            {
                ScenarioPlayer.Current.ErrorText(ex.Message);
                return CommandReturn.BlockClick;
            }
            catch (Exception ex)
            {
                ScenarioPlayer.Current.ErrorText($"（引擎异常）{ex.Message}");
                return CommandReturn.BlockClick;
            }
        }

        public static string GetKey(Command cmd)
        {
            return (from x in Commands where x.Value == cmd select x.Key).First();
        }

        public static CommandReturn Invoke(string raw)
        {
            string[] rawArgs = raw.SplitString(' ');
            return Invoke(rawArgs[0], rawArgs[1..]);
        }

        public static void RegisterDefault()
        {
            Register("say", new Say());
        }
    }
}