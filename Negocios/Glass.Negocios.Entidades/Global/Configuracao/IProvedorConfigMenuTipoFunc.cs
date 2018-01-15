namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor dos menus que o tipo de funcionário tem acesso.
    /// </summary>
    public interface IProvedorConfigMenuTipoFunc
    {
        /// <summary>
        /// Recupera a identificação do menu;
        /// </summary>
        /// <param name="idMenu"></param>
        /// <returns></returns>
        string ObtemIdentificacaoMenu(int idMenu);
    }
}
