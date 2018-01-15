using System;
using Glass.Data.Model;
using Glass.Data.Model.Cte;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;

namespace Glass.Data.SIntegra
{
    internal class Registro70 : Registro
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

        private NotaFiscal _nf;
        private ConhecimentoTransporte _cte;
        private ImpostoCte _icms;

        private string _cpfCnpjEmitente;
        private string _inscrEstadualEmitente;
        private string _ufEmitente;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Registro70(NotaFiscal nf)
        {
            _nf = nf;
            
            var fornec = nf.IdFornec > 0 ? FornecedorDAO.Instance.GetElement(nf.IdFornec.Value) : null;
            var cliente = nf.IdCliente > 0 ? ClienteDAO.Instance.GetElement(nf.IdCliente.Value) : null;

            _cpfCnpjEmitente = fornec != null ? fornec.CpfCnpj : cliente.CpfCnpj;
            _inscrEstadualEmitente = fornec != null ? fornec.RgInscEst : cliente.RgEscinst;
            _ufEmitente = fornec != null ? fornec.Uf : cliente.Uf;
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Registro70(ConhecimentoTransporte cte)
        {
            try
            {
                _cte = cte;
                _icms = ImpostoCteDAO.Instance.GetElement(cte.IdCte, Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Icms);

                var emitente = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(cte.IdCte).
                    Find(x => x.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente);

                if (emitente == null)
                    throw new Exception("O emitente do cte de n�mero " + cte.NumeroCte + " n�o foi encontrado.");

                var fornec = emitente.IdFornec > 0 ? FornecedorDAO.Instance.GetElement(emitente.IdFornec.Value) : null;
                var cliente = emitente.IdCliente > 0 ? ClienteDAO.Instance.GetElement(emitente.IdCliente.Value) : null;
                var transp = emitente.IdTransportador > 0 ? TransportadorDAO.Instance.GetElement(emitente.IdTransportador.Value) : null;
                var loja = emitente.IdLoja > 0 ? LojaDAO.Instance.GetElement(emitente.IdLoja.Value) : null;

                _cpfCnpjEmitente = fornec != null ? fornec.CpfCnpj : cliente != null ? cliente.CpfCnpj : transp != null ? transp.CpfCnpj : loja.Cnpj;
                _inscrEstadualEmitente = fornec != null ? fornec.RgInscEst : cliente != null ? cliente.RgEscinst : transp != null ? transp.InscEst : loja.InscEst;
                _ufEmitente = fornec != null ? fornec.Uf : cliente != null ? cliente.Uf : transp != null ? transp.Uf : loja.Uf;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro : CTE número: {0} {1} ",  cte.NumeroCte, ex) );

            }
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 70; }
        }

        public string Cnpj
        {
            get { return FormatCpfCnpjInscEst(_cpfCnpjEmitente); }
        }

        public string InscEst
        {
            get { return FormatCpfCnpjInscEst(_inscrEstadualEmitente); }
        }

        public DateTime DataEmissao
        {
            get
            {
                return _cte != null ? (_cte.TipoDocumentoCte == 2 ? _cte.DataEmissao : _cte.DataEntradaSaida ?? _cte.DataEmissao) :
                    _nf.TipoDocumento == 2 ? _nf.DataEmissao : _nf.DataSaidaEnt ?? _nf.DataEmissao;
            }
        }

        public string UfEmit
        {
            get { return FormatCpfCnpjInscEst(_ufEmitente); }
        }

        public string Modelo
        {
            get { return _cte != null ? _cte.Modelo : _nf.Modelo; }
        }

        public string Serie
        {
            get { return _cte != null ? _cte.Serie : _nf.Serie; }
        }

        public int Subserie
        {
            get { return 0; }
        }

        public uint Numero
        {
            get { return _cte != null ? (uint)_cte.NumeroCte : _nf.NumeroNFe; }
        }

        public string CFOP
        {
            get { return NotNullString(_cte != null ? CfopDAO.Instance.ObtemCodInterno(_cte.IdCfop) : _nf.CodCfop); }
        }

        public decimal ValorTotal
        {
            get { return _cte != null ? _cte.ValorTotal : _nf.TotalNota; }
        }

        public decimal BcIcms
        {
            get { return _icms != null ? _icms.BaseCalc : _nf != null ? _nf.BcIcms : 0; }
        }

        public decimal ValorIcms
        {
            get { return _icms != null ? _icms.Valor : _nf != null ? _nf.Valoricms : 0; }
        }

        public float Isenta
        {
            get { return 0F; }
        }

        public decimal Outras
        {
            get 
            { 
                return ValorTotal; 
            }
        }

        /// <summary>
        /// 0-Outros
        /// 1-CIF
        /// 2-FOB
        /// </summary>
        public int TipoFrete
        {
            get { return 0; }
        }

        public TipoSituacao Situacao
        {
            get
            {
                return (_cte != null && _cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.Cancelado) ||
                    (_nf != null && _nf.Situacao != (int)NotaFiscal.SituacaoEnum.Cancelada) ? TipoSituacao.Normal : TipoSituacao.Cancelado;
            }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 70 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = Cnpj.PadLeft(14, '0');
            string n03 = InscEst.PadRight(14);
            string n04 = FormatData(DataEmissao);
            string n05 = UfEmit.PadRight(2);
            string n06 = Modelo.ToString().PadLeft(2, '0');
            string n07 = Serie;
            string n08 = Subserie.ToString().PadLeft(2, '0');
            string n09 = Numero.ToString().PadLeft(6, '0');
            string n10 = CFOP.ToString().PadLeft(4, '0');
            string n11 = ValorTotal.ToString("0##########.#0").Remove(11, 1);
            string n12 = BcIcms.ToString("0###########.#0").Remove(12, 1);
            string n13 = ValorIcms.ToString("0###########.#0").Remove(12, 1);
            string n14 = Isenta.ToString("0###########.#0").Remove(12, 1);
            string n15 = Outras.ToString("0###########.#0").Remove(12, 1);
            string n16 = TipoFrete.ToString();
            string n17 = Situacao == TipoSituacao.Normal ? "N" : "S";

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14 + n15 + n16 + n17;
        }

        #endregion
    }
}