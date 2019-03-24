DROP TABLE IF EXISTS Enroll; 

CREATE TABLE Enroll (
	courseno     VARCHAR(7),
	sID  	CHAR(8),
	grade 	FLOAT NOT NULL,
	PRIMARY KEY (courseNo, sID),	
	FOREIGN KEY (courseNo) REFERENCES Course(courseNo),
	FOREIGN KEY (sID) REFERENCES Student(sID)
);


INSERT INTO Enroll(courseNo,sID,grade) VALUES('MATH115','12584189',3),
											 ('MATH115','12534189',2),
											 ('MATH115','12524189',4),
											 ('CE211','12584189',3.5),
											 ('CE211','12534189',2.5),
											 ('CE317','12584189',3),
											 ('CE317','12534189',2.25),
											 ('MATH140','12524189',4),
											 ('CE351','12584189',2.5),
											 ('CE330','12584189',2.75),
											 ('CE330','12534189',2.75),
											 ('CE417','12584189',3),
											 ('MATH251','12583289',2.5),
											 ('CHE110','12583289',2.75),
											 ('CHE211','12583289',3),
											 ('CHE321','12583289',3.25),
											 ('MATH251','12582989',3),
											 ('CHE110','12582989',2.5),
											 ('CHE211','12582989',2.75),
											 ('CHE321','12582989',3),
											 ('CHE334','12582989',3.25),
											 ('CHE398','12582989',3),
											 ('CHE433','12582989',3.25),
											 ('MATH251','12582689',4),
											 ('CHE110','12582689',4),
											 ('CHE211','12582689',4),
											 ('CHE321','12582689',4),
											 ('CHE334','12582689',4),
											 ('CHE398','12582689',3.5),
											 ('CHE433','12582689',4),
											 ('CHE451','12582689',4),
											 ('CHE476','12582689',3.75),
											 ('CHE495','12582689',4),
											 ('CHE498','12582689',3.75),
											 ('EE214','12582689',3.75),
											 ('MATH251','12579189',2),
											 ('CHE110','12579189',2.5),
											 ('CHE211','12579189',2.75),
											 ('CHE321','12579189',2.25),
											 ('CHE334','12579189',3),
											 ('CHE398','12579189',3.25),
											 ('CHE433','12579189',3),
											 ('CHE495','12579189',3),
											 ('MATH251','12578189',2.5),
											 ('CHE110','12578189',2.5),
											 ('CHE211','12578189',2.5),
											 ('CHE321','12578189',2.25),
											 ('CHE476','12578189',2),
											 ('MATH251','12577189',1.75),
											 ('CHE110','12577189',2),
											 ('CHE211','12577189',2.25),											 
											 ('MATH171','12584789',3),
											 ('MATH172','12584789',3),
											 ('CptS121','12584789',3),
											 ('CptS122','12584789',3),
											 ('CptS223','12584789',2.75),
											 ('CptS260','12584789',3),
											 ('CptS322','12584789',3),
											 ('CptS323','12584789',3),
											 ('CptS355','12584789',3.25),
											 ('CptS421','12584789',3),
											 ('CptS423','12584789',3),
											 ('CptS360','12584789',3),
											 ('CptS460','12584789',3.25),
											 ('CptS451','12584789',3),
											 ('CptS422','12584789',4),
											 ('CptS317','12584789',4),
											 ('MATH171','12583589',2.5),
											 ('MATH172','12583589',2.5),
											 ('CptS121','12583589',2.25),
											 ('CptS122','12583589',2),
											 ('CptS223','12583589',1.75),
											 ('CptS260','12583589',2),
											 ('CptS322','12583589',2.5),
											 ('CptS317','12583589',3),
											 ('CptS355','12583589',2.5),
											 ('CptS421','12583589',2),
											 ('CptS423','12583589',2),
											 ('CptS360','12583589',2),
											 ('CptS460','12583589',2),
											 ('CptS451','12583589',2),
											 ('MATH216','12583589',2),
											 ('MATH220','12583589',2),
											 ('CptS223','12582389',2.75),
											 ('CptS260','12582389',3),
											 ('CptS355','12582389',3.25),
											 ('CptS322','12582389',4),
											 ('CptS487','12582389',4),
											 ('CptS484','12582389',3.5),
											 ('CptS323','12582389',3.75),
											 ('CptS451','12582389',3.75),
											 ('MATH216','12582389',3),
											 ('MATH220','12582389',3),
											 ('MATH220','12581189',3),
											 ('CptS260','12581189',1.5),
											 ('CptS355','12581189',1),
											 ('CptS322','12581189',2),
											 ('CptS323','12581189',2),
											 ('CptS421','12581189',2),
											 ('CptS423','12581189',1.75),
											 ('CptS360','12581189',2.5),
											 ('CHE211','12581189',2.75),
											 ('EE221','12570189',3),
											 ('ME220','12567189',3.25),
											 ('ME301','12567189',3),
											 ('ME303','12567189',3.75),
											 ('ME305','12567189',4),
											 ('MATH171','12567189',4),
											 ('MATH172','12567189',4),
											 ('MATH115','12566189',2.75),
											 ('MATH140','12584489',3),
											 ('ME116','12584489',3),
											 ('ME212','12584489',3.25),
											 ('ME216','12584489',3),
											 ('ME220','12584489',3.25),
											 ('ME301','12584489',4),
											 ('ME303','12584489',3.75),
											 ('ME305','12584489',4),
											 ('ME310','12584489',3),
											 ('ME311','12584489',3),
											 ('ME313','12584489',3),
											 ('ME316','12584489',3.25),
											 ('ME348','12584489',3),
											 ('ME401','12584489',3.25),
											 ('ME402','12584489',4),
											 ('ME431','12584489',3),
											 ('ME439','12584489',3),
											 ('ME474','12584489',3),
											 ('ME495','12584489',3),
											 ('ME499','12584489',2.75),
											 ('EE499','12584489',2.75),
											 ('MATH140','12573189',2.75),
											 ('MATH115','12573189',2.75),
											 ('ME301','12573189',2.75),
											 ('EE499','12573189',2.75),
											 ('EE499','12570189',3.75);