Create Database Library;
Go

Use Library;
Go

Create Table dbo.Book
(
	BookId		int				Not Null	Identity(1,1),
	Title		nvarchar(200)	Not Null,
	PageCount	int				Not Null,
	IsActive	bit				Not Null,
	RowId		RowVersion		Not Null	
);
Go

Create Table dbo.Author
(
	AuthorId	int				Not Null	Identity(1,1),
	FirstName	nvarchar(50)	Not Null,
	MiddleName	nvarchar(50)	Null,
	LastName	nvarchar(50)	Null,
	IsActive	bit				Not Null,
	RowId		RowVersion		Not Null
);
Go

Create Table dbo.BookAuthor
(
	BookAuthorId	int				Not Null	Identity(1,1),
	BookId			int				Not Null,
	AuthorId		int				Not Null,
	RowId			RowVersion	
);
Go

Alter Table dbo.Book
Add Constraint PK_Book_BookId Primary Key (BookId);
Go

Alter Table dbo.Author
Add Constraint PK_Author_AuthorId Primary Key (AuthorId);
Go

Alter Table dbo.BookAuthor
Add Constraint PK_BookAuthor_BookAuthorId Primary Key (BookAuthorId);
Go

Alter Table dbo.BookAuthor
Add Constraint FK_Book_BookAuthor_BookId Foreign Key (BookId) References dbo.Book (BookId);
Go

Alter Table dbo.BookAuthor
Add Constraint FK_Author_BookAuthor_AuthorId Foreign Key (AuthorId) References dbo.Author (AuthorId);
Go

Insert Into dbo.Book (Title,PageCount,IsActive) Values ('Alice In Wonderland', 200, 1);
Insert Into dbo.Book (Title,PageCount,IsActive) Values ('James Bond 007', 200, 1);
Insert Into dbo.Book (Title,PageCount,IsActive) Values ('Let''s Talk Money', 200, 1);

Insert Into dbo.Author (FirstName, MiddleName, LastName, IsActive) Values ('Lewis',Null,'Carrol',1)
Insert Into dbo.Author (FirstName, MiddleName, LastName, IsActive) Values ('Ian',Null,'Fleming',1)
Insert Into dbo.Author (FirstName, MiddleName, LastName, IsActive) Values ('Monika',Null,'Halan',1)

Insert Into dbo.BookAuthor (BookId, AuthorId) Values (1,1)
Insert Into dbo.BookAuthor (BookId, AuthorId) Values (2,2)
Insert Into dbo.BookAuthor (BookId, AuthorId) Values (3,3)

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Parakh Singhal
-- Create date: 4 June 2021
-- Description:	The stored procedure returns a list of all the books.
-- =============================================
CREATE PROCEDURE dbo.GetBooks 
	-- Add the parameters for the stored procedure here	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select BookId, Title, PageCount, IsActive, RowId
	From dbo.Book
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Parakh Singhal
-- Create date: 4 June 2021
-- Description:	The stored procedure returns a list of all the authors.
-- =============================================
CREATE PROCEDURE dbo.GetAuthors 
	-- Add the parameters for the stored procedure here	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select AuthorId, FirstName, LastName, IsActive, RowId
	From dbo.Author
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Parakh Singhal
-- Create date: 4 June 2021
-- Description:	The stored procedure returns a list of all the books written by a author.
-- =============================================
CREATE PROCEDURE dbo.GetBooksByAuthor
	-- Add the parameters for the stored procedure here	
	@FirstName nvarchar(50),
	@LastName nvarchar(50)
AS
BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select Book.BookId, Book.Title, Book.PageCount, Book.IsActive, Book.RowId 
	From dbo.Book as Book
	Inner Join dbo.BookAuthor as BookAuthor on Book.BookId = BookAuthor.BookId
	Inner Join dbo.Author as Author on Author.AuthorId = BookAuthor.AuthorId
	Where Author.FirstName = @FirstName
	And Author.LastName = @LastName
END
GO