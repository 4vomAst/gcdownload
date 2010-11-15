namespace GcDownload
{
    partial class ListCachesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListCachesForm));
            this.listViewCaches = new System.Windows.Forms.ListView();
            this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderShortDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDeleteEntry = new System.Windows.Forms.Button();
            this.buttonUpdateAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewCaches
            // 
            resources.ApplyResources(this.listViewCaches, "listViewCaches");
            this.listViewCaches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderName,
            this.columnHeaderAuthor,
            this.columnHeaderShortDescription});
            this.listViewCaches.FullRowSelect = true;
            this.listViewCaches.GridLines = true;
            this.listViewCaches.MultiSelect = false;
            this.listViewCaches.Name = "listViewCaches";
            this.listViewCaches.UseCompatibleStateImageBehavior = false;
            this.listViewCaches.View = System.Windows.Forms.View.Details;
            this.listViewCaches.SelectedIndexChanged += new System.EventHandler(this.listViewCaches_SelectedIndexChanged);
            this.listViewCaches.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCaches_MouseDoubleClick);
            // 
            // columnHeaderId
            // 
            resources.ApplyResources(this.columnHeaderId, "columnHeaderId");
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderAuthor
            // 
            resources.ApplyResources(this.columnHeaderAuthor, "columnHeaderAuthor");
            // 
            // columnHeaderShortDescription
            // 
            resources.ApplyResources(this.columnHeaderShortDescription, "columnHeaderShortDescription");
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Image = global::GcDownload.Properties.Resources.SearchWebHS;
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDeleteEntry
            // 
            resources.ApplyResources(this.buttonDeleteEntry, "buttonDeleteEntry");
            this.buttonDeleteEntry.Image = global::GcDownload.Properties.Resources.DeleteHS;
            this.buttonDeleteEntry.Name = "buttonDeleteEntry";
            this.buttonDeleteEntry.UseVisualStyleBackColor = true;
            this.buttonDeleteEntry.Click += new System.EventHandler(this.buttonDeleteEntry_Click);
            // 
            // buttonUpdateAll
            // 
            this.buttonUpdateAll.Image = global::GcDownload.Properties.Resources.RefreshDocViewHS;
            resources.ApplyResources(this.buttonUpdateAll, "buttonUpdateAll");
            this.buttonUpdateAll.Name = "buttonUpdateAll";
            this.buttonUpdateAll.UseVisualStyleBackColor = true;
            this.buttonUpdateAll.Click += new System.EventHandler(this.buttonUpdateAll_Click);
            // 
            // ListCachesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonUpdateAll);
            this.Controls.Add(this.buttonDeleteEntry);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewCaches);
            this.Name = "ListCachesForm";
            this.Load += new System.EventHandler(this.ListCaches_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewCaches;
        private System.Windows.Forms.ColumnHeader columnHeaderId;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonDeleteEntry;
        private System.Windows.Forms.ColumnHeader columnHeaderAuthor;
        private System.Windows.Forms.ColumnHeader columnHeaderShortDescription;
        private System.Windows.Forms.Button buttonUpdateAll;
    }
}