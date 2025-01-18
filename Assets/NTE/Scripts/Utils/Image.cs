using System.IO;

using NTE.Project;

using UnityEngine;

namespace NTE.Utils
{
    public class Image
    {
        public Texture2D Raw;
        public float X, Y;
        public int Left, Top, Width, Height;

        private Texture2D cache;

        public Texture2D GetProceedTexture()
        {
            if (cache == null)
            {
                if (Left == 0 && Top == 0 && Width == Raw.width && Height == Raw.height)
                {
                    cache = Raw;
                    return cache;
                }

                Texture2D p = new(Width, Height);
                Color[] colors = Raw.GetPixels(Left, Top, Width, Height);
                p.SetPixels(colors);
                p.Apply();
                cache = p;
            }
            return cache;
        }

        public Image(string data, out string key)
        {
            string[] spl = data.SplitString(' ');

            key = spl[0];

            byte[] raw = File.ReadAllBytes(Path.Combine(ProjectData.Current.DataPath, @"Resources\Sprites", spl[1]));
            Raw = new(0, 0);
            Raw.LoadImage(raw);

            X = float.Parse(spl[2]);
            Y = float.Parse(spl[3]);
            Left = int.Parse(spl[4]);
            Top = int.Parse(spl[5]);
            Width = int.Parse(spl[6]);
            Height = int.Parse(spl[7]);
        }
    }
}
