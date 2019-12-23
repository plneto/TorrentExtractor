using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, Entity entity)
        {
            var domainEvents = entity.DomainEvents.ToList();

            entity.ClearDomainEvents();

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}