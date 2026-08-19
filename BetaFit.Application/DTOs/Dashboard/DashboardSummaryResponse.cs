namespace BetaFit.Application.DTOs.Dashboard;

/// <summary>
/// Métricas simples para a tela inicial do Desktop administrativo.
/// Deliberadamente enxuto: sem métricas falsas ou de vendas, já que
/// não há pedidos ou pagamentos reais neste projeto institucional.
/// </summary>
public class DashboardSummaryResponse
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int FeaturedProducts { get; set; }
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
}
