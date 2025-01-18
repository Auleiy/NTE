using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Editor.UI.Coding
{
    public class EditorCursor : Graphic
    {
        public int Position;
        public TMP_Text Text;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            UIVertex[] verts = new UIVertex[4];

            int lineNumber = Text.textInfo.characterInfo[Position].lineNumber;
            TMP_CharacterInfo ch;
            float ad;
            Vector2 origin;
            if (Position == Text.textInfo.lineInfo[lineNumber].firstCharacterIndex)
            {
                ch = Text.textInfo.characterInfo[Position];
                ad = ch.ascender - ch.descender;
                origin = (Text.verticalAlignment != VerticalAlignmentOptions.Geometry) ? new Vector2(ch.origin, ch.descender) : new Vector2(ch.origin, 0f - ad / 2f);
            }
            else
            {
                ch = Text.textInfo.characterInfo[Position - 1];
                ad = ch.ascender - ch.descender;
                origin = (Text.verticalAlignment != VerticalAlignmentOptions.Geometry) ? new Vector2(ch.xAdvance, ch.descender) : new Vector2(ch.xAdvance, 0f - ad / 2f);
            }

            float down = origin.y + ad;
            float top = down - ad;

            verts[0].position = new Vector3(origin.x, top, 0f);
            verts[1].position = new Vector3(origin.x, down, 0f);
            verts[2].position = new Vector3(origin.x + 1, down, 0f);
            verts[3].position = new Vector3(origin.x + 1, top, 0f);
            verts[0].color = Color.white;
            verts[1].color = Color.white;
            verts[2].color = Color.white;
            verts[3].color = Color.white;
            vh.AddUIVertexQuad(verts);
        }
    }
}
