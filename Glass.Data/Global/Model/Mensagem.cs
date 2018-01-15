using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MensagemDAO))]
    [PersistenceClass("mensagem")]
    public class Mensagem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMENSAGEM", PersistenceParameterType.IdentityKey)]
        public int IdMensagem { get; set; }

        [PersistenceProperty("IDREMETENTE")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdRemetente { get; set; }

        [PersistenceProperty("ASSUNTO")]
        public string Assunto { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Mensagem()
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