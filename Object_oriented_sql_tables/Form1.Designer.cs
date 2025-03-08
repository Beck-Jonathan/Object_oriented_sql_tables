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
            this.btn_selectfile = new System.Windows.Forms.Button();
            this.btn_generateTable = new System.Windows.Forms.Button();
            this.tbx_databasename = new System.Windows.Forms.TextBox();
            this.btn_read_table = new System.Windows.Forms.Button();
            this.cbx_table_names = new System.Windows.Forms.ComboBox();
            this.clb_options = new System.Windows.Forms.CheckedListBox();
            this.cbxTSql = new System.Windows.Forms.CheckBox();
            this.btn_cSharp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_db_name = new System.Windows.Forms.Label();
            this.NUD_page_size = new System.Windows.Forms.NumericUpDown();
            this.lbl_owner = new System.Windows.Forms.Label();
            this.tbx_owner_name = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_page_size)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_selectfile
            // 
            this.btn_selectfile.Location = new System.Drawing.Point(67, 23);
            this.btn_selectfile.Margin = new System.Windows.Forms.Padding(2);
            this.btn_selectfile.Name = "btn_selectfile";
            this.btn_selectfile.Size = new System.Drawing.Size(56, 19);
            this.btn_selectfile.TabIndex = 1;
            this.btn_selectfile.Text = "Select File";
            this.btn_selectfile.UseVisualStyleBackColor = true;
            this.btn_selectfile.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_generateTable
            // 
            this.btn_generateTable.Location = new System.Drawing.Point(361, 23);
            this.btn_generateTable.Margin = new System.Windows.Forms.Padding(2);
            this.btn_generateTable.Name = "btn_generateTable";
            this.btn_generateTable.Size = new System.Drawing.Size(179, 19);
            this.btn_generateTable.TabIndex = 2;
            this.btn_generateTable.Text = "Generate Table";
            this.btn_generateTable.UseVisualStyleBackColor = true;
            this.btn_generateTable.Click += new System.EventHandler(this.btn_generateTable_Click);
            // 
            // tbx_databasename
            // 
            this.tbx_databasename.Location = new System.Drawing.Point(91, 55);
            this.tbx_databasename.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_databasename.Name = "tbx_databasename";
            this.tbx_databasename.Size = new System.Drawing.Size(164, 20);
            this.tbx_databasename.TabIndex = 3;
            this.tbx_databasename.TextChanged += new System.EventHandler(this.tbx_databasename_TextChanged);
            // 
            // btn_read_table
            // 
            this.btn_read_table.Location = new System.Drawing.Point(280, 309);
            this.btn_read_table.Margin = new System.Windows.Forms.Padding(2);
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
            this.cbx_table_names.Location = new System.Drawing.Point(181, 196);
            this.cbx_table_names.Margin = new System.Windows.Forms.Padding(2);
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
            "Trigger_Delete",
            "IAccessor",
            "RealAccessor",
            "IManager",
            "RealManager",
            "DataObject",
            "XAMLWindow"});
            this.clb_options.Location = new System.Drawing.Point(363, 79);
            this.clb_options.Margin = new System.Windows.Forms.Padding(2);
            this.clb_options.Name = "clb_options";
            this.clb_options.Size = new System.Drawing.Size(187, 274);
            this.clb_options.TabIndex = 6;
            this.clb_options.SelectedIndexChanged += new System.EventHandler(this.clb_options_SelectedIndexChanged);
            // 
            // cbxTSql
            // 
            this.cbxTSql.AutoSize = true;
            this.cbxTSql.Location = new System.Drawing.Point(91, 278);
            this.cbxTSql.Name = "cbxTSql";
            this.cbxTSql.Size = new System.Drawing.Size(153, 17);
            this.cbxTSql.TabIndex = 7;
            this.cbxTSql.Text = "TSQLMode? (Experimental";
            this.cbxTSql.UseVisualStyleBackColor = true;
            this.cbxTSql.CheckedChanged += new System.EventHandler(this.cbxTSql_CheckedChanged);
            // 
            // btn_cSharp
            // 
            this.btn_cSharp.Location = new System.Drawing.Point(168, 168);
            this.btn_cSharp.Name = "btn_cSharp";
            this.btn_cSharp.Size = new System.Drawing.Size(116, 23);
            this.btn_cSharp.TabIndex = 8;
            this.btn_cSharp.Text = "cSharpOutput";
            this.btn_cSharp.UseVisualStyleBackColor = true;
            this.btn_cSharp.Click += new System.EventHandler(this.btn_cSharp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Page Size";
            // 
            // lbl_db_name
            // 
            this.lbl_db_name.AutoSize = true;
            this.lbl_db_name.Location = new System.Drawing.Point(9, 58);
            this.lbl_db_name.Name = "lbl_db_name";
            this.lbl_db_name.Size = new System.Drawing.Size(53, 13);
            this.lbl_db_name.TabIndex = 11;
            this.lbl_db_name.Text = "DB Name";
            // 
            // NUD_page_size
            // 
            this.NUD_page_size.AllowDrop = true;
            this.NUD_page_size.Location = new System.Drawing.Point(37, 239);
            this.NUD_page_size.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUD_page_size.Name = "NUD_page_size";
            this.NUD_page_size.Size = new System.Drawing.Size(120, 20);
            this.NUD_page_size.TabIndex = 12;
            this.NUD_page_size.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.NUD_page_size.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // lbl_owner
            // 
            this.lbl_owner.AutoSize = true;
            this.lbl_owner.Location = new System.Drawing.Point(12, 89);
            this.lbl_owner.Name = "lbl_owner";
            this.lbl_owner.Size = new System.Drawing.Size(38, 13);
            this.lbl_owner.TabIndex = 13;
            this.lbl_owner.Text = "Owner";
            // 
            // tbx_owner_name
            // 
            this.tbx_owner_name.Location = new System.Drawing.Point(78, 89);
            this.tbx_owner_name.Name = "tbx_owner_name";
            this.tbx_owner_name.Size = new System.Drawing.Size(177, 20);
            this.tbx_owner_name.TabIndex = 14;
            this.tbx_owner_name.TextChanged += new System.EventHandler(this.tbx_owner_name_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.tbx_owner_name);
            this.Controls.Add(this.lbl_owner);
            this.Controls.Add(this.NUD_page_size);
            this.Controls.Add(this.lbl_db_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_cSharp);
            this.Controls.Add(this.cbxTSql);
            this.Controls.Add(this.clb_options);
            this.Controls.Add(this.cbx_table_names);
            this.Controls.Add(this.btn_read_table);
            this.Controls.Add(this.tbx_databasename);
            this.Controls.Add(this.btn_generateTable);
            this.Controls.Add(this.btn_selectfile);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_page_size)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Button btn_selectfile;
        private System.Windows.Forms.Button btn_generateTable;
        private System.Windows.Forms.TextBox tbx_databasename;
        private System.Windows.Forms.Button btn_read_table;
        private System.Windows.Forms.ComboBox cbx_table_names;
        private System.Windows.Forms.CheckedListBox clb_options;
        private System.Windows.Forms.CheckBox cbxTSql;
        private System.Windows.Forms.Button btn_cSharp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_db_name;
        private System.Windows.Forms.NumericUpDown NUD_page_size;
        private System.Windows.Forms.Label lbl_owner;
        private System.Windows.Forms.TextBox tbx_owner_name;
    }
}
