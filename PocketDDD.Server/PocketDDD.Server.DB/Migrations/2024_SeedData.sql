delete [UserSessionFeedback]
delete [UserEventFeedback]
delete [Sessions]
delete TimeSlots
delete Tracks
delete EventDetail


GO

DBCC CHECKIDENT ('[EventDetail]', RESEED, 1);
DBCC CHECKIDENT ('[Tracks]', RESEED, 0);
DBCC CHECKIDENT ('[TimeSlots]', RESEED, 0);
DBCC CHECKIDENT ('[Sessions]', RESEED, 0);

GO

Insert into EventDetail values (1, 'kn91wz1x')

Insert into Tracks values (1, 'Track 1','The Junction 🚉', 0)
Insert into Tracks values (1, 'Track 2','Brunel''s Boardroom 🎩🛹', 1)
Insert into Tracks values (1, 'Track 3','Brunel''s Breakout room 🎩☕', 2)
Insert into Tracks values (1, 'Track 4','Clock tower room ⏲', 3)

GO