CREATE TABLE [dbo].[Detail] 
(
    [DetailID] INT NOT NULL PRIMARY KEY, 
    [MovieID] INT NOT NULL, 
    [CategoryID] INT NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [DateEntered] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UserName] VARCHAR(50) NOT NULL
)
