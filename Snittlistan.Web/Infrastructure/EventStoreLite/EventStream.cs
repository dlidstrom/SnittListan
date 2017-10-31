using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace EventStoreLite
{
    internal class EventStream
    {
        public EventStream()
        {
            History = new List<IDomainEvent>();
        }

        public string Id { get; set; }

        public List<IDomainEvent> History { get; internal set; }
    }
}