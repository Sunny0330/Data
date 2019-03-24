/* Find the distinct courses that ‘SYS’ track students in 'CptS' major are enrolled in.
Return the courseno and credits for those courses. Return results sorted based on courseno. */
SELECT DISTINCT enroll.courseno, course.credits
	FROM enroll,student,course
	WHERE enroll.courseno = course.courseno AND student.sID = enroll.sID AND student.trackcode = 'SYS' AND student.major = 'CptS' 
	ORDER BY courseno;
    
/* Find the sorted names, ids, majors and track codes of the students 
who are enrolled in more than 18 credits (19 or above).  */
SELECT s.sName, s.sID, s.major, s.trackcode, SUM(c.credits)
	FROM (SELECT * FROM student ORDER BY student.sName) AS s
    LEFT OUTER JOIN enroll AS e ON s.sID = e.sID
    LEFT OUTER JOIN course AS c ON e.courseno = c.courseno
    GROUP BY s.sName, s.sID, s.major,s.trackcode
    HAVING SUM(c.credits) > 18 
    ORDER BY s.sName;
    
/* Find the courses that only 'SE' track students in 'CptS major 
have been enrolled in. Give an answer without using the set EXCEPT operator. */
SELECT DISTINCT courseno
	FROM enroll 
    WHERE courseno not in
	(SELECT s.courseno
        FROM enroll s
        WHERE sid in (SELECT e1.sid
                FROM enroll e1, student s1
                WHERE e1.sID = s1.sID  
                AND NOT EXISTS (SELECT s2.sID FROM student s2 WHERE s1.sID = s2.sID AND s2.trackcode = 'SE')))
	ORDER BY courseno
    
/* Find the students who have enrolled in the courses that Diane enrolled and earned the same grade Diane 
earned in those courses. Return the student name, sID, major as well as the courseno and grade for those courses. */
SELECT s.sname, s.sID, s.major,e.courseno, e.grade
	FROM student s, enroll e
    WHERE s.sID = e.sID AND e.courseno in (SELECT e1.courseno
                        FROM student s1, enroll e1
    					WHERE s1.sID = e1.sID AND s1.sname = 'Diane')
                        AND e.grade in (SELECT e1.grade
                        FROM student s1, enroll e1
    					WHERE s1.sID = e1.sID AND s1.sname = 'Diane')
                        AND s.sname != 'Diane'
	ORDER BY s.sname

/* Find the students in 'CptS' major who are not enrolled in any classes. Return their names
and sIDs. (Note: Give a solution using OUTER JOIN) */
SELECT student.sname, student.sID
	FROM student LEFT OUTER JOIN enroll on enroll.sID = student.sID
    WHERE student.major = 'CptS' AND student.sID not in (Select enroll.sID from enroll)
    GROUP BY student.sname,student.sID
	ORDER  BY student.sname

/* Find the courses given in the ‘Sloan’ building which have enrolled more students than
their enrollment limit. Return the courseno, enroll_limit, and the actual enrollment for
those courses. */
SELECT course.courseno, course.enroll_limit, COUNT(enroll.sID) as enrollnum
	FROM course LEFT OUTER  JOIN enroll on enroll.courseno = course.courseno
    WHERE course.classroom LIKE 'Sloan%'
    GROUP BY course.courseno, course.enroll_limit
    HAVING COUNT(enroll.sID) > course.enroll_limit
    ORDER BY course.courseno


/* Find the students who took all the courses required by his/her track in his/her major. */
SELECT s.sID, s.sname, s.major
	FROM student AS s INNER JOIN enroll AS e ON s.sID = e.sID INNER JOIN trackrequirements as t ON t.courseno = e.courseno AND t.trackcode = s.trackcode AND t.major= s.major
    GROUP BY s.sID, s.sname, s.major
    HAVING COUNT(e.courseno) IN (SELECT COUNT(trackrequirements.courseno)
                                    FROM course,trackrequirements
                                    WHERE course.courseno = trackrequirements.courseno
                                    GROUP BY trackrequirements.major, trackrequirements.trackcode
                                    ORDER BY trackrequirements.major, trackrequirements.trackcode)
    ORDER BY s.sname
	
/* Find 'CptS' major students who enrolled in a course for which there exists a prerequisite
that the student got a grade lower than “2”. (For example, Alice (sid: 12583589) was
enrolled in CptS355 but had a grade 1.75 in prerequisite CptS223.) Return the names
and sIDs of those students and the courseno of the course (i.e., the course whose prereq
had a low grade).*/
SELECT s.sname, s.sID, p.courseno
 	FROM student s, enroll e, prereq p
    WHERE s.sID = e.sID AND e.courseno = p.precourseno 
    AND p.courseno IN (SELECT e.courseno
                        FROM enroll AS e INNER JOIN prereq AS p on e.courseno = p.courseno
                        GROUP BY e.courseno
                        ORDER BY e.courseno) 
    AND s.major = 'CptS' AND s.trackcode = 'SYS'
    AND e.grade < 2
        
/* For each ‘CptS’ course, find the percentage of the students who failed the course.
Assume a passing grade is 2 or above. (Note: Assume students who didn’t earn a grade in
class should be excluded in average calculation. Also assume all CptS courses start with
the ‘CptS’ prefix).*/
SELECT courseno , FLOOR((CAST(COUNT(CASE WHEN grade >= 2 then 1 else NULL end) as FLOAT) /  CAST(COUNT(grade) as FLOAT) * 100)) AS passrate
	FROM enroll 
    WHERE courseno LIKE 'CptS%'
 	GROUP BY courseno
    ORDER BY courseno
    
/* Consider the following relational algebra expression.
(i) explain what the expression is doing,
	ANSWER: The purpose of the expression is finding all courses with 2 or more prequire courses.
(ii) write an equivalent SQL query. 
*/
(SELECT  p.courseno, COUNT(p.precourseno) as pCount
        FROM course AS c NATURAL JOIN prereq AS p
        GROUP BY p.courseno
 		HAVING COUNT(p.precourseno) >= 2
        ORDER BY p.courseno) 
    

