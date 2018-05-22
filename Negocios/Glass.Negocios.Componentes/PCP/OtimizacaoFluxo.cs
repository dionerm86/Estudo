using System;
using System.Collections.Generic;
using Glass.PCP.Negocios.Entidades;
using Colosoft;
using System.Linq;
using GNCutter32;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negocios das otimizações do sistema
    /// </summary>
    public class OtimizacaoFluxo : IOtimizacaoFluxo
    {
        /// <summary>
        /// Pesquisa as otmizações do sistema
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Otimizacao> PesquisarOtimizacoes()
        {
            return SourceContext.Instance.CreateQuery()
                 .From<Glass.Data.Model.Otimizacao>()
                 .OrderBy("IdOtimizacao desc")
                 .ToVirtualResultLazy<Entidades.Otimizacao>();
        }

        /// <summary>
        /// Recupera os dados da otmização.
        /// </summary>
        /// <param name="idOtimizacao"></param>
        /// <returns></returns>
        public Entidades.Otimizacao ObterOtimizacao(int idOtimizacao)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.Data.Model.Otimizacao>()
               .Where("IdOtimizacao=?id")
               .Add("?id", idOtimizacao)
               .ProcessLazyResult<Entidades.Otimizacao>()
               .FirstOrDefault();
        }

        /// <summary>
        /// Gera a otimização linear (Alumínios)
        /// </summary>
        /// <param name="lstProdPed"></param>
        /// <param name="lstIdProd"></param>
        /// <param name="lstComprimento"></param>
        /// <param name="lstGrau"></param>
        /// <param name="projEsquadria"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult GerarOtimizacaoLinear(int[] lstProdPed, int[] lstProdOrca, int[] lstIdProd, decimal[] lstComprimento, int[] lstGrau, bool projEsquadria)
        {
            var otimizacao = SourceContext.Instance.Create<Entidades.Otimizacao>();
            otimizacao.Tipo = Glass.Data.Model.TipoOtimizacao.Aluminio;

            if (lstProdPed.IsNullOrEmpty() && lstProdOrca.IsNullOrEmpty())
                return new Colosoft.Business.SaveResult(false, "Falha ao recuperar os produtos a serem otimizados".GetFormatter());

            //Cria a lista de peças a serem otimizadas
            var lstPecaOtmizada = new List<PecaOtimizada>();
            for (int i = 0; i < lstProdPed.Length; i++)
            {
                if (lstProdPed[0] == 0)
                {
                    lstProdPed= new int[0];
                    continue;
                }

                var peca = SourceContext.Instance.Create<PecaOtimizada>();
                peca.IdProdPed = lstProdPed[i];
                peca.Comprimento = lstComprimento[i] * 1000; //Converte para mm
                peca.GrauCorte = (Glass.Data.Model.GrauCorteEnum)lstGrau[i];

                lstPecaOtmizada.Add(peca);
            }

            for (int i = 0; i < lstProdOrca.Length; i++)
            {
                if (lstProdOrca[0] == 0)
                    continue;

                var peca = SourceContext.Instance.Create<PecaOtimizada>();
                peca.IdProdOrcamento = lstProdOrca[i];
                peca.Comprimento = lstComprimento[lstProdPed.Length+i] * 1000; //Converte para mm
                peca.GrauCorte = (Glass.Data.Model.GrauCorteEnum)lstGrau[lstProdPed.Length+i];

                lstPecaOtmizada.Add(peca);
            }

            //Agrupa as peças por produto
            var dados = lstPecaOtmizada.GroupBy(f => f.IdProd);

            //Percorre as peças agrupadas para gerar uma otimização por produto
            foreach (var d in dados)
            {
                //Cria a instancia do otimizador
                var cEngine = ObterCortador(Glass.Data.Model.TipoOtimizacao.Aluminio);

                //Esquadria
                if (projEsquadria)
                {
                    double arestaEsquadria = (double)Configuracoes.PCPConfig.ArestaBarraAluminioOtimizacao / 2;
                    cEngine.TrimLeft = arestaEsquadria;
                    cEngine.TrimRight = arestaEsquadria;
                }

                //Adiciona a materia-prima
                cEngine.AddLinearStock(6000, d.Count());

                //Adiciona as peças
                var pecas = new List<PecaOtimizada>();
                foreach (var peca in d)
                {
                    //Comprimento usado no corte
                    // Soma aresta do corte e o acrescimo da peça caso for instalação de temperado
                    var comprimento = peca.Comprimento;
                    comprimento += peca.GrauCorte == Glass.Data.Model.GrauCorteEnum.H9090 || peca.GrauCorte == Glass.Data.Model.GrauCorteEnum.L9090 ?
                    Configuracoes.PCPConfig.ArestaGrau90AluminioOtimizacao : Configuracoes.PCPConfig.ArestaGrau45AluminioOtimizacao;

                    if (!projEsquadria)
                        comprimento += Configuracoes.PCPConfig.AcrescimoBarraAluminioOtimizacaoProjetoTemperado;

                    cEngine.AddLinearPart((double)comprimento, 1);
                    pecas.Add(peca);
                }

                //Executa a otimização
                ObterResultadoOtimizacaoLinear(cEngine, d.Key, pecas, otimizacao);
            }

            //Salva a otimização
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = otimizacao.Save(session);

                if (!resultado)
                    return resultado;

                resultado = session.Execute(false).ToSaveResult();

                if (!resultado)
                    return resultado;

                return new Colosoft.Business.SaveResult(true, otimizacao.IdOtimizacao.ToString().GetFormatter());
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Cria a engine para realizar os calculos de corte
        /// </summary>
        /// <param name="tipoOtimizacao"></param>
        /// <returns></returns>
        private CutEngine ObterCortador(Glass.Data.Model.TipoOtimizacao tipoOtimizacao)
        {
            var cEngine = new CutEngine();
            cEngine.Clear();

            cEngine.SetUserID("02E859FF25700400-2921571184");

            //Configs
            if (tipoOtimizacao == Glass.Data.Model.TipoOtimizacao.Aluminio)
            {
                //Define que o calculo podera usar mais tipos de layouts
                cEngine.UseLayoutMinimization = false;
                //
                cEngine.SetAutoCutDirection();
            }

            return cEngine;
        }

        /// <summary>
        /// Trata a mensagem de erro retornada na otimização
        /// </summary>
        /// <param name="erro"></param>
        /// <returns></returns>
        private string TrataErroOtimizacao(string erro)
        {
            if (erro.Contains("Could not place all parts"))
                erro = "Não foi possível colocar todas as peças." + Environment.NewLine + "Verifique a quantidade de chapas selecionadas.";
            else if (erro.Contains("Part cannot be placed because it exceeds the sheet size") || erro.Contains("Part  cannot be placed because it exceeds the sheet size"))
                erro = "Uma das peças não pode ser colocada porque excede o tamanho de uma das chapas.";
            else if(erro.Contains("Number of parts must be more than zero"))
                erro = "Número de peças deve ser superior à 0.";
            else if(erro.Contains("Size of stock sheet 0 is incorrect"))
                erro = "O estoque de chapas não pode ser 0.";
            else if(erro.Contains("There are no enough stocks to cut all parts. Add more stocks or use incomplete optimization"))
                erro = "Não há materia-prima suficientes para cortar todas as peças. Adicione mais materias-primas ou use otimização incompleta.";

            return erro;
        }

        /// <summary>
        /// Gera os resultados da otimização
        /// </summary>
        /// <param name="cEngine"></param>
        /// <param name="dados"></param>
        /// <param name="otimizacao"></param>
        /// <param name="dicPecas"></param>
        private void ObterResultadoOtimizacaoLinear(CutEngine cEngine, int idProd, List<PecaOtimizada> pecas, Entidades.Otimizacao otimizacao)
        {
            var result = cEngine.GuillotineSheet();

            if (!string.IsNullOrWhiteSpace(result))
                throw new Exception(TrataErroOtimizacao(result));

            int stockNo, stockCount, iPart, partIndex, tmp;
            double stockLen, partCount, length, x;
            bool active;

            //Percorre os layout gerados
            for (int iLayout = 0; iLayout < cEngine.LayoutCount; iLayout++)
            {
                // Obtem as informações do layout
                cEngine.GetLayoutInfo(iLayout, out stockNo, out stockCount);
                cEngine.GetLinearStockInfo(stockNo, out stockLen, out active);

                var layout = SourceContext.Instance.Create<LayoutPecaOtimizada>();

                layout.IdProd = idProd;
                layout.Qtde = stockCount;

                otimizacao.LayoutsOtimizacao.Add(layout);

                // Obtem a quantidade de peças desse layout
                partCount = cEngine.GetPartCountOnStock(stockNo);

                // Percorre as peças dentro do layout
                for (iPart = 0; iPart < partCount; iPart++)
                {
                    partIndex = cEngine.GetPartIndexOnStock(stockNo, iPart);
                    cEngine.GetResultLinearPart(partIndex, out tmp, out length, out x);

                    var peca = pecas[partIndex];

                    peca.PosicaoX = (decimal)x;
                    peca.Sobra = false;

                    layout.PecasOtimizadas.Add(peca);
                }

                // Percorre as sobras
                partCount = cEngine.GetWastePartCountOnStock(stockNo);
                for (iPart = 0; iPart < partCount; iPart++)
                {
                    partIndex = cEngine.GetWastePartIndexOnStock(stockNo, iPart);
                    cEngine.GetRemainingLinearPart(partIndex, out tmp, out length, out x);

                    var pecaSobra = SourceContext.Instance.Create<PecaOtimizada>();

                    pecaSobra.Sobra = true;
                    pecaSobra.PosicaoX = (decimal)x;
                    pecaSobra.Comprimento = (decimal)length;

                    layout.PecasOtimizadas.Add(pecaSobra);
                }
            }
        }

        #endregion
    }
}
