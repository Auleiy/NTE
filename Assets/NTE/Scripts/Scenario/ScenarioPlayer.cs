using System.Collections;
using System.IO;

using NTE.Command;
using NTE.Scenario.CG;
using NTE.UI;

using TMPro;

using UnityEditor;

using UnityEngine;

namespace NTE.Scenario
{
    /// <summary>
    /// 剧情播放器
    /// </summary>
    public class ScenarioPlayer : MonoBehaviour
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public TMP_Text Name;
        /// <summary>
        /// 显示文本前缀
        /// </summary>
        public TMP_Text ContentPrefix;
        /// <summary>
        /// 显示文本
        /// </summary>
        public TypeWriterText Text;
        /// <summary>
        /// CG管理器（暂时未使用）
        /// </summary>
        public CGManager CGManager;
        /// <summary>
        /// 文本栏可隐藏组件
        /// </summary>
        public Hidable TextBar;

        /// <summary>
        /// 角色名称前缀
        /// </summary>
        public string NameFormatPre = "【 ";
        /// <summary>
        /// 角色名称后缀
        /// </summary>
        public string NameFormatSuf = " 】";
        /// <summary>
        /// 文本前缀
        /// </summary>
        public string TextFormatPre = "「";
        /// <summary>
        /// 文本后缀
        /// </summary>
        public string TextFormatSuf = "」";

        /// <summary>
        /// 当前的剧情播放器
        /// </summary>
        public static ScenarioPlayer Current;

        /// <summary>
        /// 正在播放的文件
        /// </summary>
        public StreamReader File
        {
            get => File_P;
            set { File_P = value; StartScenario(); }
        }

        /// <summary>
        /// 正在播放的文件
        /// </summary>
        private StreamReader File_P;
        /// <summary>
        /// 播放剧情使用的携程
        /// </summary>
        private Coroutine ExecutionCoroutine;

        /// <summary>
        /// 当点击屏幕时
        /// </summary>
        public void Click()
        {
            if (!WaitingForTypingFinish)
                WaitingForClick = false;
        }

        /// <summary>
        /// 从头开始剧情
        /// </summary>
        private void StartScenario()
        {
            if (ExecutionCoroutine != null)
                StopCoroutine(ExecutionCoroutine);
            ExecutionCoroutine = StartCoroutine(Execute());
        }

        private int index;

        /// <summary>
        /// 执行剧情的携程
        /// </summary>
        private IEnumerator Execute()
        {
            for (index = 0; !File.EndOfStream; index++)
            {
                string cmd = File.ReadLine();
                CommandReturn ret = CommandRegistry.Invoke(cmd);

                // 等待时间，因为部分指令等到时会隐藏文本框，因此后面会再设置显示。
                yield return new WaitForSeconds(ret.WaitTime);
                TextBar.Show();

                // 等待点击
                WaitingForClick = ret.WaitForClick;

                while (WaitingForClick)
                    yield return null;
            }
            SetThinkText("<color=yellow>[文件末尾无结束指令，将会在5秒后退出]</color>");
            // 我也不知道为啥这里关闭文件貌似不起作用
            File.Close();
        }

        public void ErrorText(string err)
        {
            SetThinkText($"<color=red>[{((FileStream)File.BaseStream).Name}第{index + 1}行发生错误：{err}]</color>");
        }

        private void Awake()
        {
            Current = this;
            CommandRegistry.RegisterDefault();

            Text.OnWriteFinish += () => WaitingForTypingFinish = false;
#if UNITY_EDITOR
            // 如果不关的话退出游戏模式不会自动关闭，文件就会一直被占用，导致编辑器无法修改
            EditorApplication.playModeStateChanged += (e) =>
            {
                if (e == PlayModeStateChange.ExitingPlayMode)
                {
                    Debug.Log("检测到退出游戏模式，关闭文件中");
                    File.Close();
                }
            };
#endif
        }

        private bool WaitingForClick = false, WaitingForTypingFinish = false;
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="chname">角色名</param>
        /// <param name="text">文本</param>
        public void SetText(string chname, string text)
        {
            Name.text = $"{NameFormatPre}{chname}{NameFormatSuf}";
            ContentPrefix.text = TextFormatPre;
            Text.Set($"{text}{TextFormatSuf}");
            ContentPrefix.CrossFadeAlpha(1, Text.FadeDuration, false);
            WaitingForTypingFinish = true;
        }

        /// <summary>
        /// 设置内心想法文本
        /// </summary>
        /// <param name="text">文本</param>
        public void SetThinkText(string text)
        {
            Name.text = string.Empty;
            Text.Set($"{text}");
            ContentPrefix.CrossFadeAlpha(0, Text.FadeDuration, false);
            WaitingForTypingFinish = true;
        }
    }
}