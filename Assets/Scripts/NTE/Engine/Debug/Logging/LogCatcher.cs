using System;
using System.Text;

using NTE.Core.Utils;

using TMPro;

using UnityEngine;

namespace NTE.Engine.Debug
{
    public class Log : Singleton<Log>
    {
        public TextMeshProUGUI TextUI;

        public StringBuilder LogText = new();

        public event Application.LogCallback OnLogError;

        protected override void Awake()
        {
            base.Awake();
            Application.logMessageReceived += Handle + OnLogError;
        }

        public static void Warn(string text) => Handle(text, LogType.Warning);

        public static void Handle(string text, LogType type)
        {
            string color = type switch
            {
                LogType.Error => "#923",
                LogType.Assert => "#712",
                LogType.Exception => "#a34",
                LogType.Warning => "#a93",
                LogType.Log or _ => "#fff"
            };
            Instance.LogText.AppendLine($"<color={color}>[{type}] {text}</color>");
            Instance.RefreshText();
        }

        public static void Handle(string text, string stackTrace, LogType type)
        {
            string color = type switch
            {
                LogType.Error => "#923",
                LogType.Assert => "#712",
                LogType.Exception => "#a34",
                LogType.Warning => "#a93",
                LogType.Log or _ => "#fff"
            };
            Instance.LogText.AppendLine($"<color={color}>[{type}] {text}");
            if (type != LogType.Error || type != LogType.Exception)
                foreach (string s in stackTrace.Split('\n'))
                    if (!string.IsNullOrEmpty(s))
                        Instance.LogText.AppendLine($"  at {s}");
            Instance.LogText.Append("</color>");
            Instance.RefreshText();
        }

        public static void Exception(Exception ex)
        {
            Handle(ex.Message, ex.StackTrace, LogType.Error);
        }

        public void RefreshText()
        {
            TextUI.text = LogText.ToString();
        }
    }
}
