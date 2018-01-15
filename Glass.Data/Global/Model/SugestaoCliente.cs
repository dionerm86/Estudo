using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Poss�veis tipos de sugest�o.
    /// </summary>
    public enum TipoSugestao
    {
        /// <summary>
        /// Reclama��o.
        /// </summary>
        [Description("Reclama��o")]
        Reclamacao = 1,
        /// <summary>
        /// Sugest�o.
        /// </summary>
        [Description("Sugest�o")]
        Sugestao,
        /// <summary>
        /// Negocia��o.
        /// </summary>
        [Description("Negocia��o")]
        Negociacao,
        /// <summary>
        /// Outros.
        /// </summary>
        [Description("Outros")]
        Outros,
        /// <summary>
        /// Perfil.
        /// </summary>
        [Description("Perfil")]
        Perfil,
        /// <summary>
        /// Cobran�a.
        /// </summary>
        [Description("Cobran�a")]
        Cobranca
    }

    [PersistenceBaseDAO(typeof(SugestaoClienteDAO))]
	[PersistenceClass("sugestao_cliente")]
	public class SugestaoCliente : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDSUGESTAO", PersistenceParameterType.IdentityKey)]
        public int IdSugestao { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// 1-Reclama��o
        /// 2-Sugest�o
        /// 3-Negocia��o
        /// 4-Outros
        /// 5-Perfil
        /// </summary>
        [PersistenceProperty("TIPO")]
        public TipoSugestao TipoSugestao { get; set; }

        [Log("Descri��o")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Cancelada")]
        [PersistenceProperty("CANCELADA")]
        public bool Cancelada { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public int? IdPedido { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint? IdOrcamento { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NOMEFUNCIONARIO", DirectionParameter.InputOptional)]
        public string NomeFuncionario { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Tipo")]
        public string DescrTipoSugestao
        {
            get
            {
                return Colosoft.Translator.Translate(TipoSugestao).Format();
            }
        }

        public string DescrSituacao
        {
            get { return Cancelada ? "Cancelada" : "Ativa"; }
        }

        #endregion
    }
}