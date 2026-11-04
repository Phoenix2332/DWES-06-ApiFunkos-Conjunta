using ApiFunkosConjunta.Services;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace ApiFunkosConjunta.Tests.Services;

public abstract class CatalogoServiceTestBase {
    protected Mock<IDistributedCache> CacheMock = null!;
    protected CatalogoService CatalogoService = null!;
    protected Mock<ICategoriaService> CategoriaServiceMock = null!;
    protected Mock<IFunkoService> FunkoServiceMock = null!;
    protected Mock<IProveedorService> ProveedorServiceMock = null!;

    [SetUp]
    public void SetUp() {
        CategoriaServiceMock = new Mock<ICategoriaService>();
        ProveedorServiceMock = new Mock<IProveedorService>();
        FunkoServiceMock = new Mock<IFunkoService>();
        CacheMock = new Mock<IDistributedCache>();

        CacheMock
            .Setup(c => c.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        CatalogoService = new CatalogoService(
            CategoriaServiceMock.Object,
            ProveedorServiceMock.Object,
            FunkoServiceMock.Object,
            CacheMock.Object);
    }
}