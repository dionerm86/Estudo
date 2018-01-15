using System;
using System.Linq;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Estoque.Negocios.Entidades;
using GDA;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    public sealed class VolumeFluxo : BaseFluxo<VolumeFluxo>, Glass.PCP.Negocios.IVolumeFluxo
    {
        #region Retorno de Itens

        public IList<Glass.Data.Model.Volume> GetList(uint idVolume, uint idPedido, string situacao, string sortExpression, int startRow, int pageSize)
        {
            return VolumeDAO.Instance.GetList(idVolume, idPedido, situacao, sortExpression, startRow, pageSize);
        }

        public int GetListCount(uint idVolume, uint idPedido, string situacao)
        {
            return VolumeDAO.Instance.GetListCount(idVolume, idPedido, situacao);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Fecha um volume
        /// </summary>
        /// <param name="idVolume"></param>
        public void FecharVolume(uint idVolume)
        {
            ValidaFecharVolume(idVolume);

            Glass.Data.DAL.VolumeDAO.Instance.FecharVolume(idVolume);
        }

        static volatile object _addItemLock = new object();

        /// <summary>
        /// Adiciona uma lista de itens a um volume
        /// </summary>
        public uint AddItem(uint idPedido, uint idVolume, Dictionary<uint, float> itens)
        {
            lock(_addItemLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (idPedido == 0)
                            throw new Exception("Pedido não informado.");

                        if (itens == null || itens.Count == 0)
                            throw new Exception("Nenhum item informado.");

                        /* Chamado 66194. */
                        foreach (var idOrdemCarga in PedidoOrdemCargaDAO.Instance.ObterIdsOrdemCargaPeloPedido(transaction, (int)idPedido))
                        {
                            var idCarregamentoOc = OrdemCargaDAO.Instance.GetIdCarregamento(transaction, (uint)idOrdemCarga);

                            if (idCarregamentoOc > 0)
                                throw new Exception(string.Format("Não é possível gerar um volume para o pedido {0}, pois ele está vinculado a um carregamento. {1}", idPedido,
                                    string.Format("Desvincule-o do carregamento, de código {0}, para gerar um novo volume. OC: {1}.", idCarregamentoOc, idOrdemCarga)));
                        }

                        // Se o volume ainda não existir cria o mesmo.
                        if (idVolume == 0)
                            idVolume = VolumeDAO.Instance.Insert(transaction, new Glass.Data.Model.Volume(idPedido));

                        foreach (var item in itens)
                        {
                            if (item.Value == 0)
                                continue;

                            // Chamado 46217.
                            // Recupera a quantidade do produto de pedido que gerou volume.
                            var quantidadeProdutoPedidoGerouVolume = VolumeProdutosPedidoDAO.Instance.ObterQuantidadeProdutoPedidoVolumeGerado(transaction, null, (int)item.Key);
                            // Recupera a quantidade do produto de pedido.
                            var quantidadeProdutoPedido = ProdutosPedidoDAO.Instance.ObtemQtde(transaction, item.Key);

                            // Verifica se a quantidade a ser adicionada ultrapassa a quantidade disponível do produto,
                            // pelo fato do produto já ter gerado volume de sua quantidade total.
                            if (quantidadeProdutoPedidoGerouVolume + item.Value > quantidadeProdutoPedido)
                                throw new Exception("Quantidade a ser adicionada é maior que a quantidade disponível. " +
                                    "Verifique a quantidade desse produto no pedido e a quantidade desse produto que já possui volume gerado.");

                            // Busca o volume produtos pedido se o mesmo ja existir, pois pode ser que esteja adicionando
                            // mais quantidades do item ao volume.
                            var vpp = VolumeProdutosPedidoDAO.Instance.GetElement(transaction, idVolume, item.Key);

                            // Se o volume produtos pedido ainda não existir cria o mesmo.
                            if (vpp == null)
                            {
                                vpp = new Glass.Data.Model.VolumeProdutosPedido()
                                {
                                    IdVolume = idVolume,
                                    IdProdPed = item.Key
                                };
                            }

                            // Adiciona a quantidade de itens ao volume.
                            vpp.Qtde += item.Value;

                            // Salva as alterações.
                            VolumeProdutosPedidoDAO.Instance.InsertOrUpdate(transaction, vpp);
                        }
                        
                        transaction.Commit();
                        transaction.Close();

                        return idVolume;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }
        
        static volatile object _removeItemLock = new object();

        /// <summary>
        /// Remove uma lista de itens de um volume
        /// </summary>
        public void RemoveItem(uint idVolume, Dictionary<uint, float> itens)
        {
            lock(_removeItemLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (idVolume == 0)
                            throw new Exception("Volume não informado.");

                        if (itens == null || itens.Count == 0)
                            throw new Exception("Nenhum item informado.");

                        // Chamado 46217.
                        // Recupera a quantidade de produtos que existem no volume.
                        var quantidadeProdutoVolume = VolumeProdutosPedidoDAO.Instance.ObterQuantidadeProdutoPedidoVolumeGerado(transaction, (int)idVolume, null);

                        // Verifica se todos os produtos estão sendo removidos e obriga o usuário a cancelar o volume ao invés de remover todos os produtos.
                        if (quantidadeProdutoVolume - itens.Sum(f => f.Value) <= 0)
                            throw new Exception("Todos os produtos estão sendo removidos do volume, portanto, cancele-o ao invés de remover os produtos.");

                        //percorre as itens a serem removidas
                        foreach (var item in itens)
                        {
                            //Se não for informado qntde pula para o proximo item.
                            if (item.Value == 0)
                                continue;

                            var vpp = VolumeProdutosPedidoDAO.Instance.GetElement(transaction, idVolume, item.Key);

                            if (vpp == null)
                                throw new Exception("Item não encontrado");

                            //verifica se a qtde a ser removida e maior que a disponível
                            if (vpp.Qtde < item.Value)
                                throw new Exception("Qtde. a ser removida maior que a qtde. disponível.");

                            //Diminiu a quantidade
                            vpp.Qtde -= item.Value;

                            //Se a quantidade for zerada, deleta o volume produtos pedido
                            if (vpp.Qtde == 0)
                                VolumeProdutosPedidoDAO.Instance.Delete(transaction, vpp);
                            else
                                VolumeProdutosPedidoDAO.Instance.Update(transaction, vpp);
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Métodos de Validação

        /// <summary>
        /// Valida a inclusão de um item no volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <param name="idsProdPed"></param>
        public void ValidaAddItem(uint idVolume, string idsProdPed)
        {
            if (idVolume == 0)
                throw new Exception("Informe o volume.");

            if (string.IsNullOrEmpty(idsProdPed))
                throw new Exception("Infome os itens a serem adicionados no volume.");
        }

        /// <summary>
        /// Valida o fechamento de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        public void ValidaFecharVolume(uint idVolume)
        {
            if (idVolume == 0)
                throw new Exception("Informe o volume.");

            var volume = VolumeDAO.Instance.GetElement(idVolume);

            if (volume == null)
                throw new Exception("Informe o volume.");

            if (volume.Situacao != Glass.Data.Model.Volume.SituacaoVolume.Aberto)
                throw new Exception("Apenas volumes abertos podem ser fechados.");

            if (volume.QtdeItens == 0)
                throw new Exception("Este volume não possui nenhum item.");
        }

        #endregion

        #region Expedição

        static volatile object _marcaExpedicaoVolumeLock = new object();

        /// <summary>
        /// Efetua a saida de expedicao de um volume
        /// </summary>
        public string MarcaExpedicaoVolumeComTransacao(string codEtiquetaVolume, int idLiberarPedido, bool balcao)
        {
            lock(_marcaExpedicaoVolumeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var retorno = MarcaExpedicaoVolume(transaction, codEtiquetaVolume, idLiberarPedido, balcao);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Efetua a saida de expedicao de um volume
        /// </summary>
        public string MarcaExpedicaoVolume(GDASession sessao, string codEtiquetaVolume, int idLiberarPedido, bool balcao)
        {
            //Verifica se a empresa da saída de volume
            if (!EstoqueConfig.SaidaEstoqueVolume && !balcao)
                throw new Exception("Não é possível dar saída de estoque em etiquetas de volume.");

            //Busca o id do volume
            var idVolume = Glass.Conversoes.StrParaUint(codEtiquetaVolume.Substring(1));

            //Verifica se o volume existe
            if (!VolumeDAO.Instance.Exists(sessao, idVolume))
                throw new Exception("Volume não encontrado");

            //Verifica se o volume já foi expedido.
            if (!VolumeDAO.Instance.VerificaSePodeExpedir(sessao, idVolume))
                throw new Exception("Este volume já foi expedido.");

            //Busca o pedido do volume
            var idPedido = VolumeDAO.Instance.GetIdPedido(sessao, idVolume);

            //So pode expedir se o pedido do volume for entrega balcão,
            //caso contrario deve ser expedido no carregamento.
            if (PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPedido) != (int)Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                throw new Exception("Este volume deve ser expedido através do carregamento, o seu pedido não é de entrega balcão.");

            //Verifica se o volume é do carregamento informado
            if (balcao && !Glass.Data.DAL.VolumeDAO.Instance.FazParteLiberacao(sessao, idVolume, idLiberarPedido))
                throw new Exception("O volume informado não faz parte desta liberação.");

            // Se a empresa obriga o financeiro a liberar pedido para entrega, verifica se o mesmo está liberado,
            if (FinanceiroConfig.UsarControleLiberarFinanc && !PedidoDAO.Instance.IsLiberadoEntregaFinanc(sessao, idPedido))
                throw new Exception("O financeiro não liberou o pedido deste volume para entrega.");

            // Verifica se o pedido já foi liberado
            if (PedidoConfig.LiberarPedido)
            {
                var situacao = PedidoDAO.Instance.ObtemSituacao(sessao, idPedido);

                if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado &&
                    situacao != Glass.Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente)
                {
                    throw new Exception("O pedido deste volume ainda não foi liberado.");
                }
            }

            //Faz a baixa dos produtos
            var produtos = VolumeProdutosPedidoDAO.Instance.GetList(sessao, idVolume.ToString());
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
            var dados = new List<DetalhesBaixaEstoque>();

            foreach (var prod in produtos)
                dados.Add(new DetalhesBaixaEstoque()
                {
                    IdProdPed = (int)prod.IdProdPed,
                    Qtde = prod.Qtde,
                    DescricaoBaixa = prod.DescProd
                });

            //Efetua a saida dos produtos
            Pedido.Fluxo.AlterarEstoque.Instance.BaixarEstoque(sessao, idLoja, dados, idVolume, null, false);

            //Marca volume como expedido
            VolumeDAO.Instance.MarcaExpedicaoVolume(sessao, idVolume);

            return "Volume " + codEtiquetaVolume + " Pedido " + idPedido;
        }

        /// <summary>
        /// Estorna a expedição balcão de um volume
        /// </summary>
        public void EstornaExpedicaoVolume(GDASession sessao, uint idVolume)
        {
            var idPedido = VolumeDAO.Instance.GetIdPedido(sessao, idVolume);
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
            var produtos = VolumeProdutosPedidoDAO.Instance.GetList(sessao, idVolume.ToString());
            var dados = new List<DetalhesBaixaEstoque>();

            foreach (var prod in produtos)
                dados.Add(new DetalhesBaixaEstoque()
                {
                    IdProdPed = (int)prod.IdProdPed,
                    Qtde = prod.Qtde,
                    DescricaoBaixa = prod.DescProd
                });

            Pedido.Fluxo.AlterarEstoque.Instance.EstornaBaixaEstoque(sessao, idLoja, dados, idVolume, null);
            VolumeDAO.Instance.EstornaExpedicaoVolume(sessao, idVolume);
        }

        #endregion

        #region Delete

        static volatile object _apagarVolumeLock = new object();

        public int Delete(Glass.Data.Model.Volume objDelete)
        {
            lock(_apagarVolumeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (objDelete.IdVolume == 0)
                            throw new Exception("Nenhum volume informado.");

                        var idPedido = VolumeDAO.Instance.GetIdPedido(transaction, objDelete.IdVolume);

                        //Valida se o pedido deste volume ja tem OC se tiver não pode deletar
                        if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(transaction, idPedido))
                            throw new Exception("O pedido do volume informado esta vinculado a uma OC.");

                        if (VolumeDAO.Instance.TemExpedicao(transaction, objDelete.IdVolume))
                        {
                            var produtos = VolumeProdutosPedidoDAO.Instance.GetList(transaction, objDelete.IdVolume.ToString());
                            var idLoja = PedidoDAO.Instance.ObtemIdLoja(transaction, idPedido);
                            var dados = new List<DetalhesBaixaEstoque>();

                            foreach (var prod in produtos)
                                dados.Add(new DetalhesBaixaEstoque()
                                {
                                    IdProdPed = (int)prod.IdProdPed,
                                    Qtde = prod.Qtde,
                                    DescricaoBaixa = prod.DescProd
                                });

                            Pedido.Fluxo.AlterarEstoque.Instance.EstornaBaixaEstoque(transaction, idLoja, dados, objDelete.IdVolume, null);
                            VolumeDAO.Instance.EstornaExpedicaoVolume(transaction, objDelete.IdVolume);
                        }


                        //Deleta os itens do volume.
                        VolumeProdutosPedidoDAO.Instance.DeleteByVolume(transaction, objDelete.IdVolume);

                        var retorno = VolumeDAO.Instance.Delete(transaction, objDelete);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Ajax

        private static Ajax.IVolume _ajax = null;

        public static Ajax.IVolume Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Volume();

                return _ajax;
            }
        }

        #endregion
    }
}
