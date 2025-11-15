-- إضافة سجل جديد في جدول AspNetUserRoles لربط المستخدم بدور المدير
INSERT INTO [AspNetUserRoles] (UserId, RoleId)
VALUES (
    'bd483751-2fce-466b-935e-f692d1a166f8', -- Id المستخدم
    '924dc9aa-bf6f-4c12-8292-9b2f9e37692b'  -- Id دور المدير (Admin)
);