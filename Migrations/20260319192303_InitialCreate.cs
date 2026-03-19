using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoFacturacionHacienda.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroConsecutivo = table.Column<string>(type: "TEXT", nullable: false),
                    Clave = table.Column<string>(type: "TEXT", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmisorNombre = table.Column<string>(type: "TEXT", nullable: false),
                    EmisorCedula = table.Column<string>(type: "TEXT", nullable: false),
                    ReceptorNombre = table.Column<string>(type: "TEXT", nullable: false),
                    ReceptorCedula = table.Column<string>(type: "TEXT", nullable: false),
                    ReceptorCorreo = table.Column<string>(type: "TEXT", nullable: false),
                    TotalVenta = table.Column<decimal>(type: "TEXT", precision: 18, scale: 5, nullable: false),
                    TotalImpuesto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 5, nullable: false),
                    TotalComprobante = table.Column<decimal>(type: "TEXT", precision: 18, scale: 5, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    MensajeHacienda = table.Column<string>(type: "TEXT", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "TEXT", nullable: true),
                    XmlGenerado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineasDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FacturaId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroLinea = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", precision: 18, scale: 5, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", precision: 18, scale: 5, nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasDetalle_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineasDetalle_FacturaId",
                table: "LineasDetalle",
                column: "FacturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasDetalle");

            migrationBuilder.DropTable(
                name: "Facturas");
        }
    }
}
