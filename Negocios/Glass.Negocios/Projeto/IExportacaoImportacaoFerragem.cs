using Colosoft.Business;
using System.Collections.Generic;

namespace Glass.Projeto.Negocios
{
    public interface IExportacaoImportacaoFerragem
    {
        /// <summary>
        /// Exporta as ferragens selecionadas para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        byte[] Exportar(List<int> idsFerragem);

        /// <summary>
        /// Lê o arquivo de exportação e salva as ferragens contidas nele.
        /// </summary>
        SaveResult Importar(byte[] arquivo, bool substituirFerragemExistente);
    }
}
