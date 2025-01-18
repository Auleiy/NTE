using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using NTE.Core.Utils;
using NTE.Editor.UI.Tab;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

namespace NTE.Editor.UI.Coding
{
    public class CodeEditor : TabEditor
    {
        public TMP_Text DisplayText;
        public TMP_Text LineNumber;
        public RectTransform ScrollContent;

        public string[] Keywords;
        public ArgumentsHighlighter[] EnumArguments;
        public Highlight KeywordColor;
        public Highlight ArgumentColor;
        public Highlight StringColor;
        public Highlight NumberColor;

        private readonly Regex StringRegex = new(@"(""""[\S\s]*?"""")|("".*?"")");
        private readonly Regex NumberRegex = new(@"(?<!"".*|\S)([0-9]*)(?!\S)");

        public void OnValueChanged(string text)
        {
            text = text.Replace('\v', '\n');

            StringBuilder sb = new(text);
            List<(Highlight h, Match m)> matches = new();

            Regex KeywordRegex = RegexList.CreateFromStringArray(Keywords.GetKeywordRegex()).RegexExpression;
            Regex EnumArgumentsRegex = RegexList.CreateFromEnumArguments(EnumArguments).RegexExpression;

            foreach (Match m in KeywordRegex.Matches(text))
                matches.Add((KeywordColor, m));
            foreach (Match m in EnumArgumentsRegex.Matches(text))
                matches.Add((ArgumentColor, m));
            foreach (Match m in NumberRegex.Matches(text))
                matches.Add((NumberColor, m));
            foreach (Match m in StringRegex.Matches(text))
                matches.Add((StringColor, m));

            int ofs = 0;
            foreach ((Highlight h, Match m) in matches.OrderBy(x => x.m.Index))
            {
                string start = h.Start, end = h.End;
                sb.Insert(m.Index + ofs, start);
                ofs += start.Length;
                sb.Insert(m.Index + ofs + m.Length, end);
                ofs += end.Length;
            }

            sb.Append("\u200B");

            StringBuilder lnsb = new();
            for (int i = 1; i <= text.Split('\n').Length; i++)
                lnsb.AppendLine(i.ToRichString(9));
            LineNumber.text = lnsb.ToString();

            DisplayText.text = sb.ToString();

            ScrollContent.sizeDelta = new(ScrollContent.sizeDelta.x, DisplayText.preferredHeight);

            Edit();
        }
    }

    [Serializable]
    public class ArgumentsHighlighter
    {
        public string Command;
        public string[] Arguments;

        public string RegexExpression
        {
            get
            {
                StringBuilder sb = new($@"(?<=(?<!\([^\)]*){Command}(\(|[\S\s]*,\s*))(");
                for (int i = 0; i < Arguments.Length; i++)
                {
                    sb.Append(Arguments[i]);
                    if (i != Arguments.Length - 1)
                        sb.Append("|");
                }
                sb.Append(@")(?=:[\S\s]*)");
                return sb.ToString();
            }
        }
    }

    [Serializable]
    public class RegexList : List<string>
    {
        public RegexList() : base() { }
        public RegexList(IEnumerable<string> b) : base(b) { }

        public Regex RegexExpression
        {
            get
            {
                StringBuilder sb = new();
                for (int i = 0; i < Count; i++)
                {
                    sb.Append(this[i]);
                    if (i != Count - 1)
                        sb.Append('|');
                }
                return new(sb.ToString());
            }
        }

        public static RegexList CreateFromStringArray(string[] str) => new(str);

        public static RegexList CreateFromEnumArguments(ArgumentsHighlighter[] args)
        {
            RegexList l = new();
            foreach (ArgumentsHighlighter arg in args)
                l.Add(arg.RegexExpression);
            return l;
        }
    }

    [Serializable]
    public class Highlight
    {
        public Color Color;
        public bool Italic, Bold;

        public string Start
        {
            get
            {
                StringBuilder sb = new($"<color=#{Color.ToHexString()}>");
                if (Italic)
                    sb.Append($"<i>");
                if (Bold)
                    sb.Append($"<b>");
                return sb.ToString();
            }
        }

        public string End
        {
            get
            {
                StringBuilder sb = new($"</color>");
                if (Italic)
                    sb.Append($"</i>");
                if (Bold)
                    sb.Append($"</b>");
                return sb.ToString();
            }
        }
    }
}
