using Glass.Data.Model;

namespace Glass.Api
{
    /// <summary>
    /// Assinatura do fluxo de negocio dos anexos do sistema.
    /// </summary>
    public interface IAnexoFluxo
    {
        /// <summary>
        /// Insere um anexo no sistema.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="idParent"></param>
        /// <param name="buffer"></param>
        /// <param name="fileName"></param>
        /// <param name="descricao"></param>
        void AnexarArquivo(IFoto.TipoFoto tipo, uint idParent, byte[] buffer, string fileName, string descricao);
    }
}
