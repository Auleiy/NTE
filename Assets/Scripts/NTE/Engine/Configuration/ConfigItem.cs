using System;

using UnityEngine;

namespace NTE.Engine.Configuration
{
    [Obsolete]
    public class ConfigItem
    {
        public string Name;
        public string RawText { get; private set; }

        public string AsString()
        {
            if ((RawText.StartsWith('\"') && RawText.EndsWith('\"')) ||
                (RawText.StartsWith('\'') && RawText.EndsWith('\'')))
                return RawText[1..^1];
            return RawText;
        }
        public float AsFloat()
        {
            if (float.TryParse(RawText, out float value))
                return value;
            throw new ArgumentException("Value is not a float value.");
        }
        public bool AsBool()
        {
            if (bool.TryParse(RawText, out bool value))
                return value;
            throw new ArgumentException("Value is not a bool value.");
        }
        public T AsEnumValue<T>() where T : struct
        {
            if (Enum.TryParse(RawText, out T ve))
                return ve;
            else if (int.TryParse(RawText, out int vi))
                return (T)Enum.ToObject(typeof(T), vi);
            throw new ArgumentException("Value is out of the enum.");
        }
        public Color AsColor()
        {
            byte r = Convert.ToByte(RawText[..2], 16);
            byte g = Convert.ToByte(RawText[2..4], 16);
            byte b = Convert.ToByte(RawText[4..6], 16);
            byte a = Convert.ToByte(RawText[6..8], 16);

            return new Color32(r, g, b, a);
        }
    }
}
