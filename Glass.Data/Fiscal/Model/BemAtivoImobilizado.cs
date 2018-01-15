using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.EFD;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(BemAtivoImobilizadoDAO))]
    [PersistenceClass("bem_ativo_imobilizado")]
    public class BemAtivoImobilizado : Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado
    {
        #region Enumeradores

        public enum TipoEnum
        {
            Bem = 1,
            Componente = 2
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDBEMATIVOIMOBILIZADO", PersistenceParameterType.IdentityKey)]
        public uint IdBemAtivoImobilizado { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [Log("Bem Principal")]
        [PersistenceProperty("IDBEMATIVOIMOBILIZADOPRINC")]
        public uint? IdBemAtivoImobilizadoPrinc { get; set; }

        [Log("Plano de Conta Contábil", "Descricao", typeof(PlanoContaContabilDAO))]
        [PersistenceProperty("IDCONTACONTABIL")]
        public uint IdContaContabil { get; set; }

        [Log("Núm. Parcelas")]
        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [Log("Centro de Custos", "Descricao", typeof(CentroCustoDAO), "IdCentroCusto", "GetByLoja", false, (uint)0)]
        [PersistenceProperty("IDCENTROCUSTO")]
        public uint? IdCentroCusto { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Vida Útil")]
        [PersistenceProperty("VIDAUTIL")]
        public int? VidaUtil { get; set; }

        [PersistenceProperty("GRUPO")]
        public int Grupo { get; set; }

        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("USUCAD", DirectionParameter.OutputOnlyInsert)]
        public uint UsuCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CodInternoProd", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("CodInternoContaContabil", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        [PersistenceProperty("DescrProd", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        [PersistenceProperty("DescrPlanoContaContabil", DirectionParameter.InputOptional)]
        public string DescrPlanoContaContabil { get; set; }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        private string _descrCentroCusto;

        [PersistenceProperty("DescrCentroCusto", DirectionParameter.InputOptional)]
        public string DescrCentroCusto
        {
            get 
            {
                if (!String.IsNullOrEmpty(_descrCentroCusto))
                    return _descrCentroCusto;
                else if (IdCentroCusto > 0)
                {
                    CentroCusto temp = new CentroCusto();
                    temp.IdCentroCusto = (int)IdCentroCusto.Value;
                    return temp.Descricao;
                }
                else
                    return null;
            }
            set { _descrCentroCusto = value; }
        }

        #endregion

        #region Propriedades de Suporte

        [Log("Tipo")]
        public string DescrTipo
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoBemAtivoImobilizado(Tipo); }
        }

        [Log("Grupo")]
        public string DescrGrupo
        {
            get { return DataSourcesEFD.Instance.GetDescrGrupoBemAtivoImobilizado(Grupo); }
        }

        #endregion

        #region IBemAtivoImobilizado Members

        int Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoBemAtivoImobilizado
        {
            get { return (int)IdBemAtivoImobilizado; }
        }

        int Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoLoja
        {
            get { return (int)IdLoja; }
        }

        int Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        int? Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoBemAtivoImobilizadoPrincipal
        {
            get { return (int?)IdBemAtivoImobilizadoPrinc; }
        }

        int? Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoContaContabil
        {
            get { return (int?)IdContaContabil; }
        }

        int? Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.CodigoCentroCusto
        {
            get { return (int?)IdCentroCusto; }
        }

        Sync.Fiscal.Enumeracao.BemAtivoImobilizado.Tipo Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.Tipo
        {
            get { return (Sync.Fiscal.Enumeracao.BemAtivoImobilizado.Tipo)Tipo; }
        }

        int Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.NumeroParcelas
        {
            get { return NumParc; }
        }

        Sync.Fiscal.Enumeracao.BemAtivoImobilizado.GrupoBemAtivoImobilizado Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado.Grupo
        {
            get { return (Sync.Fiscal.Enumeracao.BemAtivoImobilizado.GrupoBemAtivoImobilizado)Grupo; }
        }

        #endregion
    }
}