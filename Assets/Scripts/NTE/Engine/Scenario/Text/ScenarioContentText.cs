using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using NTE.Core.UI;
using NTE.Core.Utils;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Engine.Scenario.Text
{
    public class ScenarioContentText : MonoBehaviour
    {
        public delegate void WriteEvent();

        public TMP_Text NamePrefix, NameContent, NameSuffix;
        public TMP_Text TextPrefix, TextContent; // 我做不到TextSuffix，TextMeshPro没给获取字符位置的东西

        public string TextPrefixText = "「", TextSuffixText = "」";

        public float FadeDuration = 0.1f;
        public float TextInterval = 0.05f;

        public float DecoMovement = 448, DecoMinMovement = 0.3f;

        // 我们要记住，游戏内编辑器无法直接编辑场景。
        public string NamePrefixText = "【";
        public string NameSuffixText = "】";
        public float NamePrefixSpacing = 32, NameSuffixSpacing = 32;
        public float NameFontSize = 56;

        private TMP_TextInfo TextInfo => TextContent.textInfo;

        private bool IsWriting = false;
        private Coroutine PrintCoroutine;

        bool showNameDeco = true, showTextDeco = true;

        public void Set(string text, string name = "", bool textDeco = false, bool nameDeco = false)
        {
            NameContent.fontSize = NameFontSize;
            NamePrefix.fontSize = NameFontSize;
            NameSuffix.fontSize = NameFontSize;

            NameContent.text = name;
            TextContent.text = text;

            if (showNameDeco != nameDeco)
                NamePrefix.GetComponent<Hidable>().Select(NameSuffix.GetComponent<Hidable>().Select(showNameDeco = nameDeco)); // 为了不要大括号我可以做到一切（好奇怪）

            if (showTextDeco != textDeco)
                TextPrefix.GetComponent<Hidable>().Select(showTextDeco = textDeco);

            float tweenDur(float to, float pre) => Mathf.Max(DecoMinMovement, Mathf.Abs(to - pre) / DecoMovement); // 强制平均 1 秒移动 DecoMovement 个画布单位（非线性动画）

            if (showNameDeco)
            {
                NamePrefix.text = NamePrefixText;
                NamePrefix.ForceMeshUpdate();
                NameSuffix.text = NameSuffixText;
                NameSuffix.ForceMeshUpdate();

                // 改变 Margin 后不会自动刷新布局数据（如 PereferredWidth）（TextMesh Pro我***）
                LayoutRebuilder.ForceRebuildLayoutImmediate(NamePrefix.rectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(NameContent.rectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(NameSuffix.rectTransform);

                float prefixWidth = NamePrefix.preferredWidth + NamePrefixSpacing;
                // Tween 没完成的时候 NameContent 的 PereferredWidth 与 Margin 与 PrefixWidth 不相等
                float contentWidth = NameContent.preferredWidth - NameContent.margin.x + prefixWidth + NameSuffixSpacing;
                NameContent.DOLeft(prefixWidth, tweenDur(NameContent.margin.x, prefixWidth)).SetEase(Ease.OutCubic);
                NameSuffix.DOLeft(contentWidth, tweenDur(NameSuffix.margin.x, contentWidth)).SetEase(Ease.OutCubic);
            }
            if (showTextDeco)
            {
                TextPrefix.text = TextPrefixText;
                TextPrefix.ForceMeshUpdate();
                float prefixWidth = TextPrefix.preferredWidth;
                TextContent.DOLeft(prefixWidth, tweenDur(TextContent.margin.x, prefixWidth)).SetEase(Ease.OutCubic);
                TextContent.text = text + TextSuffixText;
            }

            if (PrintCoroutine != null)
                StopCoroutine(PrintCoroutine);
            PrintCoroutine = StartCoroutine(PrintText());
        }

        private IEnumerator PrintText()
        {
            IsWriting = true;
            OnWriteStart?.Invoke();
            TextContent.ForceMeshUpdate();

            for (int i = 0; i < TextInfo.characterCount; i++)
                SetIndexAlpha(i, 0);

            int headIndex = 0;
            float textTimer = 0;
            float[] fadeProgress = new float[TextInfo.characterCount];
            List<int> fadingIndex = new() { 0 };

            while (IsWriting)
            {
                textTimer += Time.deltaTime;
                for (int i = 0; i < fadingIndex.Count; i++)
                {
                    int index = fadingIndex[i];

                    fadeProgress[index] += Time.deltaTime;
                    byte a = (byte)Mathf.Clamp(fadeProgress[index] / FadeDuration * 255, 0, 255);
                    SetIndexAlpha(index, a);

                    if (a == 255) // 避免在卡顿时出现半透明字符
                        fadingIndex.Remove(index);
                }

                if (textTimer >= TextInterval && headIndex < TextInfo.characterCount - 1)
                {
                    headIndex++;
                    textTimer = 0;
                    if (!char.IsWhiteSpace(TextInfo.characterInfo[headIndex].character)) // 空白字符不会生成面，会导致后面的字符显示两次而闪烁
                        fadingIndex.Add(headIndex);
                }

                if (fadingIndex.Count == 0 && headIndex >= TextInfo.characterCount - 1)
                    IsWriting = false;

                TextContent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return null;
            }
            OnWriteFinish?.Invoke();
        }

        private void SetIndexAlpha(int index, byte a)
        {
            var materialIndex = TextContent.textInfo.characterInfo[index].materialReferenceIndex;
            var vertexColors = TextContent.textInfo.meshInfo[materialIndex].colors32;
            var vertexIndex = TextContent.textInfo.characterInfo[index].vertexIndex;

            vertexColors[vertexIndex + 0].a = a;
            vertexColors[vertexIndex + 1].a = a;
            vertexColors[vertexIndex + 2].a = a;
            vertexColors[vertexIndex + 3].a = a;
        }

        public event WriteEvent OnWriteStart, OnWriteFinish;
    }
}
