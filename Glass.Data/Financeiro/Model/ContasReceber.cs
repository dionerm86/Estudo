using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContasReceberDAO))]
	[PersistenceClass("contas_receber")]
	public class ContasReceber : ModelBaseCadastro
    {
        [Flags]
        public enum TipoContaEnum : byte
        {
            Contabil = 1,
            NaoContabil = 2,
            Reposicao = 4,
            CupomFiscal = 8
        }

        public ContasReceber()
        {
            TipoConta = 
                FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ? 
                (byte)TipoContaEnum.CupomFiscal :
                (byte)TipoContaEnum.NaoContabil;
        }

        #region Propriedades

        [PersistenceProperty("IDCONTAR", PersistenceParameterType.IdentityKey)]
        public uint IdContaR { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Forma de pagamento", "Descricao", typeof(FormaPagtoDAO))]
        [PersistenceProperty("IDFORMAPAGTO")]
        public uint? IdFormaPagto { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [Log("Acerto")]
        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }

        [Log("Sinal")]
        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }

        [Log("Acerto parcial")]
        [PersistenceProperty("IDACERTOPARCIAL")]
        public uint? IdAcertoParcial { get; set; }

        /// <summary>
        /// Campo usado quando uma conta renegociada ou restante de um acerto é paga em outro acerto
        /// </summary>
        [Log("Acerto original")]
        [PersistenceProperty("IDACERTOORIGINAL")]
        public uint? IdAcertoOriginal { get; set; }

        [Log("Liberação de pedido")]
        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [Log("Obra")]
        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [Log("Troca/Devolução")]
        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }

        [Log("Encontro Contas")]
        [PersistenceProperty("IDENCONTROCONTAS")]
        public uint? IdEncontroContas { get; set; }

        [Log("Plano de conta", "Descricao", typeof(PlanoContasDAO))]
        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }

        [Log("Funcionário desc./acrésc.", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCDESCACRESC")]
        public uint? IdFuncDescAcresc { get; set; }

        [Log("Conta bancária", "Nome", typeof(ContaBancoDAO))]
        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [Log("Antecipação de Boleto")]
        [PersistenceProperty("IDANTECIPCONTAREC")]
        public uint? IdAntecipContaRec { get; set; }

        [Log("Devolução de Pagamento")]
        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }

        [Log("Acerto de Cheque")]
        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }

        [Log("Parcela de cartão")]
        [PersistenceProperty("ISPARCELACARTAO")]
        public bool IsParcelaCartao { get; set; }

        [PersistenceProperty("IDCONTARCARTAO")]
        public uint? IdContaRCartao { get; set; }

        [Log("Tipo Receb. Parc. Cartão")]
        [PersistenceProperty("TIPORECEBIMENTOPARCCARTAO")]
        public int? TipoRecebimentoParcCartao { get; set; }

        [Log("Data de vencimento")]
        [PersistenceProperty("DATAVEC")]
        public DateTime DataVec { get; set; }

        [Log("Valor de vencimento")]
        [PersistenceProperty("VALORVEC")]
        public decimal ValorVec { get; set; }

        [Log("Data de recebimento")]
        [PersistenceProperty("DATAREC")]
        public DateTime? DataRec { get; set; }

        [Log("Valor de recebimento")]
        [PersistenceProperty("VALORREC")]
        public decimal ValorRec { get; set; }

        [Log("Data da 1ª negociação")]
        [PersistenceProperty("DATAPRIMNEG")]
        public DateTime? DataPrimNeg { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [Log("Multa")]
        [PersistenceProperty("MULTA")]
        public decimal Multa { get; set; }

        [Log("Recebida")]
        [PersistenceProperty("RECEBIDA")]
        public bool Recebida { get; set; }

        [Log("Funcionário recebeu", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUREC")]
        public uint? UsuRec { get; set; }

        /// <summary>
        /// Refere-se a qual parcela do pedido esta parcela pertence
        /// </summary>
        [Log("Número da parcela")]
        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [Log("Número total de parcelas")]
        [PersistenceProperty("NUMPARCMAX")]
        public int NumParcMax { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("Acréscimo")]
        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        [Log("Motivo desc./acrésc.")]
        [PersistenceProperty("MOTIVODESCONTOACRESC")]
        public string MotivoDescontoAcresc { get; set; }

        [Log("Data desc./acrésc.")]
        [PersistenceProperty("DATADESCACRESC")]
        public DateTime? DataDescAcresc { get; set; }

        [PersistenceProperty("IDORIGEMDESCONTOACRESCIMO")]
        public uint? IdOrigemDescontoAcrescimo { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Renegociada")]
        [PersistenceProperty("RENEGOCIADA")]
        public bool Renegociada { get; set; }

        [Log("Juros do Cartão")]
        [PersistenceProperty("VALORJUROSCARTAO")]
        public decimal ValorJurosCartao { get; set; }

        [PersistenceProperty("IdArquivoRemessa")]
        public int? IdArquivoRemessa { get; set; }

        [Log("Número Arquivo Remessa CNAB")]
        [PersistenceProperty("NUMARQUIVOREMESSACNAB")]
        public int? NumeroArquivoRemessaCnab { get; set; }

        [Log("Número Documento CNAB")]
        [PersistenceProperty("NUMERODOCUMENTOCNAB")]
        public string NumeroDocumentoCnab { get; set; }

        [PersistenceProperty("IDNF", DirectionParameter.OutputOnlyInsert)]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDLOJA", DirectionParameter.OutputOnlyInsert)]
        public uint IdLoja { get; set; }

        [PersistenceProperty("TIPOCONTA")]
        public byte TipoConta { get; set; }

        [PersistenceProperty("TARIFABOLETO")]
        public bool TarifaBoleto { get; set; }

        [PersistenceProperty("TARIFAPROTESTO")]
        public bool TarifaProtesto { get; set; }

        [PersistenceProperty("IDCTE")]
        public int? IdCte { get; set; }

        [PersistenceProperty("VALOREXCEDENTEPCP")]
        public bool ValorExcedentePCP { get; set; }

        [PersistenceProperty("IDCONTARREF")]
        public int? IdContaRRef { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public int? IdCartaoNaoIdentificado { get; set; }

        [PersistenceProperty("IDARQUIVOQUITACAOPARCELACARTAO")]
        public int? IdArquivoQuitacaoParcelaCartao { get; set; }

        [PersistenceProperty("JURIDICO")]
        public bool Juridico { get; set; }

        //Id do funcionario que receberá a comissão
        [PersistenceProperty("IDFUNCCOMISSAOREC")]
        public int? IdFuncComissaoRec { get; set; }

        [PersistenceProperty("TOTALPAGO")]
        public decimal? TotalPago { get; set; }

        [PersistenceProperty("IDLOJARECEBIMENTO")]
        public int? IdLojaRecebimento { get; set; }

        [PersistenceProperty("DESCONTARCOMISSAO")]
        public bool? DescontarComissao { get; set; }

        [PersistenceProperty("RECEBIMENTOPARCIAL")]
        public bool? RecebimentoParcial { get; set; }

        [PersistenceProperty("RECEBIMENTOCAIXADIARIO")]
        public bool? RecebimentoCaixaDiario { get; set; }

        [PersistenceProperty("RECEBIMENTOGERARCREDITO")]
        public bool? RecebimentoGerarCredito { get; set; }

        [PersistenceProperty("DESTINOREC")]
        public string DestinoRec { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("TaxaJuros", DirectionParameter.InputOptional)]
        public string TaxaJuros { get; set; }

        [PersistenceProperty("DescricaoCartao", DirectionParameter.InputOptional)]
        public string DescricaoCartao { get; set; }

        [PersistenceProperty("IdContaBancoCnab", DirectionParameter.InputOptional)]
        public uint? IdContaBancoCnab { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public string NumeroNFe { get; set; }

        [PersistenceProperty("DescrPlanoConta", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        private string _nomeCli;

        [PersistenceProperty("NomeCli", DirectionParameter.InputOptional)]
        public string NomeCli
        {
            get { return _nomeCli != null ? _nomeCli : String.Empty; }
            set { _nomeCli = value; }
        }

        [PersistenceProperty("NOMECOMISSIONADO", DirectionParameter.InputOptional)]
        public string NomeComissionado { get; set; }

        private string _nomeFunc;

        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return !String.IsNullOrEmpty(_nomeFunc) ? BibliotecaTexto.GetTwoFirstNames(_nomeFunc) : _nomeFunc; }
            set { _nomeFunc = value; }
        }

        [PersistenceProperty("TELCLIENTE", DirectionParameter.InputOptional)]
        public string TelCliente { get; set; }

        [PersistenceProperty("CpfCnpjCliente", DirectionParameter.InputOptional)]
        public string CpfCnpjCliente { get; set; }

        [PersistenceProperty("InscEstadualCliente", DirectionParameter.InputOptional)]
        public string InscEstadualCliente { get; set; }

        [PersistenceProperty("LIMITECLIENTE", DirectionParameter.InputOptional)]
        public decimal LimiteCliente { get; set; }

        private string _nomeFuncDesc;

        [PersistenceProperty("NomeFuncDesc", DirectionParameter.InputOptional)]
        public string NomeFuncDesc
        {
            get { return !String.IsNullOrEmpty(_nomeFuncDesc) ? BibliotecaTexto.GetTwoFirstNames(_nomeFuncDesc) : _nomeFuncDesc; }
            set { _nomeFuncDesc = value; }
        }

        [PersistenceProperty("FormaPagto", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        [PersistenceProperty("NumAutCartao", DirectionParameter.InputOptional)]
        public string NumAutCartao { get; set; }

        /// <summary>
        /// Dia que este boleto foi antecipado
        /// </summary>
        [PersistenceProperty("DATAANTECIP", DirectionParameter.InputOptional)]
        public DateTime DataAntecip { get; set; }

        /// <summary>
        /// Busca valor recebido desde que não tenha recebido por crédito, usado na tela de histórico de cliente
        /// </summary>
        [PersistenceProperty("VALORRECSEMCREDITO", DirectionParameter.InputOptional)]
        public decimal ValorRecSemCredito { get; set; }

        [PersistenceProperty("TOTALCREDITOCLIENTE", DirectionParameter.InputOptional)]
        public decimal TotalCreditoCliente { get; set; }

        [PersistenceProperty("TOTALEMABERTO", DirectionParameter.InputOptional)]
        public decimal TotalEmAberto { get; set; }

        [PersistenceProperty("TOTALRECEMDIA", DirectionParameter.InputOptional)]
        public decimal TotalRecEmDia { get; set; }

        [PersistenceProperty("TOTALRECCOMATRASO", DirectionParameter.InputOptional)]
        public decimal TotalRecComAtraso { get; set; }

        [PersistenceProperty("DATALIBERACAO", DirectionParameter.InputOptional)]
        public DateTime? DataLiberacao { get; set; }

        [PersistenceProperty("CREDITOCLIENTE", DirectionParameter.InputOptional)]
        public decimal CreditoCliente { get; set; }

        [PersistenceProperty("CONTABANCO", DirectionParameter.InputOptional)]
        public string ContaBanco { get; set; }

        [PersistenceProperty("IDSCONTAS", DirectionParameter.InputOptional)]
        public string IdsContas { get; set; }

        [PersistenceProperty("VALORAGRUPADO", DirectionParameter.InputOptional)]
        public decimal ValorAgrupado { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("PEDIDOSLIBERACAO", DirectionParameter.InputOptional)]
        public string PedidosLiberacao { get; set; }

        [PersistenceProperty("PERCENTUALCOMISSAO", DirectionParameter.InputOptional)]
        public float PercentualComissao { get; set; }

        [PersistenceProperty("TIPOENTREGA", DirectionParameter.InputOptional)]
        public int? TipoEntrega { get; set; }

        // Propriedade criada para buscar o número do cheque na tela do histórico do cliente.
        [PersistenceProperty("NUMCHEQUE", DirectionParameter.InputOptional)]
        public Int64 NumCheque { get; set; }

        [PersistenceProperty("LIBERACAONAOPOSSUINOTAFISCALGERADA", DirectionParameter.InputOptional)]
        public bool LiberacaoNaoPossuiNotaFiscalGerada { get; set; }

        [PersistenceProperty("DESCRICAOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string DescricaoContaContabil { get; set; }

        [PersistenceProperty("DescrOrigemDescontoAcrescimo", DirectionParameter.InputOptional)]
        public string DescrOrigemDescontoAcrescimo { get; set; }

        [PersistenceProperty("Protestado", DirectionParameter.InputOptional)]
        public bool Protestado { get; set; }

        [PersistenceProperty("ValorImpostos", DirectionParameter.InputOptional)]
        public decimal ValorImpostos { get; set; }

        [PersistenceProperty("ValorBaseCalcComissao", DirectionParameter.InputOptional)]
        public decimal ValorBaseCalcComissao { get; set; }

        [PersistenceProperty("IdFuncComissao", DirectionParameter.InputOptional)]
        public int IdFuncComissao { get; set; }

        [PersistenceProperty("IdComissao", DirectionParameter.InputOptional)]
        public int? IdComissao { get; set; }

        [PersistenceProperty("ReferenciaPedidoHistoricoCliente", DirectionParameter.InputOptional)]
        public string ReferenciaPedidoHistoricoCliente { get; set; }

        #endregion

        #region Propriedades de Suporte

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterios { get; set; }

        public string DataVencPrimNeg
        {
            get 
            {  
                return DataPrimNeg != null ? DataVec.ToString("d") + " (1ª Neg.: " + DataPrimNeg.Value.ToString("d") + ")" : 
                    DataVec.ToString("d");
            }
        }

        public string RptData
        {
            get { return DateTime.Now.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string NumParcString
        {
            get
            {
                return NumParc + "/" + NumParcMax;
            }
            set { }
        }

        public string Color
        {
            get { return !Recebida ? "Red" : DataVec >= DataRec ? "Green" : "Blue"; }
        }

        public string DataVecString
        {
            get { return DataVec.ToString("dd/MM/yyyy"); }
            set { DataVec = !String.IsNullOrEmpty(value) ? DateTime.Parse(value) : DataVec; }
        }

        public string IdNomeCli
        {
            get { return IdCliente + " - " + _nomeCli; }
        }

        public string NomeRota
        {
            get { return RotaDAO.Instance.ObtemDescrRota(IdCliente); }
        }

        private string _enderecoCliente = null;

        public string IdNomeTelCli
        {
            get 
            {
                string nome = IdCliente + " - " + _nomeCli;

                if (FinanceiroConfig.ContasReceber.ExibirTelefoneComNomeCliente)
                    nome += " - Tel.: " + TelCliente;
                else if (FinanceiroConfig.ContasReceber.ExibirTelefoneComNomeClienteEEndereco)
                {
                    nome += " - Tel.: " + TelCliente;

                    if (_enderecoCliente == null)
                        _enderecoCliente = "\n\t" + ClienteDAO.Instance.ObtemEnderecoCompleto(IdCliente);

                    nome += _enderecoCliente;
                }

                return nome;
            }
        }

        public string RelatorioPedido
        {
            get
            {
                if (IdLiberarPedido != null)
                    return "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + IdLiberarPedido.Value;
                else if (IdPedido != null)
                    return "../Relatorios/RelPedido.aspx?idPedido=" + IdPedido.Value + "&tipo=0";
                else if (IdAcertoParcial != null)
                    return "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + IdAcertoParcial.Value;
                else if (IdSinal != null)
                    return "../Relatorios/RelBase.aspx?rel=Sinal&idSinal=" + IdSinal.Value;
                else if (IdEncontroContas != null)
                    return "../Relatorios/RelBase.aspx?rel=EncontroContas&IdEncontroContas=" + IdEncontroContas.Value;
                else if (IdObra != null)
                    return "../Relatorios/RelBase.aspx?rel=Obra&obraDetalhada=false&idObra=" + IdObra;
                else
                    return "";
            }
        }

        public string DescrSituacaoAntecip
        {
            get { return Recebida ? "Quitado" : "Antecipado"; }
        }

        public string DescrSituacaoProdPedido
        {
            get
            {
                try
                {
                    return "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public bool DeleteVisible
        {
            get { return Recebida && (!Renegociada || ValorRec > 0); }
        }

        public bool ExibirNotaPromissoria
        {
            get 
            {
                string idsConta = "," + UtilsPlanoConta.ContasTodosTiposBoleto() + "," + UtilsPlanoConta.ContasTodosTiposCartao() + "," +
                    UtilsPlanoConta.ContasTodosTiposPrazo() + ",";
                return idsConta.Contains("," + IdConta.ToString() + ",") && (PedidoConfig.LiberarPedido && IdLiberarPedido > 0 ?
                    LiberarPedidoDAO.Instance.ExibirNotaPromissoria((uint)IdLiberarPedido) : FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0);
            }
        }

        public string TextoJuros
        {
            get 
            {
                return Juros + Multa > 0 ? ("(" + (Juros > 0 ? " Juros: " + Juros.ToString("C") : String.Empty) +
                    (Multa > 0 ? " Multa: " + Multa.ToString("C") : String.Empty) + ")").Replace("( ", "(") : String.Empty;
            }
        }

        public string DescrComissao
        {
            get
            {
                return (PercentualComissao / 100).ToString("P") + " (" + (ValorRec * ((decimal)PercentualComissao / (decimal)100)).ToString("N2") + ")";
            }
        }

        /// <summary>
        /// Observação tratada para ser exibida corretamente via javascript (tela de acerto por exemplo)
        /// </summary>
        public string ObsScript
        {
            get
            {
                return Obs != null ? Obs.Replace("\n", "").Replace("\r", "").Replace("'", "").Replace("\"", String.Empty) : String.Empty;
            }
        }

        public string Referencia
        {
            get
            {
                string retorno = "";

                if (!string.IsNullOrEmpty(NumeroNFe))
                    retorno += "NF-e: " + NumeroNFe + " ";
                else if (IdNf > 0)
                    retorno += "NF-e: " + NotaFiscalDAO.Instance.ObtemNumeroNf(null, IdNf.GetValueOrDefault()) + " ";

                if (IdAcerto > 0)
                    retorno += string.Format("Acerto: {0} ", IdAcerto);

                if (IdAcertoParcial > 0)
                    retorno += string.Format("Acerto parcial: {0} ", IdAcertoParcial);

                if (IdAcertoOriginal > 0)
                    retorno += string.Format("Acerto original: {0} ", IdAcertoOriginal);

                if (IdAntecipContaRec > 0)
                    retorno += "Antecip. Boleto: " + IdAntecipContaRec + " ";

                if (IdDevolucaoPagto > 0)
                    retorno += "Devolução Pagto: " + IdDevolucaoPagto + " ";

                if (IdLiberarPedido > 0)
                    retorno += "Liberação" + (IdConta == null && IdContaR == 0 && IdPedido == null ? " " + NumParcString + ": " : ": ") + 
                        IdLiberarPedido + " ";

                if (IdObra > 0)
                    retorno += "Obra: " + IdObra + " ";

                if (IdPedido > 0)
                {
                     retorno += "Pedido: " + IdPedido + " ";
 
                    if (FinanceiroConfig.FinanceiroRec.ExibirPedCliComIdPedidoContasRec)
                    {
                        var codCli = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", "idPedido=" + IdPedido);
                        if (!String.IsNullOrEmpty(codCli))
                            retorno += "Ped. Cli.: " + codCli + " ";
                    }
                }

                if (IdTrocaDevolucao > 0)
                    retorno += "Troca/Devolução: " + IdTrocaDevolucao + " ";
                
                if (NumCheque > 0)
                    retorno += "Cheque: " + NumCheque + " ";

                if (IdAcertoCheque > 0)
                    retorno += "Acerto de Cheque: " + IdAcertoCheque + " ";

                if (IdEncontroContas > 0)
                    retorno += "Encontro de Contas a Pagar/Receber: " + IdEncontroContas + " ";

                if (IdCte > 0)
                    retorno += "CT-e: " + Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemNumeroCte((uint)IdCte.Value);

                if (IdCartaoNaoIdentificado > 0)
                    retorno += "Cartão não Identificado: " + IdCartaoNaoIdentificado + " ";

                if (IdSinal > 0)
                {
                    var idsPedido = PedidoDAO.Instance.ObtemIdsPeloSinal(IdSinal.Value);
                    idsPedido = !string.IsNullOrEmpty(idsPedido) ? string.Format("Pedido(s): {0} ", idsPedido) : string.Empty;

                    retorno += string.Format("{0} {1}", SinalDAO.Instance.GetReferencia(IdSinal.Value), idsPedido);
                }
                else if (IdLiberarPedido > 0 && FinanceiroConfig.FinanceiroRec.ExibirIdPedidoComLiberacaoContasRec)
                    retorno += string.Format("Pedido(s): {0} ", LiberarPedidoDAO.Instance.IdsPedidos(null, IdLiberarPedido.Value.ToString()));
                else if (IdAcerto > 0 && FinanceiroConfig.FinanceiroRec.ExibirIdPedidoComAcertoContasRec)
                {
                    var idsPedido = PedidoDAO.Instance.ObterIdsPedidoPeloAcerto(null, (int)IdAcerto.Value);

                    if (string.IsNullOrEmpty(idsPedido) && IdLiberarPedido.GetValueOrDefault() == 0)
                    {
                        var idsLiberarPedido = LiberarPedidoDAO.Instance.ObterIdsLiberarPedidoPeloAcerto(null, (int)IdAcerto.Value);

                        if (!string.IsNullOrEmpty(idsLiberarPedido))
                            idsPedido += LiberarPedidoDAO.Instance.IdsPedidos(null, idsLiberarPedido);
                    }
                    else if (IdLiberarPedido > 0)
                        idsPedido += LiberarPedidoDAO.Instance.IdsPedidos(null, IdLiberarPedido.Value.ToString());

                    if (!string.IsNullOrEmpty(idsPedido) || IdLiberarPedido > 0)
                        retorno += string.Format("Pedido(s): {0} ", idsPedido);
                }

                if (IdContaRRef > 0)
                    retorno += ContasReceberDAO.Instance.GetReferencia((uint)IdContaRRef) + " ";

                return retorno.Trim();
            }
        }

        public string UrlRelatorio
        {
            get
            {
                return IdAcerto > 0 ? "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + IdAcerto :
                    "../Relatorios/RelBase.aspx?rel=ContaRecebida&idContaR=" + IdContaR;
            }
        }

        /// <summary>
        /// Descrição do tipo de entrega
        /// </summary>
        public string DescrTipoEntrega
        {
            get { return TipoEntrega.HasValue ? Utils.GetDescrTipoEntrega(TipoEntrega) : string.Empty; }
        }

        public bool Contabil
        {
            get 
            { 
                return ((TipoContaEnum)TipoConta & TipoContaEnum.Contabil) == TipoContaEnum.Contabil;
            }
        }

        public bool BoletoVisivel
        {
            get
            {
                if (!Glass.Configuracoes.FinanceiroConfig.FinanceiroRec.ExibirCnab)
                    return false;

                if (FinanceiroConfig.EmitirBoletoSemNota && !FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto)
                    return true;

                if (FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto && IdConta.GetValueOrDefault(0) > 0)
                    // Chamado 12327. A condição "Contabil" foi feita especificamente para a Modelo Vidros,
                    // os boletos não estavam sendo exibidos para as outras empresas configuradas com a opção
                    // "FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto".
                    return (FinanceiroConfig.BoletoVisivelApenasContabil ? Contabil : true) &&
                        UtilsPlanoConta.ContasRecebimentoBoleto().Contains("," + IdConta.Value + ",");
                else
                    return FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ? Contabil :
                        IdNf.GetValueOrDefault(0) > 0 || !string.IsNullOrEmpty(NumeroNFe);
            }
        }

        public bool GerouCNAB
        {
            get { return IdArquivoRemessa.GetValueOrDefault(0) > 0; }
        }

        public decimal TotalChequeDevolvido { get; set; }

        public string ObsDescAcresc
        {
            get
            {
                if (Desconto > 0)
                    return " - Desc.: " + Desconto.ToString("C") + " (" + MotivoDescontoAcresc + ")";
                else if (Acrescimo > 0)
                    return " - Acresc.: " + Acrescimo.ToString("C") + " (" + MotivoDescontoAcresc + ")";

                return "";
            }
        }

        IList<RegistroArquivoRemessa> _registrosArquivoRemessa;

        private IList<RegistroArquivoRemessa> RegistrosArquivoRemessa
        {
            get
            {
                if (_registrosArquivoRemessa == null)
                    _registrosArquivoRemessa = RegistroArquivoRemessaDAO.Instance.GetListRegistros(IdContaR);

                return _registrosArquivoRemessa;
            }
        }

        string _nossoNumero;

        public string NossoNumero
        {
            get
            {
                if (string.IsNullOrEmpty(_nossoNumero))
                {
                    if (!GerouCNAB)
                        _nossoNumero = "";
                    else
                        _nossoNumero = RegistrosArquivoRemessa.Where(f => !string.IsNullOrEmpty(f.NossoNumero)).Select(f => f.NossoNumero).FirstOrDefault();
                }

                return _nossoNumero;
            }
        }

        string _banco;

        public string Banco
        {
            get
            {
                if (string.IsNullOrEmpty(_banco))
                {
                    if (!GerouCNAB)
                    {
                        _banco = "";
                    }
                    else
                    {
                        if (IdContaBancoCnab.GetValueOrDefault(0) == 0)
                            _banco = "";
                        else
                            _banco = ContaBancoDAO.Instance.ObtemNome(IdContaBancoCnab.Value);
                    }
                }

                return _banco;
            }
        }

        public decimal ValorComissao { get; set; }

        public string DescrFormaPagtoPlanoConta
        {
            get
            {
                if (IdConta == null)
                    return string.Empty;

                /* Chamados 42922 e 43563. */
                if (IdConta.Value == (int)UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ParcelamentoObra))
                    return "Parcelamento de Obra";

                var formaPagto = UtilsPlanoConta.GetFormaPagtoByIdConta(IdConta.Value);

                return formaPagto != null ? formaPagto.Descricao : string.Empty;
            }
        }

        [Log("Descricao Formas Pagto")]
        public string DescrFormaPagto
        {
            get
            {
                if (string.IsNullOrEmpty(FormaPagto))
                    return "";

                var fp = FormaPagto.Split(',');                

                for (int i = 0; i < fp.Length; i++)
                {
                    if (fp[i] == "Cartão" && NumAutCartao != null)
                    {
                        var numAutCartao = NumAutCartao.Split(',');
                        fp[i] += " (" + string.Join(", ", numAutCartao.Where(f => !string.IsNullOrEmpty(f))) + ")";
                    }
                }

                return string.Join(", ", fp.GroupBy(f => f).Select(f => f.Key)) + (Renegociada ? " (Renegociada)" : "");
            }
        }

        public string DescrTipoCartao
        {
            get
            {
                if (IdConta.GetValueOrDefault(0) == 0)
                    return "";

                var tipoCartao = UtilsPlanoConta.ObterTipoCartaoPorConta(IdConta.Value);
                
                if (tipoCartao == null)
                    return "";
                                
                return TipoCartaoCreditoDAO.Instance.ObterTipoCartaoComDescricaoCompleta(tipoCartao.Value).Descricao;
            }
        }

        #endregion
    }
}