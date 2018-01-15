using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.SIntegra
{
    internal class Registro50 : Registro
    {
        #region Enumeradores

        /// <summary>
        /// Tipo de emitente da nota fiscal.
        /// </summary>
        public enum TipoEmitente
        {
            /// <summary>
            /// P - Nota emitida pela própria empresa.
            /// </summary>
            Proprio,

            /// <summary>
            /// T - Nota emitida por outra empresa.
            /// </summary>
            Terceiro
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

        #endregion

        #region Campos Privados

        private NotaFiscal _nf = null;
        private Loja _loja = null;
        private Cliente _cliente = null;
        private Fornecedor _fornecedor = null;
        private uint _idCfop = 0;
        private float? _aliqIcms = 0;
        private decimal _valorIcms = 0;
        private decimal _bcIcms = 0;
        private decimal _valorTotal = 0;
        
        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="nf">A nota fiscal.</param>
        public Registro50(NotaFiscal nf, uint idCfop)
        {
            _nf = nf;

            if (_nf.IdCliente == null && _nf.IdFornec == null)
                throw new Exception(String.Format("O remetente ou destinatário da nota {0} não foi informado.", nf.NumeroNFe));
            
            if (_nf.IdLoja != null) _loja = LojaDAO.Instance.GetElement(_nf.IdLoja.Value);
            if (_nf.IdCliente != null) _cliente = ClienteDAO.Instance.GetElementByPrimaryKey(_nf.IdCliente.Value);
            if (_nf.IdFornec != null) _fornecedor = FornecedorDAO.Instance.GetElement(_nf.IdFornec.Value);
            _idCfop = idCfop;
            _aliqIcms = _nf.AliqIcms;
            _valorIcms = nf.Valoricms;
            _bcIcms = nf.BcIcms;
            _valorTotal = _nf.IsNfImportacao ? NotaFiscalDAO.Instance.ObtemTotal(_nf.IdNf) : _nf.TotalNota;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 50; }
        }

        public string CNPJ
        {
            get { return FormatCpfCnpjInscEst(_nf.TipoDocumento == 2 && _cliente != null ? _cliente.CpfCnpj : _fornecedor != null ? _fornecedor.CpfCnpj :
                _cliente != null ? _cliente.CpfCnpj : String.Empty); }
        }

        public string InscEstadual
        {
            get { return FormatCpfCnpjInscEst(_nf.TipoDocumento == 2 && _cliente != null ? (_cliente.TipoPessoa == "J" && _cliente.IndicadorIEDestinatario != Glass.IndicadorIEDestinatario.ContribuinteIsento ? _cliente.RgEscinst : "ISENTO") : _fornecedor != null ? _fornecedor.RgInscEst :
                _cliente != null ? (_cliente.TipoPessoa == "J" && _cliente.IndicadorIEDestinatario != Glass.IndicadorIEDestinatario.ContribuinteIsento ? _cliente.RgEscinst : "ISENTO") : String.Empty); }
        }

        public DateTime DataEmissaoRecebimento
        {
            get { return _nf.TipoDocumento == 2 ? _nf.DataEmissao : _nf.DataSaidaEnt != null ? _nf.DataSaidaEnt.Value : _nf.DataEmissao; }
        }

        public string UF
        {
            get { return NotNullString(_nf.TipoDocumento == 2 && _cliente != null ? CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + _cliente.IdCidade.Value) : _fornecedor != null ? _fornecedor.Uf :
                _cliente != null ? CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + _cliente.IdCidade.Value) : String.Empty); }
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
            get { return NotNullString(CfopDAO.Instance.ObtemValorCampo<string>("codInterno", "idCfop=" + _idCfop)); }
        }

        public TipoEmitente Emitente
        {
            get { return _nf.CnpjEmitente == _loja.Cnpj ? TipoEmitente.Proprio : TipoEmitente.Terceiro; }
        }

        public decimal ValorTotal
        {
            get { return _valorTotal; }
        }

        public decimal BaseCalcICMS
        {
            get { return _bcIcms; }
        }

        public decimal ValorICMS
        {
            get { return _valorIcms; }
        }

        /// <summary>
        /// Valor amparado por isenção ou não incidência
        /// </summary>
        public double IsentaNaoTrib
        {
            get { return 0f; }
        }

        /// <summary>
        /// Valor que não confira débito ou crédito do ICMS
        /// </summary>
        public decimal Outras
        {
            get
            {
                decimal valorOutras = _valorTotal - _bcIcms;
                if (valorOutras < 0 && valorOutras >= (decimal)-0.03)
                    valorOutras = 0;
                else if (_nf.FinalidadeEmissao == (int)NotaFiscal.FinalidadeEmissaoEnum.Complementar)
                    valorOutras = 0;

                return _loja.Crt == 1 || _loja.Crt == 2 ? _valorTotal : valorOutras;
            }
        }

        public float AliquotaICMS
        {
            get 
            {
                float aliquotaICMSTemp = (float)Math.Round(_aliqIcms != null ? _aliqIcms.Value : _nf.BcIcms > 0 ? 
                    (float)(_valorIcms / _bcIcms) * 100F : 0, 2);

                float decimais = (float)Math.Round(aliquotaICMSTemp - (int)aliquotaICMSTemp, 2);

                float aliqIcms = decimais >= 0.98f ? (float)Math.Round(aliquotaICMSTemp, 0) : 
                    decimais == 0.01f ? aliquotaICMSTemp - 0.01f : aliquotaICMSTemp;

                return aliqIcms;
            }
        }

        public TipoSituacao Situacao
        {
            get { return _nf.Situacao != 4 ? TipoSituacao.Normal : TipoSituacao.Cancelado; }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 50 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os campos para devolução do texto
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = (_nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "" : CNPJ).PadLeft(14, '0');
            string n03 = (_nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "" : InscEstadual).PadRight(14);
            string n04 = FormatData(DataEmissaoRecebimento);
            string n05 = (_nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "" : UF).PadRight(2);
            string n06 = Modelo.PadLeft(2, '0');
            string n07 = Serie.ToString().PadRight(3);
            //string n08 = Numero.ToString().PadLeft(6, '0');
            string n08 = "";
            string numero = "";
            if (Numero.ToString().Length > 6)
            {
                for (int i = Numero.ToString().Length - 6; i < Numero.ToString().Length; i++)
                {
                    numero += Numero.ToString()[i];
                }

                n08 = numero;
            }
            else
                n08 = Numero.ToString().PadLeft(6, '0');
            
            string n09 = (_nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "" : CFOP).PadLeft(4, '0');
            string n10 = Emitente.ToString()[0].ToString();
            string n11 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(13, '0') : ValorTotal.ToString("0##########.#0").Remove(11, 1);
            string n12 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(13, '0') : BaseCalcICMS.ToString("0##########.#0").Remove(11, 1);
            string n13 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(13, '0') : ValorICMS.ToString("0##########.#0").Remove(11, 1);
            string n14 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(13, '0') : IsentaNaoTrib.ToString("0##########.#0").Remove(11, 1);
            string n15 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(13, '0') : Outras.ToString("0##########.#0").Remove(11, 1);
            string n16 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "".PadLeft(4, '0') : AliquotaICMS.ToString("0#.#0").Remove(2, 1);
            string n17 = _nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada ? "S" : "N";

            // Retorna o texto formatado com os dados da nota fiscal
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14 + n15 + n16 + n17;
        }

        #endregion
    }
}