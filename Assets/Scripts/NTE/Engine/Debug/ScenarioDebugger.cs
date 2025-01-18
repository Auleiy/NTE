using DG.Tweening;

using NTE.Core.UI;
using NTE.Engine.Scenario;

using TMPro;

using UnityEngine;

namespace NTE.Engine.Debug
{
    public class ScenarioDebugger : MonoBehaviour
    {
        public Hidable ScenarioToolkitGroup;

        public ScenarioPlayer Player;

        public TMP_InputField Name, Text, CGPath, CGLayer, CGVariant;

        public RectTransform ToggleMark;

        public void ToggleShow()
        {
            ToggleMark.DOScaleX(ScenarioToolkitGroup.Toggle() ? -1 : 1, 0.2f).SetEase(Ease.InOutCubic); // 我在干什么……
        }

        public void ApplyText()
        {
            if (string.IsNullOrEmpty(Name.text))
                Player.SetThinkText(Text.text);
            else
                Player.SetText(Name.text, Text.text);
        }

        public void ApplyCG()
        {
            Player.SetCG(CGPath.text);
        }

        public void SetCGOn()
        {
            Player.SetCGOn(CGLayer.text, CGVariant.text);
        }

        public void SetCGDefault()
        {
            Player.CGManager.AddStartValue(CGLayer.text, CGVariant.text);
        }
    }
}
