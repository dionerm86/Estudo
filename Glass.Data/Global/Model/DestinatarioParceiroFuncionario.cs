using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("destinatario_parceiro_funcionario")]
    public class DestinatarioParceiroFuncionario : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMENSAGEMPARCEIRO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(MensagemParceiro), "IdMensagemParceiro")]
        public int IdMensagemParceiro { get; set; }

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }

        [PersistenceProperty("LIDA")]
        public bool Lida { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEDEST", DirectionParameter.InputOptional)]
        public string NomeDest { get; set; }

        [PersistenceProperty("TIPOFUNCDEST", DirectionParameter.InputOptional)]
        public string TipoFuncDest { get; set; }

        #endregion
    }
}