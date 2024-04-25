using appData2;
using System;
using System.Collections.Generic;

namespace Data_Objects
{
    public class TSqlTable : table, iTable
    {
        public TSqlTable(string name, List<Column> columns) : base(name, columns)
        {
            this.name = "[dbo].[" + name + "]";
            this.columns = columns;
        }

        //various components of a table




        public String gen_primary_keys()
        {
            //generate the primary keys based on key_gen that was done in the rwos
            
            
                {
                    name = name.Replace("[dbo].[", "");
                    name = name.Replace("]", "");
                    String key_string = "CONSTRAINT [PK_" + name + "] PRIMARY KEY (";
                    int count = 0;
                    foreach (Column r in columns)
                    {
                        foreach (string s in r.primary_keys)
                        {
                            if (count > 0) { key_string = key_string + " , "; }
                            key_string = key_string + s;
                            count++;
                        }
                    }
                    key_string = key_string + "),\n";

                    return key_string;
                }

                    }
        public String gen_foreign_keys()
        {//generate the foreign keys based on key_gen that was done in the rwos
            int count = 0;
            string and = "";
            foreign_keys = new List<String>();
            String output_keys = "";
            foreach (Column r in columns)
            {
                foreach (string s in r.foreign_keys)
                {
                    if (count > 0) { and = ",\n"; }
                    if (r.foreign_keys[0].Length > 0)
                    {
                        
                            String[] chunks = s.Split('.');
                            String second_table = chunks[0];
                            String formatted_key = and+"CONSTRAINT [fk_" + name + "_" + second_table + count + "] foreign key ([" + chunks[1] + "]) references [" + chunks[0] + "]([" + chunks[1] + "])" + "";

                            foreign_keys.Add(formatted_key);
                        
                        
                    }
                    count++;
                }
            };



            foreach (string tuv in foreign_keys)
            {
                String s = tuv;
                output_keys = output_keys + s;
            }
            
            return output_keys;
        }
        public string gen_alternate_keys()
        {//generate the foreign keys based on key_gen that was done in the rwos
            int count = 0;
            alternate_keys = new List<String>();
            String output_keys = "";
            foreach (Column r in columns)
            {
                {
                    if (r.unique == 'y' || r.unique == 'Y')
                    {
                        if (settings.TSQLMode)
                        {

                            String formatted_key = ",\vCONSTRAINT [AK_" + r.column_name.bracketStrip() + "] UNIQUE(" + r.column_name + ")";

                            alternate_keys.Add(formatted_key);
                        }
                        else
                        {

                            String formatted_key = "\n UNIQUE (" + r.column_name + ")\n";

                            alternate_keys.Add(formatted_key);

                        }
                    }
                    count++;
                }
            };



            foreach (string tuv in alternate_keys)
            {
                String s = tuv;
                output_keys = output_keys + s;
            }

            return output_keys;
        }

        public String gen_table_footer() {
            return ")\ngo\n";
        }
        public String gen_header()
        {
            Header.table_name = this.name;
            return this.Header.full_header_gen();
        }

        public String audit_gen_header()
        {
            Header.table_name = this.name;
            return this.Header.audit_header_gen();
        }
        public String gen_columns()
        {

            int count = 0;
            String x = this.gen_header();
            x = x + "";
            foreach (Column r in columns)
            {
                String rowtext = r.column_and_key_gen();
                if (count > 0) { x = x + ",\n"; }
                x = x + rowtext;
                count++;
            }
            x = x + ",\n";


            return x;
        }

        public String gen_audit_table()
        { // to generate the audit table
            int count = 0;
            String x = this.audit_gen_header();
            x = x + "\n";
            foreach (Column r in columns)
            {
                String rowtext = r.Column_row_gen();
                if (count > 0) { x = x + ","; }
                x = x + rowtext;
                count++;
            }
            x = x + ",action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'\n" +
                ", action_date DATETIME NOT NULL COMMENT 'when it happened'\n" +
                ", action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'\n";
            x = x + ");\n";


            return x;
        }
        // to generate the SP_update
        public String gen_update()
        {
            string x = "";
            string full_text = "";
            String comment_text = comment_box_gen.comment_box(name, 3);
            int count = 0;
            string comma = "";
            if (settings.TSQLMode)
            {
                string function_text = "";
                function_text = "CREATE PROCEDURE [dbo].[sp_update_" + name + "]\n(";
                count = 0;
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }

                    String add = comma + "\n@old" + r.column_name.bracketStrip() + r.data_type + r.length_text + "";
                    if (r.primary_key != 'y' || r.primary_key != 'Y')
                    {
                        add = add + ",\n@new" + r.column_name.bracketStrip() + r.data_type + r.length_text + "";
                    }
                    function_text = function_text + add;
                    count++;
                }
                comma = "";
                function_text = function_text + "\n)\nas\nBEGIN\nUPDATE " + name + "\nSET\n";
                count = 0;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (r.primary_key != 'Y' && r.primary_key != 'y')
                    {
                        function_text = function_text + comma + "\n"+r.column_name.bracketStrip() + " = @new" + r.column_name.bracketStrip() + "";
                        count++;
                    }


                }
                function_text = function_text + "\nWHERE\n";
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    function_text = function_text +"\n"+ comma + r.column_name.bracketStrip() + " = @old" + r.column_name.bracketStrip() + "";
                    count++;

                }



                function_text = function_text + "\nreturn @@rowcount\nend\ngo\n";
                full_text = comment_text + function_text;

            }
            
            return full_text;

        }
        // to generate the SP_delete
        public String gen_delete()
        {
            String function_text = "";

            String comment_text = comment_box_gen.comment_box(name, 4);
            if (settings.TSQLMode)
            {
                function_text = "create procedure [dbo].[sp_delete_" + name + "]\n(";
                int count = 0;
                String comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + "\n@" + r.column_name.bracketStrip() + " " + r.data_type + " " + r.length_text + "";
                        function_text = function_text + add;
                        count++;
                    }
                }
                function_text = function_text + "\n)\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text = function_text + "set active = 0\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = "\n"+comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "";
                        function_text = function_text + add;
                        count++;
                    }
                }

                function_text = function_text + "\nreturn @@rowcount \nend \ngo\n";


            }           

            String full_text = comment_text + function_text;
            return full_text;




        }

        public String gen_undelete()
        {
            String function_text = "";

            String comment_text = comment_box_gen.comment_box(name, 4);
            if (settings.TSQLMode)
            {
                function_text = "create procedure [dbo].[sp_undelete_" + name + "]\n(";
                int count = 0;
                String comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + "\n@" + r.column_name.bracketStrip() + " " + r.data_type + " " + r.length_text + "";
                        function_text = function_text + add;
                        count++;
                    }
                }
                function_text = function_text + "\n)\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text = function_text + "set active = 1\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = "\n"+comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "";
                        function_text = function_text + add;
                        count++;
                    }
                }

                function_text = function_text + "\nreturn @@rowcount \n end \n go\n";


            }



            

            String full_text = comment_text + function_text;
            return full_text;




        }
        // to generate the SP_retreive using a primary key
        public String gen_retreive_by_key()
        {

            String comment_text = comment_box_gen.comment_box(name, 5);
            
            
            
            
            String function_text = "CREATE PROCEDURE [dbo].[sp_retreive_by_pk_" + name + "]\n(";
            
            
            int count = 0;
            String comma = "";
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                     add = comma +"\n"+ r.column_name.Replace("]", "").Replace("[", "@") + " " + r.data_type + r.length_text + ""; 
                    
                    function_text = function_text + add;
                    count++;
                }
            }
            function_text = function_text + "\n)";

            count = 0;
            comma = "";
            String asString = "\nas";
            
            function_text = function_text + asString + "\n Begin \n select";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "" + genSelectLine(name, r.column_name);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {


                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                function_text = function_text + comma + genSelectLine(t.name, r.column_name);
                                count++;

                            }

                        }
                    }
                }
            }
            function_text = function_text + "\n FROM " + name + "\n";
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    function_text = function_text + "join [" + fk.referenceTable + "] on [" + fk.mainTable + "].[" + fk.fieldName + "] = [" + fk.referenceTable + "].[" + fk.fieldName + "]\n";
                }
            }
            String initial_word = "where ";
            int keys_count = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    string add = "";
                     add = initial_word + r.column_name + "=" + r.column_name.Replace("]", "").Replace("[", "@") + " \n"; 
                    
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            
                function_text = function_text + " END \n" +
                       " GO\n";
            
            



            String full_text = comment_text + function_text;
            return full_text;
        }

        //to generate retreive by fk, not implmented well yet
        public String gen_retreive_by_fkey(foreignKey key)
        {
            String comma = "";
            int count = 0;
            String comment_text = comment_box_gen.comment_box(name, 200);
            

            
            String    function_text = "CREATE PROCEDURE [dbo].[sp_retreive_" + key.referenceTable + "by" + key.mainTable + "_ID]\n(";
            
            


            String add = "";
            add = "@" + key.fieldName.bracketStrip() + " " + key.dataType + key.lengthText + "\n"; 
            
            function_text = function_text + add;



            function_text = function_text + ")";


            String asString = "\nas";
            
            function_text = function_text + asString + "\n Begin \n select \n";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + "\n"+comma + genSelectLine(name, r.column_name);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {


                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                function_text = function_text + comma + genSelectLine(t.name, r.column_name);
                                count++;

                            }

                        }
                    }
                }
            }
            function_text = function_text + "\n FROM " + name + "\n";
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    function_text = function_text + "join [" + fk.referenceTable + "] on [" + fk.mainTable + "].[" + fk.fieldName + "] = [" + fk.referenceTable + "].[" + fk.fieldName + "]\n";
                }
            }
            String initial_word = "where ";
            int keys_count = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    add = "";
                     add = initial_word + r.column_name + "=" + r.column_name.Replace("]", "").Replace("[", "@") + " \n"; 
                    
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            
                function_text = function_text + " END \n" +
                       " GO\n";
            
            



            String full_text = comment_text + function_text;
            return full_text;
        }

        // to generate the SP_retrive, showing all data in a table
        public String gen_retreive_by_all()
        {
            String gx = " ";
            String comment_text = comment_box_gen.comment_box(name, 6);
            
            
            
            String    firstLine = "";
            String    secondLine = "CREATE PROCEDURE [dbo].[sp_retreive_by_all_" + name + "]\nAS\n";
            
            String function_text = firstLine + secondLine;



            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            function_text = function_text + "begin \n SELECT ";
            count = 0;
            comma = "";

            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma+ genSelectLine(name, r.column_name);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {


                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                function_text = function_text + comma + genSelectLine(t.name, r.column_name);
                                count++;

                            }

                        }
                    }
                }
            }
            function_text = function_text + "\n FROM " + name + "\n";
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    function_text = function_text + "join [" + fk.referenceTable + "] on [" + fk.mainTable + "].[" + fk.fieldName + "] = [" + fk.referenceTable + "].[" + fk.fieldName + "]\n";
                }
            }
            function_text = function_text + "\n ;\n END  \n GO\n"; 
            



            String full_text = comment_text + function_text;
            return full_text;
        }
        public String gen_retreive_by_active()
        {
            String gx = " ";
            String comment_text = comment_box_gen.comment_box(name, 24);



            String firstLine = "";
            String secondLine = "CREATE PROCEDURE [dbo].[sp_retreive_by_active_" + name + "]\nAS\n";

            String function_text = firstLine + secondLine;



            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            function_text = function_text + "begin \n SELECT ";
            count = 0;
            comma = "";

            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text  + comma + genSelectLine(name, r.column_name);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {


                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                function_text = function_text + comma + genSelectLine(t.name, r.column_name);
                                count++;

                            }

                        }
                    }
                }
            }
            function_text = function_text + "\n FROM " + name + "\n";
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    function_text = function_text + "join [" + fk.referenceTable + "] on [" + fk.mainTable + "].[" + fk.fieldName + "] = [" + fk.referenceTable + "].[" + fk.fieldName + "]\n";
                }
            }
            function_text = function_text + "\n " + "where is_active=1\n" + ";\n END  \n GO\n";




            String full_text = comment_text + function_text;
            return full_text;
        }

        // to generate the SP_insert
        public string gen_insert()
        {
            String comment_text = comment_box_gen.comment_box(name, 7);
            
            String function_text = "CREATE PROCEDURE [dbo].[sp_insert" + name + "]\n(";
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (count > 0) { comma = ","; }
                    string add = "";
                    add = ""+comma + "\n@" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + ""; 
                    
                    function_text = function_text + add;
                    count++;
                }
            }
            if (settings.TSQLMode)
            {
                function_text = function_text + "\n)\nas \n begin\n insert into [dbo]." + name + "(\n";
                comma = "";
                foreach (Column r in columns)
                {
                    if (r.increment == 0)
                    {
                        function_text = function_text + comma + r.column_name;
                        comma = ",";
                    }
                }
            }
            
            count = 0;
            comma = "";


            
                function_text = function_text + "\n)\n VALUES \n(";
                comma = "";
                count = 0;
                foreach (Column r in columns)
                {
                    if (r.increment == 0)
                    {
                        if (count > 0) { comma = ","; }
                        function_text = function_text + comma + "\n@" + r.column_name.bracketStrip() + "";
                        count++;
                    }

                }
                function_text = function_text + "\n)\n";
            
            
            
             function_text = function_text + "return @@rowcount\nend\nGo\n"; 



            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the on update trigger
        public String gen_update_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 8);
            String function_text = "\n";

            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the on insert trigger
        public String gen_insert_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 9);
            String function_text = "\n";

            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the on delete trigger
        public String gen_delete_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 10);
            String function_text = "\n";

            String full_text = comment_text + function_text;
            return full_text;
        }
        public String gen_select_distinct_for_dropdown()
        {
            String gx = " ";
            String comment_text = comment_box_gen.comment_box(name, 27);



            String firstLine = "";
            String secondLine = "CREATE PROCEDURE [dbo].[sp_select_distinct_and_active_" + name+"for_dropdown]\nAS\n";

            String function_text = firstLine + secondLine;



            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            function_text = function_text + "begin \n SELECT DISTINCT \n";
            count = 0;
            comma = "";

            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    function_text = function_text + "\n" + comma + r.column_name;
                    count++;
                }
            }
            function_text = function_text + "\n FROM " + name + "\n " +
                "where is_active=1\n" +
                ";\n END  \n GO\n";




            String full_text = comment_text + function_text;
            return full_text;
        }


        private string genSelectLine(string tablename, string column_name)
        {


            return "\n["+tablename + "]." + column_name + " as \'" + tablename + "_" + column_name.bracketStrip() + "\'";
        }

        public string gen_sample_space()
        {
            string result = "";
            result = result + "\n";

            //comment box
            result = result + comment_box_gen.comment_box(name, 18);

            result = result + "\n";
            result = result + "/***************************************\n";
            result = result + "INSERT INTO " + name + " (";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.start == 0)
                {
                    result = result + "\n" + comma + r.column_name;
                    comma = ",";
                }
            }
            comma = "";
            result = result + "\n)\nVALUES";
            for (int i = 0; i < 5; i++)
            {
                result = result + "\n" + comma + "(";
                comma = "";
                foreach (Column r in columns)
                {
                    if (r.start == 0)
                    {
                        result = result + comma + "' '";
                        comma = ",";
                    }
                }
                result = result + ")";
            }

            result = result + "\n;\n";
            result = result + "*******************************/\n";


            return result;



        }

        
    }
}
