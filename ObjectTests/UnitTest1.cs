using Data_Objects;
using Data_Access_Interfaces;
using Data_Access;
using Data_Access_Fake;
using LogicLayer;
using System.Collections;
namespace ObjectTests

{
    [TestClass]
    public class MySql_Tests
    {
        [TestInitialize]
        public void testSetup()
        {
            file_read_manager reader = new file_read_manager(new fileReadFake());
            reader.readdata();
        }
        [TestMethod]
        public void testMySqlSelectAll()
        {

            iTable user = (iTable)data_tables.all_tables[0];
            string actual = user.gen_retreive_by_key();
            string expected = "DROP PROCEDURE IF EXISTS sp_update_User;\r\n DELIMITER $$\r\nCREATE PROCEDURE sp_update_User\r\n(oldUser_ID int\r\noldUser_Name nvarchar(100),\r\nnewUser_Name nvarchar(100),\r\noldUser_PW nvarchar(100),\r\nnewUser_PW nvarchar(100),\r\noldEmail nvarchar(100),\r\nnewEmail nvarchar(100),\r\n)\r\nbegin \r\nUPDATE User\r\nset,\r\nUser_Name = newUser_Name,\r\nUser_PW = newUser_PW,\r\nEmail = newEmail\r\nWHERE User_ID= oldUser_ID\r\nAND User_Name= oldUser_Name\r\nAND User_PW= oldUser_PW\r\nAND Email= oldEmail\r\n ;\r\nend $$";
            Assert.AreEqual(expected, actual);


        }
        [TestMethod]
        public void testCommentBoxSqlUpdate()
        {
            List<int> errors = new List<int>();
            List<char> errorschar = new List<char>();

            string actual = commentBox.genCommentBox("User", Component_Enum.SQL_Update);
            string expected = "/******************\nCreate the update script for the User table\n Created By Jonathan Beck "+ DateTime.Now.ToShortDateString() + "\n***************/\n";
            for (int i = 0; i < actual.Length; i++)
            {
                if (actual[i] != expected[i])
                {
                    errors.Add(i);
                    errorschar.Add(actual[i]);
                    errorschar.Add(expected[i]);
                }
            }
                Assert.AreEqual(expected, actual);


            }

        
    }
}
