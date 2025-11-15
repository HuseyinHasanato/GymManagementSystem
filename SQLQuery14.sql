-- ID المستخدم الحالي (admin@gym.com):
DECLARE @AdminId UNIQUEIDENTIFIER = '11db8a00-58a5-4d44-b7cb-776183a6289c';

-- التشفير الموثوق لكلمة السر "Admin1"
DECLARE @NewHash NVARCHAR(MAX) = N'AQAAAAEAACcQAAAAEG8v4XhU04+wF0+H0/8Z/1hP2sN0xY6c9lP5jQ2lQ5gV4kM1qX1x4w7u2j9K0s8M';

UPDATE AspNetUsers
SET
    PasswordHash = @NewHash,
    EmailConfirmed = 1
WHERE
    Id = @AdminId;