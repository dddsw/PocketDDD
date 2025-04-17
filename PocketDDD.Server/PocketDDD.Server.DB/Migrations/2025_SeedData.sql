-- Before running this script, ensure that FullDBScript.sql has been run to create the tables

-- Remove all the data
delete [UserSessionFeedback]
delete [UserEventFeedback]
delete [Sessions]
delete TimeSlots
delete Tracks
delete EventDetail


GO

-- Reset the identity columns
DBCC CHECKIDENT ('[Tracks]', RESEED, 0);
DBCC CHECKIDENT ('[TimeSlots]', RESEED, 0);
DBCC CHECKIDENT ('[Sessions]', RESEED, 0);

-- There is hardcoding to EventDetail ID 1 so we reset to 1 not 0
DBCC CHECKIDENT ('[EventDetail]', RESEED, 1); -- Use if this is a brand new table that has never been used before
--DBCC CHECKIDENT ('[EventDetail]', RESEED, 0); -- Use if this is an empty table that used to have rows

GO

-- Add 2025 Sessionize ID
Insert into EventDetail
values (1, '8oswqcwt')

GO
