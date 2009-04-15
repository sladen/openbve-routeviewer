using System;
using System.Windows.Forms;

namespace OpenBve {
    public partial class formMessages : Form {
        public formMessages() {
            InitializeComponent();
        }

        // show messages
        internal static DialogResult ShowMessages() {
            formMessages Dialog = new formMessages();
            Dialog.listviewMessages.Items.Clear();
            for (int i = 0; i < Interface.MessageCount; i++) {
                string t = "Unknown";
                switch (Interface.Messages[i].Type) {
                    case Interface.MessageType.Information:
                        t = "Information";
                        break;
                    case Interface.MessageType.Warning:
                        t = "Warning";
                        break;
                    case Interface.MessageType.Error:
                        t = "Error";
                        break;
                    case Interface.MessageType.Critical:
                        t = "Critical";
                        break;
                }
                ListViewItem a = Dialog.listviewMessages.Items.Add(t);
                a.SubItems.Add(Interface.Messages[i].Text);
            }
            Dialog.listviewMessages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            DialogResult Result = Dialog.ShowDialog();
            Dialog.Dispose();
            return Result;
        }

        // shown
        private void formMessages_Shown(object sender, EventArgs e) {
            buttonClose.Focus();
        }

        // ignore
        private void buttonIgnore_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Ignore;
        }

        // cancel
        private void buttonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}