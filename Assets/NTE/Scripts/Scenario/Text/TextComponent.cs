using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace NTE.Scenario.Text
{
    public class TextCollection : List<object>
    {

    }

    public class StringComponent
    {
        public string text;
    }

    public static class ComponentRegistry
    {
        public static Dictionary<string, Type> components;
        public static void Register<T>(string name)
        {
            components.Add(name, typeof(T));
        }

        public static Type Get(string name)
        {
            return components.TryGetValue(name, out Type type) ? type : throw new ArgumentOutOfRangeException(name);
        }
    }

    public abstract class Component
    {
        public Dictionary<string, object> Arguments;

        public Component(params string[] args)
        {

        }

        public static (Type, object) Convert(string text)
        {
            string trimed = text[1..^2];
            string[] spl = trimed.Split(' ');

            string name = spl[0];

            Type t = ComponentRegistry.Get(name);

            Component comp = (Component)t.Instantiate();

            return (t, comp);
        }
    }

    public class WaitComponent
    {

    }
}