using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Linq.Expressions;

namespace SistemaVenta.Test.BLL
{
    public class FirebaseServiceTest
    {
        [Fact]
        public async Task SubirStorage()
        {
            // Arrange
            var configuracionMock = new Mock<IGenericRepository<Configuracion>>();
            var firebaseService = new FireBaseService(configuracionMock.Object);

            var streamArchivo = new MemoryStream();
            var carpetaDestino = "CarpetaDestino";
            var nombreArchivo = "NombreArchivo";

            configuracionMock.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Configuracion, bool>>>()))
            .ReturnsAsync(new List<Configuracion>
            {
                new Configuracion
                {
                    Recurso = "Firebase_Storage",
                    Propiedad = "api_key",
                    Valor = "45345345fegef2",
                }
            }.AsQueryable());

            // Act
            var urlImagen = await firebaseService.SubirStorage(streamArchivo, carpetaDestino, nombreArchivo);

            // Assert
            Assert.NotNull(urlImagen);
        }

        [Fact]
        public async Task EliminarStorageTest()
        {
            // Arrange
            var configuracionMock = new Mock<IGenericRepository<Configuracion>>();
            var firebaseService = new FireBaseService(configuracionMock.Object);

            var carpetaDestino = "CarpetaDestino";
            var nombreArchivo = "NombreArchivo";

            // Configura el repositorio para devolver una configuración de prueba
            configuracionMock.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Configuracion, bool>>>()))
            .ReturnsAsync(new List<Configuracion>
            {
                new Configuracion
                {
                    Recurso = "Firebase_Storage",
                    Propiedad = "api_key",
                    Valor = "23423423143",
                }
            }.AsQueryable());

            // Act
            var resultado = await firebaseService.EliminarStorage(carpetaDestino, nombreArchivo);

            // Assert
            Assert.True(resultado);
        }
    }
}
