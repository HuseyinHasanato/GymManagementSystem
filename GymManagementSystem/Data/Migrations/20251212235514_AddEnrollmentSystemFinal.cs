using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.Data.Migrations
{
    public partial class AddEnrollmentSystemFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. حذف المفاتيح القديمة والعلاقات قبل تغيير الأعمدة
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_AspNetUsers_UserId",
                table: "ClassEnrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_ClassEnrollments_GroupClassId",
                table: "ClassEnrollments");

            // 2. حذف عمود Id القديم (سبب مشكلة IDENTITY)
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ClassEnrollments");

            // **ملاحظة:** تم حذف محاولة حذف عمود "UserId1" من هنا لتجنب الخطأ

            // 3. تعديلات على GroupClasses
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupClasses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GroupClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // 4. التأكد من أن UserId أصبح NOT NULL (مطلوب للمفتاح المركب)
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClassEnrollments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // 5. إضافة المفتاح الأساسي المركب الجديد
            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments",
                columns: new[] { "GroupClassId", "UserId" });

            // 6. إضافة المفتاح الأجنبي لـ UserId مرة أخرى 
            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_AspNetUsers_UserId",
                table: "ClassEnrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // **ملاحظة:** يتم استبدال محتوى Down بالكود الأصلي الذي أرسلته سابقاً، مع حذف أي إشارة لـ UserId1 أو تغييرات غير ضرورية.

            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_AspNetUsers_UserId",
                table: "ClassEnrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments");

            // إعادة إنشاء العمود Id بالترقيم التلقائي
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ClassEnrollments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClassEnrollments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // ... (بقية عمليات Down التي لم تسبب مشاكل) ...

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupClasses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GroupClasses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrollments_GroupClassId",
                table: "ClassEnrollments",
                column: "GroupClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_AspNetUsers_UserId",
                table: "ClassEnrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}