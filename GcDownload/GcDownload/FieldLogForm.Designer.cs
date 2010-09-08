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
            this.columnHeaderId = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTimestamp = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNote = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDeleteLog = new System.Windows.Forms.Button();
            this.buttonDeleteEntry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewFieldLog
            // 
            this.listViewFieldLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFieldLog.CheckBoxes = true;
            this.listViewFieldLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderTimestamp,
            this.columnHeaderType,
            this.columnHeaderNote});
            this.listViewFieldLog.FullRowSelect = true;
            this.listViewFieldLog.GridLines = true;
            this.listViewFieldLog.Location = new System.Drawing.Point(13, 13);
            this.listViewFieldLog.MultiSelect = false;
            this.listViewFieldLog.Name = "listViewFieldLog";
            this.listViewFieldLog.Size = new System.Drawing.Size(641, 500);
            this.listViewFieldLog.TabIndex = 0;
            this.listViewFieldLog.UseCompatibleStateImageBehavior = false;
            this.listViewFieldLog.View = System.Windows.Forms.View.Details;
            this.listViewFieldLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewFieldLog_MouseDoubleClick);
            this.listViewFieldLog.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewFieldLog_ItemChecked);
            this.listViewFieldLog.SelectedIndexChanged += new System.EventHandler(this.listViewFieldLog_SelectedIndexChanged);
            // 
            // columnHeaderId
            // 
            this.columnHeaderId.Text = "Cache ID";
            this.columnHeaderId.Width = 101;
            // 
            // columnHeaderTimestamp
            // 
            this.columnHeaderTimestamp.Text = "Time";
            this.columnHeaderTimestamp.Width = 102;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 91;
            // 
            // columnHeaderNote
            // 
            this.columnHeaderNote.Text = "Note";
            this.columnHeaderNote.Width = 346;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(554, 519);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 25);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Image = global::GcDownload.Properties.Resources.SearchWebHS;
            this.buttonSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSearch.Location = new System.Drawing.Point(448, 519);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(100, 25);
            this.buttonSearch.TabIndex = 7;
            this.buttonSearch.Text = "&Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDeleteLog
            // 
            this.buttonDeleteLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteLog.Image = global::GcDownload.Properties.Resources.DeleteFolderHS;
            this.buttonDeleteLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDeleteLog.Location = new System.Drawing.Point(13, 520);
            this.buttonDeleteLog.Name = "buttonDeleteLog";
            this.buttonDeleteLog.Size = new System.Drawing.Size(100, 25);
            this.buttonDeleteLog.TabIndex = 8;
            this.buttonDeleteLog.Text = "Delete &Log";
            this.buttonDeleteLog.UseVisualStyleBackColor = true;
            this.buttonDeleteLog.Click += new System.EventHandler(this.buttonDeleteLog_Click);
            // 
            // buttonDeleteEntry
            // 
            this.buttonDeleteEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteEntry.Image = global::GcDownload.Properties.Resources.DeleteHS;
            this.buttonDeleteEntry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDeleteEntry.Location = new System.Drawing.Point(119, 520);
            this.buttonDeleteEntry.Name = "buttonDeleteEntry";
            this.buttonDeleteEntry.Size = new System.Drawing.Size(125, 25);
            this.buttonDeleteEntry.TabIndex = 9;
            this.buttonDeleteEntry.Text = "&Delete Checked";
            this.buttonDeleteEntry.UseVisualStyleBackColor = true;
            this.buttonDeleteEntry.Click += new System.EventHandler(this.buttonDeleteEntry_Click);
            // 
            // FieldLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 556);
            this.Controls.Add(this.buttonDeleteEntry);
            this.Controls.Add(this.buttonDeleteLog);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewFieldLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FieldLogForm";
            this.Text = "Field Log on Device";
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