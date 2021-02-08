-- This is SQL for a TSQL i.e. (MS) Sql server 

---########query 1 reverting###############
-- use mssql1902540;

ALTER TABLE Twang.UserPerms       drop CONSTRAINT  fk_Userid_4;   
ALTER TABLE Twang.AccessLog       drop CONSTRAINT  fk_Userid_3;   
ALTER TABLE Twang.UserBooking     drop CONSTRAINT  fk_Userid_2;   
ALTER TABLE Twang.UserBooking     drop CONSTRAINT  fk_Bookingid;   
ALTER TABLE Twang.Booking         drop CONSTRAINT  FK_roomid;
ALTER TABLE Twang.Booking         drop CONSTRAINT  fk_Userid_1;
ALTER TABLE Twang.UserAccessStore drop CONSTRAINT  fk_Userid;   

if OBJECT_ID('Twang.UserPerms') is not null
    drop table Twang.UserPerms;

if OBJECT_ID('Twang.AccessLog') is not null
    drop table Twang.AccessLog;

if OBJECT_ID('Twang.UserAccessStore') is not null
    drop table Twang.UserAccessStore;

if OBJECT_ID('Twang.Booking') is not null
    drop table Twang.Booking;

if OBJECT_ID('Twang.Room') is not null
    drop table Twang.Room;

if OBJECT_ID('Twang.Users') is not null
    drop table Twang.Users;

if OBJECT_ID('Twang.UserBooking') is not null
    drop table Twang.UserBooking;

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'Twang')
    drop Schema Twang

---########query 2###############

create Schema Twang;

---########query 3###############




create table Twang.Users(
    id int not null,
    uname varchar(255) not null,
    pkey varchar(255) not null,
    primary key(id)
)

create table Twang.Room(
    id int not null,
    rname varchar(255) not null ,
    capacity int not null check (capacity > 0)
    unique (rname),
    primary key(id),

)

create table Twang.UserAccessStore(
    id int not null ,
    uid int not null,
    accessip varchar(16) not null,
    addtime datetime not null DEFAULT getdate(),
    Logofftime datetime DEFAULT getdate(),
    primary key(id),
    CONSTRAINT fk_Userid FOREIGN KEY (uid) REFERENCES Twang.Users(id)
)

create table Twang.Booking(
    id int not null,
    rid			int not null ,
    title		varchar(255) not null ,
    starttime		datetime not null check (starttime>getdate()),
    duration		float not null check (duration>0),
    bookingTime		datetime not null DEFAULT getdate(),
    bookingDay  	date not null DEFAULT getdate(),
    oid int not null ,
    primary key(id),
    CONSTRAINT FK_roomid FOREIGN KEY (rid) REFERENCES Twang.Room(id),
    CONSTRAINT fk_Userid_1 FOREIGN KEY (oid) REFERENCES Twang.Users(id)
)

create table Twang.UserBooking(
    id int not null,
    uid int not null ,
    bid int not null,
    primary key(id),
    CONSTRAINT fk_Userid_2 FOREIGN KEY (uid) REFERENCES Twang.Users(id),
    CONSTRAINT fk_Bookingid FOREIGN KEY (bid) REFERENCES Twang.Booking(id)
)



create table Twang.AccessLog(
    id		int not null IDENTITY,
    uid		int not null ,
    Query	varchar(400) not null ,
	ipaddress varchar(400) not null ,
	Added	datetime DEFAULT GETDATE(),
    primary key(id),
    CONSTRAINT fk_Userid_3 FOREIGN KEY (uid) REFERENCES Twang.Users(id),
)

create table Twang.UserPerms(
    id		int not null IDENTITY,
    uid		int not null ,
    tabl	varchar(30) not null ,
	attribute varchar(1) not null ,
    primary key(id),
    CONSTRAINT fk_Userid_4 FOREIGN KEY (uid) REFERENCES Twang.Users(id),
)




insert into Twang.Users (id,uname,pkey) values (-1,'Admin User','randomkey');
insert into Twang.Users (id,uname,pkey) values (11630816,'Elsa Duncan','randomkey');
insert into Twang.Users (id,uname,pkey) values (12638001,'Sandy Grant','randomkey');
insert into Twang.Users (id,uname,pkey) values (20806981,'Allan Johnstone','randomkey');
insert into Twang.Users (id,uname,pkey) values (27584059,'Chloe Walker','randomkey');
insert into Twang.Users (id,uname,pkey) values (28875222,'Natan Hamilton','randomkey');
insert into Twang.Users (id,uname,pkey) values (31951484,'Luka Gibson','randomkey');
insert into Twang.Users (id,uname,pkey) values (38277039,'Elsie Black','randomkey');
insert into Twang.Users (id,uname,pkey) values (44646137,'Mirin Burns','randomkey');
insert into Twang.Users (id,uname,pkey) values (47669629,'Fearne Cunningham','randomkey');
insert into Twang.Users (id,uname,pkey) values (50125210,'Brooklyn Douglas','randomkey');
insert into Twang.Users (id,uname,pkey) values (51702313,'Murron Hunter','randomkey');
insert into Twang.Users (id,uname,pkey) values (55375417,'Angel Kelly','randomkey');
insert into Twang.Users (id,uname,pkey) values (58104704,'Alice Sinclair','randomkey');
insert into Twang.Users (id,uname,pkey) values (64349421,'Elena MacDonald','randomkey');
insert into Twang.Users (id,uname,pkey) values (66794806,'Carrie Walker','randomkey');
insert into Twang.Users (id,uname,pkey) values (70579719,'Eloise Wood','randomkey');
insert into Twang.Users (id,uname,pkey) values (75251598,'Kacie Thompson','randomkey');
insert into Twang.Users (id,uname,pkey) values (78191010,'Murron Paterson','randomkey');
insert into Twang.Users (id,uname,pkey) values (78602723,'Nathan Hay','randomkey');
insert into Twang.Users (id,uname,pkey) values (79718733,'Jak Jamieson','randomkey');
insert into Twang.Users (id,uname,pkey) values (80076475,'Ailidh Russell','randomkey');
insert into Twang.Users (id,uname,pkey) values (82968199,'Toby McLean','randomkey');
insert into Twang.Users (id,uname,pkey) values (90006667,'Ellie Johnston','randomkey');
insert into Twang.Users (id,uname,pkey) values (92656774,'Michael Bruce','randomkey');
insert into Twang.Users (id,uname,pkey) values (99036434,'Kirstie Wright','randomkey');


--- Used in the rest api to add extra privlages to a specified user (not every table can be updated by this user)
insert into Twang.UserPerms (uid,tabl,attribute) values (-1,'Twang.Users',char(1+2+4+8));
insert into Twang.UserPerms (uid,tabl,attribute) values (-1,'Twang.UserBooking',char(1+2+4+8));
insert into Twang.UserPerms (uid,tabl,attribute) values (-1,'Twang.Room',char(1+2+4+8));

insert into Twang.Booking(id,rid,title,starttime,duration,bookingTime,bookingDay,oid) values(1,1,'Random title',DATEADD(minute, 1, GETDATE()),30,getdate(),convert(date,getdate()),11630816);
insert into Twang.UserBooking(id,uid,bid) values(1,11630816,1);

insert into Twang.Room(id,rname,capacity) values(1,'Far Far Away',6);
insert into Twang.Room(id,rname,capacity) values(2,'Hogwarts',4);
insert into Twang.Room(id,rname,capacity) values(3,'Hundred Acre Wood',10);
insert into Twang.Room(id,rname,capacity) values(4,'Narnia',15);
insert into Twang.Room(id,rname,capacity) values(5,'Neverland',6);



---##########END#################
