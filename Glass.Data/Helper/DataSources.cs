using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.EFD;
using System.Collections;
using Sync.Utils.Boleto;
using Sync.Utils.Boleto.Instrucoes;
using System.Linq;
using Glass.Configuracoes;
using Colosoft;

namespace Glass.Data.Helper
{
    public sealed class DataSources : Glass.Pool.PoolableObject<DataSources>
    {
        private DataSources() { }

        public GenericModel[] GetSituacaoNotaFiscal()
        {
            Array dados = Enum.GetValues(typeof(NotaFiscal.SituacaoEnum));

            List<GenericModel> lstRetorno = new List<GenericModel>();

            int index = 0;

            foreach (var d in dados)
            {
                lstRetorno.Add(new GenericModel((uint)index + 1, ((NotaFiscal.SituacaoEnum)d).ToString()));
                index++;
            }

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetSituacaoCliente()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)SituacaoCliente.Ativo, "Ativo"));
            lstRetorno.Add(new GenericModel((uint)SituacaoCliente.Inativo, "Inativo"));
            lstRetorno.Add(new GenericModel((uint)SituacaoCliente.Cancelado, "Cancelado"));
            lstRetorno.Add(new GenericModel((uint)SituacaoCliente.Bloqueado, "Bloqueado"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetSituacaoFornecedor()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)SituacaoFornecedor.Ativo, "Ativo"));
            lstRetorno.Add(new GenericModel((uint)SituacaoFornecedor.Inativo, "Inativo"));
            lstRetorno.Add(new GenericModel((uint)SituacaoFornecedor.Cancelado, "Cancelado"));
            lstRetorno.Add(new GenericModel((uint)SituacaoFornecedor.Bloqueado, "Bloqueado"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetSituacaoVolume()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)Volume.SituacaoVolume.Aberto, "Aberto"));
            lstRetorno.Add(new GenericModel((uint)Volume.SituacaoVolume.Fechado, "Fechado"));

            return lstRetorno.ToArray();
        }

        /// <summary>
        /// Retorna uma lista com os tipos de cálculo.
        /// </summary>
        /// <param name="exibirDecimal"></param>
        /// <param name="unidade"></param>
        /// <returns></returns>
        public GenericModel[] GetTipoCalculo(bool exibirDecimal, bool unidade)
        {
            return GetTipoCalculo(exibirDecimal, unidade, false);
        }

        /// <summary>
        /// Retorna uma lista com os tipos de cálculo.
        /// </summary>
        /// <param name="exibirDecimal"></param>
        /// <param name="unidade"></param>
        /// <returns></returns>
        public GenericModel[] GetTipoCalculo(bool exibirDecimal, bool unidade, bool nf)
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.Qtd, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.Qtd, unidade)));

            if (nf)
                lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.QtdM2, unidade)));

            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.M2, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.M2, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.M2Direto, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.Perimetro, unidade)));
            
            if (exibirDecimal)
                lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal, unidade)));

            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.MLAL0, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.MLAL05, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.MLAL1, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.MLAL6, unidade)));
            lstRetorno.Add(new GenericModel((uint)Glass.Data.Model.TipoCalculoGrupoProd.ML, Glass.Global.CalculosFluxo.GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd.ML, unidade)));

            return lstRetorno.ToArray();
        }

        /// <summary>
        /// Retorna os tipos de perda possíveis.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoPerda()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            foreach (TipoPerda tp in TipoPerdaDAO.Instance.GetOrderedList())
            {
                lstRetorno.Add(new GenericModel(tp.IdTipoPerda, tp.Descricao));
            }

            return lstRetorno.ToArray();
        }

        /// <summary>
        /// Retorna as formas de pagamento para a nota fiscal.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetFormasPagtoNf()
        {
            List<GenericModel> lstFormasPagto = new List<GenericModel>();
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Boleto, "Boleto"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Cartao, "Cartão"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, "Cheque Próprio"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro, "Cheque Terceiro"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Construcard, "Construcard"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Deposito, "Depósito"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro, "Dinheiro"));
            lstFormasPagto.Add(new GenericModel((uint)Glass.Data.Model.Pagto.FormaPagto.Prazo, FormaPagtoDAO.Instance.GetDescricao(Glass.Data.Model.Pagto.FormaPagto.Prazo)));

            return lstFormasPagto.ToArray();
        }

        /// <summary>
        /// Retorna as situações possíveis para um orçamento.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoOrcamento()
        {
            List<GenericModel> lst = new List<GenericModel>();
            foreach (int i in Enum.GetValues(typeof(Orcamento.SituacaoOrcamento)))
                lst.Add(new GenericModel((uint)i, Orcamento.GetDescrSituacao(i)));

            if (!OrcamentoConfig.NegociarParcialmente)
                lst.RemoveAt((int)Orcamento.SituacaoOrcamento.NegociadoParcialmente - 1);

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna as situações que o pedido pode ter
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoPedido()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Ativo, "Ativo"));
            lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.AtivoConferencia, "Ativo/Em Conferência"));

            if (Geral.ControleConferencia)
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.EmConferencia, "Em Conferência"));

            lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Conferido, !PedidoConfig.LiberarPedido ? "Conferido" : "Conferido COM"));

            if (!PedidoConfig.LiberarPedido)
            {
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Confirmado"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Cancelado, "Cancelado"));
            }
            else
            {
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Cancelado, "Cancelado"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.ConfirmadoLiberacao, "Confirmado PCP"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.LiberadoParcialmente, "Liberado Parcialmente"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Liberado"));
            }

            if (FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro)
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro, "Aguardando Finalização Financeiro"));

            if (FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro)
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro, "Aguardando Confirmação Financeiro"));

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna as situações que o pedido original pode ter na conferência
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoPedidoPCP()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();

            if (!PedidoConfig.LiberarPedido)
            {
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Confirmado"));
            }
            else
            {
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.ConfirmadoLiberacao, "Confirmado PCP"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.LiberadoParcialmente, "Liberado Parcialmente"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Liberado"));
            }

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna todas as situações de cheques
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoCheque()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.EmAberto, "Em Aberto"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Compensado, "Compensado"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Devolvido, "Devolvido"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Quitado, "Quitado"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Cancelado, "Cancelado"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Trocado, "Trocado"));
            lstTipo.Add(new GenericModel((int)Cheques.SituacaoCheque.Protestado, "Protestado"));

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna situações  de pedidos a serem utilizadas em telas de filtro
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoPedidoFiltro()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();

            if (!PedidoConfig.LiberarPedido)
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Confirmado"));
            else
            {
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.ConfirmadoLiberacao, "Confirmado PCP"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.LiberadoParcialmente, "Liberado Parcialmente"));
                lstTipo.Add(new GenericModel((int)Pedido.SituacaoPedido.Confirmado, "Liberado"));
            }

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna as situações que a instalação pode ter
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetSituacaoInstalacao()
        {
            List<GenericModel> lstSituacao = new List<GenericModel>();

            lstSituacao.Add(new GenericModel((int)Instalacao.SituacaoInst.Aberta, "Aberta"));
            lstSituacao.Add(new GenericModel((int)Instalacao.SituacaoInst.EmAndamento, "Em Andamento"));
            lstSituacao.Add(new GenericModel((int)Instalacao.SituacaoInst.Finalizada, "Finalizada"));
            lstSituacao.Add(new GenericModel((int)Instalacao.SituacaoInst.Continuada, "Continuada"));
            lstSituacao.Add(new GenericModel((int)Instalacao.SituacaoInst.Cancelada, "Cancelada"));

            return lstSituacao.ToArray();
        }

        /// <summary>
        /// Armazena tipos de entrega padrões do sistema em memória para não precisar ir no banco o tempo todo
        /// </summary>
        static List<GenericModel> listaTipoEntrega = null;

        /// <summary>
        /// Retorna os tipo de entrega aplicáveis à empresa
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoEntrega()
        {
            // Se a lista não estiver preenchida, carrega do banco
            if (listaTipoEntrega == null || listaTipoEntrega.Count() == 0)
            {
                listaTipoEntrega = new List<GenericModel>();

                foreach (var tipo in TipoEntregaDAO.Instance.GetAll())
                    listaTipoEntrega.Add(new GenericModel(tipo.IdTipoEntrega, tipo.Descricao));
            }

            return listaTipoEntrega.ToArray();
        }

        /// <summary>
        /// Retorna o código para o tipo de entrega "Entrega" usado pela empresa.
        /// </summary>
        /// <returns></returns>
        public uint? GetTipoEntregaEntrega()
        {
            foreach (GenericModel m in GetTipoEntrega())
                if (m.Descr == "Entrega")
                    return m.Id;

            return null;
        }

        /// <summary>
        /// Retorna o código para o tipo de entrega "Balcão" usado pela empresa.
        /// </summary>
        /// <returns></returns>
        public uint? GetTipoEntregaBalcao()
        {
            foreach (GenericModel m in GetTipoEntrega())
                if (m.Descr == "Balcão")
                    return m.Id;

            return null;
        }

        /// <summary>
        /// Retorna os tipo de entrega aplicáveis ao projeto
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoEntregaForPojeto()
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel(1, "Balcão"));
            lst.Add(new GenericModel(3, "Colocação Temperado"));

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna os turnos utilizados na medição
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTurnoMedicao()
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel(1, "Manhã"));
            lst.Add(new GenericModel(2, "Tarde"));
            lst.Add(new GenericModel(4, "Horário Comercial"));

            return lst.ToArray();
        }

        public GenericModel[] GetTipoPedidoFilter()
        {
            return GetTipoPedidoFilter(true);
        }

        public GenericModel[] GetTipoPedidoFilter(bool incluirProducao)
        {
            List<GenericModel> lst = new List<GenericModel>();
            lst.Add(new GenericModel(1, "Venda"));
            lst.Add(new GenericModel(2, "Revenda"));
            lst.Add(new GenericModel(3, "Mão de Obra"));

            //Chamado 50458
            if (EstoqueConfig.ControlarEstoqueVidrosClientes)
                lst.Add(new GenericModel(5, "Mão de Obra Especial"));

            if (incluirProducao)
                lst.Add(new GenericModel(4, "Produção"));

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna os tipos de venda de pedido.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoVenda()
        {
            return GetTipoVenda(true);
        }

        public GenericModel[] GetTipoVenda(bool incluirVazio)
        {
            return GetTipoVenda(incluirVazio, false);
        }

        public GenericModel[] GetTipoVenda(bool incluirVazio, bool paraFiltro)
        {
            var login = UserInfo.GetUserInfo;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            return GetTipoVenda(incluirVazio, paraFiltro, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario);
        }

        /// <summary>
        /// Retorna os tipos de venda de pedido.
        /// </summary>
        /// <param name="incluirVazio">Inclui uma opção vazia</param>
        /// <param name="paraFiltro">Se for para filtro, sempre busca os tipos reposição e garantia</param>
        /// <returns></returns>
        internal GenericModel[] GetTipoVenda(bool incluirVazio, bool paraFiltro, bool administrador, bool emitirGarantiaReposicao,
            bool emitirPedidoFuncionario)
        {
            List<GenericModel> lst = new List<GenericModel>();

            if (incluirVazio)
                lst.Add(new GenericModel(null, null));

            lst.Add(new GenericModel(1, "À Vista"));
            lst.Add(new GenericModel(2, "À Prazo"));

            if (paraFiltro || administrador || emitirGarantiaReposicao)
            {
                lst.Add(new GenericModel(3, "Reposição"));
                lst.Add(new GenericModel(4, "Garantia"));
            }

            lst.Add(new GenericModel(5, "Obra"));

            if (emitirPedidoFuncionario)
                lst.Add(new GenericModel(6, "Funcionário"));

            return lst.ToArray();
        }

        //Retorna os tipos de venda permitidos em orçamento
        public GenericModel[] GetTipoVendaOrcamento()
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel(null, null));
            lst.Add(new GenericModel(1, "À Vista"));
            lst.Add(new GenericModel(2, "À Prazo"));

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna a quantidade de parcelas que pode ser pago um pedido
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetNumParc()
        {
            return GetNumParc(0);
        }

        /// <summary>
        /// Retorna a quantidade de parcelas que pode ser pago um pedido
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetNumParc(uint idCliente)
        {
            List<GenericModel> lst = new List<GenericModel>();

            if (PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido)
            {
                foreach (Parcelas p in ParcelasDAO.Instance.GetByCliente(idCliente, ParcelasDAO.TipoConsulta.Prazo))
                    lst.Add(new GenericModel((uint)p.NumParcelas, p.Descricao));
            }
            else
            {
                for (int i = 1; i <= PedidoConfig.FormaPagamento.NumParcelasPedido; i++)
                    lst.Add(new GenericModel((uint)i, i.ToString()));
            }

            return lst.ToArray();
        }

        /// <summary>
        /// Armazena tipos de entrega padrões do sistema em memória para não precisar ir no banco o tempo todo
        /// </summary>
        static List<GenericModel> listaTipoInstalacao = null;

        /// <summary>
        /// Retorna os tipo de instalação aplicáveis à empresa
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoInstalacao()
        {
            // Se a lista não estiver preenchida, carrega do banco
            if (listaTipoInstalacao == null || listaTipoInstalacao.Count() == 0)
            {
                listaTipoInstalacao = new List<GenericModel>();

                foreach (var tipo in TipoInstalacaoDAO.Instance.GetAll())
                    listaTipoInstalacao.Add(new GenericModel(tipo.IdTipoInstalacao, tipo.Descricao));
            }

            return listaTipoInstalacao.ToArray();
        }

        #region Beneficiamento

        /// <summary>
        /// Retorna os tipos de jato que podem ser utilizados no beneficiamento
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoJato()
        {
            List<GenericModel> lst = new List<GenericModel>();
            lst.Add(new GenericModel(0, "Nenhum"));
            lst.Add(new GenericModel(1, "Total"));
            lst.Add(new GenericModel(2, "Friso"));
            lst.Add(new GenericModel(3, "Friso Detalhado"));
            lst.Add(new GenericModel(4, "Desenho"));
            lst.Add(new GenericModel(5, "Impermeabilizado"));
            lst.Add(new GenericModel(6, "Painel"));
            lst.Add(new GenericModel(7, "Tonalização"));

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna os tipos de canto que podem ser utilizados no beneficiamento
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoCanto()
        {
            List<GenericModel> lst = new List<GenericModel>();
            lst.Add(new GenericModel(1, "Moeda"));
            lst.Add(new GenericModel(2, "Copo"));
            lst.Add(new GenericModel(3, "Eme"));
            lst.Add(new GenericModel(4, "Mais"));
            lst.Add(new GenericModel(5, "Luiz XV"));
            lst.Add(new GenericModel(6, "Hélice"));
            lst.Add(new GenericModel(7, "Chanfro"));
            lst.Add(new GenericModel(8, "Lápis"));
            lst.Add(new GenericModel(9, "Bisotado"));
            lst.Add(new GenericModel(10, "Tartaruga"));
            lst.Add(new GenericModel(11, "Arabesco"));

            return lst.ToArray();
        }

        #endregion

        /// <summary>
        /// Retorna lista com os códigos da situação da operação do Simples Nacional
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetCSOSN()
        {
            List<GenericModel> lst = new List<GenericModel>();
            lst.Add(new GenericModel(101, "101"));
            lst.Add(new GenericModel(102, "102"));
            lst.Add(new GenericModel(103, "103"));
            lst.Add(new GenericModel(300, "300"));
            lst.Add(new GenericModel(400, "400"));
            lst.Add(new GenericModel(201, "201"));
            lst.Add(new GenericModel(202, "202"));
            lst.Add(new GenericModel(203, "203"));
            lst.Add(new GenericModel(500, "500"));
            lst.Add(new GenericModel(900, "900"));
            return lst.ToArray();
        }

        /// <summary>
        /// Retorna as possívei origens de CST de um produto
        /// 0 - Nacional, exceto as indicadas nos códigos 3 a 5;
        /// 1 - Estrangeira - Importação direta, exceto a indicada no código 6;
        /// 2 - Estrangeira - Adquirida no mercado interno, exceto a indicada no código 7;
        /// 3 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 40%;
        /// 4 - Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos 
        /// de que tratam as legislações citadas nos Ajustes;
        /// 5 - Nacional, mercadoria ou bem com Conteúdo de Importação inferior ou igual a 40%;
        /// 6 - Estrangeira - Importação direta, sem similar nacional, constante em lista da CAMEX;
        /// 7 - Estrangeira - Adquirida no mercado interno, sem similar nacional, constante em lista da CAMEX.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetOrigCST()
        {
            List<GenericModel> lst = new List<GenericModel>();
            lst.Add(new GenericModel(0, "0"));
            lst.Add(new GenericModel(1, "1"));
            lst.Add(new GenericModel(2, "2"));
            lst.Add(new GenericModel(3, "3"));
            lst.Add(new GenericModel(4, "4"));
            lst.Add(new GenericModel(5, "5"));
            lst.Add(new GenericModel(6, "6"));
            lst.Add(new GenericModel(7, "7"));
            lst.Add(new GenericModel(8, "8"));
            return lst.ToArray();
        }

        /// <summary>
        /// Retorna uma lista com as configurações.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetConfiguracoes()
        {
            List<GenericModel> lst = new List<GenericModel>();
            
            foreach (Configuracao item in ConfiguracaoDAO.Instance.GetAll())
                lst.Add(new GenericModel((uint?)item.IdConfig, item.Descricao));

            return lst.ToArray();
        }

        /// <summary>
        /// Retorna uma lista com as situações de produção.
        /// </summary>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public GenericModel[] GetSituacaoProducao()
        {
            return GetSituacaoProducao(UserInfo.GetUserInfo);
        }

        /// <summary>
        /// Retorna uma lista com as situações de produção.
        /// </summary>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public GenericModel[] GetSituacaoProducao(LoginUsuario login)
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.NaoEntregue, 
                Pedido.GetDescrSituacaoProducao(0, (int)Pedido.SituacaoProducaoEnum.NaoEntregue, 0, login)));

            if (PCPConfig.ControlarProducao)
            {
                lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.Pendente, "Pendente"));
                lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.Pronto, "Pronto"));
                lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.Entregue, "Entregue"));
            }

            if (Geral.ControleInstalacao)
            {
                lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.Instalado, "Instalado"));

                if (!PCPConfig.ControlarProducao)
                    lst.Add(new GenericModel((uint)Pedido.SituacaoProducaoEnum.Entregue, "Entregue"));
            }

            return lst.ToArray();
        }

        public GenericModel[] GetTipoFaturaNF()
        {
            Converter<int?, string> d = new Converter<int?, string>(GetDescrTipoFaturaNF);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(NotaFiscal.TipoFaturaEnum), d, false).ToArray();
        }

        public string GetDescrTipoFaturaNF(int? tipoFatura)
        {
            switch (tipoFatura)
            {
                case (int)NotaFiscal.TipoFaturaEnum.Cheque: return "Cheque";
                case (int)NotaFiscal.TipoFaturaEnum.Duplicata: return "Duplicata";
                case (int)NotaFiscal.TipoFaturaEnum.Promissoria: return "Nota promissória";
                case (int)NotaFiscal.TipoFaturaEnum.Recibo: return "Recibo";
                case (int)NotaFiscal.TipoFaturaEnum.Outros: return "Outros";
                default: return "";
            }
        }

        public GenericModel[] GetPeriodoApuracaoIpiNF()
        {
            return DataSourcesEFD.Instance.GetFromEnum(typeof(NotaFiscal.PeriodoIpiEnum), null, false).ToArray();
        }

        public GenericModel[] GetTipoPedido()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoPedido);
            var dados = DataSourcesEFD.Instance.GetFromEnum(typeof(Pedido.TipoPedidoEnum), d, false).ToArray();

            //Chamado 50458
            if (!EstoqueConfig.ControlarEstoqueVidrosClientes)
                dados = dados.Where(f => f.Id != (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial).ToArray();

                return dados;
        }

        public GenericModel[] GetTipoPedidoTrocaDev()
        {
            var dados = new List<GenericModel>();

            dados.Add(new GenericModel((int)Pedido.TipoPedidoEnum.Venda, Pedido.TipoPedidoEnum.Venda.Translate().Format()));
            dados.Add(new GenericModel((int)Pedido.TipoPedidoEnum.Revenda, Pedido.TipoPedidoEnum.Revenda.Translate().Format()));
            dados.Add(new GenericModel((int)Pedido.TipoPedidoEnum.MaoDeObra, Pedido.TipoPedidoEnum.MaoDeObra.Translate().Format()));

            return dados.ToArray();
        }

        internal string GetDescrTipoPedido(string tiposPedidos)
        {
            var tipos = tiposPedidos.Split(',').Select(x => Glass.Conversoes.StrParaInt(x));

            var lista = new List<string>();
            foreach (var t in tipos)
                lista.Add(GetDescrTipoPedido(t));

            return String.Join(", ", lista.Where(x => !String.IsNullOrEmpty(x)).ToArray());
        }

        public string GetDescrTipoPedido(int tipoPedido)
        {
            switch (tipoPedido)
            {
                case (int)Pedido.TipoPedidoEnum.Venda: return "Venda";
                case (int)Pedido.TipoPedidoEnum.Revenda: return "Revenda";
                case (int)Pedido.TipoPedidoEnum.MaoDeObra: return "Mão-de-obra";
                case (int)Pedido.TipoPedidoEnum.Producao: return "Produção";
                case (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial: return "Mão-de-obra Especial";
                default: return "";
            }
        }

        public GenericModel[] GetSituacaoExportacao()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado, "Cancelado"));
            lstTipo.Add(new GenericModel((int)PedidoExportacao.SituacaoExportacaoEnum.Chegou, "Chegou"));
            lstTipo.Add(new GenericModel((int)PedidoExportacao.SituacaoExportacaoEnum.Exportado, "Exportado"));
            lstTipo.Add(new GenericModel((int)PedidoExportacao.SituacaoExportacaoEnum.Pronto, "Pronto"));

            return lstTipo.ToArray();
        }

        public GenericModel[] GetCodValorFiscal(int tipoDocumento)
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel(1, tipoDocumento != 2 ? "Oper. com crédito do Imposto" : "Imposto Debitado"));
            lstRetorno.Add(new GenericModel(2, tipoDocumento != 2 ? "Oper. sem crédito do Imposto - Isentas ou não Tributadas" : "Isentas ou não Tributadas"));
            lstRetorno.Add(new GenericModel(3, tipoDocumento != 2 ? "Oper. sem crédito do Imposto - Outras" : "Outras"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] GetTipoImpressaoEtiqueta()
        {
            ImpressaoEtiqueta temp = new ImpressaoEtiqueta();

            List<GenericModel> lstTipoImp = new List<GenericModel>();

            if (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) || Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas))
            {
                temp.TipoImpressao = ProdutoImpressaoDAO.TipoEtiqueta.Pedido;
                lstTipoImp.Add(new GenericModel((uint)temp.TipoImpressao, temp.DescrTipoImpressao));
            }

            if (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasNFe))
            {
                temp.TipoImpressao = ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal;
                lstTipoImp.Add(new GenericModel((uint)temp.TipoImpressao, temp.DescrTipoImpressao));
            }

            return lstTipoImp.ToArray();
        }

        public enum TipoUsoAntecipacaoFornecedor
        {
            CompraOuNotaFiscal,
            ContasPagar
        }

        public GenericModel[] GetTipoUsoAntecipacaoFornecedor()
        {
            List<GenericModel> itens = new List<GenericModel>();

            itens.Add(new GenericModel((int)TipoUsoAntecipacaoFornecedor.CompraOuNotaFiscal, "Compra/NF-e"));
            itens.Add(new GenericModel((int)TipoUsoAntecipacaoFornecedor.ContasPagar, "Contas a pagar"));

            return itens.ToArray();
        }

        /// <summary>
        /// Dias da semana
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetDiasSemana()
        {
            List<GenericModel> lstDiasSemana = new List<GenericModel>();

            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Domingo, "Domingo"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Segunda, "Segunda-feira"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Terca, "Terça-feira"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Quarta, "Quarta-feira"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Quinta, "Quinta-feira"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Sexta, "Sexta-feira"));
            lstDiasSemana.Add(new GenericModel((int)Model.DiasSemana.Sabado, "Sábado"));

            return lstDiasSemana.ToArray();
        }

        public GenericModel[] GetRotasExternas()
        {
            var rotas = RotaDAO.Instance.ObtemRotasExternas().OrderBy(f => f).ToList();

            List<GenericModel> lstRotas = new List<GenericModel>();

            for (int i = 0; i < rotas.Count; i++)
                lstRotas.Add(new GenericModel(i + 1, rotas[i]));

            return lstRotas.ToArray();
        }

        public GenericModel[] GetClientesExternos()
        {
            List<GenericModel> lstRotas = new List<GenericModel>();

            var clientes = ClienteDAO.Instance.ObtemClientesExternos();

            for (int i = 0; i < clientes.Count; i++)
                lstRotas.Add(new GenericModel(i + 1, clientes[i].Key + " - " + clientes[i].Value));

            return lstRotas.ToArray();
        }

        /// <summary>
        /// Flgas do tipo de arquivo da mesa de corte
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetFlagTipoArquivoMesaCorte()
        {
            List<GenericModel> flags = new List<GenericModel>();

            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.DXF, "DXF"));
            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.FML, "FML"));
            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.FMLBasico, "FMLBasico"));
            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.FORTxt, "FORTxt"));
            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.ISO, "ISO"));
            flags.Add(new GenericModel((int)Model.TipoArquivoMesaCorte.SAG, "SAG"));

            return flags.OrderBy(f => f.Descr).ToArray();
        }

        /// <summary>
        /// Retorna todas os tipos de validação de peça do projeto.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoValidacaoPecaProjetoModelo()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.Bloquear, "Bloquear"));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.SomenteInformar, "Somente Informar"));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.ConsiderarConfiguracao, "Considerar Configuração"));

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna todas os tipos de validação de peça do projeto, para a tela de configurações.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoValidacaoPecaProjetoModeloParaConfiguracoes()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.Bloquear, "Bloquear"));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.SomenteInformar, "Somente Informar"));

            return lstTipo.ToArray();
        }

        /// <summary>
        /// Retorna todas os tipos de comparador de validação de peça do projeto.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetTipoComparadorValidacaoPecaProjetoModelo()
        {
            List<GenericModel> lstTipo = new List<GenericModel>();
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Igual, "="));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Maior, ">"));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Menor, "<"));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.MaiorOuIgual, ">="));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.MenorOuIgual, "<="));
            lstTipo.Add(new GenericModel((int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Diferente, "<>"));

            return lstTipo.ToArray();
        }

        public enum BloqEmisPedidoPorPosicaoMateriaPrima
        {
            NaoBloquear,
            Bloquear,
            Informar
        }

        public GenericModel[] GetBloqEmisPedidoPorPosicaoMateriaPrima()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrBloqEmisPedidoPorPosicaoMateriaPrima);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(BloqEmisPedidoPorPosicaoMateriaPrima), d, false).ToArray();
        }

        public string GetDescrBloqEmisPedidoPorPosicaoMateriaPrima(int tipobloqueio)
        {
            switch (tipobloqueio)
            {
                case (int)BloqEmisPedidoPorPosicaoMateriaPrima.NaoBloquear: return "Não Bloquear";
                case (int)BloqEmisPedidoPorPosicaoMateriaPrima.Bloquear: return "Bloquear";
                case (int)BloqEmisPedidoPorPosicaoMateriaPrima.Informar: return "Informar";
                default: return "";
            }
        }

        #region Config

        public enum TipoExportacaoEtiquetaEnum
        {
            Nenhum,
            CorteCerto,
            OptyWay
        }

        public GenericModel[] GetTipoExportacaoEtiqueta()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoExportacaoEtiqueta);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoExportacaoEtiquetaEnum), d, false).ToArray();
        }

        public string GetDescrTipoExportacaoEtiqueta(int tipoExportacao)
        {
            switch (tipoExportacao)
            {
                case (int)TipoExportacaoEtiquetaEnum.Nenhum: return "Nenhum";
                case (int)TipoExportacaoEtiquetaEnum.CorteCerto: return "Corte Certo";
                case (int)TipoExportacaoEtiquetaEnum.OptyWay: return "OptyWay";
                default: return "";
            }
        }

        public enum TipoDataEtiquetaEnum
        {
            Entrega,
            Fábrica
        }

        public enum TipoReposicaoEnum
        {
            Nenhum,
            Pedido,
            Peca
        }

        public GenericModel[] GetTipoReposicao()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoReposicao);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoReposicaoEnum), d, false).ToArray();
        }

        public string GetDescrTipoReposicao(int tipoReposicao)
        {
            switch (tipoReposicao)
            {
                case (int)TipoReposicaoEnum.Nenhum: return "Nenhum";
                case (int)TipoReposicaoEnum.Peca: return "Reposição de Peça";
                case (int)TipoReposicaoEnum.Pedido: return "Reposição de Pedido";
                default: return "";
            }
        }

        public enum TipoEnvioAnexoPedidoEmail
        {
            Nenhum,
            PCP,
            Pronto,
            Ambos
        }

        public enum TipoContingenciaNFe
        {
            NaoUtilizar,
            SCAN,
            FSDA
        }

        public enum TipoContingenciaCTe
        {
            NaoUtilizar,
            SVC
        }

        public enum TipoContingenciaMDFe
        {
            NaoUtilizar
        }

        public GenericModel[] GetTipoContingenciaNFe()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoContingenciaNFe);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoContingenciaNFe), d, false).ToArray();
        }

        public string GetDescrTipoContingenciaNFe(int tipoContingencia)
        {
            switch (tipoContingencia)
            {
                case (int)TipoContingenciaNFe.NaoUtilizar: return "Não Utilizar";
                case (int)TipoContingenciaNFe.SCAN: return "SCAN";
                case (int)TipoContingenciaNFe.FSDA: return "Formulário de Segurança (FS-DA)";
                default: return "";
            }
        }

        public enum TipoNomeExibirRelatorioPedido
        {
            NomeFantasia,
            RazaoSocial
        }

        public GenericModel[] GetTipoNomeExibirRelatorioPedido()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoNomeExibirRelatorioPedido);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoNomeExibirRelatorioPedido), d, false).ToArray();
        }

        public string GetDescrTipoNomeExibirRelatorioPedido(int tipoNomeExibir)
        {
            switch (tipoNomeExibir)
            {
                case (int)TipoNomeExibirRelatorioPedido.NomeFantasia: return "Nome Fantasia/Razão Social";
                case (int)TipoNomeExibirRelatorioPedido.RazaoSocial: return "Razão Social/Nome Fantasia";
                default: return "";
            }
        }

        public GenericModel[] GetTipoContingenciaCTe()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoContingenciaCTe);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoContingenciaCTe), d, false).ToArray();
        }

        public string GetDescrTipoContingenciaCTe(int tipoContingencia)
        {
            switch (tipoContingencia)
            {
                case (int)TipoContingenciaCTe.NaoUtilizar: return "Não Utilizar";
                case (int)TipoContingenciaCTe.SVC: return "SVC";                
                default: return "";
            }
        }

        public GenericModel[] GetTipoContingenciaMDFe()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoContingenciaMDFe);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoContingenciaMDFe), d, false).ToArray();
        }

        public string GetDescrTipoContingenciaMDFe(int tipoContingencia)
        {
            switch (tipoContingencia)
            {
                case (int)TipoContingenciaMDFe.NaoUtilizar: return "Não Utilizar";
                //case (int)TipoContingenciaMDFe.SVC: return "SVC";
                default: return "";
            }
        }

        public GenericModel[] GetCrtLoja()
        {
            return new[] {
                new GenericModel((int)CrtLoja.LucroPresumido, "Lucro Presumido"),
                new GenericModel((int)CrtLoja.LucroReal, "Lucro Real"),
                new GenericModel((int)CrtLoja.SimplesNacional, "Simples Nacional")
            };
        }

        #endregion

        #region Meses

        public enum MesEnum
        {
            Janeiro = 1,
            Fevereiro,
            Marco,
            Abril,
            Maio,
            Junho,
            Julho,
            Agosto,
            Setembro,
            Outubro,
            Novembro,
            Dezembro
        }

        public GenericModel[] GetMeses()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrMes);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(MesEnum), d, false).ToArray();
        }

        public string GetDescrMes(int mes)
        {
            switch (mes)
            {
                case (int)MesEnum.Marco: return "Março";
                default: return ((MesEnum)mes).ToString();
            }
        }

        #endregion

        #region CNAB

        public GenericModel[] ListaBancosCnab()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.BancoBrasil, "Banco do Brasil"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Banrisul, "Banrisul"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Bradesco, "Bradesco"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.CaixaEconomicaFederal, "Caixa Econômica Federal"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Itau, "Itaú"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Santander, "Santander"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Sicoob, "Sicoob"));
            lstRetorno.Add(new GenericModel((uint)Sync.Utils.CodigoBanco.Sicredi, "Sicredi"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] ListaCodigoOcorrencia()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel(1, "REMESSA"));
            lstRetorno.Add(new GenericModel(2, "PEDIDO DE BAIXA"));
            lstRetorno.Add(new GenericModel(4, "CONCESSÃO DE ABATIMENTO (INDICADOR 12.5)"));
            lstRetorno.Add(new GenericModel(5, "CANCELAMENTO DE ABATIMENTO"));
            lstRetorno.Add(new GenericModel(6, "ALTERAÇÃO DO VENCIMENTO"));
            lstRetorno.Add(new GenericModel(9, "PROTESTAR"));
            lstRetorno.Add(new GenericModel(10, "NÃO PROTESTAR (INIBE O PROTESTO AUTOMÁTICO)"));
            lstRetorno.Add(new GenericModel(18, "SUSTAR O PROTESTO"));
            lstRetorno.Add(new GenericModel(38, "CEDENTE NÃO CONCORDA COM A ALEGAÇÃO DO SACADO"));
            lstRetorno.Add(new GenericModel(31, "ALTERAÇÃO DE OUTROS DADOS"));
            lstRetorno.Add(new GenericModel(41, "EXCLUSÃO DE SACADOR AVALISTA"));
            
            return lstRetorno.ToArray();
        }

        public GenericModel[] ListaCarteira()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel(148, "D/148 - DIRETA COM IOF 0,38%"));
            lstRetorno.Add(new GenericModel(149, "D/149 - DIRETA COM IOF 2,38%"));
            lstRetorno.Add(new GenericModel(153, "D/153 - DIRETA COM IOF 7,38%"));
            lstRetorno.Add(new GenericModel(108, "D/108 - DIRETA ELETRÔNICA EMISSÃO INTEGRAL – CARNÊ"));
            lstRetorno.Add(new GenericModel(180, "D/180 - DIRETA ELETRÔNICA EMISSÃO INTEGRAL – SIMPLES/CONTRATUAL"));
            lstRetorno.Add(new GenericModel(280, "D/280 - DIRETA ELETRÔNICA EMISSÃO INTEGRAL – SIMPLES/CONTRATUAL"));
            lstRetorno.Add(new GenericModel(121, "D/121 - DIRETA ELETRÔNICA EMISSÃO PARCIAL - SIMPLES/CONTRATUAL"));
            lstRetorno.Add(new GenericModel(221, "D/221 - DIRETA ELETRÔNICA EMISSÃO PARCIAL - SIMPLES/CONTRATUAL"));
            lstRetorno.Add(new GenericModel(210, "D/210 - DIRETA ELETRÔNICA SEM EMISSÃO – CONTRATUAL"));
            lstRetorno.Add(new GenericModel(150, "D/150 - DIRETA ELETRÔNICA SEM EMISSÃO – DÓLAR"));
            lstRetorno.Add(new GenericModel(109, "D/109 - DIRETA ELETRÔNICA SEM EMISSÃO – SIMPLES"));
            lstRetorno.Add(new GenericModel(110, "D/110 - DIRETA ELETRÔNICA SEM EMISSÃO – SIMPLES"));
            lstRetorno.Add(new GenericModel(111, "D/111 - DIRETA ELETRÔNICA SEM EMISSÃO – SIMPLES"));
            lstRetorno.Add(new GenericModel(168, "D/168 - DIRETA ELETRÔNICA SEM EMISSÃO – TR"));
            lstRetorno.Add(new GenericModel(191, "191 - DUPLICATAS - TRANSFERÊNCIA DE DESCONTO"));
            lstRetorno.Add(new GenericModel(116, "E/116 - ESCRITURAL CARNE COM IOF 0,38%"));
            lstRetorno.Add(new GenericModel(117, "E/117 - ESCRITURAL CARNE COM IOF 2,38%"));
            lstRetorno.Add(new GenericModel(119, "E/119 - ESCRITURAL CARNE COM IOF 7,38%"));
            lstRetorno.Add(new GenericModel(134, "E/134 - ESCRITURAL COM IOF 0,38"));
            lstRetorno.Add(new GenericModel(135, "E/135 - ESCRITURAL COM IOF 2,38%"));
            lstRetorno.Add(new GenericModel(136, "E/136 - ESCRITURAL COM IOF 7,38%"));
            lstRetorno.Add(new GenericModel(104, "E/104 - ESCRITURAL ELETRÔNICA – CARNÊ"));
            lstRetorno.Add(new GenericModel(147, "E/147 - ESCRITURAL ELETRÔNICA – DÓLAR"));
            lstRetorno.Add(new GenericModel(105, "E/105 - ESCRITURAL ELETRÔNICA - DÓLAR – CARNÊ"));
            lstRetorno.Add(new GenericModel(112, "E/112 - ESCRITURAL ELETRÔNICA - SIMPLES / CONTRATUAL"));
            lstRetorno.Add(new GenericModel(212, "E/212 - ESCRITURAL ELETRÔNICA - SIMPLES / CONTRATUAL"));
            lstRetorno.Add(new GenericModel(166, "E/166 - ESCRITURAL ELETRÔNICA – TR"));
            lstRetorno.Add(new GenericModel(113, "E/113 - ESCRITURAL ELETRÔNICA - TR – CARNÊ"));
            lstRetorno.Add(new GenericModel(177, "S/177 - SEM REGISTRO EMISSÃO PARCIAL COM PROTESTO ELETRÔNICO"));
            lstRetorno.Add(new GenericModel(179, "S/179 - SEM REGISTRO SEM EMISSÃO COM PROTESTO ELETRÔNICO"));
            lstRetorno.Add(new GenericModel(172, "S/172 - SEM REGISTRO COM EMISSÃO INTEGRAL"));
            lstRetorno.Add(new GenericModel(195, "S/195 - SEM REGISTRO COM EMISSÃO INTEGRAL - 15 DÍGITOS"));
            lstRetorno.Add(new GenericModel(107, "S/107 - SEM REGISTRO COM EMISSÃO INTEGRAL - 15 DÍGITOS – CARNÊ"));
            lstRetorno.Add(new GenericModel(204, "S/204 - SEM REGISTRO COM EMISSAO COM IOF 0,38%"));
            lstRetorno.Add(new GenericModel(205, "S/205 - SEM REGISTRO COM EMISSAO COM IOF 2,38%"));
            lstRetorno.Add(new GenericModel(206, "S/206 - SEM REGISTRO COM EMISSAO COM IOF 7,38%"));
            lstRetorno.Add(new GenericModel(173, "S/173 - SEM REGISTRO COM EMISSÃO E ENTREGA"));
            lstRetorno.Add(new GenericModel(196, "S/196 - SEM REGISTRO COM EMISSÃO E ENTREGA - 15 DÍGITOS"));
            lstRetorno.Add(new GenericModel(106, "S/106 - SEM REGISTRO COM EMISSÃO E ENTREGA - 15 DÍGITOS – CARNÊ"));
            lstRetorno.Add(new GenericModel(103, "S/103 - SEM REGISTRO COM EMISSÃO E ENTREGA – CARNÊ"));
            lstRetorno.Add(new GenericModel(102, "S/102 - SEM REGISTRO COM EMISSÃO INTEGRAL – CARNÊ"));
            lstRetorno.Add(new GenericModel(174, "S/174 - SEM REGISTRO EMISSÃO PARCIAL COM PROTESTO BORDERÔ"));
            lstRetorno.Add(new GenericModel(175, "S/175 - SEM REGISTRO SEM EMISSÃO"));
            lstRetorno.Add(new GenericModel(198, "S/198 - SEM REGISTRO SEM EMISSÃO 15 DÍGITOS"));
            lstRetorno.Add(new GenericModel(167, "S/167 - SEM REGISTRO SEM EMISSAO COM IOF 0,38%"));
            lstRetorno.Add(new GenericModel(202, "S/202 - SEM REGISTRO SEM EMISSAO COM IOF 2,38%"));

            lstRetorno.Sort(
                delegate(GenericModel x1, GenericModel x2)
                {
                    return x1.Id.GetValueOrDefault().CompareTo(x2.Id.GetValueOrDefault());
                }
            );

            return lstRetorno.ToArray();
        }

        public GenericModel[] ListaEspecieDocumento()
        {
            List<GenericModel> lstRetorno = new List<GenericModel>();
            lstRetorno.Add(new GenericModel(1, "DUPLICATA MERCANTIL"));
            lstRetorno.Add(new GenericModel(2, "NOTA PROMISSÓRIA"));
            lstRetorno.Add(new GenericModel(3, "NOTA DE SEGURO"));
            lstRetorno.Add(new GenericModel(4, "MENSALIDADE ESCOLAR"));
            lstRetorno.Add(new GenericModel(5, "RECIBO"));
            lstRetorno.Add(new GenericModel(6, "CONTRATO"));
            lstRetorno.Add(new GenericModel(7, "COSSEGUROS"));
            lstRetorno.Add(new GenericModel(8, "DUPLICATA DE SERVIÇO"));
            lstRetorno.Add(new GenericModel(9, "LETRA DE CÂMBIO"));
            lstRetorno.Add(new GenericModel(13, "NOTA DE DÉBITOS"));
            lstRetorno.Add(new GenericModel(15, "DOCUMENTO DE DÍVIDA"));
            lstRetorno.Add(new GenericModel(16, "ENCARGOS CONDOMINIAIS"));
            lstRetorno.Add(new GenericModel(17, "CONTA DE PRESTAÇÃO DE SERVIÇOS"));
            lstRetorno.Add(new GenericModel(99, "DIVERSOS"));

            return lstRetorno.ToArray();
        }

        public GenericModel[] ListaInstrucoes()
        {
            ArrayList instrucoes = (ArrayList)typeof(InstrucaoCobrancaItau).ToList();

            List<GenericModel> lstRetorno = new List<GenericModel>();

            foreach (KeyValuePair<Enum, string> i in instrucoes)
            {
                lstRetorno.Add(new GenericModel(Conversoes.ConverteValor<uint?>(i.Key), i.Value));
            }

            return lstRetorno.ToArray();
        }

        #endregion

        #region Avaliação Atendimento

        public GenericModel[] GetSatisfacaoAvaliacaoAtendimento()
        {
            List<GenericModel> lstSatisfacao = new List<GenericModel>();
            lstSatisfacao.Add(new GenericModel((int)AvaliacaoAtendimento.SatisfacaoEnum.MuitoBaixa, "Muito Baixa"));
            lstSatisfacao.Add(new GenericModel((int)AvaliacaoAtendimento.SatisfacaoEnum.Baixa, "Baixa"));
            lstSatisfacao.Add(new GenericModel((int)AvaliacaoAtendimento.SatisfacaoEnum.Neutra, "Neutra"));
            lstSatisfacao.Add(new GenericModel((int)AvaliacaoAtendimento.SatisfacaoEnum.Alta, "Alta"));
            lstSatisfacao.Add(new GenericModel((int)AvaliacaoAtendimento.SatisfacaoEnum.MuitoAlta, "Muito Alta"));

            return lstSatisfacao.ToArray();
        }

        public static AvaliacaoAtendimento.SatisfacaoEnum GetSatisfacaoAvaliacaoAtendimento(uint satisfacao)
        {
            return (AvaliacaoAtendimento.SatisfacaoEnum)Enum.Parse(typeof(AvaliacaoAtendimento.SatisfacaoEnum), satisfacao.ToString());
        }

        #endregion
    }
}