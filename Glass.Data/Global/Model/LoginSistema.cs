using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LoginSistemaDAO))]
    [PersistenceClass("login_sistema")]
    public class LoginSistema
    {
        #region Enumeradores

        public enum TipoEnum
        {
            Entrou = 1,
            Saiu
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDLOGIN", PersistenceParameterType.IdentityKey)]
        public uint IdLogin { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("TIPO")]
        public TipoEnum Tipo { get; set; }

        [PersistenceProperty("MANUAL")]
        public bool Manual { get; set; }

        [PersistenceProperty("USUARIOSYNC")]
        public string UsuarioSync { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSaidaManual
        {
            get { return Tipo != TipoEnum.Saiu || Manual ? "" : "(inatividade)"; }
        }

        #endregion
    }
}