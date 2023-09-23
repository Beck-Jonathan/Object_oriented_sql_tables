
namespace Object_oriented_sql_tables
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.btn_selectfile = new System.Windows.Forms.Button();
            this.btn_generateTable = new System.Windows.Forms.Button();
            this.tbx_databasename = new System.Windows.Forms.TextBox();
            this.btn_read_table = new System.Windows.Forms.Button();
            this.cbx_table_names = new System.Windows.Forms.ComboBox();
            this.clb_options = new System.Windows.Forms.CheckedListBox();
            this.cbxTSql = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // btn_selectfile
            // 
            this.btn_selectfile.Location = new System.Drawing.Point(67, 83);
            this.btn_selectfile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_selectfile.Name = "btn_selectfile";
            this.btn_selectfile.Size = new System.Drawing.Size(56, 19);
            this.btn_selectfile.TabIndex = 1;
            this.btn_selectfile.Text = "Select File";
            this.btn_selectfile.UseVisualStyleBackColor = true;
            this.btn_selectfile.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_generateTable
            // 
            this.btn_generateTable.Location = new System.Drawing.Point(370, 74);
            this.btn_generateTable.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_generateTable.Name = "btn_generateTable";
            this.btn_generateTable.Size = new System.Drawing.Size(179, 19);
            this.btn_generateTable.TabIndex = 2;
            this.btn_generateTable.Text = "Generate Table";
            this.btn_generateTable.UseVisualStyleBackColor = true;
            this.btn_generateTable.Click += new System.EventHandler(this.btn_generateTable_Click);
            // 
            // tbx_databasename
            // 
            this.tbx_databasename.Location = new System.Drawing.Point(67, 120);
            this.tbx_databasename.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbx_databasename.Name = "tbx_databasename";
            this.tbx_databasename.Size = new System.Drawing.Size(76, 20);
            this.tbx_databasename.TabIndex = 3;
            this.tbx_databasename.TextChanged += new System.EventHandler(this.tbx_databasename_TextChanged);
            // 
            // btn_read_table
            // 
            this.btn_read_table.Location = new System.Drawing.Point(168, 83);
            this.btn_read_table.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_read_table.Name = "btn_read_table";
            this.btn_read_table.Size = new System.Drawing.Size(56, 19);
            this.btn_read_table.TabIndex = 4;
            this.btn_read_table.Text = "Read tables";
            this.btn_read_table.UseVisualStyleBackColor = true;
            this.btn_read_table.Click += new System.EventHandler(this.btn_read_table_Click);
            // 
            // cbx_table_names
            // 
            this.cbx_table_names.AllowDrop = true;
            this.cbx_table_names.Enabled = false;
            this.cbx_table_names.FormattingEnabled = true;
            this.cbx_table_names.Location = new System.Drawing.Point(156, 171);
            this.cbx_table_names.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbx_table_names.Name = "cbx_table_names";
            this.cbx_table_names.Size = new System.Drawing.Size(92, 21);
            this.cbx_table_names.TabIndex = 5;
            this.cbx_table_names.SelectedIndexChanged += new System.EventHandler(this.cbx_table_names_SelectedIndexChanged);
            // 
            // clb_options
            // 
            this.clb_options.Enabled = false;
            this.clb_options.FormattingEnabled = true;
            this.clb_options.Items.AddRange(new object[] {
            "Table",
            "Primary Keys",
            "Foreign Keys",
            "Audit Table",
            "SP_Update",
            "SP_Delete",
            "SP_Retreive_By_Key",
            "SP_Retreive_by_All",
            "SP_insert",
            "Trigger_Update",
            "Trigger_Insert",
            "Trigger_Delete"});
            this.clb_options.Location = new System.Drawing.Point(361, 136);
            this.clb_options.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.clb_options.Name = "clb_options";
            this.clb_options.Size = new System.Drawing.Size(177, 184);
            this.clb_options.TabIndex = 6;
            this.clb_options.SelectedIndexChanged += new System.EventHandler(this.clb_options_SelectedIndexChanged);
            // 
            // cbxTSql
            // 
            this.cbxTSql.AutoSize = true;
            this.cbxTSql.Location = new System.Drawing.Point(91, 246);
            this.cbxTSql.Name = "cbxTSql";
            this.cbxTSql.Size = new System.Drawing.Size(153, 17);
            this.cbxTSql.TabIndex = 7;
            this.cbxTSql.Text = "TSQLMode? (Experimental";
            this.cbxTSql.UseVisualStyleBackColor = true;
            this.cbxTSql.CheckedChanged += new System.EventHandler(this.cbxTSql_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.cbxTSql);
            this.Controls.Add(this.clb_options);
            this.Controls.Add(this.cbx_table_names);
            this.Controls.Add(this.btn_read_table);
            this.Controls.Add(this.tbx_databasename);
            this.Controls.Add(this.btn_generateTable);
            this.Controls.Add(this.btn_selectfile);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_selectfile;
        private System.Windows.Forms.Button btn_generateTable;
        private System.Windows.Forms.TextBox tbx_databasename;
        private System.Windows.Forms.Button btn_read_table;
        private System.Windows.Forms.ComboBox cbx_table_names;
        private System.Windows.Forms.CheckedListBox clb_options;
        private System.Windows.Forms.CheckBox cbxTSql;
    }
}

