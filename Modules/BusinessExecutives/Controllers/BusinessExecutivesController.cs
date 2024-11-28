using Microsoft.AspNetCore.Mvc;
using seguimiento_expotec.Modules.BusinessExecutives.Services;
using seguimiento_expotec.Modules.BusinessExecutives.Persistence.DTO;


namespace seguimiento_expotec.Modules.BusinessExecutives.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessExecutivesController : ControllerBase
    {
        private readonly BusinessExecutiveService _businessExecutiveService;

        public BusinessExecutivesController(BusinessExecutiveService businessExecutiveService)
            => _businessExecutiveService = businessExecutiveService;

        [HttpGet]
        public async Task<ActionResult<List<BusinessExecutivesDTO>>> GetBusinessExecutives()
            => Ok(await _businessExecutiveService.GetAllBusinessExecutives());

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessExecutivesDTO>> GetBusinessExecutiveByIdorName(string id)
            => Ok(await _businessExecutiveService.GetBusinessExecutiveById(id));

        [HttpPost]
        public async Task<ActionResult> CreateBusinessExecutive([FromBody] BusinessExecutivesDTO businessExecutive)
            => Ok(await _businessExecutiveService.CreateBusinessExecutive(businessExecutive));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBusinessExecutive(string id, [FromBody] BusinessExecutivesDTO updatedBusinessExecutive)
            => Ok(await _businessExecutiveService.UpdateBusinessExecutive(id, updatedBusinessExecutive));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBusinessExecutive(string id)
            => Ok(await _businessExecutiveService.DeleteBusinessExecutive(id));
    }
}
