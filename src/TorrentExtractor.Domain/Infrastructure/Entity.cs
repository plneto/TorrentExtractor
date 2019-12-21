using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace TorrentExtractor.Domain.Infrastructure
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        private List<INotification> _domainEvents;

        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}