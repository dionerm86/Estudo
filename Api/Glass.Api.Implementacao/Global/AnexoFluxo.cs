using Glass.Data.Model;

namespace Glass.Api.Implementacao
{
    /// <summary>
    /// Implementação do fluxo de anexo do sistema
    /// </summary>
    public class AnexoFluxo : IAnexoFluxo
    {
        /// <summary>
        /// Insere um anexo no sistema.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="idParent"></param>
        /// <param name="buffer"></param>
        /// <param name="fileName"></param>
        /// <param name="descricao"></param>
        public void AnexarArquivo(IFoto.TipoFoto tipo, uint idParent, byte[] buffer, string fileName, string descricao)
        {
            Glass.Data.Helper.Anexo.InserirAnexo(tipo, idParent, buffer, fileName, descricao);
        }
    }
}
