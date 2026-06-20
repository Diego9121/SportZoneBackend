using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.MovimientoStock;

namespace SportZone.Application.Interfaces.Servicios;

// Solo lectura a propósito: un MovimientoStock nunca se crea "a mano",
// solo lo generan automáticamente IngresoServicio y VentaServicio.
public interface IMovimientoStockServicio
{
    Task<PagedResultDto<MovimientoStockDto>> GetAllAsync(PaginacionQueryDto query, int? articuloVarianteId);
    Task<MovimientoStockDto?> GetByIdAsync(int id);
}
