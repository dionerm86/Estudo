using System;
using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis tipos de subgrupo de produtos.
    /// </summary>
    [Colosoft.EmptyDescription("N/D")]
    public enum TipoSubgrupoProd
    {
        /// <summary>
        /// Indefinido.
        /// </summary>
        [Description("Indefinido")]
        Indefinido,

        /// <summary>
        /// Chapas de Vidro.
        /// </summary>
        [Description("Chapas de Vidro")]
        ChapasVidro,

        /// <summary>
        /// PVB.
        /// </summary>
        [Description("PVB")]
        PVB,

        /// <summary>
        /// Chapas de Vidro Laminado.
        /// </summary>
        [Description("Chapas de Vidro Laminado")]
        ChapasVidroLaminado,

        /// <summary>
        /// Modulado.
        /// </summary>
        [Description("Modulado")]
        Modulado,

        /// <summary>
        /// Vidro Laminado.
        /// </summary>
        [Description("Vidro Laminado")]
        VidroLaminado,

        /// <summary>
        /// Vidro Duplo.
        /// </summary>
        [Description("Vidro Duplo")]
        VidroDuplo
    }

    [PersistenceBaseDAO(typeof(SubgrupoProdDAO))]
	[PersistenceClass("subgrupo_prod")]
    [Colosoft.Data.Schema.Cache]
	public class SubgrupoProd : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSUBGRUPOPROD", PersistenceParameterType.IdentityKey)]
        public int IdSubgrupoProd { get; set; }

        [PersistenceProperty("IDGRUPOPROD")]
        [PersistenceForeignKey(typeof(GrupoProd), "IdGrupoProd")]
        public int IdGrupoProd { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Descricao { get; set; }

        /// <summary>
        /// 1-Qtd 
        /// 2-m²
        /// 3-ML
        /// 4-ML AL
        /// </summary>
        [PersistenceProperty("TIPOCALCULO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoCalculoGrupoProd? TipoCalculo { get; set; }

        /// <summary>
        /// 1-Qtd 
        /// 2-m²
        /// 3-ML
        /// 4-ML AL
        /// </summary>
        [PersistenceProperty("TIPOCALCULONF")]
        public TipoCalculoGrupoProd? TipoCalculoNf { get; set; }

        [Log("Bloquear estoque")]
        [PersistenceProperty("BLOQUEARESTOQUE")]
        public bool BloquearEstoque { get; set; }

        [PersistenceProperty("NAOALTERARESTOQUE")]
        public bool NaoAlterarEstoque { get; set; }

        [PersistenceProperty("NAOALTERARESTOQUEFISCAL")]
        public bool NaoAlterarEstoqueFiscal { get; set; }

        [Log("Produtos para estoque")]
        [PersistenceProperty("PRODUTOSESTOQUE")]
        public bool ProdutosEstoque { get; set; }

        [Log("Vidro temperado")]
        [PersistenceProperty("ISVIDROTEMPERADO")]
        public bool IsVidroTemperado { get; set; }

        [Log("Exibir Mensagem Estoque")]
        [PersistenceProperty("EXIBIRMENSAGEMESTOQUE")]
        public bool ExibirMensagemEstoque { get; set; }

        [Log("Número Mínimo Dias Entrega")]
        [PersistenceProperty("NUMERODIASMINIMOENTREGA")]
        public int? NumeroDiasMinimoEntrega { get; set; }

        [PersistenceProperty("DIASEMANAENTREGA")]
        public int? DiaSemanaEntrega { get; set; }

        [Log("Gera Volume?")]
        [PersistenceProperty("GERAVOLUME")]
        public bool GeraVolume { get; set; }

        [PersistenceProperty("TIPOSUBGRUPO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoSubgrupoProd TipoSubgrupo { get; set; }

        [PersistenceProperty("IDCLI")]
        public int? IdCli { get; set; }

        [PersistenceProperty("LiberarPendenteProducao")]
        public bool LiberarPendenteProducao { get; set; }

        /// <summary>
        /// Indica se é permitida a revenda de produtos do tipo venda (solução para inclusão de embalagem no pedido de venda)
        /// </summary>
        [PersistenceProperty("PERMITIRITEMREVENDANAVENDA")]
        public bool PermitirItemRevendaNaVenda { get; set; }

        [PersistenceProperty("BLOQUEARECOMMERCE")]
        public bool BloquearEcommerce { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRGRUPO", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }
        
        #endregion

        #region Propriedades de Suporte

        [Log("Tipo de cálculo")]
        public string DescrCalculo
        {
            get { return Glass.Global.CalculosFluxo.GetDescrTipoCalculo(TipoCalculo, false); }
        }

        [Log("Tipo de cálculo NF")]
        public string DescrCalculoNf
        {
            get { return Glass.Global.CalculosFluxo.GetDescrTipoCalculo(TipoCalculoNf, false); }
        }

        [Log("Alterar estoque")]
        public bool AlterarEstoque
        {
            get { return !NaoAlterarEstoque; }
            set { NaoAlterarEstoque = !value; }
        }

        [Log("Alterar estoque fiscal")]
        public bool AlterarEstoqueFiscal
        {
            get { return !NaoAlterarEstoqueFiscal; }
            set { NaoAlterarEstoqueFiscal = !value; }
        }

        public bool ExibirProdutosEstoque
        {
            get
            {
                return GrupoProdDAO.Instance.IsVidro(IdGrupoProd);
            }
        }

        [Log("Dia Semana Entrega")]
        public string DescrDiaSemanaEntrega
        {
            get
            {
                if (DiaSemanaEntrega == null)
                    return "";

                DateTime temp = DateTime.Now;
                while ((int)temp.DayOfWeek != DiaSemanaEntrega)
                    temp = temp.AddDays(1);

                string retorno = temp.ToString("dddd");
                return retorno[0].ToString().ToUpper()[0] + retorno.Substring(1);
            }
        }

        public bool DeleteVisible
        {
            get { return IdSubgrupoProd > 8; }
        }
        
        public bool IsPadraoSistema
        {
            get { return IdSubgrupoProd <= 8; }
        }

        [Log("Tipo de Subgrupo")]
        public string DescrTipoSubgrupo
        {
            get
            {
                return Colosoft.Translator.Translate(TipoSubgrupo).Format();
            }
        }

        public string DescrGrupoSubGrupo
        {
            get { return DescrGrupo + " - " + Descricao; }
        }

        public int[] IdsLoja
        {
            get
            {
                return SubgrupoProdDAO.Instance.ObterIdsLoja(null, IdSubgrupoProd).ToArray();
            }
        }

        #endregion
	}
}