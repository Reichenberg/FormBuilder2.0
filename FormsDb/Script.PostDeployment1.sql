/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO Account
	SELECT 1, 'TestSuperAdmin', 'SuperAdmin'
	WHERE NOT EXISTS (SELECT 1 FROM Account WHERE Id = 1)
;
INSERT INTO Account
	SELECT 2, 'TestAdmin', 'Admin'
	WHERE NOT EXISTS (SELECT 1 FROM Account WHERE Id = 2)
;
INSERT INTO Account
	SELECT 3, 'TestUser', 'User'
	WHERE NOT EXISTS (SELECT 1 FROM Account WHERE Id = 3)
;
INSERT INTO WorkFlow
	SELECT 1, 'Standard'
	WHERE NOT EXISTS (SELECT 1 FROM WorkFlow WHERE Id = 1)
;
INSERT INTO AccountWorkFlow
	SELECT 1, 1, 2, 1
	WHERE NOT EXISTS (SELECT 1 FROM AccountWorkFlow WHERE Id = 1)
;
INSERT INTO AccountWorkFlow
	SELECT 2, 2, 1, 1
	WHERE NOT EXISTS (SELECT 1 FROM AccountWorkFlow WHERE Id = 2)
;
INSERT INTO WholeForm
	SELECT 1, 'TestForm', 2, 1
	WHERE NOT EXISTS (SELECT 1 FROM WholeForm WHERE Id = 1)
;
INSERT INTO FormSubmission
	SELECT 1, 'Pending', 1
	WHERE NOT EXISTS (SELECT 1 FROM FormSubmission WHERE Id = 1)
;
INSERT INTO SubmissionWhole
	SELECT 1, '2/13/2016 12:00:00 AM', 'Never', 3, 1
	WHERE NOT EXISTS (SELECT 1 FROM SubmissionWhole WHERE Id = 1)
;
INSERT INTO FormPage
	SELECT 1, 1, '<form method="post" action="/user/submit">
    <div class="form-horizontal">
        <div class="form-group">
            <label for="TestField" class="control-label col-md-2">Test Field:</label>
            <div class="col-md-10">
                <input type="text" id="TestField" name="TestField" class="form-control" value="" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="submit" value="Test Submit" class="btn btn-default" />
            </div>
        </div>
    </div>
</form>', 1
	WHERE NOT EXISTS (SELECT 1 FROM FormPage WHERE Id = 1)
;