using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovReservaLiberacaoDAO))]
    [PersistenceClass("mov_reserva_liberacao")]
    public class MovReservaLiberacao
    {
        #region Propriedades

        [PersistenceProperty("IDMOVRESERVALIBERACAO", PersistenceParameterType.IdentityKey)]
        public uint IdMovReservaLiberacao { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("TIPOMOV")]
        public int TipoMov { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATAMOV")]
        public DateTime DataMov { get; set; }

        [Log("Qtde. Reserva")]
        [PersistenceProperty("QTDERESERVA")]
        public decimal? QtdeReserva { get; set; }

        [Log("Saldo Reserva")]
        [PersistenceProperty("SALDORESERVA")]
        public decimal? SaldoReserva { get; set; }

        [Log("Qtde. Liberacao")]
        [PersistenceProperty("QTDELIBERACAO")]
        public decimal? QtdeLiberacao { get; set; }

        [Log("Saldo Liberação")]
        [PersistenceProperty("SALDOLIBERACAO")]
        public decimal? SaldoLiberacao { get; set; }

        [Log("ID Saída Estoque")]
        [PersistenceProperty("IDSAIDAESTOQUE")]
        public int? IdSaidaEstoque { get; set; }

        [Log("ID Liberação")]
        [PersistenceProperty("IDLIBERARPEDIDO")]
        public int? IdLiberarPedido { get; set; }

        [Log("ID Pedido Espelho")]
        [PersistenceProperty("IDPEDIDOESPELHO")]
        public int? IdPedidoEspelho { get; set; }

        [Log("ID Produto Produção")]
        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public int? IdProdPedProducao { get; set; }

        [Log("ID Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public int? IdPedido { get; set; }

        [Log("IDs Pedido")]
        [PersistenceProperty("IDSPEDIDO")]
        public string IdsPedido { get; set; }

        [Log("ID Produto Pedido")]
        [PersistenceProperty("IDPRODPED")]
        public int? IdProdPed { get; set; }

        [Log("Classe e Método")]
        [PersistenceProperty("CLASSEMETODO")]
        public string ClasseMetodo { get; set; }

        #endregion 
    }
}
