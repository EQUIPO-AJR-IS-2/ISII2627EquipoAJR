namespace AppForSEII.API.Models
{
    public enum MetodoPago
    {
        Bizum,
        Efectivo,
        Tarjeta,
        Transferencia,
        Metálico //Es redundante habiendo Efectivo, pero lo pongo pq así lo mustra el diagrama.
    }
}