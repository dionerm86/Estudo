using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using System.IO;
using Glass.Configuracoes;
using System.Linq;
using Glass.Data.Model.Calculos;

namespace Glass.Data.DAL
{
    public sealed class ProdutosOrcamentoDAO : BaseDAO<ProdutosOrcamento, ProdutosOrcamentoDAO>
    {
        //private ProdutosOrcamentoDAO() { }

        private string Sql(uint? idOrca, uint? idAmbiente, bool showChildren, uint? idProd, uint? idProdParent, string idsProdutos, bool temOrdenacao, bool selecionar)
        {
            string sqlProdutoTabela = "if(o.TipoEntrega in (" + (int)Orcamento.TipoEntregaOrcamento.Balcao + ", " + 
                (int)Orcamento.TipoEntregaOrcamento.Entrega + "), p.ValorBalcao, p.ValorObra)";

            string campos = selecionar ? @"po.*, (select Count(*) from produtos_orcamento where idProdParent=po.idProd) as NumChild, 
                p.CodInterno as CodInterno, p.Descricao as DescrProduto, p.IdGrupoProd, p.IdSubgrupoProd, " + sqlProdutoTabela + @" as ValorProdutoTabela,
                um.codigo as unidade, ip.obs as obsProj, o.idCliente, c.Nome as NomeCliente, ep.codInterno as codProcesso, ea.codInterno as codAplicacao" : "Count(*)";

            string orcamento = idOrca != null ? " and po.idOrcamento=" + idOrca.Value : "";
            string ambiente = idAmbiente != null && OrcamentoConfig.AmbienteOrcamento ? " and po.idAmbienteOrca=" + idAmbiente.Value : "";
            string produto = idProd != null ? " and po.idProd=" + idProd.Value : "";
            string produtoParent = idProdParent != null ? " and po.idProdParent=" + idProdParent.Value : "";
            string child = !showChildren ? " and po.idProdParent is null" : "";
            string listaProdutos = !String.IsNullOrEmpty(idsProdutos) ? " and po.idProd in (" + idsProdutos + ")" : "";

            return "Select " + campos + @" From produtos_orcamento po 
                Left Join orcamento o on (po.idOrcamento=o.idOrcamento) 
                Left Join produto p on (po.idProduto=p.idProd) 
                Left Join unidade_medida um on (p.idUnidadeMedida=um.idUnidadeMedida) 
                Left Join item_projeto ip On (po.iditemProjeto=ip.idItemProjeto) 
                Left Join cliente c on(c.Id_Cli=o.idCliente) 
                Left Join etiqueta_processo ep on (po.idProcesso=ep.idProcesso)
                Left Join etiqueta_aplicacao ea on (po.idAplicacao=ea.idAplicacao)
                Where 1 " + orcamento + ambiente + produto + produtoParent + child + listaProdutos +
                    (!temOrdenacao ? " Order By Coalesce(po.idAmbienteOrca, 0), NumSeq, po.IdProd" : "");
        }

        public ProdutosOrcamento GetElement(uint idProd)
        {
            return objPersistence.LoadOneData(Sql(null, null, true, idProd, null, null, false, true));
        }

        private IList<ProdutosOrcamento> GetReport(uint idOrca, bool showChildren, bool addMateriaisProjeto, bool addTextoAmbiente,
            bool incluirBeneficiamentos)
        {
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrca);
            var lstProd = objPersistence.LoadData(Sql(idOrca, null, showChildren, null, null, null, false, true)).ToList();

            if (showChildren && incluirBeneficiamentos)
            {
                var lstTemp = new List<ProdutosOrcamento>();
                foreach (ProdutosOrcamento p in lstProd)
                {
                    if (p.Redondo)
                    {
                        if (!BenefConfigDAO.Instance.CobrarRedondo() && !p.DescrProduto.ToLower().Contains("redondo"))
                            p.DescrProduto += " REDONDO";

                        p.Largura = 0;
                    }

                    lstTemp.Add(p);

                    GenericBenefCollection lstBenef = p.Beneficiamentos;

                    if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                    {
                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (GenericBenef pob in lstBenef)
                        {
                            ProdutosOrcamento benef = new ProdutosOrcamento();
                            benef.IdOrcamento = idOrca;
                            benef.IdAmbienteOrca = p.IdAmbienteOrca;
                            benef.IdItemProjeto = p.IdItemProjeto;
                            benef.Ambiente = p.Ambiente;
                            benef.Qtde = pob.Qtd > 0 ? pob.Qtd : 1;
                            benef.ValorProd = pob.ValorUnit;
                            benef.Total = pob.Valor;
                            benef.ValorBenef = 0;
                            benef.DescrProduto = " " + pob.DescricaoBeneficiamento +
                                Utils.MontaDescrLapBis(pob.BisAlt, pob.BisLarg, pob.LapAlt, pob.LapLarg, pob.EspBisote, null, null, false);
                            // É necessário existir esta propriedade para que, no relatório da Vidraçaria Pestana, possamos identificar
                            // produtos de orçamento que são beneficiamentos e esconder somente os que são ambientes.
                            benef.IsBeneficiamento = true;

                            lstTemp.Add(benef);
                        }
                    }
                    else
                    {
                        if (lstBenef.Count > 0)
                        {
                            ProdutosOrcamento benef = new ProdutosOrcamento();
                            benef.IdOrcamento = idOrca;
                            benef.IdAmbienteOrca = p.IdAmbienteOrca;
                            benef.IdItemProjeto = p.IdItemProjeto;
                            benef.Ambiente = p.Ambiente;
                            benef.Qtde = 0;
                            benef.ValorBenef = 0;
                            benef.IsBeneficiamento = true;

                            // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                            foreach (GenericBenef pob in lstBenef)
                            {
                                benef.ValorProd += pob.ValorUnit;
                                benef.Total = (benef.Total > 0 ? benef.Total.Value : 0) + pob.Valor;
                                string textoQuantidade = (pob.TipoCalculo == TipoCalculoBenef.Quantidade) ? pob.Qtd.ToString() + " " : "";
                                benef.DescrProduto += "; " + textoQuantidade + pob.DescricaoBeneficiamento +
                                    Utils.MontaDescrLapBis(pob.BisAlt, pob.BisLarg, pob.LapAlt, pob.LapLarg, pob.EspBisote, null, null, false);
                            }

                            benef.DescrProduto = " " + benef.DescrProduto.Substring(2);
                            lstTemp.Add(benef);
                        }
                    }
                }

                lstProd = lstTemp;
            }

            if (addMateriaisProjeto && showChildren)
            {
                for (int i = lstProd.Count - 1; i >= 0; i--)
                    if (lstProd[i].IdItemProjeto > 0)
                    {
                        var itens = MaterialItemProjetoDAO.Instance.GetByItemProjeto(lstProd[i].IdItemProjeto.Value).ToArray();

                        DescontoAcrescimo.Instance.AplicarAcrescimo(null, orcamento, 2, lstProd[i].ValorAcrescimo, itens);
                        DescontoAcrescimo.Instance.AplicarAcrescimoAmbiente(null, orcamento, lstProd[i].TipoAcrescimo, lstProd[i].Acrescimo, itens);

                        if (PedidoConfig.Comissao.ComissaoAlteraValor)
                            DescontoAcrescimo.Instance.AplicarComissao(null, orcamento, orcamento.PercComissao, itens);

                        if (PedidoConfig.RatearDescontoProdutos)
                        {
                            DescontoAcrescimo.Instance.AplicarDesconto(null, orcamento, 2, lstProd[i].ValorDesconto, itens);
                            DescontoAcrescimo.Instance.AplicarDescontoAmbiente(null, orcamento, lstProd[i].TipoDesconto, lstProd[i].Desconto, itens);
                        }

                        foreach (MaterialItemProjeto m in itens)
                        {
                            ProdutosOrcamento produto = new ProdutosOrcamento();
                            produto.IdProd = lstProd[i].IdProd;
                            produto.Altura = m.Altura;
                            produto.AlturaCalc = m.AlturaCalc;
                            produto.Ambiente = lstProd[i].Ambiente;
                            produto.Descricao = lstProd[i].Descricao;
                            produto.Beneficiamentos = m.Beneficiamentos;
                            produto.CodInterno = m.CodInterno;
                            produto.Custo = m.Custo;
                            produto.DescrProduto = m.DescrProduto;
                            produto.Espessura = m.Espessura;
                            produto.IdAmbienteOrca = lstProd[i].IdAmbienteOrca;
                            produto.IdOrcamento = lstProd[i].IdOrcamento;
                            produto.IdProdParent = lstProd[i].IdProd;
                            produto.IdProduto = m.IdProd;
                            produto.Largura = m.Largura;
                            produto.Qtde = m.Qtde;
                            produto.Redondo = m.Redondo;
                            produto.Total = m.Total;
                            produto.TotM = m.TotM;
                            produto.TotMCalc = m.TotM2Calc;
                            produto.ValorBenef = m.ValorBenef;
                            produto.ValorAcrescimo = m.ValorAcrescimo;
                            produto.ValorAcrescimoProd = m.ValorAcrescimoProd;
                            produto.ValorDesconto = m.ValorDesconto;
                            produto.ValorDescontoProd = m.ValorDescontoProd;
                            produto.ValorProd = m.Valor;
                            produto.ValorTabela = m.Valor;
                            produto.NumSeq = lstProd[i].NumSeq;

                            if (produto.Redondo)
                            {
                                if (!BenefConfigDAO.Instance.CobrarRedondo() && !produto.DescrProduto.ToLower().Contains("redondo"))
                                    produto.DescrProduto += " REDONDO";

                                produto.Largura = 0;
                            }

                            lstProd.Add(produto);

                            if (!incluirBeneficiamentos)
                                continue;

                            GenericBenefCollection lstBenef = produto.Beneficiamentos;

                            if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                            {
                                // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                                foreach (GenericBenef mipb in lstBenef)
                                {
                                    ProdutosOrcamento benef = new ProdutosOrcamento();
                                    benef.IdOrcamento = idOrca;
                                    benef.IdAmbienteOrca = produto.IdAmbienteOrca;
                                    benef.IdItemProjeto = produto.IdItemProjeto;
                                    benef.Ambiente = produto.Ambiente;
                                    benef.Qtde = mipb.Qtd > 0 ? mipb.Qtd : 1;
                                    benef.ValorProd = mipb.ValorUnit;
                                    benef.Total = mipb.Valor;
                                    benef.ValorBenef = 0;
                                    benef.DescrProduto = " " + mipb.DescricaoBeneficiamento +
                                        Utils.MontaDescrLapBis(mipb.BisAlt, mipb.BisLarg, mipb.LapAlt, mipb.LapLarg, mipb.EspBisote, null, null, false);
                                    benef.IsBeneficiamento = true;

                                    benef.NumSeq = lstProd[i].NumSeq;
                                    lstProd.Add(benef);
                                }
                            }
                            else
                            {
                                if (lstBenef.Count > 0)
                                {
                                    ProdutosOrcamento benef = new ProdutosOrcamento();
                                    benef.IdOrcamento = idOrca;
                                    benef.IdAmbienteOrca = produto.IdAmbienteOrca;
                                    benef.IdItemProjeto = produto.IdItemProjeto;
                                    benef.Ambiente = produto.Ambiente;
                                    benef.Qtde = 0;
                                    benef.ValorBenef = 0;
                                    benef.ValorProd = 0;
                                    benef.NumSeq = lstProd[i].NumSeq;
                                    benef.IsBeneficiamento = true;

                                    // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                                    foreach (GenericBenef mipb in lstBenef)
                                    {
                                        benef.ValorProd += mipb.ValorUnit;
                                        benef.Total = (benef.Total > 0 ? benef.Total.Value : 0) + mipb.Valor;
                                        string textoQuantidade = (mipb.TipoCalculo == TipoCalculoBenef.Quantidade) ? mipb.Qtd.ToString() + " " : "";
                                        benef.DescrProduto += "; " + textoQuantidade + mipb.DescricaoBeneficiamento +
                                            Utils.MontaDescrLapBis(mipb.BisAlt, mipb.BisLarg, mipb.LapAlt, mipb.LapLarg, mipb.EspBisote, null, null, false);
                                    }

                                    benef.DescrProduto = " " + benef.DescrProduto.Substring(2);
                                    lstProd.Add(benef);
                                }
                            }
                        }
                    }
            }
            
            if (showChildren)
                for (int i = lstProd.Count - 1; i >= 0; i--)
                    if (lstProd[i].NumChild > 0 || (addMateriaisProjeto && lstProd[i].IdItemProjeto > 0))
                        lstProd.RemoveAt(i);

            int count = 1;

            foreach (ProdutosOrcamento p in lstProd)
            {
                p.NumItem = (count++).ToString();

                try
                {
                    if (OrcamentoConfig.UploadImagensOrcamento && 
                        File.Exists(Utils.GetProdutosOrcamentoPath + p.NomeImagem))
                        p.ImagemProjModPath = "file:///" + Utils.GetProdutosOrcamentoPath.Replace("\\", "/") + p.NomeImagem;

                    // Pega a imagem do modelo do projeto associada à este item
                    else if (p.IdItemProjeto > 0)
                    {
                        string nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(null, p.IdItemProjeto.Value);
                        
                        if (!String.IsNullOrEmpty(nomeFigura))
                            p.ImagemProjModPath = "file:///" + Utils.GetModelosProjetoPath.Replace("\\", "/") + nomeFigura;
                    }
                }
                catch { }

                // Quando os valores forem 0, não mostrar nada
                if (p.Qtde == 0) p.Qtde = null;
                if (p.Total == 0) p.Total = null;
                if (p.ValorProd == 0) p.ValorProd = null;

                if (!showChildren && addTextoAmbiente && p.Ambiente != null)
                    p.Ambiente += ":\r\n";
            }

            return lstProd.ToArray();
        }

        public ProdutosOrcamento[] GetForRpt(uint idOrca, bool incluirBeneficiamentos)
        {
            bool imprimirProdutos = OrcamentoDAO.Instance.ObtemValorCampo<bool>("imprimirProdutosOrcamento", "idOrcamento=" + idOrca);
            List<ProdutosOrcamento> itens = new List<ProdutosOrcamento>(GetReport(idOrca, imprimirProdutos, true, false,
                incluirBeneficiamentos));

            for (int i = 1; i < itens.Count; i++)
                ObterImagemProjeto(itens, i);

            return itens.ToArray();
        }

        private static void ObterImagemProjeto(List<ProdutosOrcamento> itens, int i)
        {
            if (OrcamentoConfig.UploadImagensOrcamento &&
                File.Exists(Utils.GetProdutosOrcamentoPath + itens[i].NomeImagem))
                itens[i].ImagemProjModPath = "file:///" + Utils.GetProdutosOrcamentoPath.Replace("\\", "/") + itens[i].NomeImagem;
            // Pega a imagem do modelo do projeto associada à este item
            else if (itens[i].IdItemProjeto > 0)
            {
                string nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(null, itens[i].IdItemProjeto.Value);

                if (!String.IsNullOrEmpty(nomeFigura))
                    itens[i].ImagemProjModPath = "file:///" + Utils.GetModelosProjetoPath.Replace("\\", "/") + nomeFigura;
            }
        }

        public IList<ProdutosOrcamento> GetForMemoriaCalculo(uint idOrca)
        {
            return GetReport(idOrca, true, false, false, false);
        }

        public IList<ProdutosOrcamento> GetForRecibo(uint idOrca)
        {
            return objPersistence.LoadData(Sql(idOrca, null, true, null, null, null, false, true)).ToList();
        }

        public IList<ProdutosOrcamento> GetList(uint idOrca, uint? idAmbiente, bool showChildren, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idOrca, idAmbiente, showChildren) == 0 && Glass.Configuracoes.Geral.NaoVendeVidro())
                return new [] { new ProdutosOrcamento() };

            // Se a lista de produtos tiver sido ordenada pelo ambiente, salva numSeq de acordo com esta sequência
            if (sortExpression != null && sortExpression.ToLower().Contains("ambiente") && idOrca > 0)
            {
                string sql = "";
                List<ProdutosOrcamento> lstProdOrca = objPersistence.LoadData("Select * From produtos_orcamento Where idProdParent is null And idOrcamento=" + idOrca + " Order By " + sortExpression);
                for (int i=0; i<lstProdOrca.Count; i++)
                    sql += "Update produtos_orcamento Set numSeq=" + (i+1) + " Where idProd=" + lstProdOrca[i].IdProd + " Or idProdParent=" + lstProdOrca[i].IdProd + ";";

                // Atualiza o número de sequência dos produtos sem idParent
                objPersistence.ExecuteCommand(sql);
            }

            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "numSeq";

            return LoadDataWithSortExpression(Sql(idOrca, idAmbiente, showChildren, null, null, null, !String.IsNullOrEmpty(sortExpression), true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idOrca, uint? idAmbiente, bool showChildren)
        {
            int count = GetCountReal(idOrca, idAmbiente, showChildren);
            return count > 0 || !Glass.Configuracoes.Geral.NaoVendeVidro() ? count : 1;
        }

        public int GetCountReal(uint idOrca, uint? idAmbiente, bool showChildren)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idOrca, idAmbiente, showChildren, null, null, null, false, false), null);
        }

        public IList<ProdutosOrcamento> GetByOrcamento(uint idOrcamento, bool showChildren)
        {
            return GetByOrcamento(null, idOrcamento, showChildren);
        }

        public IList<ProdutosOrcamento> GetByOrcamento(GDASession sessao, uint idOrcamento, bool showChildren)
        {
            return objPersistence.LoadData(sessao, Sql(idOrcamento, null, showChildren, null, null, null, false, true)).ToList();
        }

        public IList<ProdutosOrcamento> GetByProdutoOrcamento(uint idProduto)
        {
            return GetByProdutoOrcamento(null, idProduto);
        }

        public IList<ProdutosOrcamento> GetByProdutoOrcamento(GDASession sessao, uint idProduto)
        {
            return objPersistence.LoadData(sessao, Sql(null, null, true, null, idProduto, null, false, true)).ToList();
        }

        public bool ContainsChildItems(uint idProduto)
        {
            return ContainsChildItems(null, idProduto);
        }

        public bool ContainsChildItems(GDASession session, uint idProduto)
        {
            return objPersistence.ExecuteScalar(session, Sql(null, null, true, null, idProduto, null, false, false)).ToString().StrParaUint() > 0;
        }

        public IList<ProdutosOrcamento> GetByIds(string idsProdutos)
        {
            return objPersistence.LoadData(Sql(null, null, true, null, null, idsProdutos, false, true)).ToList();
        }

        #region Insere/Atualiza Produto de Projeto

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <param name="idAmbienteOrca"></param>
        /// <param name="itemProj"></param>
        public uint InsereAtualizaProdProj(uint idOrcamento, uint? idAmbienteOrca, ItemProjeto itemProj)
        {
            return InsereAtualizaProdProj(null, idOrcamento, idAmbienteOrca, itemProj);
        }

        internal uint InsereAtualizaProdProj(GDASession sessao, uint idOrcamento, uint? idAmbienteOrca, ItemProjeto itemProj)
        {
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(null, idOrcamento);
            return InsereAtualizaProdProj(sessao, orcamento, idAmbienteOrca, itemProj);
        }

        /// <summary>
        /// Insere/Atualiza Produto de Projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idOrcamento"></param>
        /// <param name="idAmbienteOrca"></param>
        /// <param name="itemProj"></param>
        internal uint InsereAtualizaProdProj(GDASession sessao, Orcamento orcamento, uint? idAmbienteOrca, ItemProjeto itemProj)
        {
            try
            {
                object obj = objPersistence.ExecuteScalar(sessao, "Select numSeq From produtos_orcamento Where idItemProjeto=" + 
                    itemProj.IdItemProjeto + " limit 1");

                // Salva descrição antiga do ambiene, caso esteja utilizando o projeto OTR01
                string descricaoAnterior = ObtemValorCampo<string>(sessao, "descricao", "idItemProjeto=" + itemProj.IdItemProjeto + " limit 1");

                // Exclui o beneficiamento do produto do orçamento, caso já tenha sido inserido.
                objPersistence.ExecuteCommand(sessao, string.Format("DELETE FROM produto_orcamento_benef WHERE IdProd IN " +
                    "(SELECT IdProd FROM produtos_orcamento WHERE IdItemProjeto={0})", itemProj.IdItemProjeto));

                // Exclui produto do orçamento, caso já tenha sido inserido
                objPersistence.ExecuteCommand(sessao, "Delete from produtos_orcamento Where idItemProjeto=" + itemProj.IdItemProjeto);

                // Remove acréscimo, desconto e comissão
                float percComissao = OrcamentoDAO.Instance.RecuperaPercComissao(sessao, orcamento.IdOrcamento);

                //Chamado 49030
                if (!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                {
                    foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto))
                    {
                        MaterialItemProjeto material = mip;

                        // Verifica qual preço deverá ser utilizado
                        mip.InicializarParaCalculo(sessao, orcamento);
                        material.Valor = (mip as IProdutoCalculo).DadosProduto.ValorTabela();

                        MaterialItemProjetoDAO.Instance.CalcTotais(sessao, ref material, false);
                        MaterialItemProjetoDAO.Instance.UpdateBase(sessao, material);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(sessao, itemProj.IdItemProjeto);
                    }
                }

                ProdutosOrcamento prodOrca = new ProdutosOrcamento();
                prodOrca.IdOrcamento = orcamento.IdOrcamento;
                prodOrca.IdAmbienteOrca = idAmbienteOrca;
                prodOrca.IdItemProjeto = itemProj.IdItemProjeto;
                prodOrca.Ambiente = itemProj.Ambiente;
                
                string descricao = UtilsProjeto.FormataTextoOrcamento(sessao, itemProj);
                if (!String.IsNullOrEmpty(descricao)) prodOrca.Descricao = descricao;
                if (String.IsNullOrEmpty(prodOrca.Descricao)) prodOrca.Descricao = descricaoAnterior;

                prodOrca.Qtde = itemProj.Qtde;
                prodOrca.ValorProd = itemProj.Total / (itemProj.Qtde > 0 ? itemProj.Qtde : 1);
                prodOrca.Total = itemProj.Total;
                prodOrca.Custo = itemProj.CustoTotal;
                prodOrca.Espessura = itemProj.EspessuraVidro;

                if (obj != null && obj.ToString() != String.Empty) prodOrca.NumSeq = obj.ToString().StrParaUint();
                prodOrca.IdProd = Insert(sessao, prodOrca, orcamento);

                // Necessário para atualizar o total bruto e calcular corretamente a possível comissão
                UpdateTotaisProdutoOrcamento(sessao, prodOrca);

                // Aplica comissão
                ProdutosOrcamento[] prod = { GetElementByPrimaryKey(sessao, prodOrca.IdProd) };
                
                if (PedidoConfig.Comissao.ComissaoAlteraValor)
                {   
                    DescontoAcrescimo.Instance.AplicarComissao(sessao, orcamento, percComissao, prod);
                }

                prodOrca = prod[0];

                // Atualiza a comissão aplicada
                UpdateBase(sessao, prodOrca, orcamento);

                // Atualiza o total do orçamento
                OrcamentoDAO.Instance.UpdateTotaisOrcamento(sessao, orcamento);

                return prodOrca.IdProd;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region Exclui todos os produtos de um orçamento

        /// <summary>
        /// Exclui todos os produtos de um orçamento
        /// </summary>
        /// <param name="idOrcamento"></param>
        public void DeleteByOrcamento(uint idOrcamento)
        {
            DeleteByOrcamento(null, idOrcamento);
        }

        /// <summary>
        /// Exclui todos os produtos de um orçamento
        /// </summary>
        public void DeleteByOrcamento(GDASession sessao, uint idOrcamento)
        {
            string sql =
                "Delete from produto_orcamento_benef Where idProd In (Select idProd from produtos_orcamento Where idOrcamento=" + idOrcamento + ");" +
                "Delete From produtos_orcamento Where idOrcamento=" + idOrcamento + ";" +
                "Delete from ambiente_orcamento Where idOrcamento=" + idOrcamento;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Exclui todos os produtos de um orçamento que vieram de um projeto
        /// </summary>
        public void DeleteFromProjeto(GDASession session, uint idOrcamento)
        {
            string sql =
                @"Delete from produto_orcamento_benef Where idProd In 
                (Select idProd from produtos_orcamento Where idItemProjeto is not null and idOrcamento=" + idOrcamento + @");

                Delete From produtos_orcamento Where idItemProjeto is not null and idOrcamento=" + idOrcamento + @";

                Delete from ambiente_orcamento Where idOrcamento=" + idOrcamento + @" And idAmbienteOrca not in
                (Select idAmbienteOrca from produtos_orcamento Where idOrcamento=" + idOrcamento + ");";

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Atualiza total do beneficiamento aplicado neste produto

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <param name="beneficiamentos"></param>
        public void AtualizaBenef(GDASession sessao, uint idProd, GenericBenefCollection beneficiamentos, Orcamento orcamento)
        {
            bool temItens = objPersistence.ExecuteSqlQueryCount(sessao, @"select count(*) from produtos_orcamento
                where idProdParent=" + idProd) > 0;

            if (!temItens)
            {
                ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(sessao, idProd);

                foreach (ProdutoOrcamentoBenef b in beneficiamentos.ToProdutosOrcamento(idProd))
                    ProdutoOrcamentoBenefDAO.Instance.Insert(sessao, b);

                UpdateValorBenef(sessao, idProd, orcamento);
            }
            else
                UpdateTotaisProdutoOrcamento(sessao, idProd);
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado
        /// </summary>
        private void UpdateValorBenef(GDASession sessao, uint idProdOrcamento, Orcamento orcamento)
        {
            uint idProd = ObtemValorCampo<uint>(sessao, "idProduto", "idProd=" + idProdOrcamento);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(sessao, (int)idProd))
                return;

            string sql = "Update produtos_orcamento po set po.valorBenef=" +
                "Coalesce(Round((Select Sum(pob.Valor) from produto_orcamento_benef pob Where pob.idProd=po.idProd), 2), 0) " +
                "Where po.idProd=" + idProd;

            objPersistence.ExecuteCommand(sessao, sql);

            ProdutosOrcamento prod = GetElementByPrimaryKey(sessao, idProdOrcamento);
            
            if (prod.IdProdParent != null)
                if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produtos_orcamento Where idProdParent=" + prod.IdProdParent.Value) > 0)
                    UpdateTotaisProdutoOrcamento(sessao, prod.IdProdParent.Value);
                else
                    prod.IdProdParent = null;

            // Recalcula o total bruto/valor unitário bruto
            UpdateBase(sessao, prod, orcamento);
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado
        /// </summary>
        private void UpdateValorBenefChild(GDASession sessao, uint idProdParent)
        {
            if (Glass.Configuracoes.Geral.NaoVendeVidro())
                return;

            string sql = "Update produtos_orcamento po set po.valorBenef=" +
                "Coalesce(Round((Select Sum(pob.Valor) from produto_orcamento_benef pob Where pob.idProd=po.idProd), 2), 0) " +
                "Where po.idProdParent=" + idProdParent;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Retorna o número de sequência para o produto ou orçamento

        /// <summary>
        /// Retorna o próximo número de sequência para um produto de um orçamento.
        /// </summary>
        /// <param name="idOrca"></param>
        /// <returns></returns>
        public uint GetNextNumSeq(uint idOrca)
        {
            return GetNextNumSeq(null, idOrca);
        }

        /// <summary>
        /// Retorna o próximo número de sequência para um produto de um orçamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idOrca"></param>
        /// <returns></returns>
        public uint GetNextNumSeq(GDASession sessao, uint idOrca)
        {
            string sql = "select coalesce(max(numseq), 0) from produtos_orcamento where idOrcamento=" + idOrca;
            return objPersistence.ExecuteScalar(sessao, sql).ToString().StrParaUint() + 1;
        }

        /// <summary>
        /// Retorna o número de sequência de um produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public uint GetNumSeq(uint idProd)
        {
            string sql = "select coalesce(numseq, 0) from produtos_orcamento where idProd=" + idProd;
            return objPersistence.ExecuteScalar(sql).ToString().StrParaUint();
        }

        #endregion

        #region Retorno o IdItemProjeto do produto

        /// <summary>
        /// Retorno o IdItemProjeto do produto
        /// </summary>
        /// <param name="idProdOrca"></param>
        /// <returns></returns>
        public uint BuscaItemProjeto(uint idProdOrca)
        {
            return ExecuteScalar<uint>("Select Coalesce(idItemProjeto, 0) From produtos_orcamento Where idProd=" + idProdOrca);
        }

        #endregion

        #region Atualiza o custo e total do produto pai

        /// <summary>
        /// Atualiza os totais do produto do orçamento.
        /// </summary>
        /// <param name="idProdOrca"></param>
        public void UpdateTotaisProdutoOrcamento(uint idProdOrca)
        {
            UpdateTotaisProdutoOrcamento(null, idProdOrca);
        }

        /// <summary>
        /// Atualiza os totais do produto do orçamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdOrca"></param>
        public void UpdateTotaisProdutoOrcamento(GDASession sessao, uint idProdOrca)
        {
            UpdateTotaisProdutoOrcamento(sessao, GetElementByPrimaryKey(sessao, idProdOrca));
        }

        /// <summary>
        /// Atualiza os totais do produto do orçamento.
        /// </summary>
        /// <param name="prodOrca"></param>
        public void UpdateTotaisProdutoOrcamento(ProdutosOrcamento prodOrca)
        {
            UpdateTotaisProdutoOrcamento(null, prodOrca);
        }

        /// <summary>
        /// Atualiza os totais do produto do orçamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="prodOrca"></param>
        public void UpdateTotaisProdutoOrcamento(GDASession sessao, ProdutosOrcamento prodOrca)
        {
            string sql;

            if (prodOrca.TemItensProdutoSession(sessao))
            {
                UpdateValorBenefChild(sessao, prodOrca.IdProd);

                sql = "select coalesce(sum(round(custo, 2)), 0) from produtos_orcamento where idProdParent=" + prodOrca.IdProd;
                decimal custo = decimal.Parse(objPersistence.ExecuteScalar(sessao, sql).ToString());

                sql = "select coalesce(sum(round(total + coalesce(valorBenef, 0), 2)), 0) from produtos_orcamento where idProdParent=" + prodOrca.IdProd;
                decimal total = decimal.Parse(objPersistence.ExecuteScalar(sessao, sql).ToString());

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    var valorDesconto = prodOrca.Desconto;
                    if (prodOrca.TipoDesconto == 1)
                    {
                        valorDesconto = Math.Round(total * (valorDesconto / 100), 2);
                    }
                    total -= valorDesconto;
                }

                sql = "update produtos_orcamento set custo=?custo, valorProd=?total / Coalesce(qtde, 1), total=?total where idProd=" + prodOrca.IdProd;
                objPersistence.ExecuteCommand(sessao, sql,  new GDAParameter("?custo", custo), new GDAParameter("?total", total));
            }
            else if (prodOrca.IdItemProjeto != null)
            {
                prodOrca.Total = ItemProjetoDAO.Instance.GetTotalItemProjetoAluminio(sessao, prodOrca.IdItemProjeto.Value);
                if (prodOrca.Total != null)
                {
                    if (!PedidoConfig.RatearDescontoProdutos)
                    {
                        var valorDesconto = prodOrca.Desconto;
                        if (prodOrca.TipoDesconto == 1)
                        {
                            valorDesconto = Math.Round(prodOrca.Total.GetValueOrDefault() * (valorDesconto / 100), 2);
                        }
                        prodOrca.Total -= valorDesconto;
                    }

                    prodOrca.ValorUnitarioBruto = prodOrca.Total.Value / (prodOrca.Qtde > 0 ? (decimal)prodOrca.Qtde.Value : 1);
                    
                    objPersistence.ExecuteCommand(sessao, @"Update produtos_orcamento Set totalBruto=?total, total=?total, valorUnitBruto=?valorUnit
                        Where idProd=" + prodOrca.IdProd, new GDAParameter("?total", prodOrca.Total.Value), 
                        new GDAParameter("?valorUnit", prodOrca.ValorUnitarioBruto));
                }
                
                sql = @"update produtos_orcamento po set total=total-valorDesconto+valorAcrescimo-valorDescontoProd+valorAcrescimoProd+valorComissao,
                    valorProd=total/Coalesce(qtde,1) where idProd=" + prodOrca.IdProd;

                objPersistence.ExecuteCommand(sessao, sql);
            }
            else
            {
                sql = "update produtos_orcamento set total=(valorProd * qtde), totalbruto=(valorprod*qtde), valorunitbruto=valorprod where (total is null or total=0) and idProd=" + prodOrca.IdProd;
                objPersistence.ExecuteCommand(sessao, sql);

                if (prodOrca.IdProdParent != null)
                    UpdateTotaisProdutoOrcamento(sessao, prodOrca.IdProdParent.Value);
            }
        }

        #endregion

        #region Negociar produtos

        public void Negociar(uint idProdOrca, bool negociar)
        {
            objPersistence.ExecuteCommand("update produtos_orcamento set negociar=" + negociar.ToString().ToLower() + " where idProd=" + idProdOrca);
        }

        #endregion

        #region Acréscimo e Desconto

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool AplicarAcrescimo(GDASession session, ProdutosOrcamento produto, Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            // Deve-se recalcular o total bruto sempre, pois caso tenha sido adicionado mais algum produto após o desconto/acréscimo, 
            // o total bruto ficaria errado, causando erros ao reaplicar desconto/acréscimo
            var tabela = produto.IdItemProjeto > 0 ? "material_item_projeto" : "produtos_orcamento";
            var where = produto.IdItemProjeto > 0 ? "idItemProjeto=" + produto.IdItemProjeto : "idProdParent=" + produto.IdProd;

            produto.TotalBruto = ExecuteScalar<decimal>(session, "select sum(totalBruto) from " + tabela + " where " + where);
            produto.TotalBruto += ExecuteScalar<decimal>(session, "select sum(valorBenef" + (produto.IdItemProjeto > 0 ? "" : "-coalesce(valorComissao,0)") + ") from " + tabela + " where " + where);

            var aplicado = DescontoAcrescimo.Instance.AplicarAcrescimoAmbiente(
                session,
                orcamento,
                produto.TipoAcrescimo,
                produto.Acrescimo,
                produtosOrcamento
            );

            produto.ValorAcrescimoProd = produtosOrcamento.Sum(p => p.ValorAcrescimoProd);

            return aplicado;
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool RemoverAcrescimo(GDASession session, ProdutosOrcamento produto, Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimoAmbiente(
                session,
                orcamento,
                produtosOrcamento
            );
        }

        #endregion

        #endregion

        #region Desconto

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool AplicarDesconto(GDASession session, ProdutosOrcamento produto, Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            // Deve-se recalcular o total bruto sempre, pois caso tenha sido adicionado mais algum produto após o desconto/acréscimo, 
            // o total bruto ficaria errado, causando erros ao reaplicar desconto/acréscimo
            var tabela = produto.IdItemProjeto > 0 ? "material_item_projeto" : "produtos_orcamento";
            var where = produto.IdItemProjeto > 0 ? "idItemProjeto=" + produto.IdItemProjeto : "idProdParent=" + produto.IdProd;

            produto.TotalBruto = ExecuteScalar<decimal>(session, "select sum(totalBruto) from " + tabela + " where " + where);
            produto.TotalBruto += ExecuteScalar<decimal>(session, "select sum(valorBenef" + (produto.IdItemProjeto > 0 ? "" : "-coalesce(valorComissao,0)") + ") from " + tabela + " where " + where);

            var aplicado = DescontoAcrescimo.Instance.AplicarDescontoAmbiente(
                session,
                orcamento,
                produto.TipoDesconto,
                produto.Desconto,
                produtosOrcamento
            );

            if (aplicado)
                produto.ValorDescontoProd = produtosOrcamento.Sum(p => p.ValorDescontoProd);

            return aplicado;
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool RemoverDesconto(GDASession sessao, ProdutosOrcamento produto, Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverDescontoAmbiente(
                sessao,
                orcamento,
                produtosOrcamento
            );
        }

        #endregion

        #endregion

        #region Finalização

        internal void FinalizarAplicacaoAcrescimoDesconto(GDASession sessao, Orcamento orcamento,
            ProdutosOrcamento produtoPai, IEnumerable<ProdutosOrcamento> produtosOrcamento, bool atualizar)
        {
            if (atualizar)
            {
                foreach (ProdutosOrcamento produto in produtosOrcamento)
                {
                    UpdateBase(sessao, produto, orcamento);
                    AtualizaBenef(sessao, produto.IdProd, produto.Beneficiamentos, orcamento);
                }

                UpdateBase(sessao, produtoPai, orcamento);
                AtualizaBenef(sessao, produtoPai.IdProd, produtoPai.Beneficiamentos, orcamento);
                UpdateTotaisProdutoOrcamento(sessao, produtoPai);
            }
        }

        #endregion

        #endregion

        #region Busca de IDs relacionados a itens de projeto

        /// <summary>
        /// Retorna o id do produto do orçamento de um item projeto.
        /// </summary>
        /// <param name="idProdOrca"></param>
        /// <returns></returns>
        public uint? GetIdItemProjetoById(uint idProdOrca)
        {
            string sql = "select idItemProjeto from produtos_orcamento where idProd=" + idProdOrca;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno.ToString() != "" ? (uint?)retorno.ToString().StrParaUint() : null;
        }

        /// <summary>
        /// Retorna o id do produto do orçamento de um item projeto.
        /// </summary>
        public uint? GetIdByIdItemProjeto(uint idItemProjeto)
        {
            return GetIdByIdItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Retorna o id do produto do orçamento de um item projeto.
        /// </summary>
        public uint? GetIdByIdItemProjeto(GDASession session, uint idItemProjeto)
        {
            string sql = "select idProd from produtos_orcamento where idItemProjeto=" + idItemProjeto;
            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null && retorno.ToString() != "" ? (uint?)retorno.ToString().StrParaUint() : null;
        }

        #endregion

        #region Recalcula os valores unitários e totais brutos e líquidos

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        internal void RecalcularValores(GDASession session, ProdutosOrcamento prod, bool somarAcrescimoDesconto,
            Orcamento orcamento)
        {
            GenericBenefCollection benef = prod.Beneficiamentos;
            decimal valorBenef = prod.ValorBenef;

            try
            {
                prod.Beneficiamentos = GenericBenefCollection.Empty;
                prod.ValorBenef = 0;

                ValorBruto.Instance.Calcular(session, orcamento, prod);

                if (prod.IdProduto > 0)
                {
                    prod.ValorTabela = (prod as IProdutoCalculo).DadosProduto.ValorTabela();

                    var valorUnitario = ValorUnitario.Instance.RecalcularValor(session, orcamento, prod, !somarAcrescimoDesconto, true);
                    prod.ValorProd = valorUnitario ?? Math.Max(prod.ValorTabela, prod.ValorProd.GetValueOrDefault());

                    ValorTotal.Instance.Calcular(
                        session,
                        orcamento,
                        prod,
                        Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                        true,
                        prod.Beneficiamentos.CountAreaMinimaSession(session)
                    );
                }
            }
            finally
            {
                prod.Beneficiamentos = benef;
                prod.ValorBenef = valorBenef;
            }
        }

        #endregion

        #region Verificações da peça

        public bool IsRedondo(uint idProd)
        {
            return ObtemValorCampo<bool>("redondo", "idProd=" + idProd);
        }

        #endregion

        #region Obtém/Valida desconto

        /// <summary>
        /// Retorna o maior desconto em percentual aplicado nos produtos (ambiente) do orçamento
        /// </summary>
        internal decimal ObtemMaiorDesconto(uint idOrcamento)
        {
            return ObtemMaiorDesconto(null, idOrcamento);
        }

        /// <summary>
        /// Retorna o maior desconto em percentual aplicado nos produtos (ambiente) do orçamento
        /// </summary>
        internal decimal ObtemMaiorDesconto(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<decimal>(sessao, "Max(if(tipoDesconto=1, desconto, (desconto/totalbruto)*100))", "idorcamento=" + idOrcamento);
        }

        private bool ValidaDesconto(GDASession session, uint idOrcamento, int tipoDesconto, decimal desconto, decimal totalBruto)
        {
            if (desconto > 0 && OrcamentoConfig.Desconto.DescontoMaximoOrcamento > 0 && OrcamentoConfig.Desconto.DescontoMaximoOrcamento <= 100)
            {
                // Calcula o desconto máximo permitido verificando se foi lançado algum desconto pelo administrador
                uint idFunc = UserInfo.GetUserInfo.CodUser;
                if (Geral.ManterDescontoAdministrador)
                    idFunc = OrcamentoDAO.Instance.ObtemIdFuncDesc(session, idOrcamento).GetValueOrDefault(idFunc);

                decimal descMax = (decimal)OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(session, idFunc);

                int tipoDescontoOrca = OrcamentoDAO.Instance.ObtemValorCampo<int>(session, "tipoDesconto", "IdOrcamento=" + idOrcamento);
                decimal descontoOrca = OrcamentoDAO.Instance.ObtemValorCampo<int>(session, "desconto", "IdOrcamento=" + idOrcamento);
                decimal percDescontoOrca;

                if (tipoDescontoOrca == 1)
                    percDescontoOrca = descontoOrca;
                else
                    percDescontoOrca = (descontoOrca / OrcamentoDAO.Instance.GetTotalBruto(session, idOrcamento)) * 100;

                if (tipoDesconto == 1)
                {
                    if (desconto + percDescontoOrca > descMax)
                        return false;
                }
                else
                {
                    if (((desconto / totalBruto) * 100) + percDescontoOrca > descMax)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Atualiza o valor do Orcamento ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(ProdutosOrcamento objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);
                    
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

        /// <summary>
        /// Atualiza o valor do Orcamento ao incluir um produto ao mesmo
        /// </summary>
        public override uint Insert(GDASession sessao, ProdutosOrcamento objInsert)
        {
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(sessao, objInsert.IdOrcamento);
            return Insert(sessao, objInsert, orcamento);
        }

        internal uint Insert(GDASession sessao, ProdutosOrcamento objInsert, Orcamento orcamento)
        {
            uint returnValue = 0;

            try
            {
                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido &&
                    OrcamentoDAO.Instance.ObtemTipoOrcamento(sessao, objInsert.IdOrcamento).GetValueOrDefault() == 0)
                    throw new Exception("Informe o tipo do orçamento antes da inserção de produtos.");

                // Chamado 49342 - O ambiente do orçamento estava sendo excluido durante a inserção e depois não era possível editar os produtos.
                if (objInsert.IdAmbienteOrca.GetValueOrDefault() > 0 && !AmbienteOrcamentoDAO.Instance.AmbienteOrcamentoExiste(sessao, (uint)objInsert.IdAmbienteOrca))
                    throw new Exception("O ambiente do orçamento foi excluído durante a inserção desse(s) produto(s), volte a tela do orçamento, insira um novo ambiente e então insira o(s) produto(s) novamente.");

                if (objInsert.ValorProd > 0 && objInsert.Qtde > 0 && objInsert.Total == null || objInsert.Total == 0)
                {
                    objInsert.ValorProd = objInsert.ValorProd.Value;
                    objInsert.Total = objInsert.ValorProd * (decimal)objInsert.Qtde;

                    ProdutosOrcamento[] prod = { objInsert };
                    DescontoAcrescimo.Instance.AplicarComissao(sessao, orcamento, orcamento.PercComissao, prod);

                    objInsert = prod[0];
                }

                if (objInsert.Descricao != null && objInsert.Descricao.Length > 1500)
                    objInsert.Descricao = objInsert.Descricao.Substring(0, 1500);

                if (objInsert.IdProduto > 0)
                {
                    // Verifica se o produto é do grupo vidro.
                    if (ProdutoDAO.Instance.IsVidro(sessao, (int)objInsert.IdProduto.Value))
                    {
                        // Recupera o id do subgrupo do produto.
                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)objInsert.IdProduto.Value);
                        if (idSubgrupoProd > 0)
                            // Se o cálculo for qtde recupera os bneficiamentos inseridos no cadastro do produto.
                            if (SubgrupoProdDAO.Instance.ObtemTipoCalculo(sessao, idSubgrupoProd.Value, false) ==
                                (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd)
                            {
                                Produto prod = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, objInsert.IdProduto.Value);
                                if (prod.Beneficiamentos.Count > 0)
                                    objInsert.Beneficiamentos = prod.Beneficiamentos;

                                // Busca novamente a altura e largura do produto, caso estejam definidas no cadastro de produto
                                if (prod.Altura > 0 || prod.Largura > 0)
                                {
                                    objInsert.Altura = (float)prod.Altura;
                                    objInsert.Largura = prod.Largura.GetValueOrDefault();
                                }
                            }
                    }

                    uint? idCliente = OrcamentoDAO.Instance.ObtemIdCliente(sessao, objInsert.IdOrcamento);

                    if (idCliente > 0 && objInsert.IdProduto.Value > 0)
                    {
                        ValorTotal.Instance.Calcular(
                            sessao,
                            orcamento,
                            objInsert,
                            Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                            true,
                            objInsert.Beneficiamentos.CountAreaMinimaSession(sessao));
                    }

                    CalculaDescontoEValorBrutoProduto(sessao, objInsert, orcamento);
                }

                returnValue = base.Insert(sessao, objInsert);
                objPersistence.ExecuteCommand(sessao,
                    "update produtos_orcamento set negociar=true where idProd=" + returnValue);

                AtualizaBenef(sessao, returnValue, objInsert.Beneficiamentos, orcamento);
                objInsert.RefreshBeneficiamentos();

                if (objInsert.NumSeq == 0)
                {
                    objInsert.NumSeq = GetNextNumSeq(sessao, objInsert.IdOrcamento);
                    objPersistence.ExecuteCommand(sessao,
                        "update produtos_orcamento set NumSeq=?numSeq where idProd=?idProd",
                        new GDAParameter("?numSeq", objInsert.NumSeq), new GDAParameter("?idProd", returnValue));
                }
            }
            catch (Exception ex)
            {
                ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(sessao, returnValue);
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg(
                    "Falha ao incluir Produto no Orçamento.", ex));
            }

            try
            {
                if (objInsert.IdProdParent != null)
                    UpdateTotaisProdutoOrcamento(sessao, objInsert.IdProdParent.Value);

                OrcamentoDAO.Instance.UpdateTotaisOrcamento(sessao, orcamento, true, false);
            }
            catch (Exception ex)
            {
                base.Delete(sessao, objInsert);
                throw new Exception("Falha ao atualizar Valor do Orçamento. Erro: " + ex.Message);
            }

            return returnValue;
        }

        public int UpdateBase(GDASession sessao, ProdutosOrcamento objUpdate, Orcamento orcamento)
        {
            if (objUpdate.IdProduto > 0)
            {
                CalculaDescontoEValorBrutoProduto(sessao, objUpdate, orcamento);
            }

            return base.Update(sessao, objUpdate);
        }

        private void CalculaDescontoEValorBrutoProduto(GDASession session, ProdutosOrcamento produto, Orcamento orcamento)
        {
            DescontoAcrescimo.Instance.RemoverDescontoQtde(session, orcamento, produto);
            DescontoAcrescimo.Instance.AplicarDescontoQtde(session, orcamento, produto);
            DiferencaCliente.Instance.Calcular(session, orcamento, produto);
            ValorBruto.Instance.Calcular(session, orcamento, produto);
        }

        public int UpdateComTransacao(ProdutosOrcamento objUpdate)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw ex;
                }
            }
        }

        public override int Update(ProdutosOrcamento objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, ProdutosOrcamento objUpdate)
        {
            int retorno;
            ProdutosOrcamento prodOrca;

            try
            {
                prodOrca = GetElementByPrimaryKey(session, objUpdate.IdProd);
                objUpdate.IdOrcamento = prodOrca.IdOrcamento;

                Orcamento orca = OrcamentoDAO.Instance.GetElement(session, objUpdate.IdOrcamento);

                if (Glass.Configuracoes.Geral.NaoVendeVidro() && objUpdate.IdProduto > 0)
                    objUpdate.Descricao =
                        ProdutoDAO.Instance.GetCodInterno(session, (int)objUpdate.IdProduto.Value) + " - " +
                        ProdutoDAO.Instance.GetDescrProduto(session, (int)objUpdate.IdProduto.Value);

                prodOrca.PercDescontoQtde = objUpdate.PercDescontoQtde;

                if (Glass.Configuracoes.Geral.NaoVendeVidro())
                {
                    prodOrca.IdProduto = objUpdate.IdProduto;
                    
                    if (objUpdate.IdProduto > 0)
                    {
                        ValorTotal.Instance.Calcular(
                            session,
                            orca,
                            objUpdate,
                            Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                            true,
                            0
                        );
                    }
                }

                var tamanhoMinimoBisote = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComBisote;
                var tamanhoMinimoLapidacao = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComLapidacao;
                var tamanhoMinimoTemperado = Configuracoes.PedidoConfig.TamanhoVidro.AlturaELarguraMinimasParaPecasTemperadas;

                var retornoValidacao = string.Empty;

                if (objUpdate.Beneficiamentos != null)
                {
                    foreach (var prodBenef in objUpdate.Beneficiamentos)
                    {
                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Bisote &&
                            objUpdate.Altura < tamanhoMinimoBisote && objUpdate.Largura < tamanhoMinimoBisote)
                            retornoValidacao += $"A altura ou largura minima para peças com bisotê é de {tamanhoMinimoBisote}mm.";

                        if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == Data.Model.TipoControleBenef.Lapidacao &&
                            objUpdate.Altura < tamanhoMinimoLapidacao && objUpdate.Largura < tamanhoMinimoLapidacao)
                            retornoValidacao += $"A altura ou largura minima para peças com lapidação é de {tamanhoMinimoLapidacao}mm.";
                    }
                }

                if (objUpdate.IdProduto > 0)
                {
                    var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)objUpdate.IdProduto);
                    var idSubGrupoProd = (int?)ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)objUpdate.IdProduto);

                    if (GrupoProdDAO.Instance.IsVidroTemperado(session, idGrupoProd, idSubGrupoProd)
                        && objUpdate.Altura < tamanhoMinimoTemperado && objUpdate.Largura < tamanhoMinimoTemperado)
                    {
                        retornoValidacao += $"A altura ou largura minima para peças com tempera é de {tamanhoMinimoTemperado}mm.";
                    }
                }

                if (!string.IsNullOrWhiteSpace(retornoValidacao))
                    throw new Exception(retornoValidacao);

                int tipoDesconto = prodOrca.TipoDesconto;
                decimal desconto = prodOrca.Desconto;
                int tipoAcrescimo = prodOrca.TipoAcrescimo;
                decimal acrescimo = prodOrca.Acrescimo;

                if (objUpdate.Desconto != prodOrca.Desconto)
                {
                    orca.IdFuncDesc = null;
                    objPersistence.ExecuteCommand(session,
                        "Update orcamento Set idFuncDesc=Null Where idOrcamento=" +
                        objUpdate.IdOrcamento);
                }

                // Verifica se este orçamento pode ter desconto
                string msgErro;
                if (!OrcamentoDAO.Instance.DescontoPermitido(session, orca, out msgErro))
                    throw new Exception(msgErro);

                if (
                    !ValidaDesconto(session, objUpdate.IdOrcamento, objUpdate.TipoDesconto,
                        objUpdate.Desconto,
                        prodOrca.IdProdParent == null ? prodOrca.Total.GetValueOrDefault() : prodOrca.TotalBruto))
                    throw new Exception("Desconto acima do permitido.");

                // Atualiza o acréscimo e o desconto
                if (prodOrca.DescontoAcrescimoPermitido &&
                    (prodOrca.TemItensProdutoSession(session) || prodOrca.IdItemProjeto != null))
                {
                    prodOrca.TipoAcrescimo = objUpdate.TipoAcrescimo;
                    prodOrca.Acrescimo = objUpdate.Acrescimo;
                    prodOrca.TipoDesconto = objUpdate.TipoDesconto;
                    prodOrca.Desconto = objUpdate.Desconto;
                }
                else
                {
                    prodOrca.TipoAcrescimo = 2;
                    prodOrca.Acrescimo = 0;
                    prodOrca.TipoDesconto = 2;
                    prodOrca.Desconto = 0;
                }
                // Atualiza o produto
                prodOrca.Descricao = objUpdate.Descricao;
                prodOrca.Qtde = objUpdate.Qtde;
                prodOrca.ValorProd = objUpdate.ValorProd;
                prodOrca.Ambiente = objUpdate.Ambiente;
                /* Chamado 37451.
                 * Este campo é imprescindível para o cálculo do alumínio no pedido. */
                prodOrca.Altura = objUpdate.Altura;
                prodOrca.AlturaCalc = objUpdate.AlturaCalc;
                prodOrca.TotMCalc = objUpdate.TotMCalc;
                prodOrca.Custo = objUpdate.Custo;
                
                if (objUpdate.Total > 0)
                    prodOrca.Total = objUpdate.Total;
                else if (objUpdate.ValorProd != null && objUpdate.Qtde != null)
                    prodOrca.Total = (decimal)objUpdate.Qtde * objUpdate.ValorProd;
                else
                    prodOrca.Total = null;

                retorno = UpdateBase(session, prodOrca, orca);

                bool atualizarDesconto = prodOrca.TipoDesconto != tipoDesconto || prodOrca.Desconto != desconto;
                bool atualizarAcrescimo = prodOrca.TipoAcrescimo != tipoAcrescimo ||
                                          prodOrca.Acrescimo != acrescimo;

                var produtos = new List<ProdutosOrcamento>();

                if (prodOrca.IdItemProjeto == null)
                    produtos.AddRange(GetByProdutoOrcamento(session, prodOrca.IdProd));
                else
                    produtos.Add(prodOrca);

                if (atualizarDesconto)
                {
                    RemoverDesconto(session, prodOrca, orca, produtos);
                    AplicarDesconto(session, prodOrca, orca, produtos);
                }

                if (atualizarAcrescimo)
                {
                    RemoverAcrescimo(session, prodOrca, orca, produtos);
                    AplicarAcrescimo(session, prodOrca, orca, produtos);
                }

                if (atualizarDesconto || atualizarAcrescimo)
                {
                    FinalizarAplicacaoAcrescimoDesconto(session, orca, prodOrca, produtos, true);

                    if (prodOrca.TemItensProdutoSession(session) || prodOrca.IdItemProjeto > 0)
                        prodOrca = GetElementByPrimaryKey(session, prodOrca.IdProd);

                    UpdateTotaisProdutoOrcamento(session, prodOrca);
                }

                AtualizaBenef(session, objUpdate.IdProd, objUpdate.Beneficiamentos, orca);
                objUpdate.RefreshBeneficiamentos();
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar produto do orçamento. Erro: " +
                                    ex.Message.Replace("'", String.Empty));
            }

            try
            {
                if (objUpdate.IdProdParent != null)
                    UpdateTotaisProdutoOrcamento(session, objUpdate.IdProdParent.Value);

                OrcamentoDAO.Instance.UpdateTotaisOrcamento(session,
                    OrcamentoDAO.Instance.GetElementByPrimaryKey(session, prodOrca.IdOrcamento), true, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Valor do Orçamento. Erro: " +
                                    ex.Message.Replace("'", String.Empty));
            }

            return retorno;
        }

        public override int Delete(ProdutosOrcamento objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProd);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                int returnValue;

                try
                {
                    transaction.BeginTransaction();

                    if (CurrentPersistenceObject.ExecuteSqlQueryCount(transaction, "select count(*) from produtos_orcamento Where IdProd=" + Key) == 0)
                        return 0;

                    uint idOrcamento = ObtemValorCampo<uint>(transaction, "idOrcamento", "idProd=" + Key);
                    var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrcamento);

                    uint? idItemProjeto = ObtemValorCampo<uint?>(transaction, "idItemProjeto", "idProd=" + Key);

                    var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;

                    var produtosOrcamento = alterarAcrescimoDesconto
                        ? GetByOrcamento(orcamento.IdOrcamento, true)
                            .Where(f => !f.TemItensProdutoSession(transaction))
                            .ToList()
                        : null;

                    if (orcamento.Acrescimo > 0)
                        OrcamentoDAO.Instance.RemoverAcrescimo(transaction, orcamento, produtosOrcamento);

                    if (orcamento.Desconto > 0)
                        OrcamentoDAO.Instance.RemoverDesconto(transaction, orcamento, produtosOrcamento);

                    uint? idProdParent = ObtemValorCampo<uint?>(transaction, "idProdParent", "idProd=" + Key);

                    // Salva exclusão no log, se for produto pai
                    if (idProdParent == null)
                    {
                        LogAlteracao logExclusao = new LogAlteracao();
                        logExclusao.Campo = "Exclusão ambiente";
                        logExclusao.DataAlt = DateTime.Now;
                        logExclusao.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                        logExclusao.IdRegistroAlt = (int)idOrcamento;
                        logExclusao.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(transaction, LogAlteracao.TabelaAlteracao.Orcamento, (int)idOrcamento);
                        logExclusao.Tabela = (int)LogAlteracao.TabelaAlteracao.Orcamento;
                        logExclusao.ValorAnterior = ObtemValorCampo<string>(transaction, "Ambiente", "IdProd=" + Key);
                        LogAlteracaoDAO.Instance.Insert(transaction, logExclusao);
                    }

                    ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(transaction, Key);
                    foreach (ProdutosOrcamento child in GetByProdutoOrcamento(transaction, Key))
                        DeleteByPrimaryKey(transaction, child.IdProd);

                    // Exclui item_projeto associado à este produto
                    if (idItemProjeto > 0 && objPersistence.ExecuteSqlQueryCount(transaction,
                        "Select Count(*) From item_projeto " +
                        "Where idOrcamento=" + idOrcamento +
                            " And idItemProjeto=" + idItemProjeto +
                            " And idProjeto is null") > 0)
                        ItemProjetoDAO.Instance.DeleteByPrimaryKey(transaction, idItemProjeto.Value);

                    if (orcamento.Acrescimo > 0)
                        OrcamentoDAO.Instance.AplicarAcrescimo(transaction, orcamento, produtosOrcamento);

                    if (orcamento.Desconto > 0)
                        OrcamentoDAO.Instance.AplicarDesconto(transaction, orcamento, produtosOrcamento);

                    OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(transaction,
                        orcamento, produtosOrcamento, alterarAcrescimoDesconto);

                    returnValue = GDAOperations.Delete(transaction, new ProdutosOrcamento { IdProd = Key });

                    if (idProdParent != null)
                        UpdateTotaisProdutoOrcamento(transaction, idProdParent.Value);

                    OrcamentoDAO.Instance.UpdateTotaisOrcamento(transaction, orcamento, true, false);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception("Falha ao atualizar Valor do Orçamento. Erro: " +
                        ex.Message.Replace("'", String.Empty));
                }

                return returnValue;
            }
        }

        public int DeleteByPrimaryKeyExcluirProjeto(GDASession session, uint Key)
        {
            int returnValue;

            if (CurrentPersistenceObject.ExecuteSqlQueryCount(session, "select count(*) from produtos_orcamento Where IdProd=" + Key) == 0)
                return 0;

            uint idOrcamento = ObtemValorCampo<uint>(session, "idOrcamento", "idProd=" + Key);
            Orcamento orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);

            uint? idItemProjeto = ObtemValorCampo<uint?>(session, "idItemProjeto", "idProd=" + Key);

            var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;

            var produtosOrcamento = alterarAcrescimoDesconto
                ? GetByOrcamento(orcamento.IdOrcamento, true)
                    .Where(f => !f.TemItensProdutoSession(session))
                    .ToList()
                : null;

            if (orcamento.Acrescimo > 0)
                OrcamentoDAO.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
            
            if (orcamento.Desconto > 0)
                OrcamentoDAO.Instance.RemoverDesconto(session, orcamento, produtosOrcamento);

            ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(session, Key);
            foreach (ProdutosOrcamento child in GetByProdutoOrcamento(session, Key))
                DeleteByPrimaryKey(session, child.IdProd);

            // Exclui item_projeto associado à este produto
            if (idItemProjeto > 0 && objPersistence.ExecuteSqlQueryCount(session,
                "Select Count(*) From item_projeto " +
                "Where idOrcamento=" + idOrcamento +
                " And idItemProjeto=" + idItemProjeto +
                " And idProjeto is null") > 0)
                ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, idItemProjeto.Value);

            if (orcamento.Acrescimo > 0)
                OrcamentoDAO.Instance.AplicarAcrescimo(session, orcamento, produtosOrcamento);

            if (orcamento.Desconto > 0)
                OrcamentoDAO.Instance.AplicarDesconto(session, orcamento, produtosOrcamento);

            OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(session,
                orcamento, produtosOrcamento, alterarAcrescimoDesconto);

            uint? idProdParent = ObtemValorCampo<uint?>(session, "idProdParent", "idProd=" + Key);

            returnValue = GDAOperations.Delete(session, new ProdutosOrcamento { IdProd = Key });

            if (idProdParent != null)
                UpdateTotaisProdutoOrcamento(session, idProdParent.Value);

            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session,
                OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento), true, false);

            return returnValue;
        }

        #endregion

        public IList<ProdutosOrcamento> GetByVidroOrcamento(uint idOrcamento)
        {
            string sql = Sql(idOrcamento, 0, true, 0, 0, null, false, true);
            sql += " and p.IdGrupoProd = 1";

            sql = sql.Substring(0, sql.IndexOf("Where", StringComparison.Ordinal));

            sql += "Where po.idOrcamento=" + idOrcamento + " and po.Altura > 0 and po.Largura > 0 and po.IdProduto > 0";

            sql += @" and po.idProduto in (
                        select idProd from chapa_vidro
                        union all select pbe.idProd from produto_baixa_estoque pbe
                            inner join chapa_vidro c on (pbe.idProdBaixa=c.idProd)
                    ) and po.idprodParent is not null ";

            sql += " Order By po.idProdPed Asc";

            var produtosOrcamento = objPersistence.LoadData(sql).ToList();

            foreach (var produtoOrcamento in produtosOrcamento)
                produtoOrcamento.DescrProduto =
                    produtoOrcamento.DescrProduto.Replace("(", "").Replace(")", "").Replace("+", "").Replace("-", "");

            return produtosOrcamento;
        }

        public uint ObtemIdProdutoPorIdItemProjeto(uint idItemProjeto)
        {
            return ObtemIdProdutoPorIdItemProjeto(null, idItemProjeto);
        }

        public uint ObtemIdProdutoPorIdItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            return ObtemValorCampo<uint>(sessao, "idProd", "idItemProjeto=" + idItemProjeto);
        }

        #region Otimização de Alumínios

        public List<ProdutosOrcamento> ObterAluminiosParaOtimizacao(int idOrcamento)
        {
            var sql = @"
                SELECT po.IdOrcamento, po.IdProd, po.Altura, p.CodInterno, po.Qtde,
                    p.Descricao AS DescrProduto, CAST((p.peso * po.Altura) as DECIMAL(12, 2)) AS Peso,
                    (pot.IdPecaOtimizada IS NOT NULL) AS PecaOtimizada, po.IdProduto, mip.GrauCorte, gm.Esquadria AS ProjetoEsquadria
                FROM produtos_orcamento po
	                INNER JOIN produto p ON (po.IdProduto = p.IdProd)
                    LEFT JOIN item_projeto ip ON (po.IdItemProjeto = ip.IdItemProjeto) 
                    LEFT JOIN material_item_projeto mip ON (ip.IdItemProjeto = mip.IdItemProjeto)
                    LEFT JOIN material_projeto_modelo mpm ON (mip.IdMaterProjMod = mpm.IdMaterProjMod)
                    LEFT JOIN projeto_modelo pm ON (mpm.IdProjetoModelo = pm.IdProjetoModelo)
                    LEFT JOIN grupo_modelo gm ON (pm.IdGrupoModelo = gm.IdGrupoModelo)
	                LEFT JOIN subgrupo_prod sp ON (p.IdSubgrupoProd = sp.IdSubgrupoProd)
                    LEFT JOIN grupo_prod gp ON (p.IdGrupoProd = gp.IdGrupoProd)
                    LEFT JOIN peca_otimizada pot ON (po.IdProd = pot.IdProdOrcamento)
                WHERE COALESCE(sp.TipoCalculo, gp.TipoCalculo) IN ({0})
                    AND gp.IdGrupoProd={1}
	                AND po.IdOrcamento = {2}
                GROUP BY po.IdProd";

            var tipoCalc = (int)TipoCalculoGrupoProd.Perimetro + "," + (int)TipoCalculoGrupoProd.ML + "," + (int)TipoCalculoGrupoProd.MLAL0 + "," + (int)TipoCalculoGrupoProd.MLAL05 + "," +
                (int)TipoCalculoGrupoProd.MLAL1 + "," + (int)TipoCalculoGrupoProd.MLAL6;

            sql = string.Format(sql, tipoCalc, (int)NomeGrupoProd.Alumínio, idOrcamento);

            return objPersistence.LoadData(sql);
        }

        #endregion
    }
}
