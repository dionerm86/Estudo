using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoAdministradoraCartaoDAO))]
    [PersistenceClass("pagto_administradora_cartao")]
    public class PagtoAdministradoraCartao : Sync.Fiscal.EFD.Entidade.IPagtoAdministradoraCartao
    {
        #region Propriedades

        [PersistenceProperty("IDPAGTOADMINCARTAO", PersistenceParameterType.IdentityKey)]
        public uint IdPagtoAdminCartao { get; set; }

        [PersistenceProperty("IDADMINCARTAO")]
        public uint IdAdminCartao { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("MES")]
        public int Mes { get; set; }

        [PersistenceProperty("ANO")]
        public int Ano { get; set; }

        [PersistenceProperty("VALORCREDITO")]
        public decimal ValorCredito { get; set; }

        [PersistenceProperty("VALORDEBITO")]
        public decimal ValorDebito { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEADMINCARTAO", DirectionParameter.InputOptional)]
        public string NomeAdminCartao { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrMes
        {
            get { return DataSources.Instance.GetDescrMes(Mes); }
        }

        #endregion

        #region IPagtoAdministradoraCartao Members

        int Sync.Fiscal.EFD.Entidade.IPagtoAdministradoraCartao.CodigoAdministradoraCartao
        {
            get { return (int)IdAdminCartao; }
        }

        int Sync.Fiscal.EFD.Entidade.IPagtoAdministradoraCartao.CodigoLoja
        {
            get { return (int)IdLoja; }
        }

        #endregion
    }
}