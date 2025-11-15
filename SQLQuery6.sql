-- 1. التأكد من حذف الجدول لتجنب الأخطاء (قد يفشل هذا الأمر إذا كان الجدول غير موجود، وهذا طبيعي)
IF OBJECT_ID('ClassEnrollments', 'U') IS NOT NULL
    DROP TABLE [ClassEnrollments];

-- 2. إنشاء جدول ClassEnrollments المفقود بالحجم المتطابق (450):
CREATE TABLE [ClassEnrollments] (
    [GroupClassId] int NOT NULL,
    -- تم استخدام 450 لتتوافق مع حجم AspNetUsers.Id (وهو 450)
    [UserId] nvarchar(450) NOT NULL,
    [EnrollmentDate] datetime2 NOT NULL,

    -- المفتاح الأساسي المركب (سيكون 904 بايت، لكننا نضحي بتجاوز الحد من أجل الربط الخارجي)
    CONSTRAINT [PK_ClassEnrollments] PRIMARY KEY ([GroupClassId], [UserId]),

    -- الروابط الخارجية (ستنجح الآن بسبب تطابق الحجم 450)
    CONSTRAINT [FK_ClassEnrollments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassEnrollments_GroupClasses_GroupClassId] FOREIGN KEY ([GroupClassId]) REFERENCES [GroupClasses] ([GroupClassId]) ON DELETE NO ACTION
);

-- 3. إنشاء الفهرس اللازم:
CREATE INDEX [IX_ClassEnrollments_UserId] ON [ClassEnrollments] ([UserId]);