
Alter table dbo.ReleaseChanges
Alter Column VersionNbr VarChar(25)not null


Update dbo.ReleaseChanges set VersionNbr = '1.0.0.'+ VersionNbr where VersionNbr <> '1.0.1.0'

Insert into dbo.ReleaseChanges
Values ('1.0.1.0',GETDATE(),'-- The current printing options for Lab Users is "all or nothing".  This release will allow you print lab orders based on priority.  If a priority is selected, the orders for only that priority will print in the order from oldest date to newest date.
')