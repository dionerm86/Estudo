namespace Glass.Data.Model
{
    public interface IDadosCliente
    {
        uint Id { get; }
        bool Revenda { get; }
        bool CobrarAreaMinima { get; }
    }
}
