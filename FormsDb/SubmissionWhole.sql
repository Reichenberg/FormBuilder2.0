CREATE TABLE [dbo].[SubmissionWhole]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DateModified] DATETIME NULL, 
    [Finished] NVARCHAR(50) NULL, 
    [AccountId] INT NOT NULL, 
    [FormSubmissionId] INT NOT NULL, 
    CONSTRAINT [FK_SubmissionWhole_AccountId] FOREIGN KEY (AccountId) REFERENCES Account(Id), 
    CONSTRAINT [FK_SubmissionWhole_FormSubmission] FOREIGN KEY (FormSubmissionId) REFERENCES FormSubmission(Id)
)
