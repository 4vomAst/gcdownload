namespace GcDownload
{
    partial class FieldLogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldLogForm));
            this.listViewFieldLog = new System.Windows.Forms.ListView();
            this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNote = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDeleteLog = new System.Windows.Forms.Button();
            this.buttonDeleteEntry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewFieldLog
            // 
            resources.ApplyResources(this.listViewFieldLog, "listViewFieldLog");
            this.listViewFieldLog.CheckBoxes = true;
            this.listViewFieldLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderTimestamp,
            this.columnHeaderType,
            this.columnHeaderNote});
            this.listViewFieldLog.FullRowSelect = true;
            this.listViewFieldLog.GridLines = true;
            this.listViewFieldLog.MultiSelect = false;
            this.listViewFieldLog.Name = "listViewFieldLog";
            this.listViewFieldLog.UseCompatibleStateImageBehavior = false;
            this.listViewFieldLog.View = System.Windows.Forms.View.Details;
            this.listViewFieldLog.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewFieldLog_ItemChecked);
            this.listViewFieldLog.SelectedIndexChanged += new System.EventHandler(this.ListViewFieldLog_SelectedIndexChanged);
            this.listViewFieldLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListViewFieldLog_MouseDoubleClick);
            // 
            // columnHeaderId
            // 
            resources.ApplyResources(this.columnHeaderId, "columnHeaderId");
            // 
            // columnHeaderTimestamp
            // 
            resources.ApplyResources(this.columnHeaderTimestamp, "columnHeaderTimestamp");
            // 
            // columnHeaderType
            // 
            resources.ApplyResources(this.columnHeaderType, "columnHeaderType");
            // 
            // columnHeaderNote
            // 
            resources.ApplyResources(this.columnHeaderNote, "columnHeaderNote");
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Image = global::GcDownload.Properties.Resources.SearchWebHS;
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.ButtonSearch_Click);
            // 
            // buttonDeleteLog
            // 
            resources.ApplyResources(this.buttonDeleteLog, "buttonDeleteLog");
            this.buttonDeleteLog.Image = global::GcDownload.Properties.Resources.DeleteFolderHS;
            this.buttonDeleteLog.Name = "buttonDeleteLog";
            this.buttonDeleteLog.UseVisualStyleBackColor = true;
            this.buttonDeleteLog.Click += new System.EventHandler(this.ButtonDeleteLog_Click);
            // 
            // buttonDeleteEntry
            // 
            resources.ApplyResources(this.buttonDeleteEntry, "buttonDeleteEntry");
            this.buttonDeleteEntry.Image = global::GcDownload.Properties.Resources.DeleteHS;
            this.buttonDeleteEntry.Name = "buttonDeleteEntry";
            this.buttonDeleteEntry.UseVisualStyleBackColor = true;
            this.buttonDeleteEntry.Click += new System.EventHandler(this.ButtonDeleteEntry_Click);
            // 
            // FieldLogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonDeleteEntry);
            this.Controls.Add(this.buttonDeleteLog);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewFieldLog);
            this.Name = "FieldLogForm";
            this.Load += new System.EventHandler(this.FieldLogForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewFieldLog;
        private System.Windows.Forms.ColumnHeader columnHeaderId;
        private System.Windows.Forms.ColumnHeader columnHeaderTimestamp;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderNote;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonDeleteLog;
        private System.Windows.Forms.Button buttonDeleteEntry;
    }
}