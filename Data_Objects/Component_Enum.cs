﻿namespace Data_Objects
{
    public enum Component_Enum
    {
        SQL_table,
        SQL_Audit_Table,
        SQL_Update,
        SQL_Delete,
        SQL_retrieve_By_PK,
        SQL_retrieve_By_All,
        SQL_retrieve_By_FK,
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
        SQL_retrieve_Active,
        Java_Servlet_Delete,
        Java_JSP_ViewEdit,
        Java_Servlet_ViewEdit,
        SQL_Select_Distinct,
        CSharp_Manager_Add,
        CSharp_Manager_Delete,
        CSharp_Manager_Undelete,
        CSharp_Manager_retrieve_By_PK,
        CSharp_Manager_retrieve_All_No_Param,
        CSharp_Manager_retrieve_All_One_Param,
        CSharp_Manager_retrieve_All_Two_Param,
        CSharp_Manager_Update,
        CSharp_Manager_retrieve_By_FK_No_Param,
        CSharp_Manager_retrieve_By_FK_One_Param,
        CSharp_Manager_retrieve_By_FK_Two_Param,
        CSharp_Accessor_Add,
        CSharp_Accessor_Delete,
        CSharp_Accessor_Undelete,
        CSharp_Accessor_retrieve_By_PK,
        CSharp_Accessor_retrieve_All_Two_Param,
        CSharp_Accessor_Update,
        CSharp_Accessor_retrieve_By_FK_Two_Param,
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
        CSharp_Manager_retrieve_By_PK,
        CSharp_Manager_retrieve_All_No_Param,
        CSharp_Manager_retrieve_All_One_Param,
        CSharp_Manager_retrieve_All_Two_Param,
        CSharp_Manager_Update,
        CSharp_Manager_retrieve_By_FK_No_Param,
        CSharp_Manager_retrieve_By_FK_One_Param,
        CSharp_Manager_retrieve_By_FK_Two_Param,
        CSharp_Accessor_Add,
        CSharp_Accessor_Delete,
        CSharp_Accessor_Undelete,
        CSharp_Accessor_retrieve_By_PK,
        CSharp_Accessor_retrieve_All_Two_Param,
        CSharp_Accessor_Update,
        CSharp_Accessor_retrieve_By_FK_Two_Param,
        CSharp_Accessor_Select_Distinct_For_Dropdown,
    }
    public enum JavaDoc_Method_Type
    {
        Java_DAO_Add,
        Java_DAO_Delete,
        Java_DAO_Undelete,
        Java_DAO_retrieve_By_FK,
        Java_DAO_retrieve_All_,
        Java_DAO_retrieve_By_PK,
        Java_DAO_Update,
        Java_DAO_Get_Distinct
    }
    public enum  JavaTestType
    {
        SetterWorks,
        SetterThrowsException,
        Getter,
        ParamartizedConstructor,
        DefaultConstructor,
        CompareTo,
        TwoHundredOnGet,
        TwoHundredOnPost,
        TwoHundredIfLoggedIn,
        ThreeOhTwoOnGet,
        ThreeOhTwoOnPost,
        ThreeOhTwoIfLoggedOut,
        CanAddWithNoErrors,
        ErrorMessagesforEachField,
        CanThrowException,
        initTest,
        setupTests,
        teardownTests,
        WrongRoleThreeOhTwoGet,
        WrongRoleThreeOhTwoPost,
        LoggedInGetAllGetsAll,
        GetOneGetsOne,
        GetOneCanFail,
        GetAllCanFilter,
        DuplicateDoesNotGetAdded,
        CanUpdateWithNoErrors,
        DuplicateDoesNotGetUpdated,
        DeactivateCanDeactivate,
        DeactivateCanFailIfalreadyInactive,
        DeactiveCanFailWithKeyNotfound,
        ActivateCanActivate,
        ActivateCanFailIfAlreadayActive,
        ActivateCanFailWithKeyNotFound,
        DeleteCanDelete,
        DeleteCanFailIfIDNotExist
    }

    public enum ServletType { 
        CreateServlet,
        DeleteServlet,
        ExportServlet,
        UploadServlet,
        ViewAllSErvlet,
        ViewEditSErvlet 
    
    }
    public enum UseCaseType
    {
        createThing,
        RetrieveOneThing,
        RetrieveAllThing,
        FilterThing,
        SearchThing,
        UpdateThing,
        DeleteThing,
        DeactivateThing,
        ActivateThing,
    }
}
