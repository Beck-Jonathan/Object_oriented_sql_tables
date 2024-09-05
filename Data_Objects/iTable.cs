using System;
using System.Collections.Generic;
namespace Data_Objects
{
    public interface iTable
    {
        //various components of a table
        String name { set; get; }
        header Header { set; get; }
        List<Column> columns { set; get; }
        List<String> primary_keys { set; get; }
        List<String> foreign_keys { set; get; }
        List<String> alternate_keys { set; get; }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the primary keys of the specified SQL language Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the the Pimary Key(s) of the table </returns>
        String gen_primary_keys();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the foreign keys of the specified SQL language Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the the foreign Key(s) of the table </returns>
        String gen_foreign_keys();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the alternate keys of the specified SQL language Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the the alternate Key(s) of the table </returns>
        string gen_alternate_keys();
        /// <summary>
        /// Generates a genertic footer for a specified SQL language  <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A stringto act as a footer for the specified SQL language  <see cref="table"/> </returns>
        String gen_table_footer();
        /// <summary>
        /// 
        /// generates lines that specify the header of the specified SQL language audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the header of the audit <see cref="table"/> </returns>
        String audit_gen_header();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates lines that specify the alternate various columns and their attributes of the specified SQL language Table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the the columns of the table </returns>
        String gen_columns();
        /// <summary>
        /// 
        /// generates lines that specify the the specified SQL language audit <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string specified SQL language code that creates the audit <see cref="table"/> </returns>
        String gen_audit_table();
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a specified SQL language stored procedure that creates a standard Delete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string comment box followed by a  string specified SQL language code that creates the the delete SP for the table </returns>
        String gen_delete();
        /// <summary>
        /// 
        /// generates a string comment box followed by a  a specified SQL language stored procedure that creates a standard unDelete function. This funciton will ask for the pimary key(s) of the table
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string specified SQL language code that creates the the undelete SP for the table </returns>
        String gen_undelete();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a specified SQL language stored procedure that creates a retreive by primary key function. This funciton will ask for the pimary key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string specified SQL language code that creates the the retreive by Primary key SP for the table </returns>
        String gen_retreive_by_key();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a specified SQL language stored procedure that creates a retreive by foreign key function. This funciton will ask for a foregn key(s) of the table, 
        /// and return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string specified SQL language code that creates the the retreive by Foreign-key SP for the table </returns>
        //to generate retreive by fk, not implmented well yet
        String gen_retreive_by_fkey();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a  a specified SQL language stored procedure that creates a retreive  all key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a  string specified SQL language code that creates the the retreive by Foreign-key SP for the table </returns>
        String gen_retreive_by_all();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates generates a string comment box followed by specified SQL language stored procedure that creates a retreive active (that is, is_active==1) key function. This funciton  
        /// return all fields of the record, joining with keyed fields to return a full "view model".
        /// Typically this will return a list of objects.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by a specified SQL language code that creates the the retreive by active SP for the table </returns>
        String gen_retreive_by_active();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by a specified SQL language stored procedure that creates a standard insert function. This funciton will ask for  each field, 
        /// besides auto-increment fields.
        /// Jonathan Beck
        /// </summary>
        /// <returns>generates a string comment box followed by specified SQL language code that creates the the insert SP for the table </returns>
        string gen_insert();
        /// <summary>       
        /// generates a string comment box followed by a specified SQL language stored procedure that creates a standard trigger that fires upon updates to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by specified SQL language code that creates a trigger that fires on updates to the <see cref="table"/> object </returns>
        String gen_update_trigger();
        /// <summary>       
        /// generates a string comment box followed by a specified SQL language stored procedure that creates a standard trigger that fires upon inserts to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by specified SQL language code that creates a trigger that fires on inserts to the <see cref="table"/> object </returns>
        String gen_insert_trigger();
        /// <summary>       
        /// generates a string comment box followed by a specified SQL language stored procedure that creates a standard trigger that fires upon deletes to the table. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by specified SQL language code that creates a trigger that fires on deletes to the <see cref="table"/> object </returns>
        String gen_delete_trigger();
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a string comment box followed by  a specified SQL language stored procedure that creates a standard update function. This funciton will ask for @old and @new of each field, 
        /// besides primary key fields, which just ask for @old versions.
        /// Jonathan Beck
        /// </summary>
        /// <returns> a string comment box followed by a  string specified SQL language code that creates the the update SP for the table </returns>
        String gen_update();
        String gen_select_distinct_for_dropdown();
        /// <summary>       
        /// generates a string comment box followed by a specified SQL language insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields.. 
        /// Jonathan Beck
        /// </summary>
        /// <returns> generates a string comment box followed by specified SQL language insert statement formatted for each <see cref="Column"/> of this table, excluding auto-increment fields. </returns>
        String gen_sample_space();
    }
}
