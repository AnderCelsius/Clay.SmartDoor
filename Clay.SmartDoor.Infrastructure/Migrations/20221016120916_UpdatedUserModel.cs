using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clay.SmartDoor.Infrastructure.Migrations
{
    public partial class UpdatedUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StaffId",
                table: "AspNetUsers",
                type: "bigint",
                maxLength: 100,
                nullable: false,
                defaultValue: 0L);
        }
    }
}
