# MySQL table generator

This takes a data dictionary and creates a MySQL script with the charactaristics and stored procedures you request.

## Description

More details needed here

## Getting Started

### Dependencies

* I've only tested this on my windows 10 machine, my apologies
* This generates mySQL, so you will need MySQL workbench and a server running to actually use these results

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
* Assign a name to your dtaabse
* Click "read file" and a list of tables will be displayed
* From the drop down, you can choose which table you wish to modify
* You can modify the additions to each table, such as PK, FK, or various stored procedures
* I've also included a sql file that has insert scripts of dummy data for these tables.


## Help




## Authors

Jonathan Beck
BeckJonathanJ@gmail.com

## Version History

* 0.2
    * Various bug fixes and optimizations
    * See [commit change]() or See [release history]()
* 0.1
    * Initial Release
### Knwon issues / planned updates
* If you wish to create a second script, you have to fully close the program (Fixed, program now closes itself, 9/13 1:30 pm)
* The user experiance of which button is active when is misleading (fixed, pathing now logical, 9/13 1:40 pm)
* The words "row" and "column" were swapped, and the program needs updating to address this
* A "select all" , "selected default" "deselect all" button for options needs to be implemented
* File pathing needs to be updated to work properly on other users computers (updated 9/13 2:15 pm)
* The dummy insert script is currently not uploaded, my apologies
* Comments need to be added to explain various components (updated 9/13, 1PM) 
* Indentiy/sequence fields are not currently implemented
* rudamentary joins need to be imploemented
