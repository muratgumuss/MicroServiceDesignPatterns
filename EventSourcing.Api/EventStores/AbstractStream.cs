using EventSourcing.Shared.Events;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.Api.EventStores
{
    public abstract class AbstractStream
    {
        protected readonly LinkedList<IEvent> Events = new LinkedList<IEvent>();

        private string _streamName { get; }

        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(string streamName, IEventStoreConnection eventStoreConnection)
        {
            _streamName = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        public async Task SaveAsync()
        {
            //var newEvents = Events.ToList().Select(x => new EventStore.ClientAPI.EventData(
            //     Guid.NewGuid(),
            //     x.GetType().Name,
            //     true,
            //     Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, inputType: x.GetType())),
            //     Encoding.UTF8.GetBytes(x.GetType().FullName))).ToList();

            //await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);

            var newEvents = Events.Select(x => new EventStore.ClientAPI.EventData(
             Guid.NewGuid(),
             x.GetType().Name,
             true,
             Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x)),
             Encoding.UTF8.GetBytes(x.GetType().FullName))).ToList();

            try
            {
                await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);
                Events.Clear();
            }
            catch (ObjectDisposedException ex)
            {
                // Burada loglama yapıp bağlantıyı neden kaybettiğini incelemelisin.
                // Singleton bağlantı nesnesi birisi tarafından .Dispose() edilmiş.
                throw new Exception("Event Store bağlantısı beklenmedik şekilde kapatılmış!", ex);
            }

            Events.Clear();
        }
    }
}
