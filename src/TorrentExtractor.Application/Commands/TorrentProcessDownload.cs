using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace TorrentExtractor.Application.Commands
{
    public class TorrentProcessDownload
    {
        public class Command : IRequest
        {
        }

        public class Handler : IRequestHandler<Command>
        {
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}