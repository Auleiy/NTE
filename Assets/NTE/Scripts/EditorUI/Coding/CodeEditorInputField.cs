using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using NTE.EditorUI.Coding.Modifiers;
using NTE.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;
using NTE.Utils;
using Unity.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTE.EditorUI.Coding
{
    public partial class CodeEditorInputField : SelectableRectBehaviour, IUpdateSelectedHandler, IPointerUpHandler
    {
        protected override void Start()
        {
            base.Start();
            UpdateElement();
        }

        public void OnUpdateSelected(BaseEventData e)
        {
            while (Event.PopEvent(EventBus))
            {
                if (EventBus.rawType == EventType.KeyDown)
                    ProcessKey(EventBus);
            }
        }

        public void UpdateElement()
        {
            StringBuilder text = new();
            text.Append(Text); // Make a buffer

            LabelStyle(text);

            TextElement.text = text.ToString();
            TextElement.ForceMeshUpdate();
            CaretPositioning.text = Text + '\u200B';
            CaretPositioning.ForceMeshUpdate();
            CheckSelections();
            UpdateCaret();
        }

        protected void LabelStyle(StringBuilder src)
        {
            string textCache = src.ToString();

            int originLength = textCache.Length;
            int offset = 0;

            void InsertWithOffset(int i, string v)
            {
                src.Insert(i + offset, v);
                offset += v.Length;
            }

            List<Insertion> insertions = new();

            bool inrep = false;
            for (int i = 0; i < originLength; i++)
            {
                if (Replacements.Contains(src[i + offset]) && !inrep)
                {
                    insertions.Add(new(i, $"<color={SpaceColor.GetRichTextColor()}>"));
                    inrep = true;
                }
                else if (!Replacements.Contains(src[i + offset]) && inrep)
                {
                    insertions.Add(new(i, "</color>"));
                    inrep = false;
                }
            }
            foreach (WSDisplay i in Replacements)
            {
                src.Replace(i.Character.ToString(), i.ReplacedCharacter);
            }


            List<(Color c, Match m)> matches = new();

            Regex KeywordRegex = Keywords.GenerateFullRegex();

            foreach (Match m in KeywordRegex.Matches(textCache))
                matches.Add((KeywordColor, m));

            foreach ((Color c, Match m) in matches.OrderBy(x => x.m.Index))
            {
                string start = c.Start(), end = c.End();
                insertions.Add(new(m.Index, start));
                insertions.Add(new(m.Index + m.Length, end));
            }

            insertions.Sort(Insertion.DefaultComparer);

            foreach (Insertion i in insertions)
                InsertWithOffset(i.Index, i.Text);
            return;
        }

        public void ProcessKey(Event e)
        {
            Modifier mod = ModifierUtils.Build(e);
            switch (e.keyCode)
            {
                case KeyCode.Backspace: Backspace(); return;
                case KeyCode.RightArrow: MoveNext(mod); return;
                case KeyCode.LeftArrow: MoveBack(mod); return;
                case KeyCode.UpArrow: MoveUp(mod); return;
                case KeyCode.DownArrow: MoveDown(mod); return;
                case KeyCode.Home: MoveToHome(mod); return;
                case KeyCode.End: MoveToEnd(mod); return;
                case KeyCode.C: if (mod.Ctrl()) { Copy(); return; } break;
                case KeyCode.V: if (mod.Ctrl()) { Paste(); return; } break;
            }


            char keyChar = EventBus.character;
            if (keyChar != '\0')
                Append(keyChar);

            pointerSelecting = null;
        }

        public void Append(char c, bool forward = true)
        {
            if (HasSelection)
            {
                RemoveSelection();
                return;
            }
            Content.Insert(CursorPosition, c);
            if (forward)
                MoveNextSilently(false);
            UpdateElement();
        }

        public void Append(string str, bool forward = true)
        {
            if (HasSelection)
            {
                RemoveSelection();
                return;
            }
            Content.Insert(CursorPosition, str);
            if (forward)
                MoveNextSilently(false, str.Length);
            UpdateElement();
        }

        public void Backspace()
        {
            if (HasSelection)
                RemoveSelection();
            else if (CursorPosition > 0)
            {
                MoveBackSilently(false);
                Delete();
            }
        }

        public void Delete()
        {
            if (HasSelection)
                RemoveSelection();
            else if (CursorPosition < Content.Length)
                Delete(CursorPosition, 1);
        }

        public void RemoveSelection()
        {
            bool cursorStart = CursorPosition < SelectPosition;
            int start = cursorStart ? CursorPosition : SelectPosition;
            int end = cursorStart ? SelectPosition : CursorPosition;
            Delete(start, end - start);
        }

        public void Delete(int start, int length)
        {
            Content.Remove(start, length);
            UpdateElement();
        }

        public void Copy()
        {
            if (HasSelection)
            {
                bool cursorStart = CursorPosition < SelectPosition;
                int start = cursorStart ? CursorPosition : SelectPosition;
                int end = cursorStart ? SelectPosition : CursorPosition;
                GUIUtility.systemCopyBuffer = Text[start..end];
            }
            else
            {
                TMP_LineInfo line = CaretPositioning.textInfo.lineInfo[TextElement.textInfo.characterInfo[CursorPosition].lineNumber];
                int start = line.firstCharacterIndex;
                int end = line.lastCharacterIndex;
                GUIUtility.systemCopyBuffer = '\n' + Text[start..end];
            }
        }

        public void Paste()
        {
            Append(GUIUtility.systemCopyBuffer);
        }

        #region Cursor
        public void MoveNext(Modifier mod, int count = 1)
        {
            MoveNextSilently(mod.Shift(), count);
            UpdateCaret();
        }

        public void MoveBack(Modifier mod, int count = 1)
        {
            MoveBackSilently(mod.Shift(), count);
            UpdateCaret();
        }

        private int cachedOffset = -1;

        public void MoveUp(Modifier mod, int count = 1)
        {
            TMP_CharacterInfo[] chars = CaretPositioning.textInfo.characterInfo;
            TMP_LineInfo[] lines = CaretPositioning.textInfo.lineInfo;
            int line = chars[CursorPosition].lineNumber;
            if (line < count)
            {
                MoveToHome(mod);
                return;
            }
            if (cachedOffset == -1)
                cachedOffset = CursorPosition - lines[line].firstCharacterIndex;
            int ofst = Mathf.Min(lines[line - count].characterCount - 1, cachedOffset);
            int fin = lines[line - count].firstCharacterIndex + ofst;
            MoveTo(mod.Shift(), fin);
        }

        public void MoveDown(Modifier mod, int count = 1)
        {
            TMP_CharacterInfo[] chars = CaretPositioning.textInfo.characterInfo;
            TMP_LineInfo[] lines = CaretPositioning.textInfo.lineInfo;
            int line = chars[CursorPosition].lineNumber;
            if (line >= CaretPositioning.textInfo.lineCount - count)
            {
                MoveToEnd(mod);
                return;
            }
            if (cachedOffset == -1)
                cachedOffset = CursorPosition - lines[line].firstCharacterIndex;
            int ofst = Mathf.Min(lines[line + count].characterCount - 1, cachedOffset);
            int fin = lines[line + count].firstCharacterIndex + ofst;
            MoveTo(mod.Shift(), fin);
        }

        private void MoveNextSilently(bool select, int count = 1)
        {
            CursorPosition += count;
            CheckCursor();
            if (!select)
                SelectPosition = CursorPosition;
            cachedOffset = -1;
        }

        private void MoveBackSilently(bool select, int count = 1)
        {
            CursorPosition -= count;
            CheckCursor();
            if (!select)
                SelectPosition = CursorPosition;
            cachedOffset = -1;
        }

        public void MoveTo(bool select, int index)
        {
            CursorPosition = index;
            CheckCursor();
            if (!select)
                SelectPosition = CursorPosition;
            UpdateCaret();
        }

        public void MoveToHome(Modifier mod)
        {
            if (mod.Ctrl())
                CursorPosition = 0;
            else
            {
                int lineNum = CaretPositioning.textInfo.characterInfo[CursorPosition].lineNumber;
                int pos = CaretPositioning.textInfo.lineInfo[lineNum].firstCharacterIndex;
                CursorPosition = pos;
            }
            if (!mod.Shift())
                SelectPosition = CursorPosition;
            cachedOffset = -1;
            UpdateCaret();
        }

        public void MoveToEnd(Modifier mod)
        {
            if (mod.Ctrl())
                CursorPosition = Content.Length;
            else
            {
                int lineNum = CaretPositioning.textInfo.characterInfo[CursorPosition].lineNumber;
                int pos = CaretPositioning.textInfo.lineInfo[lineNum].lastCharacterIndex;
                CursorPosition = pos;
            }
            if (!mod.Shift())
                SelectPosition = CursorPosition;
            cachedOffset = -1;
            UpdateCaret();
        }

        protected virtual void CheckCursor()
        {
            if (CursorPosition > Content.Length)
                CursorPosition = Content.Length;
            if (CursorPosition < 0)
                CursorPosition = 0;
            if (SelectPosition > Content.Length)
                SelectPosition = Content.Length;
            if (SelectPosition < 0)
                SelectPosition = 0;
        }

        float recentHeight;
        float recentX, recentY;

        public void UpdateCaret()
        {
            TMP_CharacterInfo info = CaretPositioning.textInfo.characterInfo[CursorPosition];
            FaceInfo fontinfo = CaretPositioning.font.faceInfo;

            float ptsize = fontinfo.pointSize;
            float rpsize = TextElement.fontSize;
            float sizeratio = rpsize / ptsize;

            float height = fontinfo.lineHeight * sizeratio;

            float x = CaretPositioning.GetLineLeft(CursorPosition) * sizeratio;
            float y = -info.lineNumber * height;

            if (recentX != x || recentY != y)
            {
                if (CaretTweening != null)
                {
                    CaretTweening.Kill();
                    CaretTweening = null;
                }

                CaretTweening = Caret.DOAnchorPos(new(x, y), 0.1f).SetEase(Ease.OutQuad);
                recentX = x;
                recentY = y;
            }

            if (recentHeight != height)
            {
                if (CaretSizeTweening != null)
                {
                    CaretSizeTweening.Kill();
                    CaretSizeTweening = null;
                }

                CaretSizeTweening = Caret.DOSizeDelta(new(1, height), 0.1f).SetEase(Ease.OutQuad);
                recentHeight = height;
            }

            BlinkStart = Time.unscaledTime;

            UpdateSelection();
        }
        #endregion

        #region Selection
        public void Select(int end)
        {
            SelectPosition = end;
        }

        public void Select(int start, int end, bool rightCursor = true)
        {
            if (rightCursor)
            {
                Select(start);
                CursorPosition = end;
            }
            else
            {
                Select(end);
                CursorPosition = start;
            }
        }

        public void CheckSelections()
        {
            int lineCount = CaretPositioning.textInfo.lineCount;
            for (int i = Selections.Count; i < lineCount; i++)
                CreateSelection();
        }

        public void CreateSelection()
        {
            GameObject obj = new("Selection Line", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            obj.transform.SetParent(SelectionHolder);
            RectTransform t = obj.GetComponent<RectTransform>();
            t.anchoredPosition = new(0, 0);
            t.sizeDelta = new(0, 0);
            t.anchorMin = new(0, 1);
            t.anchorMax = new(0, 1);
            t.pivot = new(0, 1);
            t.rotation = Quaternion.identity;
            t.localScale = new(1, 1, 1);
            obj.GetComponent<RawImage>().color = new(0.25f, 0.75f, 1, 0.25f);
            Selections.Add(t);
        }

        public void ClearSelection()
        {
            foreach (RectTransform t in Selections)
                t.sizeDelta = new(0, 0);
        }

        public void UpdateSelection()
        {
            ClearSelection();
            if (!HasSelection)
                return;
            bool cursorStart = CursorPosition < SelectPosition;
            int start = cursorStart ? CursorPosition : SelectPosition;
            int end = cursorStart ? SelectPosition : CursorPosition;

            TMP_CharacterInfo[] chars = CaretPositioning.textInfo.characterInfo;

            int startLine = chars[start].lineNumber;
            int endLine = chars[end].lineNumber;

            FaceInfo fontinfo = CaretPositioning.font.faceInfo;

            float ptsize = fontinfo.pointSize;
            float rpsize = CaretPositioning.fontSize;

            float sizeratio = rpsize / ptsize;

            float startLeft = CaretPositioning.GetLineLeft(start) * sizeratio;
            float endLeft = CaretPositioning.GetLineLeft(end) * sizeratio;

            float height = fontinfo.lineHeight * sizeratio;

            if (startLine == endLine)
            {
                Selections[startLine].anchoredPosition = new(startLeft, -height * startLine);
                Selections[startLine].sizeDelta = new(endLeft - startLeft, height);
            }
            else
            {
                Selections[startLine].anchoredPosition = new(startLeft, -height * startLine);
                Selections[startLine].sizeDelta = new(CaretPositioning.GetLineEndLeft(startLine) * sizeratio - startLeft, height);
                for (int i = startLine + 1; i < endLine; i++)
                {
                    Selections[i].anchoredPosition = new(0, -height * i);
                    Selections[i].sizeDelta = new(CaretPositioning.GetLineEndLeft(i) * sizeratio, height);
                }
                Selections[endLine].anchoredPosition = new(0, -height * endLine);
                Selections[endLine].sizeDelta = new(endLeft, height);
            }

        }
        #endregion

        PointerEventData pointerSelecting = null;

        public override void OnPointerDown(PointerEventData e)
        {
            base.OnPointerDown(e);
            int index = TMP_TextUtilities.GetCursorIndexFromPosition(CaretPositioning, e.position, e.pressEventCamera, out CaretPosition insSide);
            if (insSide == CaretPosition.Left)
                CursorPosition = CaretPositioning.textInfo.characterInfo[index].index;
            else if (insSide == CaretPosition.Right)
                CursorPosition = CaretPositioning.textInfo.characterInfo[index].index + TextElement.textInfo.characterInfo[index].stringLength;
            if (e.button == PointerEventData.InputButton.Left || e.button == PointerEventData.InputButton.Middle)
            {
                Event.PopEvent(EventBus);
                Modifier mod = ModifierUtils.Build(EventBus);
                if (!mod.Shift())
                    SelectPosition = CursorPosition;
                UpdateCaret();
                cachedOffset = -1;
                pointerSelecting = e;
            }
            else
                pointerSelecting = null;
        }

        public override void OnPointerUp(PointerEventData e)
        {
            base.OnPointerUp(e);
            pointerSelecting = null;
        }

        private void OnGUI()
        {
            CaretImage.enabled = (Time.unscaledTime - BlinkStart) % BlinkRate < BlinkRate / 2;
            if (pointerSelecting != null)
            {
                int index = TMP_TextUtilities.GetCursorIndexFromPosition(CaretPositioning, pointerSelecting.position, pointerSelecting.pressEventCamera, out CaretPosition insSide);
                if (insSide == CaretPosition.Left)
                    CursorPosition = CaretPositioning.textInfo.characterInfo[index].index;
                else if (insSide == CaretPosition.Right)
                    CursorPosition = CaretPositioning.textInfo.characterInfo[index].index + TextElement.textInfo.characterInfo[index].stringLength;
                UpdateCaret();
            }
        }
    }

    internal static class TextUtils
    {
        public static float GetLineLeft(this TextMeshProUGUI text, int index)
        {
            Dictionary<uint, TMP_Character> fonchars = text.font.characterLookupTable;
            TMP_LineInfo[] lines = text.textInfo.lineInfo;
            TMP_CharacterInfo[] chars = text.textInfo.characterInfo;
            int lineIndex = lines[chars[index].lineNumber].firstCharacterIndex;
            float left = 0;
            for (int i = lineIndex; i < index; i++)
                if (fonchars.ContainsKey(chars[i].character))
                    left += fonchars[chars[i].character].glyph.metrics.horizontalAdvance;
            return left;
        }

        public static float GetLineEndLeft(this TextMeshProUGUI text, int line)
        {
            Dictionary<uint, TMP_Character> fonchars = text.font.characterLookupTable;
            TMP_LineInfo[] lines = text.textInfo.lineInfo;
            TMP_CharacterInfo[] chars = text.textInfo.characterInfo;
            int lineIndex = lines[line].firstCharacterIndex;
            int lineEnd = lines[line].lastCharacterIndex;
            float left = 0;
            for (int i = lineIndex; i < lineEnd; i++)
                if (fonchars.ContainsKey(chars[i].character))
                    left += fonchars[chars[i].character].glyph.metrics.horizontalAdvance;
            return left;
        }
    }
}