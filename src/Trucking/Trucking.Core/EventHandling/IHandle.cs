using System.Threading.Tasks;

namespace Trucking.Core.EventHandling
{
    public interface IHandle<in T> where T : IDomainEvent
    {
        Task Handle(T domainEvent);
    }
}
