using System;
using System.Collections.Generic;
using System.Linq;

using NTE.Core.Utils;
using NTE.Engine.Command.Implementations;
using NTE.Engine.Debug;
using NTE.Engine.Exceptions;
using NTE.Engine.Scenario;

using CG = NTE.Engine.Command.Implementations.CG;

namespace NTE.Engine.Command
{
    public static class CommandRegistry
    {
        public static readonly Dictionary<string, Command> Commands = new();

        public static readonly Command SayCommand = Register("say", new Say());
        public static readonly Command ThinkCommand = Register("think", new Think());
        public static readonly Command WaitCommand = Register("wait", new Wait());
        public static readonly Command JumpCommand = Register("jump", new Jump());

        public static readonly Command CG_SetCGCommand = Register("cg.setCg", new CG.SetCG());
        public static readonly Command CG_SetCGVariantCommand = Register("cg.setVariant", new CG.SetCGVariant());
        public static readonly Command CG_SetCGStartValueCommand = Register("cg.setStartValue", new CG.SetCGStartValue());

        public static Command Register(string name, Command command)
        {
            Commands.Add(name, command);
            return command;
        }

        public static string GetKey(Command cmd)
        {
            return (from x in Commands where x.Value == cmd select x.Key).First();
        }

        public static Command Get(string cmd) => Commands[cmd];

        public static CommandReturn ParseAndExecute(string text, ScenarioPlayer env)
        {
            try
            {
                string t = text.Trim();
                string[] args = NTEUtils.SplitString(text, ' ');
                Command type = Commands[args[0]];
                return type.Execute(args[1..], env);
            }
            catch (ProjectException ex)
            {
                env.ErrorText(ex.Message);
                Log.Exception(ex);
                return CommandReturn.BlockClick;
            }
            catch (Exception ex)
            {
                env.ErrorText($"[引擎异常] {ex.Message}");
                throw;
            }
        }
    }
}