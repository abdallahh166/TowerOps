namespace TowerOps.Application.Queries.Users.GetUsersByRole;

using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.UserSpecifications;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersByRoleQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var spec = new UsersByRoleSpecification(request.Role, request.OfficeId);
        var users = await _userRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<UserDto>>(users.ToList());
        return Result.Success(dtos);
    }
}

