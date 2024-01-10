using Moq;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Linq.Expressions;

namespace SistemaVenta.BLL.Implementacion.Tests
{
    public class RolServiceTests
    {
        [Fact]
        public async Task ListaRoles()
        {
            // Arrange
            var rolesMock = new List<Rol>
            {
                new Rol { IdRol = 1, Descripcion = "Admin" },
                new Rol { IdRol = 2, Descripcion = "Usuario" },
                new Rol  { IdRol = 3, Descripcion = "Cliente"}
            };

            var repositorioMock = new Mock<IGenericRepository<Rol>>();
            repositorioMock.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Rol, bool>>>()))
               .ReturnsAsync((Expression<Func<Rol, bool>> filtro) => rolesMock.AsQueryable().Where(filtro ?? (_ => true)).AsQueryable());

            var rolService = new RolService(repositorioMock.Object);

            // Act
            var result = await rolService.Lista();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(rolesMock.Count, result.Count);
        }
    }
}