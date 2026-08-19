using BetaFit.Application.DTOs.Dashboard;
using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers;

/// <summary>Fornece as métricas exibidas na tela inicial do Desktop administrativo.</summary>
[ApiController]
[Route("api/v1/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
