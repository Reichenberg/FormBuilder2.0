CREATE TABLE [dbo].[FormSubmission]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ApprovalStatus] NVARCHAR(50) NULL, 
    [WholeFormId] INT NOT NULL, 
    CONSTRAINT [FK_FormSubmission_WholeForm] FOREIGN KEY (WholeFormId) REFERENCES WholeForm(Id) 
)
