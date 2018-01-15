using System;
using System.Collections.Generic;
using System.IO;

namespace Glass.Financeiro.Negocios
{
    public interface IArquivoCartaoNaoIdentificadoFluxo
    {
        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        Colosoft.Business.SaveResult SalvarArquivoCartaoNaoIdentificado(Entidades.ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado);

        /// <summary>
        /// Cancela o arquivo passado
        /// </summary>
        Colosoft.Business.SaveResult CancelarArquivoCartaoNaoIdentificado(Entidades.ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado, string motivo);

        /// <summary>
        /// Recupera informações dos arquivos de CNI
        /// </summary>
        IList<Entidades.ArquivoCartaoNaoIdentificadoPesquisa> PesquisarArquivosCartaoNaoIdentificado(Data.Model.SituacaoArquivoCartaoNaoIdentificado? situacao,
            DateTime? dataImportIni, DateTime? dataImportFim, string funcCad);

        /// <summary>
        /// Recupera o Arquivo de Cartão não identificado com base no Id
        /// </summary>
        Entidades.ArquivoCartaoNaoIdentificado ObterArquivoCartaoNaoIdentificado(int idArquivoCartaoNaoIdentificado);

        /// <summary>
        /// Importa o arquivo
        /// </summary>
        ImportarArquivoCartaoNaoIdentificadoResultado Importar(Stream stream, string extensao);
    }
}
