using System.Collections;

using NTE.UI;

using UnityEngine;

namespace NTE.EditorUI.Layout
{
    public class DockLayout : RectBehaviour
    {
        public bool VerticalSplit;

        [Range(0, 1)]
        public float Min, Max;

        public float ASize
        {
            get => m_ASize;
            set
            {
                if (value < Min)
                    m_ASize = Min;
                else if (value > Max)
                    m_ASize = Max;
                else
                    m_ASize = value;
                UpdateLayout();
            }
        }

        [SerializeField, Range(0, 1)]
        private float m_ASize;

        public RectTransform ATransform, BTransform, DraggingBorder;

#if UNITY_EDITOR
        private void OnValidate()
        {
            StartCoroutine(UpdateSize());
        }

        private IEnumerator UpdateSize()
        {
            yield return null;
            UpdateLayout();
        }
#endif

        public void UpdateLayout()
        {
            if (VerticalSplit)
            {
                if (ATransform != null)
                    ATransform.sizeDelta = new(ATransform.sizeDelta.x, transform.rect.height * m_ASize);
                if (BTransform != null)
                    BTransform.sizeDelta = new(BTransform.sizeDelta.x, transform.rect.height * (1 - m_ASize));
                if (DraggingBorder != null)
                    DraggingBorder.anchoredPosition = new(DraggingBorder.anchoredPosition.x, -transform.rect.height * m_ASize);
            }
            else
            {
                if (ATransform != null)
                    ATransform.sizeDelta = new(transform.rect.width * m_ASize, ATransform.sizeDelta.y);
                if (BTransform != null)
                    BTransform.sizeDelta = new(transform.rect.width * (1 - m_ASize), BTransform.sizeDelta.y);
                if (DraggingBorder != null)
                    DraggingBorder.anchoredPosition = new(transform.rect.width * m_ASize, DraggingBorder.anchoredPosition.y);
            }
        }
    }
}
