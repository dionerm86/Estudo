using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CfopDAO))]
    [PersistenceClass("cfop")]
    public class Cfop : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.ICfop
    {
        #region Enumeradores

        public enum TipoClienteCalc
        {
            Nenhum = 0,
            ConsumidorFinal,
            Revenda
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCFOP", PersistenceParameterType.IdentityKey)]
        public int IdCfop { get; set; }

        [Log("Tipo", "Descricao", typeof(TipoCfopDAO))]
        [PersistenceProperty("IDTIPOCFOP")]
        public int? IdTipoCfop { get; set; }

        [Log("Tipo Mercadoria")]
        [PersistenceProperty("TIPOMERCADORIA")]
        public Data.Model.TipoMercadoria? TipoMercadoria { get; set; }

        [Log("Código")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Alterar Estoque Terceiros")]
        [PersistenceProperty("ALTERARESTOQUETERCEIROS")]
        public bool AlterarEstoqueTerceiros { get; set; }

        [Log("Alterar Estoque de Vidros de Cliente")]
        [PersistenceProperty("ALTERARESTOQUECLIENTE")]
        public bool AlterarEstoqueCliente { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRTIPO", DirectionParameter.InputOptional)]
        public string DescrTipo { get; set; }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Campo utilizado para saber se será alterado configurações de Cfop por loja
        /// </summary>
        public uint IdLoja { get; set; }

        public string CodInternoDescricao
        {
            get { return CodInterno + " - " + Descricao; }
        }

        public string DescrTipoMercadoria
        {
            get { return Colosoft.Translator.Translate(TipoMercadoria).Format(); }
        }

        #endregion

        #region ICfop Members

        int Sync.Fiscal.EFD.Entidade.ICfop.CodigoCfop
        {
            get { return IdCfop; }
        }

        string Sync.Fiscal.EFD.Entidade.ICfop.CodigoInterno
        {
            get { return CodInterno; }
        }

        private bool? _devolucao;

        bool Sync.Fiscal.EFD.Entidade.ICfop.Devolucao
        {
            get
            {
                if (_devolucao == null)
                    _devolucao = CfopDAO.Instance.IsCfopDevolucao(CodInterno);

                return _devolucao ?? false;
            }
        }

        bool Sync.Fiscal.EFD.Entidade.ICfop.CalculaCofins(int codigoLoja)
        {
            return false;
        }

        bool Sync.Fiscal.EFD.Entidade.ICfop.CalculaPis(int codigoLoja)
        {
            return false;
        }

        #endregion
    }
}