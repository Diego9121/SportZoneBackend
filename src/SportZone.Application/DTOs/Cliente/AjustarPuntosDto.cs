namespace SportZone.Application.DTOs.Cliente;

// Para acumular o redimir puntos del programa de fidelización (lo usará también el módulo de Ventas)
public class AjustarPuntosDto
{
    public int Puntos { get; set; } // positivo acumula, negativo redime
}
