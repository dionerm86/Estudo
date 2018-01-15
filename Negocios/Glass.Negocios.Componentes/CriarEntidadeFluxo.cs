using Colosoft;
using Colosoft.Business;

namespace Glass.Negocios.Componentes
{
    public class CriarEntidadeFluxo : Entidades.ICriarEntidadeProvedor
    {
        public IEntity CriarEntidade<TEntity>() where TEntity : IEntity, new()
        {
            return SourceContext.Instance.Create<TEntity>() as IEntity;
        }
    }
}


