CREATE TABLE [dbo].[WholeForm]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [AccountId] INT NOT NULL, 
    [WorkFlowId] INT NOT NULL, 
    CONSTRAINT [FK_WholeForm_Account] FOREIGN KEY (AccountId) REFERENCES Account(Id), 
    CONSTRAINT [FK_WholeForm_WorkFlow] FOREIGN KEY (WorkFlowId) REFERENCES WorkFlow(Id) 
)
