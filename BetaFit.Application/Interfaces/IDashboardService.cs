using BetaFit.Application.DTOs.Dashboard;

namespace BetaFit.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
