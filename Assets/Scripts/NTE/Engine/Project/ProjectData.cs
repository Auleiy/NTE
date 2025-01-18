using System.Collections.Generic;
using System.IO;

using NTE.Core.Project.Resource;
using NTE.Engine.Scenario;

using UnityEngine;

namespace NTE.Engine.Project
{
    public class ProjectData : MonoBehaviour
    {
        public static ProjectData Current;

        public string DataPath;
        public ScenarioPlayer Player;
        public List<string> Routes = new() { "Common" };

        private void Start()
        {
            ResourceManager.LoadCGs();
            Init();
        }

        private void Awake()
        {
            Current = this;
        }

        public void Init(int routeindex = 0, int chapter = 0, int part = 0)
        {
            /*
#if !UNITY_EDITOR
            DataPath = Path.Combine(Environment.CurrentDirectory, "ProjectData");
#endif
*/

            Read(routeindex, chapter, part);
        }

        public void Read(int routeindex, int chapter, int part)
        {
            string path = GetScenarioPath(routeindex, chapter, part);
            Player.StartScenario(path);
        }

        public string GetScenarioPath(int routeindex, int chapter, int part) => Path.Combine(DataPath, "Scenarios", Routes[routeindex], $"{chapter}", $"{part}.nte");

        public string GetAbsolutePath(string path) => Path.Combine(DataPath, path);
    }
}
