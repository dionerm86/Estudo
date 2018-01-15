using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LogProdutoDAO))]
	[PersistenceClass("log_produto")]
	public class LogProduto
    {
        #region Propriedades

        [PersistenceProperty("IDLOGPRODUTO", PersistenceParameterType.IdentityKey)]
        public uint IdLogProduto { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD")]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("IDGRUPOPROD")]
        public uint? IdGrupoProd { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("TIPOPRECOBASE")]
        public int TipoPrecoBase { get; set; }

        [PersistenceProperty("AJUSTEATACADO")]
        public float AjusteAtacado { get; set; }

        [PersistenceProperty("AJUSTEBALCAO")]
        public float AjusteBalcao { get; set; }

        [PersistenceProperty("AJUSTEOBRA")]
        public float AjusteObra { get; set; }

        [PersistenceProperty("AJUSTECUSTOCOMPRA")]
        public float AjusteCustoCompra { get; set; }

        [PersistenceProperty("AJUSTECUSTOFABBASE")]
        public float AjusteCustoFabBase { get; set; }

        [PersistenceProperty("DATAAJUSTE")]
        public DateTime DataAjuste { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeFunc;

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFunc); }
            set { _nomeFunc = value; }
        }

        #endregion

        #region Propriedades de Suporte

        public string ColunaAtacado
        {
            get { return (AjusteAtacado >= 0) ? AjusteAtacado.ToString("0.##") + "%" : "N/A"; }
        }

        public string ColunaBalcao
        {
            get { return (AjusteBalcao >= 0) ? AjusteBalcao.ToString("0.##") + "%" : "N/A"; }
        }

        public string ColunaObra
        {
            get { return (AjusteObra >= 0) ? AjusteObra.ToString("0.##") + "%" : "N/A"; }
        }

        public string ColunaCustoCompra
        {
            get { return (AjusteCustoCompra >= 0) ? AjusteCustoCompra.ToString("0.##") + "%" : "N/A"; }
        }

        public string ColunaCustoFabBase
        {
            get { return (AjusteCustoFabBase >= 0) ? AjusteCustoFabBase.ToString("0.##") + "%" : "N/A"; }
        }

        public string DataAjusteString
        {
            get { return DataAjuste.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DescrTipoPrecoBase
        {
            get 
            {
                switch (TipoPrecoBase)
                {
                    case 0: return "Custo Forn.";
                    case 1: return "Custo" + (IdGrupoProd != 0 ? " Imp." : "");
                    case 2: return "Atacado";
                    case 3: return "Balcão";
                    case 4: return "Obra";
                    default: return String.Empty;
                }
            }
        }

        #endregion
    }
}