DROP DATABASE IF EXISTS WFTDA_debug;
CREATE DATABASE WFTDA_debug;
USE WFTDA_debug;
/******************
Create the ARENAS table
***************/


DROP TABLE IF EXISTS ARENAS;

CREATE TABLE ARENAS(


arena_name	nvarchar(50)	not null	unique	comment 'The name of the arena'
,city	nvarchar(50)	not null	comment 'City where the arena is located'
,state	nvarchar(50)	not null	comment 'State wthere the arena is located'
,contact_phone	nvarchar(12)	not null	comment 'Contact phone number for the arena'
,capacity	int	not null	comment 'Seating capacity of the arena'
,CONSTRAINT ARENAS_PK PRIMARY KEY (arena_name)
);
/******************
Create the ARENAS Audit table
***************/


DROP TABLE IF EXISTS ARENAS_audit;

CREATE TABLE ARENAS_audit(


arena_name	nvarchar(50)	not null	unique	comment 'The name of the arena'
,city	nvarchar(50)	not null	comment 'City where the arena is located'
,state	nvarchar(50)	not null	comment 'State wthere the arena is located'
,contact_phone	nvarchar(12)	not null	comment 'Contact phone number for the arena'
,capacity	int	not null	comment 'Seating capacity of the arena'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the ARENAS table
***************/
DROP PROCEDURE IF EXISTS sp_update_ARENAS;
DELIMITER $$
CREATE PROCEDURE sp_update_ARENAS
(in arena_name_param nvarchar(50)
,in city_param nvarchar(50)
,in state_param nvarchar(50)
,in contact_phone_param nvarchar(12)
,in capacity_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE ARENAS
 set city = city_param
,state = state_param
,contact_phone = contact_phone_param
,capacity = capacity_param
WHERE arena_name=arena_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the ARENAS table
***************/
DROP PROCEDURE IF EXISTS sp_delete_ARENAS;
DELIMITER $$
CREATE PROCEDURE sp_delete_ARENAS
(arena_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM ARENAS
  WHERE arena_name=arena_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the ARENAS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_ARENAS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_ARENAS
(
arena_name_param nvarchar(50)
)
 Begin 
 select 
arena_name 
,city 
,state 
,contact_phone 
,capacity 

 FROM ARENAS
where arena_name=arena_name_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the ARENAS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_ARENAS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_ARENAS()
begin 
 SELECT 

arena_name
,city
,state
,contact_phone
,capacity
 FROM ARENAS
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the ARENAS table
***************/
DROP PROCEDURE IF EXISTS sp_insert_ARENAS;
DELIMITER $$
CREATE PROCEDURE sp_insert_ARENAS(
in arena_name_param nvarchar(50)
,in city_param nvarchar(50)
,in state_param nvarchar(50)
,in contact_phone_param nvarchar(12)
,in capacity_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  ARENAS
 values 
(arena_name_param
,city_param
,state_param
,contact_phone_param
,capacity_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the ARENAS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_ARENAS_after_insert $$
CREATE TRIGGER tr_ARENAS_after_insert
AFTER insert ON ARENAS
for each row
begin
insert intoARENAS_audit (
arena_name 
,city 
,state 
,contact_phone 
,capacity 

, action_type
, action_date
, action_user
) values(
new.arena_name 
,new.city 
,new.state 
,new.contact_phone 
,new.capacity 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the ARENAS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_ARENAS_after_update $$
CREATE TRIGGER tr_ARENAS_after_update
AFTER UPDATE ON ARENAS
for each row
begin
insert intoARENAS_audit (
arena_name 
,city 
,state 
,contact_phone 
,capacity 

, action_type
, action_date
, action_user
) values(
new.arena_name 
,new.city 
,new.state 
,new.contact_phone 
,new.capacity 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the TEAM table
***************/


DROP TABLE IF EXISTS TEAM;

CREATE TABLE TEAM(


team_name	nvarchar(50)	not null	unique	comment 'The team Nickname'
,city	nvarchar(50)	not null	comment 'City where the team is headquartered'
,state	nvarchar(50)	not null	comment 'Seate where the team is headquarterd'
,division	nvarchar(50)	not null	comment 'The Local Division this team is associated with'
,arena_name	nvarchar(50)	not null	comment 'The arena ID'
,CONSTRAINT TEAM_PK PRIMARY KEY (team_name)
);
/******************
Create the TEAM Audit table
***************/


DROP TABLE IF EXISTS TEAM_audit;

CREATE TABLE TEAM_audit(


team_name	nvarchar(50)	not null	unique	comment 'The team Nickname'
,city	nvarchar(50)	not null	comment 'City where the team is headquartered'
,state	nvarchar(50)	not null	comment 'Seate where the team is headquarterd'
,division	nvarchar(50)	not null	comment 'The Local Division this team is associated with'
,arena_name	nvarchar(50)	not null	comment 'The arena ID'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the TEAM table
***************/
DROP PROCEDURE IF EXISTS sp_update_TEAM;
DELIMITER $$
CREATE PROCEDURE sp_update_TEAM
(in team_name_param nvarchar(50)
,in city_param nvarchar(50)
,in state_param nvarchar(50)
,in division_param nvarchar(50)
,in arena_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE TEAM
 set city = city_param
,state = state_param
,division = division_param
,arena_name = arena_name_param
WHERE team_name=team_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the TEAM table
***************/
DROP PROCEDURE IF EXISTS sp_delete_TEAM;
DELIMITER $$
CREATE PROCEDURE sp_delete_TEAM
(team_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM TEAM
  WHERE team_name=team_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the TEAM table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_TEAM;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_TEAM
(
team_name_param nvarchar(50)
)
 Begin 
 select 
team_name 
,city 
,state 
,division 
,arena_name 

 FROM TEAM
where team_name=team_name_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the TEAM table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_TEAM;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_TEAM()
begin 
 SELECT 

team_name
,city
,state
,division
,arena_name
 FROM TEAM
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the TEAM table
***************/
DROP PROCEDURE IF EXISTS sp_insert_TEAM;
DELIMITER $$
CREATE PROCEDURE sp_insert_TEAM(
in team_name_param nvarchar(50)
,in city_param nvarchar(50)
,in state_param nvarchar(50)
,in division_param nvarchar(50)
,in arena_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  TEAM
 values 
(team_name_param
,city_param
,state_param
,division_param
,arena_name_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the TEAM table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_TEAM_after_insert $$
CREATE TRIGGER tr_TEAM_after_insert
AFTER insert ON TEAM
for each row
begin
insert intoTEAM_audit (
team_name 
,city 
,state 
,division 
,arena_name 

, action_type
, action_date
, action_user
) values(
new.team_name 
,new.city 
,new.state 
,new.division 
,new.arena_name 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the TEAM table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_TEAM_after_update $$
CREATE TRIGGER tr_TEAM_after_update
AFTER UPDATE ON TEAM
for each row
begin
insert intoTEAM_audit (
team_name 
,city 
,state 
,division 
,arena_name 

, action_type
, action_date
, action_user
) values(
new.team_name 
,new.city 
,new.state 
,new.division 
,new.arena_name 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the SKATER table
***************/


DROP TABLE IF EXISTS SKATER;

CREATE TABLE SKATER(


derby_name	nvarchar(50)	not null	unique	comment 'Skaters chosen Derby Name'
,first_name	nvarchar(50)	not null	comment 'Skater Name assigned at birth'
,last_name	nvarchar(50)	not null	comment 'Skater Family Name'
,phone_number	nvarchar(12)	not null	comment 'Skater Phone Number'
,team_affiliation	nvarchar(50)	null	comment 'The particuar team this skater skates for'
,CONSTRAINT SKATER_PK PRIMARY KEY (derby_name)
);
/******************
Create the SKATER Audit table
***************/


DROP TABLE IF EXISTS SKATER_audit;

CREATE TABLE SKATER_audit(


derby_name	nvarchar(50)	not null	unique	comment 'Skaters chosen Derby Name'
,first_name	nvarchar(50)	not null	comment 'Skater Name assigned at birth'
,last_name	nvarchar(50)	not null	comment 'Skater Family Name'
,phone_number	nvarchar(12)	not null	comment 'Skater Phone Number'
,team_affiliation	nvarchar(50)	null	comment 'The particuar team this skater skates for'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the SKATER table
***************/
DROP PROCEDURE IF EXISTS sp_update_SKATER;
DELIMITER $$
CREATE PROCEDURE sp_update_SKATER
(in derby_name_param nvarchar(50)
,in first_name_param nvarchar(50)
,in last_name_param nvarchar(50)
,in phone_number_param nvarchar(12)
,in team_affiliation_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE SKATER
 set first_name = first_name_param
,last_name = last_name_param
,phone_number = phone_number_param
,team_affiliation = team_affiliation_param
WHERE derby_name=derby_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the SKATER table
***************/
DROP PROCEDURE IF EXISTS sp_delete_SKATER;
DELIMITER $$
CREATE PROCEDURE sp_delete_SKATER
(derby_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM SKATER
  WHERE derby_name=derby_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the SKATER table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_SKATER;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_SKATER
(
derby_name_param nvarchar(50)
)
 Begin 
 select 
derby_name 
,first_name 
,last_name 
,phone_number 
,team_affiliation 

 FROM SKATER
where derby_name=derby_name_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the SKATER table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_SKATER;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_SKATER()
begin 
 SELECT 

derby_name
,first_name
,last_name
,phone_number
,team_affiliation
 FROM SKATER
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the SKATER table
***************/
DROP PROCEDURE IF EXISTS sp_insert_SKATER;
DELIMITER $$
CREATE PROCEDURE sp_insert_SKATER(
in derby_name_param nvarchar(50)
,in first_name_param nvarchar(50)
,in last_name_param nvarchar(50)
,in phone_number_param nvarchar(12)
,in team_affiliation_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  SKATER
 values 
(derby_name_param
,first_name_param
,last_name_param
,phone_number_param
,team_affiliation_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the SKATER table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_SKATER_after_insert $$
CREATE TRIGGER tr_SKATER_after_insert
AFTER insert ON SKATER
for each row
begin
insert intoSKATER_audit (
derby_name 
,first_name 
,last_name 
,phone_number 
,team_affiliation 

, action_type
, action_date
, action_user
) values(
new.derby_name 
,new.first_name 
,new.last_name 
,new.phone_number 
,new.team_affiliation 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the SKATER table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_SKATER_after_update $$
CREATE TRIGGER tr_SKATER_after_update
AFTER UPDATE ON SKATER
for each row
begin
insert intoSKATER_audit (
derby_name 
,first_name 
,last_name 
,phone_number 
,team_affiliation 

, action_type
, action_date
, action_user
) values(
new.derby_name 
,new.first_name 
,new.last_name 
,new.phone_number 
,new.team_affiliation 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the INVOICES_ISSUED table
***************/


DROP TABLE IF EXISTS INVOICES_ISSUED;

CREATE TABLE INVOICES_ISSUED(


invoice_number	int	not null	comment 'The league assigned invoice number'
,derby_name	nvarchar(50)	not null	comment 'Skaters chosen Derby Name'
,practice_date	date	not null	comment '"The Date of the practice attended, that this invoice is for"'
,date_issued	date	not null	comment 'The date the invoice was issued'
,amount	int	not null	comment 'The dollar amount of the invoice'
,receipt_date	date	null	comment 'The date we received the payment'
,CONSTRAINT INVOICES_ISSUED_PK PRIMARY KEY (invoice_number)
);
/******************
Create the INVOICES_ISSUED Audit table
***************/


DROP TABLE IF EXISTS INVOICES_ISSUED_audit;

CREATE TABLE INVOICES_ISSUED_audit(


invoice_number	int	not null	comment 'The league assigned invoice number'
,derby_name	nvarchar(50)	not null	comment 'Skaters chosen Derby Name'
,practice_date	date	not null	comment '"The Date of the practice attended, that this invoice is for"'
,date_issued	date	not null	comment 'The date the invoice was issued'
,amount	int	not null	comment 'The dollar amount of the invoice'
,receipt_date	date	null	comment 'The date we received the payment'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the INVOICES_ISSUED table
***************/
DROP PROCEDURE IF EXISTS sp_update_INVOICES_ISSUED;
DELIMITER $$
CREATE PROCEDURE sp_update_INVOICES_ISSUED
(in invoice_number_param int
,in derby_name_param nvarchar(50)
,in practice_date_param date
,in date_issued_param date
,in amount_param int
,in receipt_date_param date
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE INVOICES_ISSUED
 set derby_name = derby_name_param
,practice_date = practice_date_param
,date_issued = date_issued_param
,amount = amount_param
,receipt_date = receipt_date_param
WHERE invoice_number=invoice_number_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the INVOICES_ISSUED table
***************/
DROP PROCEDURE IF EXISTS sp_delete_INVOICES_ISSUED;
DELIMITER $$
CREATE PROCEDURE sp_delete_INVOICES_ISSUED
(invoice_number_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM INVOICES_ISSUED
  WHERE invoice_number=invoice_number_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the INVOICES_ISSUED table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_INVOICES_ISSUED;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_INVOICES_ISSUED
(
invoice_number_param int
)
 Begin 
 select 
invoice_number 
,derby_name 
,practice_date 
,date_issued 
,amount 
,receipt_date 

 FROM INVOICES_ISSUED
where invoice_number=invoice_number_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the INVOICES_ISSUED table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_INVOICES_ISSUED;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_INVOICES_ISSUED()
begin 
 SELECT 

invoice_number
,derby_name
,practice_date
,date_issued
,amount
,receipt_date
 FROM INVOICES_ISSUED
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the INVOICES_ISSUED table
***************/
DROP PROCEDURE IF EXISTS sp_insert_INVOICES_ISSUED;
DELIMITER $$
CREATE PROCEDURE sp_insert_INVOICES_ISSUED(
in invoice_number_param int
,in derby_name_param nvarchar(50)
,in practice_date_param date
,in date_issued_param date
,in amount_param int
,in receipt_date_param date
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  INVOICES_ISSUED
 values 
(invoice_number_param
,derby_name_param
,practice_date_param
,date_issued_param
,amount_param
,receipt_date_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the INVOICES_ISSUED table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_INVOICES_ISSUED_after_insert $$
CREATE TRIGGER tr_INVOICES_ISSUED_after_insert
AFTER insert ON INVOICES_ISSUED
for each row
begin
insert intoINVOICES_ISSUED_audit (
invoice_number 
,derby_name 
,practice_date 
,date_issued 
,amount 
,receipt_date 

, action_type
, action_date
, action_user
) values(
new.invoice_number 
,new.derby_name 
,new.practice_date 
,new.date_issued 
,new.amount 
,new.receipt_date 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the INVOICES_ISSUED table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_INVOICES_ISSUED_after_update $$
CREATE TRIGGER tr_INVOICES_ISSUED_after_update
AFTER UPDATE ON INVOICES_ISSUED
for each row
begin
insert intoINVOICES_ISSUED_audit (
invoice_number 
,derby_name 
,practice_date 
,date_issued 
,amount 
,receipt_date 

, action_type
, action_date
, action_user
) values(
new.invoice_number 
,new.derby_name 
,new.practice_date 
,new.date_issued 
,new.amount 
,new.receipt_date 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the MIXERS table
***************/


DROP TABLE IF EXISTS MIXERS;

CREATE TABLE MIXERS(


game_id	int	not null	unique	comment 'The internal id for this bout'
,arena_name	nvarchar(50)	not null	comment 'The Arena this mixer bout took place in'
,event_date	date	not null	comment 'The Date this mixer bout occured'
,CONSTRAINT MIXERS_PK PRIMARY KEY (game_id)
);
/******************
Create the MIXERS Audit table
***************/


DROP TABLE IF EXISTS MIXERS_audit;

CREATE TABLE MIXERS_audit(


game_id	int	not null	unique	comment 'The internal id for this bout'
,arena_name	nvarchar(50)	not null	comment 'The Arena this mixer bout took place in'
,event_date	date	not null	comment 'The Date this mixer bout occured'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the MIXERS table
***************/
DROP PROCEDURE IF EXISTS sp_update_MIXERS;
DELIMITER $$
CREATE PROCEDURE sp_update_MIXERS
(in game_id_param int
,in arena_name_param nvarchar(50)
,in event_date_param date
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE MIXERS
 set arena_name = arena_name_param
,event_date = event_date_param
WHERE game_id=game_id_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the MIXERS table
***************/
DROP PROCEDURE IF EXISTS sp_delete_MIXERS;
DELIMITER $$
CREATE PROCEDURE sp_delete_MIXERS
(game_id_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM MIXERS
  WHERE game_id=game_id_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the MIXERS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_MIXERS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_MIXERS
(
game_id_param int
)
 Begin 
 select 
game_id 
,arena_name 
,event_date 

 FROM MIXERS
where game_id=game_id_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the MIXERS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_MIXERS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_MIXERS()
begin 
 SELECT 

game_id
,arena_name
,event_date
 FROM MIXERS
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the MIXERS table
***************/
DROP PROCEDURE IF EXISTS sp_insert_MIXERS;
DELIMITER $$
CREATE PROCEDURE sp_insert_MIXERS(
in game_id_param int
,in arena_name_param nvarchar(50)
,in event_date_param date
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  MIXERS
 values 
(game_id_param
,arena_name_param
,event_date_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the MIXERS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_MIXERS_after_insert $$
CREATE TRIGGER tr_MIXERS_after_insert
AFTER insert ON MIXERS
for each row
begin
insert intoMIXERS_audit (
game_id 
,arena_name 
,event_date 

, action_type
, action_date
, action_user
) values(
new.game_id 
,new.arena_name 
,new.event_date 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the MIXERS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_MIXERS_after_update $$
CREATE TRIGGER tr_MIXERS_after_update
AFTER UPDATE ON MIXERS
for each row
begin
insert intoMIXERS_audit (
game_id 
,arena_name 
,event_date 

, action_type
, action_date
, action_user
) values(
new.game_id 
,new.arena_name 
,new.event_date 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the MIXER_PARTICIPANTS table
***************/


DROP TABLE IF EXISTS MIXER_PARTICIPANTS;

CREATE TABLE MIXER_PARTICIPANTS(


game_id	int	not null	comment 'the internal_id for this bout'
,game_count	int	not null	comment 'The assigned_id for a skater participating in this bout'
,derby_name	nvarchar(50)	not null	comment 'Skaters chosen Derby Name'
,points_scored	int	null	comment 'How many points this skater scored in this bout'
,CONSTRAINT MIXER_PARTICIPANTS_PK PRIMARY KEY (game_id , game_count)
);
/******************
Create the MIXER_PARTICIPANTS Audit table
***************/


DROP TABLE IF EXISTS MIXER_PARTICIPANTS_audit;

CREATE TABLE MIXER_PARTICIPANTS_audit(


game_id	int	not null	comment 'the internal_id for this bout'
,game_count	int	not null	comment 'The assigned_id for a skater participating in this bout'
,derby_name	nvarchar(50)	not null	comment 'Skaters chosen Derby Name'
,points_scored	int	null	comment 'How many points this skater scored in this bout'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the MIXER_PARTICIPANTS table
***************/
DROP PROCEDURE IF EXISTS sp_update_MIXER_PARTICIPANTS;
DELIMITER $$
CREATE PROCEDURE sp_update_MIXER_PARTICIPANTS
(in game_id_param int
,in game_count_param int
,in derby_name_param nvarchar(50)
,in points_scored_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE MIXER_PARTICIPANTS
 set derby_name = derby_name_param
,points_scored = points_scored_param
WHERE game_id=game_id_param
AND game_count=game_count_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the MIXER_PARTICIPANTS table
***************/
DROP PROCEDURE IF EXISTS sp_delete_MIXER_PARTICIPANTS;
DELIMITER $$
CREATE PROCEDURE sp_delete_MIXER_PARTICIPANTS
(game_id_param int
,game_count_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM MIXER_PARTICIPANTS
  WHERE game_id=game_id_param
AND game_count=game_count_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the MIXER_PARTICIPANTS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_MIXER_PARTICIPANTS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_MIXER_PARTICIPANTS
(
game_id_param int
,game_count_param int
)
 Begin 
 select 
game_id 
,game_count 
,derby_name 
,points_scored 

 FROM MIXER_PARTICIPANTS
where game_id=game_id_param
AND game_count=game_count_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the MIXER_PARTICIPANTS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_MIXER_PARTICIPANTS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_MIXER_PARTICIPANTS()
begin 
 SELECT 

game_id
,game_count
,derby_name
,points_scored
 FROM MIXER_PARTICIPANTS
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the MIXER_PARTICIPANTS table
***************/
DROP PROCEDURE IF EXISTS sp_insert_MIXER_PARTICIPANTS;
DELIMITER $$
CREATE PROCEDURE sp_insert_MIXER_PARTICIPANTS(
in game_id_param int
,in game_count_param int
,in derby_name_param nvarchar(50)
,in points_scored_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  MIXER_PARTICIPANTS
 values 
(game_id_param
,game_count_param
,derby_name_param
,points_scored_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the MIXER_PARTICIPANTS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_MIXER_PARTICIPANTS_after_insert $$
CREATE TRIGGER tr_MIXER_PARTICIPANTS_after_insert
AFTER insert ON MIXER_PARTICIPANTS
for each row
begin
insert intoMIXER_PARTICIPANTS_audit (
game_id 
,game_count 
,derby_name 
,points_scored 

, action_type
, action_date
, action_user
) values(
new.game_id 
,new.game_count 
,new.derby_name 
,new.points_scored 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the MIXER_PARTICIPANTS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_MIXER_PARTICIPANTS_after_update $$
CREATE TRIGGER tr_MIXER_PARTICIPANTS_after_update
AFTER UPDATE ON MIXER_PARTICIPANTS
for each row
begin
insert intoMIXER_PARTICIPANTS_audit (
game_id 
,game_count 
,derby_name 
,points_scored 

, action_type
, action_date
, action_user
) values(
new.game_id 
,new.game_count 
,new.derby_name 
,new.points_scored 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the VENDORS table
***************/


DROP TABLE IF EXISTS VENDORS;

CREATE TABLE VENDORS(


vendor_name	nvarchar(50)	not null	comment 'the name of the vendor'
,vendor_address	nvarchar(100)	not null	comment 'The address of the vendor'
,vendor_city	nvarchar(50)	not null	comment 'the city of the vendor'
,vendor_state	nvarchar(50)	not null	comment 'the state of the vendor'
,vendor_contact_name	nvarchar(50)	not null	unique	comment 'the contact person at the vendor'
,vendor_phone_number	nvarchar(14)	not null	unique	comment 'the most useful phone number for this vendor'
,vendor_type	enum ('arena', 'food', 'equipment','ref')	not null	comment 'an option select for what this vendor gets caregorized in for our accounting'
,CONSTRAINT VENDORS_PK PRIMARY KEY (vendor_name)
);
/******************
Create the VENDORS Audit table
***************/


DROP TABLE IF EXISTS VENDORS_audit;

CREATE TABLE VENDORS_audit(


vendor_name	nvarchar(50)	not null	comment 'the name of the vendor'
,vendor_address	nvarchar(100)	not null	comment 'The address of the vendor'
,vendor_city	nvarchar(50)	not null	comment 'the city of the vendor'
,vendor_state	nvarchar(50)	not null	comment 'the state of the vendor'
,vendor_contact_name	nvarchar(50)	not null	unique	comment 'the contact person at the vendor'
,vendor_phone_number	nvarchar(14)	not null	unique	comment 'the most useful phone number for this vendor'
,vendor_type	enum ('arena', 'food', 'equipment','ref')	not null	comment 'an option select for what this vendor gets caregorized in for our accounting'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the VENDORS table
***************/
DROP PROCEDURE IF EXISTS sp_update_VENDORS;
DELIMITER $$
CREATE PROCEDURE sp_update_VENDORS
(in vendor_name_param nvarchar(50)
,in vendor_address_param nvarchar(100)
,in vendor_city_param nvarchar(50)
,in vendor_state_param nvarchar(50)
,in vendor_contact_name_param nvarchar(50)
,in vendor_phone_number_param nvarchar(14)
,in vendor_type_param enum ('arena', 'food', 'equipment','ref')
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE VENDORS
 set vendor_address = vendor_address_param
,vendor_city = vendor_city_param
,vendor_state = vendor_state_param
,vendor_contact_name = vendor_contact_name_param
,vendor_phone_number = vendor_phone_number_param
,vendor_type = vendor_type_param
WHERE vendor_name=vendor_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the VENDORS table
***************/
DROP PROCEDURE IF EXISTS sp_delete_VENDORS;
DELIMITER $$
CREATE PROCEDURE sp_delete_VENDORS
(vendor_name_param nvarchar(50)
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM VENDORS
  WHERE vendor_name=vendor_name_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the VENDORS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_VENDORS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_VENDORS
(
vendor_name_param nvarchar(50)
)
 Begin 
 select 
vendor_name 
,vendor_address 
,vendor_city 
,vendor_state 
,vendor_contact_name 
,vendor_phone_number 
,vendor_type 

 FROM VENDORS
where vendor_name=vendor_name_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the VENDORS table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_VENDORS;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_VENDORS()
begin 
 SELECT 

vendor_name
,vendor_address
,vendor_city
,vendor_state
,vendor_contact_name
,vendor_phone_number
,vendor_type
 FROM VENDORS
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the VENDORS table
***************/
DROP PROCEDURE IF EXISTS sp_insert_VENDORS;
DELIMITER $$
CREATE PROCEDURE sp_insert_VENDORS(
in vendor_name_param nvarchar(50)
,in vendor_address_param nvarchar(100)
,in vendor_city_param nvarchar(50)
,in vendor_state_param nvarchar(50)
,in vendor_contact_name_param nvarchar(50)
,in vendor_phone_number_param nvarchar(14)
,in vendor_type_param enum ('arena', 'food', 'equipment','ref')
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  VENDORS
 values 
(vendor_name_param
,vendor_address_param
,vendor_city_param
,vendor_state_param
,vendor_contact_name_param
,vendor_phone_number_param
,vendor_type_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the VENDORS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_VENDORS_after_insert $$
CREATE TRIGGER tr_VENDORS_after_insert
AFTER insert ON VENDORS
for each row
begin
insert intoVENDORS_audit (
vendor_name 
,vendor_address 
,vendor_city 
,vendor_state 
,vendor_contact_name 
,vendor_phone_number 
,vendor_type 

, action_type
, action_date
, action_user
) values(
new.vendor_name 
,new.vendor_address 
,new.vendor_city 
,new.vendor_state 
,new.vendor_contact_name 
,new.vendor_phone_number 
,new.vendor_type 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the VENDORS table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_VENDORS_after_update $$
CREATE TRIGGER tr_VENDORS_after_update
AFTER UPDATE ON VENDORS
for each row
begin
insert intoVENDORS_audit (
vendor_name 
,vendor_address 
,vendor_city 
,vendor_state 
,vendor_contact_name 
,vendor_phone_number 
,vendor_type 

, action_type
, action_date
, action_user
) values(
new.vendor_name 
,new.vendor_address 
,new.vendor_city 
,new.vendor_state 
,new.vendor_contact_name 
,new.vendor_phone_number 
,new.vendor_type 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the INVOICES_RECEIVED table
***************/


DROP TABLE IF EXISTS INVOICES_RECEIVED;

CREATE TABLE INVOICES_RECEIVED(


invoice_sequence	int	not null	unique	comment 'The Auto incremetned id of the Vendor'
,invoice_id	nvarchar(250)	not null	comment 'The invoice_ID assigned by the issuer'
,vendor_name	nvarchar(50)	not null	comment 'the id of the vendor that issued this'
,receipt_date	date	not null	comment 'the date this invoice was received by us'
,amount	int	not null	comment 'the amount due on the invoice'
,payment_date	date	null	comment 'the date we paid this invoice'
,game_id	int	not null	comment 'The bout that this invoice is associated with.'
,CONSTRAINT INVOICES_RECEIVED_PK PRIMARY KEY (invoice_sequence)
);
/******************
Create the INVOICES_RECEIVED Audit table
***************/


DROP TABLE IF EXISTS INVOICES_RECEIVED_audit;

CREATE TABLE INVOICES_RECEIVED_audit(


invoice_sequence	int	not null	unique	comment 'The Auto incremetned id of the Vendor'
,invoice_id	nvarchar(250)	not null	comment 'The invoice_ID assigned by the issuer'
,vendor_name	nvarchar(50)	not null	comment 'the id of the vendor that issued this'
,receipt_date	date	not null	comment 'the date this invoice was received by us'
,amount	int	not null	comment 'the amount due on the invoice'
,payment_date	date	null	comment 'the date we paid this invoice'
,game_id	int	not null	comment 'The bout that this invoice is associated with.'
,action_type VARCHAR(50) NOT NULL COMMENT 'insert update or delete'
, action_date DATETIME NOT NULL COMMENT 'when it happened'
, action_user VARCHAR(255) NOT NULL COMMENT 'Who did it'
);
/******************
Create the update script for the INVOICES_RECEIVED table
***************/
DROP PROCEDURE IF EXISTS sp_update_INVOICES_RECEIVED;
DELIMITER $$
CREATE PROCEDURE sp_update_INVOICES_RECEIVED
(in invoice_sequence_param int
,in invoice_id_param nvarchar(250)
,in vendor_name_param nvarchar(50)
,in receipt_date_param date
,in amount_param int
,in payment_date_param date
,in game_id_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
UPDATE INVOICES_RECEIVED
 set invoice_id = invoice_id_param
,vendor_name = vendor_name_param
,receipt_date = receipt_date_param
,amount = amount_param
,payment_date = payment_date_param
,game_id = game_id_param
WHERE invoice_sequence=invoice_sequence_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the delete script for the INVOICES_RECEIVED table
***************/
DROP PROCEDURE IF EXISTS sp_delete_INVOICES_RECEIVED;
DELIMITER $$
CREATE PROCEDURE sp_delete_INVOICES_RECEIVED
(invoice_sequence_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
DELETE FROM INVOICES_RECEIVED
  WHERE invoice_sequence=invoice_sequence_param

 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the retreive by key script for the INVOICES_RECEIVED table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_pk_INVOICES_RECEIVED;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_pk_INVOICES_RECEIVED
(
invoice_sequence_param int
)
 Begin 
 select 
invoice_sequence 
,invoice_id 
,vendor_name 
,receipt_date 
,amount 
,payment_date 
,game_id 

 FROM INVOICES_RECEIVED
where invoice_sequence=invoice_sequence_param
 ; END $$
 DELIMITER ;
/******************
Create the retreive by all script for the INVOICES_RECEIVED table
***************/
DROP PROCEDURE IF EXISTS sp_retreive_by_all_INVOICES_RECEIVED;
DELIMITER $$
CREATE PROCEDURE sp_retreive_by_all_INVOICES_RECEIVED()
begin 
 SELECT 

invoice_sequence
,invoice_id
,vendor_name
,receipt_date
,amount
,payment_date
,game_id
 FROM INVOICES_RECEIVED
 ;
 END $$ 
 DELIMITER ;
/******************
Create the insert script for the INVOICES_RECEIVED table
***************/
DROP PROCEDURE IF EXISTS sp_insert_INVOICES_RECEIVED;
DELIMITER $$
CREATE PROCEDURE sp_insert_INVOICES_RECEIVED(
in invoice_sequence_param int
,in invoice_id_param nvarchar(250)
,in vendor_name_param nvarchar(50)
,in receipt_date_param date
,in amount_param int
,in payment_date_param date
,in game_id_param int
)
begin 
declare sql_error TINYINT DEFAULT FALSE;
declare update_count tinyint default 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
SET sql_error = true;
START TRANSACTION;
INSERT INTO  INVOICES_RECEIVED
 values 
(invoice_sequence_param
,invoice_id_param
,vendor_name_param
,receipt_date_param
,amount_param
,payment_date_param
,game_id_param
)
 ; if sql_error = FALSE then 
 SET update_count = row_count(); 
 COMMIT;
 ELSE
 SET update_count = 0;
 ROLLBACK;
 END IF;
 select update_count as 'update count'
 ; END $$
 DELIMITER ;
/******************
Create the update trigger script for the INVOICES_RECEIVED table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_INVOICES_RECEIVED_after_insert $$
CREATE TRIGGER tr_INVOICES_RECEIVED_after_insert
AFTER insert ON INVOICES_RECEIVED
for each row
begin
insert intoINVOICES_RECEIVED_audit (
invoice_sequence 
,invoice_id 
,vendor_name 
,receipt_date 
,amount 
,payment_date 
,game_id 

, action_type
, action_date
, action_user
) values(
new.invoice_sequence 
,new.invoice_id 
,new.vendor_name 
,new.receipt_date 
,new.amount 
,new.payment_date 
,new.game_id 

 , 'insert'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
/******************
Create the insert trigger script for the INVOICES_RECEIVED table
***************/
DELIMITER $$
DROP TRIGGER IF EXISTS tr_INVOICES_RECEIVED_after_update $$
CREATE TRIGGER tr_INVOICES_RECEIVED_after_update
AFTER UPDATE ON INVOICES_RECEIVED
for each row
begin
insert intoINVOICES_RECEIVED_audit (
invoice_sequence 
,invoice_id 
,vendor_name 
,receipt_date 
,amount 
,payment_date 
,game_id 

, action_type
, action_date
, action_user
) values(
new.invoice_sequence 
,new.invoice_id 
,new.vendor_name 
,new.receipt_date 
,new.amount 
,new.payment_date 
,new.game_id 

 , 'update'-- action_type
, NOW()-- action_date
,  CURRENT_USER()-- action_user
)
;
end  $$
DELIMITER ;
   ;
