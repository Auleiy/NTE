using System;
using Unity.VisualScripting;
using UnityEngine;

namespace NTE.Editor.UI.Coding.Modifiers
{
    [Flags]
    public enum Modifier
    {
        None = 0b000,
        Ctrl = 0b001,
        Shift = 0b010,
        Alt = 0b100,
    }

    public static class ModifierUtils
    {
        public static bool Ctrl(this Modifier modifier)
        {
            return modifier.HasFlag(Modifier.Ctrl);
        }

        public static bool Shift(this Modifier modifier)
        {
            return modifier.HasFlag(Modifier.Shift);
        }

        public static bool Alt(this Modifier modifier)
        {
            return modifier.HasFlag(Modifier.Alt);
        }

        public static Modifier Build(Event e)
        {
            Modifier ret = Modifier.None;
            if (e.CtrlOrCmd())
                ret |= Modifier.Ctrl;
            if (e.shift)
                ret |= Modifier.Shift;
            if (e.alt)
                ret |= Modifier.Alt;
            return ret;
        }
    }
}