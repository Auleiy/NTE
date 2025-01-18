using System.Collections.Generic;

namespace NTE.Configuration
{
    public static class Configuration
    {
        #region Configuration Properties
        public static List<ConfigItem> Items;
        #endregion

        public static int Vowel(this string str, char target)
        {
            int v = 0;
            foreach (char c in str)
            {
                if (c == target)
                    v++;
            }
            return v;
        }
    }
}
