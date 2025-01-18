using System.Collections.Generic;
using System.IO;
using NTE.Command;
using NTE.Command.Implementations;
using NTE.Scenario;
using NTE.Scenario.CG;

using UnityEngine;

namespace NTE.Project
{
    public class ProjectData : MonoBehaviour
    {
        public static ProjectData Current;

        public string DataPath;
        public ScenarioPlayer Player;
        public List<string> Routes = new() { "Common" };

        public Dictionary<string, CGConfig> Configs = new();

        private void Start()
        {
            Init();
        }

        private void Awake()
        {
            Current = this;
        }

        public void Init(int routeindex = 0, int chapter = 0, int part = 0, int line = 0)
        {
            /*
#if !UNITY_EDITOR
            DataPath = Path.Combine(Environment.CurrentDirectory, "ProjectData");
#endif
*/

            Read(routeindex, chapter, part, line);
        }

        public void Read(int routeindex, int chapter, int part, int line = 0)
        {
            string path = GetScenarioPath(routeindex, chapter, part);
            StreamReader scnFile = File.OpenText(path);
            for (int i = 0; i < line; i++)
                scnFile.ReadLine();
            Player.File = scnFile;
        }

        public string GetScenarioPath(int routeindex, int chapter, int part) => Path.Combine(DataPath, "Scenarios", Routes[routeindex], $"{chapter}", $"{part}.nte");
    }
}
