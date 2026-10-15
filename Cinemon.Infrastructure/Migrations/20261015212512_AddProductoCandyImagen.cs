using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductoCandyImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "ProductosCandy",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Backfill de la imagen en los productos ya cargados (no afecta bases nuevas).
            migrationBuilder.Sql(
                "UPDATE [ProductosCandy] SET [ImagenUrl] = CASE [Nombre] " +
                "WHEN N'Pochoclo Individual' THEN N'img/candy/Pochoclo_Individual.png' " +
                "WHEN N'Pochoclo Grande' THEN N'img/candy/Pochoclo_Clasico.png' " +
                "WHEN N'Balde de Pochoclo Familiar' THEN N'img/candy/Balde_Familiar.png' " +
                "WHEN N'Gaseosa Chica' THEN N'img/candy/Vaso_Mediano.png' " +
                "WHEN N'Gaseosa Mediana' THEN N'img/candy/Vaso_Mediano.png' " +
                "WHEN N'Gaseosa Grande' THEN N'img/candy/Vaso_Grande.png' " +
                "WHEN N'Nachos con Queso' THEN N'img/candy/NachosConQueso.png' " +
                "WHEN N'Agua Mineral' THEN N'img/candy/Agua.jpg' " +
                "WHEN N'Combo Clásico' THEN N'img/candy/Combo_Clasico.png' " +
                "WHEN N'Combo Pareja' THEN N'img/candy/Combo_Pareja.png' " +
                "WHEN N'Combo Familiar' THEN N'img/candy/Combo_Familiar.png' " +
                "ELSE [ImagenUrl] END " +
                "WHERE [ImagenUrl] IS NULL;");

            // Alta del combo nuevo solo si ya existen sus componentes
            // (en una base nueva el seed lo crea después de correr las migraciones).
            migrationBuilder.Sql(
                "IF EXISTS (SELECT 1 FROM [ProductosCandy] WHERE [Nombre] = N'Balde de Pochoclo Familiar') " +
                "AND EXISTS (SELECT 1 FROM [ProductosCandy] WHERE [Nombre] = N'Gaseosa Mediana') " +
                "AND NOT EXISTS (SELECT 1 FROM [ProductosCandy] WHERE [Nombre] = N'Combo Balde Pareja') " +
                "BEGIN " +
                "INSERT INTO [ProductosCandy] ([Nombre], [Descripcion], [Precio], [Categoria], [Activo], [FechaCreacion], [ImagenUrl]) " +
                "VALUES (N'Combo Balde Pareja', N'Balde de pochoclo familiar con dos gaseosas medianas.', 15900.00, 3, 1, SYSUTCDATETIME(), N'img/candy/Combo_Pareja2.png'); " +
                "DECLARE @comboId INT = CAST(SCOPE_IDENTITY() AS INT); " +
                "INSERT INTO [ProductoComboItems] ([ProductoId], [ComponenteProductoId], [Cantidad]) " +
                "SELECT @comboId, [Id], 1 FROM [ProductosCandy] WHERE [Nombre] = N'Balde de Pochoclo Familiar'; " +
                "INSERT INTO [ProductoComboItems] ([ProductoId], [ComponenteProductoId], [Cantidad]) " +
                "SELECT @comboId, [Id], 2 FROM [ProductosCandy] WHERE [Nombre] = N'Gaseosa Mediana'; " +
                "END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [ProductoComboItems] WHERE [ProductoId] IN " +
                "(SELECT [Id] FROM [ProductosCandy] WHERE [Nombre] = N'Combo Balde Pareja');");

            migrationBuilder.Sql(
                "DELETE FROM [ProductosCandy] WHERE [Nombre] = N'Combo Balde Pareja';");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "ProductosCandy");
        }
    }
}
