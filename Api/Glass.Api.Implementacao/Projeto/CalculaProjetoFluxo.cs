using System.Collections;
using System.Collections.Generic;
using Glass.Data.Model;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using System;
using Glass.Configuracoes;

namespace Glass.Api.Implementacao.Projeto
{
    public class CalculaProjetoFluxo : Glass.Api.Projeto.ICalculaProjetoFluxo
    {
        #region Projeto

        /// <summary>
        /// Cria novo projeto.
        /// </summary>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <param name="codPedido"></param>
        /// <returns></returns>
        public int CriarProjeto(int tipoEntrega, int idCliente, string codPedido)
        {
            // Cria nova instância do projeto.
            var projeto = new Glass.Data.Model.Projeto
            {
                ApenasVidro = true,
                TipoEntrega = tipoEntrega,
                IdCliente = (uint?)idCliente,
                PedCli = codPedido
            };

            return (int)Glass.Data.DAL.ProjetoDAO.Instance.Insert(projeto);
        }

        /// <summary>
        /// Atualiza o código do pedido.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idCliente"></param>
        /// <param name="codPedido"></param>
        public void AtualizarCodPedido(int idProjeto, int idCliente, string codPedido)
        {
            var projeto = Glass.Data.DAL.ProjetoDAO.Instance.GetElementByPrimaryKey(idProjeto);

            if (projeto == null)
                throw new Exception("Não foi encontrado projeto para o identificador " + idProjeto);

            projeto.PedCli = codPedido;
            Glass.Data.DAL.ProjetoDAO.Instance.Update(projeto);
        }

        /// <summary>
        /// Recupera um projeto baseado no identificador.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public Glass.Data.Model.Projeto ObterProjeto(int idProjeto)
        {
            return Glass.Data.DAL.ProjetoDAO.Instance.GetElementByPrimaryKey(idProjeto);
        }

        /// <summary>
        /// Obter os projetos do cliente autenticado.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idClient"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IProjetoDescritor> ObterProjetos(int idProjeto, int idClient, string dataIni, string dataFim, int startRow, int pageSize)
        {
            var resultado = Glass.Data.DAL.ProjetoDAO.Instance.GetList((uint)idProjeto, (uint)idClient, null, dataIni, dataFim, null, startRow, pageSize);

            var retorno = new List<Glass.Api.Projeto.IProjetoDescritor>();
            foreach(var result in resultado)
                retorno.Add(new Glass.Api.Implementacao.Projeto.ProjetoDescritor(result));

            return retorno;
        }

        /// <summary>
        /// Recupera valor total do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public string ObterValorProjeto(int idProjeto)
        {
            return Glass.Data.DAL.ProjetoDAO.Instance.GetTotalProjeto((uint)idProjeto).ToString("C");
        }

        /// <summary>
        /// Deleta o projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        public int DeletarProjeto(int idProjeto)
        {
            var projeto = Glass.Data.DAL.ProjetoDAO.Instance.GetElementByPrimaryKey(idProjeto);
            if (projeto.Situacao == 1)
                throw new Exception("Esse orçamento foi finalizado e não pode ser Apagado.");

            return Glass.Data.DAL.ProjetoDAO.Instance.DeleteByPrimaryKey(idProjeto);
        }

        #endregion

        #region Item Projeto

        /// <summary>
        /// Cria novo item de projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idProjetoModelo"></param>
        /// <param name="espessuraVidro"></param>
        /// <param name="idCorVidro"></param>
        /// <param name="idCorAluminio"></param>
        /// <param name="idCorFerragem"></param>
        /// <param name="apenasVidros"></param>
        /// <param name="medidaExata"></param>
        /// <returns></returns>
        public int CriarItemProjeto(int idProjeto, int idProjetoModelo, int? espessuraVidro, int idCorVidro, int idCorAluminio, int idCorFerragem, bool apenasVidros, bool medidaExata)
        {
            return (int)Glass.Data.DAL.ItemProjetoDAO.Instance.NovoItemProjetoVazio(null, (uint?)idProjeto, null, null,
                null, null, null, null, (uint)idProjetoModelo, espessuraVidro,
                (uint)idCorVidro, (uint)idCorAluminio, (uint)idCorFerragem, true, medidaExata, false).IdItemProjeto;
        }

        /// <summary>
        /// Recupera o item do projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public Glass.Api.Projeto.IItemProjeto ObterItemProjeto(int idItemProjeto)
        {
            var itemProjeto =  Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement((uint)idItemProjeto);
            return new Glass.Api.Implementacao.Projeto.ItemProjeto(itemProjeto);
        }

        /// <summary>
        /// Recupera os itens do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IItemProjetoDescritor> ObterItemsProjeto(int idProjeto)
        {
            return null;
        }

        /// <summary>
        /// Confirma item projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idProjetoModelo"></param>
        /// <param name="medidaExata"></param>
        /// <param name="ambiente"></param>
        /// <param name="espessuraVidro"></param>
        /// <param name="idCorVidro"></param>
        /// <param name="modelo"></param>
        /// <returns></returns>
        public Glass.Api.Projeto.ICalcModeloResultado ConfirmarItemProjeto(int idProjeto, int idItemProjeto, int idProjetoModelo, bool medidaExata, string ambiente, int espessuraVidro, int idCorVidro, Glass.Api.Projeto.ICalcModelo modelo)
        {
            var retornoValidacao = string.Empty;

            SalvarMedidaAreaInstalacao((Glass.Api.Implementacao.Projeto.CalcModelo)modelo);
            SalvarPecas((Glass.Api.Implementacao.Projeto.CalcModelo)modelo);

            Glass.Data.DAL.ItemProjetoDAO.Instance.AtualizaAmbiente((uint)idItemProjeto, ambiente);

            var projeto = Glass.Data.DAL.ProjetoDAO.Instance.GetElement((uint)idProjeto);

            var itemProjeto = Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement((uint)idItemProjeto);

            if (itemProjeto.EspessuraVidro != espessuraVidro)
            {
                itemProjeto.EspessuraVidro = espessuraVidro;
                Glass.Data.DAL.ItemProjetoDAO.Instance.Update(itemProjeto);
                itemProjeto = Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement((uint)idItemProjeto);
            }
            if(itemProjeto.IdCorVidro != idCorVidro)
            {
                Glass.Data.DAL.OrcamentoDAO.Instance.AlteraCorItens((uint)idItemProjeto, itemProjeto.IdOrcamento, itemProjeto.IdPedido, 
                   itemProjeto.IdPedidoEspelho, (uint)idCorVidro, 1, 1, itemProjeto.TipoEntrega, itemProjeto.IdCliente,(uint)idItemProjeto);
                itemProjeto = Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement((uint)idItemProjeto);
            }

            var projetoModelo = Glass.Data.DAL.ProjetoModeloDAO.Instance.GetElement((uint)idProjetoModelo);

            if(itemProjeto.MedidaExata != medidaExata)
            {
                itemProjeto.MedidaExata = medidaExata;
                Glass.Data.DAL.ItemProjetoDAO.Instance.Update(itemProjeto);
            }

            var lstPecaModelo = CalcMedidasPecas(null, itemProjeto, projetoModelo, false, true, true, out retornoValidacao, (Glass.Api.Implementacao.Projeto.CalcModelo)modelo);

            // Insere Peças na tabela peca_item_projeto
            Glass.Data.DAL.PecaItemProjetoDAO.Instance.InsertFromPecaModelo(itemProjeto, ref lstPecaModelo);

            // Insere Peças na tabela material_item_projeto
            Glass.Data.DAL.MaterialItemProjetoDAO.Instance.InserePecasVidro(null, projeto.IdCliente, projeto.TipoEntrega, itemProjeto, projetoModelo, lstPecaModelo);

            Glass.Data.DAL.ItemProjetoDAO.Instance.UpdateTotalItemProjeto((uint)idItemProjeto);

            Glass.Data.DAL.ProjetoDAO.Instance.UpdateTotalProjeto((uint)idProjeto);

            //uint idProd = Glass.Data.DAL.ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto((uint)idItemProjeto);
            //if (idProd > 0)
            //    Glass.Data.DAL.ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(idProd);

            //OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrcamento.Value);

            // Obter Valores
            var valorItemProjeto = ObterValorItemProjeto(idItemProjeto);
            var m2ItemProjeto = "";

            try
            {
                m2ItemProjeto = ObterM2ItemProjeto(idItemProjeto);
            }
            catch (Exception ex){ m2ItemProjeto = ex.Message; }

            var valorProjeto = ObterValorProjeto(idProjeto);

            var valorM2 = ObterValorm2ItemProjeto(idItemProjeto);

            return new Glass.Api.Implementacao.Projeto.CalcModeloResultado(idProjeto, idItemProjeto, valorProjeto, valorItemProjeto, m2ItemProjeto, valorM2);
        }

        /// <summary>
        /// Recupera o valor do item projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public string ObterValorItemProjeto(int idItemProjeto)
        {
            return Glass.Data.DAL.ItemProjetoDAO.Instance.GetTotalItemProjeto((uint)idItemProjeto).ToString("C"); 
        }

        /// <summary>
        /// Recupera o valor do m2 do projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public string ObterM2ItemProjeto(int idItemProjeto)
        {
            return Glass.Data.DAL.ItemProjetoDAO.Instance.GetM2VaoItemProjeto((uint)idItemProjeto).ToString() + "m²";
        }

        /// <summary>
        /// Recupera o valor do m2.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public string ObterValorm2ItemProjeto(int idItemProjeto)
        {
            var material = Glass.Data.DAL.MaterialItemProjetoDAO.Instance.GetList((uint)idItemProjeto, string.Empty, 0, 1);

            return material == null || material.Count() == 0 ? 0.ToString("C") : material.ElementAt(0).Valor.ToString("C");
        }

        /// <summary>
        /// Deleta o item projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public int DeletarItemProjeto(int idItemProjeto)
        {
            return Glass.Data.DAL.ItemProjetoDAO.Instance.DeleteByPrimaryKey((uint)idItemProjeto);
        }

        /// <summary>
        /// Recupera o resumo do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public Glass.Api.Projeto.IProjetoResumo ObterProjetoResumo(int idProjeto)
        {
            var projeto = Glass.Data.DAL.ProjetoDAO.Instance.GetElement((uint)idProjeto);
            var items = Glass.Data.DAL.ItemProjetoDAO.Instance.GetList((uint)idProjeto, null, 0, 0);

            return new Glass.Api.Implementacao.Projeto.ProjetoResumo(projeto, items);
        }

        #region Private Methods

        /// <summary>
        /// Calcula Medidas das peças do projeto e depois insere os cálculos na tabela
        /// </summary>
        private List<PecaProjetoModelo> CalcMedidasPecas(GDA.GDASession sessao, Glass.Data.Model.ItemProjeto itemProj,
            Glass.Data.Model.ProjetoModelo projModelo, bool calcPecaAuto, bool pcp, bool medidasAlteradas, out string retornoValidacao, Glass.Api.Implementacao.Projeto.CalcModelo calcModelo)
        {
            retornoValidacao = string.Empty;

            // Carrega a lista de medidas do modelo de projeto
            List<MedidaProjetoModelo> lstMedProjMod = Glass.Data.DAL.MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(sessao, itemProj.IdProjetoModelo, true);

            var isBoxPadrao = Glass.Data.DAL.ProjetoModeloDAO.Instance.IsBoxPadrao(sessao, projModelo.IdProjetoModelo);

            var qtd = 0;
            var largVao = 0;
            var altVao = 0;
            var largPorta = 0;
            var largVaoEsq = 0;
            var largVaoDir = 0;


            Glass.Data.Helper.UtilsProjeto.BuscarMedidasVaoItemProjeto(sessao, itemProj, isBoxPadrao, out qtd, out largVao, out altVao,
                out largPorta, out largVaoEsq, out largVaoDir);

            // Recupera a quantidade da tabela.
            var medidaQtd = new MedidaItemProjeto();
            // TextBox txtQtd = ((TextBox)tbPecaModelo.FindControl("txtQtdMedInst"));
            medidaQtd.IdItemProjeto = itemProj.IdItemProjeto;
            medidaQtd.IdMedidaProjeto = 1;

            EditableItemValued<MedidaProjetoModelo> mQtd = calcModelo.Medidas.Where(f => f.Item.DescrMedida == "Qtd").FirstOrDefault();
            medidaQtd.Valor = mQtd == null && mQtd.Value < 1 ? 1 : mQtd.Value;

            itemProj.Qtde = medidaQtd.Valor;

            // Busca as peças deste item, que serão utilizadas nas expressões
            List<PecaItemProjeto> lstPecaItemProj = Glass.Data.DAL.PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, projModelo.IdProjetoModelo);

            var pecas = Glass.Data.DAL.PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo); 

            // Busca as peças com as medidas e produtos inseridos na tela
            List<PecaProjetoModelo> lstPecas = calcModelo.Pecas.Select(f => new PecaProjetoModelo
            {
                IdPecaProjMod = f.Item.IdPecaProjMod,
                IdPecaItemProj = f.Item.IdPecaItemProj,
                IdProd = f.Item.IdProd.Value,
                Qtde = f.Item.Qtde,
                Obs = f.Item.Obs,
                Largura = f.Item.Largura,
                Altura = f.Item.Altura,
                CalculoAltura = pecas.Where(p => p.IdPecaProjMod == f.Item.IdPecaProjMod).FirstOrDefault().CalculoAltura,
                CalculoLargura = pecas.Where(p => p.IdPecaProjMod == f.Item.IdPecaProjMod).FirstOrDefault().CalculoLargura,
                CalculoQtde = pecas.Where(p => p.IdPecaProjMod == f.Item.IdPecaProjMod).FirstOrDefault().CalculoQtde
            }).ToList();

            //= Glass.Data.Helper.UtilsProjeto.GetPecasFromTable(tbPecaModelo, pcp);

            // Chamado 15837: Se alguma das peças tiver beneficiamento, não permite que as mesmas sejam trocadas
            // Chamado 16671: Se for projeto de box, uma das peças pode ser diferente da outra (fixo e instalação) para resolver foi criada
            // a validação lstPecaItemProj[i].IdProd != lstPecas[i].IdProd
            //for (int i = 0; i < lstPecaItemProj.Count; i++)
            //    foreach (var pecaNova in lstPecas)
            //        if (lstPecaItemProj[i].IdProd != pecaNova.IdProd && lstPecaItemProj[i].IdProd != lstPecas[i].IdProd)
            //        {
            //            var mater = Glass.Data.DAL.MaterialItemProjetoDAO.Instance.GetMaterialByPeca(lstPecaItemProj[i].IdPecaItemProj);

            //            if (mater != null && mater.Beneficiamentos.Count > 0)
            //                throw new Exception("Não é permitido trocar peças que possuam beneficiamentos associados.");
            //        }

            // Se for para calcular as medidas das peças de vidro automaticamente
            if ((calcPecaAuto || medidasAlteradas) && !itemProj.MedidaExata)
            {
                // Busca as peças do modelo cadastrados por padrão para inserir qtd,altura e largura padrão, 
                // e não as medidas inseridas na tela, isto e feito para manter as peças de vidro selecionadas
                List<PecaProjetoModelo> lstPecasModelo = Glass.Data.DAL.PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo);
                for (int i = 0; i < lstPecas.Count; i++)
                {
                    // Verifica se há fórmula para calcular a qtd de peças
                    int qtdPeca = !System.String.IsNullOrEmpty(lstPecasModelo[i].CalculoQtde) ?
                        (int)Glass.Data.Helper.UtilsProjeto.CalcExpressao(sessao, lstPecasModelo[i].CalculoQtde, itemProj, null, lstMedProjMod) : lstPecasModelo[i].Qtde;

                    lstPecas[i].Qtde = qtdPeca;

                    lstPecas[i].Altura =
                        Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 6 ? lstPecasModelo[i].Altura06MM :
                            itemProj.EspessuraVidro == 8 ? lstPecasModelo[i].Altura08MM :
                            itemProj.EspessuraVidro == 10 ? lstPecasModelo[i].Altura10MM :
                            itemProj.EspessuraVidro == 12 ? lstPecasModelo[i].Altura12MM : lstPecasModelo[i].Altura) :
                            lstPecasModelo[i].Altura;

                    lstPecas[i].Largura =
                        Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 6 ? lstPecasModelo[i].Largura06MM :
                            itemProj.EspessuraVidro == 8 ? lstPecasModelo[i].Largura08MM :
                            itemProj.EspessuraVidro == 10 ? lstPecasModelo[i].Largura10MM :
                            itemProj.EspessuraVidro == 12 ? lstPecasModelo[i].Largura12MM : lstPecasModelo[i].Largura) :
                            lstPecasModelo[i].Largura;

                    lstPecas[i].IdAplicacao = lstPecasModelo[i].IdAplicacao;
                    lstPecas[i].IdProcesso = lstPecasModelo[i].IdProcesso;
                    lstPecas[i].Redondo = lstPecasModelo[i].Redondo;
                }

                // Se o projeto possuir espessura de tubo, a altura da última (e penúltima) peça deve ser subtraída deste valor
                if (projModelo.TipoMedidasInst == 8 || projModelo.TipoMedidasInst == 9)
                {
                    int espTuboMedInst = Glass.Data.DAL.MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, 16, false);

                    // Se este modelo tiver apenas 3 peças, sempre a última peça deverá ter sua altura subtraída da espessura do tubo, 
                    // mas se este modelo tiver 4 peças, as duas últimas peças terão suas alturas subtraídas da espessura do tubo
                    if (lstPecas.Count >= 3)
                        lstPecas[2].Altura -= espTuboMedInst;

                    if (lstPecas.Count == 4)
                        lstPecas[3].Altura -= espTuboMedInst;
                }

                // Pega a quantidade de peças
                int qtdPecas = 0;
                foreach (PecaProjetoModelo ppm in lstPecas)
                {
                    // Verifica se há fórmula para calcular a qtd de peças
                    int qtdPeca = !System.String.IsNullOrEmpty(ppm.CalculoQtde) ? (int)Glass.Data.Helper.UtilsProjeto.CalcExpressao(sessao, ppm.CalculoQtde, itemProj, null, lstMedProjMod) : ppm.Qtde;
                    qtdPecas += qtdPeca;
                }

                // Pega a quantidade de peças por projeto, considerando apenas um projeto
                qtdPecas /= qtd;

                #region Calcula as medidas das peças

                float larguraPecas = 0;
                foreach (PecaProjetoModelo ppm in lstPecas)
                {
                    #region Cálculo de Box Padrão

                    if (isBoxPadrao)
                    {
                        int qtdPecasBP = (largVaoDir > 0 ? qtdPecas / 2 : qtdPecas);

                        float largVaoOriginal = largVao > 0 ? largVao :
                        ppm.CalculoLargura.Contains("LARGVAODIR") ? largVaoDir : largVaoEsq;
                        float largVaoCalc = largVaoOriginal;

                        largVaoCalc = largVaoCalc % 100 == 0 ? largVaoCalc : largVaoCalc % 100 <= 20 ? (largVaoCalc - (largVaoCalc % 100) + 50) : largVaoCalc % 100 > 70 ? (largVaoCalc - (largVaoCalc % 100) + 150) : (largVaoCalc - (largVaoCalc % 100) + 100);

                        if (ppm.Largura == 50 && (largVaoCalc / qtdPecasBP) % 50 == 25)
                            ppm.Largura = 25;
                        else if (ppm.Largura == 0 && (largVaoCalc / qtdPecasBP) % 50 == 25)
                            ppm.Largura = -25;
                        else if (largVaoOriginal % 100 != 0)
                            ppm.Largura = 0;

                        if (qtdPecas % 2 == 1)
                        {
                            if (ppm.Largura > 0)
                                ppm.Largura -= (int)((largVaoCalc / qtdPecasBP) % 50);
                            else
                                ppm.Largura -= (int)((largVaoCalc / qtdPecasBP) % 50);
                        }

                        ppm.Altura += (int)Glass.Data.Helper.UtilsProjeto.CalcExpressao(sessao, ppm.CalculoAltura, itemProj, lstPecaItemProj, lstMedProjMod);
                        ppm.Largura += (int)(largVaoCalc / qtdPecasBP);

                        // Modificação necessária para que caso seja box de 4 peças e o total do vão dividido por 4
                        // não dê múltiplo de 50, o ajuste abaixo corrija esta situação
                        if (qtdPecas == 4)
                        {
                            if (ppm.Largura % 50 > 0)
                                if (ppm.Tipo == 1)
                                    ppm.Largura -= ppm.Largura % 50;
                                else
                                    ppm.Largura += 50 - (ppm.Largura % 50);
                        }

                        larguraPecas += ppm.Largura * (ppm.Qtde / qtd);
                        continue;
                    }

                    #endregion

                    ppm.Altura += (int)System.Math.Ceiling(Glass.Data.Helper.UtilsProjeto.CalcExpressao(sessao, ppm.CalculoAltura, itemProj, lstPecaItemProj, lstMedProjMod));
                    ppm.Largura += (int)System.Math.Ceiling(Glass.Data.Helper.UtilsProjeto.CalcExpressao(sessao, ppm.CalculoLargura, itemProj, lstPecaItemProj, lstMedProjMod));
                }

                #region Ajuste box padrão 3 ou 4 peças

                // Ajusta a largura da porta de um box padrão de 3 peças de 50 em 50 até ficar maior que a largura do vão
                if (isBoxPadrao && qtdPecas == 3)
                {
                    if ((larguraPecas - largVao) < 30)
                    {
                        while ((larguraPecas - largVao) < 30)
                            foreach (PecaProjetoModelo ppm in lstPecas)
                                if (ppm.Tipo == 1)
                                {
                                    ppm.Largura += 50;
                                    larguraPecas += 50;
                                }
                    }
                    else if ((larguraPecas - largVao) > 75)
                    {
                        while ((larguraPecas - largVao) > 75)
                            foreach (PecaProjetoModelo ppm in lstPecas)
                                if (ppm.Tipo == 1)
                                {
                                    ppm.Largura -= 50;
                                    larguraPecas -= 50;
                                }
                    }
                }

                // Calcula o transpasse de box 4 folhas
                int transpasse = 0;
                if (larguraPecas != largVao && qtdPecas == 4)
                    transpasse = 50;

                // Modifica a largura do vão a fim de calcular corretamente o transpasse das peças móveis
                if (largVao % 50 != 0)
                    largVao = largVao + (50 - (largVao % 50));
                else
                    largVao += 50;

                // Ajusta a largura da porta de um box padrão de 4 peças de 50 em 50 até ficar maior que a largura do vão
                if (isBoxPadrao && qtdPecas == 4 && ((larguraPecas - transpasse) < largVao || larguraPecas == largVao))
                {
                    do
                    {
                        foreach (PecaProjetoModelo ppm in lstPecas)
                            if (ppm.Tipo == 1)
                            {
                                ppm.Largura += 50;
                                larguraPecas += 50;
                            }
                    }
                    while (larguraPecas + transpasse <= largVao);
                }

                // Se a largura do fixo for maior que a da porta, inverte, para que o transpasse fique na porta
                if (isBoxPadrao && qtdPecas == 4 && lstPecas.Count == 2)
                {
                    if ((lstPecas[0].Tipo == 1 && lstPecas[0].Largura < lstPecas[1].Largura) ||
                        (lstPecas[1].Tipo == 1 && lstPecas[0].Largura > lstPecas[1].Largura))
                    {
                        int larguraTemp = lstPecas[0].Largura;
                        lstPecas[0].Largura = lstPecas[1].Largura;
                        lstPecas[1].Largura = larguraTemp;
                    }
                }

                #endregion

                #endregion
            }
            else if (itemProj.MedidaExata)
            {
                // Busca as peças do modelo cadastrados para atribuir processo e aplicação
                List<PecaProjetoModelo> lstPecasModelo = Glass.Data.DAL.PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo);

                for (int i = 0; i < lstPecas.Count; i++)
                {
                    lstPecas[i].IdAplicacao = lstPecasModelo[i].IdAplicacao;
                    lstPecas[i].IdProcesso = lstPecasModelo[i].IdProcesso;
                }
            }

            Glass.Data.Helper.UtilsProjeto.ValidarMedidasPecas(sessao, itemProj, lstPecas, lstMedProjMod, out retornoValidacao);

            return lstPecas;
        }


        /// <summary>
        /// Método responsável por salvar a medida da area de instalação.
        /// </summary>
        /// <param name="calcModelo"></param>
        private void SalvarMedidaAreaInstalacao(Glass.Api.Implementacao.Projeto.CalcModelo calcModelo)
        {
            //Remove medidas para esse item de projeto.
            Glass.Data.DAL.MedidaItemProjetoDAO.Instance.DeleteByItemProjeto((uint)calcModelo.IdItemProjeto);

            // Insere os valores de medida de instalação.
            foreach (var mpm in calcModelo.Medidas.FindAll(f=> f.IsEditable))
                    Glass.Data.DAL.MedidaItemProjetoDAO.Instance.InsereMedida((uint)calcModelo.IdItemProjeto, (uint)mpm.Item.IdMedidaProjeto, mpm.Value);
        }

        /// <summary>
        /// Método responsável por salvar a medida das peças.
        /// </summary>
        /// <param name="calcModelo"></param>
        private void SalvarPecas(Glass.Api.Implementacao.Projeto.CalcModelo calcModelo)
        {
            if (!calcModelo.MedidaExata)
                return;


        }

        #endregion

        #endregion

        #region Monta Modelo

        /// <summary>
        /// Monta o modelo.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idProjetoModelo"></param>
        /// <param name="medidaExata"></param>
        /// <returns></returns>
        public Glass.Api.Projeto.IModelo MontaModelo(int idProjeto, int idItemProjeto, int idProjetoModelo, bool medidaExata)
        {
            // Recupera item projeto.
            Glass.Data.Model.ItemProjeto itemProj = Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement((uint)idItemProjeto);

            // Recupera modelo.
            Glass.Data.Model.ProjetoModelo projetoModelo = Glass.Data.DAL.ProjetoModeloDAO.Instance.GetElementByPrimaryKey(idProjetoModelo);

            // Obtém as medidas.
            var medidas = ObterMedidasAreaInstalacao(itemProj, projetoModelo, medidaExata);

            var pecas = ObterPecas(itemProj);

            return new Modelo(medidas, pecas, itemProj.ImagemUrl.Replace("../../", ServiceLocator.Current.GetInstance<Api.IConfiguracao>().EnderecoServicoImagem));
        }

        #region Private Methods

        /// <summary>
        /// Obter medidas da área de instalação.
        /// </summary>
        /// <param name="itemProj"></param>
        /// <param name="projetoModelo"></param>
        /// <param name="medidaExata"></param>
        /// <returns></returns>
        private List<Glass.Api.Projeto.IEditableItemValued> ObterMedidasAreaInstalacao(Glass.Data.Model.ItemProjeto itemProj, Glass.Data.Model.ProjetoModelo projetoModelo, bool medidaExata)
        {
            List<MedidaProjetoModelo> medidasProjModel = Glass.Data.DAL.MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(projetoModelo.IdProjetoModelo, false);

            var retorno = new List<Glass.Api.Projeto.IEditableItemValued>();

            foreach (var mpm in medidasProjModel)
            {
                bool enable = true;
                int valorMedida = Glass.Data.DAL.MedidaItemProjetoDAO.Instance.GetByItemProjeto(itemProj.IdItemProjeto, mpm.IdMedidaProjeto, false);
                if (valorMedida == 0)
                    valorMedida = Glass.Data.DAL.MedidaProjetoDAO.Instance.ObtemValorPadrao(mpm.IdMedidaProjeto);

                if (medidaExata)
                    enable = Glass.Data.DAL.MedidaProjetoDAO.Instance.ExibirCalcMedidaExata(mpm.IdMedidaProjeto);

                // pegar o valor
                if (Data.DAL.ProjetoModeloDAO.Instance.IsBoxPadrao(projetoModelo.IdProjetoModelo) && mpm.IdMedidaProjeto == 3)
                    retorno.Add(new EditableItemValued<MedidaProjetoModelo>(mpm, itemProj.EspessuraVidro == 6 ? ProjetoConfig.AlturaPadraoProjetoBox6mm : ProjetoConfig.AlturaPadraoProjetoBoxAcima6mm, false));
                else
                    retorno.Add(new EditableItemValued<MedidaProjetoModelo>(mpm, valorMedida, enable));
            }

            if (projetoModelo.EixoPuxador)
            {
                int valorDistEixoPux = 0;
                if (projetoModelo.TipoMedidasInst > 0)
                    valorDistEixoPux = Glass.Data.DAL.MedidaItemProjetoDAO.Instance.GetByItemProjeto(itemProj.IdItemProjeto, 17, false);

                if (valorDistEixoPux == 0)
                    valorDistEixoPux = 50;

                retorno.Add(new EditableItemValued<MedidaProjetoModelo>(
                    new MedidaProjetoModelo() { DescrMedida = "Dist. Eixo Puxador", IdMedidaProjeto = 17 },
                    valorDistEixoPux, true));
            }

            return retorno;
        }

        /// <summary>
        /// Recupera as peças.
        /// </summary>
        /// <param name="itemProj"></param>
        /// <returns></returns>
        private List<Glass.Api.Projeto.IEditableItemValued> ObterPecas(Glass.Data.Model.ItemProjeto itemProj)
        {
            var retorno = new List<Glass.Api.Projeto.IEditableItemValued>();

            var pConfig = Glass.Data.DAL.PecaProjetoModeloDAO.Instance.GetByModelo(null, itemProj.IdProjetoModelo);
            
            List<PecaItemProjeto> pecas = Glass.Data.DAL.PecaItemProjetoDAO.Instance.GetByItemProjeto(null, itemProj.IdItemProjeto, itemProj.IdProjetoModelo);

            foreach (var p in pecas)
                retorno.Add(new EditableItemValued<PecaItemProjeto>(p, 0, true));

            return retorno;
        }

        #endregion

        #endregion
    }
}
