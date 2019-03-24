CREATE TABLE Yelp_Business
{
	business_id 	VARCHAR(30) PRIMARY KEY, 
	name 		VARCHAR(50),
	is_open 	tinyint,
	review_count    integer,
	stars		tinyint 	CHECK(stars <= 5),
	reviewrating	tinyint	CHECK(reviewrating <= 5),
	neighborhood  VARCHAR(100),
	numCheckins   integer,
	state		VARCHAR(15),
	postal_code	VARCHAR(5),
	address		VARCHAR(100),
	city		VARCHAR(30).
	latitude		real,
	longitude	real
};

CREATE TABLE Yelp_User
{
	user_id		VARCHAR(30) PRIMARY KEY,
	average_stars	tinyint	CHECK(average_stars <= 5),
	cool		integer,
	elite		integer,
	fans		integer,
	funny		integer,
	name		VARCHAR(50),
	review_count	integer,
	useful		integer,
	yelping_since	DATE
};

CREATE TABLE isFriend 
 /*Not sure if I got this right.. but to compare if two people are friend, we need 2 user ids. That was the reason why I made 2 user_id as PRIMARY KEY.*/
{
	user_id		VARCHAR(50) REFERENCES Yelp_user(user_id),
	PRIMARY KEY (user_id, user_id)
};

CREATE TABLE Review
{
	review_id	VARCHAR(30)	PRIMARY KEY,
	user_id		VARCHAR(30)	REFERENCES Yelp_User(user_id),
	business_id	VARCHAR(30) REFERENCES Yelp_Business(business_id),
	stars		tinyint	CHECK(stars<=5),			
	date		DATE,
	text		text,
	useful		integer,	
	funny		integer,
	cool		integer
};

CREATE TABLE has
{
	business_id	VARCHAR(30) REFERENCES Yelp_Business(business_id),
	review_id	VARCHAR(30) REFERENCES Review(review_id),
	user_id		VARCHAR(30) REFERENCES Yelp_User(user_id),
	PRIMARY KEY (business_id, review_id, user_id)
};

CREATE TABLE Checkin
{
	business_id	VARCHAR(30) REFERENCES Yelp_Business(business_id),
	date		DATE,
	morning	tinyint,	
	afternoon	tinyint,
	evening		tinyint,
	PRIMARY KEY (business_id, date)
};

CREATE TABLE Attributes
{
	business_id	VARCHAR(30) PRIMARY KEY,
	RestaurantsTakeOut	tinyint,
	GoodForKids	tinyint,
	RestaurantsDelivery	tinyint,
	BikeParking	tinyint,
	RestaurantsPriceRange2	 tinyint,
	BusinessAcceptsCreditCards	tinyint,
	OutdoorSeating		tinyint,
	RestaurantsReservations	tinyint,
	NoiseLevel	VARCHAR(10),
	RestaurantsAttire	VARCHAR(10),
	Wifi	tinyint,
	RestaruatnsGoodForGroups	tinyint,
	RestaurantsTableService		tinyint,
	HasTV		tinyint,
	Caters		tinyint,
	Alcohol		VARCHAR(10),
	
	breakfast	tinyint,
	dinner		tinyint,
	dessert		tinyint,
	latenight	tinyint,
	lunch		tinyint,
	brunch		tinyint
};
