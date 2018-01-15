namespace WebGlass.Business
{
    /// <summary>
    /// Classe base para as classes de fluxo do sistema.
    /// </summary>
    /// <typeparam name="Fluxo">O tipo da classe de fluxo que está sendo herdada.</typeparam>
    public abstract class BaseFluxo<Fluxo> : Glass.Pool.PoolableObject<Fluxo>
        where Fluxo : BaseFluxo<Fluxo>
    {
        
    }
}
