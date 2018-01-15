using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.IO;
using GDA;
using System.Text;
using System.Threading;
using System.Linq;
using Glass.Configuracoes;
using Sync.Utils.Boleto.ArquivoRetorno;
using Sync.Utils.Boleto.Bancos;
using System.Drawing;
using NPOI.HSSF.UserModel;

namespace Glass.UI.Web.Utils
{
    [PersistenceBaseDAO(typeof(tempClienteDAO))]
    [PersistenceClass("cliente")]
    public class tempCliente
    {

    }


    [PersistenceBaseDAO(typeof(tempItemCarregamentoDAO))]
    [PersistenceClass("item_carregamento")]
    public class ItemCarregamentoTemp : ModelBaseCadastro
    {
        #region Enumerações

        public enum TipoItemCarregamento : long
        {
            Volume = 1,
            Venda,
            Revenda
        }

        #endregion

        #region Propiedades

        [PersistenceProperty("IDITEMCARREGAMENTO")]
        public uint IdItemCarregamento { get; set; }

        [PersistenceProperty("IDCARREGAMENTO")]
        public uint IdCarregamento { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("IDVOLUME")]
        public uint? IdVolume { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint? IdProdPedProducao { get; set; }

        [PersistenceProperty("CARREGADO")]
        public bool Carregado { get; set; }

        [PersistenceProperty("ENTREGUE")]
        public bool Entregue { get; set; }

        [PersistenceProperty("IDFUNCLEITURA")]
        public uint IdFuncLeitura { get; set; }

        [PersistenceProperty("DATALEITURA")]
        public DateTime DataLeitura { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("TipoItem", DirectionParameter.InputOptional)]
        public long? TipoItem { get; set; }

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("numEtiqueta", DirectionParameter.InputOptional)]
        public string Etiqueta { get; set; }

        [PersistenceProperty("PedidoEtiqueta", DirectionParameter.InputOptional)]
        public string PedidoEtiqueta { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.InputOptional)]
        public double Peso { get; set; }

        [PersistenceProperty("Qtde", DirectionParameter.InputOptional)]
        public double Qtde { get; set; }

        [PersistenceProperty("IdProdPedEsp", DirectionParameter.InputOptional)]
        public uint IdProdPedEsp { get; set; }

        [PersistenceProperty("IDOC", DirectionParameter.InputOptional)]
        public uint IdOc { get; set; }

        [PersistenceProperty("PEDCLI", DirectionParameter.InputOptional)]
        public string PedCli { get; set; }

        [PersistenceProperty("Rota", DirectionParameter.InputOptional)]
        public string Rota { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #region Peça

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodProduto { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescProduto { get; set; }

        [PersistenceProperty("Altura", DirectionParameter.InputOptional)]
        public double Altura { get; set; }

        [PersistenceProperty("Largura", DirectionParameter.InputOptional)]
        public double Largura { get; set; }

        [PersistenceProperty("M2", DirectionParameter.InputOptional)]
        public double M2 { get; set; }

        #endregion

        #region Volume

        [PersistenceProperty("DataFechamento", DirectionParameter.InputOptional)]
        public DateTime? DataFechamento { get; set; }

        #endregion

        #endregion

        #region Propiedades de Suporte

        public Color CorLinha
        {
            get
            {
                if (Carregado)
                    return Color.Green;
                else
                    return Color.Red;
            }
        }

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string CodInternoDescProd
        {
            get
            {
                return CodProduto + " - " + DescProduto;
            }
        }

        public bool LogEstornoVisible
        {
            get
            {
                if (IdItemCarregamento == 0)
                    return false;

                return EstornoItemCarregamentoDAO.Instance.TemEstorno(IdItemCarregamento);
            }
        }

        private string _imagemPecaUrl = null;

        public string ImagemPecaUrl
        {
            get
            {
                if (IdProdPedEsp == 0)
                    return "";

                if (_imagemPecaUrl == null)
                {
                    ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(IdProdPedEsp);
                    uint? idPecaItemProj = PecaItemProjetoDAO.Instance.ObtemIdPecaItemProjByIdProdPed(IdProdPedEsp);

                    ppe.Item = idPecaItemProj > 0 ? UtilsProjeto.GetItemPecaFromEtiqueta(PecaItemProjetoDAO.Instance.ObtemItem(idPecaItemProj.Value), PedidoEtiqueta) : 0;
                    _imagemPecaUrl = ppe.ImagemUrl;
                }

                return _imagemPecaUrl;
            }
        }

        public string SetoresPendentes
        {
            get { return SetorDAO.Instance.ObtemDescricaoSetoresRestantes(PedidoEtiqueta, IdProdPedEsp); }
        }

        public string NomeFuncLeitura
        {
            get
            {
                if (!Carregado)
                    return "";

                return FuncionarioDAO.Instance.GetNome(IdFuncLeitura);
            }
        }

        public string EtiquetaVolume
        {
            get
            {
                if (Carregado)
                    return "V" + IdVolume.GetValueOrDefault().ToString("D9");
                else
                    return string.Empty;
            }
        }

        public string PedidoEtiquetaVolume
        {
            get { return IdPedido + " (" + "V" + IdVolume.GetValueOrDefault().ToString("D9") + ")"; }
        }

        public string DataLeituraStr
        {
            get
            {
                if (!Carregado)
                    return "";

                return DataLeitura.ToString();
            }
        }

        #endregion
    }

    [PersistenceBaseDAO(typeof(tempContasReceberDAO))]
    [PersistenceClass("contas_receber")]
    public class tempContasReceber : ModelBaseCadastro
    {
        [Flags]
        public enum TipoContaEnum : byte
        {
            Contabil = 1,
            NaoContabil = 2,
            Reposicao = 4,
            CupomFiscal = 8
        }

        #region Propriedades

        [PersistenceProperty("IDCONTAR", PersistenceParameterType.IdentityKey)]
        public uint IdContaR { get; set; }


        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }


        [PersistenceProperty("IDFORMAPAGTO")]
        public uint? IdFormaPagto { get; set; }


        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }


        [PersistenceProperty("IDACERTO")]
        public uint? IdAcerto { get; set; }


        [PersistenceProperty("IDSINAL")]
        public uint? IdSinal { get; set; }


        [PersistenceProperty("IDACERTOPARCIAL")]
        public uint? IdAcertoParcial { get; set; }


        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }


        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }


        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint? IdTrocaDevolucao { get; set; }


        [PersistenceProperty("IDENCONTROCONTAS")]
        public uint? IdEncontroContas { get; set; }


        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }


        [PersistenceProperty("IDFUNCDESCACRESC")]
        public uint? IdFuncDescAcresc { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }


        [PersistenceProperty("IDANTECIPCONTAREC")]
        public uint? IdAntecipContaRec { get; set; }


        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint? IdDevolucaoPagto { get; set; }


        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint? IdAcertoCheque { get; set; }


        [PersistenceProperty("ISPARCELACARTAO")]
        public bool IsParcelaCartao { get; set; }


        [PersistenceProperty("TIPORECEBIMENTOPARCCARTAO")]
        public int? TipoRecebimentoParcCartao { get; set; }


        [PersistenceProperty("DATAVEC")]
        public DateTime DataVec { get; set; }


        [PersistenceProperty("VALORVEC")]
        public decimal ValorVec { get; set; }


        [PersistenceProperty("DATAREC")]
        public DateTime? DataRec { get; set; }


        [PersistenceProperty("VALORREC")]
        public decimal ValorRec { get; set; }


        [PersistenceProperty("DATAPRIMNEG")]
        public DateTime? DataPrimNeg { get; set; }


        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }


        [PersistenceProperty("MULTA")]
        public decimal Multa { get; set; }


        [PersistenceProperty("RECEBIDA")]
        public bool Recebida { get; set; }


        [PersistenceProperty("USUREC")]
        public uint? UsuRec { get; set; }

        /// <summary>
        /// Refere-se a qual parcela do pedido esta parcela pertence
        /// </summary>

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("NUMPARCMAX")]
        public int NumParcMax { get; set; }


        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }


        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }


        [PersistenceProperty("MOTIVODESCONTOACRESC")]
        public string MotivoDescontoAcresc { get; set; }


        [PersistenceProperty("DATADESCACRESC")]
        public DateTime? DataDescAcresc { get; set; }

        [PersistenceProperty("IDORIGEMDESCONTOACRESCIMO")]
        public uint? IdOrigemDescontoAcrescimo { get; set; }


        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }


        [PersistenceProperty("OBS")]
        public string Obs { get; set; }


        [PersistenceProperty("RENEGOCIADA")]
        public bool Renegociada { get; set; }


        [PersistenceProperty("VALORJUROSCARTAO")]
        public decimal ValorJurosCartao { get; set; }


        [PersistenceProperty("NUMARQUIVOREMESSACNAB")]
        public int? NumeroArquivoRemessaCnab { get; set; }


        [PersistenceProperty("NUMERODOCUMENTOCNAB")]
        public string NumeroDocumentoCnab { get; set; }

        [PersistenceProperty("IDNF", DirectionParameter.Input)]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDLOJA", DirectionParameter.OutputOnlyInsert)]
        public uint IdLoja { get; set; }

        [PersistenceProperty("TIPOCONTA")]
        public byte TipoConta { get; set; }

        [PersistenceProperty("TarifaBoleto")]
        public bool TarifaBoleto { get; set; }

        [PersistenceProperty("TarifaProtesto")]
        public bool TarifaProtesto { get; set; }

        #endregion

        #region Propriedades Estendidas

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

        [PersistenceProperty("DescrFormaPagto", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        /// <summary>
        /// Dia que este boleto foi antecipado
        /// </summary>
        [PersistenceProperty("DATAANTECIP", DirectionParameter.InputOptional)]
        public DateTime DataAntecip { get; set; }

        [PersistenceProperty("DESTINOREC", DirectionParameter.InputOptional)]
        public string DestinoRec { get; set; }

        /// <summary>
        /// Busca valor recebido desde que não tenha recebido por crédito, usado na tela de histórico de cliente
        /// </summary>
        [PersistenceProperty("VALORRECSEMCREDITO", DirectionParameter.InputOptional)]
        public decimal ValorRecSemCredito { get; set; }

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

        [PersistenceProperty("DESCRTIPOCARTAO", DirectionParameter.InputOptional)]
        public string DescrTipoCartao { get; set; }

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

        [PersistenceProperty("IdArquivoRemessa", DirectionParameter.InputOptional)]
        public uint? IdArquivoRemessa { get; set; }

        [PersistenceProperty("Protestado", DirectionParameter.InputOptional)]
        public bool Protestado { get; set; }

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

        public string IdNomeTelCli
        {
            get
            {
                string nome = IdCliente + " - " + _nomeCli;

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



        public string UrlRelatorio
        {
            get
            {
                return IdAcerto > 0 ? "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + IdAcerto :
                    "../Relatorios/RelBase.aspx?rel=ContaRecebida&idContaR=" + IdContaR;
            }
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
                if (FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto && IdConta.GetValueOrDefault(0) > 0)
                    return Contabil && UtilsPlanoConta.ContasRecebimentoBoleto().Contains("," + IdConta.Value + ",");
                else
                    return FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ? Contabil :
                        IdNf.GetValueOrDefault(0) > 0 || !string.IsNullOrEmpty(NumeroNFe);
            }
        }

        public bool GerouCNAB
        {
            get { return !string.IsNullOrEmpty(NumeroDocumentoCnab) || NumeroArquivoRemessaCnab.GetValueOrDefault(0) != 0; }
        }

        public decimal TotalChequeDevolvido { get; set; }

        public string ObsDescAcresc
        {
            get
            {
                if (Desconto > 0)
                    return "Desc.: " + Desconto.ToString("C") + " (" + MotivoDescontoAcresc + ")";
                else if (Acrescimo > 0)
                    return "Acresc.: " + Acrescimo.ToString("C") + " (" + MotivoDescontoAcresc + ")";

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

        #endregion
    }

    public abstract class tempBaseDAO<Model, DAO> : BaseDAO<Model, DAO>
        where Model : new()
        where DAO : tempBaseDAO<Model, DAO>, new()
    {
        public List<T> ExecuteMultipleScalar<T>(string sql, params GDAParameter[] parametros)
        {
            var metodo = this.GetType().GetMethod("ExecuteMultipleScalar", System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic).MakeGenericMethod(typeof(T));

            return metodo.Invoke(this, new object[] { sql, parametros }) as List<T>;
        }
    }

    public sealed class tempSituacaoProducaoDAO : tempBaseDAO<tempSituacaoProducaoDAO.Model, tempSituacaoProducaoDAO>
    {
        [PersistenceBaseDAO(typeof(tempSituacaoProducaoDAO))]
        public class Model
        {
            [PersistenceProperty("IdPedido")]
            public uint IdPedido { get; set; }

            [PersistenceProperty("DataPronto")]
            public DateTime? DataPronto { get; set; }

            [PersistenceProperty("Pronto")]
            public bool Pronto { get; set; }

            [PersistenceProperty("Entregue")]
            public bool Entregue { get; set; }

            [PersistenceProperty("TemEtiquetaImprimir")]
            public bool TemEtiquetaImprimir { get; set; }

            public Glass.Data.Model.Pedido.SituacaoProducaoEnum SituacaoProducao
            {
                get
                {
                    return TemEtiquetaImprimir ? Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pendente :
                        Entregue ? Glass.Data.Model.Pedido.SituacaoProducaoEnum.Entregue :
                        Pronto ? Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pronto :
                        Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pendente;
                }
            }

            [PersistenceProperty("SitProdPedido")]
            public int SitProdPedido { get; set; }

            public Glass.Data.Model.Pedido.SituacaoProducaoEnum SituacaoProducaoPedido
            {
                get
                {
                    return (Glass.Data.Model.Pedido.SituacaoProducaoEnum)SitProdPedido;
                }
            }
        }

        private string Sql()
        {
            var campoBase = "COALESCE(COUNT(ppp.IdProdPedProducao),0)=SUM(ppp.SituacaoProducao>={0})";
            var pronto = string.Format(campoBase, (int)SituacaoProdutoProducao.Pronto);
            var entregue = string.Format(campoBase, (int)SituacaoProdutoProducao.Entregue);

            var sql = string.Format(@"
                SELECT IdPedido, DataPronto, SUM(Pronto)>0 AS Pronto, SUM(Entregue)>0 AS Entregue, SUM(TemEtiquetaImprimir)>0 AS TemEtiquetaImprimir, SitProdPedido
                FROM (
	                SELECT ped.IdPedido, ped.DataPronto, {0} AS Pronto, {1} AS Entregue, SUM(pp.QtdImpresso < pp.Qtde) AS TemEtiquetaImprimir, ped.SituacaoProducao AS SitProdPedido
	                FROM pedido ped
		                INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido=pp.IdPedido)
		                INNER JOIN produto p ON (pp.IdProd=p.IdProd)
		                LEFT JOIN produto_pedido_producao ppp ON (pp.IdProdPed=ppp.IdProdPed) 
	                WHERE ped.Situacao<>{2}
		                AND ppp.Situacao={3}
		                AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
		                AND p.IdGrupoProd={4}
                        AND (pp.IdProdPedParent IS NULL OR pp.IdProdPedParent=0)
	                GROUP BY ped.IdPedido

	                UNION ALL SELECT pedExp.IdPedido, pedExp.DataPronto, {0} AS Pronto, {1} AS Entregue, SUM(pp.QtdImpresso < pp.Qtde) AS TemEtiquetaImprimir, ped.SituacaoProducao AS SitProdPedido
	                FROM pedido ped
		                INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido=pp.IdPedido)
		                INNER JOIN produto p ON (pp.IdProd=p.IdProd)
		                LEFT JOIN produto_pedido_producao ppp ON (pp.IdProdPed=ppp.IdProdPed) 
                        INNER JOIN pedido pedExp ON (ppp.IdPedidoExpedicao=pedExp.IdPedido)
	                WHERE ped.Situacao<>{2}
		                AND ppp.Situacao={3}
		                AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
		                AND p.IdGrupoProd={4}
                        AND (pp.IdProdPedParent IS NULL OR pp.IdProdPedParent=0)
	                GROUP BY ppp.IdPedidoExpedicao
                ) AS temp
                GROUP BY IdPedido
                ORDER BY IdPedido DESC", pronto, entregue, (int)Data.Model.Pedido.SituacaoPedido.Cancelado, (int)ProdutoPedidoProducao.SituacaoEnum.Producao, (int)NomeGrupoProd.Vidro);

            return sql;
        }

        public IEnumerable<Model> ObtemDadosSituacaoProducao(params uint[] idsPedidos)
        {
            string sql = Sql();
            var retorno = objPersistence.LoadData(sql).ToList();

            if (idsPedidos != null && idsPedidos.Length > 0)
                retorno = retorno.Where(x => idsPedidos.Contains(x.IdPedido)).ToList();

            return retorno;
        }

        public void AtualizaSituacaoProducao(Model item)
        {
            if (item == null || item.IdPedido == 0)
                return;

            var dataPronto = (int)item.SituacaoProducao < (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pronto ? (DateTime?)null :
                item.DataPronto ?? DateTime.Now;

            objPersistence.ExecuteCommand("update pedido set dataPronto=?pronto, situacaoProducao=" +
                (int)item.SituacaoProducao + " where idPedido=" + item.IdPedido,
                new GDAParameter("?pronto", dataPronto));
        }
    }

    public sealed class tempProdutoDAO : tempBaseDAO<tempProdutoDAO.Model, tempProdutoDAO>
    {
        [PersistenceBaseDAO(typeof(tempProdutoDAO))]
        public class Model { }

        public IList<uint> ObtemChapasVidro()
        {
            return ExecuteMultipleScalar<uint>(@"select idProd from produto p 
            inner join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
	        where s.tipoSubgrupo=1");
        }

        public void AtualizaFiscalProdutoLoja(uint idProd, uint idLoja, decimal saldo)
        {
            objPersistence.ExecuteCommand("update produto_loja set estoqueFiscal=?saldo where idProd=?idProd and idLoja=?idLoja",
                new GDAParameter("?saldo", saldo), new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        public void AtualizaSetorPeca(uint idSetor, uint idProdPedProducao)
        {
            objPersistence.ExecuteCommand("Update produto_pedido_producao set idSetor=" + idSetor + " WHERE idprodPedProducao=" + idProdPedProducao);
        }

        public bool TemLeituraSetor(uint idSetor, uint idProdPedProducao)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from leitura_producao where idProdPedProducao=" +
                idProdPedProducao + " and idSetor=" + idSetor) > 0;
        }
    }

    public sealed class tempBancoDAO : BaseDAO<tempBancoDAO.Model, tempBancoDAO>
    {
        [PersistenceBaseDAO(typeof(tempBancoDAO))]
        public class Model { }

        public void ApagarMov(uint idMovBanco)
        {
            objPersistence.ExecuteCommand("delete from mov_banco where idMovBanco=" + idMovBanco);
        }

        public void AtualizaSaldo(uint idContaBanco)
        {
            object temp = objPersistence.ExecuteScalar("select idMovBanco from mov_banco where idContaBanco=" +
                idContaBanco + " order by dataMov asc, idMovBanco asc limit 1");

            uint idMovBanco = temp != null ? (uint)temp : 0;

            if (idMovBanco == 0)
                return;

            FilaOperacoes.CorrecaoSaldoMovBanco.AguardarVez();

            try
            {
                DateTime dataMov = MovBancoDAO.Instance.ObtemDataMov(idMovBanco);

                // O round ao calcular o valor do saldo foi colocado para evitar erros caso o saldo possua 5 na terceira casa decimal
                string sqlAjuste = @"
                set @saldo := Round(Coalesce((Select saldo From mov_banco
                    Where idContaBanco=" + idContaBanco + @" and dataMov<=?dataMov 
                        And if(dataMov=?dataMov, idMovBanco<" + idMovBanco + @", true)
                    Order By dataMov desc, idMovBanco desc limit 1),0),2);

                Update mov_banco set saldo=(@saldo := @saldo + 
                    if(tipoMov=1, valorMov, -valorMov))

                Where idContaBanco=" + idContaBanco + @"
                    And dataMov>=?dataMov
                    And if(dataMov=?dataMov, idMovBanco>=" + idMovBanco + @", true)
                
                Order by dataMov asc, idMovBanco asc";

                objPersistence.ExecuteCommand(sqlAjuste, new GDAParameter("?dataMov", dataMov));
            }
            finally
            {
                FilaOperacoes.CorrecaoSaldoMovBanco.ProximoFila();
            }
        }
    }

    public sealed class tempClienteDAO : BaseDAO<tempCliente, tempClienteDAO>
    {
        public void AtualizaId(string cpfCnpj, uint idCli)
        {
            string sql = @"
                    UPDATE cliente SET id_cli=?idCli 
                    WHERE replace(replace(replace(replace(cpf_Cnpj, '.', ''), ' ', ''), '-', ''), '/', '')=?cpfCnpj";

            cpfCnpj = cpfCnpj.Replace("/", "").Replace("-", "").Replace(" ", "").Replace(".", "");

            objPersistence.ExecuteCommand(sql, new GDAParameter("?idCli", idCli), new GDAParameter("?cpfCnpj", cpfCnpj));
        }

        public void AtualizaTipoMercadoriaCodOtimizacao(uint idProd, string codOtimizacao, int? tipoMercadoria)
        {
            objPersistence.ExecuteCommand(@"
                UPDATE produto
                SET codOtimizacao=?codOtim,
                tipoMercadoria=?tm
                WHERE idProd=" + idProd, new GDAParameter("?codOtim", codOtimizacao), new GDAParameter("?tm", tipoMercadoria));
        }

        public void AtualizaIdFornec(uint idAntigo, uint idNovo)
        {
            objPersistence.ExecuteCommand(@"UPDATE fornecedor set idFornec=" + idNovo + @" WHERE idFornec=" + idAntigo);
        }

        public List<uint> BuscaTodosProdutos()
        {
            return objPersistence.LoadResult("SELECT idProd from produto").Select(f => f.GetUInt32(0)).ToList();
        }

        public void AtualizaValorTransferenciaProduto(uint idProd, decimal valor)
        {
            objPersistence.ExecuteCommand("UPDATE produto set valorTransferencia=?valor where idProd=" + idProd, new GDAParameter("?valor", valor));
        }

        public decimal ObtemValorBalcao(uint idProd)
        {
            return (decimal)objPersistence.ExecuteScalar("select valorBalcao from produto where idprod=" + idProd);
        }

        public void AtualizaAltLargBox(uint idProd, float alt, float larg/*, uint aplicacao, uint processo*/)
        {
            objPersistence.ExecuteCommand(@"
                UPDATE produto
                SET altura=?alt,
                largura=?larg
                WHERE idProd=" + idProd, new GDAParameter("?alt", alt), new GDAParameter("?larg", larg));
        }

        public void VincularProdBase(uint idCorVidro, float espessura, uint idProdBase)
        {
            string sql = @"
            UPDATE produto
            SET idProdBase=" + idProdBase + @"
            WHERE espessura=" + espessura + @"
                AND idCorVidro=" + idCorVidro + @"
                AND idSubgrupoProd=7";

            objPersistence.ExecuteCommand(sql);
        }
    }

    public sealed class tempMovEstoqueDAO : tempBaseDAO<tempMovEstoqueDAO.Model, tempMovEstoqueDAO>
    {
        [PersistenceBaseDAO(typeof(tempMovEstoqueDAO))]
        public class Model { }

        public IList<KeyValuePair<uint, uint>> ObtemIdsProdutos()
        {
            var itens = ExecuteMultipleScalar<string>(
                @"select distinct concat(m.idProd, ',', m.idLoja) from mov_estoque m
	            inner join produto p on (m.idProd=p.idProd)
	            inner join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
	            left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
            where m.idTrocaDevolucao is not null and coalesce(s.tipoCalculo, g.tipoCalculo, 1) in (2,10)");

            return itens.Select(x =>
            {
                var s = x.Split(',');
                return new KeyValuePair<uint, uint>(Glass.Conversoes.StrParaUint(s[0]), Glass.Conversoes.StrParaUint(s[1]));
            }).ToList();
        }

        public void ApagarMovimentacoes(KeyValuePair<uint, uint> item)
        {
            objPersistence.ExecuteCommand("delete from mov_estoque where idTrocaDevolucao is not null and idProd=?p and idLoja=?l",
                new GDAParameter("?p", item.Key), new GDAParameter("?l", item.Value));

            AtualizaSaldo(item.Key, item.Value);
        }

        public void AtualizaSaldo(uint idProd, uint idLoja)
        {
            uint id = ExecuteMultipleScalar<uint>(@"select idMovEstoque from mov_estoque where idProd=?p
            and idLoja=?l order by dataMov asc, idMovEstoque asc limit 1",
                new GDAParameter("?p", idProd), new GDAParameter("?l", idLoja)).FirstOrDefault();

            if (id > 0)
                MovEstoqueDAO.Instance.AtualizaSaldo(id);
        }

        public void GeraMovimentacoes(NotaFiscal nf)
        {
            // Credita o estoque real
            if (nf.GerarEstoqueReal && !EstoqueConfig.EntradaEstoqueManual)
            {
                var lstProdNf = ProdutosNfDAO.Instance.GetByNf(nf.IdNf);

                foreach (ProdutosNf p in lstProdNf)
                {
                    if (p.QtdeEntradaRestante <= 0)
                        continue;

                    bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                        Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscal(null, p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                        (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(p.IdProd, p.TotM, p.Qtde, p.Altura, p.Largura, false, false));

                    objPersistence.ExecuteCommand("update produtos_nf set qtdeEntrada=" + ProdutosNfDAO.Instance.ObtemQtdDanfe(p).ToString().Replace(",", ".") +
                        " where idProdNf=" + p.IdProdNf);
                }
            }
        }
    }

    public sealed class tempProdutoPedidoProducaoDAO : tempBaseDAO<tempProdutoPedidoProducaoDAO.Model, tempProdutoPedidoProducaoDAO>
    {
        [PersistenceBaseDAO(typeof(tempProdutoPedidoProducaoDAO))]
        public class Model { }

        public List<uint> ObtemIdsProdPedProducao()
        {
            return ExecuteMultipleScalar<uint>("select idprodpedproducao from produto_pedido_producao");
        }

        public void AtualizaSetor(uint idProdPedProducao, int idSetor)
        {
            objPersistence.ExecuteCommand(String.Format(@"update produto_pedido_producao set idSetor={1}
                where idProdPedProducao={0}", idProdPedProducao, idSetor));

            ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(idProdPedProducao, null, false);
        }

        public DateTime? ObtemDataPronto(uint idPedido)
        {
            var sql = @"
            select MAX(lp.dataLeitura)
            from leitura_producao lp
                inner join produto_pedido_producao ppp ON  (lp.idProdPedProducao = ppp.idProdPedProducao)
                inner join produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
            where idPedido = " + idPedido;

            return (DateTime?)objPersistence.ExecuteScalar(sql);
        }
    }

    public sealed class tempPedidoDAO : tempBaseDAO<tempPedidoDAO.Model, tempPedidoDAO>
    {
        [PersistenceBaseDAO(typeof(tempPedidoDAO))]
        public class Model { }

        public List<uint> GetPedidosOC()
        {
            var sql = @"
            SELECT DISTINCT p.idPedido
            FROM ordem_carga oc
                INNER JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                INNER JOIN pedido p ON (p.idPedido = poc.idPedido)
            WHERE idCarregamento IS NOT NULL
            AND tipoPedido <> 1";

            return ExecuteMultipleScalar<uint>(sql);
        }
    }

    public sealed class tempItemCarregamentoDAO : BaseDAO<ItemCarregamentoTemp, tempItemCarregamentoDAO>
    {
        public List<ItemCarregamentoTemp> GetByPedidoProduto(uint idPedido, uint idProd, uint idCarregamento)
        {
            var sql = @"
            SELECT *
            FROM item_carregamento
            WHERE idPedido = {0} AND idProd = {1} AND idCarregamento = {2}";

            return objPersistence.LoadData(string.Format(sql, idPedido, idProd, idCarregamento));
        }

        public void AtualizaIdProdPed(GDASession sessao, uint idItemCarregamento, uint idProdPed)
        {
            var sql = "UPDATE item_carregamento SET IdProdPed = ?idProdPed WHERE IdItemCarregamento = ?idItemCarregamento";

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?idItemCarregamento", idItemCarregamento), new GDAParameter("?idProdPed", idProdPed));
        }
    }

    public sealed class tempMovEstoqueClienteDAO : tempBaseDAO<tempMovEstoqueClienteDAO.Model, tempMovEstoqueClienteDAO>
    {
        [PersistenceBaseDAO(typeof(tempMovEstoqueClienteDAO))]
        public class Model { }

        public List<uint> ObtemIdMovEstoqueIni()
        {
            string sql = @"
                select idMovEstoqueCliente 
                from
                (
	                select * from mov_estoque_cliente
                    where datamov is not null
                    order by dataMov, idMovEstoqueCliente
                ) as tmp
                group by idLoja, idCliente, idProd";

            return ExecuteMultipleScalar<uint>(sql);
        }
    }

    public sealed class tempContasReceberDAO : tempBaseDAO<tempContasReceber, tempContasReceberDAO>
    {
        public IList<tempContasReceber> GetList(uint idLoja)
        {
            return objPersistence.LoadData(@"
                SELECT * 
                FROM contas_receber 
                WHERE idLiberarPedido IN (15,16) 
                    AND !recebida 
                    AND usuCad = 1 
                    AND idLoja = " + idLoja).ToList();
        }

        public void Apagar(string ids)
        {
            objPersistence.ExecuteCommand("DELETE FROM contas_receber WHERE idContaR IN (" + ids + ")");
        }
    }

    public sealed class tempSqlLimparBancoDAO : tempBaseDAO<tempSqlLimparBancoDAO.Model, tempSqlLimparBancoDAO>
    {
        [PersistenceBaseDAO(typeof(tempSqlLimparBancoDAO))]
        public class Model
        {
            [PersistenceProperty("TABLE_SCHEMA")]
            public string Table_Schema { get; set; }

            [PersistenceProperty("TABLE_NAME")]
            public string Table_Name { get; set; }

            [PersistenceProperty("TABLE_COMMENT")]
            public string Table_Comment { get; set; }

            [PersistenceProperty("SqlLimparBanco", DirectionParameter.InputOptional)]
            public string SqlLimparBanco { get; set; }
        }

        private string Sql()
        {
            var sql = @"SELECT IF(TABLE_COMMENT = 'IGNORAR LIMPAR BANCO', concat('/* A TABELA ', table_name, ' ESTÁ CONFIGURADA PARA NÃO SER APAGADA (POSSUI COMENTÁRIO IGNORAR LIMPAR BANCO EM SUA ESTRUTURA) */'), 
                CONCAT('DELETE FROM ', table_name, '; ALTER TABLE ', table_name, ' AUTO_INCREMENT=1; ')) as SqlLimparBanco 
                FROM information_schema.TABLES WHERE TABLE_SCHEMA = ?nomebanco ORDER BY TABLE_COMMENT DESC, TABLE_NAME;";

            return sql;
        }

        public string ObtemSqlLimparBanco(string nomeBanco)
        {
            string sql = Sql();
            string retorno = string.Empty;
            var listaSQL = objPersistence.LoadData(sql, new GDAParameter("?nomebanco", nomeBanco)).ToList();

            if (listaSQL != null && listaSQL.Count > 0)
                foreach (var comandoSQL in listaSQL)
                    retorno += comandoSQL.SqlLimparBanco + "\n";

            retorno += @"Update cliente Set credito=0, totalComprado=0, dt_ult_compra=Null; Update fornecedor Set credito = 0;" + "\n" +
                @"Insert Into produto_loja(IdLoja, IdProd, QtdEstoque, EstMinimo, Reserva, M2, EstoqueFiscal, Liberacao)
                (Select ? IdLoja As IdLoja, IdProd, 0 As QtdEstoque, 0 As EstMinimo, 0 As Reserva, 0 As M2, 0 As EstoqueFiscal, 0 As Liberacao From produto 
                Where IdProd Not In(Select IdProd From produto_loja Where IdLoja =? IdLoja)); ";
            return retorno;
        }
    }

    public sealed class tempProjetoModelo : tempBaseDAO<tempProjetoModelo.Model, tempProjetoModelo>
    {
        [PersistenceBaseDAO(typeof(tempProjetoModelo))]
        public class Model
        {
            [PersistenceProperty("IDPROJETOMODELO")]
            public uint IdProjetoModelo { get; set; }

            [PersistenceProperty("CODIGO")]
            public string Codigo { get; set; }

            [PersistenceProperty("NOMEFIGURAASSOCIADA")]
            public string NomeFiguraAssociada { get; set; }
        }

        private string Sql()
        {
            return "SELECT idProjetoModelo, codigo, nomeFiguraAssociada FROM projeto_modelo";
        }

        public void AtualizaNomeImagensProjetoModelo()
        {
            var sql = Sql();
            var listProjetoModelo = objPersistence.LoadData(sql).ToList();

            foreach (var projMod in listProjetoModelo)
            {
                if (projMod.NomeFiguraAssociada != null && projMod.NomeFiguraAssociada.Replace(".jpg", "").Replace(".JPG", "") == projMod.IdProjetoModelo.ToString())
                {
                    // Altera o nome da figura associada na pasta
                    ManipulacaoImagem.RenomearImagem(Data.Helper.Utils.GetModelosProjetoPath + projMod.NomeFiguraAssociada, Data.Helper.Utils.GetModelosProjetoPath + projMod.Codigo + "§E.jpg");
                    // Atualiza o nome da figura associada no banco
                    /* O nome no banco deve ser atualizado independente da existencia da imagem,
                     porque isso é utilizado ao perder todas as imagens da pasta e as imagens substittas deverão utilizar o padão novo*/
                    ProjetoModeloDAO.Instance.AtualizaNomeFigura(projMod.IdProjetoModelo, projMod.Codigo + "§E.jpg", null);
                }
            }

            var listaPathImagens = Directory.GetFiles(Data.Helper.Utils.GetModelosProjetoPath);
            foreach (var pathImagem in listaPathImagens)
            {
                var nomeImagem = pathImagem.Replace(Data.Helper.Utils.GetModelosProjetoPath, "").Replace(".jpg", "").Replace(".JPG", "");
                var idORcodigo = nomeImagem.Split('_')[0];
                uint idProjMod;
                // Se a imagem começar com o IdProjetoModelo e for uma peça
                if (uint.TryParse(idORcodigo, out idProjMod) && nomeImagem.Contains('_'))
                {
                    var item = nomeImagem.Split('_')[1];
                    var codigo = ProjetoModeloDAO.Instance.ObtemCodigo(idProjMod);
                    // Se o projeto existir no sistema
                    if (codigo != null && codigo.Length > 0)
                        ManipulacaoImagem.RenomearImagem(pathImagem, Data.Helper.Utils.GetModelosProjetoPath + codigo + "§" + item + ".jpg");
                }
            }
        }
    }

    public partial class ExecScript : System.Web.UI.Page
    {
        #region

        //    private static Thread atualizaValorBruto = null;
        //    private static Thread atualizarNumParc = null;
        //    private static Thread atualizaReservaLib = null;
        //    private static Thread atualizaNumEvento = null;
        //    private static Thread atualizaComissaoPedEsp = null;
        //    private static Thread atualizaProdutoImpressao = null;
        //    private static Thread atualizaMovEstoque = null;
        //    private static Thread atualizaValorMovEstoque = null;
        //    private static Thread atualizaCustoTotal = null;
        //    private static Thread atualizaDescontoAcrescimoCliente = null;

        //    protected void Page_Load(object sender, EventArgs e)
        //    {
        //        Label1.Text = ValorBruto.GetDescrCount() + " itens faltantes";
        //        Label2.Text = GenericDAO.Instance.ExecutarCount("select count(*) from contas_pagar where coalesce(numParc,0)=0 or coalesce(numParcMax,0)=0") + " itens faltantes";
        //        Label5.Text = (TempLogAltDAO.Instance.GetCountNumEvento() + TempLogCancDAO.Instance.GetCountNumEvento()) + " itens faltantes";
        //        Label6.Text = ComissaoPedEspDAO.Instance.Count() + " itens faltantes";
        //        Label7.Text = atualizaMovEstoque != null ? "Executando..." : "";
        //        Label8.Text = ValorMovEstoqueDAO.Instance.Count() + " itens faltantes";
        //        Label9.Text = TempDescontoAcrescimoClienteDAO.Instance.Count() + " itens faltantes";
        //    }

        //    public sealed class TempDescontoAcrescimoClienteDAO : BaseDAO<TempDescontoAcrescimoClienteDAO.Model, TempDescontoAcrescimoClienteDAO>
        //    {
        //        public class Model
        //        {
        //        }

        //        public int Count()
        //        {
        //            return GetCountByOrcamento() + GetCountByPedido() + GetCountByPedidoEspelho() +
        //                GetCountByTrocaDev() + GetCountByMaterial();
        //        }

        //        private T[] GetItens<T>(Func<bool, string> sql) where T : new()
        //        {
        //            PersistenceObject<T> obj = new PersistenceObject<T>(DBUtils.DbProvider);
        //            return obj.LoadData(sql(true)).ToArray();
        //        }

        //        private int GetCount(Func<bool, string> sql)
        //        {
        //            return objPersistence.ExecuteSqlQueryCount(sql(false));
        //        }

        //        private void Atualizar(string tabela, string campoId, IDescontoAcrescimo prod)
        //        {
        //            objPersistence.ExecuteCommand("update " + tabela + @" set valorDescontoCliente=?d,
        //                    valorAcrescimoCliente=?a where " + campoId + "=" + prod.Id, false,
        //                new GDAParameter("?d", prod.ValorDescontoCliente), new GDAParameter("?a", prod.ValorAcrescimoCliente));
        //        }

        //        #region Orçamento

        //        private string SqlOrcamento(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from produtos_orcamento
        //                    where idProdParent is not null and 
        //                        ((valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valorTabela) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valorTabela))";
        //        }

        //        public void CorrigeOrcamento()
        //        {
        //            foreach (ProdutosOrcamento po in GetItens<ProdutosOrcamento>(SqlOrcamento))
        //            {
        //                Utils.CalculaDiferencaCliente(po);
        //                Atualizar("produtos_orcamento", "idProd", po);
        //            }
        //        }

        //        private int GetCountByOrcamento()
        //        {
        //            return GetCount(SqlOrcamento);
        //        }

        //        #endregion

        //        #region Pedido

        //        private string SqlPedido(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from produtos_pedido
        //                    where (valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valorTabelaPedido) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valorTabelaPedido)";
        //        }

        //        public void CorrigePedido()
        //        {
        //            foreach (ProdutosPedido pp in GetItens<ProdutosPedido>(SqlPedido))
        //            {
        //                Utils.CalculaDiferencaCliente(pp);
        //                Atualizar("produtos_pedido", "idProdPed", pp);
        //            }
        //        }

        //        private int GetCountByPedido()
        //        {
        //            return GetCount(SqlPedido);
        //        }

        //        #endregion

        //        #region Pedido espelho

        //        private string SqlPedidoEspelho(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from produtos_pedido_espelho
        //                    where (valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valorVendido) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valorVendido)";
        //        }

        //        public void CorrigePedidoEspelho()
        //        {
        //            foreach (ProdutosPedidoEspelho pp in GetItens<ProdutosPedidoEspelho>(SqlPedidoEspelho))
        //            {
        //                Utils.CalculaDiferencaCliente(pp);
        //                Atualizar("produtos_pedido_espelho", "idProdPed", pp);
        //            }
        //        }

        //        private int GetCountByPedidoEspelho()
        //        {
        //            return GetCount(SqlPedidoEspelho);
        //        }

        //        #endregion

        //        #region Troca/devolução

        //        private string SqlTrocaDev(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from produto_troca_dev
        //                    where (valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valorVendido) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valorVendido)";
        //        }

        //        private string SqlTrocado(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from produto_trocado
        //                    where (valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valorVendido) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valorVendido)";
        //        }

        //        public void CorrigeTrocaDev()
        //        {
        //            foreach (ProdutoTrocaDevolucao pt in GetItens<ProdutoTrocaDevolucao>(SqlTrocaDev))
        //            {
        //                Utils.CalculaDiferencaCliente(pt);
        //                Atualizar("produto_troca_dev", "idProdTrocaDev", pt);
        //            }

        //            foreach (ProdutoTrocado pt in GetItens<ProdutoTrocado>(SqlTrocado))
        //            {
        //                Utils.CalculaDiferencaCliente(pt);
        //                Atualizar("produto_trocado", "idProdTrocado", pt);
        //            }
        //        }

        //        private int GetCountByTrocaDev()
        //        {
        //            return GetCount(SqlTrocaDev) + GetCount(SqlTrocado);
        //        }

        //        #endregion

        //        #region Material

        //        private string SqlMaterial(bool selecionar)
        //        {
        //            return "select " + (selecionar ? "*" : "count(*)") + @"
        //                    from material_item_projeto
        //                    where (valorDescontoCliente > 0 and valorUnitBruto-valorDescontoCliente=valor) or
        //                        (valorAcrescimoCliente > 0 and valorUnitBruto+valorAcrescimoCliente=valor)";
        //        }

        //        public void CorrigeMaterial()
        //        {
        //            foreach (MaterialItemProjeto mip in GetItens<MaterialItemProjeto>(SqlMaterial))
        //            {
        //                Utils.CalculaDiferencaCliente(mip);
        //                Atualizar("material_item_projeto", "idMaterItemProj", mip);
        //            }
        //        }

        //        private int GetCountByMaterial()
        //        {
        //            return GetCount(SqlMaterial);
        //        }

        //        #endregion
        //    }

        //    public sealed class ValorMovEstoqueDAO : BaseDAO<ValorMovEstoqueDAO.Model, ValorMovEstoqueDAO>
        //    {
        //        public class Model
        //        {
        //        }

        //        public uint[] IdsReal()
        //        {
        //            string ids = GetValoresCampo(@"
        //                    select m.idMovEstoque from mov_estoque m 
        //    	                inner join (
        //    		                select idMovEstoque, idProd, idLoja
        //    		                from (
        //    			                select * from mov_estoque
        //    			                group by idProd, idLoja
        //    			                order by dataMov asc, idMovEstoque asc
        //    		                ) temp
        //    		                where !coalesce(lancManual,false)
        //    	                ) ini on (m.idMovEstoque=ini.idMovEstoque)
        //    	                inner join produto p on (m.idProd=p.idProd)
        //                    where valorMov<>saldoValorMov", "idMovEstoque");

        //            if (String.IsNullOrEmpty(ids))
        //                return new uint[0];

        //            return Array.ConvertAll(ids.Split(','), x => Glass.Conversoes.StrParaUint(x));
        //        }

        //        public uint[] IdsFiscal()
        //        {
        //            string ids = GetValoresCampo(@"
        //                    select m.idMovEstoqueFiscal from mov_estoque_fiscal m 
        //    	                inner join (
        //    		                select idMovEstoqueFiscal, idProd, idLoja
        //    		                from (
        //    			                select * from mov_estoque_fiscal
        //    			                group by idProd, idLoja
        //    			                order by dataMov asc, idMovEstoqueFiscal asc
        //    		                ) temp
        //    		                where !coalesce(lancManual,false)
        //    	                ) ini on (m.idMovEstoqueFiscal=ini.idMovEstoqueFiscal)
        //    	                inner join produto p on (m.idProd=p.idProd)
        //                    where valorMov<>saldoValorMov", "idMovEstoqueFiscal");

        //            if (String.IsNullOrEmpty(ids))
        //                return new uint[0];

        //            return Array.ConvertAll(ids.Split(','), x => Glass.Conversoes.StrParaUint(x));
        //        }

        //        public int Count()
        //        {
        //            uint[] idsReal = IdsReal(), idsFiscal = IdsFiscal();
        //            return idsReal.Length + idsFiscal.Length;
        //        }
        //    }

        //    public sealed class ComissaoPedEspDAO : BaseDAO<ComissaoPedEspDAO.Model, ComissaoPedEspDAO>
        //    {
        //        public class Model
        //        {
        //        }

        //        private string Sql()
        //        {
        //            return @"
        //                    select ppe.idPedido from produtos_pedido_espelho ppe
        //                        inner join pedido p on (ppe.idPedido=p.idPedido)
        //                        inner join pedido_espelho pe on (p.idPedido=pe.idPedido)
        //                    where pe.percComissao>0 and p.situacao not in (5,8)
        //                    group by ppe.idPedido having sum(ppe.valorComissao)>0";
        //        }

        //        public int Count()
        //        {
        //            return objPersistence.ExecuteSqlQueryCount("select count(*) from (" + Sql() + ") as temp");
        //        }

        //        public string GetIdsPedidos()
        //        {
        //            return GetValoresCampo(Sql(), "idPedido");
        //        }
        //    }

        //    public sealed class TempMipDAO : BaseDAO<MaterialItemProjeto, TempMipDAO>
        //    {
        //        public List<MaterialItemProjeto> GetTemp()
        //        {
        //            return objPersistence.LoadData("select * from material_item_projeto where temp=true order by idPecaItemProj");
        //        }
        //    }

        //    public sealed class NumEventoDAO : BaseDAO<NumEventoDAO.NumEventoClass, NumEventoDAO>
        //    {
        //        [PersistenceProperty("num_evento")]
        //        public class NumEventoClass
        //        {
        //            private int _tabela;

        //            [PersistenceProperty("Tabela")]
        //            public int Tabela
        //            {
        //                get { return _tabela; }
        //                set { _tabela = value; }
        //            }

        //            private uint _idRegistro;

        //            [PersistenceProperty("IdRegistro")]
        //            public uint IdRegistro
        //            {
        //                get { return _idRegistro; }
        //                set { _idRegistro = value; }
        //            }

        //            private ulong _numEvento;

        //            [PersistenceProperty("NumEvento")]
        //            public ulong NumEvento
        //            {
        //                get { return _numEvento; }
        //                set { _numEvento = value; }
        //            }
        //        }

        //        public Dictionary<KeyValuePair<int, uint>, uint> GetNumEvento(string idRegistro, string tabela)
        //        {
        //            string sql = "select tabela, " + idRegistro + @" as idRegistro, cast(coalesce(max(numEvento), 0) + 1 as unsigned) as numEvento 
        //                    from " + tabela + " as num_evento group by tabela, " + idRegistro;

        //            Dictionary<KeyValuePair<int, uint>, uint> retorno = new Dictionary<KeyValuePair<int, uint>, uint>();
        //            foreach (NumEventoClass n in objPersistence.LoadData(sql))
        //            {
        //                KeyValuePair<int, uint> chave = new KeyValuePair<int, uint>(n.Tabela, n.IdRegistro);
        //                if (!retorno.ContainsKey(chave))
        //                    retorno.Add(chave, (uint)n.NumEvento);
        //            }

        //            return retorno;
        //        }
        //    }

        //    public sealed class TempLogAltDAO : BaseDAO<LogAlteracao, TempLogAltDAO>
        //    {
        //        public List<LogAlteracao> GetSemReferencia()
        //        {
        //            return objPersistence.LoadData("select * from log_alteracao where coalesce(referencia, '') = ''");
        //        }

        //        public void AtualizaRef(uint idLogAlt, string referencia)
        //        {
        //            referencia = referencia.Length <= 100 ? referencia :
        //                referencia.Substring(0, 97) + "...";

        //            objPersistence.ExecuteCommand("update log_alteracao set referencia=?ref where idLog=" + idLogAlt,
        //                false, new GDAParameter("?ref", referencia));
        //        }

        //        public int GetCountNumEvento()
        //        {
        //            return objPersistence.ExecuteSqlQueryCount("select count(*) from log_alteracao where numEvento is null");
        //        }

        //        public List<LogAlteracao> GetForAtualizacaoNumEvento()
        //        {
        //            string sql = @"select * from log_alteracao where numEvento is null
        //                    order by tabela, idRegistroAlt, dataAlt";

        //            return objPersistence.LoadData(sql);
        //        }

        //        public Dictionary<KeyValuePair<int, uint>, uint> GetNumEvento()
        //        {
        //            return NumEventoDAO.Instance.GetNumEvento("idRegistroAlt", "log_alteracao");
        //        }

        //        public void AtualizaNumEvento(uint idLog, uint numEvento)
        //        {
        //            objPersistence.ExecuteCommand("update log_alteracao set numEvento=" + numEvento +
        //                " where idLog=" + idLog);
        //        }
        //    }

        //    public sealed class TempLogCancDAO : BaseDAO<LogCancelamento, TempLogCancDAO>
        //    {
        //        public List<LogCancelamento> GetSemReferencia()
        //        {
        //            return objPersistence.LoadData("select * from log_cancelamento where coalesce(referencia, '') = ''");
        //        }

        //        public void AtualizaRef(uint idLogCanc, string referencia)
        //        {
        //            referencia = referencia.Length <= 100 ? referencia :
        //                referencia.Substring(0, 97) + "...";

        //            objPersistence.ExecuteCommand("update log_cancelamento set referencia=?ref where idLogCancelamento=" + idLogCanc,
        //                false, new GDAParameter("?ref", referencia));
        //        }

        //        public int GetCountNumEvento()
        //        {
        //            return objPersistence.ExecuteSqlQueryCount("select count(*) from log_cancelamento where numEvento is null");
        //        }

        //        public Dictionary<KeyValuePair<int, uint>, uint> GetNumEvento()
        //        {
        //            return NumEventoDAO.Instance.GetNumEvento("idRegistroCanc", "log_cancelamento");
        //        }

        //        public List<LogCancelamento> GetForAtualizacaoNumEvento()
        //        {
        //            string sql = @"select * from log_cancelamento where numEvento is null
        //                    order by tabela, idRegistroCanc, dataCanc";

        //            return objPersistence.LoadData(sql);
        //        }

        //        public void AtualizaNumEvento(uint idLog, uint numEvento)
        //        {
        //            objPersistence.ExecuteCommand("update log_cancelamento set numEvento=" + numEvento +
        //                " where idLogCancelamento=" + idLog);
        //        }
        //    }

        //    public sealed class GenericDAO : BaseDAO<GenericDAO.Model, GenericDAO>
        //    {
        //        public class Model
        //        {
        //        }

        //        public int ExecutarSql(string sql, params GDAParameter[] p)
        //        {
        //            return objPersistence.ExecuteCommand(sql, false, p);
        //        }

        //        public object ExecuteScalar(string sql, params GDAParameter[] p)
        //        {
        //            return objPersistence.ExecuteScalar(sql, p);
        //        }

        //        public int ExecutarCount(string sql, params GDAParameter[] p)
        //        {
        //            return objPersistence.ExecuteSqlQueryCount(sql, p);
        //        }
        //    }

        //    public sealed class TempPedDAO : BaseDAO<Pedido, TempPedDAO>
        //    {
        //        public List<Pedido> GetForSinal()
        //        {
        //            List<Pedido> retorno = new List<Pedido>();

        //            retorno.AddRange(objPersistence.LoadData(@"select * from pedido where valorEntrada>0 and recebeuSinal=true and idPedido not in (
        //                    select * from (select distinct idPedido from pagto_sinal) as temp)"));

        //            retorno.AddRange(objPersistence.LoadData(@"select * from pedido where valorPagamentoAntecipado>0 and pagamentoAntecipado=true and idPedido not in (
        //                    select * from (select distinct idPedido from pagto_sinal) as temp)"));

        //            return retorno;
        //        }
        //    }

        //    public class TempContasReceberDAO : BaseDAO<ContasReceber, TempContasReceberDAO>
        //    {
        //        public List<ContasReceber> GetCartao()
        //        {
        //            uint idConta = UtilsPlanoConta.GetPlanoSinal((uint)Glass.Data.Model.Pagto.FormaPagto.Cartao);
        //            string sql = "select * from contas_receber where idConta=" + idConta + @" and coalesce(isParcelaCartao,false)=false 
        //                    and ((idPedido is not null and idPedido not in (select * from (select distinct idPedido from contas_receber where isParcelaCartao and idPedido is not null) as temp))
        //                    or (idSinal is not null and idSinal not in (select * from (select distinct idSinal from contas_receber where isParcelaCartao and idSinal is not null) as temp1)))";

        //            return objPersistence.LoadData(sql);
        //        }
        //    }

        //    public class TempPagtoSinalDAO : BaseDAO<PagtoSinal, TempPagtoSinalDAO>
        //    {
        //        private string Sql(uint? idPedido, uint? idSinal, decimal valor)
        //        {
        //            string sql = "select * from pagto_sinal where idSinal";
        //            if (idSinal > 0)
        //                sql += "=" + idSinal;
        //            else if (idPedido > 0)
        //                sql += " in (select idSinal from pedido where idPedido=" + idPedido + ")";
        //            else
        //                sql += "=0";

        //            sql += " and valorPagto=?valor";
        //            return sql;
        //        }

        //        public PagtoSinal GetPagtoSinal(uint? idPedido, uint? idSinal, decimal valor)
        //        {
        //            List<PagtoSinal> item = objPersistence.LoadData(Sql(idPedido, idSinal, valor), new GDAParameter("?valor", valor));
        //            return item.Count > 0 ? item[0] : null;
        //        }
        //    }

        //    public sealed class TempComissaoPedidoDAO : BaseDAO<ComissaoPedido, TempComissaoPedidoDAO>
        //    {
        //        public void UpdateBaseCalc(string valor, uint idComissaoPedido)
        //        {
        //            string sql = "update comissao_pedido set BASECALCCOMISSAO=" + valor + " where IDCOMISSAOPEDIDO=" + idComissaoPedido;
        //            objPersistence.ExecuteCommand(sql);
        //        }
        //    }

        //    public static class ValorBruto
        //    {
        //        private const string sql = @"
        //                select {1} from {0} where totalBruto is null
        //                or valorUnitBruto is null {2} limit 500";

        //        private const string formatoSql = "update {0} set valorUnitBruto={2}, totalBruto={3} where {1}; ";

        //        private class ProdutosOrcamentoDAO : BaseDAO<ProdutosOrcamento, ProdutosOrcamentoDAO>
        //        {
        //            public ProdutosOrcamento[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idProd desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private class ProdutosPedidoDAO : BaseDAO<ProdutosPedido, ProdutosPedidoDAO>
        //        {
        //            public ProdutosPedido[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idProdPed desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private class ProdutosPedidoEspelhoDAO : BaseDAO<ProdutosPedidoEspelho, ProdutosPedidoEspelhoDAO>
        //        {
        //            public ProdutosPedidoEspelho[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idProdPed desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private class ProdutoTrocaDevDAO : BaseDAO<ProdutoTrocaDevolucao, ProdutoTrocaDevDAO>
        //        {
        //            public ProdutoTrocaDevolucao[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idProdTrocaDev desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private class ProdutoTrocadoDAO : BaseDAO<ProdutoTrocado, ProdutoTrocadoDAO>
        //        {
        //            public ProdutoTrocado[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idProdTrocado desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private class MaterialItemProjetoDAO : BaseDAO<MaterialItemProjeto, MaterialItemProjetoDAO>
        //        {
        //            public MaterialItemProjeto[] GetValorBruto()
        //            {
        //                return objPersistence.LoadData(String.Format(sql, objPersistence.TableName, "*", "order by idMaterItemProj desc")).ToArray();
        //            }

        //            public int GetCountValorBruto()
        //            {
        //                return objPersistence.ExecuteSqlQueryCount(String.Format(sql, objPersistence.TableName, "count(*)", ""));
        //            }
        //        }

        //        private static int GetCount()
        //        {
        //            return ProdutosOrcamentoDAO.Instance.GetCountValorBruto() + ProdutosPedidoDAO.Instance.GetCountValorBruto() + ProdutosPedidoEspelhoDAO.Instance.GetCountValorBruto() +
        //                ProdutoTrocaDevDAO.Instance.GetCountValorBruto() + ProdutoTrocadoDAO.Instance.GetCountValorBruto() + MaterialItemProjetoDAO.Instance.GetCountValorBruto();
        //        }

        //        public static string GetDescrCount()
        //        {
        //            int orca = ProdutosOrcamentoDAO.Instance.GetCountValorBruto();
        //            int ped = ProdutosPedidoDAO.Instance.GetCountValorBruto();
        //            int pcp = ProdutosPedidoEspelhoDAO.Instance.GetCountValorBruto();
        //            int troca = ProdutoTrocaDevDAO.Instance.GetCountValorBruto();
        //            int trocado = ProdutoTrocadoDAO.Instance.GetCountValorBruto();
        //            int projeto = MaterialItemProjetoDAO.Instance.GetCountValorBruto();

        //            return orca + " orçamento + " + ped + " pedido + " +
        //                pcp + " pcp + " + troca + " troca + " +
        //                trocado + " trocado + " + projeto + " projeto = " +
        //                (orca + ped + pcp + troca + trocado + projeto);
        //        }

        //        public static bool Atualizando = false;

        //        public static void Atualizar()
        //        {
        //            if (Atualizando)
        //                return;

        //            Atualizando = true;

        //            int sqlCount = 0;
        //            StringBuilder sb = new StringBuilder();

        //            while (ValorBruto.GetCount() > 0)
        //            {
        //                foreach (ProdutosPedidoEspelho ppe in ProdutosPedidoEspelhoDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(ppe);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "produtos_pedido_espelho", "idProdPed=" + ppe.IdProdPed,
        //                        ppe.ValorUnitarioBruto.ToString().Replace(",", "."), ppe.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                foreach (ProdutosPedido pp in ProdutosPedidoDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(pp);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "produtos_pedido", "idProdPed=" + pp.IdProdPed,
        //                        pp.ValorUnitarioBruto.ToString().Replace(",", "."), pp.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                foreach (ProdutosOrcamento po in ProdutosOrcamentoDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(po);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "produtos_orcamento", "idProd=" + po.IdProd,
        //                        po.ValorUnitarioBruto.ToString().Replace(",", "."), po.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                foreach (ProdutoTrocaDevolucao ptd in ProdutoTrocaDevDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(ptd);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "produto_troca_dev", "idProdTrocaDev=" + ptd.IdProdTrocaDev,
        //                        ptd.ValorUnitarioBruto.ToString().Replace(",", "."), ptd.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                foreach (ProdutoTrocado pt in ProdutoTrocadoDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(pt);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "produto_trocado", "idProdTrocado=" + pt.IdProdTrocado,
        //                        pt.ValorUnitarioBruto.ToString().Replace(",", "."), pt.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetValorBruto())
        //                {
        //                    Utils.CalculaValorBruto(mip);
        //                    ExecutarSql(ref sb, ref sqlCount, String.Format(formatoSql, "material_item_projeto", "idMaterItemProj=" + mip.IdMaterItemProj,
        //                        mip.ValorUnitarioBruto.ToString().Replace(",", "."), mip.TotalBruto.ToString().Replace(",", ".")));
        //                }

        //                ExecutarSql(ref sb, ref sqlCount, null);

        //                MetodosAjax.ManterLogado();
        //            }

        //            Atualizando = false;
        //            atualizaValorBruto = null;
        //        }

        //        private static void ExecutarSql(ref StringBuilder sb, ref int sqlCount, string sql)
        //        {
        //            if (sql != null && sqlCount++ < 100)
        //                sb.Append(sql);
        //            else
        //            {
        //                if (sb.Length > 0)
        //                    GenericDAO.Instance.ExecutarSql(sb.ToString());

        //                sb = new StringBuilder();
        //                sqlCount = 0;

        //                if (!String.IsNullOrEmpty(sql))
        //                    sb.Append(sql);

        //                MetodosAjax.ManterLogado();
        //            }
        //        }
        //    }

        //    protected void Button1_Click(object sender, EventArgs e)
        //    {
        //        /* if (txtImpressao.Text == "")
        //            return;

        //        foreach (ProdutoImpressao pi in Glass.Data.DAL.ProdutoImpressaoDAO.Instance.GetListImpressao(txtImpressao.Text))
        //        {
        //            int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(pi.IdPedido.Value, pi.IdProdPed.Value);
        //            int qtde = (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(pi.IdProdPed.Value);

        //            for (int i = pi.PosIniImpressao; i < (pi.QtdImpresso + pi.PosIniImpressao); i++)
        //            {
        //                if (i > qtde)
        //                    break;

        //                string etiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance.GetNumEtiqueta((uint)pi.IdPedido, pos, i, qtde);

        //                string sql = "select count(*) from produto_pedido_producao where coalesce(numEtiqueta, numEtiquetaCanc)=?num";
        //                if (GenericDAO.Instance.ExecutarCount(sql, new GDAParameter("?num", etiqueta)) == 0)
        //                {
        //                    ProdutoImpressao po = Glass.Data.DAL.ProdutoImpressaoDAO.Instance.GetByEtiqueta(etiqueta);
        //                    if (po.IdImpressao != pi.IdImpressao)
        //                        po = null;

        //                    ProdutoPedidoProducaoDAO.Instance.InserePeca(pi.IdImpressao.Value, etiqueta, po != null ? po.PlanoCorte : null, false);
        //                }
        //            }
        //        } */
        //    }

        //    protected void Button2_Click(object sender, EventArgs e)
        //    {
        //        try
        //        {
        //            // Garante que o script ainda não foi executado
        //            GenericDAO.Instance.ExecutarSql("select dadosRegistro from log_cancelamento");

        //            // Apaga os registros que já foram inseridos
        //            GenericDAO.Instance.ExecutarSql("delete from log_cancelamento where campo is not null;");
        //        }
        //        catch
        //        {
        //            return;
        //        }

        //        foreach (LogCancelamento l in LogCancelamentoDAO.Instance.GetAll())
        //        {
        //            List<string> separacoes = new List<string>();
        //            string temp = GenericDAO.Instance.ExecuteScalar("select dadosRegistro from log_cancelamento where idLogCancelamento=" + l.IdLogCancelamento).ToString();

        //            while (temp.IndexOf(", ") > -1)
        //            {
        //                string valor = temp.Substring(0, temp.IndexOf(", "));
        //                separacoes.Add(valor);
        //                temp = temp.Substring(valor.Length + 2);
        //            }

        //            LogCancelamento novo = new LogCancelamento();
        //            novo.Tabela = l.Tabela;
        //            novo.IdRegistroCanc = l.IdRegistroCanc;
        //            novo.IdFuncCanc = l.IdFuncCanc;
        //            novo.DataCanc = l.DataCanc;
        //            novo.CancelamentoManual = l.CancelamentoManual;
        //            novo.Motivo = l.Motivo;

        //            foreach (string s in separacoes)
        //            {
        //                string[] dados = s.Split(':');
        //                if (dados.Length == 1)
        //                    continue;

        //                novo.Campo = dados[0].Trim();
        //                if (novo.Campo.Length > 30)
        //                    novo.Campo = novo.Campo.Substring(0, 27) + "...";
        //                dados[1] = dados[1].Trim();
        //                novo.Valor = String.Join(":", dados, 1, dados.Length - 1);
        //                LogCancelamentoDAO.Instance.Insert(novo);
        //            }
        //        }

        //        GenericDAO.Instance.ExecutarSql(@"update log_cancelamento set campo='Tipo Receb. Parc. Cartão' where campo='Tipo de Recebimento Parc. C...';
        //                delete from log_cancelamento where campo is null;
        //                alter table log_cancelamento drop column DADOSREGISTRO;");
        //    }

        //    protected void Button3_Click(object sender, EventArgs e)
        //    {
        //        foreach (LogAlteracao log in TempLogAltDAO.Instance.GetSemReferencia())
        //            TempLogAltDAO.Instance.AtualizaRef(log.IdLog, LogAlteracao.GetReferencia(log.Tabela, log.IdRegistroAlt));

        //        foreach (LogCancelamento log in TempLogCancDAO.Instance.GetSemReferencia())
        //            TempLogCancDAO.Instance.AtualizaRef(log.IdLogCancelamento, LogCancelamento.GetReferencia(log.Tabela, log.IdRegistroCanc));
        //    }

        //    protected void Button4_Click(object sender, EventArgs e)
        //    {
        //        foreach (Acerto a in AcertoDAO.Instance.GetByCliRpt(0))
        //        {
        //            if (GenericDAO.Instance.ExecutarCount("select count(*) from pagto_acerto where idAcerto=" + a.IdAcerto) > 0)
        //                continue;

        //            // Busca movimentações relacionadas a este acerto e agrupadas pela forma de pagto
        //            CaixaDiario[] lstDiario = CaixaDiarioDAO.Instance.GetByAcerto(a.IdAcerto);
        //            CaixaGeral[] lstGeral = CaixaGeralDAO.Instance.GetByAcerto(a.IdAcerto);
        //            MovBanco[] lstMovBanco = MovBancoDAO.Instance.GetByAcerto(a.IdAcerto);

        //            int numFp = 1;
        //            DateTime menorData = DateTime.Now;
        //            uint idFunc = 0;

        //            List<KeyValuePair<uint, decimal>> inseridos = new List<KeyValuePair<uint, decimal>>();

        //            // Se estiver nas contas bancárias
        //            if (lstMovBanco.Length > 0)
        //                foreach (MovBanco m in lstMovBanco)
        //                {
        //                    KeyValuePair<uint, decimal> dados = new KeyValuePair<uint, decimal>(m.IdConta, m.ValorMov);
        //                    if (inseridos.Contains(dados))
        //                        continue;
        //                    else
        //                        inseridos.Add(dados);

        //                    if (menorData > m.DataCad)
        //                    {
        //                        menorData = m.DataCad;
        //                        idFunc = m.Usucad;
        //                    }

        //                    FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(m.IdConta);
        //                    if (fp == null)
        //                        continue;
        //                    else if (fp.IdFormaPagto == null)
        //                    {
        //                        if (fp.Descricao == "Crédito")
        //                        {
        //                            GenericDAO.Instance.ExecutarSql(@"update acerto set creditoUtilizadoCriar=?c where creditoUtilizadoCriar=null 
        //                                    and idAcerto=" + a.IdAcerto, new GDAParameter("?c", m.ValorMov));
        //                        }

        //                        continue;
        //                    }

        //                    TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(m.IdConta);

        //                    PagtoAcerto pa = new PagtoAcerto();
        //                    pa.IdAcerto = a.IdAcerto;
        //                    pa.IdContaBanco = m.IdContaBanco;
        //                    pa.IdFormaPagto = fp.IdFormaPagto.Value;
        //                    pa.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;
        //                    pa.NumFormaPagto = numFp++;
        //                    pa.ValorPagto = m.ValorMov;
        //                    PagtoAcertoDAO.Instance.Insert(pa);
        //                }

        //            // Se estiver no caixa geral
        //            if (lstGeral.Length > 0)
        //                foreach (CaixaGeral c in lstGeral)
        //                {
        //                    KeyValuePair<uint, decimal> dados = new KeyValuePair<uint, decimal>(c.IdConta, c.ValorMov);
        //                    if (inseridos.Contains(dados))
        //                        continue;
        //                    else
        //                        inseridos.Add(dados);

        //                    if (menorData > c.DataCad)
        //                    {
        //                        menorData = c.DataCad;
        //                        idFunc = c.Usucad;
        //                    }

        //                    FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(c.IdConta);
        //                    if (fp == null)
        //                        continue;
        //                    else if (fp.IdFormaPagto == null)
        //                    {
        //                        if (fp.Descricao == "Crédito")
        //                        {
        //                            GenericDAO.Instance.ExecutarSql(@"update acerto set creditoUtilizadoCriar=?c where creditoUtilizadoCriar=null 
        //                                    and idAcerto=" + a.IdAcerto, new GDAParameter("?c", c.ValorMov));
        //                        }

        //                        continue;
        //                    }

        //                    TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(c.IdConta);

        //                    PagtoAcerto pa = new PagtoAcerto();
        //                    pa.IdAcerto = a.IdAcerto;
        //                    pa.IdFormaPagto = fp.IdFormaPagto.Value;
        //                    pa.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;
        //                    pa.NumFormaPagto = numFp++;
        //                    pa.ValorPagto = c.ValorMov;
        //                    PagtoAcertoDAO.Instance.Insert(pa);
        //                }
        //            // Se estiver no caixa diário
        //            else if (lstDiario.Length > 0)
        //                foreach (CaixaDiario c in lstDiario)
        //                {
        //                    KeyValuePair<uint, decimal> dados = new KeyValuePair<uint, decimal>(c.IdConta, c.Valor);
        //                    if (inseridos.Contains(dados))
        //                        continue;
        //                    else
        //                        inseridos.Add(dados);

        //                    if (menorData > c.DataCad)
        //                    {
        //                        menorData = c.DataCad;
        //                        idFunc = c.Usucad;
        //                    }

        //                    FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(c.IdConta);
        //                    if (fp == null)
        //                        continue;
        //                    else if (fp.IdFormaPagto == null)
        //                    {
        //                        if (fp.Descricao == "Crédito")
        //                        {
        //                            GenericDAO.Instance.ExecutarSql(@"update acerto set creditoUtilizadoCriar=?c where creditoUtilizadoCriar=null 
        //                                    and idAcerto=" + a.IdAcerto, new GDAParameter("?c", c.Valor));
        //                        }

        //                        continue;
        //                    }

        //                    TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(c.IdConta);

        //                    PagtoAcerto pa = new PagtoAcerto();
        //                    pa.IdAcerto = a.IdAcerto;
        //                    pa.IdFormaPagto = fp.IdFormaPagto.Value;
        //                    pa.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;
        //                    pa.NumFormaPagto = numFp++;
        //                    pa.ValorPagto = c.Valor;
        //                    PagtoAcertoDAO.Instance.Insert(pa);
        //                }

        //            GenericDAO.Instance.ExecutarSql("update acerto set dataCad=?data, usuCad=?usu where idAcerto=" + a.IdAcerto,
        //                new GDAParameter("?data", menorData), new GDAParameter("?usu", idFunc > 0 ? (int?)idFunc : null));
        //        }

        //        GenericDAO.Instance.ExecutarSql(@"
        //                delete from acerto where idAcerto not in (select * from (
        //                    select distinct coalesce(idAcerto, idAcertoParcial) from contas_receber) as temp);
        //    
        //                update acerto a 
        //    	            left join (
        //        	            select coalesce(idAcerto, idAcertoParcial) as idAcerto, min(dataCad) as data, usuRec
        //        	            from contas_receber
        //                        where coalesce(idAcerto, idAcertoParcial) is not null
        //                        group by coalesce(idAcerto, idAcertoParcial)
        //                    ) as c on (a.idAcerto=c.idAcerto)
        //                set situacao=1, dataCad=c.data, usuCad=c.usuRec 
        //                where situacao is null or usuCad is null or usuCad=0;");
        //    }

        //    protected void Button5_Click(object sender, EventArgs e)
        //    {
        //        foreach (Pedido p in TempPedDAO.Instance.GetForSinal())
        //        {
        //            CaixaGeral[] lstGeral = CaixaGeralDAO.Instance.GetBySinal(p.IdPedido);
        //            CaixaDiario[] lstDiario = CaixaDiarioDAO.Instance.GetBySinal(p.IdPedido);
        //            MovBanco[] lstMov = MovBancoDAO.Instance.GetBySinal(p.IdSinal != null ? p.IdSinal.Value : 0);

        //            int numFormaPagto = 1;
        //            if (lstDiario.Length > 0)
        //            {
        //                foreach (CaixaDiario d in lstDiario)
        //                {
        //                    FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(d.IdConta);
        //                    if (fp == null || fp.IdFormaPagto == null)
        //                        continue;

        //                    TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(d.IdConta);

        //                    PagtoSinal pagto = new PagtoSinal();
        //                    pagto.NumFormaPagto = numFormaPagto++;
        //                    pagto.ValorPagto = d.Valor;
        //                    pagto.IdFormaPagto = fp.IdFormaPagto.Value;
        //                    pagto.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;

        //                    PagtoSinalDAO.Instance.Insert(pagto);
        //                }
        //            }
        //            else
        //            {
        //                foreach (CaixaGeral g in lstGeral)
        //                {
        //                    FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(g.IdConta);
        //                    if (fp == null || fp.IdFormaPagto == null)
        //                        continue;

        //                    TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(g.IdConta);

        //                    PagtoSinal pagto = new PagtoSinal();
        //                    pagto.NumFormaPagto = numFormaPagto++;
        //                    pagto.ValorPagto = g.ValorMov;
        //                    pagto.IdFormaPagto = fp.IdFormaPagto.Value;
        //                    pagto.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;

        //                    PagtoSinalDAO.Instance.Insert(pagto);
        //                }
        //            }

        //            foreach (MovBanco m in lstMov)
        //            {
        //                FormaPagto fp = UtilsPlanoConta.GetFormaPagtoByIdConta(m.IdConta);
        //                if (fp == null || fp.IdFormaPagto == null)
        //                    continue;

        //                TipoCartaoCredito tcc = UtilsPlanoConta.GetTipoCartaoByIdConta(m.IdConta);

        //                PagtoSinal pagto = new PagtoSinal();
        //                pagto.NumFormaPagto = numFormaPagto++;
        //                pagto.ValorPagto = m.ValorMov;
        //                pagto.IdFormaPagto = fp.IdFormaPagto.Value;
        //                pagto.IdTipoCartao = tcc != null ? (uint?)tcc.IdTipoCartao : null;
        //                pagto.IdContaBanco = m.IdContaBanco;

        //                PagtoSinalDAO.Instance.Insert(pagto);
        //            }
        //        }
        //    }

        //    protected void Button6_Click(object sender, EventArgs e)
        //    {
        //        List<uint> idsContasR = new List<uint>();

        //        foreach (ContasReceber c in TempContasReceberDAO.Instance.GetCartao())
        //        {
        //            PagtoSinal p = TempPagtoSinalDAO.Instance.GetPagtoSinal(c.IdPedido, c.IdSinal, c.ValorVec);
        //            if (p == null || p.IdTipoCartao == null)
        //                continue;

        //            TipoCartaoCredito tpCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(p.IdTipoCartao.Value);
        //            if (tpCartao.Descricao.ToLower().Contains("débito") && !FinanceiroConfig.Cartao.QuitarParcCartaoDebito)
        //                continue;

        //            Pedido ped = c.IdPedido > 0 ? PedidoDAO.Instance.GetElementByPrimaryKey(c.IdPedido.Value) : null;
        //            Loja l = LojaDAO.Instance.GetAll()[0];

        //            AssocContaBanco a = p.IdContaBanco > 0 ? null :
        //                AssocContaBancoDAO.Instance.GetContaBancoCartao(p.IdTipoCartao.Value, ped != null ? ped.IdLoja : l.IdLoja);

        //            if (a != null && a.IdContaBanco == 0)
        //                continue;

        //            ContasReceber c1 = new ContasReceber();
        //            c1.IdPedido = c.IdPedido;
        //            c1.IdSinal = c.IdSinal;
        //            c1.DataCad = c.DataCad;
        //            c1.DataVec = c.DataVec.AddDays(30);
        //            c1.IdCliente = c.IdCliente;
        //            c1.ValorVec = UtilsFinanceiro.GetValorParcela(ped.IdLoja, c.ValorVec, (int)Glass.Data.Model.Pagto.FormaPagto.Cartao, p.IdTipoCartao.Value, 1);
        //            c1.ValorJurosCartao = UtilsFinanceiro.GetValorJurosParc(ped.IdLoja, c.ValorVec, (int)Glass.Data.Model.Pagto.FormaPagto.Cartao, p.IdTipoCartao.Value, 1, 1);
        //            c1.NumParc = c.NumParc;
        //            c1.NumParcMax = c.NumParcMax;
        //            c1.NumeroNFe = c.NumeroNFe;
        //            c1.NumAutConstrucard = c.NumAutConstrucard;
        //            c1.Multa = c.Multa;
        //            c1.Juros = c.Juros;
        //            c1.IdContaBanco = p.IdContaBanco > 0 ? p.IdContaBanco.Value : a.IdContaBanco;
        //            c1.IdConta = UtilsPlanoConta.GetPlanoSinalTipoCartao(p.IdTipoCartao.Value);
        //            c1.IsParcelaCartao = true;

        //            idsContasR.Add(ContasReceberDAO.Instance.Insert(c1));
        //        }
        //    }

        //    protected void Button7_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaValorBruto == null)
        //        {
        //            atualizaValorBruto = new Thread(ValorBruto.Atualizar);
        //            atualizaValorBruto.Start();
        //        }
        //    }

        //    protected void btnSalvar_Click(object sender, EventArgs e)
        //    {
        //        ArquivoMesaCorte arquivo = new ArquivoMesaCorte();
        //        arquivo.Codigo = txtCodArqMesaCorte.Text;
        //        arquivo.Arquivo = txtArquivoMesaCorte.Text;
        //        ArquivoMesaCorteDAO.Instance.Insert(arquivo);
        //    }

        //    protected void Button8_Click(object sender, EventArgs e)
        //    {
        //        if (atualizarNumParc == null)
        //        {
        //            atualizarNumParc = new Thread(new ThreadStart(
        //                delegate()
        //                {
        //                    object atual = null;

        //                    do
        //                    {
        //                        object idContaPg = GenericDAO.Instance.ExecuteScalar("select idContaPg from contas_pagar where coalesce(numParc,0)=0 or coalesce(numParcMax,0)=0 limit 1");
        //                        if (idContaPg == null || idContaPg == DBNull.Value || idContaPg.ToString() == "")
        //                            break;

        //                        if (idContaPg.Equals(atual))
        //                            throw new Exception("Erro ao atualizar a conta a pagar número " + idContaPg.ToString());

        //                        atual = idContaPg;
        //                        ContasPagarDAO.Instance.AtualizaNumParcContaPg(Glass.Conversoes.StrParaUint(idContaPg.ToString()));
        //                    }
        //                    while (true);

        //                    atualizarNumParc = null;
        //                }
        //            ));

        //            atualizarNumParc.Start();
        //        }
        //    }

        //    protected void Button9_Click(object sender, EventArgs e)
        //    {
        //        uint idPedido;
        //        if (!uint.TryParse(txtPedido.Text, out idPedido) || !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido))
        //            return;

        //        PedidoEspelho pe = PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(idPedido);
        //        PedidoEspelhoDAO.Instance.RemoveDesconto(idPedido);
        //        PedidoEspelhoDAO.Instance.AplicaDesconto(idPedido, pe.TipoDesconto, pe.Desconto);
        //    }

        //    protected void Button10_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaNumEvento != null)
        //            return;

        //        atualizaNumEvento = new Thread(
        //            delegate()
        //            {
        //                try
        //                {
        //                    const string FORMATO_DATA = "dd/MM/yyyy HH:mm";

        //                    int tabela = 0;
        //                    uint numEvento = 0, idRegistro = 0;
        //                    DateTime data = new DateTime();
        //                    Dictionary<KeyValuePair<int, uint>, uint> numEventoIni;

        //                    List<LogAlteracao> aLA = TempLogAltDAO.Instance.GetForAtualizacaoNumEvento();
        //                    if (aLA.Count > 0)
        //                    {
        //                        numEventoIni = TempLogAltDAO.Instance.GetNumEvento();

        //                        foreach (LogAlteracao la in aLA)
        //                        {
        //                            if (la.Tabela != tabela || la.IdRegistroAlt != idRegistro)
        //                            {
        //                                KeyValuePair<int, uint> chave = new KeyValuePair<int, uint>(la.Tabela, la.IdRegistroAlt);
        //                                numEvento = numEventoIni.ContainsKey(chave) ? numEventoIni[chave] : 1;
        //                                tabela = la.Tabela;
        //                                idRegistro = la.IdRegistroAlt;
        //                                data = la.DataAlt;
        //                            }
        //                            else if (la.DataAlt.ToString(FORMATO_DATA) != data.ToString(FORMATO_DATA))
        //                            {
        //                                numEvento++;
        //                                data = la.DataAlt;
        //                            }

        //                            TempLogAltDAO.Instance.AtualizaNumEvento(la.IdLog, numEvento);
        //                        }

        //                        tabela = 0;
        //                        numEvento = 0;
        //                        idRegistro = 0;
        //                        data = new DateTime();
        //                    }

        //                    List<LogCancelamento> aLC = TempLogCancDAO.Instance.GetForAtualizacaoNumEvento();
        //                    if (aLC.Count > 0)
        //                    {
        //                        numEventoIni = TempLogCancDAO.Instance.GetNumEvento();

        //                        foreach (LogCancelamento lc in aLC)
        //                        {
        //                            if (lc.Tabela != tabela || lc.IdRegistroCanc != idRegistro)
        //                            {
        //                                KeyValuePair<int, uint> chave = new KeyValuePair<int, uint>(lc.Tabela, lc.IdRegistroCanc);
        //                                numEvento = numEventoIni.ContainsKey(chave) ? numEventoIni[chave] : 1;
        //                                tabela = lc.Tabela;
        //                                idRegistro = lc.IdRegistroCanc;
        //                                data = lc.DataCanc;
        //                            }
        //                            else if (lc.DataCanc.ToString(FORMATO_DATA) != data.ToString(FORMATO_DATA))
        //                            {
        //                                numEvento++;
        //                                data = lc.DataCanc;
        //                            }

        //                            TempLogCancDAO.Instance.AtualizaNumEvento(lc.IdLogCancelamento, numEvento);
        //                        }
        //                    }

        //                    atualizaNumEvento = null;
        //                }
        //                catch
        //                {
        //                    if (atualizaNumEvento != null)
        //                    {
        //                        atualizaNumEvento = null;
        //                        Button10_Click(sender, e);
        //                    }
        //                }
        //            }
        //        );

        //        atualizaNumEvento.Start();
        //    }

        //    protected void Button11_Click(object sender, EventArgs e)
        //    {
        //        GenericDAO.Instance.ExecutarSql(@"update produto_orcamento_benef pob
        //                inner join produtos_orcamento po on (pob.idProd=po.idProd)
        //                inner join orcamento o on (po.idOrcamento=o.idOrcamento)
        //                set pob.valorComissao=pob.valor*o.percComissao/100
        //                where o.percComissao>0 and coalesce(po.valorComissao,0)=0 and po.idProdParent is not null;
        //    
        //                update produtos_orcamento po
        //                inner join orcamento o on (po.idOrcamento=o.idOrcamento)
        //                set po.valorComissao=po.total*o.percComissao/100
        //                where o.percComissao>0 and coalesce(po.valorComissao,0)=0 and po.idProdParent is not null");

        //        GenericDAO.Instance.ExecutarSql(@"update produto_pedido_benef ppb
        //                inner join produtos_pedido pp on (ppb.idProdPed=pp.idProdPed)
        //                inner join pedido p on (pp.idPedido=p.idPedido)
        //                set ppb.valorComissao=ppb.valor*p.percComissao/100
        //                where p.percComissao>0 and coalesce(pp.valorComissao,0)=0;
        //    
        //                update produtos_pedido pp
        //                inner join pedido p on (pp.idPedido=p.idPedido)
        //                set pp.valorComissao=pp.total*p.percComissao/100
        //                where p.percComissao>0 and coalesce(pp.valorComissao,0)=0");

        //        GenericDAO.Instance.ExecutarSql(@"update produto_pedido_espelho_benef ppeb
        //                inner join produtos_pedido_espelho ppe on (ppeb.idProdPed=ppe.idProdPed)
        //                inner join pedido pe on (ppe.idPedido=pe.idPedido)
        //                set ppeb.valorComissao=ppeb.valor*pe.percComissao/100
        //                where pe.percComissao>0 and coalesce(ppe.valorComissao,0)=0;
        //    
        //                update produtos_pedido_espelho ppe
        //                inner join pedido pe on (ppe.idPedido=pe.idPedido)
        //                set ppe.valorComissao=ppe.total*pe.percComissao/100
        //                where pe.percComissao>0 and coalesce(ppe.valorComissao,0)=0");
        //    }

        //    protected void Button12_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaComissaoPedEsp != null)
        //            return;

        //        string idsPedidos = ComissaoPedEspDAO.Instance.GetIdsPedidos();

        //        foreach (string id in idsPedidos.Split(','))
        //        {
        //            uint idPed = Glass.Conversoes.StrParaUint(id);
        //            PedidoEspelhoDAO.Instance.RemoveComissao(idPed);
        //            PedidoEspelhoDAO.Instance.AplicaComissao(idPed, PedidoEspelhoDAO.Instance.RecuperaPercComissao(idPed));
        //        }

        //        atualizaComissaoPedEsp = null;
        //    }

        //    protected void Button13_Click(object sender, EventArgs e)
        //    {
        //        GenericDAO.Instance.ExecutarSql(@"update produtos_pedido pp
        //                inner join produto p on (pp.idProd=p.idProd)
        //                set pp.custoProd=round(p.custoCompra*(pp.valorUnitBruto-pp.valorDescontoCliente+pp.valorAcrescimoCliente)/pp.totalBruto, 2)
        //                where coalesce(pp.custoProd,0)=0");
        //    }

        //    protected void Button14_Click(object sender, EventArgs e)
        //    {
        //        string[] nfe = Directory.GetFiles(Utils.GetNfeXmlPath);

        //        foreach (string n in nfe)
        //        {
        //            bool mudou = false;
        //            string chaveAcesso = n.Substring(Utils.GetNfeXmlPath.Length).Replace("-nfe.xml", "");

        //            NotaFiscal nota;
        //            try
        //            {
        //                nota = NotaFiscalDAO.Instance.GetByChaveAcesso(chaveAcesso);
        //                if (nota == null || nota.TipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
        //                    continue;
        //            }
        //            catch
        //            {
        //                continue;
        //            }

        //            ProdutosNf[] lstProdNf = ProdutosNfDAO.Instance.GetByNfExtended(nota.IdNf);

        //            XmlDocument x = new XmlDocument();
        //            x.Load(n);

        //            XmlNodeList det = x["NFe"].GetElementsByTagName("det");
        //            foreach (XmlNode node in det)
        //            {
        //                int item = Glass.Conversoes.StrParaInt(node.Attributes["nItem"].Value) - 1;

        //                int? cstIpi = null, cstPis = null, cstCofins = null;
        //                float aliqPis = 0, aliqCofins = 0;
        //                decimal bcPis = 0, bcCofins = 0, valorPis = 0, valorCofins = 0;

        //                if (node["imposto"] != null && node["imposto"].ChildNodes.Count > 0)
        //                {
        //                    XmlElement imposto = node["imposto"];

        //                    try
        //                    {
        //                        if (imposto["IPI"] != null && imposto["IPI"].ChildNodes.Count > 0)
        //                        {
        //                            cstIpi = Glass.Conversoes.StrParaIntNullable(imposto["IPI"]["IPITrib"]["CST"].InnerText);

        //                            mudou = true;
        //                        }
        //                    }
        //                    catch { }

        //                    try
        //                    {
        //                        if (imposto["PIS"] != null && imposto["PIS"].ChildNodes.Count > 0)
        //                        {
        //                            cstPis = Glass.Conversoes.StrParaIntNullable(imposto["PIS"]["PISAliq"]["CST"].InnerText);
        //                            aliqPis = Glass.Conversoes.StrParaFloat(imposto["PIS"]["PISAliq"]["pPIS"].InnerText);
        //                            bcPis = Glass.Conversoes.StrParaDecimal(imposto["PIS"]["PISAliq"]["vBC"].InnerText);
        //                            valorPis = Glass.Conversoes.StrParaDecimal(imposto["PIS"]["PISAliq"]["vPIS"].InnerText);

        //                            mudou = true;
        //                        }
        //                    }
        //                    catch { }

        //                    try
        //                    {
        //                        if (imposto["COFINS"] != null && imposto["COFINS"].ChildNodes.Count > 0)
        //                        {
        //                            cstCofins = Glass.Conversoes.StrParaIntNullable(imposto["COFINS"]["COFINSAliq"]["CST"].InnerText);
        //                            aliqCofins = Glass.Conversoes.StrParaFloat(imposto["COFINS"]["COFINSAliq"]["pCOFINS"].InnerText);
        //                            bcCofins = Glass.Conversoes.StrParaDecimal(imposto["COFINS"]["COFINSAliq"]["vBC"].InnerText);
        //                            valorCofins = Glass.Conversoes.StrParaDecimal(imposto["COFINS"]["COFINSAliq"]["vCOFINS"].InnerText);

        //                            mudou = true;
        //                        }
        //                    }
        //                    catch { }
        //                }

        //                if (item < lstProdNf.Length)
        //                    GenericDAO.Instance.ExecutarSql(@"update produtos_nf set cstIpi=?ci, cstPis=?cp, aliqPis=?ap, valorPis=?vp,
        //                            cstCofins=?cc, aliqCofins=?ac, valorCofins=?vc, bcPis=?bp, bcCofins=?bc where idProdNf=" + lstProdNf[item].IdProdNf,
        //                        new GDAParameter("?ci", cstIpi), new GDAParameter("?cp", cstPis), new GDAParameter("?ap", aliqPis),
        //                        new GDAParameter("?vp", valorPis), new GDAParameter("?cc", cstCofins), new GDAParameter("?ac", aliqCofins),
        //                        new GDAParameter("?vc", valorCofins), new GDAParameter("?bp", bcPis), new GDAParameter("?bc", bcCofins));
        //            }

        //            if (mudou)
        //            {
        //                GenericDAO.Instance.ExecutarSql(@"update nota_fiscal n set
        //                        ValorPis=Round((select sum(valorPis) from produtos_nf where valorPis>0 and idNf=n.idNf), 2),
        //                        AliqPis=ValorPis/(select sum(bcPis) from produtos_nf where valorPis>0 and idNf=n.idNf)*100, 
        //                        ValorCofins=Round((select sum(valorCofins) from produtos_nf where valorCofins>0 and idNf=n.idNf), 2),
        //                        AliqCofins=ValorCofins/(select sum(bcCofins) from produtos_nf where valorCofins>0 and idNf=n.idNf)*100
        //                        where idNf=" + nota.IdNf);
        //            }
        //        }
        //    }

        //    protected void Button15_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaMovEstoque != null)
        //            return;

        //        atualizaMovEstoque = new Thread(
        //            delegate()
        //            {
        //                Utils.CorrigeMovEstoque();
        //                atualizaMovEstoque = null;
        //            }
        //        );

        //        atualizaMovEstoque.Start();
        //    }

        //    protected void txtPreencheBaseCalcComissao_Click(object sender, EventArgs e)
        //    {
        //        try
        //        {
        //            ComissaoPedido[] comissaoPedidos = ComissaoPedidoDAO.Instance.GetAll();

        //            foreach (ComissaoPedido cp in comissaoPedidos)
        //            {
        //                if (PedidoConfig.LiberarPedido)
        //                {
        //                    float perComissao = 0;

        //                    Comissao comissao = ComissaoDAO.Instance.GetElement(cp.IdComissao);

        //                    if (comissao.IdComissionado.HasValue)
        //                        perComissao = PedidoDAO.Instance.ObtemPercComissao(cp.IdPedido);
        //                    else if (comissao.IdFunc.HasValue)
        //                        perComissao = ComissaoConfigDAO.Instance.GetComissaoConfig(comissao.IdFunc.Value).PercFaixaUm;
        //                    else if (comissao.IdInstalador.HasValue)
        //                        perComissao = ComissaoConfigDAO.Instance.GetComissaoConfig(comissao.IdInstalador.Value).PercFaixaUm;

        //                    if (perComissao > 0)
        //                    {
        //                        string baseCalcComissao = ((cp.Valor * 100) / Convert.ToDecimal(perComissao)).ToString().Replace(",", ".");
        //                        TempComissaoPedidoDAO.Instance.UpdateBaseCalc(baseCalcComissao, cp.IdComissaoPedido);
        //                    }
        //                }
        //                else
        //                {
        //                    Pedido pedido = PedidoDAO.Instance.GetElement(cp.IdPedido);
        //                    string totalPedido = pedido.TotalParaComissao.ToString().Replace(",", ".");
        //                    TempComissaoPedidoDAO.Instance.UpdateBaseCalc(totalPedido, cp.IdComissaoPedido);
        //                }

        //            }
        //            Glass.MensagemAlerta.ShowMsg("Base calculo da comissão preenchida.", this);
        //        }
        //        catch (Exception ex)
        //        {
        //            Glass.MensagemAlerta.ErrorMsg("Falha ao preencher base calculo de comissao", ex, this);
        //        }
        //    }

        //    public sealed class tempLogAlteracaoDAO : BaseDAO<LogAlteracao, tempLogAlteracaoDAO>
        //    {
        //        public LogAlteracao[] GetLogIcmsIpiCliente(LogAlteracao.TabelaAlteracao tabela, uint idRegistroAlt)
        //        {
        //            string sql = @"
        //                                SELECT *
        //                                FROM log_alteracao l
        //                                WHERE l.tabela = " + (int)tabela + @"
        //                                  AND l.idRegistroAlt = " + idRegistroAlt + @"
        //                                  AND (l.campo = 'Cobrar IPI' OR l.campo = 'Cobrar ICMS ST')
        //                                ORDER BY l.dataAlt";

        //            return objPersistence.LoadData(sql).ToArray();
        //        }
        //    }

        //    public sealed class tempProdutosLiberarPedidoDAO : BaseDAO<ProdutosLiberarPedido, tempProdutosLiberarPedidoDAO>
        //    {
        //        private static uint idLoja = 0;
        //        private static uint idCli = 0;
        //        private static ConfiguracaoLoja configCalcIcmsLiberacao = null;
        //        private static ConfiguracaoLoja configCalcIpiLiberacao = null;
        //        private static ConfiguracaoLoja configCalcIcmsPedido = null;
        //        private static ConfiguracaoLoja configCalcIpiPedido = null;
        //        private static LogAlteracao[] logIcmsLiberacao = null;
        //        private static LogAlteracao[] logIpiLiberacao = null;
        //        private static LogAlteracao[] logIcmsPedido = null;
        //        private static LogAlteracao[] logIpiPedido = null;
        //        private static LogAlteracao[] logCliente = null;

        //        public void GerarIcmsIpi()
        //        {
        //            List<LiberarPedido> libPed = new List<LiberarPedido>(LiberarPedidoDAO.Instance.GetAll());


        //            libPed.Sort(delegate(LiberarPedido a, LiberarPedido b)
        //            {
        //                return a.IdCliente.CompareTo(b.IdCliente);
        //            });

        //            LiberarPedido[] lstLiberarPedido = libPed.ToArray();

        //            for (int i = 0; i < lstLiberarPedido.Length; i++)
        //            {
        //                if (lstLiberarPedido[i].IdCliente == 0)
        //                    continue;

        //                uint idLojaTemp = FuncionarioDAO.Instance.ObtemIdLoja(lstLiberarPedido[i].IdFunc);
        //                if (idLoja != idLojaTemp)
        //                {
        //                    idLoja = idLojaTemp;
        //                    configCalcIcmsLiberacao = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.CalcularIcmsLiberacao, idLoja);
        //                    configCalcIpiLiberacao = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.CalcularIpiLiberacao, idLoja);
        //                    configCalcIcmsPedido = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.CalcularIcmsPedido, idLoja);
        //                    configCalcIpiPedido = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.CalcularIpiPedido, idLoja);

        //                    logIcmsLiberacao = LogAlteracaoDAO.Instance.
        //                        GetByItem(LogAlteracao.TabelaAlteracao.ConfigLoja, configCalcIcmsLiberacao.IdConfigLoja, null, null, false);
        //                    logIpiLiberacao = LogAlteracaoDAO.Instance.
        //                        GetByItem(LogAlteracao.TabelaAlteracao.ConfigLoja, configCalcIpiLiberacao.IdConfigLoja, null, null, false);
        //                    logIcmsPedido = LogAlteracaoDAO.Instance.
        //                        GetByItem(LogAlteracao.TabelaAlteracao.ConfigLoja, configCalcIcmsPedido.IdConfigLoja, null, null, false);
        //                    logIpiPedido = LogAlteracaoDAO.Instance.
        //                        GetByItem(LogAlteracao.TabelaAlteracao.ConfigLoja, configCalcIpiPedido.IdConfigLoja, null, null, false);
        //                }

        //                if (lstLiberarPedido[i].IdCliente != idCli)
        //                {
        //                    idCli = lstLiberarPedido[i].IdCliente;
        //                    logCliente = tempLogAlteracaoDAO.Instance
        //                        .GetLogIcmsIpiCliente(LogAlteracao.TabelaAlteracao.Cliente, lstLiberarPedido[i].IdCliente);
        //                }

        //                bool calcIcmsLib = false;
        //                bool calcIpiLib = false;
        //                bool calcIcmsPed = false;
        //                bool calcIpiPed = false;

        //                if (logIcmsLiberacao.Length == 0)
        //                    calcIcmsLib = configCalcIcmsLiberacao.ValorBooleano;
        //                else
        //                    for (int j = logIcmsLiberacao.Length - 1; j >= 0; j--)
        //                    {
        //                        if (logIcmsLiberacao[j].DataAlt <= lstLiberarPedido[i].DataLiberacao)
        //                            calcIcmsLib = (logIcmsLiberacao[j].ValorAtual.Equals("Sim") ? true : false);
        //                        else
        //                            break;
        //                    }


        //                if (logIpiLiberacao.Length == 0)
        //                    calcIpiLib = configCalcIpiLiberacao.ValorBooleano;
        //                else
        //                    for (int k = logIpiLiberacao.Length - 1; k >= 0; k--)
        //                    {
        //                        if (logIpiLiberacao[k].DataAlt <= lstLiberarPedido[i].DataLiberacao)
        //                            calcIpiLib = (logIpiLiberacao[k].ValorAtual.Equals("Sim") ? true : false);
        //                        else
        //                            break;
        //                    }

        //                if (logIcmsPedido.Length == 0)
        //                    calcIcmsPed = configCalcIcmsPedido.ValorBooleano;
        //                else
        //                    for (int x = logIcmsPedido.Length - 1; x >= 0; x--)
        //                    {
        //                        if (logIcmsPedido[x].DataAlt <= lstLiberarPedido[i].DataLiberacao)
        //                            calcIcmsPed = (logIcmsPedido[x].ValorAtual.Equals("Sim") ? true : false);
        //                        else
        //                            break;
        //                    }

        //                if (logIpiPedido.Length == 0)
        //                    calcIpiPed = configCalcIpiPedido.ValorBooleano;
        //                else
        //                    for (int z = logIpiPedido.Length - 1; z >= 0; z--)
        //                    {
        //                        if (logIpiPedido[z].DataAlt <= lstLiberarPedido[i].DataLiberacao)
        //                            calcIpiPed = (logIpiPedido[z].ValorAtual.Equals("Sim") ? true : false);
        //                        else
        //                            break;
        //                    }

        //                bool calcClienteIcms = false;
        //                bool calcClienteIpi = false;

        //                bool temLogCliIpi = false;
        //                bool temLogCliIcms = false;
        //                for (int y = 0; y < logCliente.Length; y++)
        //                {
        //                    if (logCliente[y].DataAlt <= lstLiberarPedido[i].DataLiberacao)
        //                    {
        //                        if (logCliente[y].Campo.Equals("Cobrar IPI"))
        //                        {
        //                            calcClienteIpi = (logCliente[y].ValorAtual.Equals("Sim") ? true : false);
        //                            temLogCliIpi = true;
        //                        }
        //                        else
        //                        {
        //                            calcClienteIcms = (logCliente[y].ValorAtual.Equals("Sim") ? true : false);
        //                            temLogCliIcms = true;
        //                        }
        //                    }
        //                    else
        //                        break;
        //                }

        //                if (!temLogCliIcms)
        //                    calcClienteIcms = ClienteDAO.Instance.IsCobrarIcmsSt(lstLiberarPedido[i].IdCliente);
        //                if (!temLogCliIpi)
        //                    calcClienteIpi = ClienteDAO.Instance.IsCobrarIpi(lstLiberarPedido[i].IdCliente);

        //                string sql = @"
        //                                UPDATE produtos_liberar_pedido plp 
        //                                SET 
        //                                    plp.ValorIcms = " + (calcIcmsLib && (calcClienteIcms || calcIcmsPed) ?
        //                                   @"(SELECT 
        //                                            ((coalesce(pp.ValorIcms, 0) / pp.Qtde) * plp.Qtde)
        //                                        FROM
        //                                            produtos_pedido pp
        //                                        WHERE
        //                                            pp.IdProdPed = plp.IdProdPed)" : "0") + @",
        //                                    plp.ValorIpi = " + (calcIpiLib && (calcClienteIpi || calcIpiPed) ? @"(SELECT 
        //                                            ((coalesce(pp.ValorIpi, 0) / pp.Qtde) * plp.Qtde)
        //                                        FROM
        //                                            produtos_pedido pp
        //                                        WHERE
        //                                            pp.IdProdPed = plp.IdProdPed)" : "0") + @"
        //                                WHERE
        //                                    plp.idliberarpedido = " + lstLiberarPedido[i].IdLiberarPedido;

        //                objPersistence.ExecuteCommand(sql);

        //            }
        //        }
        //    }

        //    protected void txtPreencheICMSIPIProdutosLiberarPedido_Click(object sender, EventArgs e)
        //    {
        //        tempProdutosLiberarPedidoDAO.Instance.GerarIcmsIpi();
        //        Glass.MensagemAlerta.ShowMsg("Concluido.", this);
        //    }

        //    public sealed class tempPedidoDAO : BaseDAO<ProdutosPedido, tempPedidoDAO>
        //    {
        //        public void AtualizarCusto()
        //        {
        //            ProdutosPedido[] lstProdutosPedido = ProdutosPedidoDAO.Instance.GetAll();
        //            Dictionary<uint, decimal> dadosPedidosExecutar = new Dictionary<uint, decimal>();

        //            for (int i = 0; i < lstProdutosPedido.Length; i++)
        //            {
        //                if (lstProdutosPedido[i].IdPedido == 0 || lstProdutosPedido[i].IdPedido == null || lstProdutosPedido[i].InvisivelPedido)
        //                    continue;

        //                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(lstProdutosPedido[i].IdPedido);

        //                if (idCliente == 0)
        //                    continue;

        //                decimal custoProdTemp = 0, totalTemp = 0;
        //                float alturaTemp = lstProdutosPedido[i].Altura, totM2Temp = lstProdutosPedido[i].TotM;

        //                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente, lstProdutosPedido[i].IdProd, lstProdutosPedido[i].Largura, lstProdutosPedido[i].Qtde,
        //                    lstProdutosPedido[i].QtdeAmbiente, lstProdutosPedido[i].ValorVendido, lstProdutosPedido[i].Espessura, lstProdutosPedido[i].Redondo,
        //                    2, false, ref custoProdTemp, ref alturaTemp, ref totM2Temp, ref totalTemp, false, lstProdutosPedido[i].Beneficiamentos.CountAreaMinima);

        //                objPersistence.ExecuteCommand("Update produtos_pedido Set custoProd=" + custoProdTemp.ToString().Replace(",", ".") +
        //                    " Where idProdPed=" + lstProdutosPedido[i].IdProdPed);

        //                if (!dadosPedidosExecutar.ContainsKey(lstProdutosPedido[i].IdPedido))
        //                    dadosPedidosExecutar.Add(lstProdutosPedido[i].IdPedido, 0);

        //                dadosPedidosExecutar[lstProdutosPedido[i].IdPedido] += custoProdTemp;
        //            }

        //            foreach (uint idPedido in dadosPedidosExecutar.Keys)
        //                objPersistence.ExecuteCommand("Update pedido Set custoPedido=" + dadosPedidosExecutar[idPedido].ToString().Replace(",", ".") +
        //                    " Where idPedido=" + idPedido);
        //        }
        //    }

        //    public sealed class tempOrcamentoDAO : BaseDAO<ProdutosOrcamento, tempOrcamentoDAO>
        //    {
        //        public void AtualizarCusto()
        //        {
        //            ProdutosOrcamento[] lstProdutosOrcamento = ProdutosOrcamentoDAO.Instance.GetAll();
        //            Dictionary<uint, decimal> dadosOrcamentosExecutar = new Dictionary<uint, decimal>();
        //            List<uint> lstIdProd = new List<uint>();

        //            for (int i = 0; i < lstProdutosOrcamento.Length; i++)
        //            {
        //                if (lstProdutosOrcamento[i].IdOrcamento == 0 || lstProdutosOrcamento[i].IdOrcamento == null)
        //                    continue;

        //                if (lstProdutosOrcamento[i].IdProduto == 0 || lstProdutosOrcamento[i].IdProduto == null)
        //                {
        //                    if (lstProdutosOrcamento[i].IdItemProjeto == 0 || lstProdutosOrcamento[i].IdItemProjeto == null)
        //                        lstIdProd.Add(lstProdutosOrcamento[i].IdProd);

        //                    continue;
        //                }

        //                uint? idCliente = OrcamentoDAO.Instance.ObtemIdCliente(lstProdutosOrcamento[i].IdOrcamento);

        //                if (idCliente == 0 || idCliente == null)
        //                    continue;

        //                if (lstProdutosOrcamento[i].IdOrcamento == 557)
        //                    lstProdutosOrcamento[i].IdOrcamento = 557;

        //                decimal valorProduto = lstProdutosOrcamento[i].ValorProd == null || lstProdutosOrcamento[i].ValorProd == 0 ? lstProdutosOrcamento[i].ValorTabela :
        //                    (decimal)lstProdutosOrcamento[i].ValorProd;
        //                float qtdeProd = lstProdutosOrcamento[i].Qtde != null ? lstProdutosOrcamento[i].Qtde.Value : 0;

        //                decimal custoTemp = 0, totalTemp = 0;
        //                float alturaTemp = lstProdutosOrcamento[i].AlturaCalc, totM2Temp = lstProdutosOrcamento[i].TotM;

        //                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente.Value, lstProdutosOrcamento[i].IdProduto.Value, lstProdutosOrcamento[i].Largura, qtdeProd,
        //                    lstProdutosOrcamento[i].QtdeAmbiente, valorProduto, lstProdutosOrcamento[i].Espessura, lstProdutosOrcamento[i].Redondo,
        //                    2, false, ref custoTemp, ref alturaTemp, ref totM2Temp, ref totalTemp, false, lstProdutosOrcamento[i].Beneficiamentos.CountAreaMinima);

        //                objPersistence.ExecuteCommand("Update produtos_orcamento Set custo=" + custoTemp.ToString().Replace(",", ".") +
        //                    " Where idProd=" + lstProdutosOrcamento[i].IdProd);

        //                if (!dadosOrcamentosExecutar.ContainsKey(lstProdutosOrcamento[i].IdOrcamento))
        //                    dadosOrcamentosExecutar.Add(lstProdutosOrcamento[i].IdOrcamento, 0);

        //                dadosOrcamentosExecutar[lstProdutosOrcamento[i].IdOrcamento] += custoTemp;
        //            }

        //            foreach (uint idOrcamento in dadosOrcamentosExecutar.Keys)
        //                objPersistence.ExecuteCommand("Update orcamento Set custo=" + dadosOrcamentosExecutar[idOrcamento].ToString().Replace(",", ".") +
        //                    " Where idOrcamento=" + idOrcamento);

        //            foreach (uint idProd in lstIdProd)
        //            {
        //                string sql = "Select Coalesce(Sum(Round(custo, 2)), 0) From produtos_orcamento Where idProdParent=" + idProd;
        //                decimal custo = decimal.Parse(objPersistence.ExecuteScalar(sql).ToString());
        //                objPersistence.ExecuteCommand("Update produtos_orcamento Set custo=" + custo.ToString().Replace(",", ".") + " Where idProd=" + idProd);
        //            }

        //            tempPedidoDAO.Instance.AtualizarCusto();
        //        }
        //    }

        //    protected void btnAtualizaCustoTotal_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaCustoTotal != null)
        //            return;

        //        atualizaCustoTotal = new Thread(
        //            delegate()
        //            {
        //                tempOrcamentoDAO.Instance.AtualizarCusto();
        //                atualizaCustoTotal = null;
        //            });

        //        atualizaCustoTotal.Start();
        //    }

        //    protected void Button16_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaValorMovEstoque != null)
        //            return;

        //        atualizaValorMovEstoque = new Thread(
        //            delegate()
        //            {
        //                foreach (uint id in ValorMovEstoqueDAO.Instance.IdsReal())
        //                    MovEstoqueDAO.Instance.AtualizaSaldoTotal(id);

        //                foreach (uint id in ValorMovEstoqueDAO.Instance.IdsFiscal())
        //                    MovEstoqueFiscalDAO.Instance.AtualizaSaldoTotal(id);

        //                atualizaValorMovEstoque = null;
        //            }
        //        );

        //        atualizaValorMovEstoque.Start();
        //    }

        //    protected void Button17_Click(object sender, EventArgs e)
        //    {
        //        if (atualizaDescontoAcrescimoCliente != null)
        //            return;

        //        atualizaDescontoAcrescimoCliente = new Thread(
        //            delegate()
        //            {
        //                using (var dao = TempDescontoAcrescimoClienteDAO.Instance)
        //                {
        //                    dao.CorrigeOrcamento();
        //                    dao.CorrigePedido();
        //                    dao.CorrigePedidoEspelho();
        //                    dao.CorrigeTrocaDev();
        //                    dao.CorrigeMaterial();
        //                }

        //                atualizaDescontoAcrescimoCliente = null;
        //            }
        //        );

        //        atualizaDescontoAcrescimoCliente.Start();
        //    }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserInfo.GetUserInfo.IsAdminSync)
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "voltar", "window.history.go(-1);\n", true);
        }

        protected void btnImportarClientes_Click(object sender, EventArgs e)
        {
            if (!fpImportarClientes.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            StreamReader reader = new StreamReader(fpImportarClientes.FileContent, Encoding.Default);
            string txt = reader.ReadToEnd();
            reader.Close();
            string[] clientes = txt.Replace("\r", "").Replace("\n", "").Replace("\t", "").Split(new string[] { "$$$" }, StringSplitOptions.None);

            string log = clientes.Length + " clientes para importação.\n\n";

            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);

            for (int i = 0; i < clientes.Length; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(clientes[i]))
                        continue;

                    string[] colunas = clientes[i].Split('|');

                    if (string.IsNullOrEmpty(Formatacoes.LimpaCpfCnpj(colunas[4])))
                        throw new Exception("Cliente sem CPF/CNPJ");

                    Cliente cli = ClienteDAO.Instance.GetByCpfCnpj(colunas[4]);

                    if (cli == null)
                        cli = new Cliente();

                    cli.Nome = colunas[0].ToUpper().Trim();
                    cli.NomeFantasia = colunas[1].ToUpper().Trim();
                    cli.TipoPessoa = colunas[2] == "1" ? "F" : "J";
                    cli.ProdutorRural = colunas[3].ToUpper() == "SIM" ? true : false;
                    cli.CpfCnpj = Formatacoes.FormataCpfCnpj(colunas[4].Trim());
                    cli.RgEscinst = colunas[5].ToUpper().Trim();
                    cli.Suframa = colunas[6].ToUpper().Trim();
                    cli.Email = Glass.Validacoes.ValidaEmail(colunas[7].Trim()) ? colunas[7].ToUpper().Trim() : "";
                    cli.DataNasc = FuncoesData.ValidaData(colunas[8].Trim()) ? DateTime.Parse(colunas[8].Trim()) : (DateTime?)null;
                    cli.TelCont = colunas[9].Trim();
                    cli.TelCel = colunas[10].Trim();
                    cli.Fax = colunas[11].Trim();
                    cli.TipoFiscal = colunas[12].ToUpper().Trim() == "CONSUMIDOR FINAL" || colunas[12].ToUpper().Trim() == "C" ? 1 :
                        colunas[12].ToUpper().Trim() == "REVENDA" || colunas[12].ToUpper().Trim() == "R" ? 2 : (int?)null;
                    if (cli.TipoFiscal.HasValue && cli.TipoFiscal.Value == (int)TipoFiscalCliente.Revenda)
                        cli.Revenda = true;

                    cli.Endereco = colunas[13].ToUpper().Trim();
                    cli.Numero = colunas[14].ToUpper().Trim();
                    cli.Compl = colunas[15].ToUpper().Trim();
                    cli.Bairro = colunas[16].ToUpper().Trim();

                    var cidade = CidadeDAO.Instance.GetList(null, colunas[17].Trim(), null, 0, 10);
                    cli.IdCidade = cidade.Count == 1 ? cidade[0].IdCidade : (int?)null;

                    cli.Cep = colunas[18].ToUpper().Trim();

                    cli.EnderecoCobranca = colunas[19].ToUpper().Trim();
                    cli.NumeroCobranca = colunas[20].ToUpper().Trim();
                    cli.ComplCobranca = colunas[21].ToUpper().Trim();
                    cli.BairroCobranca = colunas[22].ToUpper().Trim();

                    var cidadeCobranca = CidadeDAO.Instance.GetList(null, colunas[23].Trim(), null, 0, 10);
                    cli.IdCidadeCobranca = cidadeCobranca.Count == 1 ? cidadeCobranca[0].IdCidade : (int?)null;

                    cli.CepCobranca = colunas[24].ToUpper().Trim();

                    cli.EnderecoEntrega = colunas[25].ToUpper().Trim();
                    cli.NumeroEntrega = colunas[26].ToUpper().Trim();
                    cli.ComplEntrega = colunas[27].ToUpper().Trim();
                    cli.BairroEntrega = colunas[28].ToUpper().Trim();

                    var cidadeEntrega = CidadeDAO.Instance.GetList(null, colunas[29].Trim(), null, 0, 10);
                    cli.IdCidadeEntrega = cidadeEntrega.Count == 1 ? cidadeEntrega[0].IdCidade : (int?)null;

                    cli.CepEntrega = colunas[30].ToUpper().Trim();

                    cli.Limite = Glass.Conversoes.StrParaDecimal(colunas[31].Trim());
                    cli.Obs = colunas[32].ToUpper().Trim();
                    cli.PercReducaoNFe = Glass.Conversoes.StrParaFloat(colunas[33].ToUpper().Trim());
                    cli.PercReducaoNFeRevenda = Glass.Conversoes.StrParaFloat(colunas[34].ToUpper().Trim());

                    if (!string.IsNullOrEmpty(colunas[35].Trim()) && Glass.Conversoes.StrParaUint(colunas[35].Trim()) > 0)
                        cli.IdRota = Glass.Conversoes.StrParaInt(colunas[35].Trim());

                    cli.IdFunc = (!string.IsNullOrEmpty(colunas[36].Trim()) && Glass.Conversoes.StrParaInt(colunas[36].Trim()) > 0 ? Glass.Conversoes.StrParaInt(colunas[36].Trim()) : (int?)null);

                    cli.Historico = colunas[37].ToUpper().Trim();


                    cli.IdLoja = (int)idLoja;
                    cli.Situacao = 2;
                    cli.Crt = 1;

                    if (ClienteDAO.Instance.Exists(cli))
                        ClienteDAO.Instance.Update(cli);
                    else
                        cli.IdCli = (int)ClienteDAO.Instance.Insert(cli);
                }
                catch (Exception ex)
                {
                    var cliente = string.Join("|", clientes[i].Split('|').Select(f => f.Trim()).ToArray());
                    log += "Registro: " + (i + 1) + "\t\r" + cliente + "\t\r" + ex.Message + "\n\n";
                }
            }

            txtLogImportaçãoCliente.Text = log;
        }

        protected void btnImportarFornecedores_Click(object sender, EventArgs e)
        {
            if (!fpImportarFornecedores.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            StreamReader reader = new StreamReader(fpImportarFornecedores.FileContent, Encoding.Default);
            string txt = reader.ReadToEnd();
            reader.Close();
            List<string> fornecedores = new List<string>(txt.Replace("\r", "").Replace("\n", "").Split(';'));

            string log = fornecedores.Count + " fornecedores para importação.\n\n";

            for (int i = 0; i < fornecedores.Count; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(fornecedores[i]))
                        continue;

                    string[] colunas = fornecedores[i].Split('|');

                    //string idFornec = "";

                    //if (!string.IsNullOrEmpty(Formatacoes.LimpaCpfCnpj(colunas[5])))
                    //    idFornec = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(colunas[5]);

                    Fornecedor fornec = new Fornecedor();

                    //if (string.IsNullOrEmpty(idFornec))
                    //{

                    fornec.Razaosocial = colunas[1].ToUpper().Trim();
                    fornec.Nomefantasia = !string.IsNullOrEmpty(colunas[2]) ? colunas[2].ToUpper().Trim() : colunas[1].ToUpper().Trim();
                    fornec.TipoPessoa = colunas[3] == "1" ? "F" : "J";
                    fornec.ProdutorRural = colunas[4].ToUpper() == "SIM" ? true : false;
                    fornec.CpfCnpj = !string.IsNullOrEmpty(Formatacoes.LimpaCpfCnpj(colunas[5])) ?
                        Formatacoes.FormataCpfCnpj(colunas[5].Trim()) : "";
                    fornec.RgInscEst = colunas[6].ToUpper().Trim().Replace(".", "");
                    fornec.Suframa = colunas[7].ToUpper().Trim();
                    fornec.Crt = (RegimeFornecedor)(!string.IsNullOrEmpty(colunas[8]) ? Glass.Conversoes.StrParaInt(colunas[8].ToUpper().Trim()) : 1);

                    fornec.Endereco = colunas[9].ToUpper().Trim();
                    fornec.Numero = colunas[10].ToUpper().Trim();
                    fornec.Compl = colunas[11].ToUpper().Trim();
                    fornec.Bairro = colunas[12].ToUpper().Trim();
                    fornec.IdCidade = (int)CidadeDAO.Instance.GetCidadeByNomeUf(colunas[13].Trim(), colunas[14].Trim());
                    fornec.IdPais = (int)PaisDAO.Instance.GetPaisByNome(colunas[15].ToUpper().Trim());
                    fornec.Cep = colunas[16].ToUpper().Trim();

                    fornec.Telcont = colunas[17].Trim();
                    fornec.Fax = colunas[18].Trim();
                    fornec.Email = Glass.Validacoes.ValidaEmail(colunas[19].Trim()) ? colunas[19].ToUpper().Trim() : "";
                    fornec.Vendedor = colunas[20].ToUpper().Trim();
                    fornec.Telcelvend = colunas[21].Trim();
                    fornec.Obs = colunas[22].ToUpper().Trim();

                    fornec.Situacao = SituacaoFornecedor.Inativo;
                    fornec.DataCad = DateTime.Now;
                    fornec.Usucad = UserInfo.GetUserInfo.CodUser;

                    var idFornec = FornecedorDAO.Instance.Insert(fornec);

                    var idFornecNovo = Glass.Conversoes.StrParaUint(colunas[0].ToUpper().Trim());

                    if (idFornecNovo > 0)
                        tempClienteDAO.Instance.AtualizaIdFornec(idFornec, idFornecNovo);

                    //}
                }
                catch (Exception ex)
                {
                    log += "Registro: " + (i + 1) + "\t\r" + fornecedores[i] + "\t\r" + ex.Message + "\n\n";
                }
            }

            txtLogImportacaoFornec.Text = log;
        }

        protected void btnImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!fupImportarProdutos.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            StreamReader reader = new StreamReader(fupImportarProdutos.FileContent, Encoding.Default);

            string log = "";
            string line = "";

            string sqlGrupoProd = "SELECT idGrupoProd FROM grupo_prod WHERE descricao like ";
            string sqlSubgrupoProd = "SELECT idSubgrupoProd FROM subgrupo_prod WHERE descricao like ";
            string sqlUnMedida = "SELECT idUnidadeMedida FROM unidade_medida WHERE ";
            string sqlCorVidro = "SELECT idCorVidro FROM cor_vidro WHERE ";
            string sqlCorFerragem = "SELECT idCorFerragem FROM cor_ferragem WHERE ";
            string sqlCorAluminio = "SELECT idCorAluminio FROM cor_aluminio WHERE ";

            var arquivo = reader.ReadToEnd().Split(new string[] { "$$$" }, StringSplitOptions.None);

            for (int i = 0; i < arquivo.Length; i++)
            {
                line = arquivo[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                    List<string> produto = new List<string>(line.Split('|'));

                    for (int j = 0; j < produto.Count; j++)
                        produto[j] = produto[j].Trim();

                    Produto p;

                    p = ProdutoDAO.Instance.GetByCodInterno(produto[0].Trim());

                    if (p == null)
                        p = new Produto();

                    p.CodInterno = produto[0].Trim();
                    p.Descricao = produto[1].Trim();

                    var grupo = GrupoProdDAO.Instance.GetValoresCampo(sqlGrupoProd + "'" + produto[2].Trim() + "'", "idGrupoProd");
                    if (grupo.Contains(','))
                        throw new Exception("Grupo duplicado: " + produto[2].Trim());
                    if (string.IsNullOrEmpty(grupo))
                        throw new Exception("Grupo não encontrado: " + produto[2].Trim());
                    else
                        p.IdGrupoProd = Glass.Conversoes.StrParaInt(grupo);

                    var subgrupo = SubgrupoProdDAO.Instance.GetValoresCampo(sqlSubgrupoProd + "'" + produto[3].Trim()
                        + "' AND idGrupoProd=" + grupo, "idSubgrupoProd");
                    if (string.IsNullOrEmpty(subgrupo))
                        throw new Exception("Subgrupo não encontrado: " + produto[3].Trim());
                    else
                        p.IdSubgrupoProd = Glass.Conversoes.StrParaInt(subgrupo);

                    p.ValorAtacado = !string.IsNullOrEmpty(produto[4].Trim()) && produto[4] != "#N/D" ? decimal.Parse(produto[4].Trim().Replace('.', ',')) : 0;
                    p.ValorObra = !string.IsNullOrEmpty(produto[5].Trim()) && produto[5] != "#N/D" ? decimal.Parse(produto[5].Trim().Replace('.', ',')) : 0;
                    p.ValorBalcao = !string.IsNullOrEmpty(produto[6].Trim()) && produto[6] != "#N/D" ? decimal.Parse(produto[6].Trim().Replace('.', ',')) : 0;
                    p.ValorReposicao = !string.IsNullOrEmpty(produto[7].Trim()) && produto[7] != "#N/D" ? decimal.Parse(produto[7].Trim().Replace('.', ',')) : 0;
                    p.ValorFiscal = !string.IsNullOrEmpty(produto[8].Trim()) && produto[8] != "#N/D" ? decimal.Parse(produto[8].Trim().Replace('.', ',')) : 0;
                    p.AliqICMSST = !string.IsNullOrEmpty(produto[9].Trim()) && produto[9] != "#N/D" ? float.Parse(produto[9].Trim()) : 0;
                    p.AliqIPI = !string.IsNullOrEmpty(produto[10].Trim()) && produto[10] != "#N/D" ? float.Parse(produto[10].Trim()) : 0;
                    p.Cst = string.IsNullOrEmpty(produto[11].Trim()) ? produto[11] : "00";
                    p.CstIpi = (Glass.Data.Model.ProdutoCstIpi?)(!string.IsNullOrEmpty(produto[12].Trim()) && produto[12] != "#N/D" ? Glass.Conversoes.StrParaInt(produto[12].Trim()) : 0);
                    p.Ncm = produto[13].Trim();
                    p.AreaMinima = !string.IsNullOrEmpty(produto[14]) && produto[14] != " " && produto[14] != "#N/D" ? float.Parse(produto[14]) : 0;
                    p.CodOtimizacao = produto[15].Trim();

                    string idUnMedida = "";

                    if (!string.IsNullOrEmpty(produto[16]))
                    {
                        idUnMedida = UnidadeMedidaDAO.Instance.GetValoresCampo(sqlUnMedida + " codigo like '"
                            + produto[16] + "'", "idUnidadeMedida");

                        if (string.IsNullOrEmpty(idUnMedida) || idUnMedida == " " || idUnMedida == "0")
                            idUnMedida = UnidadeMedidaDAO.Instance.Insert(new UnidadeMedida()
                            {
                                Codigo = produto[16],
                                Usucad = UserInfo.GetUserInfo.CodUser,
                                DataCad = DateTime.Now
                            }).ToString();

                        p.IdUnidadeMedida = Glass.Conversoes.StrParaInt(idUnMedida);
                        p.Unidade = UnidadeMedidaDAO.Instance.ObtemCodigo(Glass.Conversoes.StrParaUint(idUnMedida));
                    }

                    if (!string.IsNullOrEmpty(produto[17]))
                    {
                        idUnMedida = UnidadeMedidaDAO.Instance.GetValoresCampo(sqlUnMedida + " codigo like '"
                            + produto[17] + "'", "idUnidadeMedida");

                        if (string.IsNullOrEmpty(idUnMedida) || idUnMedida == " " || idUnMedida == "0")
                            idUnMedida = UnidadeMedidaDAO.Instance.Insert(new UnidadeMedida()
                            {
                                Codigo = produto[17],
                                Usucad = UserInfo.GetUserInfo.CodUser,
                                DataCad = DateTime.Now
                            }).ToString();

                        p.IdUnidadeMedidaTrib = Glass.Conversoes.StrParaInt(idUnMedida);
                        p.UnidadeTrib = UnidadeMedidaDAO.Instance.ObtemCodigo(Glass.Conversoes.StrParaUint(idUnMedida));
                    }

                    if (!string.IsNullOrEmpty(produto[18]))
                    {
                        switch (grupo)
                        {
                            case "1":
                                uint? idCorVidro = Glass.Conversoes.StrParaUintNullable(CorVidroDAO.Instance.GetValoresCampo(sqlCorVidro
                                    + " sigla like '" + produto[18] + "'", "idCorVidro"));
                                if (!idCorVidro.HasValue || idCorVidro.Value == 0)
                                {
                                    idCorVidro = CorVidroDAO.Instance.Insert(new CorVidro()
                                    {
                                        Descricao = produto[18],
                                        Sigla = produto[18]
                                    });
                                }
                                p.IdCorVidro = (int?)idCorVidro;

                                break;
                            case "3":
                                uint? idCorAluminio = Glass.Conversoes.StrParaUintNullable(CorAluminioDAO.Instance.GetValoresCampo(sqlCorAluminio
                                   + " sigla like '" + produto[18] + "'", "idCorAluminio"));
                                if (!idCorAluminio.HasValue || idCorAluminio.Value == 0)
                                {
                                    idCorAluminio = CorAluminioDAO.Instance.Insert(new CorAluminio()
                                    {
                                        Descricao = produto[18],
                                        Sigla = produto[18]
                                    });
                                }
                                p.IdCorAluminio = (int?)idCorAluminio;
                                break;
                            case "4":
                                uint? idCorFerragem = Glass.Conversoes.StrParaUintNullable(CorFerragemDAO.Instance.GetValoresCampo(sqlCorFerragem
                                   + " sigla like '" + produto[18] + "'", "idCorFerragem"));
                                if (!idCorFerragem.HasValue || idCorFerragem.Value == 0)
                                {
                                    idCorFerragem = CorFerragemDAO.Instance.Insert(new CorFerragem()
                                    {
                                        Descricao = produto[18],
                                        Sigla = produto[18]
                                    });
                                }
                                p.IdCorFerragem = (int?)idCorFerragem;
                                break;

                            default:
                                break;
                        }
                    }

                    p.Espessura = !string.IsNullOrEmpty(produto[19]) && produto[19] != " " && produto[19] != "#N/D" ? float.Parse(produto[19]) : 0;
                    p.Peso = !string.IsNullOrEmpty(produto[20]) && produto[20] != " " && produto[20] != "#N/D" ? float.Parse(produto[20].Replace('.', ',')) : 0;
                    p.Obs = produto[21].Trim();
                    p.Custofabbase = !string.IsNullOrEmpty(produto[22]) && produto[22] != " " && produto[22] != "#N/D" ? decimal.Parse(produto[22].Replace('.', ',')) : 0;
                    p.CustoCompra = !string.IsNullOrEmpty(produto[23]) && produto[23] != " " && produto[23] != "#N/D" ? decimal.Parse(produto[23].Replace('.', ',')) : 0;

                    p.Situacao = Glass.Situacao.Inativo;
                    p.Usucad = UserInfo.GetUserInfo.CodUser;
                    p.DataCad = DateTime.Now;

                    ProdutoDAO.Instance.InsertOrUpdate(p);

                }
                catch (Exception ex)
                {
                    line = string.Join("|", line.Split('|').Select(f => f.Trim()).ToArray());
                    log += line + "\t\r" + ex.Message + "\n\n";
                }
            }


            reader.Close();

            txtImportarProdutos.Text = log;
        }

        protected void btnAtualizaIdCliente_Click(object sender, EventArgs e)
        {
            if (!fupAtualizaIdCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            StreamReader reader = new StreamReader(fupAtualizaIdCliente.FileContent, Encoding.Default);
            string txt = reader.ReadToEnd();
            reader.Close();
            string[] clientes = txt.Replace("\r", "").Replace("\n", "").Split(new string[] { "$$$" }, StringSplitOptions.None);

            string log = "";

            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);

            for (int i = 0; i < clientes.Length; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(clientes[i]))
                        continue;

                    string[] colunas = clientes[i].Split('|');

                    if (string.IsNullOrEmpty(Formatacoes.LimpaCpfCnpj(colunas[4])))
                        throw new Exception("Cliente sem CPF/CNPJ");

                    if (!ClienteDAO.Instance.CheckIfExists(colunas[4]))
                        continue;

                    tempClienteDAO.Instance.AtualizaId(colunas[4], Glass.Conversoes.StrParaUint(colunas[32]));
                }
                catch (Exception ex)
                {
                    log += "Registro: " + (i + 1) + "\t\r" + clientes[i] + "\t\r" + ex.Message + "\n\n";
                }
            }

            txtLogAtualizaIdCliente.Text = log;
        }

        protected void btnAtualizarTipoCliente_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupAtualizarTipoCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupAtualizarTipoCliente.FileContent, Encoding.Default))
            {
                var tipoCli = TipoClienteDAO.Instance.GetAll();

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;



                        var c = line.Split(';');

                        var Id = Glass.Conversoes.StrParaUint(c[0].ToString().Trim());
                        var CpfCpnj = Formatacoes.FormataCpfCnpj(c[1].ToString().Trim());
                        var IdTipoCli = tipoCli.Where(tc => Glass.Conversoes.StrParaInt(tc.Descricao.Split('-')[0].Trim()) == Glass.Conversoes.StrParaInt(c[2].ToString().Trim()))
                            .Select(tc => tc.IdTipoCliente).FirstOrDefault();
                        var TipoFiscal = c[3].ToString().Trim().Equals("revenda") ? 2 : 1;

                        var cli = ClienteDAO.Instance.GetElement(Id);

                        if (cli == null)
                        {
                            cli = ClienteDAO.Instance.GetByCpfCnpj(CpfCpnj);

                            if (cli == null)
                                throw new Exception("Cliente não encontrado");
                        }

                        if (!CpfCpnj.Equals(cli.CpfCnpj))
                            throw new Exception("Cpf/Cnpj divergente");

                        cli.IdTipoCliente = IdTipoCli;
                        cli.TipoFiscal = TipoFiscal;

                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }


            txtLogAtualizarTipoCliente.Text = log;
        }

        protected void btnAtualizaroEndereco_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupAtualizarEndereco.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupAtualizarEndereco.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var c = line.Split(';');

                        var Id = Glass.Conversoes.StrParaUint(c[0].ToString().Trim());
                        var CpfCpnj = Formatacoes.FormataCpfCnpj(c[1].ToString().Trim());
                        var Endereco = c[2].ToString().Trim();
                        var Numero = c[3].ToString().Trim();

                        int num = 0;
                        if (Numero.Length > 10)
                        {
                            if (!int.TryParse(Numero.Split(' ')[0], out num))
                                throw new Exception("Número ultrapassa 10 caracteres.");
                        }

                        var cli = ClienteDAO.Instance.GetElement(Id);

                        if (cli == null)
                        {
                            cli = ClienteDAO.Instance.GetByCpfCnpj(CpfCpnj);

                            if (cli == null)
                                throw new Exception("Cliente não encontrado");
                        }

                        if (!CpfCpnj.Equals(cli.CpfCnpj))
                            throw new Exception("Cpf/Cnpj divergente");

                        cli.Endereco = Endereco;
                        if (num > 0)
                        {
                            cli.Numero = num.ToString();
                            cli.Compl = Numero.Substring(Numero.IndexOf(' ') + 1);
                        }
                        else
                        {
                            cli.Numero = Numero;
                        }

                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }


            txtLogAtualizarEndereco.Text = log;
        }

        protected void btnImportarContasReceber_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImportarContasReceber.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImportarContasReceber.FileContent, Encoding.Default))
            {
                string line;

                var lojas = LojaDAO.Instance.GetAll();

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        //if (dados.Length != 12)
                        //    throw new Exception("Dados inválidos: número de informações diferente de 12.");

                        //var idLoja = (lojas.FirstOrDefault(x => String.Compare(dados[0].Trim(), x.NomeFantasia.Trim(),
                        //    StringComparison.InvariantCultureIgnoreCase) == 0) ?? new Loja()).IdLoja;

                        var idLoja = Glass.Conversoes.StrParaUint(drpLojasContasReceber.SelectedValue);

                        var idLiberarPedido = idLoja == 1 ? 15 : 16;

                        //ulong temp;
                        //string numeroDocumentoCnab = dados[8].Trim();
                        //numeroDocumentoCnab = numeroDocumentoCnab.Length == 9 && ulong.TryParse(numeroDocumentoCnab, out temp) &&
                        //    temp.ToString() == numeroDocumentoCnab ? numeroDocumentoCnab : null;

                        var cli = ClienteDAO.Instance.GetByCpfCnpj(dados[0].Trim());

                        if (cli == null)
                            throw new Exception("Cliente não encontrado. " + dados[0].Trim());

                        var contaReceber = new ContasReceber()
                        {
                            IdLoja = idLoja,
                            IdLiberarPedido = (uint?)idLiberarPedido,
                            IdCliente = (uint)cli.IdCli,
                            DataVec = Conversoes.ConverteDataNotNull(dados[1].Trim()),
                            ValorVec = Glass.Conversoes.StrParaDecimal(dados[2].Trim().Replace(".", ",")),
                            NumParc = Glass.Conversoes.StrParaInt(dados[3].Trim()),
                            //NumParcMax = Glass.Conversoes.StrParaInt(dados[5].Trim()),
                            Obs = dados[5].Trim(),
                            // NumeroDocumentoCnab = numeroDocumentoCnab,
                            TipoConta = (byte)(ContasReceber.TipoContaEnum.NaoContabil),
                            //IdConta = dados[10].Trim() != String.Empty ? PlanoContasDAO.Instance.GetId(Glass.Conversoes.StrParaUint(dados[10].Trim()), 50) : (uint?)null
                        };

                        //if (dados[11].Trim().ToUpper() == "RP")
                        //    contaReceber.TipoConta = (byte)((ContasReceber.TipoContaEnum)contaReceber.TipoConta | ContasReceber.TipoContaEnum.Reposicao);

                        ContasReceberDAO.Instance.InsertExecScript(contaReceber);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro: " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImportarContasReceber.Text = log;
        }

        protected void btnImportarContasPagar_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImportarContasPagar.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImportarContasPagar.FileContent, Encoding.Default))
            {
                string line;
                int linha = 0;

                var fornecedores = FornecedorDAO.Instance.GetAll();
                var lojas = LojaDAO.Instance.GetAll();

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        linha++;

                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        if (dados.Length != 11)
                            throw new Exception("Dados inválidos: número de informações diferente de 11.");

                        var idFornec = (fornecedores.FirstOrDefault(x => String.Compare(dados[0].Trim(),
                            (x.CpfCnpj ?? String.Empty).Replace(".", "").Replace("/", "").Replace("-", "").Trim()) == 0) ?? new Fornecedor()).IdFornec;

                        var idLoja = (lojas.FirstOrDefault(x => String.Compare(dados[1].Trim(), x.NomeFantasia.Trim(),
                            StringComparison.InvariantCultureIgnoreCase) == 0) ?? new Loja()).IdLoja;

                        var idGrupo = Glass.Conversoes.StrParaUintNullable(dados[9].Trim());
                        var idContaGrupo = Glass.Conversoes.StrParaUintNullable(dados[10].Trim());

                        var contaPagar = new ContasPagar()
                        {
                            IdFornec = idFornec > 0 ? (uint)idFornec : (uint?)null,
                            IdLoja = (uint)idLoja,
                            DataVenc = Conversoes.ConverteDataNotNull(dados[2].Trim()),
                            ValorVenc = Glass.Conversoes.StrParaDecimal(dados[3].Trim().Replace(".", "")),
                            NumParc = Glass.Conversoes.StrParaInt(dados[4].Trim()),
                            NumParcMax = Glass.Conversoes.StrParaInt(dados[5].Trim()),
                            Obs = (dados[6].Trim() + " - " + dados[7].Trim()).TrimStart(' ', '-'),
                            Contabil = dados[8].Trim().ToUpper() == FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil.ToUpper(),
                            IdConta = idGrupo > 0 && idContaGrupo > 0 ? PlanoContasDAO.Instance.GetId(idContaGrupo.Value, idGrupo.Value) : 0
                        };

                        ContasPagarDAO.Instance.InsertExecScript(contaPagar);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImportarContasPagar.Text = log;
        }

        protected void btnDadosCliente_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupDadosCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupDadosCliente.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var c = line.Split(';');

                        var Id = Glass.Conversoes.StrParaUint(c[0].ToString().Trim());
                        var limite = Glass.Conversoes.StrParaDecimal(c[1].ToString().Trim());
                        var simpes = c[2].ToString().Trim().ToLower() == "yes" ? 2 : 1;

                        var cli = ClienteDAO.Instance.GetElement(Id);

                        if (cli == null)
                            throw new Exception("Cliente não encontrado");

                        cli.LimiteCheques = limite;
                        cli.Crt = simpes;

                        if (!string.IsNullOrEmpty(c[3].ToString().Trim()))
                            cli.DataNasc = DateTime.Parse(c[3].ToString().Trim());

                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }


            txtLogImportarDadosCliente.Text = log;
        }

        protected void btmImportarPrecoProduto_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupImportarPrecoProduto.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImportarPrecoProduto.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var p = line.Split(';');

                        var codInterno = p[0].ToString().Trim();
                        var valor = Glass.Conversoes.StrParaDecimal(p[1].ToString().Trim());

                        var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                        if (prod == null)
                            throw new Exception("Produto não encontrado");

                        prod.ValorAtacado = valor;
                        prod.ValorObra = valor;
                        prod.ValorBalcao = valor;
                        prod.ValorReposicao = valor;

                        ProdutoDAO.Instance.Update(prod);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }


            txtLogImportarPrecoProduto.Text = log;
        }

        protected void btmImportarTipoMercadoria_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupImportarTipoMercadoria.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImportarTipoMercadoria.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var p = line.Split(';');

                        var codInterno = p[0].ToString().Trim();
                        var tipoMercadoria = Glass.Conversoes.StrParaIntNullable(p[1].ToString().Trim());
                        var codOtimizacao = p[2].ToString().Trim().ToLower();

                        var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);

                        if (idProd == 0)
                            throw new Exception("Produto não encontrado");


                        tempClienteDAO.Instance.AtualizaTipoMercadoriaCodOtimizacao((uint)idProd, codOtimizacao, tipoMercadoria);
                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }


            txtLogImportarTipoMercadoria.Text = log;
        }

        protected void btnImportarDescontoCliente_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupImportarDescontoCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            var subGrupos = SubgrupoProdDAO.Instance.GetRptSubGrupo();

            using (StreamReader reader = new StreamReader(fupImportarDescontoCliente.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var d = line.Split(';');

                        var idCli = Glass.Conversoes.StrParaUint(d[0].ToString().Trim());
                        var grupo = d[1].ToString().Trim().ToLower();
                        var subGrupo = d[2].ToString().Trim().ToLower();
                        var descProd = d[3].ToString().Trim().ToLower();
                        var valor = Glass.Conversoes.StrParaFloat(d[4].ToString().Trim());

                        var cli = ClienteDAO.Instance.GetElement(idCli);

                        if (cli == null)
                            throw new Exception("Cliente não encontrado");

                        int idProd = 0;

                        if (!string.IsNullOrEmpty(descProd))
                        {
                            idProd = ProdutoDAO.Instance.ObtemIdProd(descProd);
                            if (idProd == 0)
                                throw new Exception("Produto não encontrado");
                        }

                        SubgrupoProd subGrupoCli;

                        if (string.IsNullOrEmpty(subGrupo) && idProd > 0)
                        {
                            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);

                            if (idSubgrupoProd.GetValueOrDefault(0) == 0)
                                throw new Exception("Subgrupo não encontrado");

                            subGrupoCli = SubgrupoProdDAO.Instance.GetElementByPrimaryKey((uint)idSubgrupoProd.Value);
                        }
                        else
                        {
                            subGrupoCli = subGrupos.Where(f => f.Descricao.ToLower().Equals(subGrupo)).FirstOrDefault();
                        }

                        if (subGrupoCli == null)
                            throw new Exception("Subgrupo não encontrado");

                        if (!string.IsNullOrEmpty(grupo) && !GrupoProdDAO.Instance.GetDescricao(subGrupoCli.IdGrupoProd).ToLower().Equals(grupo))
                            throw new Exception("Grupo não encontrado");



                        if (valor == 0)
                            continue;

                        DescontoAcrescimoClienteDAO.Instance.Insert(new DescontoAcrescimoCliente()
                        {
                            IdCliente = (int?)idCli,
                            IdGrupoProd = subGrupoCli.IdGrupoProd,
                            IdSubgrupoProd = subGrupoCli.IdSubgrupoProd,
                            IdProduto = idProd > 0 ? idProd : (int?)null,
                            Desconto = valor > 0 ? valor : 0,
                            Acrescimo = valor < 0 ? valor * -1 : 0
                        });
                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }
            txtLogImportarDescontoCliente.Text = log;
        }

        protected void btnImportarRotaCliente_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupImportarRotaCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImportarRotaCliente.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var c = line.Split(';');

                        var idCli = Glass.Conversoes.StrParaUint(c[0].ToString().Trim());
                        var idRota = c[1].ToString().Trim();


                        var cli = ClienteDAO.Instance.GetElement(idCli);

                        if (cli == null)
                            throw new Exception("Cliente não encontrado");

                        if (string.IsNullOrEmpty(idRota))
                        {
                            cli.IdRota = 0;
                            ClienteDAO.Instance.Update(cli);
                            continue;
                        }

                        var rota = RotaDAO.Instance.GetByCodInterno(idRota);

                        if (rota == null)
                        {
                            if (idRota == "98" || idRota == "9")
                            {
                                cli.IdRota = 0;
                                ClienteDAO.Instance.Update(cli);
                                continue;
                            }
                            else
                                throw new Exception("rota não encontrada");
                        }


                        cli.IdRota = (int)rota.IdRota;
                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }
            txtImportarRotaCliente.Text = log;
        }

        protected void btnDescEsp_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupDescEsp.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupDescEsp.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var c = line.Split(';');

                        var idCli = Glass.Conversoes.StrParaUint(c[0].ToString().Trim());
                        var percDesc = Glass.Conversoes.StrParaFloat(c[1].ToString().Trim());


                        var cli = ClienteDAO.Instance.GetElement(idCli);

                        if (cli == null)
                            throw new Exception("Cliente não encontrado");

                        if (percDesc == 0)
                            continue;


                        cli.PercReducaoNFe = percDesc;
                        cli.PercReducaoNFeRevenda = percDesc;

                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }

            txtDescEsp.Text = log;
        }

        protected void btnPrecoTransferencia_Click(object sender, EventArgs e)
        {
            string log = "";

            var produtos = tempClienteDAO.Instance.BuscaTodosProdutos();

            foreach (var p in produtos)
            {
                try
                {
                    var valorBalcao = tempClienteDAO.Instance.ObtemValorBalcao(p);
                    decimal valorTransferencia = 0;

                    if (ProdutoDAO.Instance.IsProdutoVenda(null, (int)p))
                        valorTransferencia = valorBalcao * (decimal)0.6;
                    else
                        valorTransferencia = valorBalcao * (decimal)0.7;

                    tempClienteDAO.Instance.AtualizaValorTransferenciaProduto(p, valorTransferencia);
                }
                catch (Exception ex)
                {
                    log += "Registro: " + p + "\t\r" + ex.Message + Environment.NewLine;
                }
            }



            txtPrecoTransferencia.Text = log;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";


            if (!fupImportarNomeFantasia.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            var vendedores = FuncionarioDAO.Instance.GetAtivosAssociadosCliente();

            using (StreamReader reader = new StreamReader(fupImportarNomeFantasia.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var d = line.Split(';');

                        var idCli = Glass.Conversoes.StrParaUint(d[0].ToString().Trim());
                        var nomeFantasia = d[1].ToString().Trim().ToUpper();
                        var nomeVendedor = d[2].ToString().Trim().ToLower();

                        nomeVendedor = nomeVendedor.Replace(" - ", " ");

                        var cli = ClienteDAO.Instance.GetElement(idCli);

                        if (cli == null)
                            throw new Exception("Cliente não encontrado");

                        if (string.IsNullOrEmpty(nomeFantasia))
                            throw new Exception("Nome fantasia não informado");

                        if (string.IsNullOrEmpty(nomeVendedor))
                            throw new Exception("Vendedor não informado");

                        var idVendedor = vendedores.Where(f => f.Nome.Contains(nomeVendedor.ToUpper())).Select(f => f.IdFunc).FirstOrDefault();

                        //if (idVendedor == 0)
                        //throw new Exception("Vendedor não encontrado");

                        cli.NomeFantasia = nomeFantasia;

                        if (idVendedor > 0)
                            cli.IdFunc = (int)idVendedor;


                        ClienteDAO.Instance.Update(cli);

                    }
                    catch (Exception ex)
                    {
                        log += "Registro: " + line + "\t\r" + ex.Message + "\n\n";
                    }
                }
            }
            txtLogImportarDescontoCliente.Text = log;
        }

        protected void btnImportarAltLarBox_Click(object sender, EventArgs e)
        {
            string log = "";
            string line = "";

            //var processos = EtiquetaProcessoDAO.Instance.GetAll();
            //var aplicacoes = EtiquetaAplicacaoDAO.Instance.GetAll();


            if (!fupImportarAltLarBox.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            int linha = 0;

            using (StreamReader reader = new StreamReader(fupImportarAltLarBox.FileContent, Encoding.Default))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var p = line.Split(';');

                        var codInterno = p[0].ToString().Trim();
                        var altura = Glass.Conversoes.StrParaFloat(p[2].ToString().Trim());
                        var largura = Glass.Conversoes.StrParaFloat(p[1].ToString().Trim());
                        // var aplicacaoStr = p[3].ToString().Trim().ToUpper();
                        //var processoStr = p[4].ToString().Trim().ToUpper();

                        var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);

                        if (idProd == 0)
                            throw new Exception("Produto não encontrado");

                        // var aplicacao = aplicacoes.Where(f => f.CodInterno.Trim().ToUpper().Equals(aplicacaoStr)).FirstOrDefault();
                        // var processo = processos.Where(f => f.CodInterno.Trim().ToUpper().Equals(processoStr)).FirstOrDefault();

                        //if(aplicacao == null)
                        //throw new Exception("Aplicação não encontrada");

                        //if (aplicacao == null)
                        //throw new Exception("Processo não encontrada");


                        tempClienteDAO.Instance.AtualizaAltLargBox((uint)idProd, altura * 1000, largura * 1000);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }


            txtImportarAltLarBox.Text = log;
        }

        protected void btnImpProdBase_Click(object sender, EventArgs e)
        {
            string log = "";

            var prodsOrig = new List<Produto>();

            prodsOrig.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 66));
            prodsOrig.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 67));
            prodsOrig.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 68));
            prodsOrig.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 69));
            prodsOrig.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 86));

            foreach (var prod in prodsOrig)
            {
                try
                {
                    if (prod.IdCorVidro.GetValueOrDefault(0) == 0)
                        throw new Exception("Prod:" + prod.IdProd + " - " + prod.CodInterno + " sem cor do vidro.");

                    if (prod.Espessura == 0)
                        throw new Exception("Prod:" + prod.IdProd + " - " + prod.CodInterno + " sem cor espessura.");

                    tempClienteDAO.Instance.VincularProdBase((uint)prod.IdCorVidro.Value, prod.Espessura, (uint)prod.IdProd);
                }
                catch (Exception ex)
                {
                    log += "Registro: " + ex.Message + "\n\n";
                }
            }

            txtImpProdBase.Text = log;
        }

        protected void btnImpMatPri_Click(object sender, EventArgs e)
        {
            var subgrupos = new List<SubgrupoProd>(SubgrupoProdDAO.Instance.GetAll());

            subgrupos = (from s in subgrupos
                         where s.IdGrupoProd == 1 &&
                         s.IdSubgrupoProd != 66 &&
                         s.IdSubgrupoProd != 67 &&
                         s.IdSubgrupoProd != 68 &&
                         s.IdSubgrupoProd != 69 &&
                         s.IdSubgrupoProd != 86 &&
                         s.IdSubgrupoProd != 7 &&
                         !s.Descricao.Contains("LAMINADO")
                         select s).ToList();

            var prodsBase = new List<Produto>();

            prodsBase.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 66));
            prodsBase.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 67));
            prodsBase.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 68));
            prodsBase.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 69));
            prodsBase.AddRange(ProdutoDAO.Instance.GetByGrupoSubgrupo(0, 86));

            foreach (var subgrupo in subgrupos)
            {
                var prodsMP = ProdutoDAO.Instance.GetByGrupoSubgrupo(0, subgrupo.IdSubgrupoProd);

                foreach (var prodMP in prodsMP)
                {
                    var prodBase = prodsBase.Where(f => f.Espessura == prodMP.Espessura && f.IdCorVidro == prodMP.IdCorVidro).FirstOrDefault();

                    if (prodBase == null)
                        continue;

                    if (ProdutoBaixaEstoqueDAO.Instance.GetByProd((uint)prodMP.IdProd, false, 0) == null)
                    {
                        ProdutoBaixaEstoqueDAO.Instance.Insert(new ProdutoBaixaEstoque()
                        {
                            IdProd = prodMP.IdProd,
                            IdProdBaixa = prodBase.IdProd,
                            Qtde = 1
                        });
                    }
                }
            }
        }

        protected void btnImpEstoque_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImpEstoque.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImpEstoque.FileContent, Encoding.Default))
            {
                string line;

                var lojas = LojaDAO.Instance.GetAll();
                var produtos = ProdutoDAO.Instance.GetAll();

                var calcM2 = new[] { (int)Glass.Data.Model.TipoCalculoGrupoProd.M2, (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto };
                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        if (dados.Length != 5)
                            throw new Exception("Dados inválidos: número de informações diferente de 5.");

                        var idLoja = (lojas.FirstOrDefault(x => String.Compare(dados[0].Trim(), x.NomeFantasia.Trim(),
                            StringComparison.InvariantCultureIgnoreCase) == 0) ?? new Loja()).IdLoja;

                        if (idLoja == 0)
                            throw new Exception("Loja não encontrada: " + dados[0].Trim());

                        var idProd = (produtos.FirstOrDefault(x => String.Compare(dados[1].Trim(), x.CodInterno.Trim(),
                            StringComparison.InvariantCultureIgnoreCase) == 0) ?? new Produto()).IdProd;

                        if (idProd == 0)
                            throw new Exception("Produto não encontrado: " + dados[1].Trim());

                        var produtoLoja = new ProdutoLoja()
                        {
                            IdLoja = (int)idLoja,
                            IdProd = idProd,
                            QtdEstoque = Glass.Conversoes.StrParaDouble(dados[2].Trim()),
                            EstoqueFiscal = Glass.Conversoes.StrParaDouble(dados[3].Trim() != String.Empty ? dados[3].Trim() : dados[2].Trim())
                        };

                        if (calcM2.Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(idProd)))
                            produtoLoja.M2 = produtoLoja.QtdEstoque;

                        ProdutoLojaDAO.Instance.AtualizaEstoque(produtoLoja);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImpEstoque.Text = log;
        }

        protected void btnImpCheques_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImpCheques.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImpCheques.FileContent, Encoding.Default))
            {
                string line;

                var lojas = LojaDAO.Instance.GetAll();

                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        if (dados.Length != 12)
                            throw new Exception("Dados inválidos: número de informações diferente de 12.");

                        var idLoja = (lojas.FirstOrDefault(x => String.Compare(dados[0].Trim(), x.NomeFantasia.Trim(),
                            StringComparison.InvariantCultureIgnoreCase) == 0) ?? new Loja()).IdLoja;

                        if (idLoja == 0)
                            throw new Exception("Loja não encontrada: " + dados[0].Trim());

                        var cheque = new Cheques()
                        {
                            IdLoja = (uint)idLoja,
                            Num = Glass.Conversoes.StrParaInt(dados[1].Trim()),
                            Banco = dados[2].Trim(),
                            Agencia = dados[3].Trim(),
                            Conta = dados[4].Trim(),
                            Titular = dados[5].Trim(),
                            CpfCnpjFormatado = dados[6].Trim(),
                            Valor = Glass.Conversoes.StrParaDecimal(dados[7].Trim()),
                            DataVenc = Conversoes.ConverteDataNotNull(dados[8].Trim()),
                            Obs = dados[9].Trim(),
                            Tipo = 2, //Glass.Conversoes.StrParaInt(dados[10].Trim()),
                            IdCliente = Glass.Conversoes.StrParaUintNullable(dados[11].Trim()),
                            Situacao = (int)Cheques.SituacaoCheque.EmAberto
                        };

                        ChequesDAO.Instance.InsertExecScript(cheque);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImpCheques.Text = log;
        }

        protected void btnImportarCreditoFornec_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImportarCreditoFornec.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImportarCreditoFornec.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        if (dados.Length != 2)
                            throw new Exception("Dados inválidos: número de informações diferente de 2.");

                        var idFornec = Glass.Conversoes.StrParaUint(dados[0].Trim());
                        var valorAtual = FornecedorDAO.Instance.GetCredito(idFornec);
                        var valorNovo = Glass.Conversoes.StrParaDecimal(dados[1].Trim());

                        FornecedorDAO.Instance.DebitaCredito(null, idFornec, valorAtual);
                        FornecedorDAO.Instance.CreditaCredito(null, idFornec, valorNovo);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImportarCreditoFornec.Text = log;
        }

        protected void btnImportarCreditoCliente_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!filImportarCreditoCliente.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(filImportarCreditoCliente.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');
                        if (dados.Length != 2)
                            throw new Exception("Dados inválidos: número de informações diferente de 2.");

                        var idCliente = Glass.Conversoes.StrParaUint(dados[0].Trim());
                        var valorAtual = ClienteDAO.Instance.GetCredito(idCliente);
                        var valorNovo = Glass.Conversoes.StrParaDecimal(dados[1].Trim());

                        ClienteDAO.Instance.DebitaCredito(idCliente, valorAtual);
                        ClienteDAO.Instance.CreditaCredito(idCliente, valorNovo);
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImportarCreditoCliente.Text = log;
        }

        protected void btnApagarPlanoConta_Click(object sender, EventArgs e)
        {
            string log = "";

            var planoContas = PlanoContasDAO.Instance.GetAll();

            foreach (var pc in planoContas)
            {
                try
                {
                    if (!pc.DeleteVisible)
                        continue;

                    PlanoContasDAO.Instance.Delete(pc);
                }
                catch (Exception ex)
                {
                    log += pc.IdConta + " - " + pc.DescrPlanoGrupo + " -> " + ex.Message + "\n";
                }
            }

            txtApagarPlanoContaLog.Text = log;
        }

        protected void btnApagarGrupoConta_Click(object sender, EventArgs e)
        {
            string log = "";

            var grupoContas = GrupoContaDAO.Instance.GetAll();

            foreach (var gc in grupoContas)
            {
                try
                {
                    if (!gc.DeleteVisible)
                        continue;

                    GrupoContaDAO.Instance.Delete(gc);
                }
                catch (Exception ex)
                {
                    log += gc.IdGrupo + " - " + gc.Descricao + " -> " + ex.Message + "\n";
                }
            }

            txtLogApagarGrupoConta.Text = log;
        }

        protected void btnImpPlanoConta_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupImpPlanoConta.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImpPlanoConta.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                var grupoContas = GrupoContaDAO.Instance.GetAll();

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split(';');

                        var categoriaStr = dados[0].Trim().ToUpper();
                        var grupoConta = dados[1].Trim().ToUpper();
                        var desc = dados[2].Trim().ToUpper();

                        if (string.IsNullOrEmpty(categoriaStr))
                            throw new Exception("Categoria não infomada.");

                        if (string.IsNullOrEmpty(grupoConta))
                            throw new Exception("Grupo de conta não infomado.");

                        if (string.IsNullOrEmpty(desc))
                            throw new Exception("Descrição do grupo não infomada.");

                        var grupo = grupoContas.Where(f => f.Descricao.ToUpper().Trim().Equals(grupoConta)).FirstOrDefault();

                        if (grupo == null)
                            throw new Exception("Grupo de conta não encontrado.");


                        var planoConta = new PlanoContas()
                        {
                            Descricao = desc,
                            IdGrupo = grupo.IdGrupo
                        };

                        PlanoContasDAO.Instance.Insert(planoConta);

                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImpPlanoConta.Text = log;
        }

        protected void btmImpGrupoConta_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupImgGrupoConta.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImgGrupoConta.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                var categoriasContas = CategoriaContaDAO.Instance.GetAll();

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split(';');

                        var categoriaStr = dados[0].Trim().ToUpper();
                        var desc = dados[1].Trim().ToUpper();

                        if (string.IsNullOrEmpty(categoriaStr))
                            throw new Exception("Categoria não infomada.");

                        if (string.IsNullOrEmpty(desc))
                            throw new Exception("Descrição do grupo não infomada.");

                        var categoria = categoriasContas.Where(f => f.Descricao.ToUpper().Trim().Equals(categoriaStr)).FirstOrDefault();

                        if (categoria == null)
                            throw new Exception("Categoria não encontrada.");


                        var grupoConta = new GrupoConta()
                        {
                            IdCategoriaConta = categoria.IdCategoriaConta,
                            Descricao = desc
                        };

                        GrupoContaDAO.Instance.Insert(grupoConta);

                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogImpGrupoConta.Text = log;
        }

        protected void btnCorrigirMovEstoqueFiscalChapas_Click(object sender, EventArgs e)
        {
            var lojas = LojaDAO.Instance.GetAll();

            var idsProd = tempProdutoDAO.Instance.ObtemChapasVidro();
            var produtos = ProdutoDAO.Instance.GetAll().Where(x => idsProd.Contains((uint)x.IdProd));

            var alterados = new List<uint>();

            foreach (var loja in lojas)
            {
                alterados.Clear();

                foreach (var prod in produtos)
                {
                    var movFiscal = MovEstoqueFiscalDAO.Instance.GetForRpt((uint)loja.IdLoja, prod.CodInterno, null, null, null,
                        null, null, 0, 0, 0, 0, 0, 0, 0, 0, false, false);

                    if (movFiscal.Count > 0 && !alterados.Contains((uint)prod.IdProd))
                        alterados.Add((uint)prod.IdProd);

                    foreach (var mov in movFiscal)
                    {
                        mov.IdProd = prod.DadosBaixaEstoqueFiscal.ToArray().FirstOrDefault().Key;

                        if (mov.IdProd > 0 && mov.IdProd != prod.IdProd)
                        {
                            if (!alterados.Contains(mov.IdProd))
                                alterados.Add(mov.IdProd);

                            MovEstoqueFiscalDAO.Instance.Update(mov);
                        }
                    }
                }

                foreach (var idProd in alterados)
                {
                    MovEstoqueFiscalDAO.Instance.AtualizaSaldo(idProd, (uint)loja.IdLoja);
                    tempProdutoDAO.Instance.AtualizaFiscalProdutoLoja(idProd, (uint)loja.IdLoja,
                        MovEstoqueFiscalDAO.Instance.ObtemSaldoQtdeMov(null, idProd, (uint)loja.IdLoja, false));
                }
            }
        }

        protected void btnImpMateriaPrimaLaminado_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupImpMateriaPrimaLaminado.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImpMateriaPrimaLaminado.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split(';');

                        var descProd = dados[0].Trim().ToUpper();

                        if (string.IsNullOrEmpty(descProd))
                            throw new Exception("Produto não infomado.");

                        var idProd = ProdutoDAO.Instance.ObtemIdProd(descProd);

                        if (idProd == 0)
                            throw new Exception("Produto não encontrado.");


                        for (int i = 0; i < line.Trim(';').Split(';').Length; i++)
                        {
                            if (i == 0)
                                continue;

                            var descProdBaixa = line.Split(';')[i];

                            if (string.IsNullOrEmpty(descProdBaixa))
                                throw new Exception("Matéria-prima " + i + " não infomada.");

                            var idProdBaixa = ProdutoDAO.Instance.ObtemIdProd(descProdBaixa);

                            if (idProdBaixa == 0)
                                throw new Exception("Matéria-prima " + i + " não encontrada.");

                            var prodBaixaEstoque = new ProdutoBaixaEstoque()
                            {
                                IdProd = idProd,
                                IdProdBaixa = idProdBaixa,
                                Qtde = 1
                            };

                            ProdutoBaixaEstoqueDAO.Instance.Insert(prodBaixaEstoque);
                        }

                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            txtLogMateriaPrimaLaminado.Text = log;
        }

        protected void btnApagarMovBancoEstorno_Click(object sender, EventArgs e)
        {
            var contasEstorno = new Dictionary<UtilsPlanoConta.PlanoContas, uint>();

            foreach (var v in Enum.GetValues(typeof(UtilsPlanoConta.PlanoContas)))
            {
                try
                {
                    var pc = (UtilsPlanoConta.PlanoContas)v;
                    if (pc.ToString().ToLower().Contains("estorno"))
                        contasEstorno.Add(pc, UtilsPlanoConta.GetPlanoConta(pc));
                }
                catch { }
            }

            var todas = MovBancoDAO.Instance.GetAll().OrderBy(x => x.DataMov).ThenBy(x => x.IdMovBanco);
            var estornos = todas.Where(x => contasEstorno.ContainsValue(x.IdConta) && !x.LancManual).OrderBy(x => x.DataMov).ThenBy(x => x.IdMovBanco);

            var contas = new List<uint>();
            var bancos = new List<uint>();

            foreach (var est in estornos)
            {
                if (!bancos.Contains(est.IdContaBanco))
                    bancos.Add(est.IdContaBanco);

                var original = todas.Where(x => x.IdContaBanco == est.IdContaBanco &&
                    x.TipoMov != est.TipoMov &&
                    x.IdCliente == est.IdCliente &&
                    x.IdFornecedor == est.IdFornecedor &&
                    x.DataMov <= est.DataMov &&
                    !x.LancManual &&
                    !contas.Contains(x.IdMovBanco) && (
                        x.IdAcerto == est.IdAcerto ||
                        x.IdAcertoCheque == est.IdAcertoCheque ||
                        x.IdAntecipContaRec == est.IdAntecipContaRec ||
                        x.IdAntecipFornec == est.IdAntecipFornec ||
                        x.IdCheque == est.IdCheque ||
                        x.IdContaPg == est.IdContaPg ||
                        x.IdContaR == est.IdContaR ||
                        x.IdCreditoFornecedor == est.IdCreditoFornecedor ||
                        x.IdDeposito == est.IdDeposito ||
                        x.IdDepositoNaoIdentificado == est.IdDepositoNaoIdentificado ||
                        x.IdDevolucaoPagto == est.IdDevolucaoPagto ||
                        x.IdLiberarPedido == est.IdLiberarPedido ||
                        x.IdObra == est.IdObra ||
                        x.IdPagto == est.IdPagto ||
                        x.IdPedido == est.IdPedido ||
                        x.IdSinal == est.IdSinal ||
                        x.IdSinalCompra == est.IdSinalCompra ||
                        x.IdTrocaDevolucao == est.IdTrocaDevolucao
                )).FirstOrDefault();

                if (original != null)
                    tempBancoDAO.Instance.ApagarMov(original.IdMovBanco);

                tempBancoDAO.Instance.ApagarMov(est.IdMovBanco);
            }

            foreach (var b in bancos)
                tempBancoDAO.Instance.AtualizaSaldo(b);
        }

        protected void btnCorrigirPecasRoteiro_Click(object sender, EventArgs e)
        {
            var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetAll();
            var leituras = LeituraProducaoDAO.Instance.GetAll();

            foreach (var etiq in etiquetas)
            {
                var setorObrigatorio = SetorDAO.Instance.ObtemSetoresObrigatorios(etiq.IdProdPedProducao).
                    Where(x => x.NumeroSequencia < Data.Helper.Utils.ObtemSetor(etiq.IdSetor).NumeroSequencia).
                    Select(x => x.IdSetor);

                if (setorObrigatorio.Count() == 0)
                {
                    ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(etiq.IdProdPedProducao, null, true);
                    continue;
                }

                var le = leituras.Where(x => x.IdProdPedProducao == etiq.IdProdPedProducao);

                bool inseriu = false;

                DateTime dataLeitura = DateTime.Now;

                foreach (var s in setorObrigatorio)
                {
                    if (le.Count(x => x.IdSetor == s) > 0)
                        continue;

                    try
                    {
                        var nova = new LeituraProducao()
                        {
                            IdProdPedProducao = etiq.IdProdPedProducao,
                            IdSetor = (uint)s,
                            DataLeitura = dataLeitura,
                            IdFuncLeitura = UserInfo.GetUserInfo.CodUser
                        };

                        LeituraProducaoDAO.Instance.Insert(nova);

                        inseriu = true;
                    }
                    catch { }
                }

                if (inseriu)
                    ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(etiq.IdProdPedProducao, dataLeitura, true);
            }

            btnAtualizaSituacaoProducaoPedido_Click(sender, e);
        }

        protected void btnAtualizaSituacaoProducaoPedido_Click(object sender, EventArgs e)
        {
            var pedidos = !String.IsNullOrEmpty(txtIdsPedidosAtuSituacao.Text) ?
                txtIdsPedidosAtuSituacao.Text.Split(',').Select(f => Glass.Conversoes.StrParaUint(f)).ToArray() : null;

            var itens = tempSituacaoProducaoDAO.Instance.ObtemDadosSituacaoProducao(pedidos)
                .Where(f => f.SituacaoProducaoPedido != Glass.Data.Model.Pedido.SituacaoProducaoEnum.NaoEntregue && f.SituacaoProducaoPedido != f.SituacaoProducao).ToList();

            AtualizaSituacaoProducaoPedido(itens.ToArray());

        }

        protected void btnMarcarPecaPronta_Click(object sender, EventArgs e)
        {
            //string log = "";

            //try
            //{
            //    var pedidos = txtIdsPedidosMarcarPecaPronta.Text.Split(',');

            //    if (pedidos.Length == 0)
            //        throw new Exception("Nenhum pedido foi informado.");

            //    foreach (var idPedido in pedidos.Select(f => Glass.Conversoes.StrParaUint(f)))
            //    {
            //        var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByPedido(idPedido);

            //        foreach (var etiqueta in etiquetas)
            //        {
            //            uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta).GetValueOrDefault(0);

            //            if (tempProdutoDAO.Instance.TemLeituraSetor(5, idProdPedProducao))
            //                continue;

            //            ProdutoPedidoProducaoDAO.Instance.AtualizaEstoqueEtiqueta(etiqueta, 5, idPedido, null, null, null);

            //            var nova = new LeituraProducao()
            //            {
            //                IdProdPedProducao = idProdPedProducao,
            //                IdSetor = 5,
            //                DataLeitura = DateTime.Now,
            //                IdFuncLeitura = UserInfo.GetUserInfo.CodUser
            //            };

            //            LeituraProducaoDAO.Instance.Insert(nova);

            //            tempProdutoDAO.Instance.AtualizaSetorPeca(5, idProdPedProducao);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log += "•\t" + ex.Message + "\n";
            //}

            //txtLogMarcarPecaPronta.Text = log;
        }

        protected void btnEnviarEmailCarregamento_Click(object sender, EventArgs e)
        {
            try
            {
                var idsCarregamento = txtEmailCarregamento.Text.Split(',');

                foreach (var id in idsCarregamento.Select(f => Glass.Conversoes.StrParaUint(f)).ToList())
                {
                    Email.EnviaEmailCarregamentoFinalizado(null, id);
                }


            }
            catch (Exception ex)
            {
                txtLogEnvioEmailCarregamento.Text = "â€¢\t" + ex.Message + "\n";
            }
        }

        protected void btnImpPrecoFornec_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupImpPrecoFornec.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (StreamReader reader = new StreamReader(fupImpPrecoFornec.FileContent, Encoding.Default))
            {
                string line;

                int linha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    linha++;

                    try
                    {
                        string[] dados = line.Split(';');

                        var idFornec = Glass.Conversoes.StrParaInt(dados[0]);
                        var idProduto = ProdutoDAO.Instance.ObtemIdProd(dados[1]);
                        var valor = Glass.Conversoes.StrParaDecimal(dados[3]);

                        if (!FornecedorDAO.Instance.Exists(idFornec))
                            throw new Exception("Fornec. não encontrado.");

                        if (idProduto == 0)
                            throw new Exception("Produto não encontrado.");

                        if (valor == 0)
                            throw new Exception("Valor não informado.");

                        var prods = ProdutoFornecedorDAO.Instance.ObtemParaRelatorio((uint)idProduto, (uint)idFornec, null, null);

                        if (prods.Count > 0)
                            throw new Exception("Tem prod");

                        ProdutoFornecedorDAO.Instance.Insert(new ProdutoFornecedor()
                        {
                            IdFornec = idFornec,
                            IdProd = (int)idProduto,
                            CustoCompra = valor
                        });

                    }
                    catch (Exception ex)
                    {
                        log += "• Registro (linha " + linha + "): " + line + "\t" + ex.Message + "\n\n";
                    }
                }
            }

            txtLogImpPrecoFornec.Text = log;
        }

        protected void btnProduzirPedidos_Click(object sender, EventArgs e)
        {
            //string log = "";

            //try
            //{
            //    var idsPedidos = txtIdsPedidos.Text.Split(',').Select(f => Glass.Conversoes.StrParaUint(f)).ToList();

            //    foreach (var idPedido in idsPedidos)
            //    {
            //        if (!PedidoDAO.Instance.Exists(idPedido))
            //            throw new Exception("Pedido " + idPedido + " não encontrado");

            //        if (PedidoDAO.Instance.ObtemSituacao(idPedido) != Glass.Data.Model.Pedido.SituacaoPedido.Conferido)
            //            throw new Exception("Pedido " + idPedido + " não finalizado");

            //        if (PedidoDAO.Instance.ObtemTipoEntrega(idPedido) != (int)Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
            //            throw new Exception("Pedido " + idPedido + " não é balcão");
            //    }

            //    log += "Confirmando pedidos...\n\n";

            //    var pedidosOK = "";
            //    var pedidosErro = "";

            //    PedidoDAO.Instance.ConfirmarLiberacaoPedidoComTransacao(string.Join(",", idsPedidos.Select(f => f.ToString()).ToArray()), out pedidosOK, out pedidosErro, false, false);

            //    if (!string.IsNullOrEmpty(pedidosErro))
            //        throw new Exception("Falha ao confirmar pedidos: " + pedidosErro);

            //    log += "Pedidos confirmados...\n\nGerando Espelhos...\n\n";

            //    foreach (var idPedido in idsPedidos)
            //        PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao(idPedido);

            //    log += "Espelhos gerados...\n\nFinalizando Espelhos...\n\n";

            //    WebGlass.Business.PedidoEspelho.Fluxo.Finalizar.Instance.FinalizarVarios(idsPedidos.Select(f => f.ToString()), false, null);

            //    log += "Espelhos Finalizados...\n\nLendo Setores...\n\n";

            //    foreach (var idPedido in idsPedidos)
            //    {
            //        var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByPedido(idPedido);

            //        foreach (var etiqueta in etiquetas)
            //        {
            //            uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta).GetValueOrDefault(0);

            //            if (tempProdutoDAO.Instance.TemLeituraSetor(5, idProdPedProducao))
            //                continue;

            //            ProdutoPedidoProducaoDAO.Instance.AtualizaEstoqueEtiqueta(etiqueta, 5, idPedido, null, null, null);

            //            var nova = new LeituraProducao()
            //            {
            //                IdProdPedProducao = idProdPedProducao,
            //                IdSetor = 5,
            //                DataLeitura = DateTime.Now,
            //                IdFuncLeitura = UserInfo.GetUserInfo.CodUser
            //            };

            //            LeituraProducaoDAO.Instance.Insert(nova);

            //            tempProdutoDAO.Instance.AtualizaSetorPeca(5, idProdPedProducao);

            //            var etiq = ProdutoPedidoProducaoDAO.Instance.GetElement(idProdPedProducao);
            //            var leituras = LeituraProducaoDAO.Instance.GetByProdPedProducao(idProdPedProducao);

            //            var setorObrigatorio = SetorDAO.Instance.ObtemSetoresObrigatorios(etiq.IdProdPedProducao).
            //            Where(x => x.NumeroSequencia < Data.Helper.Utils.ObtemSetor(etiq.IdSetor).NumeroSequencia).
            //            Select(x => x.IdSetor);

            //            if (setorObrigatorio.Count() == 0)
            //            {
            //                ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(etiq.IdProdPedProducao, null, true);
            //                continue;
            //            }

            //            var le = leituras.Where(x => x.IdProdPedProducao == etiq.IdProdPedProducao);

            //            bool inseriu = false;
            //            DateTime dataLeitura = DateTime.Now;

            //            foreach (var s in setorObrigatorio)
            //            {
            //                if (le.Count(x => x.IdSetor == s) > 0)
            //                    continue;

            //                try
            //                {
            //                    nova = new LeituraProducao()
            //                    {
            //                        IdProdPedProducao = etiq.IdProdPedProducao,
            //                        IdSetor = (uint)s,
            //                        DataLeitura = dataLeitura,
            //                        IdFuncLeitura = UserInfo.GetUserInfo.CodUser
            //                    };

            //                    LeituraProducaoDAO.Instance.Insert(nova);

            //                    inseriu = true;
            //                }
            //                catch { }
            //            }

            //            if (inseriu)
            //                ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(etiq.IdProdPedProducao, dataLeitura, true);
            //        }
            //    }

            //    log += "Setores lidos...\n\n";

            //}
            //catch (Exception ex)
            //{
            //    log += "• " + ex.Message + "\n\n";
            //}

            //txtLogProduzirPedidos.Text = log;
        }

        protected void btnApagarMovEstoqueTrocaDev_Click(object sender, EventArgs e)
        {
            var idsProd = tempMovEstoqueDAO.Instance.ObtemIdsProdutos();
            foreach (var item in idsProd)
                tempMovEstoqueDAO.Instance.ApagarMovimentacoes(item);
        }

        protected void btnGerarEstoqueRealNotas_Click(object sender, EventArgs e)
        {
            string log = "";

            var notas = NotaFiscalDAO.Instance.GetAll();
            var numeros = txtGerarEstoqueRealNotas.Text.Replace(" ", "").
                TrimEnd(',').Split(',').Select(x => Glass.Conversoes.StrParaUint(x));

            foreach (var n in numeros)
            {
                try
                {
                    var nota = notas.Where(x => x.NumeroNFe == n && x.TipoDocumento >= 3).FirstOrDefault();
                    if (nota == null)
                        continue;

                    tempMovEstoqueDAO.Instance.GeraMovimentacoes(nota);
                }
                catch (Exception ex)
                {
                    log += "• " + ex.Message + "\n";
                }
            }

            txtLogGerarEstoqueRealNotas.Text = log;
        }

        protected void btnSepararValores_Click(object sender, EventArgs e)
        {
            string log = "";
            var numerosNfe = txtNumNf.Text.Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).OrderBy(x => x);

            foreach (var num in numerosNfe)
            {
                try
                {
                    var nf = NotaFiscalDAO.Instance.GetByNumeroNFe(num, 2).FirstOrDefault();

                    if (nf == null)
                        nf = NotaFiscalDAO.Instance.GetByNumeroNFe(num, 3).FirstOrDefault();

                    if (nf == null)
                        throw new Exception("Nota fiscal não encontrada");

                    if (nf.FormaPagto == (int)NotaFiscal.FormaPagtoEnum.AVista)
                        throw new Exception("A forma de pagamento da nota informada é à vista");

                    if (nf.TipoDocumento == 2 && !Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasReceber.Instance.SepararComTransacao(nf.IdNf))
                        throw new Exception("Não foram encontradas contas a receber para realizar a separação.");

                    if (nf.TipoDocumento == 3 && !Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasPagar.Instance.SepararComTransacao(nf.IdNf))
                        throw new Exception("Não foram encontradas contas a pagar para realizar a separação.");

                    throw new Exception("Separação feita com sucesso!");
                }
                catch (Exception ex)
                {
                    log += "• NFe " + num + ": " + Glass.MensagemAlerta.FormatErrorMsg("", ex) + "\n";
                }
            }

            txtLogSepararValores.Text = log;
        }

        protected void btnCancelarSepararValores_Click(object sender, EventArgs e)
        {
            string log = "";
            var numerosNfe = txtNumNf.Text.Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).OrderBy(x => x);

            foreach (var num in numerosNfe)
            {
                try
                {
                    var nf = NotaFiscalDAO.Instance.GetByNumeroNFe(num, 2).FirstOrDefault();

                    if (nf == null)
                        nf = NotaFiscalDAO.Instance.GetByNumeroNFe(num, 3).FirstOrDefault();

                    if (nf == null)
                        throw new Exception("Nota fiscal não encontrada");

                    if (nf.FormaPagto == (int)NotaFiscal.FormaPagtoEnum.AVista)
                        throw new Exception("A forma de pagamento da nota informada é à vista");

                    if (nf.TipoDocumento == 2)
                        Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasReceber.Instance.CancelarComTransacao(nf.IdNf);

                    if (nf.TipoDocumento == 3)
                        Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasPagar.Instance.CancelarComTransacao(nf.IdNf);

                    throw new Exception("Cancelamento feito com sucesso!");
                }
                catch (Exception ex)
                {
                    log += "• NFe " + num + ": " + Glass.MensagemAlerta.FormatErrorMsg("", ex) + "\n";
                }
            }

            txtLogSepararValores.Text = log;
        }

        protected void btnAtualizaValorPedido_Click(object sender, EventArgs e)
        {
            string log = "";
            var idsPedidos = txtIdAtualizaValorPedido.Text.Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).OrderBy(x => x);

            foreach (var id in idsPedidos)
            {
                try
                {
                    Glass.Data.DAL.PedidoDAO.Instance.UpdateTotalPedido(id);
                    throw new Exception("atualizado");
                }
                catch (Exception ex)
                {
                    log += "• Pedido " + id + ": " + Glass.MensagemAlerta.FormatErrorMsg("", ex) + "\n";
                }
            }

            txtLogAtualizaValorPedido.Text = log;
        }

        protected void btnBuscarPecasErradas_Click(object sender, EventArgs e)
        {
            string log = "", caminho = Data.Helper.Utils.GetPecaProducaoPath, arquivo = "\\{0}_";
            var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetAll();
            var todosArquivos = Directory.GetFiles(caminho);

            foreach (var prodPed in prodPedEsp)
            {
                try
                {
                    if (prodPed.IdMaterItemProj.GetValueOrDefault() == 0)
                        continue;

                    MaterialItemProjeto material = MaterialItemProjetoDAO.Instance.GetElementByPrimaryKey(prodPed.IdMaterItemProj.Value);
                    uint? idPecaItemProj = material.IdPecaItemProj;

                    if (idPecaItemProj > 0)
                    {
                        string item = PecaItemProjetoDAO.Instance.ObtemItem(idPecaItemProj.Value);
                        var arquivos = todosArquivos.Where(x => x.Contains(String.Format(arquivo, prodPed.IdProdPed))).ToArray();

                        if (arquivos.Length == 0)
                            continue;

                        string previsto = prodPed.IdProdPed + "_" + item;

                        if (arquivos.Length != 1)
                            throw new Exception(String.Format("Arquivos encontrados além do previsto. Previsto: {0}, encontrados: {1}", previsto,
                                String.Join(", ", arquivos)));
                        else if (!arquivos[0].Contains(previsto))
                            throw new Exception(String.Format("Arquivo inválido. Previsto: {0}, encontrado: {1}", previsto, arquivos[0].Split('.')[0]));
                    }
                }
                catch (Exception ex)
                {
                    log += "• Prod. Ped. " + prodPed.IdProdPed + ": " + ex.ToString()/*Glass.MensagemAlerta.FormatErrorMsg("", ex)*/ + "\n";
                }
            }

            txtLogBuscarPecasErradas.Text = log;
        }

        protected void btnVoltarSituaçãoPeca_Click(object sender, EventArgs e)
        {
            string log = "";
            uint idProdPedProducaoLog = 0;

            try
            {
                var idsProdPedProducao = txtIdsProdPedProducao.Text.Split(',').Select(f => Glass.Conversoes.StrParaUint(f)).ToList();

                foreach (var idProdPedProducao in idsProdPedProducao)
                {
                    idProdPedProducaoLog = idProdPedProducao;
                    ProdutoPedidoProducaoDAO.Instance.VoltarPeca(idProdPedProducao, null, true);
                }
            }
            catch (Exception ex)
            {
                log += "• Prod. Ped. Produção: " + idProdPedProducaoLog + "\t" + ex.ToString() + "\n";
            }

            txtLogVoltarSituacaoPeca.Text = log;
        }

        protected void btnImportarSubgrupo_Click(object sender, EventArgs e)
        {
            if (!fupImportarSubgrupo.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            StreamReader reader = new StreamReader(fupImportarSubgrupo.FileContent, Encoding.Default);

            string log = "";
            string line = "";

            string sqlGrupoProd = "SELECT idGrupoProd FROM grupo_prod WHERE descricao like ";
            string sqlSubgrupoProd = "SELECT idSubgrupoProd FROM subgrupo_prod WHERE descricao like ";

            var arquivo = reader.ReadToEnd().Split(new string[] { "$$$" }, StringSplitOptions.None);

            for (int i = 0; i < arquivo.Length; i++)
            {
                line = arquivo[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                    var subgrupo = new List<string>(line.Split('|'));

                    for (int j = 0; j < subgrupo.Count; j++)
                        subgrupo[j] = subgrupo[j].Trim();

                    var grupo = GrupoProdDAO.Instance.GetValoresCampo(sqlGrupoProd + "'" + subgrupo[2] + "'", "idGrupoProd");


                    if (string.IsNullOrEmpty(grupo))
                        throw new Exception("Grupo não encontrado: " + subgrupo[2]);

                    var subgrupoExistente = SubgrupoProdDAO.Instance.GetValoresCampo(sqlSubgrupoProd + "'" + subgrupo[3]
                        + "' AND idGrupoProd=" + grupo, "idSubgrupoProd");

                    if (!string.IsNullOrEmpty(subgrupoExistente))
                        continue;

                    SubgrupoProdDAO.Instance.Insert(new SubgrupoProd()
                    {
                        IdGrupoProd = Glass.Conversoes.StrParaInt(grupo),
                        Descricao = subgrupo[3]
                    });
                }
                catch (Exception ex)
                {
                    log += "• Erro: " + line + "\t" + ex.Message + "\n";
                }
            }

            txtLogImportarSubgrupo.Text = log;
        }

        protected void btnRegistroArquivoRemessa_Click(object sender, EventArgs e)
        {
            string log = "";
            uint idRetorno = 0;


            var arqRemessas = ArquivoRemessaDAO.Instance.GetAll().Where(f => f.Tipo == ArquivoRemessa.TipoEnum.Retorno).ToList();

            foreach (var remessa in arqRemessas)
            {
                idRetorno = remessa.IdArquivoRemessa;

                if (!File.Exists(remessa.CaminhoArquivo))
                    continue;

                var objReader = new StreamReader(remessa.CaminhoArquivo);

                var codBeneficiario = Glass.Conversoes.StrParaUint(objReader.ReadLine().Substring(25, 6));

                uint idContaBanco = 0;

                if (codBeneficiario == 70223)
                    idContaBanco = 41;
                else if (codBeneficiario == 95980)
                    idContaBanco = 43;

                if (idContaBanco == 0)
                    throw new Exception("Conta bancaria não encontrada.");

                var banco = new Banco(ContaBancoDAO.Instance.ObtemCodigoBanco(idContaBanco).Value);

                var retorno = new ArquivoRetornoCNAB400();
                retorno.LerArquivoRetorno(banco, new MemoryStream(File.ReadAllBytes(remessa.CaminhoArquivo)));

                foreach (var d in retorno.ListaDetalhe)
                {
                    try
                    {
                        var numDocCnab = "";

                        uint idContaR = 0;

                        try
                        {
                            idContaR = ContasReceberDAO.Instance.GetIdByNumeroDocumentoCnab(banco.Codigo, d.NumeroDocumento,
                               d.NossoNumero + d.DACNossoNumero, d.UsoEmpresa, out numDocCnab);
                        }
                        catch { }

                        if (idContaR == 0)
                            continue;

                        //RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab400(remessa.IdArquivoRemessa, idContaR, idContaBanco, d);
                    }
                    catch (Exception ex)
                    {
                        log += "• Carregamento: " + idRetorno + "\t" + ex.Message + "\n";
                    }
                }
            }


            txtLogRegistroArquivoRemessa.Text = log;
        }

        protected void btnAtualizarProducaoPedido_Click(object sender, EventArgs e)
        {
            var idsProdPedProducao = tempProdutoPedidoProducaoDAO.Instance.ObtemIdsProdPedProducao();
            //var idsProdPedProducao = new List<uint>() { 29703 };

            var numThread = idsProdPedProducao.Count >= 20000 ? (idsProdPedProducao.Count / 20000) + 1 : 1;

            for (int i = 0; i < numThread; i++)
            {
                var p = new ParameterizedThreadStart(f => AtualizaSetorSituacaoProdutoProducao((List<uint>)f));
                var t = new System.Threading.Thread(p);
                t.Start(idsProdPedProducao.Skip(20000 * i).Take(20000).ToList());
            }
        }

        private void AtualizaSituacaoProducaoPedido(params tempSituacaoProducaoDAO.Model[] pedidos)
        {
            ErroDAO.Instance.InserirFromException("AtualizaSituacaoProducaoPedido - Início", new Exception(""));

            for (int i = 0; i < pedidos.Length; i++)
            {
                try
                {
                    tempSituacaoProducaoDAO.Instance.AtualizaSituacaoProducao(pedidos[i]);
                }
                catch (Exception ex)
                {
                    var urlErro = "AtualizaSituacaoProducaoPedido - IdPedido: " + pedidos[i].IdPedido;
                    ErroDAO.Instance.InserirFromException(urlErro, ex);
                }
            }

            ErroDAO.Instance.InserirFromException("AtualizaSetorSituacaoProdutoProducao - Fim", new Exception(""));
        }

        private void AtualizaSetorSituacaoProdutoProducao(List<uint> idsProdPedProducao)
        {
            ErroDAO.Instance.InserirFromException("AtualizaSetorSituacaoProdutoProducao - Início", new Exception(""));

            uint idProdPedProducao = 0;

            for (int i = 0; i < idsProdPedProducao.Count; i++)
            {
                try
                {
                    idProdPedProducao = idsProdPedProducao[i];

                    var leituras = LeituraProducaoDAO.Instance.GetByProdPedProducao(idProdPedProducao);

                    if (leituras.Count == 0)
                        continue;

                    var corrigiuRoteiro = CorrigirPecasRoteiro(idProdPedProducao, leituras);

                    if (corrigiuRoteiro)
                        leituras = LeituraProducaoDAO.Instance.GetByProdPedProducao(idProdPedProducao);

                    AtualizaSetorProdutoPedidoProducao(idProdPedProducao, leituras);
                }
                catch (Exception ex)
                {
                    var urlErro = "AtualizaSetorSituacaoProdutoProducao - PPP: " + idProdPedProducao;
                    ErroDAO.Instance.InserirFromException(urlErro, ex);
                }
            }

            ErroDAO.Instance.InserirFromException("AtualizaSetorSituacaoProdutoProducao - Fim", new Exception(""));
        }

        private bool CorrigirPecasRoteiro(uint idProdPedProducao, IList<LeituraProducao> le)
        {
            var idSetor = ProdutoPedidoProducaoDAO.Instance.ObtemIdSetor(idProdPedProducao);

            var setoresObrigatorios = SetorDAO.Instance.ObtemSetoresObrigatorios(idProdPedProducao)
                .Where(x => x.NumeroSequencia < Data.Helper.Utils.ObtemSetor(idSetor).NumeroSequencia)
                .Select(x => x.IdSetor).ToArray();

            if (setoresObrigatorios.Length == 0)
                return false;

            var corrigiuRoteiro = false;

            for (int i = 0; i < setoresObrigatorios.Length; i++)
            {
                if (le.Count(x => x.IdSetor == setoresObrigatorios[i]) > 0)
                    continue;

                var dataLeitura = le.Max(f => f.DataLeitura);

                if (!dataLeitura.HasValue)
                    dataLeitura = DateTime.Now;


                var nova = new LeituraProducao()
                {
                    IdProdPedProducao = idProdPedProducao,
                    IdSetor = (uint)setoresObrigatorios[i],
                    DataLeitura = dataLeitura,
                    IdFuncLeitura = 1
                };

                LeituraProducaoDAO.Instance.Insert(nova);

                corrigiuRoteiro = true;
            }

            return corrigiuRoteiro;
        }

        private void AtualizaSetorProdutoPedidoProducao(uint idProdPedProducao, IList<LeituraProducao> le)
        {
            var leituras = (from f in le
                            select new
                            {
                                Setor = Data.Helper.Utils.ObtemSetor(f.IdSetor)
                            }).ToList();

            var idSetor = leituras.OrderByDescending(f => f.Setor.NumeroSequencia).Select(f => f.Setor.IdSetor).FirstOrDefault();

            if (idSetor == 0)
                throw new Exception("Setor não encontrado. " + idProdPedProducao);

            tempProdutoPedidoProducaoDAO.Instance.AtualizaSetor(idProdPedProducao, idSetor);
        }

        public sealed class AjusteEstoqueNFe : BaseDAO<NotaFiscal, AjusteEstoqueNFe>
        {
            public void AtualizaEstoque(uint idNf)
            {
                // Busca produtos da nota
                ProdutosNf[] lstProd = ProdutosNfDAO.Instance.GetByNf(idNf);
                NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

                // Se for entrada e ainda não tiver dado entrada no estoque, credita estoque fiscal
                if (nf.TipoDocumento == 1)
                {
                    if (nf.EntrouEstoque == false)
                    {
                        foreach (ProdutosNf p in lstProd)
                            MovEstoqueFiscalDAO.Instance.CreditaEstoqueNotaFiscal(p.IdProd, nf.IdLoja.Value,
                                p.IdCfop > 0 ? p.IdCfop.Value : nf.IdCfop.Value, p.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(p, true), false, false);

                        objPersistence.ExecuteCommand("Update nota_fiscal Set entrouEstoque=true Where idNf=" + nf.IdNf);
                    }
                }
                // Se for saída e ainda não tiver dado saída no estoque, baixa estoque fiscal
                else if (nf.TipoDocumento == 2)
                {
                    if (nf.SaiuEstoque == false)
                    {
                        foreach (ProdutosNf p in lstProd)
                        {
                            MovEstoqueFiscalDAO.Instance.BaixaEstoqueNotaFiscal(p.IdProd, nf.IdLoja.Value,
                                p.IdCfop > 0 ? p.IdCfop.Value : nf.IdCfop.Value, p.IdNf, p.IdProdNf,
                                (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(p, true), false);

                            // Altera o estoque real dos produtos
                            if (nf.GerarEstoqueReal)
                            {
                                bool m2 = GrupoProdDAO.Instance.TipoCalculo((int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)TipoCalculoGrupoProd.M2 ||
                                    GrupoProdDAO.Instance.TipoCalculo((int)p.IdGrupoProd, (int)p.IdSubgrupoProd) == (int)TipoCalculoGrupoProd.M2Direto;

                                MovEstoqueDAO.Instance.BaixaEstoqueNotaFiscal(p.IdProd, nf.IdLoja.Value, nf.IdNf, p.IdProdNf,
                                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(p));

                                objPersistence.ExecuteCommand("Update produtos_nf Set qtdeSaida=" + ProdutosNfDAO.Instance.ObtemQtdDanfe(p).ToString().Replace(",", ".") +
                                    " Where idProdNf=" + p.IdProdNf);
                            }
                        }

                        objPersistence.ExecuteCommand("Update nota_fiscal Set saiuEstoque=true Where idNf=" + nf.IdNf);
                    }
                }
            }
        }

        public void btnAlterarEstoque_Click(object sender, EventArgs e)
        {
            // Busca produtos da nota
            AjusteEstoqueNFe.Instance.AtualizaEstoque(Conversoes.StrParaUint(txtIdNf.Text));
        }

        protected void btnAtualizaItensRevendaCarregamento_Click(object sender, EventArgs e)
        {
            var log = "";

            var pedidos = tempPedidoDAO.Instance.GetPedidosOC();
            var idsPedidos = string.Join(",", pedidos.Select(f => f.ToString()).ToArray());

            var itensRevenda = ProdutosPedidoDAO.Instance.GetByPedidosForExpCarregamento(null, idsPedidos, true, false);


            var itens =
                (from f in itensRevenda
                 group f by new
                 {
                     f.IdPedido,
                     f.IdProd
                 } into gcs
                 select new
                 {
                     IdPedido = gcs.Key.IdPedido,
                     IdProd = gcs.Key.IdProd,
                     Qtde = gcs.Sum(f => f.Qtde)
                 }).ToList();

            var ocs = itens.GroupBy(f => f.IdPedido).Select(f => f.First()).Select(f => new
            {
                IdPedido = f.IdPedido,
                IdsOcs = PedidoOrdemCargaDAO.Instance.GetIdsOCsByPedidos(null, f.IdPedido.ToString())
            }).ToList();

            for (int i = 0; i < itens.Count; i++)
            {
                try
                {
                    var idsOcs = ocs.Where(f => f.IdPedido == itens[i].IdPedido).Select(f => f.IdsOcs)
                        .FirstOrDefault().Split(',').Select(f => Conversoes.StrParaUint(f)).ToList();

                    var carregamentos = new List<uint>();

                    for (int x = 0; x < idsOcs.Count; x++)
                    {
                        carregamentos.Add(OrdemCargaDAO.Instance.GetIdCarregamento(idsOcs[x]).GetValueOrDefault(0));
                    }

                    for (int x = 0; x < carregamentos.Count; x++)
                    {
                        var itensCarregamento = tempItemCarregamentoDAO.Instance.GetByPedidoProduto(itens[i].IdPedido, itens[i].IdProd, carregamentos[x]);

                        if (itensCarregamento.Count == 0)
                        {
                            for (int j = 0; j < itens[i].Qtde; j++)
                            {
                                ItemCarregamentoDAO.Instance.Insert(new ItemCarregamento()
                                {
                                    IdCarregamento = carregamentos[x],
                                    IdPedido = itens[i].IdPedido,
                                    IdProd = itens[i].IdProd,
                                    Carregado = false,
                                    Entregue = false
                                });
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    log += ex.Message + Environment.NewLine;
                }
            }

            txtLogAtualizaItensRevendaCarregamento.Text = log;
        }

        protected void btnCorrigirProducaoCarregamento_Click(object sender, EventArgs e)
        {
            string log = "";
            var idFunc = UserInfo.GetUserInfo.CodUser;
            var idSetorCarregamento = SetorDAO.Instance.ObtemIdSetorExpCarregamento();
            var itens = ItemCarregamentoDAO.Instance.GetAll().Where(f => f.IdProdPedProducao.GetValueOrDefault(0) > 0 && f.Carregado).ToList();
            var idsPPP = itens.Select(x => x.IdProdPedProducao).ToList();
            var ppps = ProdutoPedidoProducaoDAO.Instance.GetAll()
                .Where(f => idsPPP.Contains(f.IdProdPedProducao) && (f.IdSetor != idSetorCarregamento || f.SituacaoProducao != (int)SituacaoProdutoProducao.Entregue)).ToList();

            for (int i = 0; i < itens.Count; i++)
            {
                ItemCarregamento item = null;

                try
                {
                    item = itens[i];

                    var ppp = ppps.Where(f => f.IdProdPedProducao == item.IdProdPedProducao.Value).FirstOrDefault();

                    if (ppp == null)
                        continue;

                    var pedidoExpedicao = item.IdPedido != Conversoes.StrParaUint(ppp.NumEtiqueta.Split('-')[0]);

                    if (pedidoExpedicao)
                        ItemCarregamentoDAO.Instance.EstornaAtualizaItemRevenda(item.IdCarregamento, item.IdPedido, ppp.NumEtiqueta, false);

                    ItemCarregamentoDAO.Instance.DeletaLeitura(item.IdCarregamento, ppp.NumEtiqueta);

                    WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.EfetuaLeitura(idFunc, item.IdCarregamento, ppp.NumEtiqueta, pedidoExpedicao ? (uint?)item.IdPedido : null, null, null,
                        null, null, null, null, null, 0, null, 0);

                    throw new Exception(ppp.NumEtiqueta);
                }

                catch (Exception ex)
                {
                    log += "-- " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;
                    log += "-- " + item.IdItemCarregamento + Environment.NewLine;
                }
            }

            txtLogCorrigirProducaoCarregamento.Text = log;
        }

        protected void btnCorrigirSituacaoPecas_Click(object sender, EventArgs e)
        {
            //var pedido = txtPedidoCorrigirSituacaoPecas.Text.StrParaUintNullable();

            //if (pedido.HasValue && pedido.Value > 0)
            //{
            //    var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByPedido(pedido.Value);
            //    foreach (var et in etiquetas)
            //        ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoPecaNaProducao(et, null, false);

            //    var dataPronto = tempProdutoPedidoProducaoDAO.Instance.ObtemDataPronto(pedido.Value);
            //    PedidoDAO.Instance.AtualizaSituacaoProducao(pedido.Value, null, dataPronto ?? DateTime.Now);
            //}
        }

        protected void btnEstoqueInicialCliente_Click(object sender, EventArgs e)
        {
            var ids = tempMovEstoqueClienteDAO.Instance.ObtemIdMovEstoqueIni();

            string log = "";

            foreach (var i in ids)
            {
                try
                {
                    MovEstoqueClienteDAO.Instance.AtualizaSaldo(i);
                }
                catch (Exception ex)
                {

                    log += "• Id: " + i + " - " + ex.Message + Environment.NewLine;
                }

            }

            txtLogEstoqueInicialCliente.Text = log;
        }

        protected void btnApagarContas_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupApagarContas.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            var idLoja = Glass.Conversoes.StrParaUint(drpIdLojaApagarContas.SelectedValue);
            var idLiberarPedido = idLoja == 1 ? 15 : 16;

            var contasArq = new List<ContasReceber>();
            var contasSis = tempContasReceberDAO.Instance.GetList(idLoja);

            using (StreamReader reader = new StreamReader(fupApagarContas.FileContent, Encoding.Default))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (String.IsNullOrEmpty(line))
                            continue;

                        string[] dados = line.Split('|');

                        var cli = ClienteDAO.Instance.GetByCpfCnpj(dados[0].Trim());

                        if (cli == null)
                            throw new Exception("Cliente não encontrado. " + dados[0].Trim());

                        contasArq.Add(new ContasReceber()
                        {
                            IdLoja = idLoja,
                            IdLiberarPedido = (uint?)idLiberarPedido,
                            IdCliente = (uint)cli.IdCli,
                            DataVec = Conversoes.ConverteDataNotNull(dados[1].Trim()),
                            ValorVec = Glass.Conversoes.StrParaDecimal(dados[2].Trim().Replace(".", ",")),
                            NumParc = Glass.Conversoes.StrParaInt(dados[3].Trim()),
                            Obs = dados[5].Trim(),
                            TipoConta = (byte)(ContasReceber.TipoContaEnum.NaoContabil),
                        });
                    }
                    catch (Exception ex)
                    {
                        log += "• Registro: " + line + "\t" + ex.Message + "\n";
                    }
                }
            }

            var contasApagar = new List<uint>();

            foreach (var c in contasSis)
            {
                var qtdeContas = contasArq
                    .Where
                    (
                        f => f.IdLoja == c.IdLoja &&
                            f.IdLiberarPedido == c.IdLiberarPedido &&
                            f.IdCliente == c.IdCliente &&
                            f.DataVec == c.DataVec &&
                            f.ValorVec == c.ValorVec
                    ).Count();

                if (qtdeContas <= 0)
                {
                    log += "• Removida - IdCliente: " + c.IdCliente + " DataVec: " + c.DataVec + " Valor: " + c.ValorVec + "Parc: " + c.NumParc + " OBS: " + c.Obs + "\n";
                    contasApagar.Add(c.IdContaR);
                }
            }

            tempContasReceberDAO.Instance.Apagar(string.Join(",", contasApagar.Select(f => f.ToString()).ToArray()));

            txtLogApagarContas.Text = log;
        }

        protected void btnEstoquePosicaoMateriaPrima_Click(object sender, EventArgs e)
        {
            var log = "";

            var posicao = Glass.Data.RelDAL.PosicaoMateriaPrimaDAO.Instance.GetPosMateriaPrima(null, null, null, null, null, null, null, false);

            posicao = posicao.Where(f => f.Chapas.Count > 0 || f.TotM2Estoque > 0).ToList();

            var prodsEstoque = new Dictionary<int, decimal>();

            foreach (var p in posicao)
            {
                foreach (var c in p.Chapas)
                {
                    if (!prodsEstoque.ContainsKey(c.IdProdBase))
                        prodsEstoque.Add(c.IdProdBase, c.TotalM2Chapa);
                    else
                        prodsEstoque[c.IdProdBase] = prodsEstoque[c.IdProdBase] + c.TotalM2Chapa;
                }
            }

            foreach (var prod in prodsEstoque)
            {
                try
                {
                    var saldo = MovEstoqueDAO.Instance.ObtemSaldoQtdeMov(null, (uint)prod.Key, 1, false);
                    var qtdeAlterar = saldo - prod.Value;

                    if (qtdeAlterar > 0)
                        MovEstoqueDAO.Instance.BaixaEstoqueManual((uint)prod.Key, 1, qtdeAlterar, null, DateTime.Now, "Ajuste de acordo com Posição de Matéria-Prima");
                    else if (qtdeAlterar < 0)
                        MovEstoqueDAO.Instance.CreditaEstoqueManual((uint)prod.Key, 1, Math.Abs(qtdeAlterar), null, DateTime.Now, "Ajuste de acordo com Posição de Matéria-Prima");

                }
                catch (Exception ex)
                {
                    log += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "");
                }
            }

            txtLogEstoquePosicaoMateriaPrima.Text = log;
        }

        protected void btnCriarPagtoContasRecebidas_Click(object sender, EventArgs e)
        {
            var contas = ContasReceberDAO.Instance.GetAll().Where(f => f.Recebida).ToList();
            var lstCxDiario = CaixaDiarioDAO.Instance.GetAll();
            var lstCxGeral = CaixaGeralDAO.Instance.GetAll();
            var idsContas = PlanoContasDAO.Instance.GetAll().Select(f => f.IdConta).ToList();
            var idsContasComPagto = PagtoContasReceberDAO.Instance.GetAll().Select(f => f.IdContaR).Distinct().ToList();

            contas = contas.Where(f => !idsContasComPagto.Contains(f.IdContaR)).ToList();

            var dicPlanoContaFormaPagto = new Dictionary<int, uint>();

            foreach (var idConta in idsContas)
            {
                var formaPagto = UtilsPlanoConta.GetFormaPagtoByIdConta((uint)idConta);
                if (formaPagto != null)
                    dicPlanoContaFormaPagto.Add(idConta, formaPagto.IdFormaPagto.GetValueOrDefault(0));
            }

            var log = "";
            var lstPagtoInserir = new List<PagtoContasReceber>();

            #region Acerto

            foreach (var acertoGroup in contas.Where(f => f.IdAcerto.GetValueOrDefault(0) > 0).GroupBy(f => f.IdAcerto))
            {
                var cxGeral = lstCxGeral
                        .Where(f => f.IdAcerto == acertoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                var cxDiario = lstCxDiario
                     .Where(f => f.IdAcerto == acertoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                     .ToList();

                log = InserirPagtoContasR(log, null, acertoGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region Obra

            foreach (var obraGroup in contas.Where(f => f.IdObra.GetValueOrDefault(0) > 0).GroupBy(f => f.IdObra))
            {

                var cxGeral = lstCxGeral
                    .Where(f => f.IdObra == obraGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                    .ToList();
                var cxDiario = lstCxDiario
                        .Where(f => f.IdObra == obraGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                log = InserirPagtoContasR(log, null, obraGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region NF-e

            foreach (var nfGroup in contas.Where(f => f.IdNf.GetValueOrDefault(0) > 0 && f.IdAcerto.GetValueOrDefault(0) == 0).GroupBy(f => f.IdNf))
            {
                try
                {
                    var contasRecNf = nfGroup.ToList();

                    for (int i = 0; i < contasRecNf.Count; i++)
                    {
                        var cxGeral = lstCxGeral
                            .Where(f => f.IdContaR == contasRecNf[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();
                        var cxDiario = lstCxDiario
                            .Where(f => f.IdContaR == contasRecNf[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();

                        var total = cxGeral.Sum(f => f.ValorMov) + cxDiario.Sum(f => f.Valor);

                        foreach (var cxG in cxGeral)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxG.IdConta];

                            var perc = (contasRecNf[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRecNf[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxG.ValorMov;

                            //PagtoContasReceberDAO.Instance.Insert(session, pagto);
                            lstPagtoInserir.Add(pagto);
                        }

                        foreach (var cxD in cxDiario)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxD.IdConta];

                            var perc = (contasRecNf[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRecNf[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxD.Valor;

                            lstPagtoInserir.Add(pagto);
                            //PagtoContasReceberDAO.Instance.Insert(session, pagto);
                        }
                    }

                }
                catch (Exception ex)
                {
                    log += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;
                }
            }

            #endregion

            #region Pedido

            foreach (var pedidoGroup in contas.Where(f => f.IdPedido.GetValueOrDefault(0) > 0
                && f.IdAcerto.GetValueOrDefault(0) == 0 && f.IdAcertoParcial.GetValueOrDefault(0) == 0).GroupBy(f => f.IdPedido))
            {

                var cxGeral = lstCxGeral
                    .Where(f => f.IdPedido == pedidoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                    .ToList();
                var cxDiario = lstCxDiario
                        .Where(f => f.IdPedido == pedidoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                log = InserirPagtoContasR(log, null, pedidoGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region Liberação

            foreach (var liberacaoGroup in contas.Where(f => f.IdLiberarPedido.GetValueOrDefault(0) > 0
                && f.IdAcerto.GetValueOrDefault(0) == 0 && f.IdAcertoParcial.GetValueOrDefault(0) == 0).GroupBy(f => f.IdLiberarPedido))
            {
                var cxGeral = lstCxGeral
                    .Where(f => f.IdLiberarPedido == liberacaoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                    .ToList();
                var cxDiario = lstCxDiario
                        .Where(f => f.IdLiberarPedido == liberacaoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                log = InserirPagtoContasR(log, null, liberacaoGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region Sinal

            foreach (var sinalGroup in contas.Where(f => f.IdSinal.GetValueOrDefault(0) > 0).GroupBy(f => f.IdSinal))
            {
                var cxGeral = lstCxGeral
                    .Where(f => f.IdSinal == sinalGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                    .ToList();
                var cxDiario = lstCxDiario
                        .Where(f => f.IdSinal == sinalGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                log = InserirPagtoContasR(log, null, sinalGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region Acerto Parcial

            foreach (var acertoGroup in contas.Where(f => f.IdAcertoParcial.GetValueOrDefault(0) > 0 && f.IdAcerto.GetValueOrDefault(0) == 0).GroupBy(f => f.IdAcertoParcial))
            {
                try
                {
                    var contasRec = acertoGroup.ToList();

                    for (int i = 0; i < contasRec.Count; i++)
                    {
                        var cxGeral = lstCxGeral
                            .Where(f => f.IdContaR == contasRec[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();
                        var cxDiario = lstCxDiario
                            .Where(f => f.IdContaR == contasRec[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();

                        var total = cxGeral.Sum(f => f.ValorMov) + cxDiario.Sum(f => f.Valor);

                        foreach (var cxG in cxGeral)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxG.IdConta];

                            var perc = (contasRec[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRec[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxG.ValorMov;

                            //PagtoContasReceberDAO.Instance.Insert(session, pagto);
                            lstPagtoInserir.Add(pagto);
                        }

                        foreach (var cxD in cxDiario)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxD.IdConta];

                            var perc = (contasRec[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRec[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxD.Valor;

                            //PagtoContasReceberDAO.Instance.Insert(session, pagto);
                            lstPagtoInserir.Add(pagto);
                        }
                    }

                }
                catch (Exception ex)
                {
                    log += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;
                }
            }

            #endregion

            #region Troca/Devolução

            #endregion

            #region Devolução de pagamento

            #endregion

            #region Acerto Cheque

            #endregion

            #region Insere os Pagamentos

            var sqlBase = "INSERT INTO pagto_contas_receber (IdContaR, IdFormaPagto, ValorPagto) VALUES {0};";
            var valoresInserir = new List<string>();
            int count = 0;

            for (int i = 0; i < lstPagtoInserir.Count; i++)
            {
                var p = lstPagtoInserir[i];

                valoresInserir.Add("(" + p.IdContaR + "," + p.IdFormaPagto + "," + p.ValorPagto.ToString().Replace(',', '.') + ")");
                count++;

                if (count >= 1000 || i + 1 == lstPagtoInserir.Count)
                {
                    var sql = string.Format(sqlBase, string.Join(",", valoresInserir.ToArray()));
                    PagtoContasReceberDAO.Instance.ApagarInserirPagamentosAntigos(sql);
                    valoresInserir = new List<string>();
                    count = 0;
                }
            }

            #endregion

            txtImpCriarPagtoContasRecebeidas.Text = log;
        }

        protected void btnClientesCreditoIncorreto_Click(object sender, EventArgs e)
        {
            var retorno = new List<string>();
            var dataInicio = DateTime.Parse("2000-01-01");
            var dataFim = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 23:59:59"));

            foreach (var cliente in ClienteDAO.Instance.GetAll())
            {
                var creditoAtual = ClienteDAO.Instance.GetCredito((uint)cliente.IdCli);

                Data.RelDAL.CreditoDAO.TotaisCredito totais =
                    Data.RelDAL.CreditoDAO.Instance.GetTotaisCredito((uint)cliente.IdCli, dataInicio, dataFim, null);

                if (totais.Gerado - totais.Utilizado >= 0 && creditoAtual == totais.Gerado - totais.Utilizado)
                    continue;

                retorno.Add(string.Format("Cliente: {0}-{1} | Crédito atual: {2} | Saldo crédito movimentado: {3}",
                    cliente.IdCli, cliente.Nome, creditoAtual.ToString("C"), (totais.Gerado - totais.Utilizado).ToString("C")));
            }

            txtClientesCreditoIncorreto.Text = string.Join("\n", retorno);
        }

        private static string InserirPagtoContasR(string log, GDASession session,
            IGrouping<uint?, ContasReceber> contasGroup, List<CaixaGeral> cxGeral, List<CaixaDiario> cxDiario,
            Dictionary<int, uint> dicFormasPagto, ref List<PagtoContasReceber> lstPagtoInserir)
        {
            try
            {
                var contasRec = contasGroup.ToList();
                var total = cxGeral.Sum(f => f.ValorMov) + cxDiario.Sum(f => f.Valor);
                var totalPorFormaPagamento = new Dictionary<int, decimal>();
                var totalPorFormaPagamentoAux = new Dictionary<int, decimal>();

                // Salva o valor pagto por forma de pagamento em cada movimentação do caixa geral.
                foreach (var cxG in cxGeral)
                {
                    if (totalPorFormaPagamento.ContainsKey((int)dicFormasPagto[(int)cxG.IdConta]))
                    {
                        totalPorFormaPagamento[(int)dicFormasPagto[(int)cxG.IdConta]] += cxG.ValorMov;
                        totalPorFormaPagamentoAux[(int)dicFormasPagto[(int)cxG.IdConta]] += cxG.ValorMov;
                    }
                    else
                    {
                        totalPorFormaPagamento.Add((int)dicFormasPagto[(int)cxG.IdConta], cxG.ValorMov);
                        totalPorFormaPagamentoAux.Add((int)dicFormasPagto[(int)cxG.IdConta], cxG.ValorMov);
                    }
                }

                // Salva o valor pagto por forma de pagamento em cada movimentação do caixa diário.
                foreach (var cxD in cxDiario)
                {
                    if (totalPorFormaPagamento.ContainsKey((int)dicFormasPagto[(int)cxD.IdConta]))
                    {
                        totalPorFormaPagamento[(int)dicFormasPagto[(int)cxD.IdConta]] += cxD.Valor;
                        totalPorFormaPagamentoAux[(int)dicFormasPagto[(int)cxD.IdConta]] += cxD.Valor;
                    }
                    else
                    {
                        totalPorFormaPagamento.Add((int)dicFormasPagto[(int)cxD.IdConta], cxD.Valor);
                        totalPorFormaPagamentoAux.Add((int)dicFormasPagto[(int)cxD.IdConta], cxD.Valor);
                    }
                }

                // Salva as formas de pagto de cada conta do acerto.
                for (int i = 0; i < contasRec.Count(); i++)
                {
                    if (total == 0)
                        continue;

                    var c = contasRec[i];
                    var ultConta = i + 1 == contasRec.Count();

                    // Obtém o percentual que esta conta representa em todo o acerto, é necessário remover o valor de juros para que
                    // o percentual da conta seja calculado corretamente..
                    var perc = contasRec.Count() == 1 ? 100 : (c.ValorRec * 100) / (total - (cxDiario.Sum(f => f.Juros) + cxGeral.Sum(f => f.Juros)));

                    foreach (var idFormaPagto in totalPorFormaPagamento.Keys)
                    {
                        if (totalPorFormaPagamento[idFormaPagto] == 0)
                            continue;

                        var pagto = new PagtoContasReceber();

                        pagto.IdContaR = c.IdContaR;
                        pagto.IdFormaPagto = (uint)idFormaPagto;

                        // Se for a última conta, salva o valor restante do pagto.
                        pagto.ValorPagto = ultConta ? totalPorFormaPagamentoAux[idFormaPagto] : (totalPorFormaPagamento[idFormaPagto] * perc) / 100;
                        totalPorFormaPagamentoAux[idFormaPagto] -= pagto.ValorPagto;

                        lstPagtoInserir.Add(pagto);
                    }
                }
            }
            catch (Exception ex)
            {
                log += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;
            }

            return log;
        }

        protected void btnImporarValorCentroCusto_Click(object sender, EventArgs e)
        {
            string log = "";

            if (!fupImportarValorCentroCusto.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            using (var ms = new MemoryStream(fupImportarValorCentroCusto.FileBytes))
            {
                var arquivo = new HSSFWorkbook(ms);
                var planilha = arquivo.GetSheetAt(0);

                for (int i = 1; i < planilha.LastRowNum; i++)
                {
                    try
                    {
                        var row = planilha.GetRow(i);

                        var codInterno = row.Cells[0].ToString();
                        var valor = row.Cells[2].ToString().Trim('*').StrParaDecimal();

                        var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
                        var idLoja = (int)UserInfo.GetUserInfo.IdLoja;

                        if (idProd == 0)
                            throw new Exception("Produto não encontrado.");

                        if (valor == 0)
                            throw new Exception("Não foi informado valor para o produto: " + codInterno);

                        var saldoQtde = MovEstoqueDAO.Instance.ObtemSaldoQtdeMov(null, 0, (uint)idProd, (uint)idLoja, false);

                        saldoQtde = saldoQtde == 0 ? 1 : saldoQtde;

                        var mov = new MovEstoqueCentroCusto();

                        mov.IdProd = idProd;
                        mov.TipoMov = MovEstoqueCentroCusto.TipoMovEnum.Entrada;
                        mov.IdLoja = idLoja;
                        mov.QtdeMov = 1;
                        mov.ValorMov = valor;

                        mov.SaldoQtdeMov = saldoQtde;
                        mov.SaldoValorMov = valor * saldoQtde;

                        mov.IdFunc = (int)UserInfo.GetUserInfo.CodUser;
                        mov.DataMov = DateTime.Now;

                        MovEstoqueCentroCustoDAO.Instance.Insert(mov);

                    }
                    catch (Exception ex)
                    {

                        log += ex.Message + Environment.NewLine;
                    }
                }
            }

            txtLogImportarValorCentroCusto.Text = log;
        }

        protected void btnGerarSqlLimparBanco_Click(object sender, EventArgs e)
        {
            var nomeBanco = string.Empty;

            try
            {
                nomeBanco = DBUtils.GetDBName;
            }
            catch (NullReferenceException ex)
            {
                MensagemAlerta.ErrorMsg("Verifique se existe somente uma configuração (connectionstring) no web.config com o nome do banco de dados." +
                    " Remova a configuração comentada e tente novamente. Erro: ", ex, Page);
                return;
            }

            var comando = tempSqlLimparBancoDAO.Instance.ObtemSqlLimparBanco(nomeBanco);

            txtGerarSqlLimparBanco.Text = comando;
        }

        protected void btnAtualizarNomeImagensProjetoModelo_Click(object sender, EventArgs e)
        {
            tempProjetoModelo.Instance.AtualizaNomeImagensProjetoModelo();
        }

        /// <summary>
        /// Ajuste criado para resolver casos antigos do chamado 50768.
        /// </summary>
        protected void btnAjustePagtoContasReceberDeAcerto_Click(object sender, EventArgs e)
        {
            var contas = ContasReceberDAO.Instance.ObterContasRecebidasParaAjustePagtoContasReceber();
            var lstCxDiario = CaixaDiarioDAO.Instance.ObterMovimentacoesPorData("2016-12-09", "2017-05-09");
            var lstCxGeral = CaixaGeralDAO.Instance.ObterMovimentacoesPorData("2016-12-09", "2017-05-09");
            var idsContas = PlanoContasDAO.Instance.GetAll().Select(f => f.IdConta).ToList();

            var dicPlanoContaFormaPagto = new Dictionary<int, uint>();

            foreach (var idConta in idsContas)
            {
                var formaPagto = UtilsPlanoConta.GetFormaPagtoByIdConta((uint)idConta);
                if (formaPagto != null)
                    dicPlanoContaFormaPagto.Add(idConta, formaPagto.IdFormaPagto.GetValueOrDefault(0));
            }

            var log = "";
            var lstPagtoInserir = new List<PagtoContasReceber>();

            #region Acerto

            foreach (var acertoGroup in contas.Where(f => f.IdAcerto.GetValueOrDefault(0) > 0).GroupBy(f => f.IdAcerto))
            {
                var cxGeral = lstCxGeral
                        .Where(f => f.IdAcerto == acertoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                        .ToList();

                var cxDiario = lstCxDiario
                     .Where(f => f.IdAcerto == acertoGroup.Key.Value && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                     .ToList();

                log = InserirPagtoContasR(log, null, acertoGroup, cxGeral, cxDiario, dicPlanoContaFormaPagto, ref lstPagtoInserir);
            }

            #endregion

            #region Acerto Parcial

            foreach (var acertoGroup in contas.Where(f => f.IdAcertoParcial.GetValueOrDefault(0) > 0 && f.IdAcerto.GetValueOrDefault(0) == 0).GroupBy(f => f.IdAcertoParcial))
            {
                try
                {
                    var contasRec = acertoGroup.ToList();

                    for (int i = 0; i < contasRec.Count; i++)
                    {
                        var cxGeral = lstCxGeral
                            .Where(f => f.IdContaR == contasRec[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();
                        var cxDiario = lstCxDiario
                            .Where(f => f.IdContaR == contasRec[i].IdContaR && dicPlanoContaFormaPagto.ContainsKey((int)f.IdConta))
                            .ToList();

                        var total = cxGeral.Sum(f => f.ValorMov) + cxDiario.Sum(f => f.Valor);

                        foreach (var cxG in cxGeral)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxG.IdConta];

                            var perc = (contasRec[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRec[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxG.ValorMov;

                            lstPagtoInserir.Add(pagto);
                        }

                        foreach (var cxD in cxDiario)
                        {
                            var idformaPagto = dicPlanoContaFormaPagto[(int)cxD.IdConta];

                            var perc = (contasRec[i].ValorRec * 100) / total;

                            var pagto = new PagtoContasReceber();

                            pagto.IdContaR = contasRec[i].IdContaR;
                            pagto.IdFormaPagto = idformaPagto;
                            pagto.ValorPagto = (perc / 100) * cxD.Valor;

                            lstPagtoInserir.Add(pagto);
                        }
                    }

                }
                catch (Exception ex)
                {
                    log += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;
                }
            }

            #endregion

            #region Insere os Pagamentos

            // Apaga todos os registros da tabela pagto_contas_receber associados às contas que serão ajustadas.
            PagtoContasReceberDAO.Instance.ApagarInserirPagamentosAntigos(string.Format("DELETE FROM pagto_contas_receber WHERE IdContaR IN ({0})",
                string.Join(",", lstPagtoInserir.Select(f => f.IdContaR).Distinct().ToList())));

            var sqlBase = "INSERT INTO pagto_contas_receber (IdContaR, IdFormaPagto, ValorPagto) VALUES {0};";
            var valoresInserir = new List<string>();
            int count = 0;

            for (int i = 0; i < lstPagtoInserir.Count; i++)
            {
                var p = lstPagtoInserir[i];

                valoresInserir.Add("(" + p.IdContaR + "," + p.IdFormaPagto + "," + p.ValorPagto.ToString().Replace(',', '.') + ")");
                count++;

                if (count >= 1000 || i + 1 == lstPagtoInserir.Count)
                {
                    var sql = string.Format(sqlBase, string.Join(",", valoresInserir.ToArray()));
                    PagtoContasReceberDAO.Instance.ApagarInserirPagamentosAntigos(sql);
                    valoresInserir = new List<string>();
                    count = 0;
                }
            }

            #endregion

            txtAjustePagtoContasReceberDeAcerto.Text = log;
        }

        //protected void btnIdProdPedCarregamento_Click(object sender, EventArgs e)
        //{
        //    using (var transaction = new GDATransaction())
        //    {

        //        try
        //        {
        //            transaction.BeginTransaction();

        //            var itensCarregamento = ItemCarregamentoDAO.Instance.GetAll()
        //                .Where(f => f.IdVolume.GetValueOrDefault(0) == 0)
        //                .GroupBy(f => f.IdPedido)
        //                .ToList();

        //            for (int i = 0; i < itensCarregamento.Count; i++)
        //            {
        //                var prodsPed = ProdutosPedidoDAO.Instance.GetByPedido(itensCarregamento[i].Key).Where(f => !f.InvisivelFluxo);
        //                var dicProdsPed = prodsPed.ToDictionary(f => f.IdProdPed, f => new KeyValuePair<float, float>(f.Qtde, 0));

        //                var group = itensCarregamento[i].ToList();

        //                for (int j = 0; j < group.Count; j++)
        //                {
        //                    foreach (var idProdPed in prodsPed.Where(f => f.IdProd == group[j].IdProd).Select(f => f.IdProdPed))
        //                    {
        //                        if (dicProdsPed.ContainsKey(idProdPed) && dicProdsPed[idProdPed].Key > dicProdsPed[idProdPed].Value)
        //                        {
        //                            tempItemCarregamentoDAO.Instance.AtualizaIdProdPed(transaction, group[j].IdItemCarregamento, idProdPed);
        //                            dicProdsPed[idProdPed] = new KeyValuePair<float, float>(dicProdsPed[idProdPed].Key, dicProdsPed[idProdPed].Value + 1);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            transaction.Commit();
        //            transaction.Close();

        //            MensagemAlerta.ShowMsg("Tabela item_Carregamento atualizada", Page);
        //        }
        //        catch (Exception ex)
        //        {
        //            MensagemAlerta.ErrorMsg("Falha", ex, Page);

        //            transaction.Rollback();
        //            transaction.Close();
        //        }
        //    }
        //}
    }
}