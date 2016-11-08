namespace Egs.Views
{
    public partial class AppTrayIconAndMenuItemsComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                notifyIconInTray.Visible = false;
                components.Dispose();
                if (deviceIsConnectedIcon != null) { deviceIsConnectedIcon.Dispose(); deviceIsConnectedIcon = null; }
                if (deviceIsNotConnectedIcon != null) { deviceIsNotConnectedIcon.Dispose(); deviceIsNotConnectedIcon = null; }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIconInTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripFromNotifyIconInTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            // 
            // notifyIconInTray
            // 
            this.notifyIconInTray.ContextMenuStrip = this.contextMenuStripFromNotifyIconInTray;
            // 
            // contextMenuStripFromNotifyIconInTray
            // 
            this.contextMenuStripFromNotifyIconInTray.Name = "contextMenuStripFromNotifyIconInTray";
            this.contextMenuStripFromNotifyIconInTray.Size = new System.Drawing.Size(203, 98);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconInTray;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFromNotifyIconInTray;
    }
}