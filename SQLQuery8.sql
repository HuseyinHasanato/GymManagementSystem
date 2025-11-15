SELECT Id, Email, PasswordHash
FROM AspNetUsers
WHERE NormalizedEmail = N'admin.gym@test.com'; -- استخدم البريد الإلكتروني للمدير هنا