using appData2;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Collections.Specialized.BitVector32;



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
        public List<String> alternate_keys { set; get; }
        public table(String name, List<Column> columns)
        {
            if (settings.TSQLMode)
            {
                this.name = "[dbo].[" + name + "]";
            }
            else
            {
                this.name = name;
            }
            this.columns = columns;

        }



        public String gen_IThingAccessor()
        {
            int count = 0;
            string comma = "";
            string output = "";
            string comment = comment_box_gen.comment_box(name, 11);
            string header = "public interface I" + name + "Accessor \n{\n";

            string addThing = "int add " + name + "(" + name + " _" + name + ");\n";



            string selectThingbyPK = name + " select" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string selectallThing = "List<" + name + "> selectAll" + name + "();\n";



            comma = "";
            count = 0;
            string updateThing = "int update" + name + "(";

            updateThing = updateThing + name + "_old" + name + " , " + name + " _new" + name;
            updateThing = updateThing + ");\n";


            string deleteThing = "int delete" + name + "(" + name + " _" + name + ");\n";
            string undeleteThing = "int undelete" + name + "(" + name + " _" + name + ");\n";
            string dropdownThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    dropdownThing = dropdownThing + "List<String> selectDistinct" + fk.referenceTable + "ForDropDown();\n";

                }
            }

            
            output = comment + header + addThing + selectThingbyPK + selectallThing + updateThing + deleteThing + undeleteThing + dropdownThing+ "}\n\n";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable == name)
                {
                    output = output + "List<" + key.mainTable + "> selectAll" + key.mainTable + "by" + name + "();\n";

                }
            }


            return output;
        }



        public String gen_ThingAccessor()
        {

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

            string distinctThing = genAccessorDistinct();


            string output = comment + header + addThing + selectThingbyPK + selectallThing + updateThing + deleteThing + undeleteThing + distinctThing+"}\n\n";
            //good



            return output;


        }
        public String gen_IThingManager()
        {
            int count = 0;
            string comma = "";
            string output = comment_box_gen.JavaDocComment(1, name);
            string comment = comment_box_gen.comment_box(name, 12);
            string header = "public interface I" + name + "Manager \n{\n";

            string addThing = "int add" + name + "(" + name + " _" + name + ");\n";



            string getThingbyPK = name + " get" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string getallThing = "List<" + name + "> getAll" + name + "();\n";



            comma = "";
            count = 0;
            string editThing = "int edit" + name + "(";
            editThing = editThing + name + " _old" + name + " , " + name + " _new" + name;
            editThing = editThing + ");\n";
            string purgeThing = "int purge" + name + "(string " + name + "ID);\n";
            string unPurgeThing = "int unpurge" + name + "(string " + name + "ID);\n";
            string dropdownThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey) {
                if (fk.mainTable == name) {
                    dropdownThing = dropdownThing+"List<String> getDistinct" + fk.referenceTable + "ForDropDown();\n";

                }
            }
            output = comment + header + addThing + getThingbyPK + getallThing + editThing + purgeThing + unPurgeThing +dropdownThing+ "}\n\n";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable == name)
                {
                    output = output + "List<" + key.mainTable + "> getAll" + key.mainTable + "by" + name + "();\n";

                }
            }


            return output;

        }


        public String gen_ThingMananger() {
            String result = "";
            String header = genManagerHeader();
            String Add = genManagerAdd();
            String Delete = genManagerDelete();
            String unDelete = genManagerUnDelete();
            String RetreiveByPK = genManagerPK();
            String RetrieveAll = genManagerAll();
            String Update = genManagerUpdate();
            //String dropdown = genManagerDropDown();
            String footer = "\n}\n";


            result = result
                + header
                + Add
                + Delete
                + unDelete
                + RetreiveByPK
                + RetrieveAll
                + Update
               // + dropdown
                + footer
                ;



            return result;
        
        }

        private string genManagerHeader() {
            string comment = comment_box_gen.comment_box(name, 29);

            String result = "";
            result = result + "public class "+name+"Manager : I"+name+"Manager\n";
            result = result + "{\n";
            result = result + "private I"+name+"Accessor _"+name.ToLower()+"Accessor=null;\n";
            result = result + "//default constuctor uses the database\n";
            result = result + "public " + name + "Manager()\n";
            result = result + "{\n";
            result = result + "_"+name.ToLower()+"Accessor = new "+name+"Accessor();\n";
            result = result + "}\n";
            result = result + "//the optional constuctor can accept any data provider\n";
            result = result + "public"+name+"Manager(I"+name+"Accessor "+name.ToLower()+"Accessor)\n";
            result = result + "{\n";
            result = result + "_"+name.ToLower()+"Accessor = "+name.ToLower()+"Accessor;\n";
            result = result + "}\n";

            return comment + result;

        }
        private String genManagerAdd() {
            string comment = comment_box_gen.comment_box(name, 31);
            String result = "\n";
            result = result + "public bool Add"+name+"("+name+" "+"_"+name+"){\n";
            result = result + "bool result = false;;\n";
            result = result + "try\n{";
            result = result + "result = (1 == _"+name.firstCharLower()+"Accessor.insert"+name+"(_"+name+"));\n";
            result = result + "}\n";
            result = result + "catch (Exception ex)\n";
            result = result + "{\n";
            result = result + "throw new ApplicationException(\""+name+" not added\" + ex.InnerException.Message, ex);;\n";
            result = result + "}\n";
            result = result + "return result;\n";
            result = result + "}\n";
            result = result + "\n";

            return comment + result;


        }
        private String genManagerDelete() {
            string comment = comment_box_gen.comment_box(name, 32);
            string purgeThing = "int purge" + name + "(" +name +" "+ name.ToLower() + "){\n";
            purgeThing = purgeThing + "int result = 0;\n";
            purgeThing = purgeThing + "try{\n";
            purgeThing = purgeThing + "result = _" + name.ToLower() + "Accessor.delete" + name + "("+name.ToLower()+"."+name+"Id);\n";
            purgeThing = purgeThing + "if (result == 0){\n";
            purgeThing = purgeThing + "throw new ApplicationException(\"Unable to Delete " + name+"\" );\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "catch (Exception ex){\n";
            purgeThing = purgeThing + "throw ex;\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "return result;\n}\n";


            return comment + purgeThing;

        }
        private String genManagerUnDelete() {
            string comment = comment_box_gen.comment_box(name, 32);
            string purgeThing = "int unpurge" + name + "(" + name + " " + name.ToLower() + "){\n";
            purgeThing = purgeThing + "int result = 0;\n";
            purgeThing = purgeThing + "try{\n";
            purgeThing = purgeThing + "result = _" + name.ToLower() + "Accessor.undelete" + name + "(" + name.ToLower() + "."+name+"Id);\n";
            purgeThing = purgeThing + "if (result == 0){\n";
            purgeThing = purgeThing + "throw new ApplicationException(\"Unable to restore " + name + "\" );\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "catch (Exception ex){\n";
            purgeThing = purgeThing + "throw ex;\n";
            purgeThing = purgeThing + "}\n";
            purgeThing = purgeThing + "return result;\n}\n";


            return comment + purgeThing;




        }
        private string genManagerPK() {
            string comment = comment_box_gen.comment_box(name, 33);
            string retreiveThing = name + " get" + name + "ByPrimaryKey(string " + name + "ID){\n";
            retreiveThing = retreiveThing + name+" result =null ;\n";
            retreiveThing = retreiveThing + "try{\n";
            retreiveThing = retreiveThing + "result = _" + name.ToLower() + "Accessor.select" + name + "ByPrimaryKey(" + name + "ID);\n";
            retreiveThing = retreiveThing + "if (result == null){\n";
            retreiveThing = retreiveThing + "throw new ApplicationException(\"Unable to retreive " + name + "\" );\n";
            retreiveThing = retreiveThing + "}\n";
            retreiveThing = retreiveThing + "}\n";
            retreiveThing = retreiveThing + "catch (Exception ex){\n";
            retreiveThing = retreiveThing + "throw ex;\n";
            retreiveThing = retreiveThing + "}\n";
            retreiveThing = retreiveThing + "return result;\n}\n";
            return comment + retreiveThing;



        }
        private string genManagerAll() {
            string comment = comment_box_gen.comment_box(name, 34);
            string retreiveAll ="List<"+ name + "> get" + name + "ByAll(string " + name + "ID){\n";
            retreiveAll = retreiveAll + "List<"+name + "> result =new List<"+name+">();\n";
            retreiveAll = retreiveAll + "try{\n";
            retreiveAll = retreiveAll + "result = _" + name.ToLower() + "Accessor.selectAll" + name + "();\n";
            retreiveAll = retreiveAll + "if (result.Count == 0){\n";
            retreiveAll = retreiveAll + "throw new ApplicationException(\"Unable to retreive " + name + "s\" );\n";
            retreiveAll = retreiveAll + "}\n";
            retreiveAll = retreiveAll + "}\n";
            retreiveAll = retreiveAll + "catch (Exception ex){\n";
            retreiveAll = retreiveAll + "throw ex;\n";
            retreiveAll = retreiveAll + "}\n";
            retreiveAll = retreiveAll + "return result;\n}\n";
            return comment+retreiveAll;

        }
        private string genManagerUpdate() {
            string comment = comment_box_gen.comment_box(name, 35);
            string updateThing =  "int update" + name + "( "+name+" old"+name+", "+name+ " new"+ name + "){\n";
            updateThing = updateThing +  "int result =0 ;\n";
            updateThing = updateThing + "try{\n";
            updateThing = updateThing + "result = _" + name.ToLower() + "Accessor.update" + name + "(old" + name+", new" +name+ ");\n";
            updateThing = updateThing + "if (result == 0){\n";
            updateThing = updateThing + "throw new ApplicationException(\"Unable to update " + name + "\" );\n";
            updateThing = updateThing + "}\n";
            updateThing = updateThing + "}\n";
            updateThing = updateThing + "catch (Exception ex){\n";
            updateThing = updateThing + "throw ex;\n";
            updateThing = updateThing + "}\n";
            updateThing = updateThing + "return result;\n}\n";
            return comment + updateThing;



        }
        public String gen_DataObject()
        {

            string output = "";
            output = comment_box_gen.JavaDocComment(1, name);
            output = "public class " + name + "\n{\n";
            int count = 0;


            foreach (Column r in columns)
            {

                String DataAnnotationRequired = "";
                String DataAnnotationLength = "";
                if (r.nullable == 'n' || r.nullable == 'N') {
                    DataAnnotationRequired = "[Required(ErrorMessage = \"Please enter " + r.column_name.bracketStrip() + " \")]\n";
                }
                if (r.length != 0) {
                    DataAnnotationLength = "[StringLength(" + r.length + ")]\n";

                }
                String DataAnnotationDisplayName ="[Display(Name = \""+ r.column_name.bracketStrip() + "\")]\n";
                String add = "public " + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip() + "{ set; get; }\n";
                output = output + DataAnnotationDisplayName + DataAnnotationRequired +DataAnnotationLength+ add;
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
                        String DataAnnotationRequired = "";
                        String DataAnnotationLength = "";
                        if (r.nullable == 'n' || r.nullable == 'N')
                        {
                            DataAnnotationRequired = "[Required(ErrorMessage = \"Please enter " + r.column_name + " \")]\n";
                        }
                        if (r.length != 0)
                        {
                            DataAnnotationLength = "[StringLength(" + r.length + ")]\n";

                        }
                        String DataAnnotationDisplayName = "[Display(Name = \"" + r.column_name + "\")]\n";
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
        private string genAccessorClassHeader()
        {
            string header = "";
            int count = 0;
            string comma = "";
            string output = "";
            string comment = comment_box_gen.JavaDocComment(1, name);
            header = "public class " + name + "Accessor : I" + name + "Accessor {\n";


            return header;


        }
        private String genSPHeaderA(string commandText)
        {
            //for update, insert, delete
            String output = "";
            output = "int rows = 0;\n"
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
        private String genSPHeaderB(string DataObject, string commandText)
        {
            //for single data object
            string output = "";
            output = DataObject + " output = new " + DataObject + "();\n"
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
            output = "List<" + DataObject + "> output = new " + "List<" + DataObject + ">();\n"
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
        private string genSPfooter(int mode)
        {

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
            + "return " + returntype + ";\n}\n";
            return output;
        }
        private string genAccessorAdd()
        {
            string createThing = comment_box_gen.JavaDocComment(0, name);
            int count = 0;
            string comma = "";
            createThing = "\npublic int add" + name + "(" + name + " _" + name.ToLower();

            createThing = createThing + "){\n";
            createThing = createThing + genSPHeaderA("sp_insert_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
                }
            //setting parameters
            createThing = createThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
                }
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

        private string genAccessorRetreiveByKey()
        {
            string retreiveThing = comment_box_gen.JavaDocComment(0, name);
            int count = 0;
            string comma = "";
            retreiveThing = "\npublic " + name + " select" + name + "ByPrimaryKey(";
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
            foreach (Column r in columns)
            {
                if (r.nullable.Equals('n') || r.nullable.Equals("N"))
                {
                    retreiveThing = retreiveThing + "output." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(" + count + ");\n";
                    count++;
                }
                else
                {
                    retreiveThing = retreiveThing + "output." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + count + ") ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(" + count + ");\n";
                    count++;
                }

            }
            retreiveThing = retreiveThing + "\n}\n";
            retreiveThing = retreiveThing + "else \n { throw new ArgumentException(\"" + name + " not found\");\n}\n}";

            //cath block and onwards
            retreiveThing = retreiveThing + genSPfooter(0);




            return retreiveThing;


        }

        private string genAccessorRetreiveAll()
        {

            int count = 0;
            string comma = "";
            string retreiveAllThing = comment_box_gen.JavaDocComment(0, name);
            retreiveAllThing = "\npublic List<" + name + "> selectAll" + name + "(){\n";


            retreiveAllThing = retreiveAllThing + genSPHeaderC(name, "sp_retreive_by_all_" + name);
            //no paramaters to set or add



            //excute the quuery
            retreiveAllThing = retreiveAllThing + "try \n { \n //open the connection \n conn.Open();  ";
            retreiveAllThing = retreiveAllThing + "//execute the command and capture result\n";

            retreiveAllThing = retreiveAllThing + "var reader = cmd.ExecuteReader();\n";

            //capture reuslts
            retreiveAllThing = retreiveAllThing + "//process the results\n";
            retreiveAllThing = retreiveAllThing + "if (reader.HasRows)\n while (reader.Read())\n{";
            retreiveAllThing = retreiveAllThing + "var _" + name + "= new " + name + "();\n";
            count = 0;
            foreach (Column r in columns)
            {
                if (r.nullable.Equals('n') || r.nullable.Equals("N"))
                {
                    retreiveAllThing = retreiveAllThing + "output." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(" + count + ");\n";
                    count++;
                }
                else
                {
                    retreiveAllThing = retreiveAllThing + "output." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + count + ") ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(" + count + ");\n";
                    count++;
                }

            }
            retreiveAllThing = retreiveAllThing + "output.Add(_" + name + ");";
            retreiveAllThing = retreiveAllThing + "\n}\n}";

            //cath block and onwards
            retreiveAllThing = retreiveAllThing + genSPfooter(0);


            return retreiveAllThing;


        }

        private string genAccessorUpdate()
        {
            string updateThing = comment_box_gen.JavaDocComment(0, name);
            int count = 0;
            string comma = "";
            updateThing = "\npublic int update" + name + "(";


            updateThing = updateThing + name + " _old" + name + " , " + name + " _new" + name;
            updateThing = updateThing + ");\n";

            updateThing = updateThing + genSPHeaderA("sp_update_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters.Add(\"@old" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";

                if (r.primary_key != 'y' && r.primary_key != 'Y'&&r.increment==0)
                {
                    updateThing = updateThing + "cmd.Parameters.Add(\"@new" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            updateThing = updateThing + "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters[\"@old" + r.column_name.bracketStrip() + "\"].Value = _old" + name + "." + r.column_name.bracketStrip() + ";\n";
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

        private string genAccessorDelete()
        {
            string deleteThing = comment_box_gen.JavaDocComment(0, name);
            deleteThing += "\npublic int delete" + name + "(" + name + " _" + name.ToLower() + "){\n";
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
            string deleteThing = comment_box_gen.JavaDocComment(0, name);
            deleteThing = deleteThing + "\n public int undelete" + name + "(" + name + " _" + name.ToLower() + "){\n";
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

        public string genAccessorDistinct() {
            string retreiveAllThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {


                    int count = 0;
                    string comma = "";
                    retreiveAllThing = retreiveAllThing+ comment_box_gen.JavaDocComment(0, name);
                    retreiveAllThing = retreiveAllThing + "public List<String> selectDistinct" + fk.referenceTable + "ForDropDown(){\n";



                    retreiveAllThing = retreiveAllThing + genSPHeaderC("String", "sp_select_distinct_and_active_" + fk.referenceTable + "_for_dropdown");
                    //no paramaters to set or add



                    //excute the quuery
                    retreiveAllThing = retreiveAllThing + "try \n { \n //open the connection \n conn.Open();  ";
                    retreiveAllThing = retreiveAllThing + "//execute the command and capture result\n";

                    retreiveAllThing = retreiveAllThing + "var reader = cmd.ExecuteReader();\n";

                    //capture reuslts
                    retreiveAllThing = retreiveAllThing + "//process the results\n";
                    retreiveAllThing = retreiveAllThing + "if (reader.HasRows)\n while (reader.Read())\n{";
                    retreiveAllThing = retreiveAllThing + "String _" + fk.referenceTable + "= reader.Get" + columns[0].data_type.toSqlReaderDataType() + "(0);\n";
                    count = 0;

                    retreiveAllThing = retreiveAllThing + "output.Add(_" + fk.referenceTable + ");";
                    retreiveAllThing = retreiveAllThing + "\n}\n}";

                    //cath block and onwards
                    retreiveAllThing = retreiveAllThing + genSPfooter(0);
                }
            }


            return retreiveAllThing;

        }
        public string genXAMLWindow()
        {
            string comment = comment_box_gen.comment_box(name, 16);
            string WindowCode = comment;
            int rows = columns.Count + 3;
            int width = 4;
            int height = rows * 50 + 100;
            WindowCode += "< !--set window height to " + height + "-- >\n";
            WindowCode += "< Menu Grid.Row = \"0\" Padding = \"20px, 0px\" >\n";
            WindowCode += " < MenuItem x: Name = \"mnuFile\" Header = \"File\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuExit\" Header = \"Exit\" Click = \"mnuExit_Click\" />\n";
            WindowCode += "</ MenuItem >\n";
            WindowCode += " < MenuItem x: Name = \"mnuHelp\" Header = \"Help\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuAbout\" Header = \"About\" />\n";
            WindowCode += "</ MenuItem > \n </Menu>";
            WindowCode += "<Grid>\n";
            WindowCode += "<Grid.RowDefinitions>\n";
            for (int i = 0; i < rows; i++)
            {
                WindowCode += "<RowDefinition Height=\"50\"/>\n";
            }
            WindowCode += "</Grid.RowDefinitions>\n";
            WindowCode += "<Grid.ColumnDefinitions>\n";
            for (int i = 0; i < width; i++)
            {
                WindowCode += "<ColumnDefinition />\n";
            }
            WindowCode += "</Grid.ColumnDefinitions>\n";

            for (int i = 0; i < columns.Count; i++)
            {
                WindowCode += "< Label x:Name = \"lbl" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"1\" Grid.Row = \"" + (i + 1) + "\" Content = \"" + columns[i].column_name + " \" />\n";
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
            WindowCode += "<Button x:Name=\"btnUpdate" + name + "\" Grid.Column=\"2\" Grid.Row=\"" + (rows - 1) + "\" Content=\"Edit " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "<Button x:Name=\"btnAdd" + name + "\" Grid.Column=\"3\" Grid.Row=\"" + (rows - 1) + "\" Content=\"Add " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "< StatusBar Grid.Row =" + rows + ">\n";


            WindowCode += "< StatusBarItem x: Name = \"statMessage\" Content = \"Welcome, please login to continue\" Padding = \"20px, 0px\" />\n </ StatusBar >\n </Grid>\n";
            return WindowCode;


        }

        public string genWindowCSharp()
        {

            string result = "";
            result = result + comment_box_gen.comment_box(name, 17);
            result = result + genStaticVariables();
            result = result + genConstructor();
            result = result + genWinLoad();
            result = result + genAddButton();
            result = result + genEditButton();
            return result;






        }

        private string genAddButton()
        {
            string result = "";

            //else if (columns[i].column_name.bracketStrip().ToLower() == "active") 
            // else   //this means textbox

            result = result + "private void btnAdd" + name + "_click(object sender, RoutedEventArgs e)\n";
            result = result + "if((string)btnAdd" + name + ".Content == \"Add " + name + "\")\n";
            foreach (Column c in columns)
            {
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
            foreach (Column c in columns)
            {
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
            result = "public " + name + "AddEditDelete(" + name + " " + name.Substring(0, 1).ToLower();
            result = result + ")\n{\n";
            result = result + " InitializeComponent();\n";
            result = result + "_" + name.ToLower() + "=" + name.Substring(0, 1).ToLower() + ";\n";
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
            result = "public " + name + " _" + name.ToLower() + "= null;\n";
            result = "public " + name + "Manager+ _" + name.Substring(0, 1) + "m = null;\n";
            return result;

        }

        public string genJavaModel()
        {
            string result = "";
            result = result + genJavaHeader();
            result = result + genJavaInstanceVariables();
            result = result + genJavaContructor();
            result = result + genJavaSetterAndGetter();
            result = result + genJavaFooter();

            return result;
        }

        private string genJavaHeader()
        {
            string result = comment_box_gen.JavaDocComment(1, name);
            result = result + "\n public class " + name + " {\n";
            return result;

        }
        private string genJavaInstanceVariables()
        {
            string result = "";
            foreach (Column r in columns)
            {
                result = result + "private " + r.data_type.toJavaDataType() + " " + r.column_name + ";\n";

            }
            return result;

        }
        private string genJavaContructor()
        {
            string result = "";
            string defaultConstructor = "\npublic " + name + "(){}\n";
            string ParamConsctructor = "\npublic " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                ParamConsctructor = ParamConsctructor + comma + r.data_type.toJavaDataType() + " " + r.column_name;
                comma = ", ";
            }
            ParamConsctructor = ParamConsctructor + ") {\n";
            foreach (Column r in columns)
            {

                ParamConsctructor = ParamConsctructor + "\nthis." + r.column_name + " = " + r.column_name + ";";
            }
            ParamConsctructor = ParamConsctructor + "\n}\n";
            result = defaultConstructor + ParamConsctructor;
            return result;

        }

        private string genJavaSetterAndGetter()
        {
            string result = "";
            foreach (Column r in columns)
            {
                string getter = "public " + r.data_type.bracketStrip().toJavaDataType() + " get" + r.column_name + "() {\n return " + r.column_name + ";\n}";
                string setter = "public void set" + r.column_name + "(" + r.data_type.bracketStrip().toJavaDataType() + " " + r.column_name + ") {\n";
                if (r.data_type == "nvarchar")
                {
                    setter = setter + r.column_name + " = " + r.column_name + ".replaceAll(\"[^A-Za-z0-9 - ]\",\"\");\n";
                    setter = setter + "if(" + r.column_name + ".length()<4){\n";
                    setter = setter + "throw new IllegalArgumentException(\"" + r.column_name + " is too short.\");\n}\n";
                    setter = setter + "if(" + r.column_name + ".length()>" + r.length + "){\n";
                    setter = setter + "throw new IllegalArgumentException(\"" + r.column_name + " is too long.\");\n}\n";
                }
                if (r.data_type == "int")
                {

                }
                setter = setter + "this." + r.column_name + " = " + r.column_name + ";\n}";
                result = result + getter + "\n" + setter + "\n";
            }
            return result;

        }
        private string genJavaFooter()
        {
            string result = "\n}\n";

            return result;

        }

        public string genJavaDAO()
        {
            string result = "";
            result = result + genJavaDAOHeader();        //works
            result = result + genJavaDAOCreate();       //returns ""
            result = result + genJavaDAORetreiveByKey(); // works
            result = result + genJavaDAORetreiveAll(); // wokring on it now
            result = result + genJavaDAORetriveActive(); // working on it now
            result = result + genJavaDAOUpdate(); //rturns ""
            result = result + genJavaDelete(); //returns ""
            result = result + genJavaunDelete(); //returns ""
            result = result + genJavaDAOFooter(); //work

            return result;


        }
        private string genJavaDAOHeader()
        {
            string result = comment_box_gen.JavaDocComment(1, name + "DAO");
            result = result + "import com.beck.javaiii_kirkwood.personal_project.models." + name + ";\n" +

                "import java.sql.CallableStatement;\n" +
                "import java.sql.Connection;\n" +
                "import java.sql.ResultSet;\n" +
                "import java.sql.SQLException;\n" +
                "import java.util.ArrayList;\n" +
                "import java.util.List;\n" +
                "import java.time.LocalDate;\n" +

                "import static com.beck.javaiii_kirkwood.personal_project.data.Database.getConnection;\n";


            result = result + "public class " + name + "DAO {\n\n";
            return result;


        }
        private string genJavaDAOFooter()
        {
            string result = "\n}\n";
            return result;


        }
        private string genJavaDAORetreiveByKey()
        {
            string result = "";
            // result = comment_box_gen.JavaDocComment(0, name);
            string nullValue = "";
            string comma = "";
            result = result + "public static " + name + " get" + name + "ByPrimaryKey(" + name + " _" + name.ToLower() + ") throws SQLException{\n";
            result = result + name + " result = null;\n";
            result = result + "try(Connection connection = getConnection()) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_pk_" + name + "(?)}\")) {\n";
            for (int i = 0; i < columns.Count; i++)
                if (columns[i].primary_key == 'y' || columns[i].primary_key == 'Y')
                {
                    result = result + "statement.setString(1, _" + name.ToLower() + ".get" + columns[i].column_name + "().toString());\n";
                }
            result = result + "\ntry (ResultSet resultSet = statement.executeQuery()){";
            result = result + "\nif(resultSet.next()){";
            foreach (Column r in columns)
            {


                result = result + r.data_type.toJavaDataType() + " " + r.column_name + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + r.column_name + "\");\n";
                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                {

                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + "result = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name;
                comma = ",";
            }

            result = result + ");";
            result = result + "}\n}\n}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + " throw new RuntimeException(e);\n}\n";
            result = result + "return result;\n}\n";




            return result;


        }

        private string genJavaDAORetreiveAll()
        {
            string nullValue = "\"\"";
            string result = "";
            //result = comment_box_gen.JavaDocComment(0, name);
            string comma = "";
            result = result + "public static List<" + name + "> getAll" + name + "() {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result = result + "try (Connection connection = getConnection()) { \n";
            result = result + "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_all_" + name + "()}\")) {";
            result = result + "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result = result + "while (resultSet.next()) {";
            foreach (Column r in columns)
            {
                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + r.column_name + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {

                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name;
                comma = ",";
            }
            result = result + ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result = result + "}\n";

            result = result + "return result;}\n";

            return result;


        }

        public String genJavaDAORetriveActive() {
            string nullValue = "\"\"";
            string result = "";
            //result = comment_box_gen.JavaDocComment(0, name);
            string comma = "";
            result = result + "public static List<" + name + "> getActive" + name + "() {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result = result + "try (Connection connection = getConnection()) { \n";
            result = result + "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_active_" + name + "()}\")) {";
            result = result + "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result = result + "while (resultSet.next()) {";
            foreach (Column r in columns)
            {
                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + r.column_name + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {

                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name;
                comma = ",";
            }
            result = result + ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result = result + "}\n";

            result = result + "return result;}\n";

            return result;
        }


        private string genJavaDAOUpdate()
        {
            string result = "";
            //result = comment_box_gen.JavaDocComment(0, name);
            result = result + "\n public static int update("+name+" old"+name+", "+name+" new"+name+") throws SQLException{\n";
            result = result + "int result = 0;\n";
            result = result + "try (Connection connection = getConnection()) {\n";
            result = result + "if (connection !=null){\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_update_" + name+"(";
            string comma = "";
            foreach (Column r in columns) {
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    result = result + comma + "? ";
                    comma = ",";
                }
                else {
                    result = result + comma + "?,?";
                }
            }
            result = result + ")}\"))\n {\n";
            int count = 1;
            foreach (Column r in columns) { 
            result=result+"statement.set"+r.data_type.toJavaDAODataType()+"("+count+",old"+name+".get"+r.column_name + "());\n";
                count++;
                if (r.primary_key != 'y' && r.primary_key != 'Y') {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "("+count+",new" + name + ".get" + r.column_name + "());\n";
                    count++;
                }
            }
            result = result + "result=statement.executeUpdate();\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not update " + name + " . Try again later\");\n";
            result = result + "}\n}\n}\n return result;\n}\n";
            return result;


        }

        private string genJavaDelete()
        {
            string result = "";
            //result = comment_box_gen.JavaDocComment(0, name);
            result = result + "public static int delete"+name+"(int "+name.ToLower()+"ID) {\n";
            result = result + "int rowsAffected=0;\n";
            result = result + "try (Connection connection = getConnection()) {\n";
            result = result + "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_Delete_"+name+ "( ?)}\")){\n";
            result = result + "statement.setInt(1,"+name.ToLower()+"ID);\n";
            result = result + "rowsAffected = statement.executeUpdate();\n";
            result = result + "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete "+name+". Try again later\");\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete "+name+". Try again later\");\n";
            result = result + "}\n";
            result = result + "return rowsAffected;\n";
            result = result + "}\n";
            result = result + "";
            return result;


        }
        private string genJavaunDelete()
        {
            string result = "";
            //result = comment_box_gen.JavaDocComment(0, name);
            result = result + "public static int undelete" + name + "(int " + name.ToLower() + "ID) {\n";
            result = result + "int rowsAffected=0;\n";
            result = result + "try (Connection connection = getConnection()) {\n";
            result = result + "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_unDelete_" + name + "( ?)}\")){\n";
            result = result + "statement.setInt(1," + name.ToLower() + "ID);\n";
            result = result + "rowsAffected = statement.executeUpdate();\n";
            result = result + "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result = result + "}\n";
            result = result + "return rowsAffected;\n";
            result = result + "}\n";
            result = result + "";
            return result;


        }

        private string genJavaDAOCreate()
        {
            string result = "";
            // result = comment_box_gen.JavaDocComment(0, name);
            string comma = "";
            result = result + "public static int add(" + name + " _" + name.ToLower() + ") {\n";
            result = result + "int numRowsAffected=0;";
            result = result + "try (Connection connection = getConnection()) {\n";
            result = result + "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_insert_" + name + "(";
            foreach (Column r in columns)
            {
                if (r.identity == "" &&r.default_value=="")
                {
                    result = result + comma + " ?";
                    comma = ",";
                }
            }
            result = result + ")}\")){\n";
            int count = 1;
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    count++;
                }
            }
            result = result + "numRowsAffected = statement.executeUpdate();\n";
            result = result + "if (numRowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result = result + "} \n}\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result = result + "return numRowsAffected;\n}";
            result = result + "\n";
            return result;


        }

        public string getBatch()
        {
            string result = "";
            result = result + "sqlcmd -S localhost -E -i " + name + ".sql\n";
            result = result + "ECHO .\n";
            return result;



        }

        public string genCreateServelet()
        {
            string result = "";
            result = result + importStatements();
            //do get
            result = result+ comment_box_gen.comment_box(name, 21);
            result = result + "\n@WebServlet(\"/add" + name + "\")\n";
            result = result + "public class Add" + name + "Servlet extends HttpServlet{\n";

            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');

                    result = result + "static List<" + parts[0] + "> all" + parts[0] + "s = " + parts[0] + "DAO.getActive" + parts[0] + "();\n";
                    //grab a list of the parents, assign and create a static variable
                }
            }
            result = result + "\n @Override\n";
            result = result + "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            
            result = result + privLevelStatement();
            result = result + "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Add " + name + "\");\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getAll" + parts[0] + "();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";


                }
            }


            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Add" + name + ".jsp\").forward(req, resp);\n";
            result = result + "}\n";





            //this only creates the doPost method
            result += "\n";
            //gen header
            result = result + "@Override\n";
            result = result + "  protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result = result + privLevelStatement();
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getAll" + parts[0] + "();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";


                }
            }
            //get data
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    result = result + "String _" + r.column_name + " = req.getParameter(\"" + fieldname + "\");\n";
                    result = result + "_" + r.column_name + "=_" + r.column_name + ".trim();\n";
                }
            }
            //toss it in a hashmap
            result = result + "Map<String, String> results = new HashMap<>();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result = result + "results.put(\"" + r.column_name + "\",_" + r.column_name + ");\n";
                }
            }
            //generate an object, and set errors
            result = result + name + " " + name.ToLower() + " = new " + name + "();\n";
            result = result + "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 &&r.default_value=="")
                {

                    string errorname = name.ToLower() + r.column_name + "error";
                    result = result + "try {\n";
                    if (r.data_type.ToLower().Equals("int"))
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(Integer.valueOf(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Equals("datetime"))
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(LocalDate.parse(_" + r.column_name + "));\n";
                    }
                    else
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(_" + r.column_name + ");\n";
                    }
                    result = result + "} catch(IllegalArgumentException e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result = result + "errors++;\n";
                    result = result + "}\n";

                }



            }
            //add it to the databsae, maybe?

            result = result + "int result=0;\n";
            result = result + "if (errors==0){\n";
            result = result + "try{\nresult=" + name + "DAO.add(" + name.ToLower() + ");\n}";
            result = result + "catch(Exception ex){\n";
            result = result + "results.put(\"dbStatus\",\"Database Error\");\n";
            result = result + "}\n";
            result = result + "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Added\");\n";
            result = result + "resp.sendRedirect(\"all-"+name+"s\");\n";
            result = result + "return;\n";
            result = result + "} else {\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Not Added\");\n";
            result = result + "\n}\n";
            //set db message
            result = result + "}\n";

            //send it back
            result = result + "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Create a " + name + " \");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Add" + name + ".jsp\").forward(req, resp);\n";
            result = result + "\n}\n}\n";


            //get_buttons
            //get_footer




            return result;

        }

        public string genviewAllJSP()
        {
            //comment box
            string result = comment_box_gen.comment_box(name, 20);
            //header comment
            //gen header
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result = result + "<div class = \"container\">\n";
            result = result + "<div class=\"row\">\n";
            result = result + "<div class=\"col-12\">\n";
            result = result + "<h1>All Roller " + name + "s</h1>\n";
            result = result + "<p>There ${" + name + "s.size() eq 1 ? \"is\" : \"are\"}&nbsp;${" + name + "s.size()} " + name + "${" + name + "s.size() ne 1 ? \"s\" : \"\"}</p>\n";
            result = result + "Add "+name+"   <a href=\"add"+name+"\">Add</a>\n";
            result = result + "<c:if test=\"${" + name + "s.size() > 0}\">\n";
            result = result + "<div class=\"table-responsive\">";
            result = result + "<table class=\"table table-bordered\">\n";
            result = result + "<thead>\n";
            result = result + "<tr>\n";
            foreach (Column r in columns)
            {
                result = result + "<th scope=\"col\">" + r.column_name + "</th>\n";
            }
            result = result + "<th scope=\"col\">Edit</th>\n";
            result = result + "<th scope=\"col\">Delete</th>\n";
            result = result + "</tr>\n";
            result = result + "</thead>\n";
            result = result + "<tbody>\n";
            result = result + "<c:forEach items=\"${" + name + "s}\" var=\"" + name.ToLower() + "\">\n";
            result = result + "<tr>\n";
            foreach (Column r in columns)
            {
                //https://stackoverflow.com/questions/21755757/first-character-of-string-lowercase-c-sharp

                if (!r.data_type.ToLower().Equals( "bit"))
                {
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=view\">${fn:escapeXml(" + name.ToLower()+"."+name.ToLower()+"_ID)}</a></td>";
                    }

                    else
                    {
                        result = result + "<td>${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</td>\n";
                    }

                }
                else {
                    result = result + "<td><input type=\"checkbox\" disabled <c:if test=\"${" + name.ToLower() + ".is_active}\">checked</c:if>></td>\n";
                }
                }
            result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=edit\" > Edit </a></td>\n";
            result = result + "<td><a href = \"delete" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}" +
                "&mode=" +
                "<c:choose>" +
                "<c:when test=\"${"+name.ToLower()+".is_active}\">0</c:when>\n" +
                "\t\t\t\t\t\t<c:otherwise>1</c:otherwise>\n" +
                "\t\t\t\t\t\t</c:choose>\">\n" +              
                "<c:if test=\"${!"+name.ToLower()+".is_active}\">un</c:if>Delete </a></td> \n";
            result = result + "</tr>\n";
            result = result + "</c:forEach>\n";
            result = result + "</tbody>\n";
            result = result + "</table>\n";
            result = result + "</div>\n";
            result = result + "</c:if>\n";
            result = result + "</div>\n";
            result = result + "</div>\n";
            result = result + "</div>\n";
            result = result + "</main>\n";
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";


            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer




            return result;

        }

        public string genCreateJSP()
        {
            int rowcount = 0;
            //comment box
            string result = comment_box_gen.comment_box(name, 19);
            //header comment
            //gen header
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result = result + "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/add" + name + "\" id = \"add" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.increment == 0)
                    {
                        if (r.foreign_keys.Count<1||r.foreign_keys[i] == "")
                        {
                            string inputType = "text";
                            if (r.data_type == "datetime") { inputType = "date"; }
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result = result + "<!-- " + r.column_name + " -->\n";
                            result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result = result + "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result = result + "</c:if>\n";
                            result = result + "</div>\n";
                            result = result + "</div>\n";
                            rowcount++;
                        }

                        else
                        {
                            string[] parts = r.foreign_keys[i].Split('.');
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result = result + "<!-- " + r.column_name + " -->\n";
                            result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result = result + "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                            result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                            result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\">${" + parts[0] + ".name}   </option>\n";
                            result = result + "</c:forEach>\n";
                            result = result + "</select>\n";
                            result = result + "";

                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result = result + "</c:if>\n";
                            result = result + "</div>\n";
                            result = result + "</div>\n";
                            rowcount++;
                            i++;


                        }
                    }
                }
            }
            //get_buttons
            result = result + "<div class=\"align-items-center mt-0\">\n";
            result = result + "<div class=\"d-grid\">";
            result = result + "<button class=\"btn btn-orange mb-0\" type=\"submit\">Create "+name+"  </button></div>\n";
            result = result + "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result = result + "<p>${results.dbStatus}</p>\n";
            result = result + "</c:if>\n";
            result = result + "</div>\n";
            result = result + "</form>\n";
            result = result + "</div>\n";
            
            //get_footer
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";




            return result;

        }

        public string genviewAllServlet()
        {
            //this only creates the doGet method
            string result = comment_box_gen.comment_box(name, 22);
            //gen header
            result = result + importStatements(); 
            

            result = result + "@WebServlet(\"/all-" + name + "s\")\n";
            result = result + "public class All" + name + "sServlet extends HttpServlet {";
            result = result + "@Override\n";
            result = result + "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            
            result = result + privLevelStatement();
            result =result+"session.setAttribute(\"currentPage\",req.getRequestURL());\n";

            result = result + "List<" + name + "> " + name.ToLower() + "s = null;\n";
            result = result + "\n";
            result = result + name.ToLower() + "s =" + name + "DAO.getAll" + name + "();\n";
            result = result +  "\n";
            result = result + "req.setAttribute(\"" + name + "s\", " + name.ToLower() + "s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All " + name + "s\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/all-" + name + "s.jsp\").forward(req,resp);\n";
            result = result + "\n}\n}";
            //header comment
            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer




            return result;

        }

        public string genDeleteServlet() {
            string result = comment_box_gen.comment_box(name, 25);
            result=result+importStatements();
            result = result + "@WebServlet(\"/delete" + name.ToLower() + "\")";
            result = result + "public class Delete"+name+"Servlet extends HttpServlet {\n";
            result = result + "@Override\n";
            result = result + "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result = result + "Map<String, String> results = new HashMap<>();\n";
            result = result + privLevelStatement();
            
            result = result + "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Delete "+name+"\");\n";
            result = result + "int "+name+"ID = Integer.valueOf(req.getParameter(\""+name.ToLower()+"id\"));\n";
            result = result + "int mode = Integer.valueOf(req.getParameter(\"mode\"));\n";
            result = result + "int result = 0;\n";
            result = result + "if (mode==0){\n";
            result = result + "try{\n";
            result = result + "result = "+name+"DAO.delete"+name+"("+name+"ID);\n";
            result = result + "}\n";
            result = result + "catch(Exception ex){\n";
            result = result + "results.put(\"dbStatus\",ex.getMessage());\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "else {\n";
            result = result + "try{\n";
            result = result + "result = "+name+"DAO.undelete"+name+"("+name+"ID);\n";
            result = result + "}\n";
            result = result + "catch(Exception ex){\n";
            result = result + "results.put(\"dbStatus\",ex.getMessage());\n";
            result = result + "}\n";
            result = result + "}\n";
            result = result + "List<"+name+"> "+name.ToLower()+"s = null;\n";
            result = result + name.ToLower()+"s = "+name+"DAO.getAll"+name+"();\n";
            result = result + "req.setAttribute(\"results\",results);\n";
            result = result + "req.setAttribute(\""+name+"s\", "+name.ToLower()+"s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All "+name+"\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/all-"+name+"s.jsp\").forward(req, resp);\n";
            result = result + "}\n";
            result = result + "}\n";

            return result;
        }

        public string genViewEditServlet() {
            //do get
            string result = "";
            result = result + importStatements();
            //do get
            result = result + comment_box_gen.comment_box(name, 21);
            result = result + "\n@WebServlet(\"/edit" + name + "\")\n";
            result = result + "public class Edit" + name + "Servlet extends HttpServlet{\n";

            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');

                    result = result + "static List<" + parts[0] + "> all" + parts[0] + "s = " + parts[0] + "DAO.getActive" + parts[0] + "();\n";
                    //grab a list of the parents, assign and create a static variable
                }
            }
            result = result + "\n @Override\n";
            result = result + "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result = result + privLevelStatement();
            result = result + "String mode = req.getParameter(\"mode\");\n";
            result = result + "int primaryKey = Integer.parseInt(req.getParameter(\""+name.ToLower()+"id\"))\n";
            result = result + name+" "+name.ToLower()+"= new "+name+ "();\n";
            result = result + name.ToLower() + ".set" + name + "_ID(primaryKey);\n";
            result = result + "try{\n";
            result = result + name.ToLower()+"="+name+"DAO.get"+name+"ByPrimaryKey("+name.ToLower()+");\n";
            result = result + "} catch (SQLException e) {\n";
            result = result + "req.setAttribute(\"dbStatus\",e.getMessage());\n";
            result = result + "}\n";
            result = result + "HttpSession session = req.getSession();\r\n";
            result = result + "session.setAttribute(\""+name.ToLower()+"\","+name.ToLower()+");\n";
            result = result + "req.setAttribute(\"mode\",mode);\n";
            result = result + "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Add " + name + "\");\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getAll" + parts[0] + "();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";


                }
            }


            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Edit" + name + ".jsp\").forward(req, resp);\n";
            result = result + "}\n";
            result = result + " @Override\n";
            result = result + "protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result = result + privLevelStatement();
            result = result + "Map<String, String> results = new HashMap<>();\n";
            result = result + "String mode = req.getParameter(\"mode\");\n";
            result = result + "req.setAttribute(\"mode\",mode);\n";
            result = result + "//to set the drop downs\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getAll" + parts[0] + "();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";


                }
            }
            result = result + "//to get the old "+name+"\n";
            result = result + "HttpSession session = req.getSession();\n";
            result = result + name+" _old"+name+"= ("+name+")session.getAttribute(\""+name.ToLower()+"\");\n";
            result = result + "//to get the new event's info\n";

            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    result = result + "String _" + r.column_name + " = req.getParameter(\"" + fieldname + "\");\n";
                    result = result + "_" + r.column_name + "=_" + r.column_name + ".trim();\n";
                }
            }
            //toss it in a hashmap
            
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result = result + "results.put(\"" + r.column_name + "\",_" + r.column_name + ");\n";
                }
            }

            //generate the object
            result = result + name + " _new" + name + " = new " + name + "();\n";
            result = result + "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 )
                {

                    string errorname = name.ToLower() + r.column_name + "error";
                    result = result + "try {\n";
                    if (r.data_type.ToLower().Equals("int"))
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(Integer.valueOf(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Equals("datetime"))
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(LocalDate.parse(_" + r.column_name + "));\n";
                    }
                    else
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(_" + r.column_name + ");\n";
                    }
                    result = result + "} catch(IllegalArgumentException e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result = result + "errors++;\n";
                    result = result + "}\n";

                }
            }
            result = result + "_new"+name+".setis_active(true);\n";
            result = result + "//to update the database\n";
            result = result + "int result=0;\n";
            result = result + "if (errors==0){\n";
            result = result + "try{\n";
            result = result + "result="+name+"DAO.update(_old"+name+",_new"+name+");\n";
            result = result + "}catch(Exception ex){\n";
            result = result + "results.put(\"dbStatus\",\"Database Error\");\n";
            result = result + "}\n";
            result = result + "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\""+name+" updated\");\n";
            result = result + "resp.sendRedirect(\"all-"+name+"s\");\n";
            result = result + "return;\n";
            result = result + "} else {\n";
            result = result + "results.put(\"dbStatus\",\""+name+" Not Updated\");\n";
            result = result + "}\n}\n";
            result = result + "//standard\n";
            result = result + "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Edit a "+name+" \");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Edit"+name+".jsp\").forward(req, resp);\n";
            result = result + "}\n}\n";



            return result;

           
        }

        public string genViewEditJSP() {
            int rowcount = 0;
            //comment box
            string result = comment_box_gen.comment_box(name, 26);
            //header comment
            //gen header
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result = result + "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/edit" + name + "\" id = \"edit" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.primary_key.Equals('Y') || r.primary_key.Equals('y')) {
                        
                        
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<h2>"  + r.column_name + "  :  \n";
                        
                        result = result + " ${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</h2>\n";
                        
                        result = result + "</div>\n";
                        
                        rowcount++;
                        continue;
                        
                    }
                    if ((r.foreign_keys.Count < 1 || r.foreign_keys[i] == ""))
                    {
                        string inputType = "text";
                        if (r.data_type == "datetime") { inputType = "date"; }
                        string fieldname = "input" + name.ToLower() + r.column_name;
                        string errorname = name.ToLower() + r.column_name + "error";
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                        result = result + "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result = result + "</c:if>\n";
                        result = result + "</div>\n";
                        result = result + "</div>\n";
                        rowcount++;
                    }

                    else
                    {
                        string[] parts = r.foreign_keys[i].Split('.');
                        string fieldname = "input" + name.ToLower() + r.column_name;
                        string errorname = name.ToLower() + r.column_name + "error";
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                        result = result + "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\"  <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                        result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\"" +
                        "<c:if test=\"${" + name.ToLower() + "." + parts[1].firstCharLower() + " eq " + parts[0] + "." + parts[1].firstCharLower() +"}\"> selected </c:if>>${" + parts[0] + ".name}   </option>\n";
                            result = result + "</c:forEach>\n";
                            result = result + "</select>\n";
                            result = result + "";

                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result = result + "</c:if>\n";
                            result = result + "</div>\n";
                            result = result + "</div>\n";
                            rowcount++;
                            i++;


                        }
                    
                }
            }
            //get_buttons
            result = result + "<div class=\"align-items-center mt-0\">\n";
            result = result + "<div class=\"d-grid\">";
            result = result + "<button class=\"btn btn-orange mb-0\" type=\"submit\">Edit "+name+" </button></div>\n";
            result = result + "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result = result + "<p>${results.dbStatus}</p>\n";
            result = result + "</c:if>\n";
            result = result + "</div>\n";
            result = result + "</form>\n";
            result = result + "</div>\n";

            //get_footer
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";




            return result;
        }

        public string genIndexJSP()
        {
            string result = "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            result = result + "<div class=\"table-responsive\"><table class=\"table table-bordered\">\n";
            result = result + "<thead>\n";
            result = result + "<tr>\n";
            result = result + "<th scope=\"col\">Table</th>\n";

            result = result + " <th scope=\"col\">Action</th>\n";
            result = result + "</tr>\n";
            result = result + "</thead>\n";
            result = result + "<tbody>\n";
            foreach (table t in data_tables.all_tables)
            {
                result = result + "<tr><td>View all " + t.name + "</td><td><a href=\"all-" + t.name + "s\"> View </a> </td></tr>\n";


            }
            result = result + "</tbody>\n";
            result = result + "</table>\n";
            result = result + "</div>\n";
            result = result + "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";



            
            

            return result;

        }
        
        
        private string importStatements() {
            string result = "\n";
            result = result + "import com.beck.javaiii_kirkwood.personal_project.data." + name + "DAO;\n";
            result = result + "import com.beck.javaiii_kirkwood.personal_project.models." + name + ";\n";
            result = result + "import com.beck.javaiii_kirkwood.personal_project.models.User;\n";
            result = result + "import jakarta.servlet.ServletException;\n";
            result = result + "import jakarta.servlet.annotation.WebServlet;\n";
            result = result + "import jakarta.servlet.http.HttpServlet;\n";
            result = result + "import jakarta.servlet.http.HttpServletRequest;\n";
            result = result + "import jakarta.servlet.http.HttpServletResponse;\n";
            result = result + "import jakarta.servlet.http.HttpSession;\n";

            result = result + "import java.io.IOException;\n";
            result = result + "import java.util.HashMap;\n";
            result = result + "import java.util.List;\n";
            result = result + "import java.util.Map;\n";
            return result;
        
        }

        private string privLevelStatement() {
            string result = "\n//To restrict this page based on privilege level\n";
            result = result + "int PRIVILEGE_NEEDED = 0;\n";
            result = result + "HttpSession session = req.getSession();\n";
            result = result + "User user = (User)session.getAttribute(\"User\");\n";
            result = result + "if (user==null||user.getPrivilege_ID()<PRIVILEGE_NEEDED){\n";
            result = result + "resp.sendError(HttpServletResponse.SC_FORBIDDEN);\n";
            result = result + "return;\n";
            result = result + "}\n";
            result = result + "\n";

            return result;
        
        
        }




    }


}



