using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.DAL;
using System.Linq;
using Glass.Configuracoes;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Enumeração com as situações de produção possíveis.
    /// </summary>
    public enum SituacaoProdutoProducao
    {
        /// <summary>
        /// Pendente.
        /// </summary>
        [Description("Pendente")]
        Pendente = 1,
        /// <summary>
        /// Pronto.
        /// </summary>
        [Description("Pronto")]
        Pronto,
        /// <summary>
        /// Entregue.
        /// </summary>
        [Description("Entregue")]
        Entregue,
    }

    [PersistenceBaseDAO(typeof(ProdutoPedidoProducaoDAO))]
	[PersistenceClass("produto_pedido_producao")]
	public class ProdutoPedidoProducao
    {
        #region Enumeradores

        public enum SituacaoEnum : int
        {
            Producao=1,
            Perda,
            CanceladaVenda,
            CanceladaMaoObra
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPRODPEDPRODUCAO", PersistenceParameterType.IdentityKey)]
        public uint IdProdPedProducao { get; set; }

        /// <summary>
        /// Id do produto_pedido_espelho
        /// </summary>
        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IDSETOR")]
        [Log("Setor", "Descricao", typeof(SetorDAO))]
        public uint IdSetor { get; set; }

        [PersistenceProperty("IDFUNCPERDA")]
        [Log("Funcionário Perda", "Nome", typeof(FuncionarioDAO))]
        public uint? IdFuncPerda { get; set; }

        [PersistenceProperty("IdProdPedProducaoParent")]
        public uint? IdProdPedProducaoParent { get; set; }

        [PersistenceProperty("IDPEDIDOEXPEDICAO")]
        [Log("Pedido Expedição")]
        public uint? IdPedidoExpedicao { get; set; }

        [PersistenceProperty("IDFUNCREPOS")]
        [Log("Funcionário Reposição", "Nome", typeof(FuncionarioDAO))]
        public uint? IdFuncRepos { get; set; }

        [PersistenceProperty("IDSETORREPOS")]
        [Log("Setor Reposição", "Descricao", typeof(SetorDAO))]
        public uint? IdSetorRepos { get; set; }

        /// <summary>
        /// 1-Produção
        /// 2-Perda
        /// 3-Cancelada (venda)
        /// 4-Cancelada (mão-de-obra)
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public long Situacao { get; set; }

        [PersistenceProperty("DATAPERDA")]
        [Log("Data Perda")]
        public DateTime? DataPerda { get; set; }

        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("PLANOCORTE")]
        [Log("Plano de Corte")]
        public string PlanoCorte { get; set; }

        [PersistenceProperty("TIPOPERDA")]
        [Log("Tipo de Perda", "Descricao", typeof(TipoPerdaDAO))]
        public uint? TipoPerda { get; set; }

        [PersistenceProperty("IDSUBTIPOPERDA")]
        [Log("Subtipo de Perda", "Descricao", typeof(SubtipoPerdaDAO))]
        public uint? IdSubtipoPerda { get; set; }

        [PersistenceProperty("OBS")]
        [Log("Observação")]
        public string Obs { get; set; }

        [PersistenceProperty("ENTROUESTOQUE")]
        [Log("Entrou em Estoque?")]
        public bool EntrouEstoque { get; set; }

        [PersistenceProperty("PECAREPOSTA")]
        [Log("Peça Reposta?")]
        public bool PecaReposta { get; set; }

        private uint? _tipoPerdaRepos;

        [PersistenceProperty("TIPOPERDAREPOS")]
        [Log("Tipo de Perda Reposição", "Descricao", typeof(TipoPerdaDAO))]
        public uint? TipoPerdaRepos
        {
            get { return (_descrSetor != "Troca" && _descrSetor != "Devolução") || _tipoPerdaRepos > 0 ? _tipoPerdaRepos : TipoPerdaDAO.Instance.GetIDByNomeExato("Outros"); }
            set { _tipoPerdaRepos = value; }
        }

        [PersistenceProperty("IDSUBTIPOPERDAREPOS")]
        [Log("Subtipo de Perda Reposição", "Descricao", typeof(SubtipoPerdaDAO))]
        public uint? IdSubtipoPerdaRepos { get; set; }

        [PersistenceProperty("DATAREPOS")]
        [Log("Data de Reposição")]
        public DateTime? DataRepos { get; set; }

        [PersistenceProperty("NUMETIQUETACANC")]
        public string NumEtiquetaCanc { get; set; }

        [PersistenceProperty("IDIMPRESSAO")]
        [Log("Impressão")]
        public uint? IdImpressao { get; set; }

        [PersistenceProperty("CANCELADOADMIN")]
        public bool CanceladoAdmin { get; set; }

        [PersistenceProperty("DADOSREPOSICAOPECA")]
        public string DadosReposicaoPeca { get; set; }

        [PersistenceProperty("SITUACAOPRODUCAO")]
        public int SituacaoProducao { get; set; }

        [PersistenceProperty("PECAPARADAPRODUCAO")]
        [Log("Peça Parada na Produção?")]
        public bool PecaParadaProducao { get; set; }

        [PersistenceProperty("MOTIVOPECAPARADAPRODUCAO")]
        [Log("Motivo da peça parada na produção")]
        public string MotivoPecaParadaProducao { get; set; }

        [PersistenceProperty("PosEtiquetaParent")]
        public string PosEtiquetaParent { get; set; }

        [PersistenceProperty("IdCavalete")]
        public int? IdCavalete { get; set; }

        [PersistenceProperty("IdFornada")]
        public int? IdFornada { get; set; }

        [PersistenceProperty("IdProdBaixaEst")]
        public int? IdProdBaixaEst { get; set; }

        //Campo usado para destacar a etiqueta do produto do pedido importado
        [PersistenceProperty("NumEtiquetaCliente")]
        public string NumEtiquetaCliente { get; set; }

        [PersistenceProperty("TROCADODEVOLVIDO")]
        public bool TrocadoDevolvido { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeFuncPerda;

        [PersistenceProperty("NOMEFUNCPERDA", DirectionParameter.InputOptional)]
        public string NomeFuncPerda
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncPerda); }
            set { _nomeFuncPerda = value; }
        }

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public uint IdPedido { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public int? NumeroNFe { get; set; }

        [PersistenceProperty("IDPEDIDOREP", DirectionParameter.InputOptional)]
        public uint IdPedidoRep { get; set; }

        [PersistenceProperty("CODCLIENTE", DirectionParameter.InputOptional)]
        public string CodCliente { get; set; }

        [PersistenceProperty("DATAENTREGA", DirectionParameter.InputOptional)]
        public DateTime? DataEntrega { get; set; }

        [PersistenceProperty("DATAENTREGAFABRICA", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaFabrica { get; set; }

        [PersistenceProperty("DATAENTREGAORIGINAL", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaOriginal { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint? IdCliente { get; set; }

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get 
            {
                if (_nomeCliente == null)
                    return String.Empty;

                return _nomeCliente.Contains("(") ? _nomeCliente : BibliotecaTexto.GetTwoFirstNames(_nomeCliente); 
            }
            set { _nomeCliente = value; }
        }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        private string _descrSetor;

        [PersistenceProperty("DESCRSETOR", DirectionParameter.InputOptional)]
        public string DescrSetor
        {
            get { return _descrSetor + (Situacao == (int)SituacaoEnum.Perda ? " (Perda)" : ""); }
            set { _descrSetor = value; }
        }

        private string _descrDepart;

        [PersistenceProperty("DESCRDEPART", DirectionParameter.InputOptional)]
        public string DescrDepart
        {
            get { return string.IsNullOrEmpty(_descrDepart) ? "-" : _descrDepart; }
            set { _descrDepart = value; }
        }

        [PersistenceProperty("DESCRSETORREPOS", DirectionParameter.InputOptional)]
        public string DescrSetorRepos { get; set; }

        [PersistenceProperty("ALTURA", DirectionParameter.InputOptional)]
        public double Altura { get; set; }

        [PersistenceProperty("LARGURA", DirectionParameter.InputOptional)]
        public long Largura { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("IDPEDIDOEXIBIR", DirectionParameter.InputOptional)]
        public string IdPedidoExibir { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        private double _totM2;

        [PersistenceProperty("TOTM2", DirectionParameter.InputOptional)]
        public double TotM2
        {
            get { return _totM2 > 0 ? _totM2 : Glass.Global.CalculosFluxo.ArredondaM2((int)Largura, (int)Altura, (int)Qtde, 0, false); }
            set { _totM2 = value; }
        }

        [PersistenceProperty("TOTM", DirectionParameter.InputOptional)]
        public double TotM { get; set; }

        [PersistenceProperty("TOTAL", DirectionParameter.InputOptional)]
        public decimal Total { get; set; }

        [PersistenceProperty("QTDE", DirectionParameter.InputOptional)]
        public decimal Qtde { get; set; }

        [PersistenceProperty("VALORUNIT", DirectionParameter.InputOptional)]
        public decimal ValorUnit { get; set; }

        [PersistenceProperty("PEDIDOCANCELADO", DirectionParameter.InputOptional)]
        public bool PedidoCancelado { get; set; }

        [PersistenceProperty("PEDIDOMAOOBRA", DirectionParameter.InputOptional)]
        public bool PedidoMaoObra { get; set; }

        [PersistenceProperty("PEDIDOPRODUCAO", DirectionParameter.InputOptional)]
        public bool PedidoProducao { get; set; }

        [PersistenceProperty("DATALIBERACAOPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataLiberacaoPedido { get; set; }

        [PersistenceProperty("TIPO", DirectionParameter.InputOptional)]
        public ProdutoPedidoProducaoDAO.TipoRetorno Tipo { get; set; }

        [PersistenceProperty("TIPOSETOR", DirectionParameter.InputOptional)]
        public int TipoSetor { get; set; }

        [PersistenceProperty("COR", DirectionParameter.InputOptional)]
        public string Cor { get; set; }

        [PersistenceProperty("ESPESSURA", DirectionParameter.InputOptional)]
        public float Espessura { get; set; }

        [PersistenceProperty("IDGRUPOPROD", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("DESCRSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public string DescrSubgrupoProd { get; set; }

        [PersistenceProperty("VALORCUSTOUNITARIO", DirectionParameter.InputOptional)]
        public decimal ValorCustoUnitario { get; set; }

        [PersistenceProperty("DATAAGRUPAR", DirectionParameter.InputOptional)]
        public DateTime? DataAgrupar { get; set; }

        [PersistenceProperty("NUMETIQUETACHAPA", DirectionParameter.InputOptional)]
        public string NumEtiquetaChapa { get; set; }

        [PersistenceProperty("NUMERONFECHAPA", DirectionParameter.InputOptional)]
        public string NumeroNFeChapa { get; set; }

        [PersistenceProperty("LOTECHAPA", DirectionParameter.InputOptional)]
        public string LoteChapa { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.InputOptional)]
        public double Peso { get; set; }

        [PersistenceProperty("NumCavalete", DirectionParameter.InputOptional)]
        public string NumCavalete { get; set; }

        public bool IsProdutoLaminadoComposicao
        {
            get
            {
                var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd((uint)IdProdPed);
                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);

                return tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
            }
        }

        #endregion

        #region Propriedades de Suporte

        public bool PecaCancelada
        {
            get { return NumEtiquetaCanc != null || Situacao > (int)SituacaoEnum.Perda; }
        }

        public bool EstornoCarregamentoVisible
        {
            get { return EstornoItemCarregamentoDAO.Instance.TemEstorno(0, (int)IdProdPedProducao); }
        }

        public uint IdProdPedExibir
        {
            get
            {
                return IdProdPed != null ? IdProdPed.Value : 0;
            }
        }

        [Log("Número da Etiqueta")]
        public string NumEtiquetaExibir
        {
            get { return NumEtiqueta != null ? NumEtiqueta : NumEtiquetaCanc; }
        }

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string DescrProdLargAlt
        {
            get { return CodDescrProd + " " + Largura + "x" + Altura; }
        }

        [Log("Produto")]
        public string CodDescrProd
        {
            get { return !String.IsNullOrEmpty(CodInterno) ? CodInterno + " - " + DescrProduto : DescrProduto; }
        }

        public string TituloLarguraAltura
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura X Largura" : "Largura X Altura"; }
        }

        public string LarguraAltura
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Altura + "x" + Largura : Largura + "x" + Altura; }
        }

        public string DataEntregaSemHora
        {
            get { return Conversoes.ConverteData(DataEntrega, false); }
        }

        public string DataEntregaExibicao
        {
            get
            {
                return DataEntregaSemHora + (DataEntrega != DataEntregaOriginal && DataEntregaOriginal != null ? " (" +
                    Conversoes.ConverteData(DataEntregaOriginal, false) + ")" : "");
            }
        }

        public DateTime? Data
        {
            get 
            {
                switch (Tipo)
                {
                    case ProdutoPedidoProducaoDAO.TipoRetorno.AguardandoExpedicao:
                        return DataLiberacaoPedido;
                    default:
                        return Situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Producao ? DataSetorAtual :
                            Situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda ? DataPerda : null;
                }
            }
        }

        public string CorLinha
        {
            get
            {
                Color color = Color.Red;

                if (IdProdPed == null || Situacao == (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda || 
                    Situacao == (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra)
                    color = Color.Black;
                else if (Situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                    color = Color.Gray;
                else
                    return Utils.ObtemCorSetor(IdSetor);

                return color.Name;
            }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch ((SituacaoEnum)Situacao)
                {
                    case SituacaoEnum.Producao: return "Produção";
                    case SituacaoEnum.Perda: return "Perda";
                    case SituacaoEnum.CanceladaVenda: return "Cancelada (venda)";
                    case SituacaoEnum.CanceladaMaoObra: return "Cancelada (mão-de-obra)";
                    default: return "";
                }
            }
        }

        public string SiglaTipoPedido
        {
            get
            {
                if (PedidoMaoObra)
                    return "MO";
                else if (PedidoProducao)
                    return "P";
                else
                    return "V";
            }
        }

        public string DescrBeneficiamentos
        {
            get
            {
                if (PedidoMaoObra)
                {
                    if (IdProdPedExibir == 0)
                        return "";

                    AmbientePedidoEspelho amb = AmbientePedidoEspelhoDAO.Instance.GetByIdProdPed(IdProdPedExibir);
                    if (amb == null)
                        return "";

                    uint idAmbientePedido = amb.IdAmbientePedido;
                    return "(" + AmbientePedidoEspelhoDAO.Instance.GetDescrMaoObra(idAmbientePedido).Trim() + ")";
                }
                else
                    return "";
                    //return "(" + ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(IdProdPed).Trim() + ")";
            }
        }

        public bool RemoverSituacaoVisible
        {
            get 
            {
                return !PecaCancelada && Config.PossuiPermissao(Config.FuncaoMenuPCP.VoltarSetorPecaProducao) &&
                    ((!ProdutosPedidoDAO.Instance.IsEtiquetaReposta(NumEtiqueta) && (IdSetor > 1 || Situacao == (int)SituacaoEnum.Perda) &&
                    (Situacao != (int)SituacaoEnum.Perda || !PedidoMaoObra)) ||
                    (!String.IsNullOrEmpty(DadosReposicaoPeca) && IdSetor == 1)) &&
                    SetorDAO.Instance.ObtemTipoSetor(IdSetor) != Glass.Data.Model.TipoSetor.ExpCarregamento;
            }
        }

        public string DescrTipoPerdaSemObs
        {
            get
            {
                return Situacao != (int)SituacaoEnum.Producao && TipoPerda != null ?
                    "Perda: " + TipoPerdaDAO.Instance.GetNome((uint)TipoPerda.Value) + 
                    (IdSubtipoPerda > 0 ? "  (Subtipo: " + SubtipoPerdaDAO.Instance.GetDescricao(IdSubtipoPerda.Value) + ")" : "") :
                    
                    (PecaReposta || IdProdPedProducao == 0) && TipoPerdaRepos > 0 ? "Perda: " + TipoPerdaDAO.Instance.GetNome((uint)TipoPerdaRepos) +
                    (IdSubtipoPerdaRepos > 0 ? "  (Subtipo: " + SubtipoPerdaDAO.Instance.GetDescricao(IdSubtipoPerdaRepos.Value) + ")" : "") : null;
            }
        }

        public string DescrTipoPerda
        {
            get
            {
                string descrTipoPerda = DescrTipoPerdaSemObs;
                return String.IsNullOrEmpty(descrTipoPerda) ? null :
                    descrTipoPerda + (!String.IsNullOrEmpty(Obs) ? " - " + Obs : "");
            }
        }

        public string DescrTipoPerdaLista
        {
            get
            {
                string tipoPerda = DescrTipoPerda;
                return !String.IsNullOrEmpty(tipoPerda) ? tipoPerda : null;
            }
        }

        private string _imagemPecaUrl = null;

        public string ImagemPecaUrl
        {
            get 
            {
                if (IdProdPedExibir == 0)
                    return "";

                if (_imagemPecaUrl == null)
                {
                    ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(IdProdPedExibir);
                    uint? idPecaItemProj = PecaItemProjetoDAO.Instance.ObtemIdPecaItemProjByIdProdPed(IdProdPedExibir);

                    ppe.Item = idPecaItemProj > 0 && !String.IsNullOrEmpty(NumEtiquetaExibir) ?
                        UtilsProjeto.GetItemPecaFromEtiqueta(PecaItemProjetoDAO.Instance.ObtemItem(idPecaItemProj.Value), NumEtiquetaExibir) : 0;

                    if (!string.IsNullOrEmpty(ppe.ImagemUrl))
                        _imagemPecaUrl = ppe.ImagemUrl;
                    else
                    {
                        var urlCadProd = Utils.GetProdutosVirtualPath + ppe.IdProd + ".jpg";
                        return System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(urlCadProd)) ? urlCadProd : string.Empty;
                    }
                }

                return _imagemPecaUrl;
            }
        }

        public bool ExibirRelatorioPedido
        {
            get { return PedidoDAO.ExibirRelatorioPedido(IdPedido) && (ProducaoConfig.TelaConsulta.ExibirImpressaoPedidoTelaConsulta || UserInfo.GetUserInfo.IsAdministrador); }
        }

        public bool ExibirPararPecaProducao
        {
            get 
            { 
                return SituacaoProducao == (int)SituacaoProdutoProducao.Pendente && 
                    Config.PossuiPermissao(Config.FuncaoMenuPCP.PararRetornarPecaProducao); 
            }
        }

        public decimal ValorCustoPeca
        {
            get { return ValorCustoUnitario * (decimal)_totM2; }
        }

        public string PecaParadaProducaoStr
        {
            get { return PecaParadaProducao ? "Sim" : "Não"; }
        }

        [Log("Data Leitura Setor")]
        internal DateTime? DataLeituraSetorVoltarPeca { get; set; }

        [Log("Funcionário Leitura Setor")]
        internal string NomeFuncLeituraSetorVoltarPeca { get; set; }

        /// <summary>
        /// Verifica se o produto pedido tem um arquivo SVG associado a ele
        /// </summary>
        public bool TemSvgAssociado
        {
            get
            {
                var caminho = PCPConfig.CaminhoSalvarCadProject(true) + IdProdPed + ".svg";

                return System.IO.File.Exists(caminho);
            }
        }

        #endregion

        #region Campos de leitura da produção

        private string[] _vetDataLeitura;

        public string[] VetDataLeitura
        {
            get 
            {
                if (_vetDataLeitura == null || _vetDataLeitura.Length == 0)
                {
                    if (GroupIdSetor != null)
                    {
                        List<string> vetIdSetor = new List<string>(GroupIdSetor.Split(','));
                        _vetDataLeitura = new string[vetIdSetor.Count];

                        if (GroupDataLeitura != null)
                        {
                            string[] vetLeitura = GroupDataLeitura.Split(',');

                            int pos, posLeitura = 0;
                            for (int i = 0; i < Utils.GetSetores.Length; i++)
                                if (Utils.GetSetores[i].ExibirRelatorio)
                                {
                                    pos = vetIdSetor.IndexOf(Utils.GetSetores[i].IdSetor.ToString());
                                    if (pos > -1)
                                        _vetDataLeitura[pos] = posLeitura < vetLeitura.Length ? vetLeitura[posLeitura++] : null;
                                }
                        }
                    }
                    else
                        _vetDataLeitura = new string[Utils.GetSetores.Length];
                }

                return _vetDataLeitura;
            }
        }

        private string[] _vetNomeFuncLeitura;

        public string[] VetNomeFuncLeitura
        {
            get
            {
                if (_vetNomeFuncLeitura == null || _vetNomeFuncLeitura.Length == 0)
                {
                    if (GroupIdSetor != null)
                    {
                        List<string> vetIdSetor = new List<string>(GroupIdSetor.Split(','));
                        _vetNomeFuncLeitura = new string[vetIdSetor.Count];

                        if (GroupNomeFuncLeitura != null)
                        {
                            string[] vetFuncLeitura = GroupNomeFuncLeitura.Split(',');

                            int pos, posLeitura = 0;
                            for (int i = 0; i < Utils.GetSetores.Length; i++)
                                if (Utils.GetSetores[i].ExibirRelatorio)
                                {
                                    pos = vetIdSetor.IndexOf(Utils.GetSetores[i].IdSetor.ToString());
                                    if (pos > -1)
                                        _vetNomeFuncLeitura[pos] = posLeitura < vetFuncLeitura.Length ? vetFuncLeitura[posLeitura++] : null;
                                }
                        }
                    }
                    else
                        _vetNomeFuncLeitura = new string[Utils.GetSetores.Length];
                }

                return _vetNomeFuncLeitura;
            }
        }

        private bool[] _vetSetorCorte;

        public bool[] VetSetorCorte
        {
            get
            {
                if (_vetSetorCorte == null || _vetSetorCorte.Length == 0)
                {
                    if (GroupIdSetor != null)
                    {
                        List<string> vetIdSetor = new List<string>(GroupIdSetor.Split(','));
                        _vetSetorCorte = new bool[vetIdSetor.Count];

                        if (GroupSetorCorte != null)
                        {
                            string[] vetSetorCorte = GroupSetorCorte.Split(',');

                            int pos, posLeitura = 0;
                            for (int i = 0; i < Utils.GetSetores.Length; i++)
                                if (Utils.GetSetores[i].ExibirRelatorio)
                                {
                                    pos = vetIdSetor.IndexOf(Utils.GetSetores[i].IdSetor.ToString());
                                    if (pos > -1)
                                        _vetSetorCorte[pos] = posLeitura < vetSetorCorte.Length ? vetSetorCorte[posLeitura++] == "1" : false;
                                }
                        }
                    }
                    else
                        _vetSetorCorte = new bool[Utils.GetSetores.Length];
                }

                return _vetSetorCorte;
            }
        }

        public bool[] TemLeitura
        {
            get
            {
                bool[] temLeituras = new bool[Utils.GetSetores.Length];
                for (int i = 0; i < VetDataLeitura.Length; i++)
                    temLeituras[i] = !String.IsNullOrEmpty(VetDataLeitura[i]);

                return temLeituras;
            }
        }

        private string[] _setorNaoObrigatorio;

        public string[] SetorNaoObrigatorio
        {
            get
            {
                if (_setorNaoObrigatorio == null)
                {
                    if (GroupIdSetor != null)
                    {
                        List<string> vetIdSetor = new List<string>(GroupIdSetor.Split(','));
                        _setorNaoObrigatorio = new string[Utils.GetSetores.Length];

                        var temLeitura = TemLeitura;
                        var setorObrigatorio = SetorDAO.Instance.ObtemSetoresObrigatorios(IdProdPedProducao).Select(x => x.IdSetor);

                        int pos;
                        for (int i = 0; i < Utils.GetSetores.Length; i++)
                            if (Utils.GetSetores[i].ExibirRelatorio)
                            {
                                pos = vetIdSetor.IndexOf(Utils.GetSetores[i].IdSetor.ToString());

                                if (pos > -1)
                                {
                                    _setorNaoObrigatorio[pos] = temLeitura[pos] || !Utils.GetSetores[i].SetorPertenceARoteiro ||
                                        setorObrigatorio.Contains(Utils.GetSetores[i].IdSetor) ? String.Empty : @"
                                        <div style='text-align: center; opacity: 0.5; filter: alpha(opacity=50)'>
                                            <img src='../../Images/alerta.png' alt='Setor não faz parte do roteiro de produção' 
                                                title='Setor não faz parte do roteiro de produção, portanto não necessita ser lido' />
                                        </div>";
                                }
                            }
                    }
                    else
                        _setorNaoObrigatorio = new string[Utils.GetSetores.Length];
                }

                return _setorNaoObrigatorio;
            }
        }

        public DateTime? DataSetorAtual
        {
            get 
            {
                if (VetDataLeitura == null || VetDataLeitura.Length == 0)
                    return null;

                Setor[] vetSetor = Utils.GetSetores;

                for (int i = 0; i < vetSetor.Length; i++)
                    if (vetSetor[i].IdSetor == IdSetor && !String.IsNullOrEmpty(VetDataLeitura[i]))
                        return DateTime.Parse(VetDataLeitura[i]);

                return null;
            }
        }

        [PersistenceProperty("GROUPIDSETOR", DirectionParameter.InputOptional)]
        public string GroupIdSetor { get; set; }

        [PersistenceProperty("GROUPDATALEITURA", DirectionParameter.InputOptional)]
        public string GroupDataLeitura { get; set; }

        [PersistenceProperty("GROUPNOMEFUNCLEITURA", DirectionParameter.InputOptional)]
        public string GroupNomeFuncLeitura { get; set; }

        [PersistenceProperty("GROUPSETORCORTE", DirectionParameter.InputOptional)]
        public string GroupSetorCorte { get; set; }

        /// <summary>
        /// Verifica se o produto pedido produção tem leitura em algum setor oculto.
        /// </summary>
        public bool TemLeituraSetorOculto
        {
            get
            {
                var leituras = LeituraProducaoDAO.Instance.ObterSetoresLidos(IdProdPedProducao);
                if (leituras != null && leituras.Count > 0)
                    // Verifica se algum dos setores que a peça foi lida não está sendo exibido no relatório.
                    return leituras.Any(s => !SetorDAO.Instance.ExibirNoRelatorio(s));
                else
                    return false;
            }
        }

        #endregion
    }
}