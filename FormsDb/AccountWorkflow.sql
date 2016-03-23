CREATE TABLE [dbo].[AccountWorkflow]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Order] INT NOT NULL, 
    [AccountId] INT NOT NULL, 
    [WorkFlowId] INT NOT NULL, 
    CONSTRAINT [FK_AccountWorkflow_Account] FOREIGN KEY (AccountId) REFERENCES Account(Id), 
    CONSTRAINT [FK_AccountWorkflow_WorkFlow] FOREIGN KEY (WorkFlowId) REFERENCES WorkFlow(Id)
)
