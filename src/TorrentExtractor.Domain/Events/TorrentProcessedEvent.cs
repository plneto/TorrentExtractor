using MediatR;

namespace TorrentExtractor.Domain.Events
{
    public class TorrentProcessedEvent : INotification
    {
        public TorrentProcessedEvent(string name, string destination)
        {
            Name = name;
            Destination = destination;
        }

        public string Name { get; }

        public string Destination { get; }
    }
}