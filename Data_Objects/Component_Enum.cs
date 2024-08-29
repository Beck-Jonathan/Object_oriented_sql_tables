using Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data_Objects
{
    public enum Component_Enum
    {
        SQL_table,
        SQL_Audit_Table,
        SQL_Update,
        SQL_Delete,
        SQL_Retreive_By_PK,
        SQL_Retreive_By_All,
        SQL_Retreive_By_FK,
        SQL_Insert,
        SQL_Insert_Trigger,
        SQL_Delete_Trigger,
        SQL_Update_Trigger,
        CSharp_IAccessor,
        CSharp_Accessor,
        CSharp_IManager,
        CSharp_Manager,
        CSharp_DataObject,
        CSharp_XAML_Window,
        CSharp_Window_Control,
        SQL_Sample_Data,
        Java_JSP_Add,
        Java_JSP_ViewAll,
        Java_Servlet_Add,
        Java_Servlet_ViewAll,
        SQL_Undelete,
        SQL_Retreive_Active,
        Java_Servlet_Delete,
        Java_JSP_ViewEdit,
        Java_Servlet_ViewEdit,
        SQL_Select_Distinct,
        CSharp_Manager_Add,
        CSharp_Manager_Delete,
        CSharp_Manager_Undelete,
        CSharp_Manager_Retreive_By_PK,
        CSharp_Manager_Retreive_All_No_Param,
        CSharp_Manager_Retreive_All_One_Param,
        CSharp_Manager_Retreive_All_Two_Param,
        CSharp_Manager_Update,
        CSharp_Manager_Retreive_By_FK_No_Param,
        CSharp_Manager_Retreive_By_FK_One_Param,
        CSharp_Manager_Retreive_By_FK_Two_Param,
        CSharp_Accessor_Add,
        CSharp_Accessor_Delete,
        CSharp_Accessor_Undelete,
        CSharp_Accessor_Retreive_By_PK,
        CSharp_Accessor_Retreive_All_Two_Param,
        CSharp_Accessor_Update,
        CSharp_Accessor_Retreive_By_FK_Two_Param,

    }
    public enum XMLClassType
    {
        CSharpIAccessor,
        CSharpIManager,
        CSharpManager,
        CSharpAccessor,
        CSharpDataObject,
        JavaDAO,
        JavaDataObject
    }
    public enum XML_Method_Type
    {
        CSharp_Manager_Add,
        CSharp_Manager_Delete,
        CSharp_Manager_Undelete,
        CSharp_Manager_Retreive_By_PK,
        CSharp_Manager_Retreive_All_No_Param,
        CSharp_Manager_Retreive_All_One_Param,
        CSharp_Manager_Retreive_All_Two_Param,
        CSharp_Manager_Update,
        CSharp_Manager_Retreive_By_FK_No_Param,
        CSharp_Manager_Retreive_By_FK_One_Param,
        CSharp_Manager_Retreive_By_FK_Two_Param,
        CSharp_Accessor_Add,
        CSharp_Accessor_Delete,
        CSharp_Accessor_Undelete,
        CSharp_Accessor_Retreive_By_PK,        
        CSharp_Accessor_Retreive_All_Two_Param,
        CSharp_Accessor_Update,        
        CSharp_Accessor_Retreive_By_FK_Two_Param,
        CSharp_Accessor_Select_Distinct_For_Dropdown,

    }
}
