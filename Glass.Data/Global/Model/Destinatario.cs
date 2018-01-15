using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DestinatarioDAO))]
    [PersistenceClass("destinatario")]
    public class Destinatario : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMENSAGEM", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Mensagem), "IdMensagem")]
        public int IdMensagem { get; set; }

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }

        [PersistenceProperty("LIDA")]
        public bool Lida { get; set; }

        [PersistenceProperty("CANCELADA")]
        public bool Cancelada { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEDEST", DirectionParameter.InputOptional)]
        public string NomeDest { get; set; }

        [PersistenceProperty("TIPOFUNCDEST", DirectionParameter.InputOptional)]
        public string TipoFuncDest { get; set; }

        #endregion
    }
}