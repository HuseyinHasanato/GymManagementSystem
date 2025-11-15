-- *************** التنظيف الشامل المُحسَّن ***************

-- 1. حذف سجلات الحجوزات (لحل خطأ المفتاح الخارجي مع AspNetUsers)
DELETE FROM [ClassEnrollments]; 

-- 2. حذف جميع بيانات الهوية (Identity)
DELETE FROM [AspNetUserRoles];
DELETE FROM [AspNetUsers];
DELETE FROM [AspNetRoles];


-- *************** الإعدادات الجديدة ***************
-- بيانات الأدوار
DECLARE @AdminRoleName NVARCHAR(256) = N'Admin';
DECLARE @MemberRoleName NVARCHAR(256) = N'Member';

-- بيانات المستخدمين
DECLARE @AdminEmail NVARCHAR(256) = N'admin@gym.com'; 
DECLARE @MemberEmail NVARCHAR(256) = N'member@gym.com'; 

-- كلمة السر المضمونة (Password123!)
DECLARE @PasswordHash NVARCHAR(MAX) = N'AQAAAAEAACcQAAAAEPv1h1b9f7oWqV2l3Z1Q3t8pQ5c6Q1eCgG0uY4G7M8t1C0y4v5Y7x0N0iT3t0z7r4O0u5Y6w7gH3b9F9e6E5s4V2';


-- 3. إعادة إنشاء الأدوار (لضمان وجودها)
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @MemberRoleId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
VALUES
    (@AdminRoleId, @AdminRoleName, UPPER(@AdminRoleName)),
    (@MemberRoleId, @MemberRoleName, UPPER(@MemberRoleName));


-- 4. إعادة إنشاء حساب المدير (مع القيم الضرورية التي لا تسمح بـ NULL)
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [AspNetUsers] (
    Id, Email, NormalizedEmail, UserName, NormalizedUserName, PasswordHash, EmailConfirmed, SecurityStamp,
    AccessFailedCount, LockoutEnabled, TwoFactorEnabled, PhoneNumberConfirmed
)
VALUES (
    @AdminId,
    @AdminEmail,
    UPPER(@AdminEmail),
    @AdminEmail,
    UPPER(@AdminEmail),
    @PasswordHash,
    1,
    NEWID(),
    0, -- AccessFailedCount (قيمة غير فارغة)
    0, -- LockoutEnabled (قيمة غير فارغة)
    0, -- TwoFactorEnabled (قيمة غير فارغة)
    0  -- PhoneNumberConfirmed (قيمة غير فارغة)
);

-- 5. إعادة إنشاء حساب العضو
DECLARE @MemberId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [AspNetUsers] (
    Id, Email, NormalizedEmail, UserName, NormalizedUserName, PasswordHash, EmailConfirmed, SecurityStamp,
    AccessFailedCount, LockoutEnabled, TwoFactorEnabled, PhoneNumberConfirmed
)
VALUES (
    @MemberId,
    @MemberEmail,
    UPPER(@MemberEmail),
    @MemberEmail,
    UPPER(@MemberEmail),
    @PasswordHash,
    1,
    NEWID(),
    0,
    0,
    0,
    0
);


-- 6. تعيين الأدوار للمستخدمين
INSERT INTO [AspNetUserRoles] (UserId, RoleId) VALUES (@AdminId, @AdminRoleId);
INSERT INTO [AspNetUserRoles] (UserId, RoleId) VALUES (@MemberId, @MemberRoleId);