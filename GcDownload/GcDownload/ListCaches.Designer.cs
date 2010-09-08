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
            this.columnHeaderId = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDeleteEntry = new System.Windows.Forms.Button();
            this.columnHeaderAuthor = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderShortDescription = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewCaches
            // 
            this.listViewCaches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCaches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderName,
            this.columnHeaderAuthor,
            this.columnHeaderShortDescription});
            this.listViewCaches.FullRowSelect = true;
            this.listViewCaches.GridLines = true;
            this.listViewCaches.Location = new System.Drawing.Point(13, 13);
            this.listViewCaches.MultiSelect = false;
            this.listViewCaches.Name = "listViewCaches";
            this.listViewCaches.Size = new System.Drawing.Size(641, 500);
            this.listViewCaches.TabIndex = 1;
            this.listViewCaches.UseCompatibleStateImageBehavior = false;
            this.listViewCaches.View = System.Windows.Forms.View.Details;
            this.listViewCaches.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCaches_MouseDoubleClick);
            this.listViewCaches.SelectedIndexChanged += new System.EventHandler(this.listViewCaches_SelectedIndexChanged);
            // 
            // columnHeaderId
            // 
            this.columnHeaderId.Text = "Cache ID";
            this.columnHeaderId.Width = 101;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 107;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(554, 519);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 25);
            this.buttonClose.TabIndex = 2;
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
            this.buttonSearch.TabIndex = 8;
            this.buttonSearch.Text = "&Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDeleteEntry
            // 
            this.buttonDeleteEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteEntry.Image = global::GcDownload.Properties.Resources.DeleteHS;
            this.buttonDeleteEntry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDeleteEntry.Location = new System.Drawing.Point(13, 519);
            this.buttonDeleteEntry.Name = "buttonDeleteEntry";
            this.buttonDeleteEntry.Size = new System.Drawing.Size(100, 25);
            this.buttonDeleteEntry.TabIndex = 10;
            this.buttonDeleteEntry.Text = "&Delete";
            this.buttonDeleteEntry.UseVisualStyleBackColor = true;
            this.buttonDeleteEntry.Click += new System.EventHandler(this.buttonDeleteEntry_Click);
            // 
            // columnHeaderAuthor
            // 
            this.columnHeaderAuthor.Text = "Author";
            this.columnHeaderAuthor.Width = 103;
            // 
            // columnHeaderShortDescription
            // 
            this.columnHeaderShortDescription.Text = "Short Description";
            this.columnHeaderShortDescription.Width = 164;
            // 
            // ListCachesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 556);
            this.Controls.Add(this.buttonDeleteEntry);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewCaches);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ListCachesForm";
            this.Text = "Geocaches on Device";
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
    }
}