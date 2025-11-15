-- 1. إنشاء جدول ClassEnrollments المفقود (بنيته الصحيحة):
CREATE TABLE [ClassEnrollments] (
    [GroupClassId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [EnrollmentDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ClassEnrollments] PRIMARY KEY ([GroupClassId], [UserId]),
    CONSTRAINT [FK_ClassEnrollments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassEnrollments_GroupClasses_GroupClassId] FOREIGN KEY ([GroupClassId]) REFERENCES [GroupClasses] ([GroupClassId]) ON DELETE NO ACTION
);

-- 2. إنشاء الفهرس اللازم:
CREATE INDEX [IX_ClassEnrollments_UserId] ON [ClassEnrollments] ([UserId]);

-- 3. تسجيل الهجرة يدويًا لإسكات Entity Framework Core:
-- (MigrationId يجب أن يكون: 20251213163950_CreateClassEnrollmentTable)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251213163950_CreateClassEnrollmentTable', N'8.0.22');