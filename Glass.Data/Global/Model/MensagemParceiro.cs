using System;
using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("mensagem_parceiro")]
    public class MensagemParceiro : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMENSAGEMPARCEIRO", PersistenceParameterType.IdentityKey)]
        public int IdMensagemParceiro { get; set; }

        [PersistenceProperty("IDREMETENTE")]
        public int IdRemetente { get; set; }

        [PersistenceProperty("ASSUNTO")]
        public string Assunto { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("ISFUNC")]
        public bool IsFunc { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public MensagemParceiro()
        {
            DataCad = DateTime.Now;
        }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEREMETENTE", DirectionParameter.InputOptional)]
        public string NomeRemetente { get; set; }

        [PersistenceProperty("LIDA", DirectionParameter.InputOptional)]
        public bool Lida { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Destinatarios { get; set; }

        public string Situacao
        {
            get { return Lida ? "Lida" : "Não Lida"; }
        }

        #endregion
    }
}