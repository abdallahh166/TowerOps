namespace TowerOps.Application.Queries.Users.GetUserById;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Interfaces.Repositories;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsNoTrackingAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDetailDto>("User not found");

        var dto = _mapper.Map<UserDetailDto>(user);
        return Result.Success(dto);
    }
}

