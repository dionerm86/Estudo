using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncModuloDAO))]
    [PersistenceClass("func_modulo")]
    public class FuncModulo : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMODULO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Modulo), "IdModulo")]
        public int IdModulo { get; set; }

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }

        [PersistenceProperty("PERMITIR")]
        public bool Permitir { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescrModulo", DirectionParameter.InputOptional)]
        public string DescrModulo { get; set; }

        [PersistenceProperty("GrupoModulo", DirectionParameter.InputOptional)]
        public string GrupoModulo { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Acesso")]
        public string DescrPermissao
        {
            get { return Permitir ? "Permitir" : !Permitir ? "Negar" : "Padrão"; }
        }

        public uint IdLog
        {
            get { return Glass.Conversoes.StrParaUint(IdModulo.ToString() + IdFunc.ToString().PadLeft(4, '0')); }
        }

        #endregion
    }
}