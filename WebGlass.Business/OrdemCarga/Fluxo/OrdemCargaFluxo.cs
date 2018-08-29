using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Configuracoes;
using GDA;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    public class OrdemCargaFluxo : BaseFluxo<OrdemCargaFluxo>
    {
        private OrdemCargaFluxo() { }

        #region Recupera uma listagem para gerar OCs

        /// <summary>
        /// Recupera uma listagem para gerar OCs
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <param name="idLoja"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoOC"></param>
        /// <param name="pedidosObs"></param>
        /// <returns></returns>
        public Entidade.ListagemOrdemCarga[] GetListForGerarOC(string idsRotas, uint idLoja, uint idCli, string nomeCli,
            string dtEntPedidoIni, string dtEntPedidoFin, string situacao, Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, bool pedidosObs,
            uint idPedido, string idsRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao)
        {
            if ((string.IsNullOrEmpty(idsRotas) && idPedido == 0 && string.IsNullOrEmpty(idsRotasExternas)) || idLoja == 0 || (string.IsNullOrEmpty(dtEntPedidoIni) && string.IsNullOrEmpty(dtEntPedidoFin)))
                return null;

            if (idPedido > 0)
            {
                idCli = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
                nomeCli = "";

                var idRotaCli = ClienteDAO.Instance.ObtemIdRota(idCli);

                if (!string.IsNullOrEmpty(idsRotas) && !("," + idsRotas + ",").Contains("" + idRotaCli + ","))
                {
                    idsRotas += "," + idRotaCli;
                }
                else if (string.IsNullOrEmpty(idsRotas))
                {
                    idsRotas = idRotaCli.ToString();
                }
            }

            //Buscar os ids dos pedidos para criar a listagemOrdemCarga
            var pedidos = PedidoDAO.Instance.GetIdsPedidosForOC(tipoOC, idCli, nomeCli, 0, idsRotas, idLoja,
                dtEntPedidoIni, dtEntPedidoFin, true, pedidosObs, idsRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao);

            //Cria um dicionario(idCli, pedidos) para guardar os pedidos dos cliente.
            var lstPedidos = new Dictionary<uint, KeyValuePair<uint, string>>();

            //Adiciona os pedidos ao dicionario
            if (pedidos != null && pedidos.Count > 0)
            {
                lstPedidos = pedidos.ToDictionary(p => Glass.Conversoes.StrParaUint(p.Split(';')[0]),
                    p => new KeyValuePair<uint, string>(Glass.Conversoes.StrParaUint(p.Split(';')[1]), p.Split(';')[2]));
            }

            //Se não houver pedidos retorna nulo.
            if (lstPedidos.Count == 0)
                return null;

            //Cria a listagemOrdemCarga
            var retorno = new List<Entidade.ListagemOrdemCarga>();

            //Preenche a listagemOrdemCarga
            foreach (var item in lstPedidos)
                retorno.Add(new Entidade.ListagemOrdemCarga(item, idLoja, dtEntPedidoIni, dtEntPedidoFin, tipoOC, pedidosObs, idsRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao));

            if (string.IsNullOrEmpty(situacao) || situacao.Equals("1,2"))
            {
                return retorno.OrderBy(p => p.NomeCliente).ToArray();
            }
            else
            {
                if (("," + situacao + ",").Contains(",1,"))
                    return retorno.Where(c => c.NumPedidosParaGerar == 0).OrderBy(p => p.NomeCliente).ToArray();
                else
                    return retorno.Where(c => c.NumPedidosParaGerar > 0).OrderBy(p => p.NomeCliente).ToArray();
            }            
        }

        #endregion

        #region Recupera uma listagem das OCs criadas

        /// <summary>
        /// Recupera uma lista de OCs
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="dtEntPedido"></param>
        /// <param name="tipoOC"></param>
        /// <returns></returns>
        public Glass.Data.Model.OrdemCarga[] GetList(uint idCliente, uint idLoja, string dtEntPedidoIni, string dtEntPedidoFin,
            string tipoOC)
        {
            return OrdemCargaDAO.Instance.GetList(idCliente, idLoja, 0, dtEntPedidoIni, dtEntPedidoFin, tipoOC,
                ((int)Glass.Data.Model.OrdemCarga.SituacaoOCEnum.Finalizado).ToString());
        }
        
        /// <summary>
        /// Recupera uma ordem de carga para o relatório individual
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public Glass.Data.Model.OrdemCarga[] GetForRptInd(uint idOC)
        {
            return OrdemCargaDAO.Instance.GetForRptInd(idOC);
        }

        /// <summary>
        /// Recupera uma lista de OCs
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipo"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Data.Model.OrdemCarga> GetListWithExpression(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, uint idLoja, uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo,
            uint idCliExterno, string nomeCliExterno, string idsRotasExternas, string sortExpression, int startRow, int pageSize)
        {
            return OrdemCargaDAO.Instance.GetListWithExpression(idCarregamento, idOC, idPedido, idCliente, nomeCliente, idLoja, idRota, dtEntPedidoIni,
                dtEntPedidoFin, situacao, tipo, idCliExterno, nomeCliExterno, idsRotasExternas, sortExpression, startRow, pageSize);
        }

        public int GetListWithExpressionCount(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, uint idLoja, uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo,
            uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            return OrdemCargaDAO.Instance.GetListWithExpressionCount(idCarregamento, idOC, idPedido, idCliente, nomeCliente, idLoja, idRota,
                dtEntPedidoIni, dtEntPedidoFin, situacao, tipo, idCliExterno, nomeCliExterno, idsRotasExternas);
        }

        /// <summary>
        /// Recupera uma lista de OCs
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public IList<Glass.Data.Model.OrdemCarga> GetListForRpt(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente,
            uint idLoja, uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo, uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            return OrdemCargaDAO.Instance.GetListForRpt(idCarregamento, idOC, idPedido, idCliente, nomeCliente, idLoja, idRota,
                dtEntPedidoIni, dtEntPedidoFin, situacao, tipo, idCliExterno, nomeCliExterno, idsRotasExternas);
        }

        #endregion

        #region Recupera os ids dos pedidos de uma OC

        /// <summary>
        /// Recupera os ids dos pedidos de uma OC
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public string GetIdsPedidosByOCForLiberacao(uint idOC)
        {
            if (OrdemCargaDAO.Instance.GetTipoOrdemCarga(idOC) == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia)
                throw new Exception("Não é possível liberar uma OC de transferência.");

            var ids = PedidoDAO.Instance.GetIdsPedidosForOC(idOC);

            if (ids == null || ids.Count == 0)
                throw new Exception("Nenhum pedido foi encontrado.");

            return string.Join(",", ids.Select(p => p.ToString()).ToArray());
        }

        #endregion

        #region Finalizar OC

        static volatile object _finalizarOcLock = new object();

        /// <summary>
        /// Finaliza uma OC
        /// </summary>
        public void FinalizarOC(uint idCliente, uint idLoja, Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, uint idRota,
            string dtEntPedidoIni, string dtEntPedidoFin, string pedidos)
        {
            lock(_finalizarOcLock)
            {
                var lstErros = new List<string>();

                using (var trans = new GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        if (string.IsNullOrEmpty(pedidos))
                            return;

                        //Valida a finalização
                        ValidaFinalizarOC(trans, idCliente, idLoja, tipoOC, idRota, dtEntPedidoIni, dtEntPedidoFin, pedidos);

                        var clienteExportadoPedido = new List<KeyValuePair<uint?, string>>();

                        /* Chamado 22260 e 23078.
                         * Devem ser retornados os pedidos agrupados somente se a empresa controlar os pedidos importados na ordem de carga. */
                        if (OrdemCargaConfig.ControlarPedidosImportados)
                            clienteExportadoPedido = PedidoDAO.Instance.ObtemPedidosImportadosAgrupado(trans, pedidos).ToList();
                        else
                            clienteExportadoPedido.Add(new KeyValuePair<uint?, string>(0, pedidos));

                        foreach (var cep in clienteExportadoPedido)
                        {
                            //Cria a OC
                            var idOrdemCarga = Glass.Data.DAL.OrdemCargaDAO.Instance
                                .Insert(trans, new Glass.Data.Model.OrdemCarga(idCliente, idLoja, idRota,
                                    DateTime.Parse(dtEntPedidoIni), DateTime.Parse(dtEntPedidoFin), tipoOC));

                            var idsPedidos = cep.Value.Split(',').Select(p => Glass.Conversoes.StrParaUint(p)).ToList();

                            var pedidosAdicionados = false;

                            //Adiciona os pedidos na OC
                            foreach (var idPedido in idsPedidos)
                            {
                                if (idPedido == 0)
                                    continue;

                                try
                                {
                                    //Verifica se o pedido informado ja possui uma ordem de carga que não seja parcial
                                    if (PedidoOrdemCargaDAO.Instance.VerificarSePedidoPossuiOrdemCarga(trans, tipoOC, idPedido))
                                        throw new Exception("O pedido " + idPedido + " já esta vinculado a uma ordem de carga");

                                    //Adiciona o pedido a OC.
                                    PedidoOrdemCargaDAO.Instance.Insert(trans, new Glass.Data.Model.PedidoOrdemCarga(idPedido, idOrdemCarga));

                                    pedidosAdicionados = true;
                                }
                                catch (Exception ex)
                                {
                                    lstErros.Add(ex.Message);
                                }
                            }

                            //Marca a OC como finalizada
                            if (pedidosAdicionados)
                                OrdemCargaDAO.Instance.FinalizarOC(trans, idOrdemCarga);
                            else
                                OrdemCargaDAO.Instance.DeleteByPrimaryKey(trans, idOrdemCarga);
                        }

                        trans.Commit();
                        trans.Close();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("Finaliza OC - Ids Pedidos: {0}", pedidos), ex);

                        throw;
                    }

                    if (lstErros.Count > 0)
                        throw new Exception(string.Join(", ", lstErros));
                }
            }
        }

        /// <summary>
        /// Valida a finalização de uma OC
        /// </summary>
        public void ValidaFinalizarOC(GDASession sessao, uint idCliente, uint idLoja, Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, uint idRota,
            string dtEntPedidoIni, string dtEntPedidoFin, string idsPedidos)
        {
            if (idCliente == 0)
                throw new Exception("Cliente não infomado.");

            var sitCliente = ClienteDAO.Instance.GetSituacao(sessao, idCliente);

            if (OrdemCargaConfig.SituacoesClienteNaoGerarOC.Contains(sitCliente))
                throw new Exception("O cliente " + idCliente + " esta " + Enum.GetName(typeof(Glass.Data.Model.SituacaoCliente), sitCliente) + ".");            

            if (idLoja == 0)
                throw new Exception("Loja não infomada.");

            if (idRota == 0)
                throw new Exception("Rota não infomada.");

            if (tipoOC == 0)
                throw new Exception("Tipo da OC não infomado.");

            if (string.IsNullOrEmpty(dtEntPedidoIni) && string.IsNullOrEmpty(dtEntPedidoFin))
                throw new Exception("Data de Entrega dos pedidos não informada");

            if (string.IsNullOrEmpty(idsPedidos))
                throw new Exception("Nenhum pedido foi informado.");

            foreach (var idPedido in idsPedidos.Split(','))
            {
                var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(sessao, Glass.Conversoes.StrParaUint(idPedido));

                if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && PCPConfig.HabilitarFaturamentoCarregamento)
                    throw new Exception("Não é possível finalizar OC com pedidos a vista se a configuração de efetuar faturamento do carregamento estiver ativa.");

                if (!PedidoDAO.Instance.GerouTodosVolumes(sessao, Glass.Conversoes.StrParaUint(idPedido)))
                    throw new Exception("O pedido " + idPedido + " não teve todos os volumes gerados.");
            }
        }

        #endregion

        #region Gerar OC automaticamente

        /// <summary>
        /// Gera a OCs automaticamente
        /// </summary>
        public void GerarOCs(string idsRotas, uint idCli, string nomeCli, string dtEntPedIni, string dtEntPedFin, uint idLoja,
            Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, List<uint> idsCliIgnorarBloqueio, bool pedidosObs, uint idPedido,
            string codRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao)
        {
            if ((string.IsNullOrEmpty(idsRotas) && idPedido == 0 && string.IsNullOrEmpty(codRotasExternas)) || idLoja == 0 || (string.IsNullOrEmpty(dtEntPedIni) && string.IsNullOrEmpty(dtEntPedFin)))
                return;

            if (idPedido > 0)
            {
                idCli = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
                nomeCli = "";

                var idRotaCli = ClienteDAO.Instance.ObtemIdRota(idCli);

                if (!string.IsNullOrEmpty(idsRotas) && !("," + idsRotas + ",").Contains("" + idRotaCli + ","))
                {
                    idsRotas += "," + idRotaCli;
                }
                else if (string.IsNullOrEmpty(idsRotas))
                {
                    idsRotas = idRotaCli.ToString();
                }
            }

            //Buscar os ids dos pedidos
            var pedidos = PedidoDAO.Instance.GetIdsPedidosForOC(tipoOC, idCli, nomeCli, 0, idsRotas, idLoja,
                dtEntPedIni, dtEntPedFin, false, pedidosObs, codRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao);

            //Cria um dicionario(idCli, pedidos) para guardar os pedidos dos cliente.
            var lstPedidos = new Dictionary<uint, KeyValuePair<uint, string>>();

            //Adiciona os pedidos ao dicionario
            if (pedidos != null && pedidos.Count > 0)
            {
                lstPedidos = pedidos.ToDictionary(p => Glass.Conversoes.StrParaUint(p.Split(';')[0]),
                    p => new KeyValuePair<uint, string>(Glass.Conversoes.StrParaUint(p.Split(';')[1]), p.Split(';')[2]));
            }

            //Se não houver pedidos retorna.
            if (lstPedidos.Count == 0)
                return;

            //Cria a listagemOrdemCarga
            var lstOC = new List<Entidade.ListagemOrdemCarga>();

            //Preenche a listagemOrdemCarga
            foreach (var item in lstPedidos)
                lstOC.Add(new Entidade.ListagemOrdemCarga(item, idLoja, dtEntPedIni, dtEntPedFin, tipoOC, pedidosObs, codRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao));

            //Lista dos erros ocorridos na geração das ocs.
            var lstErros = new List<string>();

            foreach (var oc in lstOC)
            {
                //Se for oc de transferência e o cliente estiver na lista de bloqueios
                //não verifica se o pedido tem obs
                if (!(tipoOC == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia &&
                    idsCliIgnorarBloqueio.Where(f => f == oc.IdCliente).Count() > 0))
                {
                    //Verifica se o pedido tem obs.
                    if (oc.Pedidos.Where(p => !string.IsNullOrEmpty(p.ObsLiberacao)).Count() > 0)
                        continue;
                }

                //Verifica se o pedido gerou todos os volumes
                if (oc.Pedidos.Where(p => !p.GerouTodosVolumes).Count() > 0)
                    continue;

                try
                {
                    //Gera oc dos pedidos de venda e revenda
                    FinalizarOC(oc.IdCliente, oc.IdLoja, oc.TipoOC, oc.IdRota, oc.DtEntPedIni, oc.DtEntPedFin, string.Join(",", oc.Pedidos
                    .Where(p => (p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao) &&
                        p.TipoVenda != (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição &&
                        p.TipoVenda != (int)Glass.Data.Model.Pedido.TipoVendaPedido.Garantia)
                    .Select(p => p.IdPedido.ToString()).ToArray()));
                }
                catch (Exception ex)
                {
                    lstErros.Add(ex.Message);
                }

                try
                {
                    //Gera oc dos pedidos de Reposição
                    FinalizarOC(oc.IdCliente, oc.IdLoja, oc.TipoOC, oc.IdRota, oc.DtEntPedIni, oc.DtEntPedFin, string.Join(",", oc.Pedidos
                    .Where(p => (p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao) &&
                        p.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição)
                    .Select(p => p.IdPedido.ToString()).ToArray()));
                }
                catch (Exception ex)
                {
                    lstErros.Add(ex.Message);
                }

                try
                {
                    //Gera oc dos pedidos de Garantia
                    FinalizarOC(oc.IdCliente, oc.IdLoja, oc.TipoOC, oc.IdRota, oc.DtEntPedIni, oc.DtEntPedFin, string.Join(",", oc.Pedidos
                    .Where(p => (p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda ||
                        p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao) &&
                        p.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Garantia)
                    .Select(p => p.IdPedido.ToString()).ToArray()));
                }
                catch (Exception ex)
                {
                    lstErros.Add(ex.Message);
                }

                try
                {
                    //Gera oc dos pedidos de mão-de-obra
                    FinalizarOC(oc.IdCliente, oc.IdLoja, oc.TipoOC, oc.IdRota, oc.DtEntPedIni, oc.DtEntPedFin, string.Join(",", oc.Pedidos
                    .Where(p => p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra)
                    .Select(p => p.IdPedido.ToString()).ToArray()));
                }
                catch (Exception ex)
                {
                    lstErros.Add(ex.Message);
                }

                try
                {
                    //Gera oc dos pedidos de mão-de-obra especial
                    FinalizarOC(oc.IdCliente, oc.IdLoja, oc.TipoOC, oc.IdRota, oc.DtEntPedIni, oc.DtEntPedFin, string.Join(",", oc.Pedidos
                    .Where(p => p.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                    .Select(p => p.IdPedido.ToString()).ToArray()));
                }
                catch (Exception ex)
                {
                    lstErros.Add(ex.Message);
                }
            }

            if (lstErros.Count > 0)
                throw new Exception("Alguns pedidos não foram adicionados. " + string.Join(", ", lstErros));
        }

        #endregion

        #region Alterações OC

        #region Retira OC carregamento
        
        /// <summary>
        /// Retira uma OC do carregamento
        /// </summary>
        public void RetiraOcCarregamento(uint idCarregamento, uint idOC)
        {
            using (var trans = new GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    if (idOC == 0)
                        throw new Exception("Nenhuma ordem de carga informada.");

                    OrdemCargaDAO.Instance.ForcarTransacaoOC(trans, idOC, true);

                    if (OrdemCargaDAO.Instance.PossuiPecaCarregada(trans, idOC))
                        throw new Exception("Não é possível remover essa OC do Carregamento, pois a mesma possui itens que já foram carregados.");

                    if (CarregamentoDAO.Instance.ObtemQtdeOCsCarregamento(trans, OrdemCargaDAO.Instance.GetIdCarregamento(trans, idOC).GetValueOrDefault(0)) == 1)
                        throw new Exception("O Carregamento só possiu esta OC, não é possível removê-la.");

                    var oc = OrdemCargaDAO.Instance.GetElementByPrimaryKey(trans, idOC);

                    // Se for OC de transferência é ja tiver gerado uma OC de venda, tem que deletar 
                    // a de venda primeiro.
                    if (oc.TipoOrdemCarga == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia)
                    {
                        if (OrdemCargaDAO.Instance.GetIdsPedidosOC(trans, idOC, Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda).Any())
                            throw new Exception("Não é possível remover esta OC do carregamento, pois a mesma possui Pedidos vinculados a uma OC de venda.");

                        foreach (var idPedido in PedidoDAO.Instance.GetIdsPedidosByOCs(trans, idOC.ToString()))
                        {
                            var pedidosNfs = PedidosNotaFiscalDAO.Instance.GetByPedido(trans, idPedido);
                            foreach (var pedidoNf in pedidosNfs)
                            {
                                var situacao = NotaFiscalDAO.Instance.ObtemSituacao(trans, pedidoNf.IdNf);

                                if (situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Cancelada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Denegada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Inutilizada)
                                    throw new Exception("Não é possível remover esta OC do Carregamento, pois ela possui Pedidos vinculados a uma nota fiscal.");
                            }
                        }
                    }

                    //Remove o vinculo com o carregamento
                    OrdemCargaDAO.Instance.DesvinculaOCsCarregamento(trans, idOC.ToString());

                    //Apaga os itens do carregamento
                    ItemCarregamentoDAO.Instance.DeleteByOC(trans, (int)idOC);

                    CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento, null);

                    var situacaoOc = OrdemCargaDAO.Instance.GetSituacao(trans, idOC);

                    /* Chamado 39159. */
                    if (situacaoOc != Glass.Data.Model.OrdemCarga.SituacaoOCEnum.Finalizado)
                        throw new Exception("Falha ao remover OC do carregamento. Não foi possível atualizar a situação da Ordem de Carga.");

                    // Insere o carregamento da ordem de carga no log de alterações.
                    LogAlteracaoDAO.Instance.LogOrdemCarga(trans, (int)idOC, string.Format("Removida do carregamento: {0}", idCarregamento));

                    //Registra o log de remoção
                    LogCancelamentoDAO.Instance.LogOrdemCarga(trans, oc, string.Format("Remoção da OC: {0}", idOC), true);

                    OrdemCargaDAO.Instance.ForcarTransacaoOC(trans, idOC, false);

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("Retira OC Carregamento - Id Carregamento: {0} Id OC: {1}",
                        idCarregamento, idOC), ex);

                    throw ex;
                }
            }
        }

        #endregion

        #region Adicionar Pedidos

        /// <summary>
        /// Verifica se pode ser adicionado um pedido na oc finalizada
        /// </summary>
        public bool PodeAdicionarPedido(uint idOC)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var tipoOC = OrdemCargaDAO.Instance.GetTipoOrdemCarga(transaction, idOC);
                    if (tipoOC == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia)
                    {
                        foreach (var idPedido in PedidoDAO.Instance.GetIdsPedidosByOCs(transaction, idOC.ToString()))
                        {
                            var pedidosNfs = PedidosNotaFiscalDAO.Instance.GetByPedido(transaction, idPedido);
                            foreach (var pedidoNf in pedidosNfs)
                            {
                                var situacao = NotaFiscalDAO.Instance.ObtemSituacao(transaction, pedidoNf.IdNf);

                                if (situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Cancelada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Denegada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Inutilizada)
                                    throw new Exception("Não é possível adicionar um Pedido nesta OC, pois ela já possui Pedidos vinculados a uma nota fiscal.");
                            }
                        }

                        if (OrdemCargaDAO.Instance.GetIdsPedidosOC(transaction, idOC, Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda).Count > 0)
                            throw new Exception("Não é possível adicionar um Pedido nesta OC, pois a mesma possui Pedidos vinculados a uma OC de venda.");
                    }
                    else
                    {
                        foreach (var idPedido in PedidoDAO.Instance.GetIdsPedidosByOCs(transaction, idOC.ToString()))
                            if (LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(transaction, idPedido).Count > 0)
                                throw new Exception("Não é possível adicionar um Pedido nesta OC, pois ela possui Pedidos Liberados.");
                    }

                    transaction.Commit();
                    transaction.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("Delete - OrdemCarga: {0}", idOC), ex);

                    throw;
                }
            }
        }
        
        /// <summary>
        /// Adiciona pedidos a uma oc que já foi finalizada
        /// </summary>
        public void AdicionarPedidosOC(uint idOC, string pedidos, Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC)
        {
            using (var trans = new GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    //valida a inclusão
                    ValidaAdicionarPedidos(trans, idOC, pedidos);

                    var idCarregamento = OrdemCargaDAO.Instance.GetIdCarregamento(trans, idOC);

                    //if (idCarregamento.GetValueOrDefault(0) == 0)
                    //    throw new Exception("Carregamento não encontrado.");

                    var idsPedidos = pedidos.Split(',').Select(p => Glass.Conversoes.StrParaUint(p)).ToList();

                    /* Chamado 32031. */
                    if (string.IsNullOrEmpty(pedidos) ||
                        idsPedidos == null ||
                        idsPedidos.Count == 0 ||
                        idsPedidos.Count(f => f == 0) > 0)
                        throw new Exception("Informe os pedidos que serão adicionados ao carregamento.");

                    //Adiciona os pedidos na OC
                    foreach (var idPedido in idsPedidos)
                    {
                        if (idPedido == 0)
                            continue;

                        // Verifica se o pedido informado ja possui uma ordem de carga que não seja parcial
                        if(PedidoOrdemCargaDAO.Instance.VerificarSePedidoPossuiOrdemCarga(trans, tipoOC, idPedido))
                            throw new Exception("O pedido " + idPedido + " já esta vinculado a uma ordem de carga");


                        //Adiciona o pedido a OC.
                        PedidoOrdemCargaDAO.Instance.Insert(trans, new Glass.Data.Model.PedidoOrdemCarga()
                        {
                            IdPedido = idPedido,
                            IdOrdemCarga = idOC
                        });
                    }

                    if (idCarregamento.GetValueOrDefault(0) > 0)
                    {
                        ItemCarregamentoDAO.Instance.CriaItensCarregamento(trans, idCarregamento.Value, null, pedidos);
                        CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento.Value, null);
                    }

                    LogAlteracaoDAO.Instance.LogOrdemCarga(trans, (int)idOC, string.Format("Pedidos adicionados: {0}", pedidos));

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("Adiciona Pedidos OC - Id OC: {0} Ids Pedidos: {1}",
                        idOC, pedidos), ex);

                    throw;
                }
            }
        }

        /// <summary>
        /// Valida inclusão de pedidos a uma oc finalizada
        /// </summary>
        public void ValidaAdicionarPedidos(GDASession sessao, uint idOC, string idsPedidos)
        {
            if (!OrdemCargaDAO.Instance.Exists(sessao, idOC))
                throw new Exception("OC não foi encontrada.");

            foreach (var idPedido in idsPedidos.Split(','))
            {
                var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(sessao, Glass.Conversoes.StrParaUint(idPedido));

                if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && PCPConfig.HabilitarFaturamentoCarregamento)
                    throw new Exception("Não é possível finalizar OC com pedidos a vista se a configuração de efetuar faturamento do carregamento estiver ativa.");

                if (!PedidoDAO.Instance.GerouTodosVolumes(sessao, Glass.Conversoes.StrParaUint(idPedido)))
                    throw new Exception("O pedido " + idPedido + " não teve todos os volumes gerados.");

                if (ProdutoPedidoProducaoDAO.Instance.VerificaPecaCanceladaSemNovaImpressao(sessao, Glass.Conversoes.StrParaUint(idPedido)))
                    throw new Exception("O pedido " + idPedido + " possui peças que foram canceladas e ainda estão sem impressão.");

                if (OrdemCargaConfig.BloquearInclusaoPedidoComNotaGerada &&
                    NotaFiscalDAO.Instance.ExistsByPedido(sessao, Glass.Conversoes.StrParaUint(idPedido)))
                    throw new Exception("O pedido " + idPedido + " ja teve uma nota fiscal gerada.");
            }

            if (("," + PedidoOrdemCargaDAO.Instance.GetIdsOCsByPedidos(sessao, idsPedidos) + ",").Contains("," + idOC + ","))
                throw new Exception("Um ou mais pedidos já foram adicionados na OC " + idOC);
        }

        #endregion

        #endregion

        #region Deletar OC

        static volatile object _apagarOcLock = new object();

        /// <summary>
        /// Deleta uma OC
        /// </summary>
        public void Delete(Glass.Data.Model.OrdemCarga objDelete)
        {
            lock(_apagarOcLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (objDelete.IdOrdemCarga == 0)
                            throw new Exception("Nenhuma ordem de carga informada.");

                        var situacaoOC = OrdemCargaDAO.Instance.GetSituacao(transaction, objDelete.IdOrdemCarga);

                        //Valida se esta oc ja foi carregada, se sim não pode deletar
                        if (situacaoOC == Glass.Data.Model.OrdemCarga.SituacaoOCEnum.PendenteCarregamento ||
                            situacaoOC == Glass.Data.Model.OrdemCarga.SituacaoOCEnum.Carregado)
                            throw new Exception("A OC " + objDelete.IdOrdemCarga + " já esta vinculada a um carregamento.");

                        //Se for OC de transferência é ja tiver gerado uma OC de venda, tem que deletar 
                        //a de venda primeiro.
                        var tipoOC = OrdemCargaDAO.Instance.GetTipoOrdemCarga(transaction, objDelete.IdOrdemCarga);
                        if (tipoOC == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia)
                        {
                            var idsPedidos = OrdemCargaDAO.Instance.GetIdsPedidosOC(transaction, objDelete.IdOrdemCarga, Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda);
                            if (idsPedidos.Count > 0)
                                throw new Exception("Falha ao excluir OC os pedidos: " + string.Join(", ", idsPedidos.Select(p => p.ToString()).ToArray())
                                    + " estão vinculados a uma OC de venda.");
                        }
                        else
                        {
                            var idsPedidos = PedidoDAO.Instance.GetIdsPedidosByOCs(transaction, objDelete.IdOrdemCarga.ToString());
                            foreach (var idPedido in idsPedidos)
                            {
                                if (OrdemCargaConfig.UsarOrdemCargaParcial && PedidoDAO.Instance.ObtemOrdemCargaParcial(transaction, idPedido))
                                {
                                    var qtdOrdemCarga = PedidoOrdemCargaDAO.Instance.ObtemQtdeOrdemCarga(transaction, idPedido);
                                    var qtdLib = LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(transaction, idPedido).Count;

                                    if (qtdLib > 0 &&  qtdLib == qtdOrdemCarga)
                                        throw new Exception("Não é possível excluir esta OC, pois ela possui Pedidos liberados.");
                                }
                                else if (LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(transaction, idPedido).Count > 0)
                                    throw new Exception("Não é possível excluir esta OC, pois ela possui Pedidos liberados.");
                            }
                        }

                        //Deleta os itens da ordem de carga.
                        PedidoOrdemCargaDAO.Instance.DeleteByOrdemCarga(transaction, objDelete.IdOrdemCarga);

                        //Deleta a OC
                        OrdemCargaDAO.Instance.DeleteByPrimaryKey(transaction, objDelete.IdOrdemCarga);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("Delete - OrdemCarga: {0}", objDelete.IdOrdemCarga), ex);

                        throw;
                    }
                }
            }
        }

        #endregion

        #region Ajax

        private static Ajax.IOrdemCarga _ajax = null;

        public static Ajax.IOrdemCarga Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.OrdemCarga();

                return _ajax;
            }
        }

        #endregion
    }
}
