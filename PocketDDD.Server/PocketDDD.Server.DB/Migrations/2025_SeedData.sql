-- Before running this script, ensure that FullDBScript.sql has been run to create the tables

-- Remove all the data
delete [UserSessionFeedback]
delete [UserEventFeedback]
delete [Sessions]
delete TimeSlots
delete Tracks
delete EventDetail

GO

-- Add 2025 Sessionize ID
Insert into EventDetail
values (1, '8oswqcwt')

GO
