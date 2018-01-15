using System;
using System.IO;
using System.Linq;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Classe de pesquisa para recuperação de dados dos Arquivos de quitar parcela de cartão
    /// </summary>
    public class ArquivoQuitacaoParcelaCartaoPesquisa
    {
        /// <summary>
        /// Id do arquivo
        /// </summary>
        public int IdArquivoQuitacaoParcelaCartao { get; set; }

        /// <summary>
        /// Data de cadastro do Arquivo
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Situação
        /// </summary>
        public Data.Model.SituacaoArquivoQuitacaoParcelaCartao Situacao { get; set; }

        public string DescrSituacao { get { return Situacao.ToString(); } }

        /// <summary>
        /// Nome do usuário de cadastro
        /// </summary>
        public string NomeFuncionarioCadastro { get; set; }

        public string NomeArquivo
        {
            get
            {
                return Path.GetFileName(Directory.GetFiles(Data.Helper.Utils.GetArquivoQuitacaoParcelaCartaoPath, "**", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Contains(string.Format(@"\{0}.", IdArquivoQuitacaoParcelaCartao))).FirstOrDefault());
            }
        }

        public bool PodeCancelar
        {
            get
            {
                return Situacao == Data.Model.SituacaoArquivoQuitacaoParcelaCartao.Ativo;
            }
        }
    }
}
