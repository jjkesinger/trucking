using System.Threading.Tasks;

namespace Trucking.Integration.Core.Capabilities
{
    public interface IHosReadProvider
    {
        Task ReadHos(object request);
    }
}
