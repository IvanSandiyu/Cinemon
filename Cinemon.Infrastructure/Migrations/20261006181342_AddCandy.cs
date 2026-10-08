using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidosCandy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosCandy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductosCandy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosCandy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PedidosCandyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    NombreProducto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosCandyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosCandyItems_PedidosCandy_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "PedidosCandy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoComboItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    ComponenteProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoComboItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoComboItems_ProductosCandy_ComponenteProductoId",
                        column: x => x.ComponenteProductoId,
                        principalTable: "ProductosCandy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoComboItems_ProductosCandy_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "ProductosCandy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosCandy_FechaPedido",
                table: "PedidosCandy",
                column: "FechaPedido");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosCandy_UsuarioId",
                table: "PedidosCandy",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosCandyItems_PedidoId",
                table: "PedidosCandyItems",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoComboItems_ComponenteProductoId",
                table: "ProductoComboItems",
                column: "ComponenteProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoComboItems_ProductoId_ComponenteProductoId",
                table: "ProductoComboItems",
                columns: new[] { "ProductoId", "ComponenteProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductosCandy_Activo",
                table: "ProductosCandy",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosCandy_Categoria",
                table: "ProductosCandy",
                column: "Categoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosCandyItems");

            migrationBuilder.DropTable(
                name: "ProductoComboItems");

            migrationBuilder.DropTable(
                name: "PedidosCandy");

            migrationBuilder.DropTable(
                name: "ProductosCandy");
        }
    }
}
