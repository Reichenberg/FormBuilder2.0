CREATE TABLE [dbo].[FormPage]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PageNumber] INT NOT NULL, 
    [HtmlCode] TEXT NULL, 
    [WholeFormId] INT NOT NULL, 
    CONSTRAINT [FK_FormPage_WholeForm] FOREIGN KEY (WholeFormId) REFERENCES WholeForm(Id)
)
