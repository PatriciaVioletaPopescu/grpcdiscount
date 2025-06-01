using Grpc.Core;
using Grpc.Core.Testing;
using GrpcDiscountService;
using GrpcDiscountService.Models;
using GrpcDiscountService.Repository;
using GrpcDiscountService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrpcDiscountTests
{
    public class DiscounterServiceTests
    {
        private Mock<IDiscountRepository> _repositoryMock;
        private Mock<ILogger<DiscounterService>> _loggerMock;
        private DiscounterService _service;
        private ServerCallContext _context;
        private const string ValidCode = "ABC1234";

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IDiscountRepository>();
            _loggerMock = new Mock<ILogger<DiscounterService>>();
            _service = new DiscounterService(_repositoryMock.Object, _loggerMock.Object);
            _context = CreateTestServerCallContext();
        }

        [TestCase(0u, 7u)]
        [TestCase(5u, 0u)]
        [TestCase(20001u, 5u)]
        public async Task Generate_InvalidRequest_ReturnsFalse(uint count, uint length)
        {
            var request = new GenerateRequest { Count = count, Length = length };
            var response = await _service.Generate(request, _context);

            Assert.That(response.Result, Is.False);
        }

        [Test]
        public async Task Generate_ValidRequest_SavesToRepository()
        {
            const uint count = 2u;
            var request = new GenerateRequest { Count = count, Length = 7 };

            var response = await _service.Generate(request, _context);

            _repositoryMock.Verify(repository => repository.AddRangeAsync(It.Is<List<Discount>>(list => list.Count == count)), Times.Once);
            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
            Assert.That(response.Result, Is.True);
        }

        [TestCase("")]
        [TestCase("ABC123")]
        [TestCase("ABC123456")]
        public async Task UseCode_InvalidCode_ReturnsInvalidInput(string code)
        {
            var request = new UseCodeRequest { Code = code };

            var response = await _service.UseCode(request, _context);

            Assert.That(response.Result, Is.EqualTo((uint)UseCodeResultEnum.InvalidInput));
        }

        [Test]
        public async Task UseCode_NotFound_ReturnsNotFound()
        {
            var request = new UseCodeRequest { Code = ValidCode };
            _repositoryMock.Setup(repository => repository.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync(null as Discount);

            var response = await _service.UseCode(request, _context);

            Assert.That(response.Result, Is.EqualTo((uint)UseCodeResultEnum.NotFound));
        }

        [Test]
        public async Task UseCode_AlreadyUsed_ReturnsAlreadyUsed()
        {
            var discount = new Discount { Code = ValidCode, IsUsed = true, UsedAt = DateTime.UtcNow };
            _repositoryMock.Setup(repository => repository.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync(discount);

            var request = new UseCodeRequest { Code = ValidCode };
            var response = await _service.UseCode(request, _context);

            Assert.That(response.Result, Is.EqualTo((uint)UseCodeResultEnum.AlreadyUsed));
        }

        [TestCase(true, UseCodeResultEnum.Success)]
        [TestCase(false, UseCodeResultEnum.AlreadyUsed)]
        public async Task UseCode_TryUpdate_ReturnsExpectedResult(bool tryUpdateResult, UseCodeResultEnum expectedResult)
        {
            var discount = new Discount { Code = ValidCode, IsUsed = false };
            _repositoryMock.Setup(repository => repository.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync(discount);
            _repositoryMock.Setup(repository => repository.TryUpdateAsync(It.IsAny<Discount>())).ReturnsAsync(tryUpdateResult);

            var request = new UseCodeRequest { Code = ValidCode };
            var response = await _service.UseCode(request, _context);

            _repositoryMock.Verify(repository => repository.TryUpdateAsync(It.IsAny<Discount>()), Times.Once);
            Assert.That(response.Result, Is.EqualTo((uint)expectedResult));
        }

        private static ServerCallContext CreateTestServerCallContext()
        {
            return TestServerCallContext.Create(
                method: "TestMethod",
                host: "localhost",
                deadline: DateTime.UtcNow.AddMinutes(1),
                requestHeaders: new Metadata(),
                cancellationToken: CancellationToken.None,
                peer: "127.0.0.1",
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: _ => Task.CompletedTask,
                writeOptionsGetter: () => null,
                writeOptionsSetter: _ => { }
            );
        }
    }
}
