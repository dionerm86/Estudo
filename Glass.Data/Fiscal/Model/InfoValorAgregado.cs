using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(InfoValorAgregadoDAO))]
    [PersistenceClass("informacao_valor_agregado")]
    public class InfoValorAgregado : Sync.Fiscal.EFD.Entidade.IInfoValorAgregado
    {
        #region Propriedades

        [PersistenceProperty("IDINFOVALORAGREGADO", PersistenceParameterType.IdentityKey)]
        public uint IdInfoValorAgregado { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDCIDADE")]
        public uint IdCidade { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodIbgeUf { get; set; }

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string NomeCidade { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CodIbgeCompleto
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        #endregion

        #region IInfoValorAgregado Members

        int Sync.Fiscal.EFD.Entidade.IInfoValorAgregado.CodigoCidade
        {
            get { return (int)IdCidade; }
        }

        int Sync.Fiscal.EFD.Entidade.IInfoValorAgregado.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        #endregion
    }
}