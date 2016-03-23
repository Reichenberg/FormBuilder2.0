CREATE TABLE [dbo].[SubmissionPart]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PageNumber] INT NOT NULL, 
    [HtmlCode] TEXT NULL, 
    [SubmissionWholeId] INT NOT NULL, 
    CONSTRAINT [FK_SubmissionPart_SubmissionWhole] FOREIGN KEY (SubmissionWholeId) REFERENCES SubmissionWhole(Id)
)
