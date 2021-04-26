CREATE TABLE [dbo].[Actor]
(
    [ActorID] INT NOT NULL PRIMARY KEY, 
    [MovieID] INT NOT NULL, 
    [Actor] VARCHAR(100) NULL, 
    [DateEntered] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UserName] VARCHAR(50) NOT NULL
)
