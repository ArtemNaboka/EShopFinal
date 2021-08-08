using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IOrderReservingService
    {
        Task ReserveAsync(Order order);
        Task SendReservationAsync(Order order);
    }
}
