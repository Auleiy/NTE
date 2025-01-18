using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EditorNeo : TMP_InputField
{
    private char CompletingText;

    private static readonly Dictionary<char, char> CompletableText = new() {
        { '"', '"' },
        { '\'', '\'' },
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
    };
    private bool DoubleQuote = false;

    protected override void Append(char input)
    {
        if (input == CompletingText)
        {
            if (input == '"' && !DoubleQuote)
            {
                DoubleQuote = true;
                goto append;
            }
            else
                MoveRight(input);
            FinishAutoCompletion(input);
            return;
        }
        else
            DoubleQuote = true;
        append:
        base.Append(input);
        if (CompletableText.TryGetValue(input, out char v))
            AppendAutoCompletion(v);
    }

    private void FinishAutoCompletion(char input)
    {
        MoveRight(input);
        UnComplete();
    }

    private void UnComplete()
    {
        DoubleQuote = false;
        CompletingText = '\0';
    }

    private void MoveRight(char input)
    {
        if (!char.IsHighSurrogate(input))
            m_CaretSelectPosition = ++m_CaretPosition;
        m_StringSelectPosition = ++m_StringPosition;
    }

    private void OnGUI()
    {
        if (Input.anyKey)
        {
            Event e = Event.current;
            if (e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Home || e.keyCode == KeyCode.End ||
                e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.DownArrow)
                UnComplete();

            EventModifiers modifiers = e.modifiers;
            bool ctrl = (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX) ? ((modifiers & EventModifiers.Command) != 0) : ((modifiers & EventModifiers.Control) != 0);
            bool shift = (modifiers & EventModifiers.Shift) != 0;
            bool alt = (modifiers & EventModifiers.Alt) != 0;

            if (!ctrl && shift && alt)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    int lineNumber = m_TextComponent.textInfo.characterInfo[caretPositionInternal].lineNumber;
                    int f = m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex;
                    int l = m_TextComponent.textInfo.lineInfo[lineNumber].lastCharacterIndex;
                    string s = text[f..l];
                    text = text.Insert(f, "\n");
                    text = text.Insert(f, s);
                }
                if (e.keyCode == KeyCode.DownArrow)
                {
                    int lineNumber = m_TextComponent.textInfo.characterInfo[caretPositionInternal].lineNumber;
                    int f = m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex;
                    int l = m_TextComponent.textInfo.lineInfo[lineNumber].lastCharacterIndex;
                    string s = text[f..l];
                    text = text.Insert(l, "\n");
                    text = text.Insert(l + 1, s);
                }
            }
        }
    }

    protected void AppendAutoCompletion(char input)
    {
        CompletingText = input;
        base.Append(input);
        if (!char.IsHighSurrogate(input))
            m_CaretSelectPosition = --m_CaretPosition;
        m_StringSelectPosition = --m_StringPosition;
    }

    protected override void Append(string input)
    {
        char[] inputc = input.ToCharArray();

        CompletingText = '\0';

        int num = Mathf.Min(stringPositionInternal, stringSelectPositionInternal);
        string text = this.text;
        if (selectionFocusPosition != selectionAnchorPosition)
        {
            if (m_isRichTextEditingAllowed || m_isSelectAll)
            {
                text = (m_StringPosition >= m_StringSelectPosition) ? this.text.Remove(m_StringSelectPosition, m_StringPosition - m_StringSelectPosition) : this.text.Remove(m_StringPosition, m_StringSelectPosition - m_StringPosition);
            }
            else if (m_CaretPosition < m_CaretSelectPosition)
            {
                m_StringPosition = m_TextComponent.textInfo.characterInfo[m_CaretPosition].index;
                m_StringSelectPosition = m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition - 1].index + m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition - 1].stringLength;
                text = this.text.Remove(m_StringPosition, m_StringSelectPosition - m_StringPosition);
            }
            else
            {
                m_StringPosition = m_TextComponent.textInfo.characterInfo[m_CaretPosition - 1].index + m_TextComponent.textInfo.characterInfo[m_CaretPosition - 1].stringLength;
                m_StringSelectPosition = m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition].index;
                text = this.text.Remove(m_StringSelectPosition, m_StringPosition - m_StringSelectPosition);
            }
        }

        if (onValidateInput != null)
        {
            for (int i = 0; i < inputc.Length; i++)
                inputc[i] = onValidateInput(text, num, inputc[i]);
        }
        else
        {
            if (characterValidation == CharacterValidation.CustomValidator)
            {
                for (int i = 0; i < inputc.Length; i++)
                    inputc[i] = Validate(text, num, inputc[i]);
                onValueChanged?.Invoke(text);
                UpdateLabel();

                return;
            }

            if (characterValidation != 0)
                for (int i = 0; i < inputc.Length; i++)
                    inputc[i] = Validate(text, num, inputc[i]);
        }

        Insert(new(inputc));
    }

    private void Insert(string str)
    {
        Delete();
        if (characterLimit <= 0 || text.Length < characterLimit)
        {
            m_Text = text.Insert(m_StringPosition, str);
            foreach (char c in str)
            {
                if (!char.IsHighSurrogate(c))
                    m_CaretSelectPosition = ++m_CaretPosition;
                m_StringSelectPosition = ++m_StringPosition;
            }
            onValueChanged?.Invoke(text);
        }
    }

    private void Delete()
    {
        if (m_StringPosition == m_StringSelectPosition)
            return;

        if (m_isRichTextEditingAllowed || m_isSelectAll)
        {
            if (m_StringPosition < m_StringSelectPosition)
            {
                m_Text = text.Remove(m_StringPosition, m_StringSelectPosition - m_StringPosition);
                m_StringSelectPosition = m_StringPosition;
            }
            else
            {
                m_Text = text.Remove(m_StringSelectPosition, m_StringPosition - m_StringSelectPosition);
                m_StringPosition = m_StringSelectPosition;
            }

            if (m_isSelectAll)
            {
                m_CaretPosition = m_CaretSelectPosition = 0;
                m_isSelectAll = false;
            }
        }
        else if (m_CaretPosition < m_CaretSelectPosition)
        {
            m_StringPosition = m_TextComponent.textInfo.characterInfo[m_CaretPosition].index;
            m_StringSelectPosition = m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition - 1].index + m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition - 1].stringLength;
            m_Text = text.Remove(m_StringPosition, m_StringSelectPosition - m_StringPosition);
            m_StringSelectPosition = m_StringPosition;
            m_CaretSelectPosition = m_CaretPosition;
        }
        else
        {
            m_StringPosition = m_TextComponent.textInfo.characterInfo[m_CaretPosition - 1].index + m_TextComponent.textInfo.characterInfo[m_CaretPosition - 1].stringLength;
            m_StringSelectPosition = m_TextComponent.textInfo.characterInfo[m_CaretSelectPosition].index;
            m_Text = text.Remove(m_StringSelectPosition, m_StringPosition - m_StringSelectPosition);
            m_StringPosition = m_StringSelectPosition;
            m_CaretPosition = m_CaretSelectPosition;
        }
        onValueChanged?.Invoke(text);
        UpdateLabel();
    }
}
