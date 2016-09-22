
use CAPSTONE_DB

-- ACCOUNT --

create table [User]
(
	id int identity(1,1),
	accId int not null,
	firtName varchar(10) not null,
	middleName varchar(10) not null,
	lastName varchar(10) not null,
	username varchar(50) not null,			-- real name and nickname
	DOB date,
	[des] varchar(500),

	primary key (id),
	foreign key (accId) references account(id)
)

create table Account
(
	id int identity(1,1),
	email varchar(100) not null,
	pass varchar(50) not null,
	[role] bit not null,
	
	primary key (id)
)

create table Author
(
	id int identity(1,1),
	firtName varchar(10) not null,
	middleName varchar(10) not null,
	lastName varchar(10) not null,
	penname varchar(100),
	SSN varchar(20),
	country varchar(50) not null,
	city varchar(50),
	district varchar(50),
	ward varchar(50),
	dateOfBirth date,
	dateOfDead date,
	
	primary key (id)
)

create  table UserAuthor
(
	userId int not null,
	authorId int not null,
	accId int not null,
	
	foreign key (userId) references [User](id),
	foreign key (authorId) references Author(id),
	foreign key (accId) references Account(id)
)

-- BOOK --

create table Category
(
	id int identity(1,1),
	[category] varchar(100),
	
	primary key (id)
)

create table Publisher
(
	id int identity(1,1),
	name varchar(100),
	[address] varchar(200),
	avgRate float,
	
	primary key (id)
)

create table Book
(
	id int identity(1,1),
	name varchar(500) not null,
	numRate int,
	avgRate float,
	
	primary key (id),
)

create table BookCategory
(
	bookId int not null,
	categoryId int not null,
	
	foreign key (bookId) references Book(id),
	foreign key (categoryId) references Category(id)
)

create table BookAuthor
(
	bookId int not null,
	authorId int not null,
	
	foreign key (bookId) references Book(id),
	foreign key (authorId) references Author(id),
)

create table BookPublisher
(
	bookId int not null,
	publisherId int not null,
	
	foreign key (bookId) references Book(id),
	foreign key (publisherId) references Publisher(id)
)

-- RECOMMENDATION --

create table Recommendation
(
	id int identity(1,1),
	userId int not null,
	content varchar(1000),
	[time] date not null,
	numOfLike int not null,
	numOfShare int not null,
	numOfComment int not null,
	[status] bit not null,			-- is edited?
	[type] bit not null,			-- personal or group?
	groupId int,
	
	primary key (id),
	foreign key (userId) references [User](id),
	foreign key (groupId) references [Group](id)
)

create table [Like]
(
	id int identity(1,1),
	recommendId int not null,
	userId int not null,
	[time] datetime not null,
	
	primary key (id),
	foreign key (recommendId) references Recommendation(id),
	foreign key (userId) references [User](id)
)

create table Comment
(
	id int identity(1,1),
	recommendId int not null,
	userId int not null,
	[time] datetime not null,
	content varchar(1000) not null,
	numOfLike int not null,
	parentId int,
	[status] bit not null,  -- is edited?
	
	primary key (id),
	foreign key (recommendId) references Recommendation(id),
	foreign key (userId) references [User](id),
	foreign key (parentId) references Comment(id)
)

create table Share
(
	id int identity(1,1),
	recommendId int not null,
	userId int not null,
	[time] datetime not null,
	content varchar(1000),
	
	primary key (id),
	foreign key (recommendId) references Recommendation(id),
	foreign key (userId) references [User](id),
)

create table [Notification]
(
	id int identity(1,1),
	userId int not null,				--user who has been notified
	content varchar(200) not null,
	[time] datetime not null,
	notiUrl varchar(500) not null,
	[status] int not null,				-- read or not
	
	primary key (id),
	foreign key (userId) references [User](id),
)

-- INTERACT --

create table Follow
(
	followerId int not null,
	followingId int not null,
	[time] datetime not null,
	
	foreign key (followerId) references [User](id),
	foreign key (followingId) references [User](id),
)

create table [Conversation]
(
	id int identity(1,1),
	userId1	int not null,
	userId2 int not null,
	[time] datetime not null,  -- time create convesation
	
	primary key (id),
	foreign key (userId1) references [User](id),
	foreign key (userId2) references [User](id),
)

create table [Message]
(
	id int identity(1,1),
	conversationId int not null,
	senderId int not null,
	content varchar(200) not null,
	[time] datetime not null,
	[status] int not null, --sending, send sucessfully, fail to send
	
	primary key (id),
	foreign key (conversationId) references [Conversation](id),
	foreign key (senderId) references [User](id),
)

create table [Group]
(
	id int identity(1,1),
	userId int not null,		 -- id of iniator
	name varchar(100) not null,
	bookId int,
	authorId int,
	
	primary key (id),
	foreign key (userId) references [User](id),
	foreign key (bookId) references Book(id),
	foreign key (authorId) references Author(id),
)

create table GroupMember
(
	userId int not null,
	groupId int not null,
	[role] int not null,			-- member/admin
	
	foreign key (userId) references [User](id),
	foreign key (groupId) references [Group](id),
)

create table ReportMessage
(
	id int identity(1,1),
	reporterId int not null,
	reportedId int not null,
	[time] datetime not null,
	content varchar(500) not null,
	
	primary key (id),
	foreign key (reporterId) references [User](id),
	foreign key (reportedId) references [User](id),
)