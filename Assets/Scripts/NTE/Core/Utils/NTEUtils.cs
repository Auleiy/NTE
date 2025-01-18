using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using NTE.Core.UI;
using NTE.Editor.UI.Coding;
using NTE.Editor.UI.FileTree;

using TMPro;

using UnityEditor;

using UnityEngine;

namespace NTE.Core.Utils
{
    public static class NTEUtils
    {
        public static void WaitInvoke(this MonoBehaviour o, Action act, float waitTime) => o.StartCoroutine(WaitInvoke_G(act, waitTime));

        private static IEnumerator WaitInvoke_G(Action a, float b)
        {
            yield return new WaitForSeconds(b);
            a();
        }

        public static string[] SplitString(this string v, char sep)
        {
            List<string> ret = new();
            bool strStarted = false;
            StringBuilder cur = new();
            foreach (char c in v)
            {
                if (c == '"')
                {
                    strStarted = !strStarted;
                    continue;
                }
                if (!strStarted && c == sep)
                {
                    ret.Add(cur.ToString());
                    cur.Clear();
                    continue;
                }
                cur.Append(c);
            }
            ret.Add(cur.ToString());
            return ret.ToArray();
        }

        public static Texture2D GetIcon(this FileType type)
        {
            return type switch
            {
                FileType.Root => Resources.Load<Texture2D>("EditorIcon/TGProject"),
                FileType.ResourcesDirectory => Resources.Load<Texture2D>("EditorIcon/TGResources"),
                FileType.ScenariosDirectory => Resources.Load<Texture2D>("EditorIcon/TGScenarios"),
                FileType.ConfigsDirectory => Resources.Load<Texture2D>("EditorIcon/TGConfigs"),
                FileType.CGConfig => Resources.Load<Texture2D>("EditorIcon/TGConfig"),
                FileType.CGConfigs => Resources.Load<Texture2D>("EditorIcon/TGDirectory"),
                FileType.CharacterConfig => Resources.Load<Texture2D>("EditorIcon/TGConfig"),
                FileType.CharacterConfigs => Resources.Load<Texture2D>("EditorIcon/TGDirectory"),
                FileType.Sprite => Resources.Load<Texture2D>("EditorIcon/TGSprite"),
                FileType.Sprites => Resources.Load<Texture2D>("EditorIcon/TGResources"),
                FileType.SpritesChildDirectory => Resources.Load<Texture2D>("EditorIcon/TGDirectory"),
                FileType.Vocal => Resources.Load<Texture2D>("EditorIcon/TGVocal"),
                FileType.Vocals => Resources.Load<Texture2D>("EditorIcon/TGResources"),
                FileType.VocalsChildDirectory => Resources.Load<Texture2D>("EditorIcon/TGDirectory"),
                FileType.ScenarioChapter => Resources.Load<Texture2D>("EditorIcon/TGChapter"),
                FileType.ScenarioScript => Resources.Load<Texture2D>("EditorIcon/TGScript"),
                FileType.UnknownDirectory => Resources.Load<Texture2D>("EditorIcon/TGDirectory"),
                FileType.UnknownFile => Resources.Load<Texture2D>("EditorIcon/TGFile"),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public static string GetRelativePathOf(this string abs, string root)
        {
            List<string> aspl = abs.Split('\\', '/').ToList();
            List<string> rspl = root.Split('\\', '/').ToList();
            bool rel = false;
            StringBuilder sb = new();
            for (int i = 0; i < aspl.Count; i++)
            {
                if (!rel && (i >= rspl.Count || aspl[i] != rspl[i]))
                    rel = true;
                if (rel)
                {
                    sb.Append(aspl[i]);
                    if (i < aspl.Count - 1)
                        sb.Append('/');
                }
            }
            return sb.ToString();
        }

        private static readonly Regex StartingZeroRegex = new(@"(?<![0-9])0+");

        public static string ToRichString(this int value, int integerDigits)
        {
            StringBuilder sb = new(value.ToString($"D{integerDigits}"));
            Match m = StartingZeroRegex.Match(sb.ToString());
            if (m.Success)
            {
                sb.Insert(m.Index, "<color=#222>");
                sb.Insert(m.Index + 12 + m.Length, "</color>");
            }
            return sb.ToString();
        }

        public static string[] GetKeywordRegex(this string[] kw)
        {
            string[] ret = new string[kw.Length];
            for (int i = 0; i < kw.Length; i++)
                ret[i] = $@"(?<!\([^\)]*){kw[i]}(?=\([\S\s]*\))";
            return ret;
        }

        public static bool IsNullOrEmpty(this string v) => string.IsNullOrEmpty(v);

        public static bool Ctrl(this EventModifiers m) => (m & EventModifiers.Control) != 0;
        public static bool Shift(this EventModifiers m) => (m & EventModifiers.Shift) != 0;
        public static bool Alt(this EventModifiers m) => (m & EventModifiers.Alt) != 0;

        public static string ToHex(this Color color) => $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}{(int)(color.a * 255):X2}";

        public static bool Contains(this CodeEditorInputField.WSDisplay[] array, char c)
        {
            foreach (var i in array)
                if (i.Character == c)
                    return true;
            return false;
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DOLeft(this TMP_Text text, float dest, float duration)
        {
            return DOTween.To(() => text.margin, x => text.margin = x, new(dest, 0, 0, 0), duration);
        }

        public static bool IsDirectory(this string path)
        {
            return Directory.Exists(path);
        }

        public static void CopyFrom(this RectTransform to, RectTransform from)
        {
            to.anchoredPosition = from.anchoredPosition;
            to.anchorMin = from.anchorMin;
            to.anchorMax = from.anchorMax;
            to.pivot = from.pivot;
            to.sizeDelta = from.sizeDelta;
            to.rotation = from.rotation;
            to.localScale = from.localScale;
            to.SetParent(from.parent);
        }

        public static void FullRectTransform(this RectTransform trans)
        {
            trans.anchorMin = new(0, 0);
            trans.anchorMax = new(1, 1);
            trans.offsetMin = new(0, 0);
            trans.offsetMax = new(0, 0);
        }

        public static void AsNonInteractive(this Hidable h)
        {
            h.ChangeBlockRaycasts = false;
            h.ChangeInteractable = false;
        }

        public static void InitNonInteractiveHiding(this Hidable h)
        {
            h.HideImmediate();
            h.AsNonInteractive();
        }

        public static IEnumerable<int> IndexesOf(this string str, char c)
        {
            for (int i = 0; i < str.Length; )
            {
                i = str.IndexOf(c, i);
                if (i == -1)
                    break;

                yield return i;
                i++;
            }
        }

        /// <summary>
        /// 自动根据环境退出游戏
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 一个空函数，用来在调试中打断点
        /// </summary>
#if UNITY_EDITOR
        public static void Breakpoint() { }
#endif
    }
}
