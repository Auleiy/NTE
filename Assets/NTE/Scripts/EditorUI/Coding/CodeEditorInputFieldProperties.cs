using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using NTE.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NTE.EditorUI.Coding
{
    public partial class CodeEditorInputField
    {
        public StringBuilder Content = new();
        public string Text
        {
            get { return Content.ToString(); }
            set { Content = new(value); }
        }

        public TextMeshProUGUI TextElement;
        public TextMeshProUGUI CaretPositioning;
        public RectTransform Caret;

        public RectTransform SelectionHolder;
        public List<RectTransform> Selections;

        public int CursorPosition = 0;
        public int SelectPosition = 0;

        public float BlinkRate = 0.5f;

        private readonly Event EventBus = new();

        private Tween CaretTweening, CaretSizeTweening;
        private float BlinkStart = 0;

        public bool HasSelection => CursorPosition != SelectPosition;

        public bool ShowSpace = true;
        public Color SpaceColor = new(1, 1, 1, 0.1f);

        #region Highlight
        [InspectorLabel("Highlight")]
        public List<string> Keywords;
        public Color KeywordColor;
        #endregion

        private RawImage CaretImage => Caret.GetComponent<RawImage>();

        private WSDisplay[] Replacements = {
            WSDisplay.Space,
            WSDisplay.Enter
        };

        public struct WSDisplay
        {
            public char Character;
            public string ReplacedCharacter; // Nerd font supportation

            public static readonly WSDisplay Space = new(' ', "·");
            public static readonly WSDisplay Enter = new('\n', "󱞥\n");

            public WSDisplay(char ch, string repch)
            {
                Character = ch;
                ReplacedCharacter = repch;
            }
        }

        public struct Insertion
        {
            public int Index;
            public string Text;

            public static readonly Comparer DefaultComparer = new();

            public Insertion(int index, string text)
            {
                Index = index;
                Text = text;
            }

            public class Comparer : IComparer<Insertion>
            {
                public int Compare(Insertion x, Insertion y)
                {
                    return x.Index - y.Index;
                }
            }
        }
    }
    internal static class InternalUtils
    {
        public static Regex GenerateFullRegex(this List<string> l)
        {
            StringBuilder sb = new("(?<!.)(");
            foreach (string h in l)
            {
                sb.Append(h);
                sb.Append("|");
            };
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return new(sb.ToString(), RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public static string Start(this Color c) => $"<color={c.GetRichTextColor()}>";

        public static string End(this Color c) => "</color>";
    }
}