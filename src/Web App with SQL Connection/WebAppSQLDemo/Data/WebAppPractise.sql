CREATE TABLE Course
(
	CourseID int,
	CourseName varchar(1000),
	Rating numeric(2,1)
)

INSERT INTO COURSE(CourseID,CourseName,Rating) VALUES(1,'AZ-204 Developing Azure Solution',4.5)
INSERT INTO COURSE(CourseID,CourseName,Rating) VALUES(2,'AZ-303 Architecting Azure Solution',4.6)
INSERT INTO COURSE(CourseID,CourseName,Rating) VALUES(2,'DP-203 Azure Data Engineer',4.7)

SELECT * FROM Course