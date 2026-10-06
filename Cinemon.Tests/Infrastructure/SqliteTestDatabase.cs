using Cinemon.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Cinemon.Tests.Infrastructure
{
    /// <summary>
    /// Base SQLite real (archivo temporal) para los tests.
    ///
    /// Por qué SQLite y no el provider InMemory de EF: InMemory no
    /// respeta índices únicos ni bloqueos entre escrituras, y justamente
    /// esas garantías son las que hay que verificar con los tests de
    /// concurrencia de reservas. Al usar un archivo, cada conexión
    /// compite por el mismo lock como en producción.
    /// </summary>
    public sealed class SqliteTestDatabase : IDisposable
    {
        private readonly string _rutaArchivo;
        private bool _disposed;

        public SqliteTestDatabase()
        {
            _rutaArchivo = Path.Combine(
                Path.GetTempPath(),
                $"cinemon-tests-{Guid.NewGuid():N}.db");

            using var contexto = CreateContext();

            contexto.Database.EnsureCreated();
        }

        private string CrearConnectionString() =>
            $"DataSource={_rutaArchivo};Default Timeout=30";

        public CinemonDbContext CreateContext()
        {
            return new CinemonDbContext(
                new DbContextOptionsBuilder<CinemonDbContext>()
                    .UseSqlite(CrearConnectionString())
                    .Options);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            SqliteConnection.ClearAllPools();

            try
            {
                if (File.Exists(_rutaArchivo))
                    File.Delete(_rutaArchivo);
            } catch (IOException)
            {
                // Si el archivo quedó tomado, el SO lo limpia después.
            }
        }
    }
}
