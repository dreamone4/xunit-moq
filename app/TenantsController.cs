using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tenants.API.Application.Services.Tenant;
using Tenants.API.Application.Dto.Tenant;
using System.Collections.Generic;
using Platform360.Common;

namespace Tenants.API.Controllers
{
    [Route("api/v1/[controller]")]
    
    public class TenantsController : Platform360Controller
    {
        private readonly ITenantService _tenantService;
        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Get Platform360.Tenant By Id
        /// </summary>
        /// <param name="id">long tenantId</param>
        /// <returns>Platform360.Tenant</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TenantDto), (int)HttpStatusCode.OK)]
       
        public async Task<IActionResult> GetTenantById(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return BadRequest();
            }

            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return Ok(tenant);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<TenantDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllTenants(TenantFilterDto tenantFilterDto)
        {
            var records = await _tenantService.GetAllAsync(tenantFilterDto);
            return PagedOk(records, tenantFilterDto, records.TotalPageCount);
        }

        /// <summary>
        /// Creates New Platform360.Tenant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateTenant([FromBody]TenantCreateDto model)
        {
            var result = await _tenantService.AddAsync(model);
            return CreatedAtAction(nameof(GetTenantById), new { id = result.Id }, null);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateTenant([FromBody] TenantUpdateDto model)
        {
            var result = await _tenantService.UpdateAsync(model);
            return CreatedAtAction(nameof(GetTenantById), new { id = result.Id }, null);
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteApplication([FromBody]TenantDeleteDto model)
        {
            await _tenantService.DeleteAsync(model);
            return NoContent();
        }
    }
}