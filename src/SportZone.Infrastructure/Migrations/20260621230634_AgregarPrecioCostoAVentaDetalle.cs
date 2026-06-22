using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPrecioCostoAVentaDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCosto",
                table: "VentaDetalles",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioCosto",
                table: "VentaDetalles");
        }
    }
}
