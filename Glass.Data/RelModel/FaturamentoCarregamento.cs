using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.RelModel
{
    /// <summary>
    /// 'Entidade' responsavel por salvar os dados para gerar o faturamento do carregamento
    /// </summary>
    [PersistenceBaseDAO(typeof(FaturamentoCarregamentoaDAO))]
    public class FaturamentoCarregamento
    {
        #region Variaveis Locais

        private Parcelas _parcelas;
        private decimal _valorTotalPedidos;

        #endregion

        #region Propriedades

        [PersistenceProperty("IdCliente")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("IdFormaPagto")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("IdsPedidos")]
        public string IdsPedidos { get; set; }

        #endregion

        #region Propiedades de Suporte

        public List<uint> LstIdsPedidos
        {
            get
            {
                if (string.IsNullOrEmpty(IdsPedidos))
                    return new List<uint>();

                return IdsPedidos.Split(',').Select(f => f.StrParaUint()).ToList();
            }
        }

        public Parcelas Parcelas
        {
            get
            {
                if (_parcelas == null)
                    _parcelas = ParcelasDAO.Instance.GetPadraoCliente(null, IdCliente);

                return _parcelas;
            }
        }

        public decimal ValorTotalPedidos
        {
            get
            {
                if (_valorTotalPedidos == 0)
                    foreach (var idPedido in LstIdsPedidos)
                        _valorTotalPedidos += PedidoDAO.Instance.GetTotal(null, idPedido);

                return _valorTotalPedidos;
            }
        }

        #endregion

        #region Métodos Publicos

        /// <summary>
        /// Valida se o cliente pode realizar o faturamento
        /// </summary>
        public string ValidaCliente()
        {
            if (Parcelas == null)
            {
                IdsPedidos = null;
                return "Erro ao faturar. O cliente " + IdCliente + " deve possuir parcela padrão configurada.\r\n---------------\r\n";
            }

            return null;
        }

        /// <summary>
        /// Valida se os pedidos podem ser faturados
        /// </summary>
        /// <returns></returns>
        public string ValidaPedidos()
        {
            var retorno = new List<string>();
            var idsPedidosValidos = new List<uint>();

            foreach (var idPedido in LstIdsPedidos)
            {
                var aux = new List<string>();

                if (PedidoDAO.Instance.ObtemTipoVenda(null, idPedido) == (int)Pedido.TipoVendaPedido.AVista)
                    aux.Add("\t** Pedido à vista não pode ser faturado." + Environment.NewLine);

                if (NotaFiscalDAO.Instance.ExistsByPedido(null, idPedido))
                    aux.Add("\t** Pedido já tem notas fiscais cadastradas." + Environment.NewLine);

                var retornoValidacao = PedidoDAO.Instance.ValidaPedidoLiberacao(null, idPedido, null, null, false, null, string.Empty).Split('|');

                if (retornoValidacao[0] != "true")
                    aux.Add("\t** " + retornoValidacao[1]);

                if (aux.Count > 0)
                {
                    aux.Insert(0, "Erro ao faturar o pedido : " + idPedido + Environment.NewLine);
                    aux.Add("\r\n---------------\r\n");
                    retorno.AddRange(aux);
                }
                else
                {
                    idsPedidosValidos.Add(idPedido);
                }
            }

            IdsPedidos = string.Join(",", idsPedidosValidos);

            return string.Join("", retorno);
        }

        #endregion
    }
}
