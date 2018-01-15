using Colosoft.Business;

namespace Glass.Negocios.Entidades
{
    public interface ICriarEntidadeProvedor
    {
        IEntity CriarEntidade<TEntity>() where TEntity : IEntity, new();
    }
}
