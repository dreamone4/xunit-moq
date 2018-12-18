using System;
using System.Collections.Generic;
using System.Text;
using Tenants.API.Application.Services.Tenant;
using Tenants.API.Application.Dto.Tenant;
using Tenants.API.Controllers;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tenants.API.UnitTests
{
    public class TenantsControllerTests
    {
        [Fact]
        public void TentantsController_GetTenantById_ShouldReturnTentant()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var mockTenantService = new Mock<ITenantService>();
            mockTenantService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                          .ReturnsAsync(new TenantDto());
            var tenantController = new TenantsController(mockTenantService.Object);            

            // Act
            var okResult = tenantController.GetTenantById(testId);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
            mockTenantService.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());

            // Reference #1: https://code-maze.com/unit-testing-aspnetcore-web-api/
            // Reference #2: http://dontcodetired.com/blog/post/Mocking-in-NET-Core-Tests-with-Moq
        }
    }
}
