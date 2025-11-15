-- *************** تنظيف شامل لجداول الهوية ***************
-- حذف جميع الأدوار والروابط أولاً (بسبب قيود المفاتيح الخارجية)
DELETE FROM [AspNetUserRoles];
DELETE FROM [AspNetUserClaims];
DELETE FROM [AspNetUserLogins];
DELETE FROM [AspNetUserTokens];
DELETE FROM [AspNetUsers];
DELETE FROM [AspNetRoles];

-- *************** الإعدادات الجديدة ***************
-- بيانات الأدوار
DECLARE @AdminRoleName NVARCHAR(256) = N'Admin';
DECLARE @MemberRoleName NVARCHAR(256) = N'Member';

-- بيانات المستخدمين
DECLARE @AdminEmail NVARCHAR(256) = N'admin@gym.com'; -- الايميل الأول (المدير)
DECLARE @MemberEmail NVARCHAR(256) = N'member@gym.com'; -- الايميل الثاني (العضو)

-- كلمة السر المضمونة (Password123!)
DECLARE @PasswordHash NVARCHAR(MAX) = N'AQAAAAEAACcQAAAAEPv1h1b9f7oWqV2l3Z1Q3t8pQ5c6Q1eCgG0uY4G7M8t1C0y4v5Y7x0N0iT3t0z7r4O0u5Y6w7gH3b9F9e6E5s4V2';


-- 1. إعادة إنشاء الأدوار
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @MemberRoleId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
VALUES
    (@AdminRoleId, @AdminRoleName, UPPER(@AdminRoleName)),
    (@MemberRoleId, @MemberRoleName, UPPER(@MemberRoleName));


-- 2. إعادة إنشاء حساب المدير
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [AspNetUsers] (Id, Email, NormalizedEmail, UserName, NormalizedUserName, PasswordHash, EmailConfirmed, SecurityStamp)
VALUES (
    @AdminId,
    @AdminEmail,
    UPPER(@AdminEmail),
    @AdminEmail,
    UPPER(@AdminEmail),
    @PasswordHash,
    1, -- تأكيد البريد الإلكتروني (مؤكد)
    NEWID()
);

-- 3. إعادة إنشاء حساب العضو
DECLARE @MemberId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [AspNetUsers] (Id, Email, NormalizedEmail, UserName, NormalizedUserName, PasswordHash, EmailConfirmed, SecurityStamp)
VALUES (
    @MemberId,
    @MemberEmail,
    UPPER(@MemberEmail),
    @MemberEmail,
    UPPER(@MemberEmail),
    @PasswordHash,
    1, -- تأكيد البريد الإلكتروني (مؤكد)
    NEWID()
);


-- 4. تعيين الأدوار للمستخدمين
-- ربط حساب المدير بدور Admin
INSERT INTO [AspNetUserRoles] (UserId, RoleId)
VALUES (@AdminId, @AdminRoleId);

-- ربط حساب العضو بدور Member
INSERT INTO [AspNetUserRoles] (UserId, RoleId)
VALUES (@MemberId, @MemberRoleId);