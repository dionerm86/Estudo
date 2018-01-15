namespace Glass.Negocios.Entidades
{
    /// <summary>
    /// Assinatura da interface de obtenção do descritor de uma entidade.
    /// </summary>
    public interface IObtemDescritor
    {
        /// <summary>
        /// Obtém o descritor de uma entidade.
        /// </summary>
        /// <returns></returns>
        Colosoft.IEntityDescriptor ObtemDescritor<T>(int uid)
            where T : class, Colosoft.Business.IEntity;
    }
}
