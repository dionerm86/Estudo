using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovFuncDAO))]
    [PersistenceClass("mov_func")]
    public class MovFunc
    {
        #region Enums

        public enum TipoMovEnum
        {
            Entrada = 1,
            Saida = 2
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDMOVFUNC", PersistenceParameterType.IdentityKey)]
        public uint IdMovFunc { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        [PersistenceProperty("VALORMOV")]
        public decimal ValorMov { get; set; }

        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipoMov
        {
            get { return TipoMov == 1 ? "Entrada" : TipoMov == 2 ? "Saída" : String.Empty; }
        }

        private bool _exibirColunas = true;

        public bool ExibirColunas
        {
            get { return _exibirColunas; }
            set { _exibirColunas = value; }
        }

        public bool ExcluirVisible
        {
            get
            {
                return ((IdLiberarPedido == null || IdLiberarPedido == 0) && (IdPedido == null || IdPedido == 0)) && IdFunc > 0;
            }
        }


        #endregion
    }
}