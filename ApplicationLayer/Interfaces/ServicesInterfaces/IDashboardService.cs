using ApplicationLayer.DTOs;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardRelatedData();
    }
}
