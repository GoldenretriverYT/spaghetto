using SpaghettoIDE.ProjectManaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaghettoIDE.Controls {
    public partial class ProjectHistoryEntryUI : UserControl {
        public ProjectHistoryEntry Entry { get; set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );


        public ProjectHistoryEntryUI() {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void ProjectHistoryEntry_Click(object sender, EventArgs e) {
            StartScreen.Instance.OpenProject(Entry);
        }

        private void ProjectHistoryEntryUI_Load(object sender, EventArgs e) {

        }

        public void UpdateUI(ProjectHistoryEntry entry) {
            this.Entry = entry;
            this.label1.Text = Entry.FileName;
            this.label2.Text = Entry.Path;
        }

        private void ProjectHistoryEntryUI_Resize(object sender, EventArgs e) {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
    }
}
