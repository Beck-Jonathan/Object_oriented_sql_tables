using appData2;
using System;
using System.Collections.Generic;

namespace Data_Objects
{
    public class Row
    {
        //various components of the row object
        public String row_name { get; set; }
        public String data_type { get; set; }
        public int length { get; set; }
        public String default_value { get; set; }
        public String identity { get; set; }
        public int start { get; set; }
        public int increment { get; set; }

        public char nullable { get; set; }
        public string index { get; set; }

        public char unique { get; set; }
        public char primary_key { get; set; }
        public String foreign_key { get; set; }
        public String integrity { get; set; }
        public String references { get; set; }
        public String description { get; set; }

        public String row_text { get; set; }
        public List<String> primary_keys { set; get; }
        public List<String> foreign_keys { set; get; }
        public String length_text = "";
        //constructor

        public Row(string row_name, string data_type, int length, string default_value, string identity, int start, int increment,
            char nullable, string index, char unique, char primary_key, string foreign_key, string integrity, string references, string description)
        {
            if (settings.TSQLMode)
            {
                this.row_name = "[" + row_name + "]";
                this.data_type = "[" + data_type + "]";
            }
            else { 
            this.row_name = row_name;
            this.data_type = data_type;
            }
            this.length = length;
            this.default_value = default_value;
            this.identity = identity;
            this.start = start;
            this.increment = increment;
            this.nullable = nullable;
            this.index = index;
            this.unique = unique;
            this.primary_key = primary_key;
            this.foreign_key = foreign_key;
            this.integrity = integrity;
            this.references = references;
            this.description = description;
            //override user error on data table
            if (data_type.Equals("nvarchar")&&length==0){ length = 50; }
            if (data_type.Equals("date")){ length = 0; }
            if (length == 0) {
                length_text = "";
            }
            if (length > 0) {
                length_text = "(" + length + ")";
            };
        }

        public String row_and_key_gen()
        {
            //generate this as a sql statement, and create an array of primary and foreign keys
            primary_keys = new List<String>();
            foreign_keys = new List<String>();
            String row_text = "";
            row_text = row_text + row_name + "\t";
            row_text = row_text + data_type + length_text+"\t";
            if (nullable.Equals('Y') || nullable.Equals('y')) { row_text = row_text + "null\t"; }
            else { row_text = row_text + "not null\t"; }
            //if (auto_increment.Equals('Y') || auto_increment.Equals('y')) { row_text = row_text + "auto_increment\t"; }
            if (unique.Equals('Y') || unique.Equals('y')) { row_text = row_text + "unique\t"; }
            if (primary_key.Equals('Y') || primary_key.Equals('y')) { primary_keys.Add(row_name); }
            if (foreign_key.Length >= 1) { foreign_keys.Add(this.references); }
            if (settings.TSQLMode)
            {row_text=row_text + "\n";
            }

            else { row_text = row_text + "comment \'" + description + "\'\n"; }
            return row_text;

        }

        public String audit_row_gen()
        {
            //generate each row as a sql statement
            String row_text = "";
            row_text = row_text + row_name + "\t";
            row_text = row_text + data_type + length_text+"\t";
            if (nullable.Equals('Y') || nullable.Equals('y')) { row_text = row_text + "null\t"; }
            else { row_text = row_text + "not null\t"; }
            if (unique.Equals('Y') || unique.Equals('y')) { row_text = row_text + "unique\t"; }
            row_text = row_text + "comment \'" + description + "\'\n";
            return row_text;


        }

    }
}
