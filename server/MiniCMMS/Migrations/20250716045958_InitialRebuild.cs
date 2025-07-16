using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniCMMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialRebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "TasksAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "MaintenanceTasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "MaintenanceTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TasksAssignments_TechnicianId",
                table: "TasksAssignments",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_CreatedById",
                table: "MaintenanceTasks",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceTasks_Users_CreatedById",
                table: "MaintenanceTasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTasks_Users_CreatedById",
                table: "MaintenanceTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TasksAssignments_Users_TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TasksAssignments_TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceTasks_CreatedById",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "TasksAssignments");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "MaintenanceTasks");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TaskHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaintenanceTaskId = table.Column<int>(type: "integer", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskHistories_MaintenanceTasks_MaintenanceTaskId",
                        column: x => x.MaintenanceTaskId,
                        principalTable: "MaintenanceTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_MaintenanceTaskId",
                table: "TaskHistories",
                column: "MaintenanceTaskId");
        }
    }
}
