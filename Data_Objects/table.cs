using appData2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
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
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates an data access layer interface for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c#</returns>
        public String gen_IThingAccessor()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IAccessor);
            string header = "public interface I" + name + "Accessor \n{\n";
            string addThing = "int add" + name + "(" + name + " _" + name + ");\n";
            string selectThingbyPK = name + " select" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string selectallThing = "List<" + name + "> selectAll" + name + "();\n";
            string updateThing = "int update" + name + "(";
            updateThing = updateThing + name + "_old" + name + " , " + name + " _new" + name;
            updateThing += ");\n";
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
            string selectfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    selectfkThing = selectfkThing + "select" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ", int limit, int offset);";
                }
            }
            string output = comment + header + addThing + selectThingbyPK + selectallThing + selectfkThing + updateThing + deleteThing + undeleteThing + dropdownThing + "}\n\n";
            return output;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates an data access layer  for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer  in c# </returns>
        public String gen_ThingAccessor()
        {
           
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Accessor);
            
            string header = genAccessorClassHeader();
            
            string addThing = genAccessorAdd();
            
            string selectThingbyPK = genAccessorRetreiveByKey();
            
            string selectallThing = genAccessorRetreiveAll();
            
            string selectbyFK = genAccessorRetreivefk();
            
            string updateThing = genAccessorUpdate();
          
            string deleteThing = genAccessorDelete();
            
            string undeleteThing = genAccessorUndelete();
            string distinctThing = genAccessorDistinct();
            string output = comment + header + addThing + selectThingbyPK + selectallThing + selectbyFK + updateThing + deleteThing + undeleteThing + distinctThing + "}\n\n";
            //good
            return output;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a logic layer interface for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c# </returns>
        public String gen_IThingManager()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpIManager);
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IManager);
            string header = "public interface I" + name + "Manager \n{\n";
            string addThing = "int add" + name + "(" + name + " _" + name + ");\n";
            string getThingbyPK = name + " get" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string getallThing = "List<" + name + "> getAll" + name + "();\n" +
                                   "List<" + name + "> getAll" + name + "(int offset);\n" +
                                   "List<" + name + "> getAll" + name + "(int limit, int offset);\n";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            string getfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    getfkThing = getfkThing + "List<" + name + "> get" + name + "by" + fk_table + "(" + fk_name + ");\n" +
                                   "List<" + name + "> getAll" + name + "by" + fk_table + "(" + fk_name + ",int offset);\n" +
                                   "List<" + name + "> getAll" + name + "by" + fk_table + "(" + fk_name + ",int limit, int offset);\n";
                }
            }
            string editThing = "int edit" + name + "(";
            editThing = editThing + name + " _old" + name + " , " + name + " _new" + name;
            editThing += ");\n";
            string purgeThing = "int purge" + name + "(string " + name + "ID);\n";
            string unPurgeThing = "int unpurge" + name + "(string " + name + "ID);\n";
            string dropdownThing = "";
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    dropdownThing = dropdownThing + "List<String> getDistinct" + fk.referenceTable + "ForDropDown();\n";
                }
            }
            string output = comment + header + addThing + getThingbyPK + getallThing + getfkThing + editThing + purgeThing + unPurgeThing + dropdownThing + "}\n\n";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable == name)
                {
                    output = output + "List<" + key.mainTable + "> getAll" + key.mainTable + "by" + name + "();\n";
                }
            }
            return output;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a logic layer  for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c# </returns>
        public String gen_ThingMananger()
        {
            String result = "";
            String header = genManagerHeader();
            String Add = genManagerAdd();
            String Delete = genManagerDelete();
            String unDelete = genManagerUnDelete();
            String RetreiveByPK = genManagerPK();
            String RetreiveByFK = genManagerFK();
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
                + RetreiveByFK
                + RetrieveAll
                + Update
                // + dropdown
                + footer
                ;
            return result;
        }
        /// <summary>
        /// Generates a dependency-inversion based header for the Logic layer manager, which incorportates a constructor
        /// that takes the data access object.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the logic layer </returns>
        private string genManagerHeader()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IManager);
            String result = "";
            result = result + "public class " + name + "Manager : I" + name + "Manager\n";
            result += "{\n";
            result = result + "private I" + name + "Accessor _" + name.ToLower() + "Accessor=null;\n";
            result += "//default constuctor uses the database\n";
            result = result + "public " + name + "Manager()\n";
            result += "{\n";
            result = result + "_" + name.ToLower() + "Accessor = new " + name + "Accessor();\n";
            result += "}\n";
            result += "//the optional constuctor can accept any data provider\n";
            result = result + "public " + name + "Manager(I" + name + "Accessor " + name.ToLower() + "Accessor)\n";
            result += "{\n";
            result = result + "_" + name.ToLower() + "Accessor = " + name.ToLower() + "Accessor;\n";
            result += "}\n";
            return comment + result;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for adding to the database.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer add method, in c#. </returns>
        private String genManagerAdd()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Add);
            String result = "\n";
            result = result + "public bool Add" + name + "(" + name + " " + "_" + name + "){\n";
            result += "bool result = false;;\n";
            result += "try\n{";
            result = result + "result = (1 == _" + name.firstCharLower() + "Accessor.insert" + name + "(_" + name + "));\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + " not added\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            result += "\n";
            return comment + result;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for deleting
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer delete method, in c#. </returns>
        private String genManagerDelete()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Delete);
            string purgeThing = "public int purge" + name + "(" + name + " " + name.ToLower() + "){\n";
            purgeThing += "int result = 0;\n";
            purgeThing += "try{\n";
            purgeThing += "result = _" + name.ToLower() + "Accessor.delete" + name + "(" + name.ToLower() + "." + name + "Id);\n";
            purgeThing += "if (result == 0){\n";
            purgeThing += "throw new ApplicationException(\"Unable to Delete " + name + "\" );\n";
            purgeThing += "}\n";
            purgeThing += "}\n";
            purgeThing += "catch (Exception ex){\n";
            purgeThing += "throw ex;\n";
            purgeThing += "}\n";
            purgeThing += "return result;\n}\n";
            return comment + purgeThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for undeleting
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer undelete method, in c#. </returns>
        private String genManagerUnDelete()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Undelete);
            string purgeThing = "public int unpurge" + name + "(" + name + " " + name.ToLower() + "){\n";
            purgeThing += "int result = 0;\n";
            purgeThing += "try{\n";
            purgeThing = purgeThing + "result = _" + name.ToLower() + "Accessor.undelete" + name + "(" + name.ToLower() + "." + name + "Id);\n";
            purgeThing += "if (result == 0){\n";
            purgeThing = purgeThing + "throw new ApplicationException(\"Unable to restore " + name + "\" );\n";
            purgeThing += "}\n";
            purgeThing += "}\n";
            purgeThing += "catch (Exception ex){\n";
            purgeThing += "throw ex;\n";
            purgeThing += "}\n";
            purgeThing += "return result;\n}\n";
            return comment + purgeThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object ID and passes it to the data
        /// access layer for accessing a record by primary key
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer retreive by PK method, in c#. </returns>
        private string genManagerPK()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Retreive_By_PK); string retreiveThing = "public " + name + " get" + name + "ByPrimaryKey(string " + name + "ID){\n";
            retreiveThing = retreiveThing + name + " result =null ;\n";
            retreiveThing += "try{\n";
            retreiveThing = retreiveThing + "result = _" + name.ToLower() + "Accessor.select" + name + "ByPrimaryKey(" + name + "ID);\n";
            retreiveThing += "if (result == null){\n";
            retreiveThing = retreiveThing + "throw new ApplicationException(\"Unable to retreive " + name + "\" );\n";
            retreiveThing += "}\n";
            retreiveThing += "}\n";
            retreiveThing += "catch (Exception ex){\n";
            retreiveThing += "throw ex;\n";
            retreiveThing += "}\n";
            retreiveThing += "return result;\n}\n";
            return comment + retreiveThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in no paramaters and passes a request to the data
        /// access layer for accessing all records for this <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer retreive by all method, in c#. </returns>
        private string genManagerAll()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_All_No_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Retreive_All_No_Param);
            string retreiveAll = comment + "\npublic List<" + name + "> get" + name + "ByAll(){\n";
            retreiveAll = retreiveAll + "return get" + name + "ByAll(0," + appData2.settings.page_size + ");\n}\n";
            comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_All_One_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Retreive_All_One_Param);
            retreiveAll = retreiveAll + comment + "\npublic List<" + name + "> get" + name + "ByAll(int offset){\n";
            retreiveAll = retreiveAll + "return get" + name + "ByAll(offset, " + appData2.settings.page_size + ");\n}\n";
            comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_All_Two_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Retreive_All_Two_Param);
            retreiveAll = retreiveAll + comment + "\npublic List<" + name + "> get" + name + "ByAll(int offset, int limit){\n";
            retreiveAll = retreiveAll + "List<" + name + "> result =new List<" + name + ">();\n";
            retreiveAll += "try{\n";
            retreiveAll = retreiveAll + "result = _" + name.ToLower() + "Accessor.selectAll" + name + "(offset,limit);\n";
            retreiveAll += "if (result.Count == 0){\n";
            retreiveAll = retreiveAll + "throw new ApplicationException(\"Unable to retreive " + name + "s\" );\n";
            retreiveAll += "}\n";
            retreiveAll += "}\n";
            retreiveAll += "catch (Exception ex){\n";
            retreiveAll += "throw ex;\n";
            retreiveAll += "}\n";
            retreiveAll += "return result;\n}\n";
            return retreiveAll;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object ID and passes it to the data
        /// access layer for accessing a record by foreign key
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing logic layer retreive by FK method, in c#.</returns>
        private string genManagerFK()
        {
            string getfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_By_FK_No_Param);
                    getfkThing = getfkThing + "\npublic List<" + name + "> get" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + "){\n" +
                        "return getAll" + name + "by" + fk_table + "(" + fk_name + "," + appData2.settings.page_size + ",0);" +
                        "\n}\n";
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_By_FK_One_Param);
                    getfkThing = getfkThing +
                    "\npublic List<" + name + "> getAll" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ",int offset){\n" +
                 "return getAll" + name + "by" + fk_table + "(" + fk_name + "," + appData2.settings.page_size + ",offset);" +
                "\n}\n";
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_Retreive_By_FK_Two_Param);
                    getfkThing = getfkThing + "public List<" + name + "> getAll" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ",int limit, int offset){\n";
                    getfkThing = getfkThing + "List<" + name + "> result =new List<" + name + ">();\n";
                    getfkThing += "try{\n";
                    getfkThing = getfkThing + "result = _" + name.ToLower() + "Accessor.select" + name + "by" + fk_table + "(" + fk_name + ",offset,limit);\n";
                    getfkThing += "if (result.Count == 0){\n";
                    getfkThing = getfkThing + "throw new ApplicationException(\"Unable to retreive " + name + "s\" );\n";
                    getfkThing += "}\n";
                    getfkThing += "}\n";
                    getfkThing += "catch (Exception ex){\n";
                    getfkThing += "throw ex;\n";
                    getfkThing += "}\n";
                    getfkThing += "return result;\n}\n";
                }
            }
            return getfkThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in two instances of an object (old and new) and passes them to the data
        /// access layer for updating
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing logic layer update, in c#. </returns>
        private string genManagerUpdate()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Update);
            string updateThing = "public int update" + name + "( " + name + " old" + name + ", " + name + " new" + name + "){\n";
            updateThing += "int result =0 ;\n";
            updateThing += "try{\n";
            updateThing = updateThing + "result = _" + name.ToLower() + "Accessor.update" + name + "(old" + name + ", new" + name + ");\n";
            updateThing += "if (result == 0){\n";
            updateThing = updateThing + "throw new ApplicationException(\"Unable to update " + name + "\" );\n";
            updateThing += "}\n";
            updateThing += "}\n";
            updateThing += "catch (Exception ex){\n";
            updateThing += "throw ex;\n";
            updateThing += "}\n";
            updateThing += "return result;\n}\n";
            return comment + updateThing;
        }
        /// Generates a c# data object with markup reflecting min and max length, etc.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing c# data object </returns>
        public String gen_DataObject()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpDataObject);
            string output = "public class " + name + "\n{\n";
            int count = 0;
            foreach (Column r in columns)
            {
                String DataAnnotationRequired = "";
                String DataAnnotationLength = "";
                if (r.nullable == 'n' || r.nullable == 'N')
                {
                    DataAnnotationRequired = "[Required(ErrorMessage = \"Please enter " + r.column_name.bracketStrip() + " \")]\n";
                }
                if (r.length != 0)
                {
                    DataAnnotationLength = "[StringLength(" + r.length + ")]\n";
                }
                String DataAnnotationDisplayName = "[Display(Name = \"" + r.column_name.bracketStrip() + "\")]\n";
                String add = "public " + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip() + "{ set; get; }\n";
                output = output + DataAnnotationDisplayName + DataAnnotationRequired + DataAnnotationLength + add;
                count++;
            }
            output += "\n}\n";
            if (foreign_keys.Count > 0)
            {
                output = output + "public class " + name + "VM: " + name + "\n{\n";
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
                output += "}\n";
            }
            return output;
        }
        //Not implemented
        public String gen_functions()
        {
            string x = " ";
            return x;
        }
        /// <summary>
        /// Generates a header for the data access layer accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer </returns>
        private string genAccessorClassHeader()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpAccessor);
            string header = "public class " + name + "Accessor : I" + name + "Accessor {\n";
            return header;
        }
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderA(string commandText)
        {
            //for update, insert, delete
            string output = "int rows = 0;\n"
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
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderB(string DataObject, string commandText)
        {
            //for single data object
            string output = DataObject + " output = new " + DataObject + "();\n"
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
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderC(string DataObject, string commandText)
        {
            //for list of data object
            string output = "List<" + DataObject + "> output = new " + "List<" + DataObject + ">();\n"
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
        /// <summary>
        /// Generates a footer for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>A string that is c# code for the footer of the data access layer methods </returns>
        private string genSPfooter(int mode)
        {
            string returntype = "output";
            if (mode == 2) { returntype = "rows"; }
            string output = " \ncatch (Exception ex)\n"
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
        /// <summary>
        /// Generates an method for the data access layer add method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for adding to the database </returns>
        private string genAccessorAdd()
        {
            string createThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Add);
            createThing += "\npublic int add" + name + "(" + name + " _" + name.ToLower();
            createThing += "){\n";
            createThing += genSPHeaderA("sp_insert_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            createThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            createThing += "try \n { \n //open the connection \n conn.Open();  ";
            createThing += "//execute the command and capture result\n";
            createThing += "rows = cmd.ExecuteNonQuery();\n}\n";
            //capture reuslts
            createThing += "";
            //cath block and onwards
            createThing += genSPfooter(2);
            return createThing;
        }
        /// <summary>
        /// Generates an method for the data access layer retreive by PK method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retreiving by PK from the database </returns>
        private string genAccessorRetreiveByKey()
        {
            string retreiveThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Retreive_By_PK);
            int count = 0;
            string comma = "";
            retreiveThing += "\npublic " + name + " select" + name + "ByPrimaryKey(";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (count > 0) { comma = "  "; }
                    String add = comma + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip();
                    retreiveThing += add;
                    count++;
                }
            }
            retreiveThing += "){\n";
            retreiveThing += genSPHeaderB(name, "sp_retreive_by_pk_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retreiveThing = retreiveThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            retreiveThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retreiveThing = retreiveThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            retreiveThing += "try \n { \n //open the connection \n conn.Open();  ";
            retreiveThing += "//execute the command and capture result\n";
            retreiveThing += "var reader = cmd.ExecuteReader();\n";
            //capture reuslts
            retreiveThing += "//process the results\n";
            retreiveThing += "if (reader.HasRows)\n if (reader.Read())\n{";
            count = 0;
            foreach (Column r in columns)
            {
                retreiveThing = getCSharpOrdinal(r);
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
                            retreiveThing = retreiveThing + "Output." + t.name + "= new" + t.name + "();\n";
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                retreiveThing += getCSharpOrdinal(t, r);
                                count++;
                            }
                        }
                    }
                }
            }
            retreiveThing += "\n}\n";
            retreiveThing = retreiveThing + "else \n { throw new ArgumentException(\"" + name + " not found\");\n}\n}";
            //cath block and onwards
            retreiveThing += genSPfooter(0);
            return retreiveThing;
        }
        /// <summary>
        /// Generates an method for the data access layer retreive by all method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retreving all from the database 
        private string genAccessorRetreiveAll()
        {
            string retreiveAllThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Retreive_All_Two_Param);
            retreiveAllThing += "\npublic List<" + name + "> selectAll" + name + "(int limit, int offset){\n";
            retreiveAllThing += genSPHeaderC(name, "sp_retreive_by_all_" + name);
            //no paramaters to set or add
            retreiveAllThing += "cmd.Parameters.Add(\"@limit SqlDbType.Int);\n";
            retreiveAllThing += "cmd.Parameters.Add(\"@offset SqlDbType.Int);\n";
            retreiveAllThing += "cmd.Parameters[\"@limit\"].Value = limit ;\n";
            retreiveAllThing += "cmd.Parameters[\"@offset\"].Value = offset ;\n";
            //excute the quuery
            retreiveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
            retreiveAllThing += "//execute the command and capture result\n";
            retreiveAllThing += "var reader = cmd.ExecuteReader();\n";
            //capture reuslts
            retreiveAllThing += "//process the results\n";
            retreiveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
            retreiveAllThing = retreiveAllThing + "var _" + name + "= new " + name + "();\n";
            int count = 0;
            foreach (Column r in columns)
            {
                retreiveAllThing += getCSharpOrdinal(r);
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
                            retreiveAllThing = retreiveAllThing + "Output." + t.name + "= new" + t.name + "();\n";
                            foreach (Column r in t.columns)
                            {
                                if (count > 0)
                                {
                                }
                                retreiveAllThing += getCSharpOrdinal(t, r);
                                count++;
                            }
                        }
                    }
                }
            }
            retreiveAllThing = retreiveAllThing + "output.Add(_" + name + ");";
            retreiveAllThing += "\n}\n}";
            //cath block and onwards
            retreiveAllThing += genSPfooter(0);
            return retreiveAllThing;
        }
        /// <summary>
        /// Generates an method for the data access layer retreive by FK method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retereiving by FK from the database
        private string genAccessorRetreivefk()
        {
            string retreiveAllThing = "";
            foreach (Column q in columns)
            {
                if (q.references != "")
                {
                    string[] parts = q.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    int count = 0;
                    retreiveAllThing += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Retreive_By_FK_Two_Param);
                    retreiveAllThing += "\npublic List<" + name + "> select" + name + "by" + fk_table + "(" + q.data_type.toCSharpDataType() + " " + fk_name + ",int limit, int offset){\n";
                    retreiveAllThing += genSPHeaderC(name, "sp_retreive_" + name + "_by_" + q.column_name.bracketStrip());
                    //no paramaters to set or add
                    retreiveAllThing += "cmd.Parameters.Add(\"@" + q.column_name.bracketStrip() + "\", SqlDbType." + q.data_type.bracketStrip().toSQLDBType(q.length) + ");\n";
                    retreiveAllThing += "cmd.Parameters.Add(\"@limit SqlDbType.Int);\n";
                    retreiveAllThing += "cmd.Parameters.Add(\"@offset SqlDbType.Int);\n";
                    retreiveAllThing += "cmd.Parameters[\"@" + q.column_name.bracketStrip() + "\"].Value = " + fk_name + " ;\n";
                    retreiveAllThing += "cmd.Parameters[\"@limit\"].Value = limit ;\n";
                    retreiveAllThing += "cmd.Parameters[\"@offset\"].Value = offset ;\n";
                    //excute the quuery
                    retreiveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
                    retreiveAllThing += "//execute the command and capture result\n";
                    retreiveAllThing += "var reader = cmd.ExecuteReader();\n";
                    //capture reuslts
                    retreiveAllThing += "//process the results\n";
                    retreiveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
                    retreiveAllThing = retreiveAllThing + "var _" + name + "= new " + name + "();\n";
                    count = 0;
                    foreach (Column r in columns)
                    {
                        retreiveAllThing += getCSharpOrdinal(r);
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
                                    retreiveAllThing = retreiveAllThing + "Output." + t.name + "= new" + t.name + "();\n";
                                    foreach (Column r in t.columns)
                                    {
                                        if (count > 0)
                                        {
                                        }
                                        retreiveAllThing += getCSharpOrdinal(t, r);
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                    retreiveAllThing = retreiveAllThing + "output.Add(_" + name + ");";
                    retreiveAllThing += "\n}\n}";
                    //cath block and onwards
                    retreiveAllThing += genSPfooter(0);
                }
            }
            return retreiveAllThing;
        }
        /// <summary>
        /// Generates an method for the data access layer update method method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for updating a record on the database
        private string genAccessorUpdate()
        {
            string updateThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Update);
            updateThing += "\npublic int update" + name + "(";
            updateThing = updateThing + name + " _old" + name + " , " + name + " _new" + name;
            updateThing += "){\n";
            updateThing += genSPHeaderA("sp_update_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters.Add(\"@old" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y' && r.increment == 0)
                {
                    updateThing = updateThing + "cmd.Parameters.Add(\"@new" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            updateThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters[\"@old" + r.column_name.bracketStrip() + "\"].Value = _old" + name + "." + r.column_name.bracketStrip() + ";\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    updateThing = updateThing + "cmd.Parameters[\"@new" + r.column_name.bracketStrip() + "\"].Value = _new" + name + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            updateThing += "try \n { \n //open the connection \n conn.Open();  ";
            updateThing += "//execute the command and capture result\n";
            updateThing += "rows = cmd.ExecuteNonQuery();\n";
            updateThing += "if (rows == 0) {\n //treat failed update as exception \n ";
            updateThing += "throw new ArgumentException(\"invalid values, update failed\");\n}\n}";
            //capture reuslts
            updateThing += "";
            //cath block and onwards
            updateThing += genSPfooter(2);
            return updateThing;
        }
        /// <summary>
        /// Generates an method for the data access layer delete method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for deleting from the database
        private string genAccessorDelete()
        {
            string deleteThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Delete);
            deleteThing += "\npublic int delete" + name + "(" + name + " _" + name.ToLower() + "){\n";
            deleteThing += genSPHeaderA("sp_delete_" + name);
            //add parameters bit
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            deleteThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing += "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing += "if (rows == 0){\n";
            deleteThing += "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing += genSPfooter(2);
            return deleteThing;
        }
        /// <summary>
        /// Generates an method for the data access layer undelete method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for undeleting from the database
        private string genAccessorUndelete()
        {
            string deleteThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Undelete);
            deleteThing = deleteThing + "\n public int undelete" + name + "(" + name + " _" + name.ToLower() + "){\n";
            deleteThing += genSPHeaderA("sp_undelete_" + name);
            //add parameters bit
            //add parameters
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            deleteThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing += "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing += "if (rows == 0){\n";
            deleteThing += "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing += genSPfooter(2);
            return deleteThing;
        }
        //Generates the get distinct for drop downs componoent of the accessor
        public string genAccessorDistinct()
        {
            string retreiveAllThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    retreiveAllThing += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Select_Distinct_For_Dropdown);
                    retreiveAllThing = retreiveAllThing + "public List<String> selectDistinct" + fk.referenceTable + "ForDropDown(){\n";
                    retreiveAllThing += genSPHeaderC("String", "sp_select_distinct_and_active_" + fk.referenceTable + "_for_dropdown");
                    //no paramaters to set or add
                    //excute the quuery
                    retreiveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
                    retreiveAllThing += "//execute the command and capture result\n";
                    retreiveAllThing += "var reader = cmd.ExecuteReader();\n";
                    //capture reuslts
                    retreiveAllThing += "//process the results\n";
                    retreiveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
                    retreiveAllThing = retreiveAllThing + "String _" + fk.referenceTable + "= reader.Get" + columns[0].data_type.toSqlReaderDataType() + "(0);\n";
                    retreiveAllThing = retreiveAllThing + "output.Add(_" + fk.referenceTable + ");";
                    retreiveAllThing += "\n}\n}";
                    //cath block and onwards
                    retreiveAllThing += genSPfooter(0);
                }
            }
            return retreiveAllThing;
        }
        /// <summary>
        /// Generates a rudamentary xaml window for creating records for  this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is xaml window for adding to this table
        public string genXAMLWindow()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_XAML_Window);
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
        /// <summary>
        /// Generates a rudamentary c# codebase for controlling the XAML window for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# to control the XAML window for this table
        public string genWindowCSharp()
        {
            string result = "";
            result += commentBox.genCommentBox(name, Component_Enum.CSharp_Window_Control);
            result += genStaticVariables();
            result += genConstructor();
            result += genWinLoad();
            result += genAddButton();
            result += genEditButton();
            return result;
        }
        //Generates the add button for the XAML window
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
            result += "else\n{\nif (validInputs())\n{\n";
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
            result += "try\n{\n";
            result = result + "bool result = _" + name.Substring(0, 1).ToLower() + "m.add" + name + "(new" + name + ");\n";
            result += "if (result)\n{\n MessageBox.Show(\"added!\");\n";
            result += "this.DialogResult=true;\n}\n";
            result += "else \n { throw new ApplicationException();\n}\n}\n";
            result += "catch (Exception ex)\n{\n MessageBox.Show(\"add failed\");";
            result += "this.DialogResult=false\n}\n}";
            result += "\nelse{\nMessageBox.Show(\"invalid inputs\");\n}\n}\n}";
            return result;
        }
        //Generates the edit button of the window
        private string genEditButton()
        {
            string result = "";
            return result;
        }
        //Generates the input validator for the window
        private string genValidInputs()
        {
            string result = "";
            return result;
        }
        //Generates the input constructor for the window
        private string genConstructor()
        {
            string result = "public " + name + "AddEditDelete(" + name + " " + name.Substring(0, 1).ToLower();
            result += ")\n{\n";
            result += " InitializeComponent();\n";
            result = result + "_" + name.ToLower() + "=" + name.Substring(0, 1).ToLower() + ";\n";
            result = result + "_" + name.Substring(0, 1).ToLower() + "m = new" + name + "Manager();\n}\n";
            return result;
        }
        //Generates the input winload events for the window
        private string genWinLoad()
        {
            string result = "";
            return result;
        }
        //Generates the get static variables method for the window
        private string genStaticVariables()
        {
            _ = "public " + name + " _" + name.ToLower() + "= null;\n";
            string result = "public " + name + "Manager+ _" + name.Substring(0, 1) + "m = null;\n";
            return result;
        }
        /// <summary>
        /// Generates a rudamentary Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is Java code for this object.
        public string genJavaModel()
        {
            string result = "";
            result += genJavaHeader();
            result += genJavaInstanceVariables();
            result += genJavaContructor();
            result += genJavaSetterAndGetter();
            result += genJavaFooter();
            return result;
        }
        /// <summary>
        /// Generates a the header for a rudamentary Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is the header Java code for this object.
        private string genJavaHeader()
        {
            string result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDataObject);
            result = result + "\n public class " + name + " {\n";
            return result;
        }
        /// <summary>
        /// Generates the instance variables for Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's instance variables.
        private string genJavaInstanceVariables()
        {
            string result = "";
            foreach (Column r in columns)
            {
                result = result + "private " + r.data_type.toJavaDataType() + " " + r.column_name + ";\n";
            }
            return result;
        }
        /// <summary>
        /// Generates the constructor for the Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's constructor.
        private string genJavaContructor()
        {
            string defaultConstructor = "\npublic " + name + "(){}\n";
            string ParamConsctructor = "\npublic " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                ParamConsctructor = ParamConsctructor + comma + r.data_type.toJavaDataType() + " " + r.column_name;
                comma = ", ";
            }
            ParamConsctructor += ") {\n";
            foreach (Column r in columns)
            {
                ParamConsctructor = ParamConsctructor + "\nthis." + r.column_name + " = " + r.column_name + ";";
            }
            ParamConsctructor += "\n}\n";
            string result = defaultConstructor + ParamConsctructor;
            return result;
        }
        /// <summary>
        /// Generates the setters and getters for the  instance variables for Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's setters and getters for this object's instance variables.
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
        /// <summary>
        /// Generates the footer for this Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's footer.
        private string genJavaFooter()
        {
            string result = "\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object.
        public string genJavaDAO()
        {
            string result = "";
            result += genJavaDAOHeader();        //works
            result += genJavaDAOCreate();       //returns ""
            result += genJavaDAORetreiveByKey(); // works
            result += genJavaDAORetreiveAll(); // wokring on it now
            result += genJavaDAORetriveActive(); // working on it now
            result += genJavaDAORetriveByFK();
            result += genJavaDAOUpdate(); //rturns ""
            result += genJavaDelete(); //returns ""
            result += genJavaunDelete(); //returns ""
            result += genJavaDAOFooter(); //work
            return result;
        }
        /// <summary>
        /// Generates the header for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object header.
        public string genJavaiDAO() {
            int count = 0;
            string comma = "";
            String result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result += "public interface i"+name+"DAO{\n";

            //add
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Add);
            result += "int add ("+name+" _"+name.ToLower()+ ") throws SQLException;\n";
            //get by pk
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_By_FK);
            result += name+" get"+name+"ByPrimaryKey("+name+" _"+name.ToLower()+") throws SQLException;\n";
            //get by fk
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_By_FK);
            result += "";
            //update
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Update);
            result += "int update("+name+" old"+name+", "+name+" new"+name+ ") throws SQLException;\n";
            //get all
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_All_);
            result += "List<"+name+"> getAll"+name+ "() throws SQLException;\n";
            // get active
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_All_);
            result += "List<" + name + "> getActive" + name + "() throws SQLException;\n";
            //delete
            count = 0;
            comma = "";
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Delete);
            result += "int delete" + name + "(";
            foreach (Column r in columns) {
                if (count > 0) { 
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y') {
                    result +=comma+" "+ r.data_type.toJavaDAODataType() +" "+ r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException;\n";

            //undelete
            count = 0;
            comma = "";
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Undelete);
            result += "int undelete" + name + "(";
            foreach (Column r in columns)
            {
                if (count > 0)
                {
                    comma = ",";
                }
                else {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException;\n";
            result += "}\n";
            return result;      
        
        }
        private string genJavaDAOHeader()
        {
            string result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result = result + "import com.beck.javaiii_kirkwood.personal_project.models." + name + ";\n" +
                "import java.sql.CallableStatement;\n" +
                "import java.sql.Connection;\n" +
                "import java.sql.ResultSet;\n" +
                "import java.sql.SQLException;\n" +
                "import java.util.ArrayList;\n" +
                "import java.util.List;\n" +
                "import java.time.LocalDate;\n" +
                "import static com.beck.javaiii_kirkwood.personal_project.idata.i"+name+"DAO;\n"+
            "import static com.beck.javaiii_kirkwood.personal_project.data.Database.getConnection;\n";
            result += commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result +=  "public class " + name + "DAO implements i"+name+"DAO{\n\n";
            return result;
        }
        /// <summary>
        /// Generates the footer for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object footer.
        private string genJavaDAOFooter()
        {
            string result = "\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the retreive by PK for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retreive by PK function.
        private string genJavaDAORetreiveByKey()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_By_PK);
            string nullValue = "";
            string comma = "";
            result = result + "public " + name + " get" + name + "ByPrimaryKey(" + name + " _" + name.ToLower() + ") throws SQLException{\n";
            result = result + name + " result = null;\n";
            result += "try(Connection connection = getConnection()) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_pk_" + name + "(?)}\")) {\n";
            for (int i = 0; i < columns.Count; i++)
                if (columns[i].primary_key == 'y' || columns[i].primary_key == 'Y')
                {
                    result = result + "statement.setString(1, _" + name.ToLower() + ".get" + columns[i].column_name.bracketStrip() + "().toString());\n";
                }
            result += "\ntry (ResultSet resultSet = statement.executeQuery()){";
            result += "\nif(resultSet.next()){";
            foreach (Column r in columns)
            {
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
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
                                result = result + r.data_type.toJavaDataType() + " " + t.name + "_" + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + t.name + "_" + r.column_name.bracketStrip() + "\");\n";
                                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                                {
                                    result = result + "if(resultSet.wasNull()){\n" + t.name + "_" + r.column_name.bracketStrip() + "=" + nullValue + ";}\n";
                                }
                            }
                        }
                    }
                }
            }
            result = result + "result = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result += "}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result += " throw new RuntimeException(e);\n}\n";
            result += "return result;\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the retreive by all for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retreive all.
        private string genJavaDAORetreiveAll()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_All_);
            string comma = "";
            result = result + "public List<" + name + "> getAll" + name + "() {\n";
            result = result + "return getAll" + name + "(" + appData2.settings.page_size + ",0);";
            result += "}\n";
            result = result + "public static List<" + name + "> getAll" + name + "(int pagesize) {\n";
            result = result + "return getAll" + name + "(pagesize" + ",0);";
            result += "}\n";
            result = result + "public static List<" + name + "> getAll" + name + "(int limit, int offset) {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result += "try (Connection connection = getConnection()) { \n";
            result += "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_all_" + name + "(?,?)}\")) {\n" +
                "statement.setInt(1,limit)\n;" +
                "statement.setInt(2,offset);\n";
            result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result += "while (resultSet.next()) {";
            foreach (Column r in columns)
            {
                string nullValue;
                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result += "}\n";
            result += "return result;}\n";
            return result;
        }
        /// <summary>
        /// Generates the retreive active for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retreive active function.
        public String genJavaDAORetriveActive()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_All_);
            string comma = "";
            result = result + "public List<" + name + "> getActive" + name + "() {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result += "try (Connection connection = getConnection()) { \n";
            result += "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_by_active_" + name + "()}\"))\n {";
            result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result += "while (resultSet.next()) {\n";
            foreach (Column r in columns)
            {
                string nullValue;
                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result += "}\n";
            result += "return result;}\n";
            return result;
        }
        /// <summary>
        /// Generates the retreive by FK for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retreive by FK.
        public String genJavaDAORetriveByFK()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Retreive_By_FK);
            string comma = "";
            foreach (Column s in columns)
            {
                if (s.references != "")
                {
                    string[] parts = s.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result = result + "public List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + ") {\n";
                    result = result + "return get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "," + appData2.settings.page_size + ",0);";
                    result += "}\n";
                    result = result + "public static List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "int pagesize) {\n";
                    result = result + "return get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + ",pagesize" + ",0);";
                    result += "}\n";
                    result = result + "public static List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "int pagesize,int offset) {\n";
                    result = result + "List<" + name + "> result = new ArrayList<>();\n";
                    result += "try (Connection connection = getConnection()) { \n";
                    result += "if (connection != null) {\n";
                    result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retreive_" + name + "_by" + fk_table + "(?,?,?)}\")) {\n" +
                        "statement.set" + s.data_type.toJavaDAODataType() + "(1," + fk_name + ")\n;" +
                        "statement.setInt(2,limit)\n;" +
                        "statement.setInt(3,offset);\n";
                    result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
                    result += "while (resultSet.next()) {";
                    foreach (Column r in columns)
                    {
                        string nullValue;
                        if (r.data_type.toJavaDataType().Equals("Integer"))
                        {
                            nullValue = "0";
                        }
                        else { nullValue = "\"\""; }
                        result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                        if (r.nullable == 'y' || r.nullable == 'Y')
                        {
                            result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                        }
                    }
                    result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
                    foreach (Column r in columns)
                    {
                        result = result + comma + " " + r.column_name.bracketStrip();
                        comma = ",";
                    }
                    result += ");";
                    result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
                    result += "} catch (SQLException e) {\n";
                    result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
                    result += "}\n";
                    result += "return result;\n}\n";
                }
            }
            return result;
        }
        /// <summary>
        /// Generates the update function for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object update function.
        private string genJavaDAOUpdate()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Update);
            result = result + "\n public int update(" + name + " old" + name + ", " + name + " new" + name + ") throws SQLException{\n";
            result += "int result = 0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection !=null){\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_update_" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    result = result + comma + "? ";
                    comma = ",";
                }
                else
                {
                    result = result + comma + "?,?";
                }
            }
            result += ")}\"))\n {\n";
            int count = 1;
            foreach (Column r in columns)
            {
                result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",old" + name + ".get" + r.column_name.bracketStrip() + "());\n";
                count++;
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",new" + name + ".get" + r.column_name.bracketStrip() + "());\n";
                    count++;
                }
            }
            result += "result=statement.executeUpdate();\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not update " + name + " . Try again later\");\n";
            result += "}\n}\n}\n return result;\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the delete method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object delete method.
        private string genJavaDelete()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Delete);
            int count = 0;
            string comma = "";
            result += "int delete" + name + "(";
            foreach (Column r in columns)
            {
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException{\n";
            result += "int rowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_Delete_" + name + "( ?)}\")){\n";
            result = result + "statement.setInt(1," + name.ToLower() + "ID);\n";
            result += "rowsAffected = statement.executeUpdate();\n";
            result += "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete " + name + ". Try again later\");\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete " + name + ". Try again later\");\n";
            result += "}\n";
            result += "return rowsAffected;\n";
            result += "}\n";
            result += "";
            return result;
        }
        /// <summary>
        /// Generates the undelete method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object undelete method.
        private string genJavaunDelete()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Undelete);
            int count = 0;
            string comma = "";
            result += "int undelete" + name + "(";
            foreach (Column r in columns)
            {
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException{\n";
            result += "int rowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_unDelete_" + name + "( ?)}\")){\n";
            result = result + "statement.setInt(1," + name.ToLower() + "ID);\n";
            result += "rowsAffected = statement.executeUpdate();\n";
            result += "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result += "}\n";
            result += "return rowsAffected;\n";
            result += "}\n";
            result += "";
            return result;
        }
        /// <summary>
        /// Generates the create method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object create method.
        private string genJavaDAOCreate()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Add);
            string comma = "";
            result = result + "public int add(" + name + " _" + name.ToLower() + ") {\n";
            result += "int numRowsAffected=0;";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_insert_" + name + "(";
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + comma + " ?";
                    comma = ",";
                }
            }
            result += ")}\")){\n";
            int count = 1;
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",_" + name.ToLower() + ".get" + r.column_name.bracketStrip() + "());\n";
                    count++;
                }
            }
            result += "numRowsAffected = statement.executeUpdate();\n";
            result += "if (numRowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result += "} \n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result += "return numRowsAffected;\n}";
            result += "\n";
            return result;
        }
        /// <summary>
        /// Generates the batch command to load the <see cref="database"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  a batch command to load this <see cref="database"/>
        public string getBatch()
        {
            string result = "";
            result = result + "sqlcmd -S localhost -E -i " + name + ".sql\n";
            result += "ECHO .\n";
            return result;
        }
        /// <summary>
        /// Generates the create Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated create servlet.
        public string genCreateServelet()
        {
            string result = "";
            result += importStatements(name);
            //do get
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Add);
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
            result += initMethod();
            result += "\n @Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
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
            result += "}\n";
            //this only creates the doPost method
            result += "\n";
            //gen header
            result += "@Override\n";
            result += "  protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
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
            result += "Map<String, String> results = new HashMap<>();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result = result + "results.put(\"" + r.column_name + "\",_" + r.column_name + ");\n";
                }
            }
            //generate an object, and set errors
            result = result + name + " " + name.ToLower() + " = new " + name + "();\n";
            result += "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.default_value == "")
                {
                    string errorname = name.ToLower() + r.column_name + "error";
                    result += "try {\n";
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
                    result += "} catch(IllegalArgumentException e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result += "errors++;\n";
                    result += "}\n";
                }
            }
            //add it to the databsae, maybe?
            result += "int result=0;\n";
            result += "if (errors==0){\n";
            result = result + "try{\nresult=" + name.ToLower() + "DAO.add(" + name.ToLower() + ");\n}";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",\"Database Error\");\n";
            result += "}\n";
            result += "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Added\");\n";
            result = result + "resp.sendRedirect(\"all-" + name + "s\");\n";
            result += "return;\n";
            result += "} else {\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Not Added\");\n";
            result += "\n}\n";
            //set db message
            result += "}\n";
            //send it back
            result += "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Create a " + name + " \");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Add" + name + ".jsp\").forward(req, resp);\n";
            result += "\n}\n}\n";
            //get_buttons
            //get_footer
            return result;
        }
        /// <summary>
        /// Generates the View All JSP for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated View all JSP.</returns>
        public string genviewAllJSP()
        {
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_ViewAll);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result += "<div class=\"row\">\n";
            result += "<div class=\"col-12\">\n";
            result = result + "<h1>All Roller " + name + "s</h1>\n";
            result = result + "<p>There ${" + name + "s.size() eq 1 ? \"is\" : \"are\"}&nbsp;${" + name + "s.size()} " + name + "${" + name + "s.size() ne 1 ? \"s\" : \"\"}</p>\n";
            result = result + "Add " + name + "   <a href=\"add" + name + "\">Add</a>\n";
            result = result + "<c:if test=\"${" + name + "s.size() > 0}\">\n";
            result += "<div class=\"table-responsive\">";
            result += "<table class=\"table table-bordered\">\n";
            result += "<thead>\n";
            result += "<tr>\n";
            foreach (Column r in columns)
            {
                result = result + "<th scope=\"col\">" + r.column_name + "</th>\n";
            }
            result += "<th scope=\"col\">Edit</th>\n";
            result += "<th scope=\"col\">Delete</th>\n";
            result += "</tr>\n";
            result += "</thead>\n";
            result += "<tbody>\n";
            result = result + "<c:forEach items=\"${" + name + "s}\" var=\"" + name.ToLower() + "\">\n";
            result += "<tr>\n";
            foreach (Column r in columns)
            {
                //https://stackoverflow.com/questions/21755757/first-character-of-string-lowercase-c-sharp
                if (!r.data_type.ToLower().Equals("bit"))
                {
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=view\">${fn:escapeXml(" + name.ToLower() + "." + name.ToLower() + "_ID)}</a></td>";
                    }
                    else
                    {
                        result = result + "<td>${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</td>\n";
                    }
                }
                else
                {
                    result = result + "<td><input type=\"checkbox\" disabled <c:if test=\"${" + name.ToLower() + ".is_active}\">checked</c:if>></td>\n";
                }
            }
            result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=edit\" > Edit </a></td>\n";
            result = result + "<td><a href = \"delete" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}" +
                "&mode=" +
                "<c:choose>" +
                "<c:when test=\"${" + name.ToLower() + ".is_active}\">0</c:when>\n" +
                "\t\t\t\t\t\t<c:otherwise>1</c:otherwise>\n" +
                "\t\t\t\t\t\t</c:choose>\">\n" +
                "<c:if test=\"${!" + name.ToLower() + ".is_active}\">un</c:if>Delete </a></td> \n";
            result += "</tr>\n";
            result += "</c:forEach>\n";
            result += "</tbody>\n";
            result += "</table>\n";
            result += "</div>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</div>\n";
            result += "</div>\n";
            result += "</main>\n";
            result += "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";
            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer
            return result;
        }
        //Generates the Java Create JSP
        public string genCreateJSP()
        {
            int rowcount = 0;
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_Add);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/add" + name + "\" id = \"add" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.increment == 0)
                    {
                        if (r.foreign_keys.Count < 1 || r.foreign_keys[i] == "")
                        {
                            string inputType = "text";
                            if (r.data_type == "datetime") { inputType = "date"; }
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result = result + "<!-- " + r.column_name + " -->\n";
                            result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result += "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
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
                            result += "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                            result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                            result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\">${" + parts[0] + ".name}   </option>\n";
                            result += "</c:forEach>\n";
                            result += "</select>\n";
                            result += "";
                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                            rowcount++;
                            i++;
                        }
                    }
                }
            }
            //get_buttons
            result += "<div class=\"align-items-center mt-0\">\n";
            result += "<div class=\"d-grid\">";
            result = result + "<button class=\"btn btn-orange mb-0\" type=\"submit\">Create " + name + "  </button></div>\n";
            result += "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result += "<p>${results.dbStatus}</p>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</form>\n";
            result += "</div>\n";
            //get_footer
            result += "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";
            return result;
        }
        /// <summary>
        /// Generates the View all Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated view all servlet.</returns>
        public string genviewAllServlet()
        {
            //this only creates the doGet method
            string result = commentBox.genCommentBox(name, Component_Enum.Java_Servlet_ViewAll);
            //gen header
            result += importStatements(name);
            result +=  "@WebServlet(\"/all-" + name + "s\")\n";
            result +=  "public class All" + name + "sServlet extends HttpServlet {";
            result += initMethod();
            result += "@Override\n";
            result += "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "List<" + name + "> " + name.ToLower() + "s = null;\n";
            result += "\n";
            result = result + name.ToLower() + "s =" + name.ToLower() + "DAO.getAll" + name + "();\n";
            result += "\n";
            result = result + "req.setAttribute(\"" + name + "s\", " + name.ToLower() + "s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All " + name + "s\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/all-" + name + "s.jsp\").forward(req,resp);\n";
            result += "\n}\n}";
            //header comment
            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer
            return result;
        }
        /// <summary>
        /// Generates the delete Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated delete servlet.</returns>
        public string genDeleteServlet()
        {
            string result = commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Delete);
            result += importStatements(name);
            result = result + "@WebServlet(\"/delete" + name.ToLower() + "\")";
            result = result + "public class Delete" + name + "Servlet extends HttpServlet {\n";
            result += initMethod();
            result += "@Override\n";
            result += "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += "Map<String, String> results = new HashMap<>();\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Delete " + name + "\");\n";
            result = result + "int " + name + "ID = Integer.valueOf(req.getParameter(\"" + name.ToLower() + "id\"));\n";
            result += "int mode = Integer.valueOf(req.getParameter(\"mode\"));\n";
            result += "int result = 0;\n";
            result += "if (mode==0){\n";
            result += "try{\n";
            result = result + "result = " + name.ToLower() + "DAO.delete" + name + "(" + name + "ID);\n";
            result += "}\n";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "}\n";
            result += "}\n";
            result += "else {\n";
            result += "try{\n";
            result = result + "result = " + name.ToLower() + "DAO.undelete" + name + "(" + name + "ID);\n";
            result += "}\n";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "}\n";
            result += "}\n";
            result = result + "List<" + name + "> " + name.ToLower() + "s = null;\n";
            result = result + name.ToLower() + "s = " + name.ToLower() + "DAO.getAll" + name + "();\n";
            result += "req.setAttribute(\"results\",results);\n";
            result = result + "req.setAttribute(\"" + name + "s\", " + name.ToLower() + "s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All " + name + "\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/all-" + name + "s.jsp\").forward(req, resp);\n";
            result += "}\n";
            result += "}\n";
            return result;
        }
        /// <summary>
        /// Generates the edit Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated edit servlet.</returns>
        public string genViewEditServlet()
        {
            //do get
            string result = "";
            result += importStatements(name);
            //do get
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_ViewEdit);
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
            result += initMethod();
            result += "\n @Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "String mode = req.getParameter(\"mode\");\n";
            result = result + "int primaryKey = Integer.parseInt(req.getParameter(\"" + name.ToLower() + "id\"))\n";
            result = result + name + " " + name.ToLower() + "= new " + name + "();\n";
            result = result + name.ToLower() + ".set" + name + "_ID(primaryKey);\n";
            result += "try{\n";
            result = result + name.ToLower() + "=" + name.ToLower() + "DAO.get" + name + "ByPrimaryKey(" + name.ToLower() + ");\n";
            result += "} catch (SQLException e) {\n";
            result += "req.setAttribute(\"dbStatus\",e.getMessage());\n";
            result += "}\n";
            result += "HttpSession session = req.getSession();\r\n";
            result = result + "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "req.setAttribute(\"mode\",mode);\n";
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
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
            result += "}\n";
            result += " @Override\n";
            result += "protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "Map<String, String> results = new HashMap<>();\n";
            result += "String mode = req.getParameter(\"mode\");\n";
            result += "req.setAttribute(\"mode\",mode);\n";
            result += "//to set the drop downs\n";
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
            result = result + "//to get the old " + name + "\n";
            result += "HttpSession session = req.getSession();\n";
            result = result + name + " _old" + name + "= (" + name + ")session.getAttribute(\"" + name.ToLower() + "\");\n";
            result += "//to get the new event's info\n";
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
            result += "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string errorname = name.ToLower() + r.column_name + "error";
                    result += "try {\n";
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
                    result += "} catch(IllegalArgumentException e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result += "errors++;\n";
                    result += "}\n";
                }
            }
            result = result + "_new" + name + ".setis_active(true);\n";
            result += "//to update the database\n";
            result += "int result=0;\n";
            result += "if (errors==0){\n";
            result += "try{\n";
            result = result + "result=" + name.ToLower() + "DAO.update(_old" + name + ",_new" + name + ");\n";
            result += "}catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",\"Database Error\");\n";
            result += "}\n";
            result += "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " updated\");\n";
            result = result + "resp.sendRedirect(\"all-" + name + "s\");\n";
            result += "return;\n";
            result += "} else {\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Not Updated\");\n";
            result += "}\n}\n";
            result += "//standard\n";
            result += "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Edit a " + name + " \");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/personal-project/Edit" + name + ".jsp\").forward(req, resp);\n";
            result += "}\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the View Single/Edit JSP for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated View Single/Edit JSP.</returns>
        public string genViewEditJSP()
        {
            int rowcount = 0;
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_ViewEdit);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/edit" + name + "\" id = \"edit" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                    {
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<h2>" + r.column_name + "  :  \n";
                        result = result + " ${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</h2>\n";
                        result += "</div>\n";
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
                        result += "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result += "</c:if>\n";
                        result += "</div>\n";
                        result += "</div>\n";
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
                        result += "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\"  <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                        result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\"" +
                        "<c:if test=\"${" + name.ToLower() + "." + parts[1].firstCharLower() + " eq " + parts[0] + "." + parts[1].firstCharLower() + "}\"> selected </c:if>>${" + parts[0] + ".name}   </option>\n";
                        result += "</c:forEach>\n";
                        result += "</select>\n";
                        result += "";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result += "</c:if>\n";
                        result += "</div>\n";
                        result += "</div>\n";
                        rowcount++;
                        i++;
                    }
                }
            }
            //get_buttons
            result += "<div class=\"align-items-center mt-0\">\n";
            result += "<div class=\"d-grid\">";
            result = result + "<button class=\"btn btn-orange mb-0\" type=\"submit\">Edit " + name + " </button></div>\n";
            result += "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result += "<p>${results.dbStatus}</p>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</form>\n";
            result += "</div>\n";
            //get_footer
            result += "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";
            return result;
        }
        /// <summary>
        /// Generates the index JSP with links to each tables view all JSP for this <see cref="database"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  JSP for a project homepage.</returns>
        public string genIndexJSP()
        {
            string result = "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            result += "<div class=\"table-responsive\"><table class=\"table table-bordered\">\n";
            result += "<thead>\n";
            result += "<tr>\n";
            result += "<th scope=\"col\">Table</th>\n";
            result += " <th scope=\"col\">Action</th>\n";
            result += "</tr>\n";
            result += "</thead>\n";
            result += "<tbody>\n";
            foreach (table t in data_tables.all_tables)
            {
                result = result + "<tr><td>View all " + t.name + "</td><td><a href=\"all-" + t.name + "s\"> View </a> </td></tr>\n";
            }
            result += "</tbody>\n";
            result += "</table>\n";
            result += "</div>\n";
            result += "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";
            return result;
        }
        /// <summary>
        /// Generates the standard important statements for the Java Servlet. 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for standard import statements.</returns>
        private string importStatements(string objectname)
        {
            string result = "\n";
            result = result + "import com.beck.javaiii_kirkwood.personal_project.data." + name + "DAO;\n";
            result = result + "import com.beck.javaiii_kirkwood.personal_project.models." + name + ";\n";
            result += "import com.beck.javaiii_kirkwood.personal_project.models.User;\n";
            result += "import com.beck.javaiii_kirkwood.personal_project.iData.i"+ objectname + "DAO;\n";
            result += "import jakarta.servlet.ServletException;\n";
            result += "import jakarta.servlet.annotation.WebServlet;\n";
            result += "import jakarta.servlet.http.HttpServlet;\n";
            result += "import jakarta.servlet.http.HttpServletRequest;\n";
            result += "import jakarta.servlet.http.HttpServletResponse;\n";
            result += "import jakarta.servlet.http.HttpSession;\n";
            result += "import java.io.IOException;\n";
            result += "import java.util.HashMap;\n";
            result += "import java.util.List;\n";
            result += "import java.util.Map;\n";
            
            return result;
        }
        /// <summary>
        /// Generates the standard access level control statements for the Java Servlet. 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for standard access level control on the servlet. </returns>
        private string privLevelStatement()
        {
            string result = "\n//To restrict this page based on privilege level\n";
            result += "int PRIVILEGE_NEEDED = 0;\n";
            result += "HttpSession session = req.getSession();\n";
            result += "User user = (User)session.getAttribute(\"User\");\n";
            result += "if (user==null||user.getPrivilege_ID()<PRIVILEGE_NEEDED){\n";
            result += "resp.sendError(HttpServletResponse.SC_FORBIDDEN);\n";
            result += "return;\n";
            result += "}\n";
            result += "\n";
            return result;
        }
        /// <summary>
        /// Generates the retreive line for C# data access layers, using only a column name
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// <returns>A string that is  C# code for retreiveing a particular </returns>
        private string getCSharpOrdinal(Column r)
        {
            String retreiveThing = "";
            if (!r.nullable.Equals('n') || !r.nullable.Equals("N"))
            {
                retreiveThing = retreiveThing + "output." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            else
            {
                retreiveThing = retreiveThing + "output." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\")) ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            return retreiveThing;
        }
        /// <summary>
        /// Generates the retreive line for C# data access layers, using  a column name and a table name
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// /// <param name="t"> The table you are requesting data from </param>
        /// <returns>A string that is  C# code for retreiveing a particular </returns>
        private string getCSharpOrdinal(table t, Column r)
        {
            String retreiveThing = "";
            if (!r.nullable.Equals('n') || !r.nullable.Equals("N"))
            {
                retreiveThing = retreiveThing + "output." + t.name + "." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            else
            {
                retreiveThing = retreiveThing + "output." + t.name + "." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\")) ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            return retreiveThing;
        }
        /// <summary>
        /// Generates the rudamentary jQuery validation for this <see cref="table"/>.
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// /// <param name="t"> The table you are requesting data from </param>
        /// <returns>A string that is jQuery code for rudamentary data validation for this table.</returns>
        public string jQueryValidation()
        {
            //to start the js file
            string result = "$(document).ready(function() {\n";
            result += "";
            result += "";
            result += "";
            //to make the submit button
            result += "";
            result += "";
            //to assign variables
            //to loop through each input field
            foreach (Column r in columns)
            {
                if (!r.identity.Equals("Y") && !r.identity.Equals("y"))
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    string jsname = r.column_name + "_input";
                    result += "// to clearn the field, then set event listener for validating the input for " + r.column_name + "\n";
                    result += "var " + jsname + "= document.getElementById(\"" + fieldname + "\");\n";
                    result += jsname + ".value=\'\';\n";
                    result += jsname + ".addEventListener(\'keyup',function(){\n";
                    //if for numeric check
                    if (r.length == 0)
                    {
                        result += "if (" + jsname + ".value!=\"\"&& $.isNumeric(" + jsname + ".value)){\n";
                    }
                    //if for varchar check
                    else
                    {
                        result += "if (" + jsname + ".value!=\"\"&& " + jsname + ".value.length>1 && " + jsname + ".value.length<=" + r.length + ")){\n";
                    }
                    //good input
                    result += "$(" + jsname + ").addClass(\"ui-state-active\");\n";
                    result += "$(" + jsname + ").removeClass(\"ui-state-error\");\n";
                    //bad input
                    result += "}\n else {\n";
                    result += "$(" + jsname + ").removeClass(\"ui-state-active\");\n";
                    result += "$(" + jsname + ").addClass(\"ui-state-error\");\n";
                    result += "}\n}\n);\n";
                }
            }
            // to end the js file
            result += "\n}\n";
            return result;
        }
        private string initMethod() {
            string result;
            result = "private i" + name + "DAO " + name.ToLower() + "DAO;\n";
            result += "@Override\n";
            result += "private void init() throws ServletException{\n";
            result += name.ToLower() + "DAO = new " + name + "DAO();\n";
            result += "}\n";
            return result;

        }
    }
}
