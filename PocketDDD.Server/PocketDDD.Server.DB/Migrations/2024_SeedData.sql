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
DBCC CHECKIDENT ('[EventDetail]', RESEED, 1); -- There is hardcoding to EventDetail ID 1 so we reset to 1 not 0
DBCC CHECKIDENT ('[Tracks]', RESEED, 0);
DBCC CHECKIDENT ('[TimeSlots]', RESEED, 0);
DBCC CHECKIDENT ('[Sessions]', RESEED, 0);

GO

-- Add 2024 Sessionize ID
Insert into EventDetail values (1, 'kn91wz1x')

GO

-- Add breaks (these don't get imported from Sessionize)
Insert into TimeSlots values (1,'2024-04-27 08:30:00.0000000 +01:00','2024-04-27 09:00:00.0000000 +01:00', 'Registration & Breakfast | Sponsored by Elastic Mint')
Insert into TimeSlots values (1,'2024-04-27 09:00:00.0000000 +01:00','2024-04-27 09:30:00.0000000 +01:00', 'Welcome briefing') 
Insert into TimeSlots values (1,'2024-04-27 10:30:00.0000000 +01:00','2024-04-27 10:45:00.0000000 +01:00', 'Tea & Coffee') 
Insert into TimeSlots values (1,'2024-04-27 11:45:00.0000000 +01:00','2024-04-27 12:00:00.0000000 +01:00', 'Tea & Coffee') 
Insert into TimeSlots values (1,'2024-04-27 13:00:00.0000000 +01:00','2024-04-27 14:15:00.0000000 +01:00', 'Lunch | Sponsored by Just Eat Takeaway.com') 
Insert into TimeSlots values (1,'2024-04-27 15:15:00.0000000 +01:00','2024-04-27 15:45:00.0000000 +01:00', 'Afternoon Tea | Sponsored by dxw') 
Insert into TimeSlots values (1,'2024-04-27 16:45:00.0000000 +01:00','2024-04-27 17:15:00.0000000 +01:00', 'Closing & Prize draw')

GO
