using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.UtilEndpoints
{
    public class CompanyInfoUpdateAPI : BaseAsyncEndpoint.WithRequest<CompanyInfo>.WithResponse<CompanyInfo>
    {
        private IAsyncRepository<CompanyInfo> _companyRepository;
        private readonly IAuthorizationService _authorizationService;

        public CompanyInfoUpdateAPI(IAsyncRepository<CompanyInfo> companyRepository, IAuthorizationService authorizationService)
        {
            _companyRepository = companyRepository;
            _authorizationService = authorizationService;
        }

        [SwaggerOperation(
            Summary = "Update companyinfo",
            Description = "Update companyinfo",
            OperationId = "company.update",
            Tags = new[] { "UtilsEndpoints" })
        ]
        [HttpPut("api/company")]
        public override async Task<ActionResult<CompanyInfo>> HandleAsync(CompanyInfo request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.COMPANY_INFO, UserOperations.Update))
                return Unauthorized();

            await _companyRepository.UpdateAsync(request);
            return Ok(request);
        }
    }
    
    public class CompanyInfoGetAPI : BaseAsyncEndpoint.WithoutRequest.WithResponse<CompanyInfo>
    {
        private IAsyncRepository<CompanyInfo> _companyRepository;

        public CompanyInfoGetAPI(IAsyncRepository<CompanyInfo> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [SwaggerOperation(
            Summary = "Get company info by Id",
            Description = "Get company info by Id",
            OperationId = "company.get",
            Tags = new[] { "UtilsEndpoints" })
        ]
        [HttpGet("api/company")]
        public override async Task<ActionResult<CompanyInfo>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
         
            var companyInfo = await _companyRepository.GetByIdAsync("CPM_INFO");
            return Ok(companyInfo);
        }
    }
}