using appData2;
using Data_Access;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.Windows.Forms;



namespace Object_oriented_sql_tables
{
    public partial class Form1 : Form
    {
        public static List<char> these_settings = new List<char>();
        public Form1()
        {
            settings.app_path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            settings.app_path = settings.app_path.Substring(0, settings.app_path.Length - 30);
            settings.TSQLMode = false;
            InitializeComponent();
            tbx_databasename.Text = "WFTDA_debug";
            settings.database_name = tbx_databasename.Text;
            initialize_settings();
            for (int i = 0; i < 3; i++)
            {
                clb_options.SetItemChecked(i, true);
            }
            for (int i = 3; i < 12; i++)
            {
                clb_options.SetItemChecked(i, false);
            }
            btn_selectfile.Enabled = true;
            btn_read_table.Enabled = false;
            cbx_table_names.Enabled = false;
            btn_generateTable.Enabled = false;
            clb_options.Enabled = false;
            }

        //open file dialog
        private void button1_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\Users\\jjbec\\Desktop\\classes\\structured_systems\\";
            //openFileDialog.InitialDirectory = settings.app_path;
            openFileDialog.ShowDialog();
            string filepath = openFileDialog.FileName;
            settings.path = filepath;
            btn_selectfile.Enabled = false;
            btn_read_table.Enabled = true;
            cbx_table_names.Enabled = false;
            btn_generateTable.Enabled = false;
            clb_options.Enabled = false;
        }
        //generate table button
        private void btn_generateTable_Click(object sender, System.EventArgs e)
        {

            String database_head = database.print_database_header();
            file_write.WriteBuddy.Write(database_head);
            int count = 0;
            List<Boolean> these_settings = new List<Boolean>();

            foreach (table t in data_tables.all_tables)
            {
                these_settings = settings.all_options[count];
                string s = "";
                List<String> st = new List<String>();
                // if selected, add rows to table
                if (these_settings[0])
                {
                    s = t.gen_columns();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add primary keys to table
                if (these_settings[1])
                {
                    s = t.gen_primary_keys();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add foreign keys to table
                if (these_settings[2])
                {
                    s = t.gen_foreign_keys();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, create a matching audit table
                if (these_settings[3])
                {
                    s = t.gen_audit_table();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add an SP_update to table
                if (these_settings[4])
                {
                    s = t.gen_update();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add an SP_delete and su_undelete to table
                if (these_settings[5])
                {
                    s = t.gen_delete();

                    file_write.WriteBuddy.Write(s);
                    s = t.gen_undelete();

                    file_write.WriteBuddy.Write(s);


                }
                // if selected, add an SP_retreive that requires a PK
                if (these_settings[6])
                {
                    s = t.gen_retreive_by_key();
                    file_write.WriteBuddy.Write(s);
                }
                //if selected an an SP_retreive that shows all data in table
                if (these_settings[7])
                {
                    s = t.gen_retreive_by_all();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add SP_insert to add records to table
                if (these_settings[8])
                {
                    s = t.gen_insert();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add a trigger for inserts
                if (these_settings[9])
                {
                    s = t.gen_insert_trigger();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add a trigger for updates
                if (these_settings[10])
                {
                    s = t.gen_update_trigger();
                    file_write.WriteBuddy.Write(s);
                }
                // if selected, add a triger for delets
                if (these_settings[11])
                {
                    s = t.gen_delete_trigger();
                    file_write.WriteBuddy.Write(s);
                }
                if (these_settings[12]) {
                    s = t.gen_IThingAccessor();
                    file_write.WriteBuddy.Write(s);
                }
                if (these_settings[13])
                {
                    s = t.gen_ThingAccessor();
                    file_write.WriteBuddy.Write(s);
                }
                if (these_settings[14])
                {
                    s = t.gen_IThingManager();
                    file_write.WriteBuddy.Write(s);
                }
                if (these_settings[15])
                {
                    //s=t.realMananger
                    //file_write.CSharpBuddy.Write(s)
                }
                if (these_settings[16])
                {
                    s=t.gen_DataObject();
                    file_write.WriteBuddy.Write(s);
                }
                if (true) //change this to these setings 17
                {
                    s = t.genXAMLWindow();
                    file_write.XAMLBuddy.Write(s);
                }
                if (true) //change this to these setings 18
                {
                    s = t.genWindowCSharp();
                    file_write.XAMLBuddy.Write(s);
                }




                count++;
            }
            file_write.WriteBuddy.Flush();
            file_write.CSharpBuddy.Flush();
            file_write.XAMLBuddy.Flush();
            MessageBox.Show("generation complete");
            this.Close();


        }

        private void initialize_settings()
        {
            for (int i = 0; i < settings.table_count; i++) {
                for (int j = 0; j < 18; j++) {
                    settings.all_options[i][j] = true;
                }
            
            }



        }

        private void tbx_databasename_TextChanged(object sender, EventArgs e)
        {
            settings.database_name = tbx_databasename.Text;
        }

        private void btn_read_table_Click(object sender, EventArgs e)
        {
            file_read.readdata();
            
            settings.table_count = data_tables.all_tables.Count;
            for (int i=0; i < settings.table_count; i++) { 
            
            }
            foreach (table t in data_tables.all_tables)
            {
                settings.table_names.Add(t.name);
            }
            cbx_table_names.DataSource = settings.table_names;
            cbx_table_names.Enabled = true;
            btn_selectfile.Enabled = false;
            btn_read_table.Enabled = false;
            
            btn_generateTable.Enabled = true;
            clb_options.Enabled = true;
        }

        private void cbx_table_names_SelectedIndexChanged(object sender, EventArgs e)
        {
            clb_options.Enabled = true;
            List<Boolean> these_settings = settings.all_options[cbx_table_names.SelectedIndex];
            for (int i = 0; i < these_settings.Count; i++)
            {
                clb_options.SetItemChecked(i, these_settings[i]);
            }

        }

        private void clb_options_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Boolean> these_settings = new List<Boolean>();
            for (int i = 0; i < 18; i++)
            {
                these_settings.Add(clb_options.GetItemChecked(i));
            }
            settings.all_options[cbx_table_names.SelectedIndex] = these_settings;

        }

        private void cbxTSql_CheckedChanged(object sender, EventArgs e)
        {
            settings.TSQLMode= cbxTSql.Checked;
        }

        private void btn_cSharp_Click(object sender, EventArgs e)
        {
           // csharpsettings();

        }

        
    }
}
