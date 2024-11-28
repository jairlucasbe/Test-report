using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using seguimiento_expotec.Models;
using seguimiento_expotec.Services;

namespace seguimiento_expotec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService) => _companyService = companyService;

        [HttpGet]
        public async Task<ActionResult<List<Company>>> GetCompanies() => Ok(await _companyService.GetAllCompanies());

        [HttpGet("{filter}")]
        public async Task<ActionResult<Company>> GetCompanyByIdorRuc(string filter) => Ok(await _companyService.GetCompanyByIdOrRuc(filter));

        [HttpPost] public async Task<ActionResult> CreateCompany([FromBody] Company company) => Ok(await _companyService.CreateCompany(company));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(string id, [FromBody] Company updatedCompany) => Ok(await _companyService.UpdateCompany(id, updatedCompany));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCompany(string id) => Ok(await _companyService.DeleteCompany(id));
    }
}
