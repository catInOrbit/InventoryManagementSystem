// using System.Threading;
// using System.Threading.Tasks;
// using Ardalis.ApiEndpoints;
// using AutoMapper;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.eShopWeb.ApplicationCore.Interfaces;
// using Swashbuckle.AspNetCore.Annotations;
// using System;
// using System.Threading;
// using System.Linq;
//
//
//
// namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
// {
//     public class GetIMSUsers : BaseAsyncEndpoint.WithoutRequest.WithResponse<UsersListResponse>
//     {
//         private readonly IAsyncRepository<UserTest> _userListRepository;
//         private readonly IMapper _mapper;
//
//         public GetIMSUsers(IAsyncRepository<UserTest> userListRepository, IMapper mapper)
//         {
//             _userListRepository = userListRepository;
//             _mapper = mapper;
//         }
//
//
//         [HttpGet("api/getusers")]
//         [SwaggerOperation(
//             Summary = "List users",
//             Description = "List users for role manager",
//             OperationId = "GetIMSUser.List",
//             Tags = new[] { "ManagerEndpoints" })
//         ]
//         public override async Task<ActionResult<UsersListResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
//         {
//             var response = new UsersListResponse();
//             var users = _userListRepository.ListAllAsync(cancellationToken);
//             
//             // response.UserListDTO.AddRange(users.Select(_mapper.Map<UserTestDTO>));
//
//         }
//     }
// }