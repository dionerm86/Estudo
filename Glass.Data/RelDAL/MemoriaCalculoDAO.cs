using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Data.Helper.DescontoAcrescimo;

namespace Glass.Data.RelDAL
{
    public sealed class MemoriaCalculoDAO : Glass.Pool.PoolableObject<MemoriaCalculoDAO>
    {
        private MemoriaCalculoDAO() { }

        #region Objeto principal

        public MemoriaCalculo[] GetMemoriaCalculo(Orcamento orca)
        {
            List<MemoriaCalculo> retorno = new List<MemoriaCalculo>();
            retorno.Add(new MemoriaCalculo(orca));

            return retorno.ToArray();
        }

        public MemoriaCalculo[] GetMemoriaCalculo(Pedido ped)
        {
            List<MemoriaCalculo> retorno = new List<MemoriaCalculo>();
            retorno.Add(new MemoriaCalculo(ped));

            return retorno.ToArray();
        }

        public MemoriaCalculo[] GetMemoriaCalculo(PedidoEspelho pedEsp)
        {
            List<MemoriaCalculo> retorno = new List<MemoriaCalculo>();
            retorno.Add(new MemoriaCalculo(pedEsp));

            return retorno.ToArray();
        }

        #endregion

        #region Objetos dos produtos

        public DadosMemoriaCalculo[] GetDadosMemoriaCalculo(Orcamento orca)
        {
            List<DadosMemoriaCalculo> retorno = new List<DadosMemoriaCalculo>();
            
            decimal totalAcrescimo = 0;
            decimal totalDesconto = 0;

            var lstAluminio = new Dictionary<uint, KeyValuePair<KeyValuePair<string, string>, MaterialItemProjeto>>();
            
            var produtos = ProdutosOrcamentoDAO.Instance.GetForMemoriaCalculo(orca.IdOrcamento);
            List<MaterialItemProjeto> itensProjeto = new List<MaterialItemProjeto>();

            bool orcamentoSemTipoEntrega = false;
            if (orca.TipoEntrega == null)
            {
                orcamentoSemTipoEntrega = true;
                orca.TipoEntrega = (int)Orcamento.TipoEntregaOrcamento.Balcao;
            }

            foreach (ProdutosOrcamento po in produtos)
                if (po.IdItemProjeto == null)
                {
                    retorno.Add(new DadosMemoriaCalculo(po, (Orcamento.TipoEntregaOrcamento)orca.TipoEntrega, orca.IdCliente));
                    foreach (ProdutoOrcamentoBenef pob in po.Beneficiamentos.ToProdutosOrcamento())
                        retorno.Add(new DadosMemoriaCalculo(pob));
                }
                else
                {
                    totalAcrescimo += po.ValorAcrescimo;
                    totalDesconto += PedidoConfig.RatearDescontoProdutos ? po.ValorDesconto : 0;

                    var itens = MaterialItemProjetoDAO.Instance.GetByItemProjeto(po.IdItemProjeto.Value).ToArray();
                    Calcular.Instance.AplicaAcrescimoAmbiente(po.TipoAcrescimo, po.Acrescimo, itens, orca);
                    Calcular.Instance.AplicaDescontoAmbiente(po.TipoDesconto, po.Desconto, itens, orca);

                    foreach (MaterialItemProjeto mip in itens)
                    {
                        mip.Ambiente = po.Ambiente;
                        mip.DescrAmbiente = po.Descricao;
                        itensProjeto.Add(mip);
                    }
                }

            // Insere os itens do alumínio no projeto, se eles tiverem sido agrupados
            foreach (uint key in lstAluminio.Keys)
            {
                string ambiente = lstAluminio[key].Key.Key;
                string descrAmbiente = lstAluminio[key].Key.Value;
                MaterialItemProjeto item = lstAluminio[key].Value;

                // Atualiza a quantidade de itens e a altura
                int i = 0;
                while (item.Altura > 6 * i)
                    i++;

                item.Qtde = i;
                item.Altura = i > 0 ? 6f : 0f;
                item.AlturaCalc = item.Altura;
                item.Ambiente = ambiente;
                item.DescrAmbiente = descrAmbiente;
                item.Total = item.Valor * i;
                itensProjeto.Add(item);
            }

            var materiaisItemProjeto = itensProjeto.ToArray();
            if (PedidoConfig.Comissao.ComissaoAlteraValor)
                Calcular.Instance.AplicaComissao(orca.PercComissao, materiaisItemProjeto, orca);

            Calcular.Instance.AplicaAcrescimo(2, totalAcrescimo, materiaisItemProjeto, orca);
            Calcular.Instance.AplicaDesconto(2, totalDesconto, materiaisItemProjeto, orca);

            foreach (MaterialItemProjeto mip in materiaisItemProjeto)
            {
                retorno.Add(new DadosMemoriaCalculo(mip, (Orcamento.TipoEntregaOrcamento)orca.TipoEntrega, orca.PercComissao, mip.Ambiente, 
                    mip.DescrAmbiente, orca.IdCliente, false));

                foreach (MaterialProjetoBenef mpb in mip.Beneficiamentos.ToMateriaisProjeto())
                {
                    retorno.Add(new DadosMemoriaCalculo(mpb, (Orcamento.TipoEntregaOrcamento)orca.TipoEntrega, orca.PercComissao, mip.Ambiente,
                        mip.DescrAmbiente, orca.IdCliente));
                }
            }

            if (orcamentoSemTipoEntrega)
            {
                orca.TipoEntrega = null;
            }

            return retorno.ToArray();
        }

        public DadosMemoriaCalculo[] GetDadosMemoriaCalculo(Pedido ped)
        {
            var retorno = new List<DadosMemoriaCalculo>();
            var ambientesSomados = new List<uint>();

            var produtos = ProdutosPedidoDAO.Instance.GetByPedido(ped.IdPedido);
            foreach (ProdutosPedido pp in produtos)
            {
                float qtdeSomar = 0;
                if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra && !ambientesSomados.Contains(pp.IdAmbientePedido.Value))
                {
                    qtdeSomar = pp.QtdeAmbiente;
                    ambientesSomados.Add(pp.IdAmbientePedido.Value);
                }

                retorno.Add(new DadosMemoriaCalculo(pp, ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra, qtdeSomar, 
                    (Pedido.TipoEntregaPedido?)ped.TipoEntrega, ped.IdCli, ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição));

                foreach (ProdutoPedidoBenef ppb in pp.Beneficiamentos.ToProdutosPedido())
                    retorno.Add(new DadosMemoriaCalculo(ppb));
            }

            return retorno.ToArray();
        }

        public DadosMemoriaCalculo[] GetDadosMemoriaCalculo(PedidoEspelho pedEsp)
        {
            Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(pedEsp.IdPedido);

            var retorno = new List<DadosMemoriaCalculo>();
            var ambientesSomados = new List<uint>();

            var produtos = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(pedEsp.IdPedido, false);
            foreach (ProdutosPedidoEspelho ppe in produtos)
            {
                float qtdeSomar = 0;
                if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra && !ambientesSomados.Contains(ppe.IdAmbientePedido.Value))
                {
                    qtdeSomar = ppe.QtdeAmbiente;
                    ambientesSomados.Add(ppe.IdAmbientePedido.Value);
                }

                retorno.Add(new DadosMemoriaCalculo(ppe, ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra, qtdeSomar,
                    (Pedido.TipoEntregaPedido?)ped.TipoEntrega, ped.IdCli, ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição));

                foreach (ProdutoPedidoEspelhoBenef ppeb in ppe.Beneficiamentos.ToProdutosPedidoEspelho())
                    retorno.Add(new DadosMemoriaCalculo(ppeb));
            }

            return retorno.ToArray();
        }

        #endregion
    }
}
