using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using TMPro;

using UnityEngine;

namespace NTE.Core.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class RichTextHelper : MonoBehaviour
    {
        public TMP_Text Text => GetComponent<TMP_Text>();

        public string RawText
        {
            get => m_RawText; set
            {
                m_RawText = value;
                UpdateText();
            }
        }
        private string m_RawText;
        public ObservableCollection<RichTag> Tags = new();

        private void Awake()
        {
            Tags.CollectionChanged += UpdateText;
        }

        private void UpdateText(object sender, NotifyCollectionChangedEventArgs e) => UpdateText();

        private void UpdateText()
        {
            StringBuilder sb = new(m_RawText);
            int offset = 0;
            foreach (RichTag t in Tags.OrderBy(x => x.Start))
            {
                sb.Insert(t.Start + offset, t.StartText);
                offset += t.StartText.Length;
                sb.Insert(t.End + offset, t.EndText);
                offset += t.EndText.Length;
            }
        }
    }

    public struct RichTag
    {
        public string TagName;
        public string Value;
        public int Start, End;

        private readonly string ValueSuffix => string.IsNullOrEmpty(Value) ? "" : $"={Value}";

        public readonly string StartText => $"<{TagName}{ValueSuffix}>";
        public readonly string EndText => $"</{TagName}>";
    }
}