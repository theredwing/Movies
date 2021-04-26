CREATE TABLE [dbo].[Title]
(
    [MovieId] INT NOT NULL PRIMARY KEY, 
    [Title] VARCHAR(255) NOT NULL, 
    [DateEntered] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UserName] VARCHAR(50) NOT NULL
)
