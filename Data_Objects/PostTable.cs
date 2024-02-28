using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Objects
{
    public class PostTable : table, iTable
    {
        public PostTable(string name, List<Column> columns) : base(name, columns)
        {
        }

        public string audit_gen_header()
        {
            throw new NotImplementedException();
        }

        public string gen_alternate_keys()
        {
            throw new NotImplementedException();
        }

        public string gen_audit_table()
        {
            throw new NotImplementedException();
        }

        public string gen_columns()
        {
            throw new NotImplementedException();
        }

        public string gen_delete()
        {
            throw new NotImplementedException();
        }

        public string gen_delete_trigger()
        {
            throw new NotImplementedException();
        }

        public string gen_foreign_keys()
        {
            throw new NotImplementedException();
        }

        public string gen_header()
        {
            throw new NotImplementedException();
        }

        public string gen_insert()
        {
            throw new NotImplementedException();
        }

        public string gen_insert_trigger()
        {
            throw new NotImplementedException();
        }

        public string gen_primary_keys()
        {
            throw new NotImplementedException();
        }

        public string gen_retreive_by_all()
        {
            throw new NotImplementedException();
        }

        public string gen_retreive_by_fkey(foreignKey key)
        {
            throw new NotImplementedException();
        }

        public string gen_retreive_by_key()
        {
            throw new NotImplementedException();
        }

        public string gen_sample_space()
        {
            throw new NotImplementedException();
        }

        public string gen_undelete()
        {
            throw new NotImplementedException();
        }

        public string gen_update()
        {
            throw new NotImplementedException();
        }

        public string gen_update_trigger()
        {
            throw new NotImplementedException();
        }
    }
}
