using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Poss�veis tipos de grupo de produto.
    /// </summary>
    [Colosoft.EmptyDescription("N/D")]
    public enum TipoGrupoProd
    {
        /// <summary>
        /// Indefinido.
        /// </summary>
        [Description("Indefinido")]
        Indefinido,
        /// <summary>
        /// Mercao
        /// </summary>
        [Description("Mercadoria")]
        Mercadoria,
        /// <summary>
        /// Uso Consumo.
        /// </summary>
        [Description("Uso/Consumo")]
        UsoConsumo,
        /// <summary>
        /// Diversos.
        /// </summary>
        [Description("Diversos")]
        Diversos
    }

    /// <summary>
    /// Nomes dos grupos padr�es do sistema.
    /// </summary>
    public enum NomeGrupoProd : int
    {
        /// <summary>
        /// Vidro.
        /// </summary>
        [Description("Vidro")]
        Vidro = 1,
        /// <summary>
        /// Alum�nio
        /// </summary>
        [Description("Alum�nio")]
        Alum�nio = 3,
        /// <summary>
        /// Ferragem.
        /// </summary>
        [Description("Ferragem")]
        Ferragem = 4,
        /// <summary>
        /// M�o de Obra.
        /// </summary>
        [Description("M�o de Obra")]
        MaoDeObra = 5,
        /// <summary>
        /// Kit para box.
        /// </summary>
        [Description("Kit para Box")]
        KitParaBox = 6,
        /// <summary>
        /// Pel�cula.
        /// </summary>
        [Description("Pel�cula")]
        Pelicula = 7,
        /// <summary>
        /// Esquadria Alum�nio.
        /// </summary>
        [Description("Esquadria Alum�nio")]
        EsquadriaAluminio = 8,
        /// <summary>
        /// Acr�lico.
        /// </summary>
        [Description("Acr�lico")]
        Acr�lico = 11,
        /// <summary>
        /// Moldura.
        /// </summary>
        [Description("Moldura")]
        Moldura = 12
    }

    [PersistenceBaseDAO(typeof(GrupoProdDAO))]
	[PersistenceClass("grupo_prod")]
    [Colosoft.Data.Schema.Cache]
	public class GrupoProd : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDGRUPOPROD", PersistenceParameterType.IdentityKey)]
        public int IdGrupoProd { get; set; }

        [Log("Descri��o")]
        [PersistenceProperty("DESCRICAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Descricao { get; set; }

        /// <summary>
        /// 1-Qtd 
        /// 2-m�
        /// 3-ML
        /// 4-ML AL
        /// </summary>
        [PersistenceProperty("TIPOCALCULO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoCalculoGrupoProd? TipoCalculo { get; set; }

        /// <summary>
        /// 1-Qtd 
        /// 2-m�
        /// 3-ML
        /// 4-ML AL
        /// </summary>
        [PersistenceProperty("TIPOCALCULONF")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoCalculoGrupoProd? TipoCalculoNf { get; set; }

        [Log("Bloquear estoque")]
        [PersistenceProperty("BLOQUEARESTOQUE")]
        public bool BloquearEstoque { get; set; }

        [PersistenceProperty("NAOALTERARESTOQUE")]
        public bool NaoAlterarEstoque { get; set; }

        [PersistenceProperty("NAOALTERARESTOQUEFISCAL")]
        public bool NaoAlterarEstoqueFiscal { get; set; }

        [PersistenceProperty("TIPOGRUPO")]
        public TipoGrupoProd TipoGrupo { get; set; }

        [Log("Exibir Mensagem Estoque")]
        [PersistenceProperty("EXIBIRMENSAGEMESTOQUE")]
        public bool ExibirMensagemEstoque { get; set; }

        [Log("Gera Volume?")]
        [PersistenceProperty("GERAVOLUME")]
        public bool GeraVolume { get; set; }        

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get { return IdGrupoProd > 10; }
        }

        [Log("Tipo de c�lculo")]
        public string DescrCalculo
        {
            get { return Colosoft.Translator.Translate(TipoCalculo).Format(); }
        }

        [Log("Tipo de c�lculo NF")]
        public string DescrCalculoNf
        {
            get { return Colosoft.Translator.Translate(TipoCalculoNf).Format(); }
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

        [Log("Tipo de grupo")]
        public string DescrTipoGrupo
        {
            get
            {
                return Colosoft.Translator.Translate(TipoGrupo).Format();
            }
        }

        #endregion
    }
}