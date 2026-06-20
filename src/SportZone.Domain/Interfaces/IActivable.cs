namespace SportZone.Domain.Interfaces;

// Polimorfismo: cualquier entidad que implemente esta interfaz puede activarse/desactivarse
// sin que el código que la usa necesite conocer de qué clase concreta se trata.
public interface IActivable
{
    bool Activo { get; set; }
}
