﻿using appData2;
using System;
using System.Collections.Generic;
namespace Data_Objects
{
    public class TSqlTable : table, iTable
    {
        ///<inheritdoc>
        public TSqlTable(string name, List<Column> columns) : base(name, columns)
        {
            this.name = "[dbo].[" + name + "]";
            this.columns = columns;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the primary keys of the Transact-SQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the the Pimary Key(s) of the table </returns>
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
                        if (count > 0) { key_string += " , "; }
                        key_string += s;
                        count++;
                    }
                }
                key_string += "),\n";
                return key_string;
            }
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the foreign keys of the Transact-SQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the the foreign Key(s) of the table </returns>
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
                        String formatted_key = and + "CONSTRAINT [fk_" + name + "_" + second_table + count + "] foreign key ([" + chunks[1] + "]) references [" + chunks[0] + "]([" + chunks[1] + "])" + "";
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
        /// generates lines that specify the alternate keys of the Transact-SQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the the alternate Key(s) of the table </returns>
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
                output_keys += s;
            }
            return output_keys;
        }
        /// <summary>
        /// Generates a genertic footer for a Transact-SQL  <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A stringto act as a footer for the Transact-SQL  <see cref="table"/> </returns>
        public String gen_table_footer()
        {
            return ")\ngo\n";
        }
        /// <summary>
        /// 
        /// generates lines that specify the header of the Transact-SQL  <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the header of the  <see cref="table"/> </returns>
        public String gen_header()
        {
            Header.table_name = this.name;
            return this.Header.full_header_gen();
        }
        /// <summary>
        /// 
        /// generates lines that specify the header of the Transact-SQL audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the header of the audit <see cref="table"/> </returns>
        public String audit_gen_header()
        {
            Header.table_name = this.name;
            return this.Header.audit_header_gen();
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the alternate various columns and their attributes of the Transact-SQL Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the the columns of the table </returns>
        public String gen_columns()
        {
            int count = 0;
            String x = this.gen_header();
            x += "";
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
        /// generates lines that specify the the Transact-SQL audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string Transact-SQL code that creates the audit <see cref="table"/> </returns>
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
        /// generates a string comment box followed by  a Transact-SQL stored procedure that creates a standard update function. This funciton will ask for @old and @new of each field, 
        /// besides primary key fields, which just ask for @old versions.
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string Transact-SQL code that creates the the update SP for the table </returns>
        public String gen_update()
        {
            string full_text = "";
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Delete);
            if (settings.TSQLMode)
            {
                string function_text = "CREATE PROCEDURE [dbo].[sp_update_" + name + "]\n(";
                int count = 0;
                string comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    String add = comma + "\n@old" + r.column_name.bracketStrip() + r.data_type + r.length_text + "";
                    if (r.primary_key != 'y' || r.primary_key != 'Y')
                    {
                        add = add + ",\n@new" + r.column_name.bracketStrip() + r.data_type + r.length_text + "";
                    }
                    function_text += add;
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
                        function_text = function_text + comma + "\n" + r.column_name.bracketStrip() + " = @new" + r.column_name.bracketStrip() + "";
                        count++;
                    }
                }
                function_text += "\nWHERE\n";
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    function_text = function_text + "\n" + comma + r.column_name.bracketStrip() + " = @old" + r.column_name.bracketStrip() + "";
                    count++;
                }
                function_text += "\nreturn @@rowcount\nend\ngo\n";
                full_text = comment_text + function_text;
            }
            return full_text;
        }
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a Transact-SQL stored procedure that creates a standard Delete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string comment box followed by a  string Transact-SQL code that creates the the delete SP for the table </returns>
        public String gen_delete()
        {
            String function_text = "";
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Delete);
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
                        function_text += add;
                        count++;
                    }
                }
                function_text += "\n)\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text += "set active = 0\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = "\n" + comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "";
                        function_text += add;
                        count++;
                    }
                }
                function_text += "\nreturn @@rowcount \nend \ngo\n";
            }
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a Transact-SQL stored procedure that creates a standard unDelete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string Transact-SQL code that creates the the undelete SP for the table </returns>
        public String gen_undelete()
        {
            String function_text = "";
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Undelete);
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
                        function_text += add;
                        count++;
                    }
                }
                function_text += "\n)\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text += "set active = 1\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = "\n" + comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "";
                        function_text += add;
                        count++;
                    }
                }
                function_text += "\nreturn @@rowcount \n end \n go\n";
            }
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a Transact-SQL stored procedure that creates a retrieve by primary key function. This funciton will ask for the pimary key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string Transact-SQL code that creates the the retrieve by Primary key SP for the table </returns>
        public String gen_retrieve_by_key()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_retrieve_By_PK);
            String function_text = "CREATE PROCEDURE [dbo].[sp_retrieve_by_pk_" + name + "]\n(";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                    add = comma + "\n" + r.column_name.Replace("]", "").Replace("[", "@") + " " + r.data_type + r.length_text + "";
                    function_text += add;
                    count++;
                }
            }
            function_text += "\n)";
            count = 0;
            comma = "";
            String asString = "\nas";
            function_text = function_text + asString + "\n Begin \n select";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "" + gen_Select_Line(name, r.column_name);
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
                                function_text = function_text + comma + gen_Select_Line(t.name, r.column_name);
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
                    function_text += add;
                    keys_count++;
                }
            }
            function_text = function_text + " END \n" +
                   " GO\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a Transact-SQL stored procedure that creates a retrieve by foreign key function. This funciton will ask for a foregn key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string Transact-SQL code that creates the the retrieve by Foreign-key SP for the table </returns>
        public String gen_retrieve_by_fkey()
        {
            String full_text = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    String comma = "";
                    int count = 0;
                    String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_retrieve_By_FK);
                    String function_text = "CREATE PROCEDURE [dbo].[sp_retrieve_" + name + "_by_" + fk_table + "]\n(";
                    String add = "";
                    add = "@" + fk_name.bracketStrip() + "_param " + r.data_type + r.length_text + "\n";
                    function_text = function_text + add +
                        "@limit_param int\n" +
                        "@offset_param int\n ";
                    function_text += ")";
                    String asString = "\nas";
                    function_text = function_text + asString + "\n Begin \n select \n";
                    foreach (Column s in columns)
                    {
                        if (count > 0) { comma = ","; }
                        function_text = function_text + comma + gen_Select_Line(name, s.column_name);
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
                                    foreach (Column u in t.columns)
                                    {
                                        if (count > 0) { comma = ","; }
                                        function_text = function_text + comma + gen_Select_Line(t.name, u.column_name);
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
                    foreach (Column s in columns)
                    {
                        if (s.primary_key.Equals('y') || s.primary_key.Equals('Y'))
                        {
                            if (keys_count > 0) { initial_word = "AND "; }
                            add = "";
                            add = initial_word + name + "." + fk_name + "=" + s.column_name.Replace("]", "").Replace("[", "@") + "_param\n";
                            function_text += add;
                            keys_count++;
                        }
                    }
                    function_text += "\nORDER BY ";
                    count = 0;
                    foreach (Column s in columns)
                    {
                        if (count > 0) { comma = ","; }
                        if (s.primary_key.Equals('y') || s.primary_key.Equals('Y'))
                        {
                            add = "";
                            add = comma + "[" + s.column_name + "]\n";
                            function_text += add;
                            count++;
                        }
                    }
                    function_text += "limit @limit_param\n";
                    function_text += "offset @offset_param\n";
                    function_text = function_text + " END \n" +
                               " GO\n";
                    full_text += comment_text + function_text;
                }
            }
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a Transact-SQL stored procedure that creates a retrieve  all key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string Transact-SQL code that creates the the retrieve by all SP for the table </returns>
        public String gen_retrieve_by_all()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_retrieve_By_All);
            String firstLine = "";
            String secondLine = "CREATE PROCEDURE [dbo].[sp_retrieve_by_all_" + name + "](\n" +
                "@limit_param int\n" +
                "@offset_param int " +
                ")\nAS\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT ";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + gen_Select_Line(name, r.column_name);
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
                                function_text = function_text + comma + gen_Select_Line(t.name, r.column_name);
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
            function_text += "\nORDER BY ";
            count = 0;
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                    add = comma + "[" + r.column_name + "]\n";
                    function_text += add;
                    count++;
                }
            }
            function_text += "limit @limit_param\n";
            function_text += "offset @offset_param\n";
            function_text += "\n ;\n END  \n GO\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates generates a string comment box followed by Transact-SQL stored procedure that creates a retrieve active (that is, is_active==1) key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a Transact-SQL code that creates the the retrieve by active SP for the table </returns>
        public String gen_retrieve_by_active()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_retrieve_Active);
            String firstLine = "";
            String secondLine = "CREATE PROCEDURE [dbo].[sp_retrieve_by_active_" + name + "]\nAS\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT ";
            int count = 0;
            string comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + gen_Select_Line(name, r.column_name);
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
                                function_text = function_text + comma + gen_Select_Line(t.name, r.column_name);
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
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a Transact-SQL stored procedure that creates a standard insert function. This funciton will ask for  each field, 
        /// besides auto-increment fields.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by Transact-SQL code that creates the the insert SP for the table </returns>
        public string gen_insert()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Insert);
            String function_text = "CREATE PROCEDURE [dbo].[sp_insert" + name + "]\n(";
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (count > 0) { comma = ","; }
                    string add = "";
                    add = "" + comma + "\n@" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "";
                    function_text += add;
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
            function_text += "\n)\n VALUES \n(";
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
            function_text += "\n)\n";
            function_text += "return @@rowcount\nend\nGo\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a Transact-SQL stored procedure that creates a standard trigger that fires upon updates to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by Transact-SQL code that creates a trigger that fires on updates to the <see cref="table"/> object </returns>
        public String gen_update_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Update_Trigger);
            String function_text = "\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a Transact-SQL stored procedure that creates a standard trigger that fires upon inserts to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by Transact-SQL code that creates a trigger that fires on inserts to the <see cref="table"/> object </returns>
        public String gen_insert_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Insert_Trigger);
            String function_text = "\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        /// <summary>       
        /// generates a string comment box followed by a Transact-SQL stored procedure that creates a standard trigger that fires upon deletes to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by Transact-SQL code that creates a trigger that fires on deletes to the <see cref="table"/> object </returns>
        public String gen_delete_trigger()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Delete_Trigger);
            String function_text = "\n";
            String full_text = comment_text + function_text;
            return full_text;
        }
        public String gen_select_distinct_for_dropdown()
        {
            String comment_text = commentBox.genCommentBox(name, Component_Enum.SQL_Select_Distinct);
            String firstLine = "";
            String secondLine = "CREATE PROCEDURE [dbo].[sp_select_distinct_and_active_" + name + "_for_dropdown]\nAS\n";
            String function_text = firstLine + secondLine;
            function_text += "begin \n SELECT DISTINCT \n";
            int count = 0;
            string comma = "";
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
        /// <summary>       
        /// generates a standard Transact-SQL select line for to be used by the stored procedures, such as select by PK or select by FK.
        /// Takes a string table name nad string column name as paramaters
        /// Jonathan Beck       
        /// </summary>
        ///<param name = "tablename" >the name of <see cref="table"/> this <see cref="Column"/> belongs to </param>
        ///<param name="column_name">the name of this <see cref="Column"/></param>
        /// <returns>  a standard Transact-SQL select line to be used by the stored procedures, </returns>
        private string gen_Select_Line(string tablename, string column_name)
        {
            return "\n[" + tablename + "]." + column_name + " as \'" + tablename + "_" + column_name.bracketStrip() + "\'";
        }
        /// <summary>       
        /// generates a string comment box followed by a Transact-SQL insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields.. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by Transact-SQL insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields. </returns>
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
                foreach (Column r in columns)
                {
                    if (r.start == 0)
                    {
                        result = result + comma + "' '";
                        comma = ",";
                    }
                }
                result += ")";
            }
            result += "\n;\n";
            result += "*******************************/\n";
            return result;
        }

        public string gen_count()
        {
            throw new NotImplementedException();
        }
    }
}
