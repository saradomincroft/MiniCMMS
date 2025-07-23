using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCMMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTasksAssignmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TasksAssignments_Users_UserId",
                table: "TasksAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TasksAssignments",
                table: "TasksAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TasksAssignments_TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TasksAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "TasksAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId1",
                table: "TasksAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TasksAssignments",
                table: "TasksAssignments",
                columns: new[] { "TechnicianId", "MaintenanceTaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_TasksAssignments_TechnicianId1",
                table: "TasksAssignments",
                column: "TechnicianId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId1",
                table: "TasksAssignments",
                column: "TechnicianId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId1",
                table: "TasksAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TasksAssignments",
                table: "TasksAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TasksAssignments_TechnicianId1",
                table: "TasksAssignments");

            migrationBuilder.DropColumn(
                name: "TechnicianId1",
                table: "TasksAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "TasksAssignments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TasksAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TasksAssignments",
                table: "TasksAssignments",
                columns: new[] { "UserId", "MaintenanceTaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_TasksAssignments_TechnicianId",
                table: "TasksAssignments",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksAssignments_Users_UserId",
                table: "TasksAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
