using NTE.Core.UI;
using NTE.Engine.Command;

namespace NTE.Editor.UI.VisualEditor
{
    public class Line : RectBehaviour
    {
        public int LineNumber = 1;

        public EditorDataReader.Command Command;

        public static void Create(EditorDataReader.Command cmd)
        {
             
        }
    }
}