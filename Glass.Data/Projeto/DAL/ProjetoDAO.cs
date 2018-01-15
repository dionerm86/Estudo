using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Global;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class ProjetoDAO : BaseDAO<Projeto, ProjetoDAO>
	{
        //private ProjetoDAO() { }

        #region Listagem Padr�o

        private string Sql(uint idProjeto, uint idCliente, string nomeCliente, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "p.*, c.Revenda as CliRevenda, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja" : "Count(*)";

            StringBuilder str = new StringBuilder();
            str.Append("Select " + campos + @" From projeto p
                Left Join cliente c On (p.idCliente=c.id_Cli)
                Left Join funcionario f On (p.IdFunc=f.IdFunc)
                Left Join loja l On (p.IdLoja = l.IdLoja) Where 1");

            if (idProjeto > 0)
                str.Append(" And p.idProjeto=" + idProjeto);
            else if (idCliente > 0)
                str.Append(" And p.idCliente=" + idCliente);
            else if (!String.IsNullOrEmpty(nomeCliente))
                str.Append(" And p.NomeCliente Like ?nomeCliente");

            if (!String.IsNullOrEmpty(dataIni))
                str.Append(" And p.DataCad>?dataIni");

            if (!String.IsNullOrEmpty(dataFim))
                str.Append(" And p.DataCad<?dataFim");

            return str.ToString();
        }

        public Projeto GetElement(uint idProjeto)
        {
            return objPersistence.LoadOneData(Sql(idProjeto, 0, null, null, null, true), "");
        }

        public IList<Projeto> GetList(uint idProjeto, uint idCliente, string nomeCliente, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "p.DataCad Desc" : sortExpression;

            return LoadDataWithSortExpression(Sql(idProjeto, idCliente, nomeCliente, dataIni, dataFim, true), filtro, startRow, pageSize, GetParam(nomeCliente, dataIni, dataFim));
        }

        public int GetCount(uint idProjeto, uint idCliente, string nomeCliente, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idProjeto, idCliente, nomeCliente, dataIni, dataFim, false), GetParam(nomeCliente, dataIni, dataFim));
        }

        public GDAParameter[] GetParam(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Projeto> GetListAcessoExterno(uint idProjeto, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0)
                return null;

            return GetList(idProjeto, UserInfo.GetUserInfo.IdCliente.Value, null, dataIni, dataFim, sortExpression, startRow, pageSize);
        }

        public int GetCountAcessoExterno(uint idProjeto, string dataIni, string dataFim)
        {
            if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0)
                return 0;

            return GetCount(idProjeto, UserInfo.GetUserInfo.IdCliente.Value, null, dataIni, dataFim);
        }

        #endregion

        #region Retorna o valor total do projeto

        /// <summary>
        /// Retorna o valor total do projeto
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public Single GetTotalProjeto(uint idProjeto)
        {
            string sql = "Select coalesce(total, 0) From projeto p Where p.idProjeto=" + idProjeto;

            return Single.Parse(objPersistence.ExecuteScalar(sql).ToString().Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
        }

        #endregion

        #region Finaliza Projeto

        /// <summary>
        /// Finaliza o projeto
        /// </summary>
        public void Finaliza(uint idProjeto)
        {
            Finaliza(null, idProjeto);
        }

        /// <summary>
        /// Finaliza o projeto
        /// </summary>
        public void Finaliza(GDASession session, uint idProjeto)
        {
            string sql = "Update projeto set dataFin=now(), situacao=" + (int)Projeto.SituacaoProjeto.Finalizado + " Where idProjeto=" + idProjeto;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Reabrir projeto

        /// <summary>
        /// Reabre um projeto.
        /// </summary>
        public void ReabrirProjeto(uint idProjeto)
        {
            ReabrirProjeto(null, idProjeto);
        }

        /// <summary>
        /// Reabre um projeto.
        /// </summary>
        public void ReabrirProjeto(GDASession session, uint idProjeto)
        {
            string sql = "Update projeto set dataFin=null, situacao=" + (int)Projeto.SituacaoProjeto.Aberto + " Where idProjeto=" + idProjeto;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Gerar Pedido

        /// <summary>
        /// Gera um pedido para o projeto passado, retornando o idPedido
        /// </summary>
        public uint GerarPedido(uint idProjeto, bool apenasVidros, int? tipoEntrega, bool parceiro)
        {
            FilaOperacoes.GerarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Busca o projeto.
                    Projeto projeto = GetElementByPrimaryKey(transaction, idProjeto);

                    #region Valida��es inciais

                    // Se este projeto estiver finalizado, n�o pode gerar pedido
                    if (projeto.Situacao == (int)Projeto.SituacaoProjeto.Finalizado)
                        throw new Exception("Este projeto j� gerou um pedido.");

                    // Verifica se o cliente foi informado
                    if (projeto.IdCliente == null)
                        throw new Exception("Selecione um cliente para o projeto para gerar pedido.");

                    #endregion

                    #region Cria o pedido

                    Pedido pedido = new Pedido();
                    pedido.IdProjeto = idProjeto;
                    pedido.IdLoja = projeto.IdLoja;
                    pedido.IdFunc = projeto.IdFunc;
                    pedido.IdCli = projeto.IdCliente.Value;

                    var rota = RotaDAO.Instance.GetByCliente(transaction, projeto.IdCliente.Value);

                    pedido.TipoEntrega =
                        rota != null && rota.EntregaBalcao ? (int)Pedido.TipoEntregaPedido.Balcao :
                        tipoEntrega != null ? tipoEntrega : (int)PedidoConfig.TipoEntregaPadraoPedido;

                    pedido.Situacao = Pedido.SituacaoPedido.Ativo;
                    pedido.DataPedido = DateTime.Now;
                    pedido.AliquotaIcms = 0.12f;
                    pedido.CustoPedido = projeto.CustoTotal;
                    pedido.CodCliente = projeto.PedCli;
                    pedido.Obs = projeto.Obs;
                    pedido.TipoPedido = projeto.TipoVenda;
                    pedido.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;
                    pedido.FastDelivery = projeto.FastDelivery;

                    if (parceiro)
                    {
                        Parcelas parc = ParcelasDAO.Instance.GetPadraoCliente(transaction, pedido.IdCli);
                        if (parc != null && parc.TipoPagto >= 1)
                            pedido.TipoVenda = (int)Pedido.TipoVendaPedido.APrazo;
                    }

                    uint idPedido = PedidoDAO.Instance.Insert(transaction, pedido);

                    if (idPedido == 0)
                        throw new Exception("Inser��o do pedido retornou 0.");

                    #endregion

                    #region Insere produtos de pedido a partir dos materiais de projeto

                    // Busca os itens do projeto
                    var lstItemProjeto = ItemProjetoDAO.Instance.GetForPedido(transaction, idProjeto);

                    var prodPed = new ProdutosPedido();
                    var lstProdPed = new List<ProdutosPedido>();
                    var possuiVidro = false;
                    var revendaParceiro = (projeto.TipoVenda == 2 && parceiro);

                    // Para cada item de projeto cadastra seus materiais como produtos do pedido gerado
                    foreach (ItemProjeto ip in lstItemProjeto)
                    {
                        uint idItemProjeto = 0;

                        // Se o produto for um c�lculo de projeto, faz uma c�pia para o pedido
                        if (revendaParceiro)
                            idItemProjeto = ip.IdItemProjeto;
                        else
                            idItemProjeto = PedidoDAO.Instance.ClonaItemProjeto(transaction, ip.IdItemProjeto, idPedido);

                        // Gera ambientes, independente de a empresa usar ou n�o ambientes.
                        AmbientePedido ambiente = new AmbientePedido();
                        ambiente.IdPedido = idPedido;
                        ambiente.Ambiente = ip.Ambiente;

                        string descricao = UtilsProjeto.FormataTextoOrcamento(transaction, ip);
                        if (!String.IsNullOrEmpty(descricao))
                            ambiente.Descricao = descricao;

                        if (revendaParceiro)
                            ambiente.Ambiente = "Revenda";
                        else
                            ambiente.IdItemProjeto = idItemProjeto;

                        uint idAmbientePedido = AmbientePedidoDAO.Instance.Insert(transaction, ambiente);

                        // Busca todos os materiais deste itemProjeto
                        var lstMaterialItem = MaterialItemProjetoDAO.Instance.GetByItemProjeto(transaction, idItemProjeto);

                        // Para cada item_projeto busca-se os materiais do mesmo para inserir nos produtos do pedido
                        foreach (MaterialItemProjeto material in lstMaterialItem)
                        {
                            if (apenasVidros && !GrupoProdDAO.Instance.IsVidro((int)material.IdGrupoProd))
                                continue;

                            if (parceiro && !possuiVidro && GrupoProdDAO.Instance.IsVidro((int)material.IdGrupoProd))
                                possuiVidro = true;

                            prodPed = new ProdutosPedido();
                            prodPed.IdPedido = idPedido;
                            prodPed.IdProd = material.IdProd;
                            prodPed.IdAmbientePedido = idAmbientePedido;
                            prodPed.IdProcesso = material.IdProcesso;
                            prodPed.IdAplicacao = material.IdAplicacao;
                            prodPed.Altura = material.IdPecaItemProj > 0 || revendaParceiro ? material.Altura : material.AlturaCalc;
                            prodPed.AlturaReal = material.IdPecaItemProj > 0 || revendaParceiro ? material.AlturaCalc : material.Altura;
                            prodPed.Largura = material.Largura;
                            prodPed.TotM = material.TotM;
                            prodPed.TotM2Calc = material.TotM2Calc;
                            prodPed.Qtde = material.Qtde;
                            prodPed.Total = material.Total;
                            prodPed.CustoProd = material.Custo;
                            prodPed.AliqIcms = material.AliqIcms;
                            prodPed.ValorIcms = prodPed.Total * (decimal)(prodPed.AliqIcms / 100);
                            prodPed.PedCli = material.PedCli;
                            prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)material.IdProd, projeto.TipoEntrega, projeto.IdCliente, false, false, 0);
                            prodPed.ValorVendido = material.Valor;
                            prodPed.ValorDescontoCliente = material.ValorDescontoCliente;
                            prodPed.ValorAcrescimoCliente = material.ValorAcrescimoCliente;
                            prodPed.ValorUnitarioBruto = material.ValorUnitarioBruto;
                            prodPed.TotalBruto = material.TotalBruto;
                            prodPed.Redondo = material.Redondo;

                            if (!revendaParceiro)
                            {
                                prodPed.IdMaterItemProj = material.IdMaterItemProjOrig;
                                prodPed.IdItemProjeto = idItemProjeto;
                                prodPed.IdMaterItemProj = material.IdMaterItemProj;
                            }

                            uint idProdPed = ProdutosPedidoDAO.Instance.InsertFromProjeto(transaction, prodPed);

                            foreach (MaterialProjetoBenef benef in material.Beneficiamentos.ToMateriaisProjeto())
                            {
                                ProdutoPedidoBenef ppb = new ProdutoPedidoBenef();
                                ppb.BisAlt = benef.BisAlt;
                                ppb.BisLarg = benef.BisLarg;
                                ppb.EspBisote = benef.EspBisote;
                                ppb.EspFuro = benef.EspFuro;
                                ppb.IdBenefConfig = benef.IdBenefConfig;
                                ppb.IdProdPed = idProdPed;
                                ppb.LapAlt = benef.LapAlt;
                                ppb.LapLarg = benef.LapLarg;
                                ppb.Qtd = benef.Qtd;
                                ppb.Valor = benef.Valor;
                                ppb.Custo = benef.Custo;

                                ProdutoPedidoBenefDAO.Instance.Insert(transaction, ppb);
                            }

                            ProdutosPedidoDAO.Instance.UpdateValorBenef(transaction, idProdPed);
                        }
                    }

                    #endregion

                    #region Insere produtos de pedido a partir dos alum�nios

                    var lstAluminio = new Dictionary<uint, KeyValuePair<uint?, MaterialItemProjeto>>();

                    // Insere os itens do alum�nio no projeto, se eles tiverem sido agrupados
                    foreach (uint key in lstAluminio.Keys)
                    {
                        uint? idAmbientePedido = lstAluminio[key].Key;
                        MaterialItemProjeto item = lstAluminio[key].Value;

                        // Atualiza a quantidade de itens e a altura
                        int i = 0;
                        while (item.Altura > 6 * i)
                            i++;

                        item.Qtde = i;
                        item.Altura = i > 0 ? 6f : 0f;
                        item.AlturaCalc = item.Altura;

                        prodPed = new ProdutosPedido();
                        prodPed.IdPedido = idPedido;
                        prodPed.IdProd = item.IdProd;
                        prodPed.IdAmbientePedido = idAmbientePedido;
                        prodPed.IdProcesso = item.IdProcesso;
                        prodPed.IdAplicacao = item.IdAplicacao;
                        prodPed.Altura = item.Altura;
                        prodPed.AlturaReal = item.Altura;
                        prodPed.Largura = item.Largura;
                        prodPed.TotM = item.TotM;
                        prodPed.TotM2Calc = item.TotM2Calc;
                        prodPed.Qtde = item.Qtde;
                        prodPed.ValorVendido = item.Valor;
                        prodPed.Total = item.Valor * (decimal)item.Qtde;
                        prodPed.AliqIcms = item.AliqIcms;
                        prodPed.ValorIcms = prodPed.Total * ((decimal)prodPed.AliqIcms / 100);
                        prodPed.PedCli = item.PedCli;
                        prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)item.IdProd, projeto.TipoEntrega, projeto.IdCliente, false, false, 0);
                        prodPed.ValorDescontoCliente = item.ValorDescontoCliente;
                        prodPed.ValorAcrescimoCliente = item.ValorAcrescimoCliente;
                        prodPed.ValorUnitarioBruto = item.ValorUnitarioBruto;
                        prodPed.TotalBruto = item.TotalBruto;

                        if (!revendaParceiro)
                        {
                            prodPed.IdItemProjeto = item.IdItemProjeto;
                            prodPed.IdMaterItemProj = item.IdMaterItemProj;
                        }

                        uint idProdPed = ProdutosPedidoDAO.Instance.InsertFromProjeto(transaction, prodPed);

                        foreach (MaterialProjetoBenef benef in item.Beneficiamentos.ToMateriaisProjeto())
                        {
                            ProdutoPedidoBenef ppb = new ProdutoPedidoBenef();
                            ppb.BisAlt = benef.BisAlt;
                            ppb.BisLarg = benef.BisLarg;
                            ppb.EspBisote = benef.EspBisote;
                            ppb.EspFuro = benef.EspFuro;
                            ppb.IdBenefConfig = benef.IdBenefConfig;
                            ppb.IdProdPed = idProdPed;
                            ppb.LapAlt = benef.LapAlt;
                            ppb.LapLarg = benef.LapLarg;
                            ppb.Qtd = benef.Qtd;
                            ppb.Valor = benef.Valor;
                            ppb.Custo = benef.Custo;

                            ProdutoPedidoBenefDAO.Instance.Insert(transaction, ppb);
                        }

                        ProdutosPedidoDAO.Instance.UpdateValorBenef(transaction, idProdPed);
                    }

                    #endregion

                    #region Atualiza o total do pedido e finaliza o projeto

                    // Atualiza os totais deste pedido
                    PedidoDAO.Instance.UpdateTotalPedido(transaction, idPedido);

                    // Finaliza o projeto
                    Finaliza(transaction, idProjeto);

                    #endregion

                    #region Salva as imagens do projeto e arquivo de marca��o

                    var lstImagensPcp = new List<string>();

                    if (!revendaParceiro)
                    { 
                        // Copia as imagens de projeto que podem ter sido criadas no comercial, alterando o idProdPed do produtos_pedido para o
                        // idProdPed de produtos_pedido_espelho, rec�m criado
                        foreach (ItemProjeto ip in ItemProjetoDAO.Instance.GetByPedido(transaction, idPedido))
                        {
                            foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(transaction, ip.IdItemProjeto, false))
                            {
                                // Com base no campo IdMaterItemProjOrig, recupera o produtos_pedido associado ao mesmo, para verificar se 
                                // existe ou n�o alguma figura editada neste produto
                                var idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(transaction, idPedido, mip.IdMaterItemProjOrig.Value);

                                // Pega a pe�a associada � este produtos_pedido_espelho
                                var peca = PecaItemProjetoDAO.Instance.GetByProdPed(transaction, idProdPed);

                                if (peca == null)
                                    continue;

                                foreach (string item in UtilsProjeto.GetItensFromPeca(peca.Item))
                                {
                                    string caminhoImagemProjeto = Utils.GetPecaProjetoPath + peca.IdPecaItemProj.ToString().PadLeft(10, '0') + "_" + item + ".jpg";
                                    string caminhoImagemComercial = Utils.GetPecaComercialPath + idProdPed.ToString().PadLeft(10, '0') + "_" + item + ".jpg";

                                    if (File.Exists(caminhoImagemProjeto))
                                    {
                                        File.Copy(caminhoImagemProjeto, caminhoImagemComercial, true);
                                        lstImagensPcp.Add(caminhoImagemComercial);
                                    }
                                }

                                #region Copia o arquivo DXF do comercial para o pcp, se tiver sido editado

                                var caminhoProjeto = PCPConfig.CaminhoSalvarCadProjectProjeto() + peca.IdPecaItemProj;
                                var caminhoComercial = PCPConfig.CaminhoSalvarCadProject(false) + idProdPed;

                                //Copia o DXF
                                if (File.Exists(caminhoProjeto + ".dxf"))
                                    File.Copy(caminhoProjeto + ".dxf", caminhoComercial + ".dxf");

                                //Copia o SVG
                                if (File.Exists(caminhoProjeto + ".svg"))
                                    File.Copy(caminhoProjeto + ".svg", caminhoComercial + ".svg");

                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region WebGlass Parceiro

                    if (parceiro)
                    {
                        pedido.IdFunc = UserInfo.GetUserInfo.IdCliente > 0 ?
                            ClienteDAO.Instance.ObtemIdFunc(transaction, UserInfo.GetUserInfo.IdCliente.Value).GetValueOrDefault() : 0;
                        pedido.GeradoParceiro = true;

                        #region Define a loja do projeto e do pedido

                        if (pedido.IdLoja == 0)
                        {
                            var idLoja = possuiVidro ? ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoComVidro :
                                ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoSemVidro;

                            pedido.IdLoja = idLoja > 0 ? (uint)idLoja :
                                UserInfo.GetUserInfo.IdLoja > 0 ? UserInfo.GetUserInfo.IdLoja :
                                pedido.IdFunc > 0 ? FuncionarioDAO.Instance.ObtemIdLoja(transaction, pedido.IdFunc) : 0;

                            if (pedido.IdLoja == 0)
                            {
                                if (LojaDAO.Instance.GetCount() == 0)
                                    throw new Exception("Cadastre uma loja no sistema e tente novamente.");

                                pedido.IdLoja = (uint)LojaDAO.Instance.GetAll()[0].IdLoja;
                            }
                        }
                        
                        /* Chamado 48322. */
                        if (projeto.IdLoja == 0 && pedido.IdLoja > 0)
                            AtualizarIdLojaProjeto(transaction, (int)idProjeto, (int)pedido.IdLoja);

                        #endregion

                        PedidoDAO.Instance.GeraParcelaParceiro(transaction, ref pedido);

                        PedidoDAO.Instance.Update(transaction, pedido);

                        // Caso n�o seja permitido editar pedidos gerados pelo WebGlass Parceiros, finaliza o pedido na mesma
                        // transa��o onde ele � gerado, para que, caso ocorra algum problema, o pedido n�o seja inserido.
                        if (!PedidoConfig.PodeEditarPedidoGeradoParceiro)
                        {
                            // Deixa o pedido conferido.
                            bool temp = false;
                            PedidoDAO.Instance.FinalizarPedido(transaction, idPedido, ref temp, false);
                        }

                        /* Chamado 49811. */
                        if (!pedido.DataEntrega.HasValue || !PedidoDAO.Instance.ObtemDataEntrega(transaction, pedido.IdPedido).HasValue ||
                            PedidoDAO.Instance.ObtemDataEntrega(transaction, pedido.IdPedido).Value.Date != pedido.DataEntrega.Value.Date)
                            throw new Exception("N�o foi poss�vel calcular a data de entrega do pedido. Tente gerar o pedido novamente. " +
                                "Caso a mensagem persista, entre em contato com o suporte do software WebGlass.");
                    }

                    #endregion

                    transaction.Commit();
                    transaction.Close();

                    return idPedido;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.GerarPedido.ProximoFila();
                }
            }
        }

        #endregion

        #region Gerar Pedido Parceiro
        
        /// <summary>
        /// Gera um pedido de um parceiro.
        /// </summary>
        public uint GerarPedidoParceiro(uint idProjeto, bool apenasVidros, out Exception erro)
        {
            uint idPedido = 0;
            erro = null;

            Projeto proj = GetElementByPrimaryKey(idProjeto);
            if (String.IsNullOrEmpty(proj.PedCli))
            {
                erro = new Exception("Cadastre o seu c�digo de pedido para continuar.");
                return 0;
            }

            var itemProjeto = ItemProjetoDAO.Instance.GetByProjeto(idProjeto);

            // N�o permite gerar o pedido caso n�o exista item cadastrado
            if (itemProjeto.Count == 0)
            {
                erro = new Exception("N�o existem c�lculos neste or�amento. Inclua pelo menos um para gerar um pedido.");
                return 0;
            }

            foreach (ItemProjeto item in itemProjeto)
            {
                //Verifica se foi informado o ambiente no item
                if(proj.TipoVenda == (int)Pedido.TipoPedidoEnum.Venda && string.IsNullOrWhiteSpace(item.Ambiente))
                {
                    erro = new Exception("Existe pelo menos um projeto sem ambiente, edite e informe o ambiente antes de gerar o pedido.");
                    return 0;
                }

                // N�o permite gerar o pedido se houver algum item de projeto com o valor zerado
                if (MaterialItemProjetoDAO.Instance.GetCount(item.IdItemProjeto) == 0)
                {
                    erro = new Exception("Existe pelo menos um c�lculo sem produtos. Corrija-o(s) para continuar.");
                    return 0;
                }
                else if (item.Total == 0)
                {
                    erro = new Exception("Existe pelo menos um c�lculo sem valor. Corrija-o(s) para continuar.");
                    return 0;
                }
            }

            #region Gera pedido e finaliza

            try
            {
                idPedido = GerarPedido(idProjeto, apenasVidros, proj.TipoEntrega, true);
            }
            catch (Exception ex)
            {
                erro = new Exception("Falha ao gerar o pedido.", ex);
                return 0;
            }

            if (erro != null)
            {
                ErroDAO.Instance.InserirFromException("GerarPedidoParceiro", erro);
                return idPedido;
            }

            #endregion

            #region Confirma pedido, gera PCP e finaliza PCP

            if (ProjetoConfig.TelaCadastroParceiros.ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido)
            {
                try
                {
                    // Deixa o pedido Confirmado.
                    var idsPedidos = string.Empty;
                    var idsPedidosErro = string.Empty;

                    /* Chamado 47326. */
                    if (GetTipoVenda(null, idProjeto) != (uint)Pedido.TipoPedidoEnum.Revenda &&
                        PedidoDAO.Instance.ObtemSituacao(idPedido) != Pedido.SituacaoPedido.ConfirmadoLiberacao)
                        PedidoDAO.Instance.ConfirmarLiberacaoPedidoComTransacao(idPedido.ToString(), out idsPedidos, out idsPedidosErro, true, false);
                }
                catch (Exception ex)
                {
                    erro = new Exception("Falha ao confirmar o pedido.", ex);
                    ErroDAO.Instance.InserirFromException("GerarPedidoParceiro", erro);
                    return idPedido;
                }

                if (ObtemValorCampo<int>("TipoVenda", "IdProjeto=" + idProjeto) == (int)Pedido.TipoPedidoEnum.Venda)
                {
                    try
                    {
                        // Gera o espelho do pedido.
                        PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao(idPedido);
                    }
                    catch (Exception ex)
                    {
                        erro = new Exception("Falha ao gerar a confer�ncia do pedido.", ex);
                        ErroDAO.Instance.InserirFromException("GerarPedidoParceiro", erro);
                        return idPedido;
                    }

                    try
                    {
                        // Deixa a confer�ncia do pedido finalizada.
                        PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao(idPedido);
                    }
                    catch (Exception ex)
                    {
                        erro = new Exception("Falha ao finalizar a confer�ncia do pedido.", ex);
                        ErroDAO.Instance.InserirFromException("GerarPedidoParceiro", erro);
                        return idPedido;
                    }
                }
            }

            #endregion

            if (erro != null)
                ErroDAO.Instance.InserirFromException("GerarPedidoParceiro", erro);
            
            return idPedido;
        }

        #endregion        

        #region Verifica se projeto pode ser editado

        /// <summary>
        /// Verifica se este projeto pode ser editado.
        /// Verifica o cliente tamb�m se for chamado dentro do WebGlassParceiros.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public bool CanBeEdited(uint idProjeto)
        {
            string sql = "Select Count(*) From projeto Where situacao=" + (int)Projeto.SituacaoProjeto.Aberto + 
                " And idProjeto=" + idProjeto;

            if (UserInfo.GetUserInfo.IdCliente > 0)
                sql += " And idCliente=" + UserInfo.GetUserInfo.IdCliente.Value;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion
        
        #region Busca o pedido relacionado ao projeto

        /// <summary>
        /// Busca o pedido relacionado ao projeto passado
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public uint GetIdPedidoByProjeto(uint idProjeto)
        {
            string sql = "Select idPedido From pedido Where idProjeto=" + idProjeto + " and situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            object retorno = objPersistence.ExecuteScalar(sql);

            if (retorno == null || String.IsNullOrEmpty(retorno.ToString()))
                return 0;
            else
                return Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sql).ToString());
        }

        #endregion

        #region Recupera o total do projeto

        /// <summary>
        /// Retorna o total do Projeto
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public decimal GetTotal(GDASession sessao, uint idProjeto)
        {
            return ObtemValorCampo<decimal>(sessao, "total", "idProjeto=" + idProjeto);
        }

        /// <summary>
        /// Retorna o tipo venda do Projeto
        /// </summary>
        public uint GetTipoVenda(GDASession sessao, uint idProjeto)
        {
            return ObtemValorCampo<uint>(sessao, "TipoVenda", "idProjeto=" + idProjeto);
        }
        #endregion

        #region Atualiza total do projeto

        /// <summary>
        /// Atualiza o valor total do projeto, somando os totais dos produtos relacionados � ele
        /// </summary>
        public void UpdateTotalProjeto(uint idProjeto)
        {
            UpdateTotalProjeto(null, idProjeto);
        }

        /// <summary>
        /// Atualiza o valor total do projeto, somando os totais dos produtos relacionados � ele
        /// </summary>
        public void UpdateTotalProjeto(GDASession sessao, uint idProjeto)
        {
            Projeto proj = GetElementByPrimaryKey(sessao, idProjeto);

            string sql = "update projeto p set p.total=" +
                "Round(((Select Sum(Total) From item_projeto ip Where ip.idProjeto=p.idProjeto)), 2), " +
                "p.CustoTotal=Round((select sum(CustoTotal) from item_projeto ip where ip.idProjeto=p.idProjeto), 2) " +
                "Where idProjeto=" + idProjeto;

            objPersistence.ExecuteCommand(sessao, sql);

            // Calcula o valor do ICMS do projeto
            if (PedidoConfig.Impostos.CalcularIcmsPedido)
            {
                var calcIcmsSt = CalculoIcmsStFactory.ObtemInstancia(sessao, (int)proj.IdLoja, (int?)proj.IdCliente, null, null, null);

                string idProd = "mip.idProd";
                string total = "mip.Total + coalesce(mip.ValorBenef, 0)";
                string desconto = null;
                string aliquotaIcmsSt = "mip.AliqIcms";

                sql = @"
                    update material_item_projeto mip
                    set mip.AliqIcms=Round((" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, total, desconto, aliquotaIcmsSt) + @"), 2), 
                        mip.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(total, desconto, aliquotaIcmsSt) + @")
                    where mip.idItemProjeto in (select idItemProjeto from item_projeto where idProjeto=" + idProjeto + ")";

                objPersistence.ExecuteCommand(sessao, sql);

                /*
                sql = "update projeto set AliqIcms=Round((select sum(coalesce(AliqIcms, 0)) from material_item_projeto where idProjeto=" + idProjeto + ") / (select count(*) from material_item_projeto where idItemProjeto in (select idItemProjeto from item_projeto where idProjeto=" + idProjeto + ") and AliqIcms>0), 2) where idProjeto=" + idProjeto;
                objPersistence.ExecuteCommand(sql);
                */

                sql = "update projeto set ValorIcms=Round((select sum(coalesce(ValorIcms, 0)) from material_item_projeto where idItemProjeto in (select idItemProjeto from item_projeto where idProjeto=" + idProjeto + ")), 2), Total=(Total + ValorIcms) where idProjeto=" + idProjeto;
                objPersistence.ExecuteCommand(sessao, sql);
            }
        }

        #endregion

        #region Atualiza o cliente do projeto com o cliente do pedido

        /// <summary>
        /// Atualiza o cliente do projeto com o cliente do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizarClienteByPedido(uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar("select coalesce(idProjeto, 0) from pedido where idPedido=" + idPedido);
            uint idProjeto = retorno != null && retorno.ToString() != String.Empty ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;

            if (idProjeto == 0)
                return;

            objPersistence.ExecuteCommand("update projeto p set idCliente=(select idCli from pedido where idPedido=" + idPedido + "), " +
                "nomeCliente=(select nome from cliente where id_Cli=p.idCliente) where idProjeto=" + idProjeto);
        }

        #endregion

        #region Atualiza a loja do projeto

        /// <summary>
        /// Atualiza a loja do projeto.
        /// </summary>
        public void AtualizarIdLojaProjeto(GDASession session, int idProjeto, int idLoja)
        {
            if (idProjeto == 0)
                throw new Exception("Informe o projeto a ser atualizado.");

            if (idLoja == 0)
                throw new Exception("Informe a loja a ser associada ao projeto.");

            objPersistence.ExecuteCommand(session, "UPDATE projeto SET IdLoja=?idLoja WHERE IdProjeto=?idProjeto",
                new GDAParameter("?idLoja", idLoja), new GDAParameter("?idProjeto", idProjeto));
        }

        #endregion

        #region Obtem dados do projeto

        public int ObtemTipoEntrega(uint idProjeto)
        {
            return ObtemValorCampo<int>("tipoEntrega", "idProjeto=" + idProjeto);
        }

        public uint? ObtemIdCliente(uint idProjeto)
        {
            return ObtemValorCampo<uint?>("idCliente", "idProjeto=" + idProjeto);
        }

        public int ObterIdLoja(GDASession session, int idProjeto)
        {
            return ObtemValorCampo<int>(session, "IdLoja", "IdProjeto=" + idProjeto);
        }

        #endregion

        #region M�todos sobrescritos

        public override uint Insert(Projeto objInsert)
        {
            /* Chamado 45006. */
            if (objInsert.TipoVenda == (int)Pedido.TipoPedidoEnum.Revenda)
            {
                var idProjetoModeloOtr = ProjetoModeloDAO.Instance.ObtemId("OTR01");

                if (idProjetoModeloOtr == 0)
                    throw new Exception("Para inserir um or�amento do tipo Revenda � necess�rio cadastrar o projeto de c�digo OTR01, contate o suporte WebGlass.");
            }

            LoginUsuario login = UserInfo.GetUserInfo;
            objInsert.IdFunc = login.CodUser;

            var idLoja = objInsert.TipoVenda == (int)Model.Pedido.TipoPedidoEnum.Venda ? ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoComVidro : ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoSemVidro;
            //objInsert.IdLoja = idLoja > 0 ? (uint)idLoja.Value : login.IdLoja;
            /* Chamado 48322.
             * Caso a empresa utilize loja por tipo de pedido, os projetos cadastrados no WebGlass Parceiros
             * devem ser gerados sem loja definida, para que a loja seja definida ao inserir o primeiro produto no projeto.
             * (Obs.: produto que esteja associado a um subgrupo e, este, associado a alguma loja).
             * Sen�o, caso a empresa utilize a configura��o de loja por tipo de pedido, somente informa o ID recuperado.
             * Sen�o, salva a loja associada ao login no campo IdLoja do projeto. */
            objInsert.IdLoja = login.IdCliente > 0 && idLoja > 0 ? 0 : idLoja > 0 ? (uint)idLoja.Value : login.IdLoja;

            objInsert.DataCad = DateTime.Now;

            // Se o idCliente tiver sido informado, busca o nome do cliente direto do banco de dados
            if (objInsert.IdCliente > 0)
                objInsert.NomeCliente = ClienteDAO.Instance.GetNome(objInsert.IdCliente.Value);

            // Se o idOrcamento tiver sido informado, verifica se o mesmo existe e est� em aberto
            if (objInsert.IdOrcamento != null && !(OrcamentoDAO.Instance.ExistsOrcamentoEmAberto(objInsert.IdOrcamento)))
                throw new Exception("O n�mero de or�amento passado n�o existe.");

            return base.Insert(objInsert);
        }

        public override int Update(Projeto objUpdate)
        {
            Projeto projetoOld = GetElementByPrimaryKey(objUpdate.IdProjeto);
            objUpdate.IdFunc = projetoOld.IdFunc;
            objUpdate.Situacao = projetoOld.Situacao;
            objUpdate.DataCad = projetoOld.DataCad;

            //Chamado 46180
            var idLoja = objUpdate.TipoVenda == (int)Pedido.TipoPedidoEnum.Venda ? ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoComVidro : ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoSemVidro;
            objUpdate.IdLoja = idLoja > 0 ? (uint)idLoja.Value : projetoOld.IdLoja;

            // Se o idCliente tiver sido informado, busca o nome do cliente direto do banco de dados
            if (objUpdate.IdCliente > 0)
                objUpdate.NomeCliente = ClienteDAO.Instance.GetNome(objUpdate.IdCliente.Value);

            // Se o idOrcamento tiver sido informado, verifica se o mesmo existe
            if (objUpdate.IdOrcamento != null && !(OrcamentoDAO.Instance.ExistsOrcamentoEmAberto(objUpdate.IdOrcamento)))
                throw new Exception("O n�mero de or�amento passado n�o existe.");

            int retorno = base.Update(objUpdate);

            // Atualiza total do projeto tendo em vista que um cliente possa ter sido selecionado e 
            // talvez seja necess�rio calcular a taxa � prazo do mesmo
            UpdateTotalProjeto(objUpdate.IdProjeto);

            return retorno;
        }

        public override int Delete(Projeto objDelete)
        {
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From pedido Where idProjeto=" + objDelete.IdProjeto).ToString()) > 0)
                throw new Exception("Este projeto n�o pode ser exclu�do por haver um pedido relacionado ao mesmo.");

            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From orcamento Where idProjeto=" + objDelete.IdProjeto).ToString()) > 0)
                throw new Exception("Este projeto n�o pode ser exclu�do por haver um ou mais or�amentos relacionados ao mesmo.");

            objPersistence.ExecuteCommand("Delete From material_projeto_benef where idMaterItemProj In (Select idMaterItemProj From material_item_projeto Where idItemProjeto In (Select IdItemProjeto From item_projeto Where idProjeto=" + objDelete.IdProjeto + "))");
            objPersistence.ExecuteCommand("Delete From material_item_projeto where idItemProjeto In (Select IdItemProjeto From item_projeto Where idProjeto=" + objDelete.IdProjeto + ")");
            objPersistence.ExecuteCommand("Delete From peca_item_projeto where idItemProjeto In (Select IdItemProjeto From item_projeto Where idProjeto=" + objDelete.IdProjeto + ")");
            objPersistence.ExecuteCommand("Delete From medida_item_projeto where idItemProjeto In (Select IdItemProjeto From item_projeto Where idProjeto=" + objDelete.IdProjeto + ")");
            objPersistence.ExecuteCommand("Delete From item_projeto where idprojeto=" + objDelete.IdProjeto);

            return base.Delete(objDelete);
        }

        #endregion
    }
}