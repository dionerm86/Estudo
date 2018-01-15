using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Classe de pesquisa para recuperação de dados dos Arquivos de cartão
    /// </summary>
    public class ArquivoCartaoNaoIdentificadoPesquisa
    {
        /// <summary>
        /// Id do arquivo
        /// </summary>
        public int IdArquivoCartaoNaoIdentificado { get; set; }

        /// <summary>
        /// Data de cadastro do Arquivo
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Situação
        /// </summary>
        public Data.Model.SituacaoArquivoCartaoNaoIdentificado Situacao { get; set; }

        /// <summary>
        /// Nome do funcionário de cadastro
        /// </summary>
        public string NomeFuncCad { get; set; }

        public string DescrSituacao { get { return Situacao.ToString(); } }

        public string NomeArquivo
        {
            get
            {              
                return Path.GetFileName(Directory.GetFiles(Data.Helper.Utils.GetArquivosCNIPath, "**", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Contains(string.Format(@"\{0}.", IdArquivoCartaoNaoIdentificado))).FirstOrDefault());
            }
        }

        public bool PodeCancelar
        {
            get
            {
                return ArquivoCartaoNaoIdentificado
                    .PodeCancelarArquivo(IdArquivoCartaoNaoIdentificado);
            }
        }
    }
}
