using Colosoft;

namespace Glass.Negocios.Componentes
{
    /// <summary>
    /// Implementação do método que recupera o descritor da entidade.
    /// </summary>
    public class ObtemDescritorEntidade : Negocios.Entidades.IObtemDescritor
    {
        /// <summary>
        /// Obtém o descritor de uma entidade.
        /// </summary>
        /// <returns></returns>
        public Colosoft.IEntityDescriptor ObtemDescritor<T>(int uid) where T : class, Colosoft.Business.IEntity
        {
            return SourceContext.Instance.GetDescriptor<T>(uid);
        }
    }
}
