using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_GroupClasses_GroupClassId",
                table: "ClassEnrollments");

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HeightCm = table.Column<int>(type: "int", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    FitnessGoal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_MemberId",
                table: "UserProfiles",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_GroupClasses_GroupClassId",
                table: "ClassEnrollments",
                column: "GroupClassId",
                principalTable: "GroupClasses",
                principalColumn: "GroupClassId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_GroupClasses_GroupClassId",
                table: "ClassEnrollments");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_GroupClasses_GroupClassId",
                table: "ClassEnrollments",
                column: "GroupClassId",
                principalTable: "GroupClasses",
                principalColumn: "GroupClassId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
