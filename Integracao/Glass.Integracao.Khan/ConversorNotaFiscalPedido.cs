// <copyright file="ConversorNotaFiscalPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o conversor da nota fiscal para o pedido do serviço da khan.
    /// </summary>
    internal class ConversorNotaFiscalPedido
    {
        private readonly GDA.GDASession sessao;
        private readonly Data.Model.NotaFiscal notaFiscal;

        private IEnumerable<Data.Model.ProdutosNf> produtos;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorNotaFiscalPedido"/>.
        /// </summary>
        /// <param name="sessao">Sessão de conexão com o banco de dados.</param>
        /// <param name="notaFiscal">Nota fiscal que será convertida.</param>
        public ConversorNotaFiscalPedido(GDA.GDASession sessao, Data.Model.NotaFiscal notaFiscal)
        {
            this.sessao = sessao;
            this.notaFiscal = notaFiscal;
        }

        private KhanPedidoServiceReference.ItemPedido Converter(Data.Model.ProdutosNf produto)
        {
            return new KhanPedidoServiceReference.ItemPedido
            {
                NUMBICO = null,
                NUMTICKET_TROCA = null,
                NUM_ABASTECIMENTO = null,
                NUM_SEQ_ABASTECIMENTO = null,
                PCOFINS = produto.AliqCofins,
                PICMS = produto.AliqIcms,
                PICMSSUB = produto.AliqIcmsSt,
                PIPI = produto.AliqIpi,
                PPIS = produto.AliqPis,
                SITTRIB = null,
                SITTRIB_COFINS = null,
                SITTRIB_IPI = null,
                SITTRIB_PIS = null,
                TANQUE = null,
                codbar = null,
                codempresa = null,
                codpro = produto.CodInterno,
                idt = 0,
                lote = null,
                numped_int = 0,
                obsvint = produto.Obs,
                prunit = (float)produto.ValorUnitario,
                qtdpro = produto.Qtde,
                seq = 0,
                seqped_int = 0,
                vdesc = 0,
                vfrete = (float)produto.ValorFrete,
            };
        }

        private void CarregarProdutos()
        {
            this.produtos = Data.DAL.ProdutosNfDAO.Instance.GetByNf(this.notaFiscal.IdNf);
        }

        /// <summary>
        /// Faz a conversão dos dados da nota para o pedido.
        /// </summary>
        /// <returns>Pedido convertido.</returns>
        public KhanPedidoServiceReference.Pedido Converter()
        {
            this.CarregarProdutos();

            var itens = new List<KhanPedidoServiceReference.ItemPedido>();
            var sequencia = 1;

            foreach (var produto in this.produtos)
            {
                var item = this.Converter(produto);
                item.seq = sequencia++;

                itens.Add(item);
            }

            throw new NotImplementedException();
        }
    }
}
