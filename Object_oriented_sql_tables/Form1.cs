using appData2;
using Data_Objects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
namespace Object_oriented_sql_tables
{
    public partial class Form1 : Form
    {
        public iFile_read_manager file_read;
        public iFile_write_manager file_write;
        public static int page_size;
        public static List<char> these_settings = new List<char>();
        public Form1()
        {
            file_read = new file_read_manager();
            file_write = new file_write_manager();
            page_size = 5;
            settings.app_path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            settings.app_path = settings.app_path.Substring(0, settings.app_path.Length - 30);
            settings.TSQLMode = false;
            InitializeComponent();
            tbx_databasename.Text = "WFTDA_debug";
            settings.database_name = tbx_databasename.Text;
            tbx_owner_name.Text = "Owner";
            settings.owner_name = tbx_owner_name.Text;
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
                System.IO.File.Delete(file_write_manager.SettingsPath);
                System.IO.File.Copy(file_write_manager.SettingsPath2, file_write_manager.SettingsPath);
                openFileDialog.InitialDirectory = file_read.readlocaiton();
            }
            catch (Exception)
            {
                openFileDialog.InitialDirectory = "c:\\";
            }
            //openFileDialog.InitialDirectory = settings.app_path;
            _ = openFileDialog.ShowDialog();
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
            string alloutput = "";
            int x = 0;
            page_size = (int)NUD_page_size.Value;
            appData2.settings.page_size = page_size;
            // to clean the destination foloder
            var folder = file_write_manager.SeparatePath;
            try
            {
                file_write.startUp(new DirectoryInfo(folder));
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
            _ = database.print_database_header();
            //file_write.sqlBuddy2.Write(database_head);
            int count = 0;
            _ = new List<Boolean>();
            string analysis = data_tables.analyzeRelationships();
            file_write.fileWrite(analysis, "analysis", "analysis", "analysis");
            string headerJSP = data_tables.genHeaderJSP();
            file_write.fileWrite(headerJSP, settings.database_name, "JavaJSP", "headerJSP");
            string footerJSP = data_tables.genFooterJSP();
            file_write.fileWrite(footerJSP, settings.database_name, "JavaJSP", "footerJSP");



            foreach (iTable t in data_tables.all_tables)

            {
                t.name = t.name.Replace("[dbo].[", "");
                t.name = t.name.Replace("]", "");
                List<bool> these_settings = settings.all_options[count];
                _ = new List<String>();
                string s;
                // if selected, add rows to table
                if (these_settings[0])
                {
                    s = t.gen_columns();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add primary keys to table
                if (these_settings[1])
                {
                    s = t.gen_primary_keys();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, create the alternate keys
                if (true)
                {
                    s = t.gen_alternate_keys();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add foreign keys to table
                if (these_settings[2])
                {
                    s = t.gen_foreign_keys();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                //gen table footer
                if (true)
                {
                    s = t.gen_table_footer();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, create a matching audit table
                if (these_settings[3])
                {
                    s = t.gen_audit_table();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "audit_table");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add an SP_update to table
                if (these_settings[4])
                {
                    s = t.gen_update();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add an SP_delete and su_undelete to table
                if (these_settings[5])
                {
                    s = t.gen_delete();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                    s = t.gen_undelete();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add an SP_retrieve that requires a PK
                if (these_settings[6])
                {
                    s = t.gen_retrieve_by_key();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                //if selected an an SP_retrieve that shows all data in table
                if (these_settings[7])
                {
                    s = t.gen_retrieve_by_all();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                //if selected create an sp_retrive that gets all active data in the table
                if (these_settings[7])
                {
                    s = t.gen_retrieve_by_active();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                //if selected create an sp_select that gets all data by a foreign key
                if (true)
                {
                    s = t.gen_retrieve_by_fkey();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                //select distinct for drop downs
                if (true)
                {
                    s = t.gen_select_distinct_for_dropdown();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add SP_insert to add records to table
                if (these_settings[8])
                {
                    s = t.gen_insert();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    //file_write.sqlBuddy2.Write(s);
                }
                // if selected, add a trigger for inserts
                if (these_settings[9])
                {
                    s = t.gen_insert_trigger();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Triggers");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                // if selected, add a trigger for updates
                if (these_settings[10])
                {
                    s = t.gen_update_trigger();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Triggers");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                // if selected, add a triger for delets
                if (these_settings[11])
                {
                    s = t.gen_delete_trigger();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Triggers");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                //if selected, add a function that counts the number of records
                if (true)
                {
                    s = t.gen_count();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sql", "Stored_Procedures");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                //if selected, add a space for adding sample data
                if (true)
                {
                    s = t.gen_sample_space();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, tbx_databasename.Text, "sql", "Sample");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
            }
            foreach (table t in data_tables.all_tables)
            {
                String s = "";
                if (true)
                {
                    s = t.gen_IThingAccessor();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "IAccessor");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                //needs to be these settings 13
                if (true)
                {
                    s = t.gen_ThingCharpDatabaseAccessor();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "Accessor");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                //needs to be these settings 1.5
                if (true)
                {
                    s = t.gen_ThingCharpFileAccessor();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharpFile", "Accessor");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                //needs to be these settings[14]
                if (true)
                {
                    s = t.gen_IThingManager();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "IManager");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    s = t.gen_ThingMananger();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "Manager");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    s = t.gen_CSharpDataObject();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "DataObject");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 17
                {
                    s = t.genXAMLWindow();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "XAMLWindow");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 18
                {
                    s = t.genWindowCSharp();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharp", "CSharpWindowCode");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 19
                {
                    s = t.genJavaModel();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaModel", "Model");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                    s = t.genJavaModelNM();
                    if (t.foreign_keys.Count > 0)
                    {
                        try
                        {
                            file_write.fileWrite(s, t.name, "JavaModel", "ModelVM");
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                        }
                    }
                }
                if (true) //change this to these setings 20
                {
                    s = t.genJavaDAO();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaModelDAO", "ModelDAO");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
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
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaJSP", "CreateJSP");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 23
                {
                    s = t.genUploadJSP();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaJSP", "UploadJSP");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 24
                {
                    s = t.genCreateServelet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "CreateServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 24
                {
                    s = t.genUploadServlet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "UploadServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 24
                {
                    s = t.genExportServlet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "ExportServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 25
                {
                    s = t.genviewAllJSP();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaJSP", "ViewAllJSP");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 26
                {
                    s = t.genviewAllServlet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "ViewAllServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 27
                {
                    s = t.genViewEditWithLineItemsJSP();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaJSP", "ViewAllWithLineItemsJSP");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true) //change this to these setings 27
                {
                    s = t.genDeleteServlet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "DeleteServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (x == 0) //change this to these setings 28 && x==0
                {
                    x++;
                    s = t.genIndexJSP();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, "index", "JavaJSP", "Index");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {  //change this to these settings 29
                    s = t.genViewEditJSP();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaJSP", "ViewEditJSP");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 30
                    s = t.genViewEditServlet();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaServlet", "ViewEditServlet");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 31
                    s = t.jQueryValidation();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "jQuery", "AddEditValidate");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 31.5
                    s = t.jQueryDelete();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "jQuery", "DeleteValidate");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }

                if (true)
                {    //change to these settings 32
                    s = t.genJavaiDAO();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "iDAO", "Interface");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 33
                    s = t.sp_definitions();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "sp_definitions", "sp_def");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }

                if (true)
                {    //change to these settings 34
                    s = t.createJavaModelTests();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "Javatests", "tests");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 34
                    s = t.genCSharpManagerTests();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharpTests", "CSharpManager");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 34
                    s = t.createJavaModelVMTests();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name + "VM", "Javatests", "tests");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 34
                    s = t.createCSharpModelTests();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharpTests", "tests");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {    //change to these settings 34
                    s = t.createCSharpModelVMTests();
                    alloutput += s + "\n";
                    try
                    {
                        file_write.fileWrite(s, t.name + "VM", "CSharpTests", "tests");
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 35
                    s = t.genJavaDataAccessFakes();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Javafakes", "fakes");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 35
                    s = t.genCSharpDataAccessFakes();

                    try
                    {
                        file_write.fileWrite(s, t.name, "CSharpfakes", "fakes");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 36
                    s = t.genJavaGetAllServletTests();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Tests", "GetAll");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }

                if (true)
                {
                    //change to these setting 37
                    s = t.genJavaCreateServletTests();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Tests", "Create");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 38
                    s = t.genJavaDeleteServletTests();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Tests", "Delete");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 39
                    s = t.genJavaEditServletTests();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Tests", "Edit");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 40
                    s = t.genJavascriptObject();

                    try
                    {
                        file_write.fileWrite(s, t.name, "JavaScript", "Objects");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 40
                    s = t.genPythonObject();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Python", "Objects");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }
                if (true)
                {
                    //change to these setting 41
                    s = t.genPythonCommmands();

                    try
                    {
                        file_write.fileWrite(s, t.name, "Python", "Commands");

                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                    }
                }

                count++;
            }
            //file_write.sqlBuddy2.Flush();
            //file_write.CSharpBuddy.Flush();
            //file_write.XAMLBuddy.Flush();
            //file_write.BatchBuddy.Flush();
            //file_write.JSPBuddy.Flush();
            //file_write.ServletBuddy.Flush();

            _ = MessageBox.Show("generation complete");
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
            try
            {
                file_read.readdata();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                this.Close();
            }
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
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void tbx_owner_name_TextChanged(object sender, EventArgs e)
        {
            settings.owner_name = tbx_owner_name.Text;
        }
    }
}
