using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public class ProdutoNfItemProjetoDAO : BaseDAO<ProdutoNfItemProjeto, ProdutoNfItemProjetoDAO>
    {
        //private ProdutoNfItemProjetoDAO() { }

        /// <summary>
        /// Apaga os registros a partir do produto NF.
        /// </summary>
        /// <param name="idProdNf"></param>
        public void DeleteByIdProdNf(uint idProdNf)
        {
            objPersistence.ExecuteCommand("delete from produto_nf_item_projeto where idProdNf=" + idProdNf);
        }

        /// <summary>
        /// Apaga os registros a partir do item de projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public void DeleteByIdItemProjeto(uint idItemProjeto)
        {
            objPersistence.ExecuteCommand("delete from produto_nf_item_projeto where idItemProjeto=" + idItemProjeto);
        }

        public void Inserir(GDASession sessao, uint idProdNf, string idsItemProjeto)
        {
            foreach (string id in idsItemProjeto.Split(','))
            {
                ProdutoNfItemProjeto item = new ProdutoNfItemProjeto
                {
                    IdProdNf = idProdNf,
                    IdItemProjeto = Glass.Conversoes.StrParaUint(id)
                };

                #region Gera o novo item de projeto se for "apenas vidros"

                if (ItemProjetoDAO.Instance.ApenasVidros(item.IdItemProjeto) && FiscalConfig.NotaFiscalConfig.UsarProdutoCestaSeApenasVidros)
                {
                    ItemProjeto ip = ItemProjetoDAO.Instance.GetElementByPrimaryKey(sessao, item.IdItemProjeto);
                    ProjetoModelo pm = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(sessao, ip.IdProjetoModelo);

                    uint? idCliente = ip.IdCliente;
                    int? tipoEntrega = ip.TipoEntrega;

                    uint? idObra = ip.IdPedido > 0 ? PedidoDAO.Instance.GetIdObra(ip.IdPedido.Value) :
                        ip.IdPedidoEspelho > 0 ? PedidoDAO.Instance.GetIdObra(ip.IdPedidoEspelho.Value) :
                        null;

                    ip.IdOrcamento = null;
                    ip.IdPedido = null;
                    ip.IdPedidoEspelho = null;
                    ip.IdProjeto = null;

                    ip.ApenasVidros = false;
                    ip.IdCorAluminio = (uint)CorAluminioDAO.Instance.GetAll().FirstOrDefault().IdCorAluminio;
                    ip.IdCorFerragem = (uint)CorFerragemDAO.Instance.GetAll().FirstOrDefault().IdCorFerragem;

                    List<PecaItemProjeto> pecasItemProjeto = PecaItemProjetoDAO.Instance.GetByItemProjeto(item.IdItemProjeto, ip.IdProjetoModelo);

                    // Corrige projetos de medida exata, calculando área do vão
                    // Segundo Sidinei, usar altura da peça item "1" e largura somada de todas
                    // as outras peças
                    bool medidaExata = ip.MedidaExata;
                    var medidas = ((List<MedidaItemProjeto>)MedidaItemProjetoDAO.Instance.GetListByItemProjeto(item.IdItemProjeto)).ToArray();

                    if (medidaExata)
                    {
                        ip.MedidaExata = false;
                        int altura = 0, largura = 0;

                        if (pecasItemProjeto.Count > 0)
                        {
                            PecaItemProjeto p = pecasItemProjeto.Find(x =>
                            {
                                string[] itens = UtilsProjeto.GetItensFromPeca(x.Item);
                                return Array.Exists(itens, y => y == "1");
                            });

                            altura = p != null ? p.Altura : pecasItemProjeto[0].Altura;
                            pecasItemProjeto.ForEach(x => largura += x.Largura);
                        }

                        ip.M2Vao = Glass.Global.CalculosFluxo.ArredondaM2(largura, altura, ip.Qtde, 0, false);

                        #region Corrige as medidas do projeto

                        // Qtde
                        int index = Array.FindIndex(medidas, x => x.IdMedidaProjeto == 1);
                        if (index > -1)
                            medidas[index].Valor = ip.Qtde;

                        // Largura
                        index = Array.FindIndex(medidas, x => x.IdMedidaProjeto == 2);
                        if (index > -1)
                            medidas[index].Valor = largura;

                        // Altura
                        index = Array.FindIndex(medidas, x => x.IdMedidaProjeto == 3);
                        if (index > -1)
                            medidas[index].Valor = altura;

                        #endregion
                    }

                    ip.IdItemProjeto = ItemProjetoDAO.Instance.Insert(sessao, ip);

                    foreach (MedidaItemProjeto m in medidas)
                        MedidaItemProjetoDAO.Instance.InsereMedida(sessao, ip.IdItemProjeto, m.IdMedidaProjeto, m.Valor);

                    List<PecaProjetoModelo> pecasModelo = PecaProjetoModeloDAO.Instance.GetByModelo(pm.IdProjetoModelo);
                    
                    foreach (PecaItemProjeto p in pecasItemProjeto)
                    {
                        pecasModelo.Find(x => x.IdPecaProjMod == p.IdPecaProjMod).IdProd = p.IdProd.GetValueOrDefault();
                        pecasModelo.Find(x => x.IdPecaProjMod == p.IdPecaProjMod).Qtde = p.Qtde;

                        p.Beneficiamentos = p.Beneficiamentos;
                        p.IdPecaItemProj = 0;
                        p.IdItemProjeto = ip.IdItemProjeto;

                        // Utiliza apenas peças com qtd maior que zero    
                        if (p.Qtde > 0)
                            PecaItemProjetoDAO.Instance.Insert(sessao, p);
                    }

                    // Calcula os materiais do projeto, muda para medida exata para que a altura e largura dos materiais tenham
                    // valor, caso contrário apenas as folgas serão salvas nos campos altura e largura
                    ip.MedidaExata = true;
                    MaterialItemProjetoDAO.Instance.InserePecasVidro(sessao, idObra, idCliente, tipoEntrega, ip, pm, pecasModelo.Where(f => f.Qtde > 0).ToList(), true);
                    ip.MedidaExata = false;

                    foreach (MaterialItemProjeto m in ItemProjetoDAO.Instance.CalculaMateriais(sessao, ip, idCliente, tipoEntrega, true))
                        MaterialItemProjetoDAO.Instance.InsertFromNovoItemProjeto(sessao, m);

                    item.IdItemProjeto = ip.IdItemProjeto;
                }

                #endregion

                Insert(sessao, item);
            }
        }

        public bool ProdNfCadastrado(uint idProdNf)
        {
            return ProdNfCadastrado(null, idProdNf);
        }

        public bool ProdNfCadastrado(GDASession session, uint idProdNf)
        {
            return objPersistence.ExecuteSqlQueryCount(session, @"select count(*) from produto_nf_item_projeto
                where idProdNf=" + idProdNf) > 0;
        }

        public IList<ProdutoNfItemProjeto> GetByIdProdNf(uint idProdNf)
        {
            return GetByIdProdNf(null, idProdNf);
        }

        public IList<ProdutoNfItemProjeto> GetByIdProdNf(GDASession session, uint idProdNf)
        {
            return objPersistence.LoadData(session, @"select * from produto_nf_item_projeto 
                where idProdNf=" + idProdNf).ToList();
        }

        public ProdutosNf[] GetAsProdNf(uint idProdNf)
        {
            return GetAsProdNf(null, idProdNf);
        }

        public ProdutosNf[] GetAsProdNf(GDASession session, uint idProdNf)
        {
            ProdutosNf orig = ProdutosNfDAO.Instance.GetElementByPrimaryKey(session, idProdNf);

            uint idLojaNf = NotaFiscalDAO.Instance.ObtemIdLoja(session, orig.IdNf);
            uint? idCliNf = NotaFiscalDAO.Instance.ObtemIdCliente(session, orig.IdNf);
            uint? idFornecNf = NotaFiscalDAO.Instance.ObtemIdFornec(session, orig.IdNf);
            var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(session, orig.IdNf);
            bool saida = tipoDocumento == (int)NotaFiscal.TipoDoc.Saída ||
                /* Chamado 32984 e 39660. */
                (tipoDocumento == (int)NotaFiscal.TipoDoc.Entrada &&
                CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(orig.IdNaturezaOperacao.Value)));

            List<ProdutosNf> retorno = new List<ProdutosNf>();

            foreach (ProdutoNfItemProjeto p in GetByIdProdNf(session, idProdNf))
                foreach (MaterialItemProjeto m in MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, p.IdItemProjeto))
                {
                    ProdutosNf pnf = new ProdutosNf
                    {
                        Altura = m.Altura,
                        CodCfop = orig.CodCfop,
                        CodCont = orig.CodCont,
                        CodCred = orig.CodCred,
                        CodExportador = orig.CodExportador,
                        CodInterno = m.CodInterno,
                        CodValorFiscal = orig.CodValorFiscal,
                        CodValorFiscalIPI = orig.CodValorFiscalIPI,
                        Csosn = orig.Csosn,
                        Cst = orig.Cst,
                        CstCofins = orig.CstCofins,
                        CstIpi = orig.CstIpi,
                        CstOrig = orig.CstOrig,
                        CstPis = orig.CstPis,
                        DataDesembaraco = orig.DataDesembaraco,
                        DataRegDocImp = orig.DataRegDocImp,
                        DescrProduto = m.DescrProduto,
                        DespAduaneira = orig.DespAduaneira,
                        Espessura = m.Espessura,
                        GTINProduto = orig.GTINProduto,
                        IdNaturezaOperacao = orig.IdNaturezaOperacao,
                        IdContaContabil = orig.IdContaContabil,
                        IdGrupoProd = m.IdGrupoProd,
                        IdNf = orig.IdNf,
                        IdProd = m.IdProd,
                        IdSubgrupoProd = m.IdSubgrupoProd,
                        IndNaturezaFrete = orig.IndNaturezaFrete,
                        InfAdic = orig.InfAdic,
                        Largura = m.Largura,
                        LocalDesembaraco = orig.LocalDesembaraco,
                        Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(session, (int)idProdNf, idLojaNf, (int?)idFornecNf, idCliNf, saida),
                        NaturezaBcCred = orig.NaturezaBcCred,
                        Ncm = ProdutoDAO.Instance.ObtemNcm(session, (int)m.IdProd, idLojaNf),
                        NumACDrawback = orig.NumACDrawback,
                        NumDocImp = orig.NumDocImp,
                        Obs = orig.Obs,
                        PercRedBcIcms = orig.PercRedBcIcms,
                        PercRedBcIcmsSt = orig.PercRedBcIcmsSt,
                        Peso = Utils.CalcPeso(session, (int)m.IdProd, m.Espessura, m.TotM, m.Qtde, m.Altura, true),
                        Qtde = m.Qtde,
                        QtdeTrib = m.Qtde,
                        TipoDocumentoImportacao = orig.TipoDocumentoImportacao,
                        Total = m.Total + m.ValorBenef,
                        TotM = m.TotM,
                        UfDesembaraco = orig.UfDesembaraco,
                        Unidade = ProdutoDAO.Instance.ObtemUnidadeMedida(session, (int)m.IdProd),
                        UnidadeTrib = ProdutoDAO.Instance.ObtemUnidadeMedidaTrib(session, m.IdProd),
                        ValorDesconto = Math.Round((m.Total + m.ValorBenef) / orig.Total * orig.ValorDesconto, 2),
                        ValorFrete = Math.Round((m.Total + m.ValorBenef) / orig.Total * orig.ValorFrete, 2),
                        ValorSeguro = Math.Round((m.Total + m.ValorBenef) / orig.Total * orig.ValorSeguro, 2),
                        ValorUnitario = m.Valor
                    };

                    retorno.Add(pnf);
                }

            ProdutosNfDAO.Instance.CalcImposto(session, ref retorno, false, false);
            retorno.ForEach(x => x.IdProdNf = idProdNf);

            return retorno.ToArray();
        }
    }
}
