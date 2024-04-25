using appData2;
using Data_Access;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



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
            for (int i = 0; i < 9; i++)
            {
                if (i == 3) { continue; }
                clb_options.SetItemChecked(i, true);
            }
            for (int i = 9; i < 12; i++)
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
            try
            {
                System.IO.File.Delete(file_write.SettingsPath);
                System.IO.File.Copy(file_write.SettingsPath2, file_write.SettingsPath);
                openFileDialog.InitialDirectory = file_read.readlocaiton();
            }
            catch (Exception)
            {

                openFileDialog.InitialDirectory = "c:\\";
            }

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
            int x = 0;
            // to clean the destination foloder
            var folder = file_write.SeparatePath;
            file_write.startUp(new DirectoryInfo(folder));
            String database_head = database.print_database_header();
            file_write.sqlBuddy2.Write(database_head);
            int count = 0;
            List<Boolean> these_settings = new List<Boolean>();
            List<foreignKey> foreignKeys = data_tables.all_foreignKey;
            bool batchcreated = false;
            string analysis = data_tables.analyzeRelationships();
            file_write.fileWrite(analysis, "analysis", "analysis","analysis");
            foreach (iTable t in data_tables.all_tables)
            {
                t.name = t.name.Replace("[dbo].[", "");
                t.name = t.name.Replace("]", "");
                these_settings = settings.all_options[count];
                string s = "";
                List<String> st = new List<String>();
                // if selected, add rows to table
                if (these_settings[0])
                {
                    s = t.gen_columns();
                    file_write.fileWrite(s,t.name,"sql","table");
                    file_write.sqlBuddy2.Write(s);
                }
                // if selected, add primary keys to table
                if (these_settings[1])
                {
                    s = t.gen_primary_keys();
                    file_write.fileWrite(s, t.name, "sql", "table");
                    file_write.sqlBuddy2.Write(s);
                }

                // if selected, create the alternate keys
                if (true)
                {
                    s = t.gen_alternate_keys();
                    file_write.fileWrite(s, t.name, "sql", "table");
                    file_write.sqlBuddy2.Write(s);
                }
                // if selected, add foreign keys to table
                if (these_settings[2])
                {
                    s = t.gen_foreign_keys();
                    file_write.fileWrite(s, t.name, "sql", "table");
                    file_write.sqlBuddy2.Write(s);
                }

                //gen table footer
                if (true) {
                    s = t.gen_table_footer();
                    file_write.fileWrite(s, t.name, "sql", "table");
                    file_write.sqlBuddy2.Write(s);
                }


                // if selected, create a matching audit table
                if (these_settings[3])
                {
                    s = t.gen_audit_table();
                    file_write.fileWrite(s, t.name, "sql", "audit_table");
                    file_write.sqlBuddy2.Write(s);
                }




                // if selected, add an SP_update to table
                if (these_settings[4])
                {
                    s = t.gen_update();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                }
                // if selected, add an SP_delete and su_undelete to table
                if (these_settings[5])
                {
                    s = t.gen_delete();

                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                    s = t.gen_undelete();

                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);


                }
                // if selected, add an SP_retreive that requires a PK
                if (these_settings[6])
                {
                    s = t.gen_retreive_by_key();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                }
                //if selected an an SP_retreive that shows all data in table
                if (these_settings[7])
                {
                    s = t.gen_retreive_by_all();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                }
                //if selected create an sp_retrive that gets all active data in the table
                if (these_settings[7])
                {
                    s = t.gen_retreive_by_active();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                }
                //select distinct for drop downs
                if (true) {
                    
                        s = t.gen_select_distinct_for_dropdown();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);


                }
                // if selected, add SP_insert to add records to table
                if (these_settings[8])
                {
                    s = t.gen_insert();
                    file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    file_write.sqlBuddy2.Write(s);
                }
                // if selected, add a trigger for inserts
                if (these_settings[9])
                {
                    s = t.gen_insert_trigger();
                    file_write.fileWrite(s, t.name, "sql", "Triggers");
                }
                // if selected, add a trigger for updates
                if (these_settings[10])
                {
                    s = t.gen_update_trigger();
                    file_write.fileWrite(s, t.name, "sql", "Triggers");
                }
                // if selected, add a triger for delets
                if (these_settings[11])
                {
                    s = t.gen_delete_trigger();
                    file_write.fileWrite(s, t.name, "sql", "Triggers");
                }

                //if selected, add a space for adding sample data
                if (true)
                {
                    s = t.gen_sample_space();
                    file_write.fileWrite(s,tbx_databasename.Text, "sql", "Sample");

                }
            }
            foreach (table t in data_tables.all_tables)
            {
                String s = "";
                if (true)
                {
                    s = t.gen_IThingAccessor();
                    file_write.fileWrite(s, t.name, "CSharp", "IAccessor");
                }
                //needs to be these settings 13
                if (true)
                {
                    s = t.gen_ThingAccessor();
                    file_write.fileWrite(s, t.name, "CSharp", "Accessor");
                }
                //needs to be these settings[14]
                if (true)
                {
                    s = t.gen_IThingManager();
                    file_write.fileWrite(s, t.name, "CSharp", "IManager");
                }
                if (true)
                 {
                    s = t.gen_ThingMananger();
                    file_write.fileWrite(s, t.name, "CSharp", "Manager");
                }
                if (true)
                {
                    s = t.gen_DataObject();
                    file_write.fileWrite(s, t.name, "CSharp", "DataObject");
                }
                if (true) //change this to these setings 17
                {
                    s = t.genXAMLWindow();
                    file_write.fileWrite(s, t.name, "CSharp", "XAMLWindow");
                }
                if (true) //change this to these setings 18
                {
                    s = t.genWindowCSharp();
                    file_write.fileWrite(s, t.name, "CSharp", "CSharpWindowCode");
                }
                if (true) //change this to these setings 19
                {
                    s = t.genJavaModel();
                    file_write.fileWrite(s, t.name, "JavaModel", "Model");
                }

                if (true) //change this to these setings 20
                {
                    s = t.genJavaDAO();
                    file_write.fileWrite(s, t.name, "JavaModelDAO", "ModelDAO");
                }
                // if (true) //change this to these setings 21 Creates a sql file iwth the table name
                // {
                //   s = t.name.bracketStrip().Replace("?","");
                //  System.IO.File.Create(file_write.FilesPath + s + ".sql");

                //}
                //if (true) //change this to these setings 22, creates a bat file to run all of the sql iles you just created
                // {
                //   s = t.getBatch();
                //  file_write.BatchBuddy.Write(s);
                // batchcreated = true;
                //}
                if (true) //change this to these setings 23
                {
                    s = t.genCreateJSP();
                    file_write.fileWrite(s, t.name, "JavaJSP", "CreateJSP");
                }
                if (true) //change this to these setings 24
                {
                    s = t.genCreateServelet();
                    file_write.fileWrite(s, t.name, "JavaServlet", "CreateServlet");
                }
                if (true) //change this to these setings 25
                {
                    s = t.genviewAllJSP();
                    file_write.fileWrite(s, t.name, "JavaJSP", "ViewAllJSP");
                }
                if (true) //change this to these setings 26
                {
                    s = t.genviewAllServlet();
                    file_write.fileWrite(s, t.name, "JavaServlet", "ViewAllServlet");
                }

                if (true) //change this to these setings 27
                {
                    s = t.genDeleteServlet();
                    file_write.fileWrite(s, t.name, "JavaServlet", "DeleteServlet");
                }

                if ( x==0) //change this to these setings 28 && x==0
                {
                    x++;
                    s = t.genIndexJSP();
                    file_write.fileWrite(s, "index", "JavaJSP", "Index");
                }

                if (true)
                {  //change this to these settings 29
                s=t.genViewEditJSP();
                    file_write.fileWrite(s, t.name, "JavaJSP", "ViewEditJSP");
                }

                if (true)
                {    //change to these settings 30
                    s = t.genViewEditServlet();
                        file_write.fileWrite(s, t.name, "JavaServlet", "ViewEditServlet");

                }
                



                count++;
            }

            file_write.sqlBuddy2.Flush();
            file_write.CSharpBuddy.Flush();
            file_write.XAMLBuddy.Flush();
            file_write.BatchBuddy.Flush();
            file_write.JSPBuddy.Flush();
            file_write.ServletBuddy.Flush();
            MessageBox.Show("generation complete");
            this.Close();


        }

        private void initialize_settings()
        {
            for (int i = 0; i < settings.table_count; i++)
            {
                for (int j = 0; j < 18; j++)
                {
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
            for (int i = 0; i < settings.table_count; i++)
            {

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
            settings.TSQLMode = cbxTSql.Checked;
        }

        private void btn_cSharp_Click(object sender, EventArgs e)
        {
            // csharpsettings();

        }


    }
}
