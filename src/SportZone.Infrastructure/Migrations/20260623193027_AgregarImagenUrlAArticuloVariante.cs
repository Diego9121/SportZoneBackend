using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenUrlAArticuloVariante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "ArticuloVariantes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "ArticuloVariantes");
        }
    }
}
