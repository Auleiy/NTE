using System;
using System.Collections.Generic;
using System.IO;

using NTE.Utils;

namespace NTE.Scenario.CG
{
    public class CGConfig
    {
        public string Key;
        public Image Base;
        public Dictionary<string, Image> Layers = new();

        public CGConfig(string filename)
        {
            StreamReader sr = new(filename);

            Key = sr.ReadLine();
            Base = new(sr.ReadLine(), out string key);
            if (key != "base")
                throw new Exception("The second line should be base image.");
            while (!sr.EndOfStream)
            {
                Image i = new(sr.ReadLine(), out key);
                Layers.Add(key, i);
            }

            sr.Close();
            sr.Dispose();
        }
    }
}