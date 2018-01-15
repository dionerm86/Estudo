namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor dos menus que o usuário tem acesso.
    /// </summary>
    public interface IProvedorConfigMenuFunc
    {
        /// <summary>
        /// Recupera a identificação do menu;
        /// </summary>
        /// <param name="idMenu"></param>
        /// <returns></returns>
        string ObtemIdentificacaoMenu(int idMenu);
    }
}
