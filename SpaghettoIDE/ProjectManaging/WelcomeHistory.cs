using Newtonsoft.Json;
using SpaghettoIDE.Utils.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaghettoIDE.ProjectManaging {
    internal class WelcomeHistory {
        public List<ProjectHistoryEntry> Entries { get; set; }
        
        private WelcomeHistory() {
            this.Entries = new();
        }

        public static WelcomeHistory Load() {
            WelcomeHistory wHist = new();
            string data = FileHelpers.ReadAllTextOrDefault("history.json", "[]");

            wHist.Entries = JsonConvert.DeserializeObject<List<ProjectHistoryEntry>>(data);

            return wHist;
        }

        public static void AddEntry(ProjectHistoryEntry ent) {
            WelcomeHistory wHist = new();
            string data = FileHelpers.ReadAllTextOrDefault("history.json", "[]");

            wHist.Entries = JsonConvert.DeserializeObject<List<ProjectHistoryEntry>>(data);
            wHist.Entries.Add(ent);

            File.WriteAllText("history.json", JsonConvert.SerializeObject(wHist.Entries));
        }
    }

    public class ProjectHistoryEntry {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string FullName { get; set; }
    }
}
