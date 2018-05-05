namespace Glass.Data.Model
{
    public interface IAmbienteCalculo
    {
        uint Id { get; }
        int TipoDesconto { get; }
        decimal Desconto { get; }
        int TipoAcrescimo { get; }
        decimal Acrescimo { get; }
    }
}
