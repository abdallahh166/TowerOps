using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelecomPM.Application.Common;

using MediatR;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}