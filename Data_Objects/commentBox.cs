using appData2;
using System;
namespace Data_Objects
{
    public class commentBox
    {
        public static String GenXMLClassComment(table table, XMLClassType classtype)
        {

            String name = "* @ author Jonathan Beck\n";
            String version = "* @ version 1.0\n";
            String since = "* @ since 1.0\n";
            switch (classtype)
            {
                case XMLClassType.CSharpIAccessor:
                    break;
                case XMLClassType.CSharpIManager:
                    break;
                case XMLClassType.CSharpManager:
                    break;
                case XMLClassType.CSharpAccessor:
                    break;
                case XMLClassType.CSharpDataObject:
                    break;
                case XMLClassType.JavaDAO:
                    break;
                case XMLClassType.JavaDataObject:
                    break;
                default:
                    break;
            }

            return "/**\n" + name + version + since + "*/\n";
        }
        public static String GenXMLMethodComment(table table, XML_Method_Type method)
        {
            string result = "";
            String summary = "";
            String name = "///<para />Created By Jonathan Beck " + DateTime.Now.ToString() + "<para />";
            String Params = "";
            String returns = "";
            switch (method)
            {
                case XML_Method_Type.CSharp_Manager_Add:
                    summary = "Logic layer add method for " + table.name + " objects";
                    Params = "\n///<param name=\"_" + table.name + "\">the <see cref=\"" + table.name + "\"/> to be added</param>";
                    returns = "bool. True if added, False otherwise.";
                    break;
                case XML_Method_Type.CSharp_Manager_Delete:
                    summary = "Logic layer delete method for " + table.name + " objects";
                    Params = "\n///<param name=\"_" + table.name + "\">the <see cref=\"" + table.name + "\"/> to be deleted</param>";
                    returns = "Int, number of records deleted";
                    break;
                case XML_Method_Type.CSharp_Manager_Undelete:
                    summary = "Logic layer undelete method for " + table.name + " objects";
                    Params = "\n///<param name=\"_" + table.name + "\">the <see cref=\"" + table.name + "\"/> to be restored</param>";
                    returns = "Int, number of records restored";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_By_PK:
                    summary = "Logic layer retreive by pk method for " + table.name + " objects";
                    foreach (Column r in table.columns)
                    {
                        if (r.primary_key == 'y' || r.primary_key == 'Y')
                        {
                            Params += "\n///<param name=\"" + r.column_name + "\">the <see cref=\"" + r.data_type.toCSharpDataType() + "\"/> that is the primary key of this record</param>";
                        }
                    }
                    returns = "<see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_All_No_Param:
                    summary = "Logic layer retreive all method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_All_One_Param:
                    summary = "Logic layer retreive all method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_All_Two_Param:
                    summary = "Logic layer retreive all method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Update:
                    summary = "Logic layer update method for " + table.name + " objects";
                    Params = "///<param name=\"old" + table.name.ToLower() + "\">the <see cref=\"" + table.name + "\"/> to be updated</param>";
                    Params += "\n///<param name=new\"" + table.name.ToLower() + "\">the <see cref=\"" + table.name + "\"/> the updated version</param>";
                    returns = "Int, number of records updated";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_By_FK_No_Param:
                    summary = "Logic layer retrevei by fk method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_By_FK_One_Param:
                    summary = "Logic layer retrevei by fk method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Manager_Retreive_By_FK_Two_Param:
                    summary = "Logic layer retrevei by fk method for" + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Accessor_Add:
                    summary = "Data Access layer Add method for " + table.name + " objects";
                    Params += "\n///<param name=\"_" + table.name.ToLower() + "\">the <see cref=\"" + table.name + "\"/> to be added</param>";
                    returns = "Int, number of records added";
                    break;
                case XML_Method_Type.CSharp_Accessor_Delete:
                    summary = "Data Access layer Delete method for " + table.name + " objects";
                    foreach (Column r in table.columns)
                    {
                        if (r.primary_key == 'y' || r.primary_key == 'Y')
                        {
                            Params += "\n///<param name=\"" + r.column_name + "\">the <see cref=\"" + r.data_type.toCSharpDataType() + "\"/> that is the primary key of this record</param>";
                        }
                    }
                    returns = "Int, number of records deleted";
                    break;
                case XML_Method_Type.CSharp_Accessor_Undelete:
                    summary = "Data Access layer undelete method for " + table.name + " objects";
                    foreach (Column r in table.columns)
                    {
                        if (r.primary_key == 'y' || r.primary_key == 'Y')
                        {
                            Params += "\n///<param name=\"" + r.column_name + "\">the <see cref=\"" + r.data_type.toCSharpDataType() + "\"/> that is the primary key of this record</param>";
                        }
                    }
                    returns = "Int, number of records restored";
                    break;
                case XML_Method_Type.CSharp_Accessor_Retreive_By_PK:
                    summary = "Data Access layer retreive by PK method for " + table.name + " objects";
                    foreach (Column r in table.columns)
                    {
                        if (r.primary_key == 'y' || r.primary_key == 'Y')
                        {
                            Params += "\n///<param name=\"" + r.column_name + "\">the <see cref=\"" + r.data_type.toCSharpDataType() + "\"/> that is the primary key of this record</param>";
                        }
                    }
                    returns = "<see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Accessor_Retreive_All_Two_Param:
                    summary = "Data Access layer Retreive All method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
                case XML_Method_Type.CSharp_Accessor_Update:
                    summary = "Data Access layer Update method for " + table.name + " objects";
                    Params = "\n///<param name=\"_old" + table.name + "\">the <see cref=\"" + table.name + "\"/> to be updated</param>";
                    Params += "\n///<param name=\"new" + table.name + "\">the <see cref=\"" + table.name + "\"/> the updated version</param>";
                    returns = "Int, number of records updated";
                    break;
                case XML_Method_Type.CSharp_Accessor_Retreive_By_FK_Two_Param:
                    summary = "Data Access layer Retreive by FK method for " + table.name + " objects";
                    Params = "";
                    returns = "List of <see cref=\"" + table.name + "\">";
                    break;
            }
            summary = "\n///<summary>\n///" + summary + "\n" + name + "\n///</summary>";
            returns = "\n///<returns>\n///" + returns + "\n///</returns>\n";
            result = result + summary + Params + returns;
            return result;
        }
        public static String GenJavaDocMethodComment(table table, JavaDoc_Method_Type method)
        {
            string result = "";
            String summary = "";
            String header = "/**";
            String name = "\n* @author Jonathan Beck";
            String Params = "";
            String returns = "";
            switch (method)
            {
                case JavaDoc_Method_Type.Java_DAO_Add:
                    summary = "\n* DAO Method to add " + table.name + " objects";
                    Params = "\n* @param " + table.name + " the " + table.name + " to be added";
                    returns = "\n* @return number of records added";
                    break;
                case JavaDoc_Method_Type.Java_DAO_Delete:
                    summary = "\n* DAO Method to delete " + table.name + " objects";
                    Params = "\n* @param " + table.name + " the " + table.name + " to be deleted";
                    returns = "\n* @return number of records deleted";
                    break;
                case JavaDoc_Method_Type.Java_DAO_Undelete:
                    summary = "\n* DAO Method to undelete " + table.name + " objects";
                    Params = "\n* @param " + table.name + " the " + table.name + " to be undeleted";
                    returns = "\n* @return number of records undeleted";
                    break;
                case JavaDoc_Method_Type.Java_DAO_Retreive_By_FK:
                    summary = "\n* DAO Method to retreive by Foreign Key " + table.name + " objects";
                    Params = "";
                    returns = "\n* @return List of " + table.name;
                    break;
                case JavaDoc_Method_Type.Java_DAO_Retreive_All_:
                    summary = "\n* DAO Method to retreive all " + table.name + " objects";
                    Params = "";
                    returns = "\n* @return List of " + table.name;
                    break;
                case JavaDoc_Method_Type.Java_DAO_Retreive_By_PK:
                    summary = "\n* DAO Method to retreive by ID " + table.name + " objects";
                    Params = "\n* @param " + table.name + " the " + table.name + " to be retreived";
                    returns = "\n* @return List of " + table.name;
                    break;
                case JavaDoc_Method_Type.Java_DAO_Update:
                    summary = "\n* DAO Method to update " + table.name + " objects";
                    Params = "\n* @param old" + table.name + " the " + table.name + " to be updated";
                    Params += "\n* @param new" + table.name + " the updated version of the " + table.name;
                    returns = "\n* @return number of records updated";
                    break;
                case JavaDoc_Method_Type.Java_DAO_Get_Distinct:
                    summary = "\n* DAO Method to select distinct " + table.name + " for dropdowns";
                    Params = "";

                    returns = "\n* @return list of string";
                    break;
                default:
                    break;
            }
            result = result + header + summary + Params + returns + name + "\n */\n";
            return result;
        }
        //creates various commeent boxes based on
        //table name and function type.
        public static String genCommentBox(String table, Component_Enum type)
        {
            String start_stars = "/******************\n";
            String end_stars = "\n***************/\n";
            String middle = " ";
            if (type.Equals(Component_Enum.SQL_table))
            {
                middle = "Create the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Audit_Table))
            {
                middle = "Create the " + table + " Audit table";
            }
            if (type.Equals(Component_Enum.SQL_Update))
            {
                middle = "Create the update script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Delete))
            {
                middle = "Create the delete script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Retreive_By_PK))
            {
                middle = "Create the retreive by key script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Retreive_By_All))
            {
                middle = "Create the retreive by all script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Insert))
            {
                middle = "Create the insert script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Insert_Trigger))
            {
                middle = "Create the insert trigger script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Update_Trigger))
            {
                middle = "Create the update trigger script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Delete_Trigger))
            {
                middle = "Create the delete trigger script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_IAccessor))
            {
                middle = "Create the I Accessor for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_IManager))
            {
                middle = "create the I manager for the " + table + " table;";
            }
            if (type.Equals(Component_Enum.CSharp_Accessor))
            {
                middle = "create the Accessor for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager))
            {
                middle = "create the manager for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_DataObject))
            {
                middle = "create the data object for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_XAML_Window))
            {
                middle = "create the XAML Window for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Window_Control))
            {
                middle = "create the c# Window for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Sample_Data))
            {
                middle = "Insert Sample Data For The  " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_JSP_Add))
            {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For adding to The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_JSP_ViewAll))
            {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For Viewing All of The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_Servlet_Add))
            {
                middle = "Create the Servlet  For adding to The  " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_Servlet_ViewAll))
            {
                middle = "Create the Servlet  For Viewing all of the  " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Undelete))
            {
                middle = "Create the undelete script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Retreive_Active))
            {
                middle = "Create The Retreive_By_Active script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_Servlet_Delete))
            {
                middle = "Create the Servlet For Deleteing from the " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_JSP_ViewEdit))
            {
                start_stars = "<%--************\n";
                middle = "Create the JSP For Viuw/Edit from the " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_Servlet_ViewEdit))
            {
                middle = "Create the Servlet Viuw/Edit from the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Select_Distinct))
            {
                middle = "Create the Select Disctint for Drop downs from the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager))
            {
                middle = "Create the Logic Layer Manager for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Add))
            {
                middle = "Create the Logic Layer Add method for the  " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Delete))
            {
                middle = "Create the Logic Layer Delete method for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Undelete))
            {
                middle = "Create the Logic Layer Undelete method for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_PK))
            {
                middle = "Create the Logic Layer Retreive by Primary Key method for the" + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_No_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with no paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_One_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with one paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_Two_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with two paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Update))
            {
                middle = "Create the Logic Layer Update method for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_No_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with no paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_One_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with one paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_Two_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with two paramater supplied.";
            }
            middle += "\n Created By Jonathan Beck " + DateTime.Now.ToShortDateString();
            string PrintStatement = "print '' Print '***" + middle + "***' \n go \n";
            string full_comment_box = start_stars + middle + end_stars;
            if (settings.TSQLMode) { full_comment_box = full_comment_box + PrintStatement; }
            return full_comment_box;
        }


        public static String genJavaSetterJavaDoc(String tablename,Column r)
        {
            string result = "";
            result = "\n";
            result += "/**\n";
            result += "* <p> Sets the " + r.column_name + " of the associated " + tablename + " object </p>\n";
            result += "* @param " + r.column_name + " the " + r.column_name.ToLower() + " of the " + tablename.ToLower() + ",\n";
            if (r.length != 0)
            {
                result += "* throws IllegalArgumentException if " + r.column_name + " under 3 characters or longer than " + r.length + " characters\n";
            }

            else if (r.data_type.ToLower().Equals("int"))
            {
                result += "* throws IllegalArgumentException if " + r.column_name + " is negative or greater than 10,000\n";
            }
            else
            {
                result += "* throws IllegalArgumentException if " + r.column_name + " is outside of a logical range\n";
            }
            result += "*/\n";
            return result;

        }
        public static String genJavaGetterJavaDoc(String tablename, Column r)
        {
            string result = "";
            result = "\n";
            result += "/**\n";
            result += "* <p> Gets the " + r.column_name + " of the associated " + tablename + " object </p>\n";
            result += "* @return the " + r.column_name + " of this " + tablename + " object.\n";
            result += "*/\n";
            return result;

        }

        public static String genJavaTestJavaDoc(JavaTestType test, table t) {
            String result = "\n/**\n";
            result += "<p> ";
            
            switch (test)
            {
                case JavaTestType.SetterWorks:
                    result += "Tests That the Setters for the " + t.name + "object work";
                    break;
                case JavaTestType.SetterThrowsException:
                    result += "Tests That the Setters for the " + t.name + "object can throw exceptions with invalid inputs";
                    break;
                case JavaTestType.Getter:
                    result += "Tests That the getters for the " + t.name + "object work";
                    break;
                case JavaTestType.ParamartizedConstructor:
                    result += "Tests That the Paramaterized Constructor for the " + t.name + "object works";
                    break;
                case JavaTestType.DefaultConstructor:
                    result += "Tests That the default constructor for the " + t.name + "object works";
                    break;
                case JavaTestType.CompareTo:
                    result += "Tests That the CompareTo Method for the " + t.name + "object works";
                    break;
                case JavaTestType.TwoHundredOnGet:
                    result += "Test That a logged in user gets a 200 status on doGet";
                    break;
                case JavaTestType.TwoHundredOnPost:
                    result += "Test That a logged in user gets a 200 status on doPost";
                    break;
                case JavaTestType.TwoHundredIfLoggedIn:
                    result += "Tests That the user will received a 200 status on doGet if they are logged in";
                    break;
                case JavaTestType.ThreeOhTwoOnGet:
                    result += "Tests That the user will received a 302 status on doGet if they are logged out ";

                    break;
                case JavaTestType.ThreeOhTwoOnPost:
                    result += "Tests That the user will received a 302 status on doPost if they are logged out ";

                    break;
                case JavaTestType.ThreeOhTwoIfLoggedOut:
                    result += "";
                    break;

                case JavaTestType.CanAddWithNoErrors:
                    result += "Tests That We can add to the database if all input fields are validated ";

                    break;
                case JavaTestType.CanThrowException:
                    result += "Tests That the user will received a error messages for each incorrectly filled out field on the form.";

                    break;
                case JavaTestType.initTest:
                    result += "Test That initialzing the Servlet Does Not Crash or cause an exception";
                    break;
                case JavaTestType.setupTests:
                    result += "setup the tests by creating a new instance of the servlet and setting some standard variablges";
                    break;
                case JavaTestType.teardownTests:
                    result += "tear down by setting all variablges to null.";
                    break;
                case JavaTestType.WrongRoleThreeOhTwoGet:
                    result += "Test that a user in the wrong role will get a 302 on a doGet";
                    break;
                case JavaTestType.WrongRoleThreeOhTwoPost:
                    result += "Test that a user in the wrong role will get a 302 on a doPost";
                    break;
                case JavaTestType.LoggedInGetAllGetsAll:
                    result += "Test that a logged in user is able to retreive all of the " + t.name+ " objects.";
                    break;
                case JavaTestType.GetOneGetsOne:
                    result += "Test that a logged in user is able to retreive a specific one of the "+t.name+" objects.";
                    break;
                case JavaTestType.GetOneCanFail:
                    result += "Test that getting one "+ t.name+" can fail.";
                    break;
                case JavaTestType.GetAllCanFilter:
                    result += "Test that getting all " + t.name + " can filter.";
                    break;
                case JavaTestType.ErrorMessagesforEachField:
                    result += "Test that error messages are sent for each field for adding" + t.name + " objects. That is to say, testing server" +
                        "side validation";
                    break;
                case JavaTestType.DuplicateDoesNotGetAdded:
                    result += "Test that "+t.name+ " objects with duplicate primary keys don't get added, and proper error handling exists.";
                    break;
                case JavaTestType.CanUpdateWithNoErrors:
                    result += "Test that we are able to update " + t.name+" objects if there are no errors in the input fields";
                    break;
                case JavaTestType.DuplicateDoesNotGetUpdated:
                    result += "Test that a duplicate primary key will get caught when trying to update " + t.name + " objects.";

                    break;
                case JavaTestType.DeactivateCanDeactivate:
                    result += "Test that the deactivation servlet can deactivate a " + t.name;
                    break;
                case JavaTestType.DeactivateCanFailIfalreadyInactive:
                    result += "Test that the deactivate servlet will fail if the "+ t.name + " is already insactive.";
                    break;
                    
                case JavaTestType.ActivateCanActivate:
                    result += "Test that the activation servlet can active a "+t.name;
                    break;
                case JavaTestType.ActivateCanFailIfAlreadayActive:
                    result += "Test that the activation servlet can fail if the "+ t.name+" is already active";
                   
                    break;
                case JavaTestType.DeleteCanDelete:
                    result += "Test that the delete servlet can delete a "+ t.name;
                    break;
                case JavaTestType.DeleteCanFailIfIDNotExist:
                    result += "Test that the delete servlet can fail if the given "+t.name + " does not exist.";
                    break;
                default:
                    result += "Test something, needs more specificity";
                    break;
            }

            result += " </p>\n";
            result += "*/\n";

            return result;

        }

    }
}
