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
       



        public String gen_primary_keys()
        {
            //generate the primary keys based on key_gen that was done in the rwos
            if (!settings.TSQLMode)
            {
                String key_string = ",CONSTRAINT " + name + "_PK PRIMARY KEY (";
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
                key_string = key_string + ")\n";

                return key_string;
            }
            else
            {
                {
                    name = name.Replace("[dbo].[", "");
                    name = name.Replace("]", "");
                    String key_string = ",CONSTRAINT [PK_" + name + "] PRIMARY KEY (";
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
                    key_string = key_string + ")\n";

                    return key_string;
                }
            }
        }
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
                        if (settings.TSQLMode)
                        {
                            String[] chunks = s.Split('.');
                            String second_table = chunks[0];
                            String formatted_key = ",CONSTRAINT [fk_" + name + "_" + second_table + count + "] foreign key ([" + chunks[1] + "]) references [" + chunks[0] + "]([" + chunks[1] + "])" + "\n";

                            foreign_keys.Add(formatted_key);
                        }
                        else
                        {
                            String[] chunks = s.Split('.');
                            String second_table = chunks[0];
                            String formatted_key = ",CONSTRAINT fk_" + name + "_" + second_table + count + " foreign key (" + chunks[1] + ") references " + chunks[0] + " (" + chunks[1] + ")" + "\n";

                            foreign_keys.Add(formatted_key);

                        }
                    }
                    count++;
                }
            };



            foreach (string tuv in foreign_keys)
            {
                String s = tuv;
                output_keys = output_keys + s;
            }
           
            
            
                output_keys = output_keys + ");\n";
            
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

                            String formatted_key = ",CONSTRAINT [AK_" + r.column_name.bracketStrip() + "] UNIQUE(" + r.column_name + ")\n";

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

        public String gen_header()
        {
            Header.table_name = name;
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
            x = x + "\n";
            foreach (Column r in columns)
            {
                String rowtext = r.column_and_key_gen();
                if (count > 0) { x = x + ","; }
                x = x + rowtext;
                count++;
            }
            ;


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
                function_text = "CREATE PROCEDURE [DBO].[sp_update_" + name + "]\n(";
                count = 0;
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }

                    String add = comma + "@old" + r.column_name.bracketStrip() + r.data_type + r.length_text + "\n";
                    if (r.primary_key != 'y' || r.primary_key != 'Y')
                    {
                        add = add + ",@new" + r.column_name.bracketStrip() + r.data_type + r.length_text + "\n";
                    }
                    function_text = function_text + add;
                    count++;
                }
                comma = "";
                function_text = function_text + ")\nas\nBEGIN\nUPDATE " + name + "\nSET\n";
                count = 0;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (r.primary_key != 'Y' && r.primary_key != 'y')
                    {
                        function_text = function_text + comma + r.column_name.bracketStrip() + " = @new" + r.column_name.bracketStrip() + "\n";
                        count++;
                    }


                }
                function_text = function_text + "WHERE\n";
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    function_text = function_text + comma + r.column_name.bracketStrip() + " = @old" + r.column_name.bracketStrip() + "\n";
                    count++;

                }



                function_text = function_text + "return @@rowcount\nend\ngo\n";
                full_text = comment_text + function_text;

            }
            else
            {
                x = " ";

                string firstLine = "DROP PROCEDURE IF EXISTS sp_update_" + name + ";\n DELIMITER $$\n";
                if (settings.TSQLMode)
                {
                    firstLine = "";
                }
                string secondLine = "CREATE PROCEDURE sp_update_" + name + "\n"
                    + "(";

                String function_text =
                     firstLine + secondLine; foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }

                    String add = comma + "old" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "\n";
                    if (r.primary_key != 'y' || r.primary_key != 'Y')
                    {
                        add = add + ",new" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "\n";
                    }
                    function_text = function_text + add;
                    count++;
                }


                function_text = function_text + ")\n" +
                    "begin \n" +
                    "declare sql_error TINYINT DEFAULT FALSE;\n" +
                    "declare update_count tinyint default 0;\n" +
                    "DECLARE CONTINUE HANDLER FOR SQLEXCEPTION\n" +
                    "SET sql_error = true;\n" +
                    "START TRANSACTION;\n" +
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
                        function_text = function_text + add;
                        count++;
                    }
                }
                int keys_count = 0;
                String initial_word = "WHERE ";
                foreach (Column r in columns)
                {
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        if (keys_count > 0) { initial_word = "AND "; }
                        String add = initial_word + r.column_name + "= old" + r.column_name + "\n";
                        function_text = function_text + add;
                        keys_count++;
                    }
                }
                function_text = function_text + "\n" +
                   " ; if sql_error = FALSE then \n" +
                   " SET update_count = row_count(); \n" +
                   " COMMIT;\n" +
                   " ELSE\n" +
                   " SET update_count = 0;\n" +
                   " ROLLBACK;\n" +
                   " END IF;\n" +
                   " select update_count as 'update count'\n" +
                   " ; END $$\n" +
                   " DELIMITER ;\n";

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
                function_text = "create procedure [dbo].[sp_delete_" + name + "]\n(\n";
                int count = 0;
                String comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + "@" + r.column_name.bracketStrip() + " " + r.data_type + " " + r.length_text + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                function_text = function_text + ")\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text = function_text + "set active = 0\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }

                function_text = function_text + "return @@rowcount \n end \n go\n";


            }



            else
            {
                function_text =
                     "DROP PROCEDURE IF EXISTS sp_delete_" + name + ";\n"
                    + "DELIMITER $$\n"
                    + "CREATE PROCEDURE sp_delete_" + name + "\n"
                    + "(";
                int count = 0;
                String comma = "";
                comma = "";
                count = 0;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        String add = comma + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                count = 0;
                function_text = function_text + ")\n" +
                    "begin \n" +
                    "declare sql_error TINYINT DEFAULT FALSE;\n" +
                    "declare update_count tinyint default 0;\n" +
                    "DECLARE CONTINUE HANDLER FOR SQLEXCEPTION\n" +
                    "SET sql_error = true;\n" +
                    "START TRANSACTION;\n" +
                    "DELETE FROM " + name + "\n  "
                    ;
                comma = "";
                int keys_count = 0;
                String initial_word = "WHERE ";
                foreach (Column r in columns)
                {
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        if (keys_count > 0) { initial_word = "AND "; }
                        String add = initial_word + r.column_name + "=" + r.column_name + "_param\n";
                        function_text = function_text + add;
                        keys_count++;
                    }
                }
                function_text = function_text + "\n" +
                   " ; if sql_error = FALSE then \n" +
                   " SET update_count = row_count(); \n" +
                   " COMMIT;\n" +
                   " ELSE\n" +
                   " SET update_count = 0;\n" +
                   " ROLLBACK;\n" +
                   " END IF;\n" +
                   " select update_count as 'update count'\n" +
                   " ; END $$\n" +
                   " DELIMITER ;\n";
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
                function_text = "create procedure [dbo].[sp_undelete_" + name + "]\n(\n";
                int count = 0;
                String comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + "@" + r.column_name.bracketStrip() + " " + r.data_type + " " + r.length_text + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                function_text = function_text + ")\nas\nbegin\n";
                function_text = function_text + "update [" + name + "]\n";
                function_text = function_text + "set active = 1\n";
                count = 0;
                comma = "where ";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = "and "; }
                    if (true)
                    {
                        String add = comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip() + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }

                function_text = function_text + "return @@rowcount \n end \n go\n";


            }



            else
            {
                function_text =
                     "DROP PROCEDURE IF EXISTS sp_undelete_" + name + ";\n"
                    + "DELIMITER $$\n"
                    + "CREATE PROCEDURE sp_undelete_" + name + "\n"
                    + "(";
                int count = 0;
                String comma = "";
                comma = "";
                count = 0;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                count = 0;
                function_text = function_text + ")\n" +
                    "begin \n" +
                    "declare sql_error TINYINT DEFAULT FALSE;\n" +
                    "declare update_count tinyint default 0;\n" +
                    "DECLARE CONTINUE HANDLER FOR SQLEXCEPTION\n" +
                    "SET sql_error = true;\n" +
                    "START TRANSACTION;\n" +
                    "DELETE FROM " + name + "\n  "
                    ;
                comma = "";
                int keys_count = 0;
                String initial_word = "WHERE ";
                foreach (Column r in columns)
                {
                    if (true)
                    {
                        if (keys_count > 0) { initial_word = "AND "; }
                        String add = initial_word + r.column_name + "=" + r.column_name + "_param\n";
                        function_text = function_text + add;
                        keys_count++;
                    }
                }
                function_text = function_text + "\n" +
                   " ; if sql_error = FALSE then \n" +
                   " SET update_count = row_count(); \n" +
                   " COMMIT;\n" +
                   " ELSE\n" +
                   " SET update_count = 0;\n" +
                   " ROLLBACK;\n" +
                   " END IF;\n" +
                   " select update_count as 'update count'\n" +
                   " ; END $$\n" +
                   " DELIMITER ;\n";
            }

            String full_text = comment_text + function_text;
            return full_text;




        }
        // to generate the SP_retreive using a primary key
        public String gen_retreive_by_key()
        {

            String comment_text = comment_box_gen.comment_box(name, 5);
            String firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_by_pk_" + name + ";\n"
                + "DELIMITER $$\n";
            String secondLine = "CREATE PROCEDURE sp_retreive_by_pk_" + name + "\n"
                + "(\n";
            if (settings.TSQLMode)
            {
                firstLine = "";
                secondLine = "CREATE PROCEDURE [DBO].[sp_retreive_by_pk_" + name + "]\n(";
            }
            String function_text = firstLine + secondLine;
            int count = 0;
            String comma = "";
            comma = "";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = "";
                    if (settings.TSQLMode) { add = comma + r.column_name.Replace("]", "").Replace("[", "@") + " " + r.data_type + r.length_text + "\n"; }
                    else { add = comma + r.column_name + " " + r.data_type + r.length_text + "\n"; }
                    function_text = function_text + add;
                    count++;
                }
            }
            function_text = function_text + ")";

            count = 0;
            comma = "";
            String asString = "";
            if (settings.TSQLMode) { asString = "\nas"; }
            function_text = function_text + asString + "\n Begin \n select \n";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n FROM " + name + "\n";
            String initial_word = "where ";
            int keys_count = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    string add = "";
                    if (settings.TSQLMode) { add = initial_word + r.column_name + "=" + r.column_name.Replace("]", "").Replace("[", "@") + " \n"; }
                    else
                    {
                        add = initial_word + r.column_name + "=" + r.column_name + "\n";
                    }
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            if (settings.TSQLMode)
            {
                function_text = function_text + " END \n" +
                       " GO\n";
            }
            if (!settings.TSQLMode)
            {
                function_text = function_text + " ; END $$\n" +
                   " DELIMITER ;\n";
            }



            String full_text = comment_text + function_text;
            return full_text;
        }

        //to generate retreive by fk, not implmented well yet
        public String gen_retreive_by_fkey(foreignKey key)
        {
            String comma = "";
            int count = 0;
            String comment_text = comment_box_gen.comment_box(name, 200);
            String firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_" + key.referenceTable + "by" + key.mainTable + ";\n"
                + "DELIMITER $$\n";
            String secondLine = "CREATE PROCEDURE sp_retreive_" + key.referenceTable + "by" + key.mainTable + "_ID;\n"
                + "(\n";

            if (settings.TSQLMode)
            {
                firstLine = "";
                secondLine = "CREATE PROCEDURE [DBO].[sp_retreive_" + key.referenceTable + "by" + key.mainTable + "_ID]\n(";
            }
            String function_text = firstLine + secondLine;


            String add = "";
            if (settings.TSQLMode) { add = "@" + key.fieldName.bracketStrip() + " " + key.dataType + key.lengthText + "\n"; }
            else { add = key.fieldName + " " + key.dataType + key.lengthText + "\n"; }
            function_text = function_text + add;



            function_text = function_text + ")";


            String asString = "";
            if (settings.TSQLMode) { asString = "\nas"; }
            function_text = function_text + asString + "\n Begin \n select \n";
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.column_name + " \n";
                count++;
            }
            function_text = function_text + "\n FROM " + name + "\n";
            String initial_word = "where ";
            int keys_count = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    add = "";
                    if (settings.TSQLMode) { add = initial_word + r.column_name + "=" + r.column_name.Replace("]", "").Replace("[", "@") + " \n"; }
                    else
                    {
                        add = initial_word + r.column_name + "=" + r.column_name + "\n";
                    }
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            if (settings.TSQLMode)
            {
                function_text = function_text + " END \n" +
                       " GO\n";
            }
            if (!settings.TSQLMode)
            {
                function_text = function_text + " ; END $$\n" +
                   " DELIMITER ;\n";
            }



            String full_text = comment_text + function_text;
            return full_text;
        }

        // to generate the SP_retrive, showing all data in a table
        public String gen_retreive_by_all()
        {
            String gx = " ";
            String comment_text = comment_box_gen.comment_box(name, 6);
            string firstLine = "DROP PROCEDURE IF EXISTS sp_retreive_by_all_" + name + ";\n"
                + "DELIMITER $$\n";
            string secondLine = "CREATE PROCEDURE sp_retreive_by_all_" + name + "()\n";
            if (settings.TSQLMode)
            {
                firstLine = "";
                secondLine = "CREATE PROCEDURE [DBO].[sp_retreive_by_all_" + name + "]\nAS\n";
            }
            String function_text = firstLine + secondLine;



            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            function_text = function_text + "begin \n SELECT \n";
            count = 0;
            comma = "";

            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + "\n" + comma + r.column_name;
                count++;
            }
            if (settings.TSQLMode) { function_text = function_text + "\n FROM " + name + "\n ;\n END  \n GO\n"; }
            else { function_text = function_text + "\n FROM " + name + "\n ;\n END $$ \n DELIMITER ;\n"; }



            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the SP_insert
        public string gen_insert()
        {
            String comment_text = comment_box_gen.comment_box(name, 7);
            String firstLine =
                 "DROP PROCEDURE IF EXISTS sp_insert_" + name + ";\n"
                + "DELIMITER $$\n";
            String secondLine = "CREATE PROCEDURE sp_insert_" + name + "(\n";

            if (settings.TSQLMode)
            {
                firstLine = "";
                secondLine = "CREATE PROCEDURE [DBO].[sp_insert" + name + "]\n(";
            }
            String function_text = firstLine + secondLine;
            int count = 0;
            String comma = "";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (count > 0) { comma = ","; }
                    string add = "";
                    if (settings.TSQLMode) { add = comma + "@" + r.column_name.bracketStrip() + " " + r.data_type + r.length_text + "\n"; }
                    else
                    {
                        add = comma + "in " + r.column_name + "_param " + r.data_type + r.length_text + "\n";
                    }
                    function_text = function_text + add;
                    count++;
                }
            }
            if (settings.TSQLMode)
            {
                function_text = function_text + ")as \n begin\n insert into [dbo]." + name + "(\n";
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
            else
            {
                function_text = function_text + ")\n" +
                "begin \n" +

                "INSERT INTO  " + name + "\n(";
                comma = "";
                foreach (Column r in columns)
                {
                    if (r.increment == 0)
                    {
                        function_text = function_text + comma + r.column_name;
                        comma = ",";
                    }
                }

                function_text = function_text + ")\n values \n(";
                count = 0;
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (r.increment == 0)
                    {
                        String add = comma + r.column_name + "_param\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                function_text = function_text + ")\n";
            }
            count = 0;
            comma = "";


            if (settings.TSQLMode)
            {
                function_text = function_text + ")\n VALUES (\n";
                comma = "";
                count = 0;
                foreach (Column r in columns)
                {
                    if (r.increment == 0)
                    {
                        if (count > 0) { comma = ","; }
                        function_text = function_text + comma + "@" + r.column_name.bracketStrip() + "\n";
                        count++;
                    }

                }
                function_text = function_text + ")\n";
            }
            else
            {



            }
            if (!settings.TSQLMode)
            {
                function_text = function_text +
                   " ; END $$\n" +
                   " DELIMITER ;\n";
            }
            else { function_text = function_text + "return @@rowcount\nend\nGo\n"; }



            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the on update trigger
        public String gen_update_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 8);
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
        // to generate the on insert trigger
        public String gen_insert_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 9);
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
        // to generate the on delete trigger
        public String gen_delete_trigger()
        {
            String comment_text = comment_box_gen.comment_box(name, 10);
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
