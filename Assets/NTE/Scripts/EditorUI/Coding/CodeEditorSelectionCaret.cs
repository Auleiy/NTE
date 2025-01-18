using UnityEngine;
using UnityEngine.UI;

namespace NTE.EditorUI.Coding
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class CodeEditorSelectionCaret : MaskableGraphic
    {
        public override void Cull(Rect clipRect, bool validRect)
        {
            if (validRect)
            {
                canvasRenderer.cull = false;
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }

            base.Cull(clipRect, validRect);
        }

        protected override void UpdateGeometry()
        { }
    }
}