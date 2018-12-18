using System;
using System.Net;
using System.Threading.Tasks;
using Tenants.API.Application.Dto;
using Tenants.API.Infrastructure.Exceptions;
using Tenants.API.Domain;
using Tenants.API.Application.Services.Company;
using Tenants.API.Application.Services.Product;
using Tenants.API.Application.Services.Tenant.Validator;
using Tenants.API.Application.Dto.Tenant;
using Mapster;
using System.Collections.Generic;
using Tenants.API.Application.Dto.TenantApplication;
using Platform360.Common;
using Platform360.ExceptionHandling;
namespace Tenants.API.Application.Services.Tenant
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICompanyService _companyService;
        private readonly IProductService _productService;
        public TenantService(ITenantRepository tenantRepository, IProductService productService, ICompanyService companyService)
        {
            _tenantRepository = tenantRepository;
            _companyService = companyService;
            _productService = productService;
        }

        public async Task<TenantDto> GetByIdAsync(Guid id)
        {

            if (id.Equals(Guid.Empty))
            {
                throw new TenantException(TenantErrorCodes.TenantIdCannotBeNull);
            }
            var record = await _tenantRepository.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException();
            }
            var result = record.Adapt<TenantDto>();
            return result;

        }

        public async Task<Paged<TenantDto>> GetAllAsync(TenantFilterDto tenantFilterDto)
        {
            if (tenantFilterDto.PageSize < 1 || tenantFilterDto.PageSize > 250)
            {
                throw new TenantException(TenantErrorCodes.PageSizeIsNotInRange, $"PageSize {tenantFilterDto.PageSize} is not in valid range");
            }

            if (tenantFilterDto.PageIndex < 1)
            {
                throw new TenantException(TenantErrorCodes.PageIndexIsNotInRange, $"PageIndex {tenantFilterDto.PageIndex} is smaller than 1");
            }

            var records = await _tenantRepository.GetAllAsync(tenantFilterDto);
            if (records.List.Count == 0)
            {
                throw new KeyNotFoundException();
            }
            var result = records.Adapt<Paged<TenantDto>>();
            return result;
        }
        public async Task<bool> IsTenantNameExists(string tenantName)
        {
            var record = await _tenantRepository.GetByNameAsync(tenantName);
            return record != null;
        }

        public async Task<bool> IsTenantIdExists(Guid id)
        {
            var record = await _tenantRepository.GetByIdAsync(id);
            return record != null;
        }

        public async Task<TenantDto> AddAsync(TenantCreateDto model)
        {
            #region Validation
            if (!model.Validate(out IList<ValidationError> errors))
            {
                throw new TenantException(TenantErrorCodes.TenantCreateDtoIsNotValid, errors, model);
            }
            #endregion
            if (await IsTenantNameExists(model.Name))
            {
                throw new TenantException(TenantErrorCodes.TenantNameAlreadyExist, $"Tenant {model.Name}");
            }
            var entity = model.Adapt<Domain.Tenant>();
            await _tenantRepository.AddAsync(entity);
            var result = entity.Adapt<TenantDto>();
            return result;
        }

        public async Task<TenantDto> UpdateAsync(TenantUpdateDto model)
        {
            #region Validation
            if (!model.Validate(out IList<ValidationError> errors))
            {
                throw new TenantException(TenantErrorCodes.TenantUpdateDtoIsNotValid, errors, model);
            }
            #endregion
            var record = await _tenantRepository.GetByIdAsync(model.Id);
            if (record == null)
            {
                throw new TenantException(TenantErrorCodes.TenantCouldNotBeFound, $"Tenant {model.Id}");
            }
            if (await IsTenantNameExists(model.Name))
            {
                throw new TenantException(TenantErrorCodes.TenantNameAlreadyExist, $"Tenant Name {model.Name}");
            }
            if (!(await _companyService.IsCompanyIdExists(model.CompanyId)))
            {
                throw new TenantException(TenantErrorCodes.CompanyCouldNotBeFound, $"Company {model.CompanyId}");
            }
            if (!(await _productService.IsProductIdExists(model.ProductId)))
            {
                throw new TenantException(TenantErrorCodes.ProductCouldNotBeFound, $"Product {model.ProductId}");
            }
            var entityDto = model.Adapt(record);
            await _tenantRepository.UpdateAsync(entityDto);
            var result = record.Adapt<TenantDto>();
            return result;
        }

        public async Task DeleteAsync(TenantDeleteDto model)
        {
            #region Validation
            if (!model.Validate(out IList<ValidationError> errors))
            {
                throw new TenantException(TenantErrorCodes.TenantDeleteDtoIsNotValid, errors, model);
            }
            #endregion
            var record = await _tenantRepository.GetByIdAsync(model.Id);
            if (record == null)
            {
                throw new TenantException(TenantErrorCodes.TenantCouldNotBeFound, $"Tenant {model.Id}");
            }
            if (record.TenantApplication == null)
            {
                throw new TenantException(TenantErrorCodes.RelationTenantApplicationShouldBeDeleted, $"Tenant {model.Id}");
            }
            await _tenantRepository.DeleteAsync(record);
        }

       
    }
}
