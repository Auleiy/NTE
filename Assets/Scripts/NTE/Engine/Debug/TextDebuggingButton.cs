using NTE.Engine.Scenario.Text;

using UnityEngine;

namespace NTE.Engine.Debug
{
    public class TextDebuggingButton : MonoBehaviour
    {
        public ScenarioContentText Text;

        public string Name, String;
        public bool showNameDeco = true, showTextDeco = true;

        public void Click()
        {
            Text.Set(String, Name, showTextDeco, showNameDeco);
        }
    }
}