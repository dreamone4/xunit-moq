using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using Tenants.API.Application.Services.Tenant;
using Tenants.API.Domain;
using Tenants.API.Application.Services.Product;
using Tenants.API.Application.Services.Company;
using Tenants.API.Application.Dto.Tenant;
using System.Threading.Tasks;
using Tenants.API.Infrastructure.Exceptions;

namespace Tenants.API.UnitTests
{
    public class TenantServiceTests
    {

        [Fact]
        public void TentantsService_GetByIdAsync_ShouldReturnTentant()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var mockTenantRepository = new Mock<ITenantRepository>();
            var mockProductService = new Mock<IProductService>();
            var mockCompanyService = new Mock<ICompanyService>();
            var tenantService = new TenantService(mockTenantRepository.Object, mockProductService.Object, mockCompanyService.Object);

            // Act
            var tenantResult = tenantService.GetByIdAsync(testId);

            // Assert
            Assert.IsType<Task<TenantDto>>(tenantResult);
            mockTenantRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
        }

        [Fact]
        public async Task TentantsService_GetByIdAsync_ShouldThrowTenantException()
        {
            // Arrange
            var testId = Guid.Empty;
            var mockTenantRepository = new Mock<ITenantRepository>();
            var mockProductService = new Mock<IProductService>();
            var mockCompanyService = new Mock<ICompanyService>();
            var tenantService = new TenantService(mockTenantRepository.Object, mockProductService.Object, mockCompanyService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<TenantException>(() => tenantService.GetByIdAsync(testId));

            // Reference #1: https://stackoverflow.com/questions/45017295/assert-an-exception-using-xunit
            // Reference #2: https://stackoverflow.com/questions/40828272/xunit-assert-throwsasync-does-not-work-properly
        }
    }
}
