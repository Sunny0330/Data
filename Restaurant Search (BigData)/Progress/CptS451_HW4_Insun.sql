--Insun Lee (11440888)

--Q1
SELECT course.courseno, course.credits
FROM enroll, student, course
WHERE student.trackcode='SYS' AND student.major='CptS' AND student.sid = enroll.sid AND enroll.courseno = course.courseno
ORDER BY enroll.courseno

--Q2
SELECT student.sname, student.sid, student.major, student.trackcode, SUM(credits) as sum
FROM (course LEFT OUTER JOIN enroll ON course.courseno = enroll.courseno) LEFT OUTER JOIN student ON enroll.sid = student.sid
GROUP BY student.sid, student.sname, student.major, student.trackcode
HAVING SUM (credits) >19
ORDER BY student.sname

--Q3
SELECT DISTINCT courseno
FROM enroll 
WHERE courseno not in
	(SELECT E.courseno
	FROM enroll E
	WHERE sid in 
	 	(SELECT E1.sid
		FROM enroll E1, student S
                       WHERE E1.sID = S.sID AND NOT EXISTS 
		(SELECT S1.sID 
		FROM student S1 
		WHERE S.sID = S1.sID AND S1.trackcode = 'SE')))
ORDER BY courseno

--Q4
SELECT student.sname, student.sID, student.major, enroll.courseno, enroll.grade
FROM student, enroll
WHERE student.sID = enroll.sID AND enroll.courseno in 
	(SELECT E.courseno
            FROM student S, enroll E
            WHERE S.sID = E.sID AND S.sname = 'Diane') AND enroll.grade in 
		(SELECT E.grade
                       FROM student S, enroll E
    	           WHERE S.sID = E.sID AND S.sname = 'Diane') AND student.sname != 'Diane'
ORDER BY student.sname

--Q5
SELECT student.sname, student.sID
FROM student LEFT OUTER JOIN enroll on enroll.sID = student.sID
WHERE student.major = 'CptS' AND student.sID not in 
	(Select enroll.sID From enroll)
GROUP BY student.sname,student.sID
ORDER  BY student.sname

--Q6
SELECT course.courseno, course.enroll_limit, COUNT(enroll.sID) as enrollnum
FROM course LEFT OUTER  JOIN enroll on enroll.courseno = course.courseno
WHERE course.classroom LIKE 'Sloan%'
GROUP BY course.courseno, course.enroll_limit
HAVING COUNT(enroll.sID) > course.enroll_limit
ORDER BY course.courseno

--Q7
SELECT student.sID, student.sname, student.major
FROM student INNER JOIN enroll ON student.sID = enroll.sID INNER JOIN trackrequirements ON trackrequirements.courseno = enroll.courseno AND trackrequirements.trackcode = student.trackcode AND trackrequirements.major= student.major
GROUP BY student.sID, student.sname, student.major
HAVING COUNT(enroll.courseno) IN 
	(SELECT COUNT(trackrequirements.courseno)
            FROM course,trackrequirements
            WHERE course.courseno = trackrequirements.courseno
            GROUP BY trackrequirements.major, trackrequirements.trackcode)
ORDER BY student.sname



--Q8
SELECT student.sname, student.sID, prereq.courseno
FROM student, enroll, prereq
WHERE student.sID = enroll.sID AND enroll.courseno = prereq.precourseno AND student.major = 'CptS' AND student.trackcode = 'SYS' AND enroll.grade < 2 AND prereq.courseno IN 
	(SELECT enroll.courseno
           FROM enroll INNER JOIN prereq ON enroll.courseno = prereq.courseno
           GROUP BY enroll.courseno
           ORDER BY enroll.courseno) 

--Q9
SELECT courseno , FLOOR((CAST(COUNT(CASE WHEN grade >= 2 then 1 else NULL end) as FLOAT) /  CAST(COUNT(grade) as FLOAT) * 100)) AS passrate
FROM enroll 
WHERE courseno LIKE 'CptS%'
GROUP BY courseno
ORDER BY courseno

--Q10
--i
-- This returns classes with more than 2 pre-required classes.

--ii
SELECT  E.courseno, COUNT(E.precourseno) as pcount
FROM course NATURAL JOIN prereq AS E
GROUP BY E.courseno
HAVING COUNT(E.precourseno) >= 2
ORDER BY E.courseno 
