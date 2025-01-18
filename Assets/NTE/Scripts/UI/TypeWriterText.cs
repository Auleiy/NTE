using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;

using UnityEngine;

namespace NTE.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypeWriterText : MonoBehaviour
    {
        public float FadeDuration;
        public float TextInterval;

        public TMP_Text Text => GetComponent<TMP_Text>();

        public Hidable WaitObject;
        private TMP_TextInfo TextInfo => Text.textInfo;

        private bool IsWriting = false;
        private Coroutine PrintCoroutine;

        public void Set(string text)
        {
            Text.text = text;

            if (PrintCoroutine != null)
                StopCoroutine(PrintCoroutine);
            PrintCoroutine = StartCoroutine(PrintText());
        }

        public event Action OnWriteStart, OnWriteFinish;

        private void Awake()
        {
            OnWriteStart += () => WaitObject.Hide();
            OnWriteFinish += () => WaitObject.Show();
        }

        public event OnTextCommandEventHandler OnTextCommand;

        private IEnumerator PrintText()
        {
            IsWriting = true;
            OnWriteStart?.Invoke();
            Text.ForceMeshUpdate();

            for (int i = 0; i < TextInfo.characterCount; i++)
                SetCharAlpha(i, 0);

            int printIndex = 0;
            float ttimer = 0;
            float[] fprg = new float[TextInfo.characterCount];
            HashSet<int> processingCharacter = new() { 0 };

            while (IsWriting)
            {
                ttimer += Time.deltaTime;
                bool FadeComplete = true;
                for (int i = 0; i < fprg.Length; i++)
                {
                    if (!processingCharacter.Contains(i))
                        continue;

                    fprg[i] += Time.deltaTime;
                    byte a = (byte)Mathf.Clamp(fprg[i] / FadeDuration * 255, 0, 255);
                    FadeComplete &= a == 255;
                    SetCharAlpha(i, a);

                    if (fprg[i] >= FadeDuration)
                        processingCharacter.Remove(i);
                }

                Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (ttimer >= TextInterval && printIndex < TextInfo.characterCount - 1)
                {
                    printIndex++;

                    ttimer = 0;
                    processingCharacter.Add(printIndex);
                }

                if (processingCharacter.Count == 0 && printIndex >= TextInfo.characterCount - 1)
                    IsWriting = false;

                yield return null;
            }
            Debug.Log("Write Finish");
            OnWriteFinish?.Invoke();
        }
        private float waitTime;
        private bool waitClick = false;

        public void Click()
        {
            if (waitTime > 0)
                waitTime = 0;
            if (waitClick)
                waitClick = false;
        }

        private void Update()
        {
            if (waitTime > 0)
                waitTime -= Time.deltaTime;
        }

        private void SetCharAlpha(int index, byte a)
        {
            var materialIndex = Text.textInfo.characterInfo[index].materialReferenceIndex;
            var vertexColors = Text.textInfo.meshInfo[materialIndex].colors32;
            var vertexIndex = Text.textInfo.characterInfo[index].vertexIndex;

            vertexColors[vertexIndex + 0].a = a;
            vertexColors[vertexIndex + 1].a = a;
            vertexColors[vertexIndex + 2].a = a;
            vertexColors[vertexIndex + 3].a = a;
        }
    }

    public delegate void OnTextCommandEventHandler(OnTextCommandEventArgs e);

    public class OnTextCommandEventArgs : EventArgs
    {
        public string Data;
        public bool WaitForClick = false;
        public float WaitTime = 0;
    }
}
