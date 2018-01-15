using Glass.Financeiro.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios
{
    public interface IQuitacaoParcelaCartaoFluxo
    {
        /// <summary>
        /// Retorna a lista de Quitação Parcela Cartão referente ao id do arquivo passado
        /// </summary>
        /// <param name="idArquivoQuitacaoParcelaCartao"></param>
        /// <returns></returns>
        IList<QuitacaoParcelaCartaoPesquisa> PesquisarQuitacaoParcelaCartao(int idArquivoQuitacaoParcelaCartao);

        List<QuitacaoParcelaCartaoPesquisa> CarregarArquivo(Stream stream);

        /// <summary>
        /// Importa o arquivo para quitar as parcelas do cartão
        /// </summary>
        Colosoft.Business.SaveResult QuitarParcelas(List<QuitacaoParcelaCartao> quitacaoParcelaCartaoPesquisa);

        /// <summary>
        /// Cancela o arquivo passado
        /// </summary>
        Colosoft.Business.SaveResult CancelarArquivoQuitacaoParcelaCartao(int idArquivoQuitacaoParcelaCartao, bool estornarMovimentacaoBancaria, DateTime? dataEstornoBanco, string motivo);

        /// <summary>
        /// Retorna lista de arquivos quitação parcela cartão
        /// </summary>
        IList<ArquivoQuitacaoParcelaCartaoPesquisa> PesquisarArquivoQuitacaoParcelaCartao();

        /// <summary>
        /// Insere um novo arquivo
        /// </summary>
        Colosoft.Business.SaveResult InserirNovoArquivo(Stream stream, string extensao);
    }
}
