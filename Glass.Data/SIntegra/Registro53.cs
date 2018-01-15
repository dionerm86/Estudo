using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro53 : Registro
    {
        #region Enumeradores

        /// <summary>
        /// Tipo de emitente da nota fiscal.
        /// </summary>
        public enum TipoEmitente
        {
            /// <summary>
            /// P - Próprio
            /// </summary>
            Proprio,

            /// <summary>
            /// T - Terceiros
            /// </summary>
            Terceiros
        }

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

        /// <summary>
        /// Tipo da antecipação tributária.
        /// </summary>
        public enum TipoAntecipacao
        {
            /// <summary>
            /// " " - Substituição Tributária informada pelo substituto ou pelo substituído.
            /// </summary>
            SubstitutoOuSubstituido,

            /// <summary>
            /// "1" - Pagamento de substituição efetuada pelo destinatário, quando não efetuada ou efetuada a menor pelo substituto.
            /// </summary>
            PagamentoSTDestinatario,

            /// <summary>
            /// "2" - Antecipação tributária efetuada pelo destinatário apenas com complementação do diferencial de aliquota.
            /// </summary>
            AntecipacaoDestinatario,

            /// <summary>
            /// "3" - Antecipação tributária com MVA (Margem de Valor Agregado), efetuada pelo destinatário sem encerrar a fase de tributação.
            /// </summary>
            MVASemEncerrarTributacao,

            /// <summary>
            /// "4" - Antecipação tributária com MVA (Margem de Valor Agregado), efetuada pelo destinatário encerrando a fase de tributação.
            /// </summary>
            MVAEncerrandoTributacao
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
        public Registro53(NotaFiscal nf)
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
            get { return 53; }
        }

        public string CNPJ
        {
            get
            {
                if (_cliente != null || _fornecedor != null)
                    return FormatCpfCnpjInscEst(_cliente != null ? _cliente.CpfCnpj : _fornecedor.CpfCnpj);
                else
                    throw new Exception(string.Format("Nota Fiscal: {0} não possui cliente ou fornecedor. Favor Informar!", _nf.NumeroNFe));
            }
            
        }

        public string InscEstadual
        {
            get
            {
                if (_cliente != null || _fornecedor != null)
                    return FormatCpfCnpjInscEst(_cliente != null ? (_cliente.TipoPessoa == "J" ? _cliente.RgEscinst : "ISENTO") : _fornecedor.RgInscEst);
                else
                    throw new Exception(string.Format("Nota Fiscal: {0} não possui cliente ou fornecedor. Favor Informar!", _nf.NumeroNFe));
            }
        }

        public DateTime DataEmissaoRecebimento
        {
            get { return _nf.TipoDocumento == 2 || _nf.DataSaidaEnt == null ? _nf.DataEmissao : _nf.DataSaidaEnt.Value; }
        }

        public string UF
        {
            get
            {
                if (_cliente != null || _fornecedor != null)
                    return NotNullString(_cliente != null ? CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + _cliente.IdCidade.Value) : _fornecedor.Uf);
                else
                    throw new Exception(string.Format("Nota Fiscal: {0} não possui cliente ou fornecedor. Favor Informar!", _nf.NumeroNFe));
            }
        }

        public string Modelo
        {
            get { return NotNullString(_nf.Modelo); }
        }

        public string Serie
        {
            get { return NotNullString(_nf.Serie); }
        }

        public uint Numero
        {
            get { return _nf.NumeroNFe; }
        }

        public string CFOP
        {
            get { return NotNullString(_nf.CodCfop); }
        }

        public TipoEmitente Emitente
        {
            get
            {
                return new List<int> { (int)NotaFiscal.TipoDoc.Entrada, (int)NotaFiscal.TipoDoc.Saída }.Contains(_nf.TipoDocumento) ?
                    TipoEmitente.Proprio : TipoEmitente.Terceiros;
            }
        }

        public decimal BaseCalcICMSST
        {
            get { return _nf.BcIcmsSt; }
        }

        public decimal ICMSRetido
        {
            get { return _nf.ValorIcmsSt; }
        }

        public decimal DespesasAcessorias
        {
            get { return _nf.ValorFrete + _nf.ValorSeguro + _nf.OutrasDespesas; }
        }

        public TipoSituacao Situacao
        {
            get { return _nf.Situacao != (int)NotaFiscal.SituacaoEnum.Cancelada ? TipoSituacao.Normal : TipoSituacao.Cancelado; }
        }

        public TipoAntecipacao CodigoAntecipacao
        {
            get { return TipoAntecipacao.SubstitutoOuSubstituido; }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 53 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString();
            string n02 = CNPJ.PadLeft(14, '0');
            string n03 = InscEstadual.PadRight(14);
            string n04 = FormatData(DataEmissaoRecebimento);
            string n05 = UF.PadRight(2);
            string n06 = Modelo.PadLeft(2, '0');
            string n07 = Serie.ToString().PadRight(3);
            string n08 = Numero.ToString().PadLeft(6, '0');
            string n09 = CFOP.PadLeft(4, '0');
            string n10 = Emitente == TipoEmitente.Proprio ? "P" : "T";
            string n11 = BaseCalcICMSST.ToString("0##########.#0").Remove(11, 1);
            string n12 = ICMSRetido.ToString("0##########.#0").Remove(11, 1);
            string n13 = DespesasAcessorias.ToString("0##########.#0").Remove(11, 1);
            string n14 = Situacao == TipoSituacao.Normal ? "N" : "S";
            string n15 = (int)CodigoAntecipacao != 0 ? ((int)CodigoAntecipacao).ToString() : " ";
            string n16 = "".PadLeft(29);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14 + n15 + n16;
        }

        #endregion
    }
}