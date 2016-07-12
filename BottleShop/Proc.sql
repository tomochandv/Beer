/*
사용자 기본 정보 관련
*/
CREATE PROC SP_MEMBER_INFO_I
@USERID	VARCHAR(50),			-- ID
@PWD		VARCHAR(500),		-- PASSWORD
@NAME	VARCHAR(30),				-- NAME
@TELL	VARCHAR(30),			-- PHONE
@EMAIL	VARCHAR(150),			-- EMAIL
@ADDR	VARCHAR(150),			-- ADDRESS
@ISSMS	VARCHAR(1),				-- SMS SEND (Y :YES, N: NO)
@ISEMAIL	VARCHAR(1)			-- EMAIL SEND (Y :YES, N: NO)
AS
BEGIN
	INSERT INTO MEMBER_INFO(USERID	
							,PWD		
							,NAME	
							,TELL	
							,EMAIL	
							,ADDR	
							,ISSMS	
							,ISEMAIL	
							,INDT	
							)
	VALUES(@USERID	
							,@PWD		
							,@NAME	
							,@TELL	
							,@EMAIL	
							,@ADDR	
							,@ISSMS	
							,@ISEMAIL	
							,GETDATE()
							)
	SELECT @@IDENTITY
END
GO

CREATE PROC SP_MEMBER_INFO_U
@USERID	VARCHAR(50),			-- ID
@PWD		VARCHAR(500),		-- PASSWORD
@NAME	VARCHAR(30),				-- NAME
@TELL	VARCHAR(30),			-- PHONE
@EMAIL	VARCHAR(150),			-- EMAIL
@ADDR	VARCHAR(150),			-- ADDRESS
@ISSMS	VARCHAR(1),				-- SMS SEND (Y :YES, N: NO)
@ISEMAIL	VARCHAR(1)			-- EMAIL SEND (Y :YES, N: NO)
AS
BEGIN
	UPDATE MEMBER_INFO SET
		PWD = @PWD,
		NAME = @NAME,
		TELL = @NAME,
		EMAIL = @EMAIL,
		ADDR = @ADDR,
		ISSMS = @ISSMS,
		ISEMAIL = @EMAIL,
		UPDT = GETDATE()
	WHERE USERID = @USERID
END
GO

alter PROC SP_MEMBER_INFO_S
@USERID	VARCHAR(50)			-- ID
AS
BEGIN
	SELECT * FROM MEMBER_INFO WHERE USERID = @USERID
END
GO

CREATE PROC SP_MEMBER_INFO_S1
@USERID	VARCHAR(50)
AS
BEGIN
	SELECT COUNT(1) FROM MEMBER_INFO WHERE USERID = @USERID
END
GO

CREATE PROC SP_MEMBER_INFO_S2
@SINDEX INT,
@EINDEX INT,
@USERID	VARCHAR(50),
@NAME	VARCHAR(30)	
AS
BEGIN
SELECT * FROM
(
	SELECT 
		ROW_NUMBER() OVER(ORDER BY IDX DESC) ROWNUM,
		IDX, USERID, PWD, NAME, TELL, EMAIL, ADDR, ISSMS, ISEMAIL, INDT, UPDT
	FROM MEMBER_INFO
	WHERE USERID LIKE '%' + @USERID + '%' AND NAME LIKE '%' + @NAME + '%'
) PAGETABLE
WHERE PAGETABLE.ROWNUM BETWEEN @SINDEX AND @EINDEX
END
GO

/*
결제정보
*/
CREATE PROC SP_MEMBER_PAYINFO_I
	@USERID	VARCHAR(50),				-- ID
	@PTYPE	VARCHAR(1)	,				-- PAY TYPE (S: SUCESS, C: CANCLE, P: Promo code)
	@PRICE	FLOAT		,				-- PRICE
	@SDATE	DATETIME	,				-- START DATE
	@EDATE	DATETIME	,				-- END DATE
	@ISUSE	VARCHAR(1)					-- USE (Y,N)
AS
BEGIN
	INSERT INTO MEMBER_PAYINFO (USERID, PTYPE, PRICE, SDATE, EDATE, ISUSE, INDT)
	VALUES(@USERID, @PTYPE, @PRICE, @SDATE, @EDATE, @ISUSE, GETDATE())
END
GO

CREATE PROC SP_MEMBER_PAYINFO_U
@USERID	VARCHAR(50),				-- ID
@PTYPE	VARCHAR(1)					-- PAY TYPE (S: SUCESS, C: CANCLE, P: Promo code)
AS
BEGIN
	UPDATE MEMBER_PAYINFO SET PTYPE = @PTYPE WHERE USERID = @USERID
END
GO

CREATE PROC SP_MEMBER_PAYINFO_S
@USERID	VARCHAR(50)
AS
BEGIN
	SELECT * FROM MEMBER_PAYINFO WHERE USERID = @USERID
END
GO

CREATE PROC SP_MEMBER_PAYINFO_S1
@SINDEX INT,
@EINDEX INT,
@USERID	VARCHAR(50),
@UDATE	DATETIME
AS
BEGIN
SELECT * FROM
(
	SELECT 
		ROW_NUMBER() OVER(ORDER BY IDX DESC) ROWNUM,
		IDX, USERID, PTYPE, PRICE, SDATE, EDATE, ISUSE, INDT
	FROM MEMBER_PAYINFO
	WHERE USERID LIKE '%' + @USERID + '%'
		AND CONVERT(VARCHAR(8),@UDATE, 112) BETWEEN CONVERT(VARCHAR(8),SDATE, 112) AND CONVERT(VARCHAR(8),EDATE, 112)
) PAGETABLE
WHERE PAGETABLE.ROWNUM BETWEEN @SINDEX AND @EINDEX
END
GO

/*
login history
*/
CREATE PROC SP_MEMBER_LOGIN_I
	@IDX		INT,		-- PK
	@USERID	VARCHAR(50) ,				-- ID
	@IP		VARCHAR(20),						-- IP
	@AGENT	VARCHAR(MAX)						-- AGENT
AS
BEGIN
	INSERT INTO MEMBER_LOGIN (USERID, IP, AGENT, INDT)
	VALUES(@USERID, @IP, @AGENT, GETDATE())
END
GO

CREATE PROC SP_MEMBER_LOGIN_S
@SINDEX INT,
@EINDEX INT
AS
BEGIN
SELECT * FROM
(
	SELECT 
		ROW_NUMBER() OVER(ORDER BY IDX DESC) ROWNUM,
		IDX, USERID, IP, AGENT, INDT
	FROM MEMBER_LOGIN
) PAGETABLE
WHERE PAGETABLE.ROWNUM BETWEEN @SINDEX AND @EINDEX
END
GO

/*
CATEGORY는 일단 UI 없이
*/
DECLARE @TC_IDX INT;
INSERT INTO CATEGORY_INFO(NAME, INDT) VALUES('ALCHOL', GETDATE())
SET @TC_IDX = @@IDENTITY
INSERT INTO CATEGORY_INFO1(TC_IDX, NAME, INDT)
VALUES(@TC_IDX, 'WINE', GETDATE());
INSERT INTO CATEGORY_INFO1(TC_IDX, NAME, INDT)
VALUES(@TC_IDX, 'BEER', GETDATE());
INSERT INTO CATEGORY_INFO1(TC_IDX, NAME, INDT)
VALUES(@TC_IDX, 'Whisky', GETDATE());
INSERT INTO CATEGORY_INFO1(TC_IDX, NAME, INDT)
VALUES(@TC_IDX, 'Special Order', GETDATE());




