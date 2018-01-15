using System;
using GDA;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(EstoqueProdutosDAO))]
    [PersistenceClass("estoque_produtos")]
    public class EstoqueProdutos
    {
        #region Propriedades

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IdCliente")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("IdProjeto")]
        public uint? IdProjeto { get; set; }

        [PersistenceProperty("IdProd")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IdGrupoProd")]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd")]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("QtdeReserva")]
        public double QtdeReserva { get; set; }

        [PersistenceProperty("QtdeLiberacao")]
        public double? QtdeLiberacao { get; set; }

        [PersistenceProperty("QtdeEntradaEstoque")]
        public double QtdeEntradaEstoque { get; set; }

        [PersistenceProperty("NomeCliente")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DataPedido")]
        public DateTime DataPedido { get; set; }

        [PersistenceProperty("DataEntrega")]
        public DateTime? DataEntrega { get; set; }

        [PersistenceProperty("DataConf")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("DataLiberacao")]
        public DateTime? DataLiberacao { get; set; }

        [PersistenceProperty("CodInterno")]
        public string CodInterno { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        [PersistenceProperty("NomeGrupo")]
        public string NomeGrupo { get; set; }

        [PersistenceProperty("NomeSubgrupo")]
        public string NomeSubgrupo { get; set; }

        [PersistenceProperty("QtdeEstoque")]
        public double QtdeEstoque { get; set; }

        [PersistenceProperty("M2Estoque")]
        public double M2Estoque { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string DescrProduto
        {
            get { return CodInterno + " - " + Descricao; }
        }

        public string NomeGrupoSubgrupo
        {
            get { return NomeGrupo + (!String.IsNullOrEmpty(NomeSubgrupo) ? " - " + NomeSubgrupo : ""); }
        }

        public string DescrTipoCalculo
        {
            get 
            {
                return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? "m²" : ""; 
            }
        }

        public string DescrEstoque
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd);
                string estoque = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? 
                    Math.Round(M2Estoque, 2).ToString() : QtdeEstoque.ToString();
                return estoque + DescrTipoCalculo;
            }
        }

        public string EstoqueDisponivel
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd);
                string disponivel = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? 
                    Math.Round(M2Estoque - QtdeReserva - (PedidoConfig.LiberarPedido ? QtdeLiberacao.GetValueOrDefault() : 0), 2).ToString() :
                    (QtdeEstoque - QtdeReserva - (PedidoConfig.LiberarPedido ? QtdeLiberacao : 0)).ToString();
                return disponivel + DescrTipoCalculo;
            }
        }

        #endregion
    }
}