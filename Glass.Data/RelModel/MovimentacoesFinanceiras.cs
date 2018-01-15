using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MovimentacoesFinanceirasDAO))]
    [PersistenceClass("movimentacoes_financeiras")]
    public class MovimentacoesFinanceiras
    {
        #region Propriedades

        [PersistenceProperty("TIPOMOV")]
        public long TipoMov { get; set; }

        [PersistenceProperty("NOMEMOV")]
        public string NomeMov { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("IDGRUPOCONTA")]
        public uint IdGrupoConta { get; set; }

        [PersistenceProperty("IDCATEGORIACONTA")]
        public uint IdCategoriaConta { get; set; }

        [PersistenceProperty("SALDOANTERIORDIA")]
        public decimal SaldoAnteriorDia { get; set; }

        [PersistenceProperty("ENTRADASDIA")]
        public decimal EntradasDia { get; set; }

        [PersistenceProperty("SAIDASDIA")]
        public decimal SaidasDia { get; set; }

        [PersistenceProperty("SALDOANTERIORMES")]
        public decimal SaldoAnteriorMes { get; set; }

        [PersistenceProperty("ENTRADASMES")]
        public decimal EntradasMes { get; set; }

        [PersistenceProperty("SAIDASMES")]
        public decimal SaidasMes { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("DESCRGRUPOCONTA", DirectionParameter.InputOptional)]
        public string DescrGrupoConta { get; set; }

        [PersistenceProperty("DESCRCATEGORIACONTA", DirectionParameter.InputOptional)]
        public string DescrCategoriaConta { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal MovimentoDia
        {
            get { return EntradasDia - SaidasDia; }
        }

        public decimal SaldoDia
        {
            get { return SaldoAnteriorDia + MovimentoDia; }
        }

        public decimal MovimentoMes
        {
            get { return EntradasMes - SaidasMes; }
        }

        public decimal SaldoMes
        {
            get { return SaldoAnteriorMes + MovimentoMes; }
        }

        public string DescrTipoMov
        {
            get 
            {
                switch (TipoMov)
                {
                    case 1: return "Bancos";
                    case 2: return "Carteiras";
                    default: return "";
                }
            }
        }

        private bool _descricaoCategoriaRelatorio = false;

        public bool DescricaoCategoriaRelatorio
        {
            get { return _descricaoCategoriaRelatorio; }
            set { _descricaoCategoriaRelatorio = value; }
        }

        public string DescricaoPlanoConta
        {
            get { return !_descricaoCategoriaRelatorio ? DescrGrupoConta : DescrCategoriaConta; }
        }

        private bool _saldoAnteriorDetalhado = false;

        public bool SaldoAnteriorDetalhado
        {
            get { return _saldoAnteriorDetalhado; }
            set { _saldoAnteriorDetalhado = value; }
        }

        #endregion
    }
}