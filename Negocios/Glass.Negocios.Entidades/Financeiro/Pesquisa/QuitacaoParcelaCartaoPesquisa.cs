using System;
using System.Drawing;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Classe de pesquisa para recuperação dos dados de quitar parcela de cartão
    /// </summary>
    public class QuitacaoParcelaCartaoPesquisa
    {
        /// <summary>
        /// Id do Quitação Parcela de Cartão
        /// </summary>
        public int IdQuitacaoParcelaCartao { get; set; }

        /// <summary>
        /// Id do Arquivo Quitação Parcela de Cartão
        /// </summary>
        public int IdArquivoQuitacaoParcelaCartao { get; set; }

        /// <summary>
        /// Número de autorização do cartão
        /// </summary>
        public string NumAutCartao { get; set; }

        /// <summary>
        /// ùltimos Dígitos do Cartão
        /// </summary>
        public string UltimosDigitosCartao { get; set; }

        /// <summary>
        /// Valor da Parcela
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo do cartão (Debito/Credito)
        /// </summary>
        public Glass.Data.Model.TipoCartaoEnum Tipo { get; set; }

        /// <summary>
        /// Bandeira do Cartão
        /// </summary>
        public int Bandeira { get; set; }

        /// <summary>
        /// Número da Parcela
        /// </summary>
        public int NumParcela { get; set; }

        /// <summary>
        /// Total de parcelas
        /// </summary>
        public int NumParcelaMax { get; set; }

        /// <summary>
        /// Parcela
        /// </summary>
        public string Parcela
        {
            get { return NumParcela + "/" + NumParcelaMax; }
        }

        /// <summary>
        /// Tarifa 
        /// </summary>
        public decimal Tarifa { get; set; }

        /// <summary>
        /// Identifica se o registro quitou sua parcela
        /// </summary>
        public bool Quitada { get; set; }

        /// <summary>
        /// Data de vencimento da parcela
        /// </summary>
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Data de cadastro
        /// </summary>
        public DateTime DataCadastro { get; set; }

        public int IdUsuarioCadastro { get; set; }

        /// <summary>
        /// Nome do funcionario que realizou o cadastro
        /// </summary>
        public string NomeFuncionarioCadastro { get; set; }

        public Color CorLinha
        {
            get
            {
                return Quitada ? Color.Green : Color.Red;
            }
        }
    }
}
