using System.Collections;

using NTE.Core.UI;
using NTE.Core.Utils;
using NTE.Engine.Command;
using NTE.Engine.Scenario.CG;
using NTE.Engine.Scenario.Core;
using NTE.Engine.Scenario.Text;

using UnityEngine;

namespace NTE.Engine.Scenario
{
    /// <summary>
    /// 剧情播放器
    /// </summary>
    public class ScenarioPlayer : Singleton<ScenarioPlayer>
    {
        /// <summary>
        /// 文本管理器
        /// </summary>
        public ScenarioContentText Text;
        /// <summary>
        /// 文本栏可隐藏组件
        /// </summary>
        public Hidable TextBar;

        public CGManager CGManager;

        private Coroutine ExecutionCoroutine;

        public void Click()
        {
            if (!WaitingForTypingFinish)
                WaitingForClick = false;
        }

        /// <summary>
        /// 从头开始剧情
        /// </summary>
        public void StartScenario(string path)
        {
            Script = new(path, this);
            if (ExecutionCoroutine != null)
                StopCoroutine(ExecutionCoroutine);
            ExecutionCoroutine = StartCoroutine(Execute());
        }

        public CachedScriptStream Script;

        /// <summary>
        /// 执行剧情的携程
        /// </summary>
        private IEnumerator Execute()
        {
            while (!Script.EndOfScript)
            {
                CommandReturn ret = Script.ExecuteNext();

                // 等待时间，因为部分指令等到时会隐藏文本框，因此后面会再设置显示。
                if (ret.WaitTime > 0)
                    yield return new WaitForSeconds(ret.WaitTime);
                TextBar.Show();

                // 等待点击
                WaitingForClick = ret.WaitForClick;

                while (WaitingForClick)
                    yield return null;
            }
            SetThinkText("<color=#a93>文件末尾无结束指令，将会在5秒后退出</color>");

            yield return new WaitForSeconds(5);

            NTEUtils.Quit();
        }

        /// <summary>
        /// 设置错误文本
        /// </summary>
        /// <param name="err">错误内容</param>
        public void ErrorText(string err)
        {
            SetThinkText($"<color=#a34>{Script.ScriptPath} {Script.Position + 1}行: {err}</color>");
        }

        protected override void Awake()
        {
            base.Awake();
            Text.OnWriteFinish += () => WaitingForTypingFinish = false;
        }

        private bool WaitingForClick = false, WaitingForTypingFinish = false;
        /// <summary>
        /// 设置文本（带有角色名称的显示）
        /// </summary>
        /// <param name="chname">角色名</param>
        /// <param name="text">文本</param>
        public void SetText(string chname, string text)
        {
            Text.Set(text, chname, true, true); // 待改为配置模式
            WaitingForTypingFinish = true;
        }

        /// <summary>
        /// 设置想法/调试文本（不带有角色名称的显示，并且根据配置不显示文本的装饰）
        /// </summary>
        /// <param name="text">文本</param>
        public void SetThinkText(string text)
        {
            Text.Set(text); // 待改为配置模式
            WaitingForTypingFinish = true;
        }

        public void SetCG(string name)
        {
            CGManager.Set(name);
        }

        public void SetCGOn(string layer, string variant)
        {
            CGManager.SetOn(layer, variant);
        }
    }
}