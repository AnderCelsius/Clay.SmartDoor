using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clay.SmartDoor.Infrastructure.Migrations
{
    public partial class AddedAccessGroupandDoorAssignmententities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_AspNetUsers_ActionBy",
                table: "ActivityLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Door",
                table: "Door");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog");

            migrationBuilder.RenameTable(
                name: "Door",
                newName: "Doors");

            migrationBuilder.RenameTable(
                name: "ActivityLog",
                newName: "ActivityLogs");

            migrationBuilder.RenameIndex(
                name: "IX_Door_Name_Tag",
                table: "Doors",
                newName: "IX_Doors_Name_Tag");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_ActionBy",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_ActionBy");

            migrationBuilder.AddColumn<string>(
                name: "AccessGroupId",
                table: "AspNetUsers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doors",
                table: "Doors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AccessGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatorBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date_Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Last_Modified_Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessGroups", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DoorAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DoorId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatorBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date_Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Last_Modified_Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoorAssignment_AccessGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "AccessGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AccessGroupId",
                table: "AspNetUsers",
                column: "AccessGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorAssignment_DoorId",
                table: "DoorAssignment",
                column: "DoorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorAssignment_GroupId",
                table: "DoorAssignment",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_ActionBy",
                table: "ActivityLogs",
                column: "ActionBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AccessGroups_AccessGroupId",
                table: "AspNetUsers",
                column: "AccessGroupId",
                principalTable: "AccessGroups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_ActionBy",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AccessGroups_AccessGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DoorAssignment");

            migrationBuilder.DropTable(
                name: "AccessGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AccessGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doors",
                table: "Doors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "AccessGroupId",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Doors",
                newName: "Door");

            migrationBuilder.RenameTable(
                name: "ActivityLogs",
                newName: "ActivityLog");

            migrationBuilder.RenameIndex(
                name: "IX_Doors_Name_Tag",
                table: "Door",
                newName: "IX_Door_Name_Tag");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_ActionBy",
                table: "ActivityLog",
                newName: "IX_ActivityLog_ActionBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Door",
                table: "Door",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_AspNetUsers_ActionBy",
                table: "ActivityLog",
                column: "ActionBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
