namespace Egs
{
    partial class DebugForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.monitorMouseCheckBox = new System.Windows.Forms.CheckBox();
            this.copyToClipboardButton = new System.Windows.Forms.Button();
            this.loggingCheckBox = new System.Windows.Forms.CheckBox();
            this.limitTo100CheckBox = new System.Windows.Forms.CheckBox();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.logListBox = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.monitorMouseCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.copyToClipboardButton);
            this.splitContainer1.Panel2.Controls.Add(this.loggingCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.limitTo100CheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.clearLogButton);
            this.splitContainer1.Size = new System.Drawing.Size(1417, 626);
            this.splitContainer1.SplitterDistance = 581;
            this.splitContainer1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.logListBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1417, 581);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "HID Packet Log";
            // 
            // monitorMouseCheckBox
            // 
            this.monitorMouseCheckBox.AutoSize = true;
            this.monitorMouseCheckBox.Location = new System.Drawing.Point(276, 8);
            this.monitorMouseCheckBox.Name = "monitorMouseCheckBox";
            this.monitorMouseCheckBox.Size = new System.Drawing.Size(190, 16);
            this.monitorMouseCheckBox.TabIndex = 13;
            this.monitorMouseCheckBox.Text = "Enumerate Touch By RawMouse";
            this.monitorMouseCheckBox.UseVisualStyleBackColor = true;
            // 
            // copyToClipboardButton
            // 
            this.copyToClipboardButton.Location = new System.Drawing.Point(472, 4);
            this.copyToClipboardButton.Name = "copyToClipboardButton";
            this.copyToClipboardButton.Size = new System.Drawing.Size(40, 23);
            this.copyToClipboardButton.TabIndex = 12;
            this.copyToClipboardButton.Text = "Copy";
            this.copyToClipboardButton.UseVisualStyleBackColor = true;
            // 
            // loggingCheckBox
            // 
            this.loggingCheckBox.AutoSize = true;
            this.loggingCheckBox.Location = new System.Drawing.Point(7, 8);
            this.loggingCheckBox.Name = "loggingCheckBox";
            this.loggingCheckBox.Size = new System.Drawing.Size(63, 16);
            this.loggingCheckBox.TabIndex = 11;
            this.loggingCheckBox.Text = "Logging";
            this.loggingCheckBox.UseVisualStyleBackColor = true;
            // 
            // limitTo100CheckBox
            // 
            this.limitTo100CheckBox.AutoSize = true;
            this.limitTo100CheckBox.Checked = true;
            this.limitTo100CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.limitTo100CheckBox.Location = new System.Drawing.Point(157, 8);
            this.limitTo100CheckBox.Name = "limitTo100CheckBox";
            this.limitTo100CheckBox.Size = new System.Drawing.Size(113, 16);
            this.limitTo100CheckBox.TabIndex = 10;
            this.limitTo100CheckBox.Text = "limit to 100 Lines";
            this.limitTo100CheckBox.UseVisualStyleBackColor = true;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Location = new System.Drawing.Point(76, 4);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(75, 23);
            this.clearLogButton.TabIndex = 9;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1155, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 " +
    "27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53" +
    " 54 55 56 57 58 59 60 61 62 63";
            // 
            // logListBox
            // 
            this.logListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListBox.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F);
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 11;
            this.logListBox.Location = new System.Drawing.Point(3, 15);
            this.logListBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(1411, 563);
            this.logListBox.TabIndex = 1;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1417, 626);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DebugForm";
            this.Text = "Setting";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox monitorMouseCheckBox;
        private System.Windows.Forms.Button copyToClipboardButton;
        private System.Windows.Forms.CheckBox loggingCheckBox;
        private System.Windows.Forms.CheckBox limitTo100CheckBox;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox logListBox;
    }
}

