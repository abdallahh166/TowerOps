namespace TelecomPM.Application.Queries.Users.GetUsersByOffice;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.UserSpecifications;

public class GetUsersByOfficeQueryHandler : IRequestHandler<GetUsersByOfficeQuery, Result<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersByOfficeQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersByOfficeQuery request, CancellationToken cancellationToken)
    {
        var spec = new UsersByOfficeSpecification(request.OfficeId, request.OnlyActive ?? true);
        var users = await _userRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<UserDto>>(users.ToList());
        return Result.Success(dtos);
    }
}

