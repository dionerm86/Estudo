using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosPedidoEspelhoDAO))]
	[PersistenceClass("produtos_pedido_espelho")]
	public class ProdutosPedidoEspelho : IResumoCorte, IProdutoCalculo
    {
        /*
            Criação de campos novos nesta model devem ser incluídos nos métodos SqlProdEtiq() e SqlImpIndiv(), na ProdutosPedidoEspelhoDAO
         */

        #region Propriedades

        [PersistenceProperty("IDPRODPED", PersistenceParameterType.IdentityKey)]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IdProdPedParent")]
        public uint? IdProdPedParent { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("IDMATERITEMPROJ")]
        public uint? IdMaterItemProj { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDO")]
        public uint? IdAmbientePedido { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("VALORVENDIDO")]
        public decimal ValorVendido { get; set; }

        [PersistenceProperty("ALTURA")]
        public Single Altura { get; set; }

        private Single _alturaReal;

        [PersistenceProperty("ALTURAREAL")]
        public Single AlturaReal
        {
            get { return _alturaReal > 0 ? _alturaReal : Altura; }
            set { _alturaReal = value; }
        }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        private int _larguraReal;

        [PersistenceProperty("LARGURAREAL")]
        public int LarguraReal
        {
            get { return _larguraReal > 0 ? _larguraReal : Largura; }
            set { _larguraReal = value; }
        }

        [PersistenceProperty("TOTM")]
        public Single TotM { get; set; }

        [PersistenceProperty("TOTM2CALC")]
        public Single TotM2Calc { get; set; }

        [PersistenceProperty("QTDIMPRESSO")]
        public int QtdImpresso { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("ALIQICMS")]
        public Single AliqIcms { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliqIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [PersistenceProperty("AMBIENTE")]
        public string Ambiente { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("OBSGRID")]
        public string ObsGrid { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("ALTURABENEF")]
        public int? AlturaBenef { get; set; }

        [PersistenceProperty("LARGURABENEF")]
        public int? LarguraBenef { get; set; }

        [PersistenceProperty("ESPBENEF")]
        public float? EspessuraBenef { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("VALORACRESCIMOPROD")]
        public decimal ValorAcrescimoProd { get; set; }

        [PersistenceProperty("VALORDESCONTOPROD")]
        public decimal ValorDescontoProd { get; set; }

        [PersistenceProperty("PERCDESCONTOQTDE")]
        public float PercDescontoQtde { get; set; }

        [PersistenceProperty("VALORDESCONTOQTDE")]
        public decimal ValorDescontoQtde { get; set; }

        [PersistenceProperty("VALORDESCONTOCLIENTE")]
        public decimal ValorDescontoCliente { get; set; }

        [PersistenceProperty("VALORACRESCIMOCLIENTE")]
        public decimal ValorAcrescimoCliente { get; set; }

        [PersistenceProperty("INVISIVELFLUXO")]
        public bool InvisivelFluxo { get; set; }

        [PersistenceProperty("INVISIVELADMIN")]
        public bool InvisivelAdmin { get; set; }

        [PersistenceProperty("VALORUNITBRUTO")]
        public decimal ValorUnitarioBruto { get; set; }

        [PersistenceProperty("TOTALBRUTO")]
        public decimal TotalBruto { get; set; }

        [PersistenceProperty("QTDEINVISIVEL", DirectionParameter.Input)]
        public float QtdeInvisivel { get; set; }

        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("PESO", DirectionParameter.Input)]
        public float Peso { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [PersistenceProperty("IdProdBaixaEst")]
        public int? IdProdBaixaEst { get; set; }

        #endregion

        #region Propriedades Estendidas

        /// <summary>
        /// Propriedade usada para impressão individual de etiqueta
        /// </summary>
        [PersistenceProperty("IDAMBIENTEPEDIDOIMPR", DirectionParameter.InputOptional)]
        public long IdAmbientePedidoImpr
        {
            set { IdAmbientePedido = (uint)value; }
        }

        [PersistenceProperty("AMBIENTEPEDIDO", DirectionParameter.InputOptional)]
        public string AmbientePedido { get; set; }

        [PersistenceProperty("DESCRAMBIENTEPEDIDO", DirectionParameter.InputOptional)]
        public string DescrAmbientePedido { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint IdSubgrupoProd { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CODOTIMIZACAO", DirectionParameter.InputOptional)]
        public string CodOtimizacao { get; set; }

        [PersistenceProperty("CodCliente", DirectionParameter.InputOptional)]
        public string CodCliente { get; set; }

        private string _m2Minimo;

        [PersistenceProperty("AreaMinima", DirectionParameter.InputOptional)]
        public string M2Minimo
        {
            get { return !String.IsNullOrEmpty(_m2Minimo) ? _m2Minimo.ToString().Replace(',', '.') : "0"; }
            set { _m2Minimo = value; }
        }

        private string _nomeCliente;

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        [PersistenceProperty("RotaCliente", DirectionParameter.InputOptional)]
        public string RotaCliente { get; set; }

        [PersistenceProperty("NomeCidade", DirectionParameter.InputOptional)]
        public string NomeCidade { get; set; }

        [PersistenceProperty("DataPedido", DirectionParameter.InputOptional)]
        public DateTime DataPedido { get; set; }

        [PersistenceProperty("DataEntrega", DirectionParameter.InputOptional)]
        public DateTime DataEntrega { get; set; }

        [PersistenceProperty("DataFabrica", DirectionParameter.InputOptional)]
        public DateTime DataFabrica { get; set; }

        [PersistenceProperty("QtdeSomada", DirectionParameter.InputOptional)]
        public double QtdeSomada { get; set; }

        [PersistenceProperty("QtdeComprada", DirectionParameter.InputOptional)]
        public long QtdeComprada { get; set; }

        [PersistenceProperty("COR", DirectionParameter.InputOptional)]
        public string Cor { get; set; }

        [PersistenceProperty("TotMSomada", DirectionParameter.InputOptional)]
        public double TotMSomada { get; set; }

        [PersistenceProperty("PesoSomado", DirectionParameter.InputOptional)]
        public double PesoSomado { get; set; }

        [PersistenceProperty("COMPRADO", DirectionParameter.InputOptional)]
        public bool Comprado { get; set; }

        private string _etiquetasLegenda;

        public string EtiquetasLegenda
        {
            get 
            {
                if (String.IsNullOrEmpty(_etiquetasLegenda))
                {
                    _etiquetasLegenda = PecaItemProjetoDAO.Instance.ObtemEtiquetas(IdPedido, IdProdPed, (int)Qtde);

                    if (!String.IsNullOrEmpty(_etiquetasLegenda) && _etiquetasLegenda.IndexOf(", ") > -1 && !_etiquetasLegenda.Contains(" e "))
                    {
                        string[] etiquetas = _etiquetasLegenda.Split(',');

                        int numLinha = 0;
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < etiquetas.Length; i++)
                        {
                            if (i == (etiquetas.Length - 1))
                                sb.Append(" e ");
                            else if (i > 0)
                            {
                                sb.Append(",");
                                if (++numLinha == 4)
                                {
                                    sb.Append("<br />");
                                    numLinha = 0;
                                }
                                else
                                    sb.Append(" ");
                            }

                            sb.Append(etiquetas[i].Trim());
                        }

                        _etiquetasLegenda = sb.ToString();
                    }
                }

                return _etiquetasLegenda;
            }
            set { _etiquetasLegenda = value; }
        }

        [PersistenceProperty("OBSPROJ", DirectionParameter.InputOptional)]
        public string ObsProj { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("PECAREPOSTA", DirectionParameter.InputOptional)]
        public bool PecaReposta { get; set; }

        [PersistenceProperty("NUMETIQUETA", DirectionParameter.InputOptional)]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("QTDEREPOSTA", DirectionParameter.InputOptional)]
        public long QtdeReposta { get; set; }

        [PersistenceProperty("CustoCompraProduto", DirectionParameter.InputOptional)]
        public decimal CustoCompraProduto { get; set; }

        [PersistenceProperty("IdProdNf", DirectionParameter.InputOptional)]
        public int IdProdNf { get; set; }

        [PersistenceProperty("NumeroNFe", DirectionParameter.InputOptional)]
        public int NumeroNFe { get; set; }

        [PersistenceProperty("PedidoMaoObra", DirectionParameter.InputOptional)]
        public bool PedidoMaoObra { get; set; }

        [PersistenceProperty("DescrProdutoBenef", DirectionParameter.InputOptional)]
        public string DescrProdutoBenef { get; set; }

        [PersistenceProperty("ProfundidadeCaixa", DirectionParameter.InputOptional)]
        public string ProfundidadeCaixa { get; set; }

        [PersistenceProperty("CompraGerada", DirectionParameter.InputOptional)]
        public string CompraGerada { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float TotM2Rpt
        {
            get { return PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? 
            TotM2Calc : TotM * (PedidoMaoObra ? QtdeAmbiente : 1); }
        }

        public float QtdeRpt
        {
            get { return PedidoMaoObra ? QtdeAmbiente : Qtde; }
        }

        public string PlanoCorte { get; set; }

        public int QtdAImprimir { get; set; }

        private string _etiquetas;

        /// <summary>
        /// Utilizado apenas para auxiliar na exportação do arquivo de otimização
        /// </summary>
        public string Etiquetas
        {
            get { return _etiquetas; }
            set { _etiquetas = value; }
        }

        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaRpt : LarguraRpt; }
        }

        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? LarguraRpt : AlturaRpt; }
        }

        public string TotM2CalcSemChapaString
        {
            get
            {
                var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(null, IdPedido);
                return Glass.Global.CalculosFluxo.CalcM2Calculo(IdCliente, (int)Altura, Largura, Qtde, (int)IdProd, Redondo,
                    Beneficiamentos.CountAreaMinima, ProdutoDAO.Instance.ObtemAreaMinima((int)IdProd), false, 0, !isPedidoProducaoCorte).ToString();
            }
        }

        public string AlturaRpt
        {
            get { return TipoCalc == 4 ? (AlturaProducao).ToString("N2") + "ml" : (AlturaProducao).ToString(); }
        }

        public string LarguraRpt
        {
            get { return Redondo ? "0" : LarguraProducao.ToString(); }
        }

        public float AlturaProducao
        {
            get { return _alturaReal > 0 ? _alturaReal : Altura; }
        }

        public int LarguraProducao
        {
            get { return _larguraReal > 0 ? _larguraReal : Largura; }
        }

        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString().ToLower(); }
        }

        public string IsAluminio
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd).ToString().ToLower(); }
        }

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd); }
        }

        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public bool EditDeleteVisible
        {
            get 
            { 
                return QtdImpresso <= 0 && !PCPConfig.BloquearExclusaoProdutosPCP; 
            }
        }

        public bool DesmembrarVisible { get; set; }

        public int QtdeComprar
        {
            get { return (int)Qtde - (int)QtdeComprada; }
        }

        public string AlturaLista
        {
            get
            {
                if (IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Alumínio)
                    return Altura.ToString();
                else
                    return Altura != _alturaReal ? (_alturaReal > 0 ? _alturaReal.ToString() + " (" + Altura.ToString() + ")" : Altura.ToString()) : Altura.ToString();
            }
        }

        public bool BenefVisible
        {
            get 
            {
                return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro();
            }
        }

        private bool _benefEtiqueta = false;

        public bool BenefEtiqueta
        {
            get { return _benefEtiqueta; }
            set { _benefEtiqueta = value; }
        }

        public string DescricaoProdutoComBenef
        {
            get
            {
                string retorno = DescrProduto;

                if (AlturaBenef > 0 || LarguraBenef > 0)
                {
                    bool usarCompl = CompraConfig.ExibicaoDescrBenefCustomizada && _benefEtiqueta;
                    string complAltura = usarCompl ? " (" + Altura + ")" : "";
                    string complLargura = usarCompl ? " (" + Largura + ")" : "";

                    int altura = AlturaBenef != null ? AlturaBenef.Value : 0;
                    int largura = LarguraBenef != null ? LarguraBenef.Value : 0;
                    retorno += " " + altura + complAltura + " x " + largura + complLargura;
                }

                if (EspessuraBenef > 0)
                    retorno += " Esp. " + EspessuraBenef.Value;

                if (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() && !retorno.ToLower().Contains("redondo"))
                    retorno += " REDONDO";

                if (Beneficiamentos != null && Beneficiamentos.Count > 0)
                    retorno += string.Format("\n{0}", Beneficiamentos.DescricaoBeneficiamentos);

                return retorno;
            }
        }

        public string DescrAmbienteObsProj
        {
            get
            {
                string ambObs = DescrAmbientePedido;

                if (!String.IsNullOrEmpty(ObsProj))
                    return ambObs + " (Obs. Projeto: " + ObsProj + ")";

                return ambObs;
            }
        }

        /// <summary>
        /// Usado para indicar a imagem que será usada para a peça.
        /// </summary>
        public int Item { get; set; }

        /// <summary>
        /// Indica a URL da imagem para salvar/apagar.
        /// </summary>
        public string ImagemUrlSalvarItem
        {
            get { return Utils.GetPecaProducaoVirtualPath + IdProdPed.ToString().PadLeft(10, '0') + "_" + Item + ".jpg"; }
        }

        /// <summary>
        /// Indica a URL da imagem para salvar/apagar.
        /// </summary>
        public string ImagemUrlSalvar
        {
            get { return Utils.GetPecaProducaoVirtualPath + IdProdPed.ToString().PadLeft(10, '0') + ".jpg"; }
        }

        /// <summary>
        /// Indica a URL da imagem que será usada no sistema, nas telas de produção.
        /// </summary>
        public string ImagemUrl
        {
            get 
            {
                if (PedidoReposicaoDAO.Instance.IsPedidoReposicao(IdPedido))
                {
                    string numEtiqRepos = ProdutosPedidoEspelhoDAO.Instance.ObtemNumEtiquetaRepos(IdProdPed);

                    if (!String.IsNullOrEmpty(numEtiqRepos))
                    {
                        ProdutoPedidoProducao prodPedProducao = ProdutoPedidoProducaoDAO.Instance.GetForImagemPeca(numEtiqRepos);
                        return prodPedProducao != null ? prodPedProducao.ImagemPecaUrl : String.Empty;
                    }

                    return String.Empty;
                }
                else
                {
                    string nomeImagem = ImagemUrlSalvarItem;
                    if (Utils.ArquivoExiste(nomeImagem))
                        return nomeImagem;

                    nomeImagem = ImagemUrlSalvar;
                    if (Utils.ArquivoExiste(nomeImagem))
                        return nomeImagem;

                    if ((IdItemProjeto == null || IdItemProjeto == 0) && (IdMaterItemProj == null || IdMaterItemProj == 0))
                    {
                        // Chamado 16226: Ao invés de retornar String.Empty neste ponto, verifica se tem alguma imagem associada ao produto deste item
                        var urlCadProd = Utils.GetProdutosVirtualPath + IdProd + ".jpg";
                        return System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(urlCadProd)) ? urlCadProd : string.Empty;
                    }
                    else if (IdItemProjeto == null || IdItemProjeto == 0)
                        IdItemProjeto = MaterialItemProjetoDAO.Instance.GetIdItemProjeto(IdMaterItemProj.Value);

                    // Chamado 9590.
                    if (!ItemProjetoDAO.Instance.Exists(IdItemProjeto.Value))
                        return "";

                    uint idProjetoModelo = ItemProjetoDAO.Instance.GetIdProjetoModelo(IdItemProjeto.Value);

                    if (IdMaterItemProj == null)
                        return "";

                    uint? idPecaItemProj = MaterialItemProjetoDAO.Instance.GetIdPecaByMaterial(IdMaterItemProj.Value);

                    if (idPecaItemProj == null || idPecaItemProj == 0 || PecaItemProjetoDAO.Instance.ObtemTipo(idPecaItemProj.Value) == 2)
                        return "";

                    if (Item > 0)
                    {
                        nomeImagem = UtilsProjeto.GetFiguraAssociadaUrl(IdItemProjeto.Value, idProjetoModelo, idPecaItemProj.Value, Item);
                        if (Utils.ArquivoExiste(nomeImagem))
                        {
                            if (System.Web.HttpContext.Current != null)
                                return System.Web.HttpContext.Current.Request.Url.ToString().ToLower().Contains("/glass/") ? nomeImagem.Replace("../../", "../") : nomeImagem;
                            else
                                return nomeImagem;                            
                        }
                    }

                    /* 
                     * Imagem do modelo do projeto
                     * 
                
                    nomeImagem = UtilsProjeto.GetFiguraAssociadaUrl(_idItemProjeto.Value, idProjetoModelo, idPecaItemProj.Value, 0);
                    if (Utils.ArquivoExiste(nomeImagem))
                        return nomeImagem;
                 
                     */

                    return "";
                }
            }
        }

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

        public string LegendaImagemPeca
        {
            get
            {
                return !String.IsNullOrEmpty(EtiquetasLegenda) ? "Etiqueta" + (EtiquetasLegenda.IndexOf(", ") > -1 ? "s" : "") +
                    ": " + EtiquetasLegenda : "";
            }
        }

        public double QtdeVidro
        {
            get { return bool.Parse(IsVidro) ? QtdeSomada : 0; }
        }

        public bool IsProdutoLaminadoComposicao
        {
            get
            {
                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd);

                return tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
            }
        }

        public bool IsProdFilhoLamComposicao
        {
            get { return IdProdPedParent.GetValueOrDefault(0) > 0; }
        }

        public double QtdeImpressaoProdLamComposicao
        {
            get
            {
                if (!IsProdutoLaminadoComposicao)
                    return 0;

                return ProdutosPedidoEspelhoDAO.Instance.ObterQtdePecasParaImpressaoComposicao((int)IdProdPed);
            }
        }

        /// <summary>
        /// IdProdPedParent do produto pedido original
        /// </summary>
        public uint? IdProdPedParentOrig { get; set; }

        public float PesoResumoCorte
        {
            get { return Peso; }
        }
        #endregion

        #region Propriedades do Beneficiamento

        private List<ProdutoPedidoEspelhoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdProdPed > 0)
                    {
                        if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                            _beneficiamentos = new List<ProdutoPedidoEspelhoBenef>();

                        if (_beneficiamentos == null)
                            _beneficiamentos = new List<ProdutoPedidoEspelhoBenef>(ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(IdProdPed));
                    }
                    else if (_beneficiamentos == null)
                        _beneficiamentos = new List<ProdutoPedidoEspelhoBenef>();
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoPedidoEspelhoBenef>();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion

        #region IProdutoCalculo

        uint IResumoCorte.Id
        {
            get { return IdProdPed; }
        }

        IContainerCalculo IProdutoCalculo.Container { get; set; }
        IAmbienteCalculo IProdutoCalculo.Ambiente { get; set; }
        IDadosProduto IProdutoCalculo.DadosProduto { get; set; }

        uint IProdutoCalculo.Id
        {
            get { return IdProdPed; }
        }

        uint? IProdutoCalculo.IdAmbiente
        {
            get { return IdAmbientePedido; }
        }

        decimal IProdutoCalculo.ValorUnit
        {
            get { return ValorVendido; }
            set { ValorVendido = value; }
        }

        uint IProdutoCalculo.IdProduto
        {
            get { return IdProd; }
        }

        public int QtdeAmbiente
        {
            get { return PedidoMaoObra ? AmbientePedidoEspelhoDAO.Instance.GetQtde(IdAmbientePedido) : 1; }
        }

        float IProdutoCalculo.AlturaCalc
        {
            get { return Altura; }
        }

        int? IProdutoCalculo.AlturaBenef
        {
            get { return AlturaBenef; }
        }

        int? IProdutoCalculo.LarguraBenef
        {
            get { return LarguraBenef; }
        }

        decimal IProdutoCalculo.ValorTabelaPedido
        {
            get { return 0; }
        }

        decimal IProdutoCalculo.CustoProd { get; set; }

        #endregion
    }
}