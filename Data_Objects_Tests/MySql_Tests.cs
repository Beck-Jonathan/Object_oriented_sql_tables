using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Data_Access;
using Data_Access_Fake;
using Data_Access_Interfaces;
using Data_Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data_Objects_Tests
{
    [TestClass]
    public class MySql_Tests
    {
        [TestInitialize]
        public void testSetup()
        {
           file_read reader = new file_read(new fileReadFake());
            reader.readdata();
        }
        [TestMethod]
        public void testMySqlSelectAll() {

            iTable user = (iTable)data_tables.all_tables[0];
            string actual = user.gen_retreive_by_key();
            string expected = "";
            Assert.AreEqual(expected, actual);

            
        }

    }
}
