using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using seguimiento_expotec.Modules.Company.Persistence.DTO;
using seguimiento_expotec.Modules.Company.Services;

namespace seguimiento_expotec.Modules.Company.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
            => _companyService = companyService;

        [HttpGet]
        public async Task<ActionResult<List<CompanyDTO>>> GetCompanies()
            => Ok(await _companyService.GetAllCompanies());

        [HttpGet("{filter}")]
        public async Task<ActionResult<CompanyDTO>> GetCompanyByIdorRuc(string filter)
            => Ok(await _companyService.GetCompanyByIdOrRuc(filter));

        [HttpPost]
        public async Task<ActionResult> CreateCompany([FromBody] CompanyDTO company)
            => Ok(await _companyService.CreateCompany(company));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(string id, [FromBody] CompanyDTO updatedCompany)
            => Ok(await _companyService.UpdateCompany(id, updatedCompany));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCompany(string id)
            => Ok(await _companyService.DeleteCompany(id));
    }
}
