# Object Oriented Code Generator
This takes a data dictionary and creates code in a variety of program languages.
* MySQL (Table definitions and stored procedures)
* T-Sql (Microsoft SQL Server) (Table definitions and stored procedures)
* Java (Object Definitions, JSP View, Servlet Controllers, DAO objects for database calls) 
* C# (Object Definitions, Logic Layer Interface and manager Data Access Layer Interface and Manager)
* JavaScript
## Description
By looping through each table, column and column property, a large amount of code is generated.
## Getting Started
### Dependencies
* None
### Installing
* Included in the V0.2 alpha release are 
    * An example data dictionary
    * That same data dictionary convered into a tab separated txt file
    * That same data dictionary ran through this program with all options selected, so you can see the outputted mySql.
*Unzip this to the directory of your choosing, then run "Object_oriented_sql_tables.exe"
### Executing program
* Click select file
   * during the first run, it is suggested to use the derby_database.txt file that was included in the zip
    * During later runs, you can modify the derby_database.xlsx file, then "Save as" a tab separated txt file
* Assign a name to your database
* Click run
## Help
## Authors
Jonathan Beck
BeckJonathanJ@gmail.com
## Version History
* 0.3
   * A years worth of updates, adding T-SQL, C#, Java, Javascript     
* 0.2
    * Various bug fixes and optimizations
    * See [commit change]() or See [release history]()
* 0.1
    * Initial Release
### Knwon issues / planned updates

* Comment generation for SQL scripts needs improvements
* A web front end
* Database creation within app.
