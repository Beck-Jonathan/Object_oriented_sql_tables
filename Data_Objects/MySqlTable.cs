using appData2;
using System;
using System.Collections.Generic;
namespace Data_Objects
{
    public class MySqlTable : table, iTable
    {
        public MySqlTable(string name, List<Column> columns) : base(name, columns)
        {
            this.name = name;
            this.columns = columns;
        }
        //various components of a table
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the primary keys of the MySQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the the Pimary Key(s) of the table </returns>
        public String gen_primary_keys()
        {
            //generate the primary keys based on key_gen that was done in the rwos
            String key_string = "CONSTRAINT " + name + "_PK PRIMARY KEY (";
            int count = 0;
            foreach (Column r in columns)
            {
                foreach (string s in r.primary_keys)
                {
                    if (count > 0) { key_string += " , "; }
                    key_string += s;
                    count++;
                }
            }
            key_string += "),";
            return key_string;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the foreign keys of the MySQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the the foreign Key(s) of the table </returns>
        public String gen_foreign_keys()
        {//generate the foreign keys based on key_gen that was done in the rwos
            int count = 0;
            foreign_keys = new List<String>();
            String output_keys = "";
            foreach (Column r in columns)
            {
                foreach (string s in r.foreign_keys)
                {
                    if (r.foreign_keys[0].Length > 0)
                    {
                        string start = "";
                        if (count > 0) { start = ",\n"; }
                        String[] chunks = s.Split('.');
                        String second_table = chunks[0];
                        String formatted_key = start + "CONSTRAINT fk_" + name + "_" + second_table + count + " foreign key (" + chunks[1] + ") references " + chunks[0] + " (" + chunks[1] + ")" + "";
                        foreign_keys.Add(formatted_key);
                    }
                    count++;
                }
            };
            foreach (string tuv in foreign_keys)
            {
                String s = tuv;
                output_keys += s;
            }
            return output_keys;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the alternate keys of the MySQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the the alternate Key(s) of the table </returns>
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
                        String formatted_key = ",\n UNIQUE (" + r.column_name + ")";
                        alternate_keys.Add(formatted_key);
                    }
                    count++;
                }
            };
            foreach (string tuv in alternate_keys)
            {
                String s = tuv;
                output_keys += s;
            }
            return output_keys;
        }
        /// <summary>
        /// 
        /// generates lines that specify the header of the MySQL  <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the header of the  <see cref="table"/> </returns>
        public String gen_header()
        {
            Header.table_name = name;
            return this.Header.full_header_gen();
        }
        /// <summary>
        /// Generates a genertic footer for a MySQL  <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A stringto act as a footer for the MySQL  <see cref="table"/> </returns>
        public String gen_table_footer()
        {
            return ");\n";
        }
        /// <summary>
        /// 
        /// generates lines that specify the header of the MySQL audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the header of the audit <see cref="table"/> </returns>
        public String audit_gen_header()
        {
            Header.table_name = this.name;
            return this.Header.audit_header_gen();
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the alternate various columns and their attributes of the MySQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the the columns of the table </returns>
        public String gen_columns()
        {
            int count = 0;
            String x = this.gen_header();
            x += "\n";
            foreach (Column r in columns)
            {
                String rowtext = r.column_and_key_gen();
                if (count > 0) { x += ",\n"; }
                x += rowtext;
                count++;
            }
            x += ",\n";
            return x;
        }
        /// <summary>
        /// 
        /// generates lines that specify the the MySQL audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string MySQL code that creates the audit <see cref="table"/> </returns>
        public String gen_audit_table()
        { // to generate the audit table
            int count = 0;
            String x = this.audit_gen_header();
            x += "\n";
            foreach (Column r in columns)
            {
                String rowtext = r.Column_row_gen();
                if (count > 0) { x += ","; }
                x += rowtext;
                count++;
            }
            x = x + ",action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'\n" +
                ", action_date DATETIME NOT NULL COMMENT 'when it happened'\n" +
                ", action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'\n";
            x += ");\n";
            return x;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by  a MySQL stored procedure that creates a standard update function. This funciton will ask for @old and @new of each field, 
        /// besides primary key fields, which just ask for @old versions.
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string MySQL code that creates the the update SP for the table </returns>
        public String gen_update()
        {
            string full_text = commentBox.genCommentBox(name, Component_Enum.SQL_Update);
            int count = 0;
            string comma = "";
            string firstLine = "DROP PROCEDURE IF EXISTS sp_update_" + name + ";\n DELIMITER $$\n";
            string secondLine = "CREATE PROCEDURE sp_update_" + name + "\n"
                + "(";
            String function_text =
                 firstLine + secondLine; foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                String add = comma + "old" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    add = add + ",new" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "\n";
                }
                function_text += add;
                count++;
            }
            function_text = function_text + ")\n" +
                "begin \n" +
                "UPDATE " + name + "\n set "
                ;
            comma = "";
            count = 0;
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (!r.primary_key.Equals('y') && !r.primary_key.Equals('Y'))
                {
                    String add = comma + r.column_name + " = " + "new" + r.column_name + "\n";
                    function_text += add;
                    count++;
                }
            }
            int keys_count = 0;
            String initial_word = "WHERE ";
            foreach (Column r in columns)
            {
                if (keys_count > 0) { initial_word = "AND "; }
                String add = initial_word + r.column_name + "= old" + r.column_name + "\n";
                function_text += add;
                keys_count++;
            }
            function_text = function_text + "\n ; end $$\n" +
               " DELIMITER ;\n";
            full_text += function_text;
            return full_text;
        }
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a MySQL stored procedure that creates a standard Delete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string comment box followed by a  string MySQL code that creates the the delete SP for the table </returns>
        public String gen_delete()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Delete);
            string function_text =
     "DROP PROCEDURE IF EXISTS sp_delete_" + name + ";\n"
    + "DELIMITER $$\n"
    + "CREATE PROCEDURE sp_delete_" + name + "\n"
    + "(";
            int count;
            string comma = "";
            count = 0;
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = comma + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                    function_text += add;
                    count++;
                }
            }
            function_text = function_text + ")\n" +
                "BEGIN\n" +
                "UPDATE " + name + "\n  "
                ;
            function_text += " set is_active=0\n";
            int keys_count = 0;
            String initial_word = "WHERE ";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    String add = initial_word + r.column_name + "=" + r.column_name + "_param\n";
                    function_text += add;
                    keys_count++;
                }
            }
            function_text = function_text + "\n" +
               " ; END $$\n" +
               " DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a MySQL stored procedure that creates a standard unDelete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string MySQL code that creates the the undelete SP for the table </returns>
        public String gen_undelete()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Undelete);
            string function_text =
     "DROP PROCEDURE IF EXISTS sp_undelete_" + name + ";\n"
    + "DELIMITER $$\n"
    + "CREATE PROCEDURE sp_undelete_" + name + "\n"
    + "(";
            String comma = "";
            int count = 0;
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = comma + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                    function_text += add;
                    count++;
                }
            }
            function_text = function_text + ")\n" +
                "BEGIN\n" +
                "UPDATE " + name + "\n  "
                ;
            function_text += " set is_active=1\n";
            int keys_count = 0;
            String initial_word = "WHERE ";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    String add = initial_word + r.column_name + "=" + r.column_name + "_param\n";
                    function_text += add;
                    keys_count++;
                }
            }
            function_text = function_text + "\n" +
               " ; END $$\n" +
               " DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a MySQL stored procedure that creates a retreive by primary key function. This funciton will ask for the pimary key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string MySQL code that creates the the retreive by Primary key SP for the table </returns>
        public String gen_retreive_by_key()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Retreive_By_PK);
            String firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_by_pk_" + name + ";\n"
                + "DELIMITER $$\n";
            String secondLine = "CREATE PROCEDURE sp_retreive_by_pk_" + name + "\n"
                + "(\n";
            String function_text = firstLine + secondLine;
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                    add = comma + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                    function_text += add;
                    count++;
                }
            }
            function_text += ")";
            count = 0;
            comma = "";
            String asString = "";
            if (settings.TSQLMode) { asString = "\nas"; }
            function_text = function_text + asString + "\n Begin \n select \n";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + genSelectLine(name, r.column_name);
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
                    function_text = function_text + "join " + fk.referenceTable + " on " + fk.mainTable + "." + fk.fieldName + " = " + fk.referenceTable + "." + fk.fieldName + "\n";
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
                    add = initial_word + r.column_name + "=" + r.column_name + "_param\n";
                    function_text += add;
                    keys_count++;
                }
            }
            function_text = function_text + " ; END $$\n" +
               " DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a MySQL stored procedure that creates a retreive by foreign key function. This funciton will ask for a foregn key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string MySQL code that creates the the retreive by Foreign-key SP for the table </returns>
        public String gen_retreive_by_fkey()
        {
            string fulltext = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    String comma = "";
                    int count = 0;
                    String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Retreive_By_FK);
                    String firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_" + name + "_by_" + fk_table + ";\n"
                        + "DELIMITER $$\n";
                    String secondLine = "CREATE PROCEDURE sp_retreive_" + name + "_by_" + fk_table + " \n"
                        + "(\n";
                    String function_text = firstLine + secondLine;
                    String add = "";
                    add = fk_name + "_param " + r.data_type + r.length_text + ",\n";
                    function_text = function_text + add +
                    "limit_param int ,\n " +
                        "offset_param int \n";
                    function_text += ")";
                    String asString = "";
                    function_text = function_text + asString + "\n Begin \n select \n";
                    foreach (Column s in columns)
                    {
                        if (count > 0) { comma = ","; }
                        function_text = function_text + comma + genSelectLine(name, s.column_name);
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
                                    foreach (Column q in t.columns)
                                    {
                                        if (count > 0) { comma = ","; }
                                        function_text = function_text + comma + genSelectLine(t.name, q.column_name);
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
                            function_text = function_text + "join " + fk.referenceTable + " on " + fk.mainTable + "." + fk.fieldName + " = " + fk.referenceTable + "." + fk.fieldName + "\n";
                        }
                    }
                    String initial_word = "where ";
                    int keys_count = 0;
                    foreach (Column t in columns)
                    {
                        if (t.primary_key.Equals('y') || t.primary_key.Equals('Y'))
                        {
                            if (keys_count > 0) { initial_word = "AND "; }
                            add = "";
                            add = initial_word + name + "." + fk_name + "=" + fk_name + "_param\n";
                            function_text += add;
                            keys_count++;
                        }
                    }
                    function_text += "\nORDER BY ";
                    count = 0;
                    comma = "";
                    foreach (Column t in columns)
                    {
                        if (count > 0) { comma = ","; }
                        if (t.primary_key.Equals('y') || t.primary_key.Equals('Y'))
                        {
                            add = "";
                            add = comma + t.column_name + "\n";
                            function_text += add;
                            count++;
                        }
                    }
                    function_text += "limit limit_param\n";
                    function_text += "offset offset_param\n";
                    function_text = function_text + " ; END $$\n" +
                           " DELIMITER ;\n";
                    fulltext = fulltext + comment_text + function_text;
                }
            }
            return fulltext;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a MySQL stored procedure that creates a retreive  all key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string MySQL code that creates the the retreive by all SP for the table </returns>
        public String gen_retreive_by_all()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Retreive_By_All);
            string firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_by_all_" + name + ";\n"
                + "DELIMITER $$\n";
            string secondLine = "CREATE PROCEDURE sp_retreive_by_all_" + name + "(\n" +
                "limit_param int ,\n " +
                "offset_param int \n" +
                ")" +
                "\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT \n";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + "" + comma + genSelectLine(name, r.column_name);
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
                    function_text = function_text + "join " + fk.referenceTable + " on " + fk.mainTable + "." + fk.fieldName + " = " + fk.referenceTable + "." + fk.fieldName + "\n";
                }
            }
            function_text += "\nORDER BY ";
            count = 0;
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                    add = comma + r.column_name + "\n";
                    function_text += add;
                    count++;
                }
            }
            function_text += "limit limit_param\n";
            function_text += "offset offset_param\n";
            function_text += "\n ;\n END $$ \n DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates generates a string comment box followed by MySQL stored procedure that creates a retreive active (that is, is_active==1) key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a MySQL code that creates the the retreive by active SP for the table </returns>
        public String gen_retreive_by_active()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Retreive_Active);
            string firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_by_active_" + name + ";\n"
                + "DELIMITER $$\n";
            string secondLine = "CREATE PROCEDURE sp_retreive_by_active_" + name + "()\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT \n";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + "" + comma + genSelectLine(name, r.column_name);
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
                    function_text = function_text + "join " + fk.referenceTable + " on " + fk.mainTable + "." + fk.fieldName + " = " + fk.referenceTable + "." + fk.fieldName + "\n";
                }
            }
            function_text = function_text + "where is_active=1\n" +
                " ;\n END $$ \n DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a MySQL stored procedure that creates a standard insert function. This funciton will ask for  each field, 
        /// besides auto-increment fields.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by MySQL code that creates the the insert SP for the table </returns>
        public string gen_insert()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Insert);
            String firstLine =
                 "DROP PROCEDURE IF EXISTS sp_insert_" + name + ";\n"
                + "DELIMITER $$\n";
            String secondLine = "CREATE PROCEDURE sp_insert_" + name + "(\n";
            String function_text = firstLine + secondLine;
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.default_value == "")
                {
                    if (count > 0) { comma = ","; }
                    string add = "";
                    add = comma + "in " + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                    function_text += add;
                    count++;
                }
            }
            function_text = function_text + ")\n" +
            "begin \n" +
            "INSERT INTO  " + name + "\n(";
            comma = "";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.default_value == "")
                {
                    function_text = function_text + comma + r.column_name;
                    comma = ",";
                }
            }
            function_text += ")\n values \n(";
            count = 0;
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.increment == 0 && r.default_value == "")
                {
                    String add = comma + r.column_name + "_param\n";
                    function_text += add;
                    count++;
                }
            }
            function_text += ")\n";
            function_text = function_text +
               " ; END $$\n" +
               " DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a MySQL stored procedure that creates a standard trigger that fires upon updates to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by MySQL code that creates a trigger that fires on updates to the <see cref="table"/> object </returns>
        public String gen_update_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Update_Trigger);
            String function_text = "DELIMITER $$\n"
                + "DROP TRIGGER IF EXISTS tr_" + name + "_after_update $$\n"
                + "CREATE TRIGGER tr_" + name + "_after_update\n"
                + "AFTER UPDATE ON " + name + "\n"
                + "for each row\n"
                + "begin\n"
                + "insert into " + name + "_audit (\n";
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "new." + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n , 'update'-- action_type"
                 + "\n, NOW()-- action_date"
                + "\n,  CURRENT_USER()-- action_user"
                + "\n)"
                + "\n;"
                + "\nend  $$"
                + "\nDELIMITER ;"
                + "\n   ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a MySQL stored procedure that creates a standard trigger that fires upon inserts to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by MySQL code that creates a trigger that fires on inserts to the <see cref="table"/> object </returns>
        public String gen_insert_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Insert_Trigger);
            String function_text = "DELIMITER $$\n"
                + "DROP TRIGGER IF EXISTS tr_" + name + "_after_insert $$\n"
                + "CREATE TRIGGER tr_" + name + "_after_insert\n"
                + "AFTER insert ON " + name + "\n"
                + "for each row\n"
                + "begin\n"
                + "insert into " + name + "_audit (\n";
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "new." + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n , 'insert'-- action_type"
                 + "\n, NOW()-- action_date"
                + "\n,  CURRENT_USER()-- action_user"
                + "\n)"
                + "\n;"
                + "\nend  $$"
                + "\nDELIMITER ;"
                + "\n   ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a MySQL stored procedure that creates a standard trigger that fires upon deletes to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by MySQL code that creates a trigger that fires on deletes to the <see cref="table"/> object </returns>
        public String gen_delete_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Delete_Trigger);
            String function_text = "DELIMITER $$\n"
                + "DROP TRIGGER IF EXISTS tr_" + name + "_after_delete $$\n"
                + "CREATE TRIGGER tr_" + name + "_after_delete\n"
                + "AFTER delete ON " + name + "\n"
                + "for each row\n"
                + "begin\n"
                + "insert into " + name + "_audit (\n";
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "old." + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n , 'delete'-- action_type"
                 + "\n, NOW()-- action_date"
                + "\n,  CURRENT_USER()-- action_user"
                + "\n)"
                + "\n;"
                + "\nend  $$"
                + "\nDELIMITER ;"
                + "\n   ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        public String gen_select_distinct_for_dropdown()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Select_Distinct);
            string firstLine = "DROP PROCEDURE IF EXISTS sp_select_distinct_and_active_" + name + "_for_dropdown;\n"
                + "DELIMITER $$\n";
            string secondLine = "CREATE PROCEDURE sp_select_distinct_and_active_" + name + "_for_dropdown()\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT DISTINCT\n";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    function_text = function_text + "\n" + comma + r.column_name;
                }
                count++;
            }
            function_text = function_text + "\n FROM " + name + "\n" +
                "where is_active=1\n" +
                " ;\n END $$ \n DELIMITER ;\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a standard MySQL select line for to be used by the stored procedures, such as select by PK or select by FK.
        /// Takes a string table name nad string column name as paramaters
        /// Jonathan Beck       
        /// </summary>
        ///<param name = "tablename" >the name of <see cref="table"/> this <see cref="Column"/> belongs to </param>
        ///<param name="column_name">the name of this <see cref="Column"/></param>
        /// <returns>  a standard MySQL select line to be used by the stored procedures, </returns>
        private string genSelectLine(string tablename, string column_name)
        {
            return "\n" + tablename + "." + column_name + " as \'" + tablename + "_" + column_name.bracketStrip() + "\'";
        }
        /// <summary>       
        /// generates a string comment box followed by a MySQL insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields.. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by MySQL insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields. </returns>
        public string gen_sample_space()
        {
            string result = "";
            result += "\n";
            //comment box
            result += commentBox.genCommentBox(name, Component_Enum.SQL_Sample_Data);
            result += "\n";
            result += "/***************************************\n";
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
            result += "\n)\nVALUES";
            for (int i = 0; i < 5; i++)
            {
                result = result + "\n" + comma + "(";
                comma = "";
                string quotes = "' '";
                foreach (Column r in columns)
                {
                    if (r.data_type.ToLower().Contains("int") || r.data_type.ToLower().Contains("bool")
                        || r.data_type.ToLower().Contains("bit")
                        || r.data_type.ToLower().Contains("money"))
                    {
                        quotes = "";
                    }
                    if (r.start == 0)
                    {
                        result = result + comma + quotes;
                        comma = ",";
                        quotes = "' '";
                    }
                }
                result += ")";
            }
            result += "\n;\n";
            result += "*******************************/\n";
            return result;
        }
    }
}
