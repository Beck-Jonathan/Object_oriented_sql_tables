using appData2;
using System;
using System.Collections.Generic;



namespace Data_Objects
{
    public class table
    {
        //various components of a table
        public String name { set; get; }
        public header Header { set; get; }
        public List<Row> rows { set; get; }
        public List<String> primary_keys { set; get; }
        public List<String> foreign_keys { set; get; }
        public table(String name, List<Row> rows)
        {
            if (settings.TSQLMode) {
                this.name = "[dbo].[" + name + "]";
            }
            else
            {
                this.name = name;
            }
            this.rows = rows;

        }


        public String gen_primary_keys()
        {
            //generate the primary keys based on key_gen that was done in the rwos
            if (!settings.TSQLMode)
            {
                String key_string = ",CONSTRAINT " + name + "_PK PRIMARY KEY (";
                int count = 0;
                foreach (Row r in rows)
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
                    foreach (Row r in rows)
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
            foreach (Row r in rows)
            {
                foreach (string s in r.foreign_keys)
                {
                    String[] chunks = s.Split('.');
                    String second_table = chunks[0];
                    String formatted_key = ",CONSTRAINT [fk_" + name + "_" + second_table + "] foreign key ([" +chunks[1]+"]) references ["+ chunks[0] + "]([" + chunks[1] +"])"+ "\n";

                    foreign_keys.Add(formatted_key);
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
        public String gen_rows()
        {

            int count = 0;
            String x = this.gen_header();
            x = x + "\n";
            foreach (Row r in rows)
            {
                String rowtext = r.row_and_key_gen();
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
            foreach (Row r in rows)
            {
                String rowtext = r.audit_row_gen();
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
            String x = " ";
            String comment_text = comment_box_gen.comment_box(name, 3);
            String function_text =
                 "DROP PROCEDURE IF EXISTS sp_update_" + name + ";\n"
                + "DELIMITER $$\n"
                + "CREATE PROCEDURE sp_update_" + name + "\n"
                + "(";
            int count = 0;
            String comma = "";
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                String add = comma + "in " + r.row_name + "_param " + r.data_type + r.length_text + "\n";
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
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                if (!r.primary_key.Equals('y') && !r.primary_key.Equals('Y'))
                {
                    String add = comma + r.row_name + " = " + r.row_name + "_param\n";
                    function_text = function_text + add;
                    count++;
                }
            }
            int keys_count = 0;
            String initial_word = "WHERE ";
            foreach (Row r in rows)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    String add = initial_word + r.row_name + "=" + r.row_name + "_param\n";
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

            String full_text = comment_text + function_text;
            return full_text;

        }
        // to generate the SP_delete
        public String gen_delete()
        {

            String comment_text = comment_box_gen.comment_box(name, 4);
            String function_text =
                 "DROP PROCEDURE IF EXISTS sp_delete_" + name + ";\n"
                + "DELIMITER $$\n"
                + "CREATE PROCEDURE sp_delete_" + name + "\n"
                + "(";
            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = comma + r.row_name + "_param " + r.data_type + r.length_text + "\n";
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
            foreach (Row r in rows)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    String add = initial_word + r.row_name + "=" + r.row_name + "_param\n";
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

            String full_text = comment_text + function_text;
            return full_text;




        }
        // to generate the SP_retreive using a primary key
        public String gen_retreive_by_key()
        {

            String comment_text = comment_box_gen.comment_box(name, 5);
            String function_text =
                 "DROP PROCEDURE IF EXISTS sp_retreive_by_pk_" + name + ";\n"
                + "DELIMITER $$\n"
                + "CREATE PROCEDURE sp_retreive_by_pk_" + name + "\n"
                + "(\n";

            int count = 0;
            String comma = "";
            comma = "";
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    String add = comma + r.row_name + "_param " + r.data_type + r.length_text + "\n";
                    function_text = function_text + add;
                    count++;
                }
            }
            function_text = function_text + ")";

            count = 0;
            comma = "";
            function_text = function_text + "\n Begin \n select \n";
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.row_name + " \n";
                count++;
            }
            function_text = function_text + "\n FROM " + name + "\n";
            String initial_word = "where ";
            int keys_count = 0;
            foreach (Row r in rows)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (keys_count > 0) { initial_word = "AND "; }
                    String add = initial_word + r.row_name + "=" + r.row_name + "_param\n";
                    function_text = function_text + add;
                    keys_count++;
                }
            }

            function_text = function_text + " ; END $$\n" +
               " DELIMITER ;\n";



            String full_text = comment_text + function_text;
            return full_text;
        }

        // to generate the SP_retrive, showing all data in a table
        public String gen_retreive_by_all()
        {
            String gx = " ";
            String comment_text = comment_box_gen.comment_box(name, 6);
            String function_text =
                 "DROP PROCEDURE IF EXISTS sp_retreive_by_all_" + name + ";\n"
                + "DELIMITER $$\n"
                + "CREATE PROCEDURE sp_retreive_by_all_" + name + "()\n";

            int count = 0;
            String comma = "";
            comma = "";
            count = 0;
            function_text = function_text + "begin \n SELECT \n";
            count = 0;
            comma = "";

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + "\n" + comma + r.row_name;
                count++;
            }
            function_text = function_text + "\n FROM " + name + "\n ;\n END $$ \n DELIMITER ;\n";


            String full_text = comment_text + function_text;
            return full_text;
        }
        // to generate the SP_insert
        public string gen_insert()
        {
            String comment_text = comment_box_gen.comment_box(name, 7);
            String function_text =
                 "DROP PROCEDURE IF EXISTS sp_insert_" + name + ";\n"
                + "DELIMITER $$\n"
                + "CREATE PROCEDURE sp_insert_" + name + "(\n";

            int count = 0;
            String comma = "";
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                String add = comma + "in " + r.row_name + "_param " + r.data_type  + r.length_text + "\n";
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
                "INSERT INTO  " + name + "\n values \n("
                ;
            count = 0;
            comma = "";
            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                String add = comma + r.row_name + "_param" + "\n";
                function_text = function_text + add;
                count++;
            }

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

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.row_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "new." + r.row_name + " \n";
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

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.row_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "new." + r.row_name + " \n";
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

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + r.row_name + " \n";
                count++;
            }
            function_text = function_text + "\n, action_type"
                + "\n, action_date"
                 + "\n, action_user"
                + "\n) values(\n";
            count = 0;
            comma = "";

            foreach (Row r in rows)
            {
                if (count > 0) { comma = ","; }
                function_text = function_text + comma + "old." + r.row_name + " \n";
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

        public String gen_functions()
        {
            string x = " ";

            return x;


        }

    }
}


