using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro51 : Registro
    {
        #region Enumeradores

        /// <summary>
        /// Tipo de situação da nota fiscal.
        /// </summary>
        public enum TipoSituacao
        {
            /// <summary>
            /// N - Nota fiscal normal.
            /// </summary>
            Normal,

            /// <summary>
            /// S - Nota fiscal cancelada.
            /// </summary>
            Cancelado
        }

        #endregion

        #region Campos Privados

        private NotaFiscal _nf = null;
        private Loja _loja = null;
        private Cliente _cliente = null;
        private Fornecedor _fornecedor = null;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="nf">A nota fiscal.</param>
        public Registro51(NotaFiscal nf)
        {
            _nf = nf;

            if (_nf.IdLoja != null) _loja = LojaDAO.Instance.GetElement(_nf.IdLoja.Value);
            if (_nf.IdCliente != null) _cliente = ClienteDAO.Instance.GetElementByPrimaryKey(_nf.IdCliente.Value);
            if (_nf.IdFornec != null) _fornecedor = FornecedorDAO.Instance.GetElement(_nf.IdFornec.Value);
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 51; }
        }

        public string CNPJ
        {
            get { return FormatCpfCnpjInscEst(_nf.TipoDocumento == 2 && _cliente != null ? _cliente.CpfCnpj : _fornecedor != null ? _fornecedor.CpfCnpj : string.Empty); }
        }

        public string InscEstadual
        {
            get { return FormatCpfCnpjInscEst(_nf.TipoDocumento == 2 && _cliente != null ? (_cliente.TipoPessoa == "J" ? _cliente.RgEscinst : "ISENTO") : _fornecedor != null ? _fornecedor.RgInscEst : string.Empty); }
        }

        public DateTime DataEmissaoRecebimento
        {
            get { return _nf.TipoDocumento == 2 || _nf.DataSaidaEnt == null ? _nf.DataEmissao : _nf.DataSaidaEnt.Value; ; }
        }

        public string UF
        {
            get { return NotNullString(_nf.TipoDocumento == 2 && _cliente != null ? CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + _cliente.IdCidade.Value) : _fornecedor != null ? _fornecedor.Uf : string.Empty); }
        }

        public string Serie
        {
            get { return _nf.Serie; }
        }

        public uint Numero
        {
            get { return _nf.NumeroNFe; }
        }

        public string CFOP
        {
            get { return NotNullString(_nf.CodCfop); }
        }

        public decimal ValorTotal
        {
            get { return _nf.TotalNota; }
        }

        /// <summary>
        /// Montante do IPI
        /// </summary>
        public decimal ValorIPI
        {
            get { return _nf.ValorIpi; }
        }

        /// <summary>
        /// Valor amparado por isenção ou não incidência do IPI
        /// </summary>
        public decimal ValorIsentoIPI
        {
            get { return 0; }//(_nf.TotalNota - _nf.TotalProd) > 0 ? _nf.TotalNota - _nf.TotalProd : 0; }
        }

        /// <summary>
        /// Valor que não confira débito ou crédito do IPI (valor da nota para empresas do simples)
        /// </summary>
        public decimal ValorNaoIPI
        {
            get { return _nf.TotalNota; }
        }

        public TipoSituacao Situacao
        {
            get { return _nf.Situacao != (int)NotaFiscal.SituacaoEnum.Cancelada ? TipoSituacao.Normal : TipoSituacao.Cancelado; }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 51 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = CNPJ.PadLeft(14, '0');
            string n03 = InscEstadual.PadRight(14);
            string n04 = FormatData(DataEmissaoRecebimento);
            string n05 = UF.PadRight(2);
            string n06 = Serie.ToString().PadRight(3);
            string n07 = Numero.ToString().PadLeft(6, '0');
            string n08 = CFOP.PadLeft(4, '0');
            string n09 = ValorTotal.ToString("0##########.#0").Remove(11, 1);
            string n10 = ValorIPI.ToString("0##########.#0").Remove(11, 1);
            string n11 = ValorIsentoIPI.ToString("0##########.#0").Remove(11, 1);
            string n12 = ValorNaoIPI.ToString("0##########.#0").Remove(11, 1);
            string n13 = "".PadRight(20);
            string n14 = Situacao == TipoSituacao.Normal ? "N" : "S";

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14;
        }

        #endregion
    }
}