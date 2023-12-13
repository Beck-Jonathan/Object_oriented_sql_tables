using appData2;
using System;
using System.Collections.Generic;
using Data_Objects;



namespace Data_Objects
{
    public class table
    {
        //various components of a table
        public String name { set; get; }
        public header Header { set; get; }
        public List<Column> columns { set; get; }
        public List<String> primary_keys { set; get; }
        public List<String> foreign_keys { set; get; }
        public table(String name, List<Column> columns)
        {
            if (settings.TSQLMode) {
                this.name = "[dbo].[" + name + "]";
            }
            else
            {
                this.name = name;
            }
            this.columns = columns;

        }


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
            else {
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
                            String formatted_key = ",CONSTRAINT [fk_" + name + "_" + second_table + "] foreign key ([" + chunks[1] + "]) references [" + chunks[0] + "]([" + chunks[1] + "])" + "\n";

                            foreign_keys.Add(formatted_key);
                        }
                        else {
                            String[] chunks = s.Split('.');
                            String second_table = chunks[0];
                            String formatted_key = ",CONSTRAINT fk_" + name + "_" + second_table + " foreign key (" + chunks[1] + ") references " + chunks[0] + " (" + chunks[1] + ")" + "\n";

                            foreign_keys.Add(formatted_key);

                        }
                    }
                }
            };

            foreach (string tuv in foreign_keys)
            {
                String s = tuv;
                output_keys = output_keys + s;
            }
            if (settings.TSQLMode) { output_keys = output_keys + ")\ngo\n"; }
            else
            {
                output_keys = output_keys + ");\n";
            }
            return output_keys;
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
            if (settings.TSQLMode) {
                string function_text = "";
                function_text=  "CREATE PROCEDURE [DBO].[sp_update_" + name + "]\n(";
                count = 0;
                comma = "";
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    
                    String add = comma + "@old" + r.column_name.bracketStrip()  + r.data_type + r.length_text + "\n";
                    if (r.primary_key != 'y' || r.primary_key != 'Y')
                    {
                        add = add + ",@new" + r.column_name.bracketStrip() +  r.data_type + r.length_text + "\n";
                    }
                    function_text = function_text + add;
                    count++;
                }
                comma = "";
                function_text = function_text + ")\nas\nBEGIN\nUPDATE " + name +"\nSET\n";
                count = 0;
                foreach (Column r in columns) {
                    if (count > 0) { comma = ","; }
                    if (r.primary_key != 'Y' && r.primary_key != 'y')
                    {
                        function_text = function_text + comma + r.column_name.bracketStrip() + " = @new" + r.column_name.bracketStrip() + "\n";
                        count++;
                    }
                
                
                }
                function_text = function_text + "WHERE\n";
                comma = "";
                foreach (Column r in columns) {
                    if (count > 0) { comma = "and "; }
                    function_text=function_text+comma+r.column_name.bracketStrip() + " = @old" + r.column_name.bracketStrip() + "\n";
                    count++;

                }



                function_text = function_text + "return @@rowcount\nend\ngo\n";
                full_text = comment_text + function_text;

            }
            else { x = " ";
                
                string firstLine = "DROP PROCEDURE IF EXISTS sp_update_" + name + ";\n DELIMITER $$\n";
                if (settings.TSQLMode) {
                    firstLine = "";
                }
                string secondLine = "CREATE PROCEDURE sp_update_" + name + "\n"
                    + "(";
                
                String function_text =
                     firstLine + secondLine;

                
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
                        String add = comma + r.column_name + " = " + r.column_name + "_param\n";
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
            { function_text = "create procedure [dbo].[sp_delete_" + name + "]\n(\n";
                int count = 0;
                String comma = "" ;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    if (true)
                    {
                        String add = comma + "@"+r.column_name.bracketStrip() + " " + r.data_type +" "+ r.length_text + "\n";
                        function_text = function_text + add;
                        count++;
                    }
                }
                    function_text = function_text + ")\nas\nbegin\n";
                    function_text = function_text + "update [" + name+"]\n";
                    function_text = function_text + "set active = 0\n";
                count = 0;
                comma = "where ";
                    foreach (Column r in columns)
                    {
                        if (count > 0) { comma = "and "; }
                        if (true)
                        {
                            String add = comma + "@" + r.column_name.bracketStrip() + " =" + r.column_name.bracketStrip()+ "\n";
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
            if (settings.TSQLMode) {
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
                    if (settings.TSQLMode) { add = comma + r.column_name.Replace("]","").Replace("[","@") + " " + r.data_type + r.length_text + "\n"; }
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
            function_text = function_text + asString+"\n Begin \n select \n";
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
                    if (settings.TSQLMode) { add = initial_word + r.column_name + "=" + r.column_name.Replace("]", "").Replace("[", "@")+ " \n"; }
                    else
                    {
                        add = initial_word + r.column_name + "=" + r.column_name + "\n";
                    }
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            if (settings.TSQLMode) { 
            function_text=function_text + " END \n" +
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
            if (settings.TSQLMode) { firstLine = "";
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
            if (settings.TSQLMode) {
                function_text = function_text + ")as \n begin\n insert into [dbo]." + name + "(\n";
            }
            else
            {
                function_text = function_text + ")\n" +
                "begin \n" +
                "declare sql_error TINYINT DEFAULT FALSE;\n" +
                "declare update_count tinyint default 0;\n" +
                "DECLARE CONTINUE HANDLER FOR SQLEXCEPTION\n" +
                "SET sql_error = true;\n" +
                "START TRANSACTION;\n" +
                "INSERT INTO  " + name + "\n values \n("
                ;
            }
            count = 0;
            comma = "";
            
            foreach (Column r in columns)
            {
                if (count > 0) { comma = ","; }
                String add = comma +   r.column_name   + "\n";
                function_text = function_text + add;
                count++;
            }
            if (settings.TSQLMode)
            {
                function_text = function_text + ")\n VALUES (\n";
                comma = "";
                count = 0;
                foreach (Column r in columns)
                {
                    if (count > 0) { comma = ","; }
                    function_text = function_text + comma + "@" + r.column_name.bracketStrip()+"\n";
                    count++;

                }
                function_text = function_text + ")\n";
            }
            if (!settings.TSQLMode)
            {
                function_text = function_text + ")\n" +
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
            else {function_text = function_text + "return @@rowcount\nend\nGo\n"; }



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

        public String gen_IThingAccessor()
        {
            int count = 0;
            string comma = "";
            string output = "";
            string comment = comment_box_gen.comment_box(name, 11);
            string header = "public interface I"+name+"Accessor \n{\n";

            string addThing = "int add "+name+"(" + name + " _" + name + ");\n";

            

            string selectThingbyPK = name+ " select" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string selectallThing = "List<"+name+"> selectAll"+name+"();\n";



            comma = "";
            count = 0;
            string updateThing = "int update" + name +"(";

            updateThing = updateThing + name + "_old" + name + " , " + name + " _new" + name;
            updateThing = updateThing + ");\n";

           
            string deleteThing = "int delete"+name+ "(" + name + " _" + name + ");\n";
            string undeleteThing = "int delete" + name + "(" + name + " _" + name + ");\n";
            output =comment+ header + addThing + selectThingbyPK + selectallThing + updateThing + deleteThing + undeleteThing+"}\n\n";



            return output;

        }

        public String gen_ThingAccessor() {

            //nneds proper number and code choice
            string comment = comment_box_gen.comment_box(name, 13);
            //good
            string header = genAccessorClassHeader();
            //good
            string addThing = genAccessorAdd();
            //good
            string selectThingbyPK = genAccessorRetreiveByKey();
            //needs implemented
            string selectallThing = genAccessorRetreiveAll();
            //good
            string updateThing = genAccessorUpdate(); 
            //good
            string deleteThing = genAccessorDelete();
            //good
            string undeleteThing = genAccessorUndelete();
            string output = comment + header + addThing + selectThingbyPK + selectallThing + updateThing + deleteThing + "}\n\n";
            //good


            return output;


        }
        public String gen_IThingManager()
        {
            int count = 0;
            string comma = "";
            string output = "";
            string comment = comment_box_gen.comment_box(name, 12);
            string header = "public interface I" + name + "Manager \n{\n";

            string addThing = "int add" + name + "(" +name +" _"+name+");\n";
            
            

            string getThingbyPK = name + " get" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string getallThing = "List<" + name + "> getAll" + name + "();\n";



            comma = "";
            count = 0;
            string editThing = "int edit" + name + "(";
            editThing  = editThing + name + " _old"+name+" , " +name +" _new" + name;
            editThing = editThing + ");\n";
            string purgeThing = "int purge" + name + "(string " + name + "ID);\n";
            string unPurgeThing = "int unpurge" + name + "(string " + name + "ID);\n";
            output = comment + header + addThing + getThingbyPK + getallThing + editThing + purgeThing + unPurgeThing+ "}\n\n";



            return output;

        }


        public String gen_DataObject() {

            string output = "";
            output = comment_box_gen.comment_box(name,15);
            output = "public class " + name+"\n{\n";
            int count = 0;


            foreach (Column r in columns)
            {
                String add = "public " + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip()+ "{ set; get; }\n";
                output = output + add;
                count++;
            }
            output = output + "\n}\n";

            if (foreign_keys.Count > 0)
            {
                output = output + "public class" + name + "VM: " + name + "\n{\n";
                foreach (Column r in columns)
                {

                    if (r.foreign_key == "y" || r.foreign_key == "Y")
                    {
                        int dotPosition = r.references.IndexOf('.');
                        string keytype = r.references.Substring(0, dotPosition);
                        output = output + "public " + keytype + " _" + keytype.ToLower() + "{ get ; set; }\n";

                    }

                }
                output = output + "}\n";
            }




            return output;
        
        
        }
        public String gen_functions()
        {
            string x = " ";

            return x;


        }
        private string genAccessorClassHeader() {
            string header = "";
            int count = 0;
            string comma = "";
            string output = "";
            string comment = comment_box_gen.comment_box(name, 11);
            header = "public class " + name + "Accessor : I" + name + "Accessor {\n";


            return header;
        
        
        }
        private String genSPHeaderA(string commandText) {
            //for update, insert, delete
            String output = "";
            output= "int rows = 0;\n"
            +"// start with a connection object\n"
            +"var conn = SqlConnectionProvider.GetConnection();\n"
            +"// set the command text\n"
            +"var commandText = \""+commandText+"\";\n"
            + "// create the command object\n"
            + "var cmd = new SqlCommand(commandText, conn);\n"
            + "// set the command type\n"
            + "cmd.CommandType = CommandType.StoredProcedure;\n"
            + "// we need to add parameters to the command\n";


            return output;        
        
        }
        private String genSPHeaderB(string DataObject, string commandText) {
            //for single data object
            string output = "";
            output = DataObject+" output = new "+DataObject+"();\n"
            + "// start with a connection object\n"
            + "var conn = SqlConnectionProvider.GetConnection();\n"
            + "// set the command text\n"
            + "var commandText = \"" + commandText + "\";\n"
            + "// create the command object\n"
            + "var cmd = new SqlCommand(commandText, conn);\n"
            + "// set the command type\n"
            + "cmd.CommandType = CommandType.StoredProcedure;\n"
            + "// we need to add parameters to the command\n";


            return output;
        }
        private String genSPHeaderC(string DataObject, string commandText)
        {
            //for list of data object
            string output = "";
            output ="List<"+ DataObject + "> output = new " +"List<"+ DataObject + ">();\n"
            + "// start with a connection object\n"
            + "var conn = SqlConnectionProvider.GetConnection();\n"
            + "// set the command text\n"
            + "var commandText = \"" + commandText + "\";\n"
            + "// create the command object\n"
            + "var cmd = new SqlCommand(commandText, conn);\n"
            + "// set the command type\n"
            + "cmd.CommandType = CommandType.StoredProcedure;\n"
            + "// There are no parameters to set or add\n";
            return output;
        }
        private string genSPfooter(int mode) {
                     
            string returntype = "output";
            if (mode == 2) { returntype = "rows"; }
            string output = "";
            output = " \ncatch (Exception ex)\n"
            + "{\n"
            + "    throw ex;\n"
            + "}\n"
            + "finally\n"
            + "{\n"
            + "    conn.Close();\n"
            + "}\n"
            +"return "+ returntype+";\n}\n";
            return output;
        }
        private string genAccessorAdd() {
            string createThing = "";
            int count = 0;
            string comma = "";
            createThing = "public int add" + name + "("+name+" _"+name.ToLower();
           
            createThing = createThing + "){\n";
            createThing = createThing + genSPHeaderA("sp_insert_" + name);
            //add parameters
            foreach (Column r in columns) {
                createThing = createThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            createThing = createThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns) {
                createThing = createThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " +"_"+name.ToLower()+"."+ r.column_name.bracketStrip()+";\n";
            }
            //excute the quuery
            createThing = createThing + "try \n { \n //open the connection \n conn.Open();  ";
            createThing = createThing + "//execute the command and capture result\n";
            createThing = createThing + "rows = cmd.ExecuteNonQuery();\n}\n";
            //capture reuslts
            createThing = createThing + "";

            //cath block and onwards
            createThing = createThing + genSPfooter(2);




            return createThing;
        }

        private string genAccessorRetreiveByKey() {
            string retreiveThing = "";
            int count = 0;
            string comma = "";
            retreiveThing = "public "+name +" select" + name + "ByPrimaryKey(";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (count > 0) { comma = "  "; }
                    String add = comma + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip();
                    retreiveThing = retreiveThing + add;
                    count++;
                }
            }
            retreiveThing = retreiveThing + "){\n";
            retreiveThing = retreiveThing + genSPHeaderB(name, "sp_retreive_by_pk_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retreiveThing = retreiveThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
                }
            //setting parameters
            retreiveThing = retreiveThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retreiveThing = retreiveThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            retreiveThing = retreiveThing + "try \n { \n //open the connection \n conn.Open();  ";
            retreiveThing = retreiveThing + "//execute the command and capture result\n";

            retreiveThing = retreiveThing + "var reader = cmd.ExecuteReader();\n";
            
            //capture reuslts
            retreiveThing = retreiveThing + "//process the results\n";
            retreiveThing = retreiveThing + "if (reader.HasRows)\n if (reader.Read())\n{";
            count = 0;
            foreach (Column r in columns) { 
            retreiveThing = retreiveThing + "output."+r.column_name.bracketStrip()+" = reader.Get"+r.data_type.toSqlReaderDataType()+"("+count + ");\n";
                count++;
            
            }
            retreiveThing = retreiveThing + "\n}\n";
            retreiveThing = retreiveThing + "else \n { throw new ArgumentException(\"" +name+ " not found\");\n}\n}";

            //cath block and onwards
            retreiveThing = retreiveThing + genSPfooter(0);




            return retreiveThing;


        }

        private string genAccessorRetreiveAll() {
            
            int count = 0;
            string comma = "";
            string retreiveAllThing = "";
             retreiveAllThing = "public List<" + name + "> selectAll" + name + "(){\n";
            
            
            retreiveAllThing = retreiveAllThing + genSPHeaderC(name, "sp_retreive_by_all_" + name);
            //no paramaters to set or add
            
            
           
            //excute the quuery
            retreiveAllThing = retreiveAllThing + "try \n { \n //open the connection \n conn.Open();  ";
            retreiveAllThing = retreiveAllThing + "//execute the command and capture result\n";

            retreiveAllThing = retreiveAllThing + "var reader = cmd.ExecuteReader();\n";

            //capture reuslts
            retreiveAllThing = retreiveAllThing + "//process the results\n";
            retreiveAllThing = retreiveAllThing + "if (reader.HasRows)\n while (reader.Read())\n{";
            retreiveAllThing = retreiveAllThing + "var _" + name + "= new "+ name+"();\n";
            count = 0;
            foreach (Column r in columns)
            {
                retreiveAllThing = retreiveAllThing + "_"+name+"." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(" + count + ");\n";
                count++;

            }
            retreiveAllThing = retreiveAllThing + "output.Add(_" + name + ");";
            retreiveAllThing = retreiveAllThing + "\n}\n}";

            //cath block and onwards
            retreiveAllThing = retreiveAllThing + genSPfooter(0);


            return retreiveAllThing;
            
        
        }

        private string genAccessorUpdate() {
            string updateThing = "";
            int count = 0;
            string comma = "";
            updateThing = "public int update" + name + "(";
            

            updateThing = updateThing + name + " _old" + name + " , " + name + " _new" + name;
            updateThing = updateThing + ");\n";
            
            updateThing = updateThing + genSPHeaderA("sp_update_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters.Add(\"@old" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";

                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    updateThing = updateThing + "cmd.Parameters.Add(\"@new" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            updateThing = updateThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters[\"@old" + r.column_name.bracketStrip() + "\"].Value = _old"+name+"." + r.column_name.bracketStrip() + ";\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    updateThing = updateThing + "cmd.Parameters[\"@new" + r.column_name.bracketStrip() + "\"].Value = _new" + name + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            updateThing = updateThing + "try \n { \n //open the connection \n conn.Open();  ";
            updateThing = updateThing + "//execute the command and capture result\n";
            updateThing = updateThing + "rows = cmd.ExecuteNonQuery();\n";
            updateThing = updateThing + "if (rows == 0) {\n //treat failed update as exception \n ";
            updateThing = updateThing + "throw new ArgumentException(\"invalid values, update failed\");\n}\n}";
            //capture reuslts
            updateThing = updateThing + "";

            //cath block and onwards
            updateThing = updateThing + genSPfooter(2);




            return updateThing;

        }

        private string genAccessorDelete() {
            string deleteThing = "public int delete" + name + "(" + name + " _" + name.ToLower() + "){\n";
            deleteThing = deleteThing + genSPHeaderA("sp_delete_" + name);
            //add parameters bit
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            deleteThing = deleteThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing = deleteThing + "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing = deleteThing + "if (rows == 0){\n";
            deleteThing = deleteThing + "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing = deleteThing + genSPfooter(2);
            return deleteThing;
            }

        private string genAccessorUndelete()
        {
            string deleteThing = "public int undelete" + name + "("+name+" _"+name.ToLower()+"){\n";
            deleteThing = deleteThing + genSPHeaderA("sp_undelete_" + name);
            //add parameters bit
            //add parameters
            foreach (Column r in columns)
            {
                    deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
                //setting parameters
                deleteThing = deleteThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                    deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing = deleteThing + "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing = deleteThing + "if (rows == 0){\n";
            deleteThing = deleteThing + "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing = deleteThing + genSPfooter(2);
            return deleteThing;
        }

        public string genXAMLWindow() {
            string comment = comment_box_gen.comment_box(name, 16);
            string WindowCode = comment;
            int rows = columns.Count+3;
            int width = 4;
            int height = rows * 50+100;
            WindowCode += "< !--set window height to "+height+"-- >\n";
            WindowCode += "< Menu Grid.Row = \"0\" Padding = \"20px, 0px\" >\n";
            WindowCode += " < MenuItem x: Name = \"mnuFile\" Header = \"File\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuExit\" Header = \"Exit\" Click = \"mnuExit_Click\" />\n";
            WindowCode += "</ MenuItem >\n";
            WindowCode += " < MenuItem x: Name = \"mnuHelp\" Header = \"Help\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuAbout\" Header = \"About\" />\n";
            WindowCode += "</ MenuItem > \n </Menu>";
            WindowCode += "<Grid>\n";
            WindowCode += "<Grid.RowDefinitions>\n";
            for (int i = 0; i < rows; i++) {
                WindowCode += "<RowDefinition Height=\"50\"/>\n";
            }
            WindowCode += "</Grid.RowDefinitions>\n";
            WindowCode += "<Grid.ColumnDefinitions>\n";
            for (int i = 0; i < width; i++)
            {
                WindowCode += "<ColumnDefinition />\n";
            }
            WindowCode += "</Grid.ColumnDefinitions>\n";

            for (int i = 0; i < columns.Count; i++) { 
            WindowCode += "< Label x:Name = \"lbl"+name+columns[i].column_name.bracketStrip()+"\" Grid.Column = \"1\" Grid.Row = \""+(i+1)+"\" Content = \"" +columns[i].column_name+" \" />\n";
                if (columns[i].foreign_key == "y" || columns[i].foreign_key == "Y")
                {
                    WindowCode += "< ComboBox x:Name = \"cbx" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }
                else if (columns[i].column_name.bracketStrip().ToLower() == "active") 
                {
                    WindowCode += "< CheckBox x:Name = \"chk" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }
                else
                {
                    WindowCode += "< TextBox x:Name = \"tbx" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }

            }
            WindowCode += "<Button x:Name=\"btnUpdate" + name + "\" Grid.Column=\"2\" Grid.Row=\""+(rows-1)+"\" Content=\"Edit " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "<Button x:Name=\"btnAdd" + name + "\" Grid.Column=\"3\" Grid.Row=\"" + (rows - 1) + "\" Content=\"Add " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "< StatusBar Grid.Row =" + rows + ">\n";


            WindowCode += "< StatusBarItem x: Name = \"statMessage\" Content = \"Welcome, please login to continue\" Padding = \"20px, 0px\" />\n </ StatusBar >\n </Grid>\n";
            return WindowCode;
        
        
        }

        public string genWindowCSharp() {

            string result = "";
            result=result+ comment_box_gen.comment_box(name, 17);
            result = result + genStaticVariables();
            result = result + genConstructor();
            result = result + genWinLoad();
            result = result + genAddButton();
            result = result + genEditButton();
            return result;






        }

        private string genAddButton() {
            string result = "";
            
            //else if (columns[i].column_name.bracketStrip().ToLower() == "active") 
            // else   //this means textbox

            result = result + "private void btnAdd" + name + "_click(object sender, RoutedEventArgs e)\n";
            result = result + "if((string)btnAdd" + name + ".Content == \"Add " + name + "\")\n";
            foreach (Column c in columns) {
                if (c.foreign_key == "y" || c.foreign_key == "Y")
                {
                    result = result + "cbx" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "cbx" + name + c.column_name.bracketStrip() + ".SelectedItem=null;\n";

                }
                else if (c.column_name.bracketStrip().ToLower() == "active")
                {
                    result = result + "chk" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "chk" + name + c.column_name.bracketStrip() + ".IsChecked=true;\n";

                }
                else
                {
                    result = result + "tbx" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "tbx" + name + c.column_name.bracketStrip() + ".Text=\"\";\n";
                }
            }
                result = result + "else\n{\nif (validInputs())\n{\n";
                
                result = result + name + " new" + name + " = new " + name + "();\n";
                foreach (Column c in columns) {
                if (c.foreign_key == "y" || c.foreign_key == "Y")
                {
                    
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " = " + "cbx" + name + c.column_name.bracketStrip() + ".Text;\n";
                }
                else if (c.column_name.bracketStrip().ToLower() == "active")
                {
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " =  true;\n";
                }
                else
                {
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " = " + "tbx" + name + c.column_name.bracketStrip() + ".Text;\n";
                }                   
                
                }
            result = result + "try\n{\n";
            result = result + "bool result = _" + name.Substring(0, 1).ToLower() + "m.add" + name + "(new" + name + ");\n";
            result = result + "if (result)\n{\n MessageBox.Show(\"added!\");\n";
            result = result + "this.DialogResult=true;\n}\n";
            result = result + "else \n { throw new ApplicationException();\n}\n}\n";
            result = result + "catch (Exception ex)\n{\n MessageBox.Show(\"add failed\");";
            result = result + "this.DialogResult=false\n}\n}";
            result = result + "\nelse{\nMessageBox.Show(\"invalid inputs\");\n}\n}\n}";
            return result;
        
        }

        private string genEditButton()
        {
            string result = "";

            return result;

        }
        private string genValidInputs()
        {
            string result = "";

            return result;

        }

        private string genConstructor()
        {
            string result = "";
            result = "public " + name + "AddEditDelete(" + name +" "+ name.Substring(0, 1).ToLower();
            result = result + ")\n{\n";
            result = result + " InitializeComponent();\n";
            result=result+"_"+name.ToLower() +"="+ name.Substring(0, 1).ToLower()+";\n";
            result = result + "_" + name.Substring(0, 1).ToLower() + "m = new" + name + "Manager();\n}\n";
            

            return result;

        }

        private string genWinLoad()
        {
            string result = "";

            return result;

        }

        private string genStaticVariables()
        {
            string result = "";
            result = "public" + name + " _" + name.ToLower() + "= null;\n";
            result = "public" + name + "Manager+ _" + name.Substring(0,1) + "m = null;\n";
            return result;

        }







    }


    }



