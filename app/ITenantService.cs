using Platform360.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenants.API.Application.Dto;
using Tenants.API.Application.Dto.Tenant;
using Tenants.API.Application.Dto.TenantApplication;

namespace Tenants.API.Application.Services.Tenant
{
    public interface ITenantService 
    {
        Task<TenantDto> GetByIdAsync(Guid id);
        Task<Paged<TenantDto>> GetAllAsync(TenantFilterDto tenantFilterDto);
        Task<bool> IsTenantNameExists(string tenantName);
        Task<bool> IsTenantIdExists(Guid id);
        Task<TenantDto> AddAsync(TenantCreateDto model);
        Task<TenantDto> UpdateAsync(TenantUpdateDto model);
        Task DeleteAsync(TenantDeleteDto model);
        
    }
}
