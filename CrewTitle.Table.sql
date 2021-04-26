CREATE TABLE [dbo].[CrewTitle]
(
    [CrewTitleID] INT NOT NULL PRIMARY KEY, 
    [CrewTitle] VARCHAR(50) NOT NULL, 
    [DateAdded] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UserName] VARCHAR(50) NOT NULL
)
