using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Xml.Serialization;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MaterialItemProjetoDAO))]
	[PersistenceClass("material_item_projeto")]
	public class MaterialItemProjeto : IProdutoCalculo, IMaterialItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDMATERITEMPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdMaterItemProj { get; set; }

        [PersistenceProperty("IDMATERITEMPROJORIG", DirectionParameter.Input)]
        public uint? IdMaterItemProjOrig { get; set; }

        [PersistenceProperty("IDMATERPROJMOD")]
        public uint? IdMaterProjMod { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint IdItemProjeto { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPECAITEMPROJ")]
        public uint? IdPecaItemProj { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("ALTURA")]
        public Single Altura { get; set; }

        [PersistenceProperty("ALTURACALC")]
        public Single AlturaCalc { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM")]
        public Single TotM { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("ALIQICMS")]
        public float AliqIcms { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliquotaIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        private float _totM2Calc;

        [PersistenceProperty("TOTM2CALC")]
        public float TotM2Calc
        {
            get { return _totM2Calc != 0 ? _totM2Calc : TotM; }
            set { _totM2Calc = value; }
        }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("VALORDESCONTOCLIENTE")]
        public decimal ValorDescontoCliente { get; set; }

        [PersistenceProperty("VALORACRESCIMOCLIENTE")]
        public decimal ValorAcrescimoCliente { get; set; }

        [PersistenceProperty("VALORUNITBRUTO")]
        public decimal ValorUnitarioBruto { get; set; }

        [PersistenceProperty("TOTALBRUTO")]
        public decimal TotalBruto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("GrauCorte")]
        public GrauCorteEnum? GrauCorte { get; set; }

        #endregion

        #region Propriedades Estendidas

        #region Totais

        [XmlIgnore]
        [PersistenceProperty("SumQtde", DirectionParameter.InputOptional)]
        public long SumQtde
        {
            get { return Convert.ToInt64(Qtde); }
            set { Qtde = Convert.ToInt32(value); }
        }

        [XmlIgnore]
        [PersistenceProperty("SumCusto", DirectionParameter.InputOptional)]
        public decimal SumCusto
        {
            get { return Custo; }
            set { Custo = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("SumTotal", DirectionParameter.InputOptional)]
        public decimal SumTotal
        {
            get { return Total; }
            set { Total = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("SumAltura", DirectionParameter.InputOptional)]
        public double SumAltura
        {
            get { return Convert.ToDouble(Altura); }
            set { Altura = Convert.ToSingle(value); }
        }

        [XmlIgnore]
        [PersistenceProperty("SumTotM", DirectionParameter.InputOptional)]
        public double SumTotM
        {
            get { return Convert.ToDouble(TotM); }
            set { TotM = Convert.ToSingle(value); }
        }

        #endregion

        /// <summary>
        /// Verifica se o produto associadao à esta material está inativo
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("PRODUTOINATIVO", DirectionParameter.InputOptional)]
        public bool ProdutoInativo { get; set; }

        [XmlIgnore]
        [PersistenceProperty("AMBIENTE", DirectionParameter.InputOptional)]
        public string Ambiente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DESCRAMBIENTE", DirectionParameter.InputOptional)]
        public string DescrAmbiente { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DescrGrupoProd", DirectionParameter.InputOptional)]
        public string DescrGrupoProd { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint IdSubgrupoProd { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        private string _m2Minimo;

        [XmlIgnore]
        [PersistenceProperty("AreaMinima", DirectionParameter.InputOptional)]
        public string M2Minimo
        {
            get { return !String.IsNullOrEmpty(_m2Minimo) ? _m2Minimo.ToString().Replace(',', '.') : "0"; }
            set { _m2Minimo = value; }
        }

        [PersistenceProperty("CODMATERIAL", DirectionParameter.InputOptional)]
        public string CodMaterial { get; set; }

        /// <summary>
        /// Quantidade deste material que deve haver neste modelo
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("QTDMODELO", DirectionParameter.InputOptional)]
        public int QtdModelo { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CALCULOQTDE", DirectionParameter.InputOptional)]
        public string CalculoQtde { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CALCULOALTURA", DirectionParameter.InputOptional)]
        public string CalculoAltura { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeSomada", DirectionParameter.InputOptional)]
        public decimal QtdeSomada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeComprada", DirectionParameter.InputOptional)]
        public long QtdeComprada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("EditDeleteVisible", DirectionParameter.InputOptional)]
        public bool EditDeleteVisible { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public ulong IdCliente { get; set; }

        [PersistenceProperty("Ncm", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        [PersistenceProperty("CustoCompraProduto", DirectionParameter.InputOptional)]
        public decimal CustoCompraProduto { get; set; }

        [PersistenceProperty("ITEM", DirectionParameter.InputOptional)]
        public string Item { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDORCAMENTO", DirectionParameter.InputOptional)]
        public uint IdOrcamento { get; set; }

        #endregion

        #region Propriedades de Suporte

        [XmlIgnore]
        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString(); }
        }

        [XmlIgnore]
        public string IsAluminio
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd).ToString().ToLower(); }
        }

        [XmlIgnore]
        public string IsFerragem
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsFerragem((int)IdGrupoProd).ToString().ToLower(); }
        }

        [XmlIgnore]
        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)this.IdProd); }
        }

        [XmlIgnore]
        public bool AlturaEnabled
        {
            get 
            { 
                bool alturaEnabled = Glass.Calculos.AlturaEnabled(TipoCalc);

                // Não permite alterar altura de peças de vidro associadas a alguma peca_item_projeto
                return alturaEnabled && (IdPecaItemProj == null || !ItemProjetoDAO.Instance.IsBoxPadrao(IdItemProjeto)) &&
                    (IdPecaItemProj == null || !PedidoConfig.LiberarPedido);
            }
        }

        [XmlIgnore]
        public bool LarguraEnabled
        {
            get 
            { 
                bool larguraEnabled = Glass.Calculos.LarguraEnabled(TipoCalc);

                // Não permite alterar altura de peças de vidro associadas a alguma peca_item_projeto
                return larguraEnabled && (IdPecaItemProj == null || !PedidoConfig.LiberarPedido);
            }
        }

        /// <summary>
        /// Campo utilizado para importar beneficiamento quando for fazer conferência deste projeto
        /// </summary>
        [XmlIgnore]
        public uint IdProdPed { get; set; }

        [XmlIgnore]
        public string AlturaRpt
        {
            get 
            {  
                string alt = TipoCalc == 4 ? Altura.ToString("N2") + "ml" : Altura.ToString();

                return alt;
            }
        }

        [XmlIgnore]
        public bool EditVisible
        {
            get { return EditDeleteVisible; }
        }

        [XmlIgnore]
        public bool DeleteVisible
        {
            get { return EditDeleteVisible && IdPecaItemProj == null; }
        }

        [XmlIgnore]
        public string AlturaLista
        {
            get
            {
                if (IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Alumínio)
                    return Altura.ToString();
                else
                    return Altura != AlturaCalc ? (AlturaCalc > 0 ? Altura.ToString() + " (" + AlturaCalc.ToString() + ")" : Altura.ToString()) : Altura.ToString();
            }
        }

        [XmlIgnore]
        public bool BenefVisible
        {
            get
            {
                return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro();
            }
        }

        [XmlIgnore]
        public int QtdeComprar
        {
            get { return (int)Qtde - (int)QtdeComprada; }
        }

        [XmlIgnore]
        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        [XmlIgnore]
        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        [XmlIgnore]
        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaRpt : Largura.ToString(); }
        }

        [XmlIgnore]
        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura.ToString() : AlturaRpt; }
        }

        [XmlIgnore]
        public float TotM2SemChapa
        {
            get { return Glass.Global.CalculosFluxo.CalcM2Calculo((uint)IdCliente, (int)Altura, Largura, Qtde, (int)IdProd, Redondo, Beneficiamentos.CountAreaMinima, ProdutoDAO.Instance.ObtemAreaMinima((int)IdProd), false, 0, true); }
        }

        #endregion

        #region Propriedades do Beneficiamento

        private List<MaterialProjetoBenef> _beneficiamentos = null;

        [XmlIgnore]
        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                        _beneficiamentos = new List<MaterialProjetoBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = MaterialProjetoBenefDAO.Instance.GetByMaterial(IdMaterItemProj);
                }
                catch
                {
                    _beneficiamentos = new List<MaterialProjetoBenef>();
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

        /// <summary>
        /// Usado para exportação de pedido.
        /// </summary>
        public string ServicosInfoBenef
        {
            get
            {
                GenericBenefCollection benef = Beneficiamentos.ToMateriaisProjeto(IdMaterItemProj);
                foreach (GenericBenef b in benef)
                    b.IdBenefConfig = 0;

                return benef.ServicosInfo;
            }
            set { Beneficiamentos.ServicosInfo = value; }
        }

        #endregion

        #region IProdutoCalculo

        [XmlIgnore]
        IContainerCalculo IProdutoCalculo.Container { get; set; }

        [XmlIgnore]
        IAmbienteCalculo IProdutoCalculo.Ambiente { get; set; }

        [XmlIgnore]
        IDadosProduto IProdutoCalculo.DadosProduto { get; set; }

        [XmlIgnore]
        uint IProdutoCalculo.Id
        {
            get { return IdMaterItemProj; }
        }

        [XmlIgnore]
        uint? IProdutoCalculo.IdAmbiente
        {
            get { return null; }
        }

        [XmlIgnore]
        uint IProdutoCalculo.IdProduto
        {
            get { return IdProd; }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.ValorUnit
        {
            get { return Valor; }
            set { Valor = value; }
        }

        [XmlIgnore]
        public decimal ValorAcrescimoProd { get; set; }

        [XmlIgnore]
        public decimal ValorDescontoProd { get; set; }

        [XmlIgnore]
        public int QtdeAmbiente
        {
            get { return 1; }
        }

        [XmlIgnore]
        public decimal ValorDescontoQtde
        {
            get { return 0; }
            set
            {
                // não faz nada
            }
        }

        [XmlIgnore]
        public float PercDescontoQtde
        {
            get { return 0; }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.ValorComissao
        {
            get { return 0; }
            set
            {
                // não faz nada
            }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.PercentualComissao
        {
            get
            {
                if (PedidoConfig.Comissao.ComissaoPedido && PedidoConfig.Comissao.ComissaoAlteraValor)
                {
                    var idOrcamento = IdOrcamento > 0 ? IdOrcamento : ItemProjetoDAO.Instance.GetIdOrcamento(null, IdItemProjeto);

                    if (idOrcamento > 0)
                    {
                        return (decimal)OrcamentoDAO.Instance.RecuperaPercComissao(null, idOrcamento);
                    }

                    var idPedido = ItemProjetoDAO.Instance.ObtemIdPedido(null, IdItemProjeto);

                    if (idPedido > 0)
                    {
                        return (decimal)PedidoDAO.Instance.ObterPercentualComissao(null, (int)idPedido);
                    }

                    var idPedidoEspelho = ItemProjetoDAO.Instance.ObtemIdPedidoEspelho(null, IdItemProjeto);

                    if (idPedidoEspelho > 0)
                    {
                        return (decimal)PedidoEspelhoDAO.Instance.RecuperaPercComissao(null, idPedidoEspelho.Value);
                    }
                }

                return 0;
            }
        }

        [XmlIgnore]
        int? IProdutoCalculo.AlturaBenef
        {
            get { return 0; }
        }

        [XmlIgnore]
        int? IProdutoCalculo.LarguraBenef
        {
            get { return 0; }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.ValorTabelaPedido
        {
            get { return 0; }
        }

        Guid? IMaterialItemProjeto.IdPecaItemProj
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.CustoProd
        {
            get { return Custo; }
            set { Custo = value; }
        }

        #endregion
    }
}
