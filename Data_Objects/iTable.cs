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




        String gen_primary_keys();

        String gen_foreign_keys();

        string gen_alternate_keys();

        String gen_table_footer();
        String gen_header();

        String audit_gen_header();

        String gen_columns();


        String gen_audit_table();

        // to generate the SP_update

        // to generate the SP_delete
        String gen_delete();


        String gen_undelete();

        // to generate the SP_retreive using a primary key
        String gen_retreive_by_key();


        //to generate retreive by fk, not implmented well yet
        String gen_retreive_by_fkey(foreignKey key);


        // to generate the SP_retrive, showing all data in a table
        String gen_retreive_by_all();
        // to generate the SP_retrive_active, showing all data in a table
        String gen_retreive_by_active();

        // to generate the SP_insert
        string gen_insert();

        // to generate the on update trigger
        String gen_update_trigger();

        // to generate the on insert trigger
        String gen_insert_trigger();

        // to generate the on delete trigger
        String gen_delete_trigger();
        String gen_update();

        String gen_select_distinct_for_dropdown();

        String gen_sample_space();











    }


}





