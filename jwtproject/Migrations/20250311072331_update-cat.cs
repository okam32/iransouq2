using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jwtproject.Migrations
{
    /// <inheritdoc />
    public partial class updatecat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicAddress",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicAddress",
                table: "Category");
        }
    }
}
