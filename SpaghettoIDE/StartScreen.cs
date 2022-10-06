using SpaghettoIDE.Controls;
using SpaghettoIDE.ProjectManaging;
using SpaghettoIDE.Utils;
using System.Runtime.InteropServices;

namespace SpaghettoIDE {
    public partial class StartScreen : Form {
        public static StartScreen Instance { get; private set; }
        private double ratio;

        public StartScreen() {
            Instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (!WindowHelpers.UseImmersiveDarkMode(Handle, true)) MessageBox.Show(null, "Enabling dark window mode failed. This might be caused by an outdated OS. This message should not be considered a SpaghettoIDE bug on Windows versions older than Windows 10 Anniversary Update.", "Error");

            ratio = Width / Height;
            WelcomeHistory.AddEntry(new() { FileName = "amogus", FullName = "C:\\amogus.sideprj", Path = "C:\\" });
            WelcomeHistory hist = WelcomeHistory.Load();

            flowLayoutPanel1.SuspendLayout();

            foreach(ProjectHistoryEntry ent in hist.Entries) {
                ProjectHistoryEntryUI el = new();
                el.UpdateUI(ent);
                el.Visible = true;
                el.Dock = DockStyle.Left;
                el.Location = new(0, 0);
                el.Size = new(flowLayoutPanel1.Width-40, el.Size.Height);
                flowLayoutPanel1.Controls.Add(el);
            }

            this.Refresh();

            flowLayoutPanel1.ResumeLayout();
        }

        internal void OpenProject(ProjectHistoryEntry entry) {
            throw new NotImplementedException();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}