namespace GcDownload
{
    partial class SettingsForm
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
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxGarminDrive = new System.Windows.Forms.ComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxArchivPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseArchivDirectory = new System.Windows.Forms.Button();
            this.buttonDetect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxGarminDrive
            // 
            resources.ApplyResources(this.comboBoxGarminDrive, "comboBoxGarminDrive");
            this.comboBoxGarminDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGarminDrive.FormattingEnabled = true;
            this.comboBoxGarminDrive.Name = "comboBoxGarminDrive";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxArchivPath
            // 
            resources.ApplyResources(this.textBoxArchivPath, "textBoxArchivPath");
            this.textBoxArchivPath.Name = "textBoxArchivPath";
            this.textBoxArchivPath.ReadOnly = true;
            // 
            // buttonBrowseArchivDirectory
            // 
            resources.ApplyResources(this.buttonBrowseArchivDirectory, "buttonBrowseArchivDirectory");
            this.buttonBrowseArchivDirectory.Name = "buttonBrowseArchivDirectory";
            this.buttonBrowseArchivDirectory.UseVisualStyleBackColor = true;
            this.buttonBrowseArchivDirectory.Click += new System.EventHandler(this.buttonBrowseArchivDirectory_Click);
            // 
            // buttonDetect
            // 
            resources.ApplyResources(this.buttonDetect, "buttonDetect");
            this.buttonDetect.Name = "buttonDetect";
            this.buttonDetect.UseVisualStyleBackColor = true;
            this.buttonDetect.Click += new System.EventHandler(this.buttonDetect_Click);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonDetect);
            this.Controls.Add(this.buttonBrowseArchivDirectory);
            this.Controls.Add(this.textBoxArchivPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.comboBoxGarminDrive);
            this.Controls.Add(this.label1);
            this.Name = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxGarminDrive;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxArchivPath;
        private System.Windows.Forms.Button buttonBrowseArchivDirectory;
        private System.Windows.Forms.Button buttonDetect;
    }
}