/*removing the old tables*/
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
/*************************/

CREATE TABLE Yelp_Business_Entity
(
	business_id 	VARCHAR(30) PRIMARY KEY, 
	name 			VARCHAR(50),
	is_open 		INT,
	review_count    INT,
	stars			INT CHECK(stars <= 5),
	reviewrating	INT	CHECK(reviewrating <= 5),
	neighborhood  	VARCHAR(100),
	numCheckins   	INT,
	state			VARCHAR(15),
	postal_code		VARCHAR(5),
	address			VARCHAR(100),
	city			VARCHAR(30),
	latitude		REAL,
	longitude		REAL
);

CREATE TABLE Yelp_User_Entity
(
	user_id			VARCHAR(30) PRIMARY KEY,
	average_stars	INT	CHECK(average_stars <= 5),
	cool			INT,
	elite			INT,
	fans			INT,
	funny			INT,
	name			VARCHAR(50),
	review_count	INT,
	useful			INT,
	yelping_since	DATE,
    isFriend_to 	VARCHAR(30) null REFERENCES Yelp_User_Entity(user_id)
    
);

CREATE TABLE Review_Entity
(
	review_id		VARCHAR(30)	PRIMARY KEY,
	stars			INT	CHECK(stars<=5),			
	date			DATE,
	text			TEXT,
	useful			INT,	
	funny			INT,
	cool			INT
);

CREATE TABLE hasReviewOfBussinessByUsers_Relationship
(
	business_id		VARCHAR(30),
	review_id		VARCHAR(30),
	user_id			VARCHAR(30), 
	FOREIGN KEY (business_id)	REFERENCES 	Yelp_Business_Entity(business_id),
   	FOREIGN KEY (review_id) 	REFERENCES  Review_Entity(review_id),
    FOREIGN KEY (user_id) 		REFERENCES  Yelp_User_Entity(user_id)
    
);

CREATE TABLE Checkin_Entity
(
	business_id		VARCHAR(30),
	date			DATE,
	morning			INT,	
	afternoon		INT,
	evening			INT,
	PRIMARY KEY (business_id, date),
    FOREIGN KEY (business_id) REFERENCES Yelp_Business_Entity(business_id)
);

CREATE TABLE Categories_Entity
(
	business_id		VARCHAR(30),
    name 			VARCHAR(30),
    PRIMARY KEY (business_id, name),
    FOREIGN KEY (business_id) REFERENCES Yelp_Business_Entity(business_id)
);

CREATE TABLE Attributes_Entity
(
	business_id		VARCHAR(30),
	name 			VARCHAR(30) UNIQUE,
    value 			VARCHAR(10),
    PRIMARY KEY (business_id, name),
    FOREIGN KEY (business_id) REFERENCES Yelp_Business_Entity(business_id)
);

CREATE TABLE Hour_Entity
(
    business_id		VARCHAR(30),
	the_date 		DATE,
    the_time 		TIME,
    PRIMARY KEY (business_id, the_date),
    FOREIGN KEY (business_id) REFERENCES Yelp_Business_Entity(business_id)
);


