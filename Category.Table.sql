CREATE TABLE [dbo].[Category]
(
    [CategoryID] INT NOT NULL PRIMARY KEY, 
    [Category] VARCHAR(50) NOT NULL, 
    [DateEntered] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UserName] VARCHAR(50) NOT NULL
)
