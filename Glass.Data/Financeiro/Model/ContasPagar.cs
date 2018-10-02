using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Data.DAL.CTe;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContasPagarDAO))]
	[PersistenceClass("contas_pagar")]
	public class ContasPagar : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCONTAPG", PersistenceParameterType.IdentityKey)]
        public uint IdContaPg { get; set; }

        [Log("Fornecedor", "Nome", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [Log("Transportador", "Nome", typeof(TransportadorDAO))]
        [PersistenceProperty("IDTRANSPORTADOR")]
        public uint? IdTransportador { get; set; }

        [Log("Plano de conta", "Descricao", typeof(PlanoContasDAO))]
        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }

        [Log("Compra")]
        [PersistenceProperty("IDCOMPRA")]
        public uint? IdCompra { get; set; }

        [Log("Sinal da Compra")]
        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [Log("Antecipação pagto. fornecedor")]
        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [Log("Custo Fixo")]
        [PersistenceProperty("IDCUSTOFIXO")]
        public uint? IdCustoFixo { get; set; }

        [Log("Encontro Contas")]
        [PersistenceProperty("IDENCONTROCONTAS")]
        public uint? IdEncontroContas { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja { get; set; }

        [Log("Pagamento")]
        [PersistenceProperty("IDPAGTO")]
        public uint? IdPagto { get; set; }

        [Log("Pagamento restante")]
        [PersistenceProperty("IDPAGTORESTANTE")]
        public uint? IdPagtoRestante { get; set; }

        [Log("Nota Fiscal", "NumeroNFe", typeof(NotaFiscalDAO))]
        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [Log("Cheque")]
        [PersistenceProperty("IDCHEQUEPAGTO")]
        public uint? IdChequePagto { get; set; }

        [Log("Comissão")]
        [PersistenceProperty("IDCOMISSAO")]
        public uint? IdComissao { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint? IdFormaPagto { get; set; }

        [Log("Lançamento de Imposto/Serviço Avulso")]
        [PersistenceProperty("IDIMPOSTOSERV")]
        public uint? IdImpostoServ { get; set; }

        [Log("CT-e")]
        [PersistenceProperty("IDCTE")]
        public uint? IdCte { get; set; }

        [Log("Crédito de Fornecedor")]
        [PersistenceProperty("IDCREDITOFORNECEDOR")]
        public uint? IdCreditoFornecedor { get; set; }

        [Log("Data de Vencimento")]
        [PersistenceProperty("DATAVENC")]
        public DateTime DataVenc { get; set; }

        [Log("Valor de Vencimento")]
        [PersistenceProperty("VALORVENC")]
        public decimal ValorVenc { get; set; }

        [Log("Data de Pagamento")]
        [PersistenceProperty("DATAPAGTO")]
        public DateTime? DataPagto { get; set; }

        [Log("Valor Pago")]
        [PersistenceProperty("VALORPAGO")]
        public decimal ValorPago { get; set; }

        [Log("Multa")]
        [PersistenceProperty("MULTA")]
        public decimal Multa { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [Log("Paga")]
        [PersistenceProperty("PAGA")]
        public bool Paga { get; set; }

        [Log("Boleto chegou")]
        [PersistenceProperty("BOLETOCHEGOU")]
        public bool BoletoChegou { get; set; }

        [Log("Número Boleto")]
        [PersistenceProperty("NUMBOLETO")]
        public string NumBoleto { get; set; }

        [Log("Contábil")]
        [PersistenceProperty("CONTABIL")]
        public bool Contabil { get; set; }

        [Log("Renegociada")]
        [PersistenceProperty("RENEGOCIADA")]
        public bool Renegociada { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("À Vista")]
        [PersistenceProperty("AVISTA")]
        public bool AVista { get; set; }

        [PersistenceProperty("DESCONTOPARC")]
        public decimal DescontoParc { get; set; }

        [PersistenceProperty("ACRESCIMOPARC")]
        public decimal AcrescimoParc { get; set; }

        [PersistenceProperty("MOTIVODESCONTOACRESC")]
        public string MotivoDescontoAcresc { get; set; }

        [PersistenceProperty("IDFUNCDESCACRESC")]
        public uint? IdFuncDescAcresc { get; set; }

        [PersistenceProperty("DATADESCACRESC")]
        public DateTime? DataDescAcresc { get; set; }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("NUMPARCMAX")]
        public int NumParcMax { get; set; }

        [PersistenceProperty("IDCHEQUE")]
        public int IdCheque { get; set; }

        [PersistenceProperty("DESTINOPAGTO")]
        public string DestinoPagto { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeFornec;

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec
        {
            get { return IdFornec > 0 ? IdFornec + " - " + _nomeFornec : _nomeFornec; }
            set { _nomeFornec = value; }
        }

        private string _nomeTransportador;

        [PersistenceProperty("NOMETRANSPORTADOR", DirectionParameter.InputOptional)]
        public string NomeTransportador
        {
            get { return IdTransportador > 0 ? IdTransportador + " - " + _nomeTransportador : null; }
            set { _nomeTransportador = value; }
        }

        public string NomeExibir
        {
            get { return NomeFornec ?? NomeTransportador; }
        }

        /// <summary>
        /// Retorna o nome do fornecedor/funcionário/comissionado sem a data que a comissão foi gerada,
        /// este campo é utilizado para agrupar por nome no relatório de contas pagas.
        /// </summary>
        [PersistenceProperty("NOMEFORNECSEMDATA", DirectionParameter.InputOptional)]
        public string NomeFornecSemData { get; set; }

        [PersistenceProperty("NF", DirectionParameter.InputOptional)]
        public string Nf { get; set; }

        [PersistenceProperty("NUMERONF", DirectionParameter.InputOptional)]
        public uint? NumeroNf { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        //[PersistenceProperty("FORMAPAGTO", DirectionParameter.InputOptional)]
        //public ulong FormaPagto { get; set; }
        private string _formaPagtoCompra;

        [Log("Forma Pagto.")]
        [PersistenceProperty("FORMAPAGTOCOMPRA", DirectionParameter.InputOptional)]
        public string FormaPagtoCompra
        {
            get
            {
                if (IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito)
                    return "Pagto. Bancário";
                else
                    return _formaPagtoCompra;
            }
            set { _formaPagtoCompra = value; }
        }

        [PersistenceProperty("DESCRCUSTOFIXO", DirectionParameter.InputOptional)]
        public string DescrCustoFixo { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("OBSCOMPRA", DirectionParameter.InputOptional)]
        public string ObsCompra { get; set; }

        [PersistenceProperty("OBSIMPOSTOSERV", DirectionParameter.InputOptional)]
        public string ObsImpostoServ { get; set; }

        [PersistenceProperty("TOTALEMABERTO", DirectionParameter.InputOptional)]
        public decimal TotalEmAberto { get; set; }

        [PersistenceProperty("TOTALRECEMDIA", DirectionParameter.InputOptional)]
        public decimal TotalRecEmDia { get; set; }

        [PersistenceProperty("TOTALRECCOMATRASO", DirectionParameter.InputOptional)]
        public decimal TotalRecComAtraso { get; set; }

        [PersistenceProperty("PREVISAOCUSTOFIXO", DirectionParameter.InputOptional)]
        public bool PrevisaoCustoFixo { get; set; }

        [PersistenceProperty("NOMEFUNCDESC", DirectionParameter.InputOptional)]
        public string NomeFuncDesc { get; set; }

        [PersistenceProperty("DESCRICAOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string DescricaoContaContabil { get; set; }

        [PersistenceProperty("CreditoFornec", DirectionParameter.InputOptional)]
        public decimal CreditoFornec { get; set; }

        [PersistenceProperty("NOMEFUNCPAGTO", DirectionParameter.InputOptional)]
        public string NomeFuncPagto { get; set; }

        private string _descrFormaPagto;

        [PersistenceProperty("DescrFormaPagto", DirectionParameter.InputOptional)]
        public string DescrFormaPagto
        {
            get
            {
                if (IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito)
                    return "Pagto. Bancário";
                else
                    return _descrFormaPagto;
            }
            set { _descrFormaPagto = value; }
        }

        #endregion

        #region Propriedades de Suporte

        public string NumNfGeral
        {
            get { return NumeroNf > 0 ? NumeroNf.ToString() : Nf; }
        }

        public string DescrContaPagar
        {
            get 
            {
                return
                    // Mostra a observação da conta a pagar
                    DescrPlanoConta + (Obs != null && Obs.Trim() != String.Empty ? " (" + Obs + ")" : String.Empty) +

                    // Mostra a observação da compra se houver
                    (FinanceiroConfig.FinanceiroPagto.ExibirObsCompraContasPagar && ObsCompra != null && ObsCompra.Trim() != String.Empty ?
                    " (" + ObsCompra + ")" : String.Empty) +

                    // Mostra a observação do imposto/serviço se houver
                    (FinanceiroConfig.FinanceiroPagto.ExibirObsImpostoServico && ObsImpostoServ != null && ObsImpostoServ.Trim() != String.Empty ?
                    " (" + ObsImpostoServ + ")" : String.Empty) +

                    // Se for conta parcial, mostra de qual pagamento veio
                    (IdPagtoRestante > 0 && IdConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoBoleto) ?
                    " (" + "Rest. Pagto: " + IdPagtoRestante + ")" : String.Empty) +

                    (IdCustoFixo > 0 ? " (Custo Fixo: " + CustoFixoDAO.Instance.ObtemValorCampo<string>("descricao", "idCustoFixo=" + IdCustoFixo.Value) + ")" : 
                    String.Empty);
            }
        }

        public string Referencia
        {
            get
            {
                string refer = String.Empty;

                if (IdCompra > 0)
                    refer += "Compra: " + IdCompra + " ";

                if (IdSinalCompra >0)
                    refer += "Sinal da Compra: " + IdSinalCompra + " ";

                if (IdPagto > 0)
                    refer += "Pagto: " + IdPagto + " ";

                if (!String.IsNullOrEmpty(NumNfGeral))
                    refer += "NF: " + NumNfGeral + " ";

                if (IdCustoFixo > 0)
                    refer += "Custo Fixo: " + IdCustoFixo + " ";

                if (IdImpostoServ > 0)
                {
                    var nfPedido = ImpostoServDAO.Instance.ObtemValorCampo<string>("Nf", "IdImpostoServ="+IdImpostoServ, null);

                    refer += "Imposto/Serv.: " + IdImpostoServ + " ";

                    if(!string.IsNullOrEmpty(nfPedido))
                        refer += "NF/Pedido.: " + nfPedido + " ";
                }

                if (IdAntecipFornec > 0)
                    refer += "Antecip. Pagto. Fornecedor: " + IdAntecipFornec + " ";

                if (IdEncontroContas > 0)
                    refer += "Encontro de Contas a Pagar/Receber: " + IdEncontroContas + " ";

                if (IdCte > 0)
                    refer += "CT-e: " + ConhecimentoTransporteDAO.Instance.ObtemValorCampo<uint>("numeroCte", "idCte=" + IdCte) + " ";

                if (IdCreditoFornecedor > 0)
                    refer += "Cr? Fornec.: " + IdCreditoFornecedor + " ";

                if (!String.IsNullOrEmpty(NumBoleto))
                    refer += "Cheque/Boleto: " + NumBoleto + " ";

                if (IdsNfDevolucao.Count > 0)
                {
                    var nunsNfe = new List<string>();

                    foreach (var id in IdsNfDevolucao)
                        nunsNfe.Add(NotaFiscalDAO.Instance.ObtemNumeroNf(null, id).ToString());

                    refer += "NF(Devolução): " + string.Join(",", nunsNfe.ToArray());
                }

                if (IdComissao > 0)
                    refer += "Comissão: " + IdComissao + " ";

                // Se a conta tiver sido paga, mas estiver com o valor pago zerado, é possível que a mesma tenha sido acertada em um encontro de contas
                if (Paga && ValorPago == 0)
                {
                    var idEncontroContas = ContasPagarEncontroContasDAO.Instance.ObterIdEncontroContas(IdContaPg);
                    if (idEncontroContas > 0)
                        refer += string.Format("Encontro contas: {0} ", idEncontroContas);
                }

                if (string.IsNullOrEmpty(refer) && IdPagtoRestante > 0)
                    refer = "Rest. Pagto: " + IdPagtoRestante + " ";

                return refer;
            }
        }

        public string BoletoChegouString
        {
            get { return BoletoChegou ? "Sim" : "Não"; }
        }

        public string DescrValorPagto
        {
            get
            {
                return ValorPago.ToString("C") + (Juros > 0 ? " (Juros: " + Juros.ToString("C") + ")" : String.Empty) +
                    (Multa > 0 ? " (Multa: " + Multa.ToString("C") + ")" : String.Empty);
            }
        }
        
        public bool EditVisible
        {
            get 
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                if (IdContaPg == 0)
                    return false;

                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);
            }
        }

        public bool ShowFormaPagto
        {
            get { return false; }
        }

        public string PagaString
        {
            get { return Paga ? "Sim" : "Não"; }
        }

        public string Color
        {
            get { return !Paga ? "Red" : DataVenc >= DataPagto ? "Green" : "Blue"; }
        }

        public string DescrNumParc
        {
            get { return NumParc + NumParcMax > 0 && IdCustoFixo == null ? NumParc + " / " + NumParcMax : ""; }
        }

        public string ObsDescAcresc
        {
            get
            {
                if (DescontoParc > 0)
                    return "Desc.: " + DescontoParc.ToString("C") + " (" + MotivoDescontoAcresc + ")";
                else if (AcrescimoParc > 0)
                    return "Acresc.: " + AcrescimoParc.ToString("C") + " (" + MotivoDescontoAcresc + ")";

                return "";
            }
        }

        private List<uint> _idsNfDevolucao = null;

        public List<uint> IdsNfDevolucao
        {
            get
            {
                if(_idsNfDevolucao == null)
                {
                    _idsNfDevolucao = new List<uint>();

                    if (IdNf.GetValueOrDefault(0) == 0)
                        return _idsNfDevolucao;

                    var idsNotas = NotaFiscalDAO.Instance.ObtemIdsNfReferenciadas(IdNf.Value);

                    if (idsNotas.Count == 0)
                        return _idsNfDevolucao;

                    foreach (var idNf in idsNotas)
                    {
                        if (CfopDAO.Instance.IsCfopDevolucao(NotaFiscalDAO.Instance.GetIdCfop(null, idNf)))
                            _idsNfDevolucao.Add(idNf);
                    }
                }

                return _idsNfDevolucao;
            }
        }

        public bool PossuiNfDevolucao
        {
            get
            {
                return IdsNfDevolucao.Count > 0;
            }
        }

        #endregion
    }
}