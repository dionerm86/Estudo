using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Poss�veis tipos de c�lculo do beneficiamento.
    /// </summary>
    public enum TipoCalculoBenef
    {
        /// <summary>
        /// Metro quadrado.
        /// </summary>
        [Description("m�")]
        MetroQuadrado = 1,
        /// <summary>
        /// Metro linear.
        /// </summary>
        [Description("ML")]
        MetroLinear,
        /// <summary>
        /// Quantidade.
        /// </summary>
        [Description("Qtd")]
        Quantidade,
        /// <summary>
        /// Porcentagem
        /// </summary>
        [Description("%")]
        Porcentagem
    }

    /// <summary>
    /// Poss�veis tipos de control de beneficiamento.
    /// </summary>
    public enum TipoControleBenef
    {
        /// <summary>
        /// Lapida��o.
        /// </summary>
        [Description("Lapida��o")]
        Lapidacao = 1,
        /// <summary>
        /// Bisote.
        /// </summary>
        [Description("Bisote")]
        Bisote,
        /// <summary>
        /// Sele��o simples.
        /// </summary>
        [Description("Sele��o Simples")]
        SelecaoSimples,
        /// <summary>
        /// Sele��o Multipla Inclusiva.
        /// </summary>
        [Description("Sele��o m�ltipla inclusiva")]
        SelecaoMultiplaInclusiva,
        /// <summary>
        /// Sele��o Multipla Exclusiva.
        /// </summary>
        [Description("Sele��o m�ltipla exclusiva")]
        SelecaoMultiplaExclusiva,
        /// <summary>
        /// Lista de Sele��o.
        /// </summary>
        [Description("Lista de Sele��o")]
        ListaSelecao,
        /// <summary>
        /// Quantidade.
        /// </summary>
        [Description("Quantidade")]
        Quantidade,
        /// <summary>
        /// Lista sele��o c/ qtd.
        /// </summary>
        [Description("Lista sele��o c/ qtd")]
        ListaSelecaoQtd,
    }

    /// <summary>
    /// Poss�veis tipos de espessura do beneficiamento.
    /// </summary>
    public enum TipoEspessuraBenef
    {
        /// <summary>
        /// Item n�o possui.
        /// </summary>
        [Description("Item n�o possui espessura")]
        ItemNaoPossui,
        /// <summary>
        /// Item possui.
        /// </summary>
        [Description("Item possui")]
        ItemPossui,
        /// <summary>
        /// Item � espessura.
        /// </summary>
        [Description("Item � espessura")]
        ItemEEspessura
    }

    /// <summary>
    /// Poss�veis tipos de beneficiamento.
    /// </summary>
    public enum TipoBenef
    {
        /// <summary>
        /// Todos.
        /// </summary>
        Todos = 0,
        /// <summary>
        /// Venda.
        /// </summary>
        [Description("Venda")]
        Venda,
        /// <summary>
        /// M�o de obra especial
        /// </summary>
        [Description("M�o-de-obra Especial")]
        MaoDeObraEspecial
    }

    [PersistenceBaseDAO(typeof(BenefConfigDAO))]
	[PersistenceClass("benef_config")]
    [Colosoft.Data.Schema.Cache]
	public class BenefConfig : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDBENEFCONFIG", PersistenceParameterType.IdentityKey)]
        public int IdBenefConfig { get; set; }

        [Log("Pai", "Nome", typeof(BenefConfigDAO))]
        [PersistenceProperty("IDPARENT")]
        [PersistenceForeignKey(typeof(BenefConfig), "IdBenefConfig")]
        public int? IdParent { get; set; }

        [Log("Nome")]
        [PersistenceProperty("NOME")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Nome { get; set; }

        [Log("Descricao")]
        [PersistenceProperty("DESCRICAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Descricao { get; set; }

		private TipoControleBenef _tipoControle;

        /// <summary>
        /// 1-Lapidacao
        /// 2-Bisote
        /// 3-SelecaoSimples
        /// 4-SelecaoMultiplaInclusiva
        /// 5-SelecaoMultiplaExclusiva
        /// 6-ListaSelecao
        /// 7-Quantidade
        /// 8-ListaSelecaoQtd
        /// </summary>
		[PersistenceProperty("TIPOCONTROLE")]
        public TipoControleBenef TipoControle
		{
            get { return TipoControleParent != null ? TipoControleParent.Value : _tipoControle; }
            set { _tipoControle = value; }
		}

        private TipoCalculoBenef _tipoCalculo;

		[PersistenceProperty("TIPOCALCULO")]
        public TipoCalculoBenef TipoCalculo
		{
            get { return TipoCalculoParent != null ? TipoCalculoParent.Value : _tipoCalculo; }
            set { _tipoCalculo = value; }
		}

        /// <summary>
        /// 0-Item n�o possui
        /// 1-Item possui
        /// 2-Item � espessura
        /// </summary>
        [PersistenceProperty("TIPOESPESSURA")]
        public TipoEspessuraBenef TipoEspessura { get; set; }

        [Log("Cobran�a opcional")]
        [PersistenceProperty("COBRANCAOPCIONAL")]
        public bool CobrancaOpcional { get; set; }

        [PersistenceProperty("NUMSEQ")]
        [Colosoft.Data.Schema.CacheIndexed]
        public int NumSeq { get; set; }

        /// <summary>
        /// 1 - Ativo
        /// 2 - Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [Log("Cobrar �rea m�nima")]
        [PersistenceProperty("COBRARAREAMINIMA")]
        public bool CobrarAreaMinima { get; set; }

        [Log("Aplica��o", "CodInterno", typeof(EtiquetaAplicacaoDAO))]
        [PersistenceProperty("IDAPLICACAO")]
        [PersistenceForeignKey(typeof(EtiquetaAplicacao), "IdAplicacao")]
        public int? IdAplicacao { get; set; }

        [Log("Processo", "CodInterno", typeof(EtiquetaProcessoDAO))]
        [PersistenceProperty("IDPROCESSO")]
        [PersistenceForeignKey(typeof(EtiquetaProcesso), "IdProcesso")]
        public int? IdProcesso { get; set; }

        [Log("Produto", "CodInterno", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int? IdProd { get; set; }

        [Log("Acr�scimo Altura")]
        [PersistenceProperty("ACRESCIMOALTURA")]
        public int AcrescimoAltura { get; set; }

        [Log("Acr�scimo Largura")]
        [PersistenceProperty("ACRESCIMOLARGURA")]
        public int AcrescimoLargura { get; set; }

        [Log("N�o exibir descri��o do beneficiamento na etiqueta?")]
        [PersistenceProperty("NAOEXIBIRETIQUETA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public bool NaoExibirEtiqueta { get; set; }
        
        [PersistenceProperty("TIPOBENEF")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoBenef TipoBenef { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRSUBGRUPO", DirectionParameter.InputOptional)]
        public string DescrSubgrupo { get; set; }

        private string _descrBenefValor;

        [PersistenceProperty("DESCRBENEFVALOR", DirectionParameter.InputOptional)]
        public string DescrBenefValor
        {
            get 
            { 
                return !String.IsNullOrEmpty(_descrBenefValor) && _tipoCalculo > 0 ? _descrBenefValor + " (" + DescrTipoCalculo + ")" : 
                    _descrBenefValor; 
            }
            set { _descrBenefValor = value; }
        }

        [PersistenceProperty("NUMCHILD", DirectionParameter.InputOptional)]
        public long NumChild { get; set; }

        private int _numSeqMin;

        [PersistenceProperty("NUMSEQMIN", DirectionParameter.InputOptional)]
        public int NumSeqMin
        {
            get { return _numSeqMin > 0 ? _numSeqMin : 1; }
            set { _numSeqMin = value; }
        }

        [PersistenceProperty("NUMSEQMAX", DirectionParameter.InputOptional)]
        public int NumSeqMax { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("DESCRPARENT", DirectionParameter.InputOptional)]
        public string DescricaoParent { get; set; }

        [PersistenceProperty("TIPOCONTROLEPARENT", DirectionParameter.InputOptional)]
        public TipoControleBenef? TipoControleParent { get; set; }

        [PersistenceProperty("TIPOCALCULOPARENT", DirectionParameter.InputOptional)]
        public TipoCalculoBenef? TipoCalculoParent { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region M�todos internos est�ticos

        internal static string GetDescrTipoCalculo(int tipoCalculo)
        {
            return tipoCalculo == 1 ? "m�" : tipoCalculo == 2 ? "ML" : tipoCalculo == 3 ? "Qtd" :
                tipoCalculo == 4 ? "%" : String.Empty;
        }

        #endregion

        [Log("Cobrar por espessura")]
        public bool CobrarPorEspessura { get; set; }

        [Log("Cobrar por cor")]
        public bool CobrarPorCor { get; set; }

        public uint? IdSubgrupoProd { get; set; }

        public string ListaSelecao { get; set; }

        public string ListaItens { get; set; }

        /// <summary>
        /// Utilizado para salvar as op��es de uma listbox
        /// </summary>
        public string[] LstOpcoes
        {
            get 
            {
                var lstOpcoes = BenefConfigDAO.Instance.GetOpcoes((uint)IdBenefConfig);
                var vetOpcoes = new string[lstOpcoes.Length];
                for (var i = 0; i < lstOpcoes.Length; i++)
                    vetOpcoes[i] = lstOpcoes[i].Nome.Replace(Nome + " ", "");

                return vetOpcoes;
            }
        }

        public string[] LstItens
        {
            get
            {
                var lstOpcoes = BenefConfigDAO.Instance.GetOpcoes((uint)IdBenefConfig);
                var vetRetorno = new string[lstOpcoes.Length];

                for (var i = 0; i < lstOpcoes.Length; i++)
                {
                    var dadosProc = ";";
                    if (lstOpcoes[i].IdProcesso > 0)
                        dadosProc = lstOpcoes[i].IdProcesso.Value + ";" + EtiquetaProcessoDAO.Instance.ObtemCodInterno((uint)lstOpcoes[i].IdProcesso.Value);

                    var dadosApl = ";";
                    if (lstOpcoes[i].IdAplicacao > 0)
                        dadosApl = lstOpcoes[i].IdAplicacao.Value + ";" + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno((uint)lstOpcoes[i].IdAplicacao.Value);

                    var dadosProd = ";;";
                    if (lstOpcoes[i].IdProd > 0)
                        dadosProd = lstOpcoes[i].IdProd.Value + ";" + ProdutoDAO.Instance.GetCodInterno((int)lstOpcoes[i].IdProd.Value) + ";" +
                            ProdutoDAO.Instance.ObtemDescricao((int)lstOpcoes[i].IdProd.Value);

                    var acrescimoAltura = ";";
                    if (lstOpcoes[i].AcrescimoAltura > 0)
                        acrescimoAltura = lstOpcoes[i].AcrescimoAltura.ToString();

                    var acrescimoLargura = ";";
                    if (lstOpcoes[i].AcrescimoLargura > 0)
                        acrescimoLargura = lstOpcoes[i].AcrescimoLargura.ToString();

                    vetRetorno[i] = dadosProc + ";" + dadosApl + ";" + dadosProd + ";" + acrescimoAltura + ";" + acrescimoLargura;
                }

                return vetRetorno;
            }
        }

        [Log("Controle")]
        public string DescrTipoControle
        {
            get 
            {
                return Colosoft.Translator.Translate(_tipoControle).Format();
            }
        }

        [Log("C�lculo")]
        public string DescrTipoCalculo
        {
            get { return Colosoft.Translator.Translate(_tipoCalculo).Format(); }
        }

        [Log("Situa��o")]
        public string DescrSituacao
        {
            get { return Colosoft.Translator.Translate(Situacao).Format(); }
        }

        public bool Calcular
        {
            get
            {
                if (IdParent == null)
                    return NumChild == 0;
                else
                    return TipoEspessura != TipoEspessuraBenef.ItemPossui;
            }
        }

        public string DescricaoCompleta
        {
            get 
            {
                return DescricaoParent == null || Descricao.ToLower().IndexOf(DescricaoParent.ToLower()) > -1 ? 
                    Descricao : DescricaoParent + " " + Descricao;
            }
        }

        [Log("Lista de op��es")]
        internal string ListaOpcoesLog
        {
            get
            {
                var vetOpcoes = new List<String>();
                var vetItens = new List<String>();

                if (!String.IsNullOrEmpty(ListaSelecao) && !String.IsNullOrEmpty(ListaItens))
                {
                    vetOpcoes.AddRange(ListaSelecao.TrimEnd('|').Split('|'));
                    vetItens.AddRange(ListaItens.TrimEnd('|').Split('|'));
                }
                else
                {
                    vetOpcoes.AddRange(LstOpcoes);
                    vetItens.AddRange(LstItens);

                    for (var i = 0; i < vetItens.Count; i++)
                        vetItens[i] = vetItens[i].Split(';')[0] + ";" + vetItens[i].Split(';')[2] + ";;" +
                            vetItens[i].Split(';')[5] + ";" + vetItens[i].Split(';')[6] + ";" +
                            vetItens[i].Split(';')[7] + ";" + vetItens[i].Split(';')[8];
                }

                var retorno = "";
                for (var i = 0; i < vetOpcoes.Count; i++)
                {
                    var dadosItens = vetItens[i].Split(';');
                    
                    var itens = (!String.IsNullOrEmpty(dadosItens[0]) ? ", Proc: " + EtiquetaProcessoDAO.Instance.ObtemCodInterno(Glass.Conversoes.StrParaUint(dadosItens[0])) + "" : "") +
                        (!String.IsNullOrEmpty(dadosItens[1]) ? ", Apl: " + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(Glass.Conversoes.StrParaUint(dadosItens[1])) + "" : "") +
                        (!String.IsNullOrEmpty(dadosItens[3]) ? ", Produto Compra: " + dadosItens[3] + " - " + dadosItens[4] + "" : "") +
                        (!String.IsNullOrEmpty(dadosItens[5]) ? ", Acr�s. Alt.: " + dadosItens[5] + "" : "") +
                        (!String.IsNullOrEmpty(dadosItens[6]) ? ", Acr�s. Larg.: " + dadosItens[6] + "" : "");


                    retorno += vetOpcoes[i] + (!String.IsNullOrEmpty(itens) ? " (" + itens.TrimStart(' ', ',') + ")" : "") + ", ";
                }

                return retorno.TrimEnd(',', ' ');
            }
        }

        public bool IsRedondo
        {
            get { return Nome == "Redondo"; }
        }

        [Log("Tipo do Beneficiamento")]
        public string DescrTipoBenef
        {
            get
            {
                return Colosoft.Translator.Translate(TipoBenef).Format();
            }
        }

        #region Relat�rio

        public string Criterio { get; set; }

        [PersistenceProperty("QtdBenef", DirectionParameter.InputOptional)]
        public double QtdBenef { get; set; }

        [PersistenceProperty("SumValor", DirectionParameter.InputOptional)]
        public decimal SumValor { get; set; }

        [PersistenceProperty("SumTotM", DirectionParameter.InputOptional)]
        public double SumTotM { get; set; }

        #endregion

        #endregion
    }
}