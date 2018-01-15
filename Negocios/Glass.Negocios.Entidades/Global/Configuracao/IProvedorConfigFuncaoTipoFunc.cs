namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor das funções que o tipo de funcionário tem acesso.
    /// </summary>
    public interface IProvedorConfigFuncaoTipoFunc
    {
        /// <summary>
        /// Recupera a identificação da função do menu;
        /// </summary>
        /// <param name="idFuncaoMenu"></param>
        /// <returns></returns>
        string ObtemIdentificacaoFuncaoMenu(int idFuncaoMenu);
    }
}
