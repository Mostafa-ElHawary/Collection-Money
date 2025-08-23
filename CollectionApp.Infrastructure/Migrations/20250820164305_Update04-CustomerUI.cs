using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update04CustomerUI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceOfFunds",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SourceOfFunds",
                table: "Contracts");
        }
    }
}
