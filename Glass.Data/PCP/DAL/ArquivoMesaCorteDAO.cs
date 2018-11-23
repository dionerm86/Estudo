using CalcEngine;
using CalcEngine.Text.Diff.DiffBuilder;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;
using Ionic.Utils.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;

namespace Glass.Data.DAL
{
    public sealed class ArquivoMesaCorteDAO : BaseDAO<ArquivoMesaCorte, ArquivoMesaCorteDAO>
    {
        //private ArquivoMesaCorteDAO() { }

        public ArquivoMesaCorte GetElement(uint idArquivoMesaCorte)
        {
            return GetElement(null, idArquivoMesaCorte);
        }

        public ArquivoMesaCorte GetElement(GDASession session, uint idArquivoMesaCorte)
        {
            var sql = @"
                SELECT amc.*, ac.nome AS Codigo FROM arquivo_mesa_corte amc
                    LEFT JOIN arquivo_calcengine ac ON (amc.idArquivoCalcEngine=ac.idArquivoCalcEngine)
                WHERE amc.IdArquivoMesaCorte={0}
                ";

            return objPersistence.LoadOneData(session, string.Format(sql, idArquivoMesaCorte));
        }

        public IList<ArquivoMesaCorte> GetOrdered()
        {
            var sql =
                @"SELECT amc.*, ac.nome AS Codigo FROM arquivo_mesa_corte amc
                    LEFT JOIN arquivo_calcengine ac ON (amc.idArquivoCalcEngine=ac.idArquivoCalcEngine)
                GROUP BY Codigo                
                ORDER BY Codigo";

            return objPersistence.LoadData(sql).ToList();
        }

        public ArquivoMesaCorte ObterPeloArquivoCalcEngine(GDASession session, uint idArquivoCalcEngine)
        {
            var sql = @"
                SELECT amc.*, ac.nome AS Codigo FROM arquivo_mesa_corte amc
                    LEFT JOIN arquivo_calcengine ac ON (amc.idArquivoCalcEngine=ac.idArquivoCalcEngine)
                WHERE amc.IdArquivoCalcEngine={0}
                ";

            return objPersistence.LoadOneData(session, string.Format(sql, idArquivoCalcEngine));
        }

        /// <summary>
        /// Retorna o Arquivo que será usado na mesa de corte para a peça passada
        /// </summary>
        public bool GetArquivoMesaCorte(GDASession session, uint idPedido, uint idProdPedEsp, uint? idSetor, ref uint? idArquivoMesaCorte,
            bool arquivoOtimizacao, Stream arquivo, ref int tipoArquivo, bool forSGlass, bool forIntermac)
        {
            // Retorna o arquivo enviado em caso de pedido importado
            if (PedidoDAO.Instance.IsPedidoImportado(session, idPedido))
            {
                var nomeArquivo = Utils.GetArquivoMesaCorteImpPath + idProdPedEsp + ".sag";
                if (File.Exists(nomeArquivo))
                {
                    idArquivoMesaCorte = 0;

                    using (FileStream f = File.OpenRead(nomeArquivo))
                    {
                        var buffer = new byte[1024];
                        var read = 0;
                        while ((read = f.Read(buffer, 0, buffer.Length)) > 0)
                            arquivo.Write(buffer, 0, read);

                        return true;
                    }
                }
            }

            var pecaItemProjeto = new PecaItemProjeto();
            var idMaterItemProj = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idMaterItemProj", "idProdPed=" + idProdPedEsp);
            var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + idProdPedEsp);
            var altura = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "if(alturaReal>0, alturaReal, altura)", "idProdPed=" + idProdPedEsp);
            var largura = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<int>(session, "if(larguraReal>0, larguraReal, largura)", "idProdPed=" + idProdPedEsp);
            var flags = new List<FlagArqMesa>();

            // Se a peça não possuir material associado, não há associação com o arquivo de mesa de corte vindo do projeto
            if (idMaterItemProj != null)
            {
                var alterouTipoArquivo = false;
                pecaItemProjeto = PecaItemProjetoDAO.Instance.GetByMaterial(session, idMaterItemProj.Value);

                if (pecaItemProjeto == null || pecaItemProjeto.Item == null)
                    return false;

                // Se o material não estiver associado à uma peça, não há associação com o arquivo de mesa de corte vindo do projeto
                if (pecaItemProjeto != null)
                {
                    int item;

                    /* Chamado 58078. */
                    if (!int.TryParse(pecaItemProjeto.Item, out item))
                        item = pecaItemProjeto.Item[0].ToString().StrParaInt();

                    var pecaProjMod = PecaProjetoModeloDAO.Instance.GetByItem(session, ItemProjetoDAO.Instance.GetIdProjetoModelo(session, pecaItemProjeto.IdItemProjeto), item);

                    // Verifica se esta peça possui arquivo de mesa corte, não há associação com o arquivo de mesa de corte vindo do projeto
                    if (pecaProjMod.IdArquivoMesaCorte != null)
                    {
                        idArquivoMesaCorte = pecaProjMod.IdArquivoMesaCorte;

                        if (tipoArquivo == 0)
                        {
                            tipoArquivo = pecaProjMod.TipoArquivo == null ? 0 : (int)pecaProjMod.TipoArquivo;
                            alterouTipoArquivo = true;
                        }
                    }

                    flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaProjMod.IdPecaProjMod, true);

                    if ((tipoArquivo == 0 || alterouTipoArquivo) && !arquivoOtimizacao && tipoArquivo == (int)TipoArquivoMesaCorte.SAG &&
                        flags.Any(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.DXF.ToString().ToLower() || f.Descricao.ToLower() == TipoArquivoMesaCorte.FML.ToString().ToLower()))
                    {
                        tipoArquivo = flags.Where(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.DXF.ToString().ToLower() || f.Descricao.ToLower() == TipoArquivoMesaCorte.FML.ToString().ToLower())
                            .Select(f => (int)((TipoArquivoMesaCorte)Enum.Parse(typeof(TipoArquivoMesaCorte), f.Descricao, true)))
                            .FirstOrDefault();
                    }
                }
            }

            tipoArquivo = tipoArquivo == 0 && idArquivoMesaCorte.GetValueOrDefault() > 0 ? ObtemTipoArquivo(session, idArquivoMesaCorte.Value) : tipoArquivo;
            var produtoPossuiImagemEditada = ProdutosPedidoEspelhoDAO.Instance.PossuiImagemAssociada(session, idProdPedEsp);
            var pecaPossuiFiguraAssociada = PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(session, pecaItemProjeto.IdPecaItemProj);
            // Chamado 74968.
            // A propriedade ImagemEditada é marcada como true somente ao salvar a edição do projeto, ou seja, caso o usuário somente abra a tela o sistema não deve considerar que a peça foi editada.
            var pecaPossuiEdicaoCadProject = ProdutosPedidoEspelhoDAO.Instance.PossuiEdicaoCadProject(idProdPedEsp, true) && pecaItemProjeto.ImagemEditada;

            // Se possuir imagem associada, não deve gerar arquivo de mesa, a menos que seja .fml básico ou tenha sido editado no CadProject
            if ((produtoPossuiImagemEditada || pecaItemProjeto.ImagemEditada || pecaPossuiFiguraAssociada) &&
                (idArquivoMesaCorte == null || tipoArquivo != (int)TipoArquivoMesaCorte.FMLBasico) && !pecaPossuiEdicaoCadProject)
            {
                if (idArquivoMesaCorte.GetValueOrDefault() > 0 && (tipoArquivo == (int)TipoArquivoMesaCorte.FML ||
                    tipoArquivo == (int)TipoArquivoMesaCorte.FMLBasico))
                    /* Chamado 44384.
                     * Após este momento, o tipo do arquivo não pode ser alterado, para que o FML básico seja gerado. */
                    tipoArquivo = (int)TipoArquivoMesaCorte.FMLBasico;
                else
                {
                    idArquivoMesaCorte = null;
                    return false;
                }
            }

            // Se não houver associação do arquivo de mesa de corte com o projeto, procura associação no produto
            if ((idArquivoMesaCorte <= 0 || idArquivoMesaCorte == null) && tipoArquivo != (int)TipoArquivoMesaCorte.FMLBasico)
            {
                var alterouTipoArquivo = false;

                idArquivoMesaCorte = ProdutoDAO.Instance.ObtemIdArquivoMesaCorte(session, idProd);

                if (idArquivoMesaCorte > 0)
                {
                    var tipoArquivoMesaCorteProduto = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(session, (int)idProd);

                    pecaItemProjeto.Altura = (int)altura;
                    pecaItemProjeto.Largura = largura;
                    tipoArquivo =
                        tipoArquivoMesaCorteProduto > 0 ? (int)tipoArquivoMesaCorteProduto :
                        PCPConfig.TipoArquivoMesaPadrao == "DXF" ? (int)TipoArquivoMesaCorte.DXF :
                        PCPConfig.TipoArquivoMesaPadrao == "FML" ? (int)TipoArquivoMesaCorte.FML :
                        (int)TipoArquivoMesaCorte.SAG;

                    //Busca as flags do produto
                    flags = FlagArqMesaDAO.Instance.ObtemPorProduto((int)idProd, true);

                    if ((tipoArquivo == 0 || alterouTipoArquivo) && !arquivoOtimizacao && tipoArquivo == (int)TipoArquivoMesaCorte.SAG &&
                        flags.Any(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.DXF.ToString().ToLower() || f.Descricao.ToLower() == TipoArquivoMesaCorte.FML.ToString().ToLower()))
                    {
                        tipoArquivo = flags.Where(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.DXF.ToString().ToLower() || f.Descricao.ToLower() == TipoArquivoMesaCorte.FML.ToString().ToLower())
                            .Select(f => (int)((TipoArquivoMesaCorte)Enum.Parse(typeof(TipoArquivoMesaCorte), f.Descricao, true)))
                            .FirstOrDefault();
                    }
                }
                else
                    return false;
            }
            
            tipoArquivo = tipoArquivo == 0 && idArquivoMesaCorte.GetValueOrDefault() > 0 ? ObtemTipoArquivo(session, idArquivoMesaCorte.Value) : tipoArquivo;
            var idArquivoCalcEngine = ObtemIdArquivoCalcEngine(session, idArquivoMesaCorte.Value);
            var tipoProjeto = ObtemTipoProjeto(session, idArquivoMesaCorte.Value);

            var espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)idProd);
            var codOtimizacao = ProdutoDAO.Instance.ObtemValorCampo<string>(session, "codOtimizacao", "idProd=" + idProd);
            
            // Busca o pedido e o projeto para identificar pedido/projeto com possível erro
            var codProjeto = pecaItemProjeto != null && pecaItemProjeto.IdItemProjeto > 0 ? ProjetoModeloDAO.Instance.ObtemCodigo(session,
                ItemProjetoDAO.Instance.GetIdProjetoModelo(session, pecaItemProjeto.IdItemProjeto)) : string.Empty;
            var mensagemErro = " Pedido: " + idPedido + (!string.IsNullOrEmpty(codProjeto) ? " Projeto: " + codProjeto : string.Empty) + ".";

            var codigoArquivo = ArquivoCalcEngineDAO.Instance.ObtemValorCampo<string>(session, "nome", "idArquivoCalcEngine=" + idArquivoCalcEngine);
            var conteudoArquivo = ObtemValorCampo<string>(session, "arquivo", "idArquivoMesaCorte=" + idArquivoMesaCorte);

            if (codigoArquivo == null)
                throw new Exception("Um dos arquivos de mesa está associado à um calc engine inválido.");
            
            var idsBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(session, idProdPedEsp).Select(f => (int)f.IdBenefConfig).ToList();
            var descricaoBeneficiamento = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(session, idProdPedEsp);
            uint idProcesso = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProcesso(session, idProdPedEsp);

            var etiquetaProcesso =  EtiquetaProcessoDAO.Instance.GetElementByPrimaryKey(idProcesso);
            if (etiquetaProcesso != null && !etiquetaProcesso.GerarArquivoDeMesa)
            {
                idArquivoMesaCorte = null;
                return false;
            }

            // Caso o processo exija a geração de SAG o sistema verifica se foi chamado da tela "imprimir etiqueta"
            if (etiquetaProcesso != null && etiquetaProcesso.ForcarGerarSag)
            {
                // Caso tenha sido chamado da tela imprimir etiqueta
                // Remove todas as flags e adiciona apenas a de SAG e define o tipo arqiivo como SAG
                if (arquivoOtimizacao)
                {
                    var flag = FlagArqMesaDAO.Instance.GetAll().Where(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.SAG.ToString().ToLower()).FirstOrDefault();

                    if ( flag != null && !flags.Contains(flag))
                        flags.Add(flag);

                    tipoArquivo = (int)TipoArquivoMesaCorte.SAG;
                }
                else
                {
                    idArquivoMesaCorte = null;
                    return false;
                }
            }

            var aumentoPeca = ImpressaoEtiquetaDAO.Instance.GetAresta(session, (int)idProd, idArquivoMesaCorte, idsBenef, descricaoBeneficiamento, (int)idProcesso);
            var descontoLap = ObterDescontoLapidacao(session, ref pecaItemProjeto, idArquivoMesaCorte, (int)idProd, idsBenef, descricaoBeneficiamento, (int)idProcesso);

            var config = new ConfiguracoesArqMesa(pecaItemProjeto.Largura, espessura, descontoLap, codigoArquivo);
            
            if (idArquivoCalcEngine > 0 &&
                ((tipoArquivo == (int)TipoArquivoMesaCorte.SAG && arquivoOtimizacao) ||
                (tipoArquivo != (int)TipoArquivoMesaCorte.SAG && tipoArquivo != (int)TipoArquivoMesaCorte.FMLBasico && !arquivoOtimizacao) ||

                /* Chamado 45059.
                 * Caso o tipo do arquivo seja FML básico e a peça tenha sido editada,
                 * o arquivo salvo na pasta Upload deve ser recuperado e o arquivo de marcação deve ser gerado a partir dele. */
                /* Chamado 52613.
                 * O arquivo não deve ser gerado caso a geração tenha sido solicitada através da tela de impressão de etiquetas. */
                (!arquivoOtimizacao && tipoArquivo == (int)TipoArquivoMesaCorte.FMLBasico && File.Exists(PCPConfig.CaminhoSalvarCadProject(true) + idProdPedEsp + ".dxf"))))
            {
                #region Arquivos CalcEngine

                bool? retorno = null;

                GerarArquivoCalcEngine(session, idArquivoCalcEngine, descontoLap, tipoArquivo, true, idProdPedEsp, pecaItemProjeto, altura, largura, ref mensagemErro, codigoArquivo, config,
                    espessura, arquivo, flags, ref retorno, false, forSGlass, forIntermac);

                if (retorno.HasValue)
                    return retorno.Value;

                #endregion
            }

            if (tipoArquivo == (int)TipoArquivoMesaCorte.SAG && conteudoArquivo != null)
            {
                #region Arquivos SAG

                // Parâmetros comuns
                conteudoArquivo = conteudoArquivo
                    .Replace("?TituloLargPeca?", Formatacoes.TrataValorDouble(pecaItemProjeto.Largura + aumentoPeca, 3))
                    .Replace("?TituloAltPeca?", Formatacoes.TrataValorDouble(pecaItemProjeto.Altura + aumentoPeca, 3))
                    .Replace("?LargPeca?", Formatacoes.TrataValorDouble(pecaItemProjeto.Largura + aumentoPeca, 6))
                    .Replace("?AltPeca?", Formatacoes.TrataValorDouble(pecaItemProjeto.Altura + aumentoPeca, 6))
                    .Replace("?Espessura?", Formatacoes.TrataValorDouble(espessura, 6))
                    .Replace("?CodMaterial?", codOtimizacao)
                    .Replace("?DescLap?", Formatacoes.TrataValorDouble(descontoLap, 6).ToString().Replace(',', '.'));

                if (!GerarArquivoSAG(altura, aumentoPeca, codigoArquivo, config, ref conteudoArquivo, descontoLap, largura, mensagemErro, pecaItemProjeto))
                    return false;
                
                #endregion
            }
            else if (tipoArquivo == (int)TipoArquivoMesaCorte.FORTxt)
            {
                #region Arquivos FOR txt

                conteudoArquivo = conteudoArquivo
                    .Replace("?Altura?", altura.ToString())
                    .Replace("?Largura?", largura.ToString())
                    .Replace("?Espessura?", espessura.ToString());

                if (!GerarArquivoFORTxt(altura, codigoArquivo, config, ref conteudoArquivo, espessura, largura, mensagemErro, pecaItemProjeto))
                    return false;
                
                #endregion
            }
            else if (tipoArquivo == (int)TipoArquivoMesaCorte.DXF && !arquivoOtimizacao)
            {
                #region Arquivos DXF

                CalcEngine.ProjectFilesPackage pacote = null;

                codigoArquivo = codigoArquivo.Contains("DXF_") || codigoArquivo.Contains("FML_") ? codigoArquivo : "DXF_" + codigoArquivo;

                using (System.IO.Stream pacoteStream = System.IO.File.OpenRead(Utils.GetArquivoDxfPath + codigoArquivo + ".calcpackage"))
                {
                    // Esse método deserializa os dados do pacote que estão contidos
                    // na Stream a recupera a instancia do pacote de configuração
                    pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                }

                // Lê a configuração do projeto
                var projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);
                var variaveis = new System.Collections.Generic.Dictionary<string, double>();

                // Altera o ReferenceValueProvider para buscar os Valores das Constantes da ferragem cadastrada no WebGlass
                projeto.ReferenceValueProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<CalcEngine.IReferenceValueProvider>();

                return GerarArquivoDXF(altura, arquivo, codigoArquivo, espessura, flags, idProdPedEsp, largura, mensagemErro, pecaItemProjeto, projeto, (TipoArquivoMesaCorte)tipoArquivo,
                    ref variaveis);

                #endregion
            }
            else if ((tipoArquivo == (int)TipoArquivoMesaCorte.FML || tipoArquivo == (int)TipoArquivoMesaCorte.FMLBasico) && !arquivoOtimizacao)
            {
                #region Arquivos FML

                CalcEngine.ProjectFilesPackage pacote = null;

                codigoArquivo = codigoArquivo.Contains("FML_") || codigoArquivo.Contains("DXF_") ? codigoArquivo : "FML_" + codigoArquivo;

                if (tipoArquivo == (int)TipoArquivoMesaCorte.FMLBasico)
                {
                    using (var stream = typeof(ArquivoMesaCorteDAO).Assembly.GetManifestResourceStream("Glass.Data.Resources.FML_Basico.calcpackage"))
                        pacote = CalcEngine.ProjectFilesPackage.LoadPackage(stream);
                }
                else
                    using (Stream pacoteStream = File.OpenRead(Utils.GetArquivoFmlPath + codigoArquivo + ".calcpackage"))
                    {
                        // Esse método deserializa os dados do pacote que estão contidos
                        // na Stream a recupera a instancia do pacote de configuração
                        pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                    }

                // Lê a configuração do projeto
                var projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);
                var variaveis = new Dictionary<string, double>();

                // Altera o ReferenceValueProvider para buscar os Valores das Constantes da ferragem cadastrada no WebGlass
                projeto.ReferenceValueProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<CalcEngine.IReferenceValueProvider>();

                return GerarArquivoFML(altura, arquivo, codigoArquivo, espessura, flags, idProdPedEsp, largura, mensagemErro, pecaItemProjeto, projeto, (TipoArquivoMesaCorte)tipoArquivo,
                    ref variaveis);

                #endregion
            }
            else
            {
                idArquivoMesaCorte = null;
                conteudoArquivo = string.Empty;
            }

            if (!string.IsNullOrEmpty(conteudoArquivo))
            {
                var writer = new System.IO.StreamWriter(arquivo);
                writer.Write(conteudoArquivo);
                writer.Flush();
                return true;
            }
            else
                return false;
        }

        #region Geração de arquivos por tipo

        #region SAG

        /// <summary>
        /// Monta o arquivo de marcação do tipo SAG, com base nos arquivos fixos no código.
        /// </summary>
        private static bool GerarArquivoSAG(float altura, float aumentoPeca, string codigoArquivo, ConfiguracoesArqMesa config, ref string conteudoArquivo, float descontoLap, int largura,
            string mensagemErro, PecaItemProjeto pecaItemProjeto)
        {
            #region Arquivos SAG
            
            #region Blindex

                if (codigoArquivo == "BASCULABLINDEX")
                {
                    #region BASCULABLINDEX

                    int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                    if (distBorda1523 == 0)
                        distBorda1523 = altura > 300 ? 50 : 100;

                    decimal raioRecorte3010 = 9;
                    decimal raioTrinco3004 = 9;
                    decimal recuo1123 = 10;

                    // 3010 Esquerda
                    decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                    decimal yRecEsq1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)11.438631);

                    decimal xRecEsq2 = xRecEsq1 + (decimal)23.449897;
                    decimal yRecEsq2 = yRecEsq1 - (decimal)2.488899;

                    decimal xRecEsq3 = xRecEsq2;
                    decimal yRecEsq3 = yRecEsq2 - (decimal)17.899464;
                    decimal iRecEsq3 = xRecEsq3 - (decimal)0.949897;
                    decimal jRecEsq3 = yRecEsq3 + (decimal)8.949732;

                    decimal xRecEsq4 = xRecEsq1;
                    decimal yRecEsq4 = yRecEsq3 - (decimal)2.488899;

                    // 3004 Trinco
                    decimal xTrinco1 = (largura - distBorda1523) + (decimal)descontoLap + (decimal)5.667213;
                    decimal yTrinco1 = (decimal)altura - ((decimal)descontoLap + (decimal)20.508384);

                    decimal xTrinco2 = xTrinco1;
                    decimal yTrinco2 = yTrinco1;
                    decimal iTrinco2 = xTrinco2 - (decimal)5.667213;
                    decimal jTrinco2 = yTrinco2 - (decimal)6.991616;

                    // 3010 Direita
                    decimal xRecDir1 = (largura - recuo1123) + (decimal)descontoLap;
                    decimal yRecDir1 = ((decimal)(altura / 2) + 50) + (decimal)descontoLap - (decimal)11.438631;

                    decimal xRecDir2 = xRecDir1 - (decimal)23.449897;
                    decimal yRecDir2 = yRecDir1 + (decimal)2.488899;

                    decimal xRecDir3 = xRecDir2;
                    decimal yRecDir3 = yRecDir2 + (decimal)17.899464;
                    decimal iRecDir3 = xRecDir3 + (decimal)0.949897;
                    decimal jRecDir3 = yRecDir3 - (decimal)8.949732;

                    decimal xRecDir4 = xRecDir1;
                    decimal yRecDir4 = yRecDir3 + (decimal)2.488899;

                    conteudoArquivo = conteudoArquivo
                        // Faz o recorte na 3010 da esquerda
                        .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                        .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                        .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                        .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                        .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                        .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                        .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                        .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                        .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte3010, 6))

                        .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                        .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))

                        // Faz o furo do trinco
                        .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                        .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))

                        .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                        .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                        .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                        .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                        .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(raioTrinco3004, 6))

                        // Faz o recorte na 3010 da direita
                        .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                        .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                        .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                        .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                        .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                        .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                        .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                        .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                        .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecorte3010, 6))

                        .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                        .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6));

                    #endregion
                }
                else if (codigoArquivo == "BASCULABLINDEXCENTRAL")
                {
                    #region BASCULABLINDEXCENTRAL

                    int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                    if (distBorda1523 == 0)
                        distBorda1523 = altura > 300 ? 50 : 100;

                    decimal raioRecorte3010 = 9;
                    decimal recuo1123 = 10;

                    // 3010 Esquerda
                    decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                    decimal yRecEsq1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)11.438631);

                    decimal xRecEsq2 = xRecEsq1 + (decimal)23.449897;
                    decimal yRecEsq2 = yRecEsq1 - (decimal)2.488899;

                    decimal xRecEsq3 = xRecEsq2;
                    decimal yRecEsq3 = yRecEsq2 - (decimal)17.899464;
                    decimal iRecEsq3 = xRecEsq3 - (decimal)0.949897;
                    decimal jRecEsq3 = yRecEsq3 + (decimal)8.949732;

                    decimal xRecEsq4 = xRecEsq1;
                    decimal yRecEsq4 = yRecEsq3 - (decimal)2.488899;

                    // 3010 Direita
                    decimal xRecDir1 = (largura - recuo1123) + (decimal)descontoLap;
                    decimal yRecDir1 = ((decimal)(altura / 2) + 50) + (decimal)descontoLap - (decimal)11.438631;

                    decimal xRecDir2 = xRecDir1 - (decimal)23.449897;
                    decimal yRecDir2 = yRecDir1 + (decimal)2.488899;

                    decimal xRecDir3 = xRecDir2;
                    decimal yRecDir3 = yRecDir2 + (decimal)17.899464;
                    decimal iRecDir3 = xRecDir3 + (decimal)0.949897;
                    decimal jRecDir3 = yRecDir3 - (decimal)8.949732;

                    decimal xRecDir4 = xRecDir1;
                    decimal yRecDir4 = yRecDir3 + (decimal)2.488899;

                    conteudoArquivo = conteudoArquivo
                        // Faz o recorte na 3010 da esquerda
                        .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                        .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                        .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                        .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                        .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                        .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                        .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                        .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                        .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte3010, 6))

                        .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                        .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))

                        // Faz o recorte na 3010 da direita
                        .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                        .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                        .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                        .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                        .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                        .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                        .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                        .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                        .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecorte3010, 6))

                        .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                        .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6));

                    #endregion
                }
                else if (codigoArquivo == "BASCULABLINDEXLATERAL")
                {
                    #region BASCULABLINDEXLATERAL

                    int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                    if (distBorda1523 == 0)
                        distBorda1523 = altura > 300 ? 50 : 100;

                    decimal raioRecorte3010 = 9;
                    decimal recuo1123 = 10;

                    // 3010 Esquerda
                    decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                    decimal yRecEsq1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)11.438631);

                    decimal xRecEsq2 = xRecEsq1 + (decimal)23.449897;
                    decimal yRecEsq2 = yRecEsq1 - (decimal)2.488899;

                    decimal xRecEsq3 = xRecEsq2;
                    decimal yRecEsq3 = yRecEsq2 - (decimal)17.899464;
                    decimal iRecEsq3 = xRecEsq3 - (decimal)0.949897;
                    decimal jRecEsq3 = yRecEsq3 + (decimal)8.949732;

                    decimal xRecEsq4 = xRecEsq1;
                    decimal yRecEsq4 = yRecEsq3 - (decimal)2.488899;

                    conteudoArquivo = conteudoArquivo
                        // Faz o recorte na 3010 da esquerda
                        .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                        .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                        .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                        .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                        .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                        .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                        .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                        .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                        .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte3010, 6))

                        .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                        .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6));

                    #endregion
                }
                else if (codigoArquivo == "BASCULABLINDEXSUPERIOR")
                {
                    #region BASCULABLINDEXSUPERIOR

                    int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                    if (distBorda1523 == 0)
                        distBorda1523 = altura > 300 ? 50 : 100;

                    decimal raioTrinco3004 = 9;

                    // 3004 Trinco
                    decimal xTrinco1 = distBorda1523 + (decimal)descontoLap - (decimal)5.667213;
                    decimal yTrinco1 = (decimal)descontoLap + (decimal)25.508384;

                    decimal xTrinco2 = xTrinco1;
                    decimal yTrinco2 = yTrinco1;
                    decimal iTrinco2 = distBorda1523 + (decimal)descontoLap;
                    decimal jTrinco2 = (decimal)descontoLap + (decimal)32.5;

                    conteudoArquivo = conteudoArquivo

                        // Faz o furo do trinco
                        .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                        .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))

                        .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                        .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                        .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                        .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                        .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(raioTrinco3004, 6));

                    #endregion
                }
                else if (codigoArquivo == "PIVBLINDEX3010TRIBAIXO")
                {
                    #region PIVBLINDEX3010TRIBAIXO

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    decimal raioPiv = 9;
                    decimal recuoPivo = 10;
                    decimal aberturaPivo = 23; // Distância do ponto inicial ao ponto final da marcação do pivô
                    decimal distBordaYCentroPivo = (decimal)32.5;

                    // Faz o pivô de baixo
                    decimal xPivBaixo1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                    decimal yPivBaixo1 = recuoPivo + (decimal)descontoLap;

                    decimal xPivBaixo2 = xPivBaixo1 + (decimal)2.5;
                    decimal yPivBaixo2 = yPivBaixo1 + (decimal)23.5;

                    decimal xPivBaixo3 = xPivBaixo2 + 18;
                    decimal yPivBaixo3 = yPivBaixo2;
                    decimal iPivBaixo3 = (largura / 2) + (decimal)descontoLap;
                    decimal jPivBaixo3 = (decimal)descontoLap + distBordaYCentroPivo;

                    decimal xPivBaixo4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                    decimal yPivBaixo4 = yPivBaixo1;

                    // Faz o pivô de cima
                    decimal xPivCima1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                    decimal yPivCima1 = (decimal)altura - recuoPivo + (decimal)descontoLap;

                    decimal xPivCima2 = xPivCima1 + (decimal)2.5;
                    decimal yPivCima2 = yPivCima1 - (decimal)23.5;

                    decimal xPivCima3 = xPivCima2 + 18;
                    decimal yPivCima3 = yPivCima2;
                    decimal iPivCima3 = (largura / 2) + (decimal)descontoLap; ;
                    decimal jPivCima3 = (decimal)altura + (decimal)descontoLap - distBordaYCentroPivo;

                    decimal xPivCima4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                    decimal yPivCima4 = yPivCima1;

                    // Faz o trinco
                    decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                    decimal jTrinco = yTrinco;

                    conteudoArquivo = conteudoArquivo

                        // Faz o pivotante de baixo
                        .Replace("?XPivBaixo1?", Formatacoes.TrataValorDecimal(xPivBaixo1, 6))
                        .Replace("?YPivBaixo1?", Formatacoes.TrataValorDecimal(yPivBaixo1, 6))

                        .Replace("?XPivBaixo2?", Formatacoes.TrataValorDecimal(xPivBaixo2, 6))
                        .Replace("?YPivBaixo2?", Formatacoes.TrataValorDecimal(yPivBaixo2, 6))

                        .Replace("?XPivBaixo3?", Formatacoes.TrataValorDecimal(xPivBaixo3, 6))
                        .Replace("?YPivBaixo3?", Formatacoes.TrataValorDecimal(yPivBaixo3, 6))
                        .Replace("?IPivBaixo3?", Formatacoes.TrataValorDecimal(iPivBaixo3, 6))
                        .Replace("?JPivBaixo3?", Formatacoes.TrataValorDecimal(jPivBaixo3, 6))
                        .Replace("?RaioPivBaixo3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                        .Replace("?XPivBaixo4?", Formatacoes.TrataValorDecimal(xPivBaixo4, 6))
                        .Replace("?YPivBaixo4?", Formatacoes.TrataValorDecimal(yPivBaixo4, 6))

                        // Faz o pivotante de cima
                        .Replace("?XPivCima1?", Formatacoes.TrataValorDecimal(xPivCima1, 6))
                        .Replace("?YPivCima1?", Formatacoes.TrataValorDecimal(yPivCima1, 6))

                        .Replace("?XPivCima2?", Formatacoes.TrataValorDecimal(xPivCima2, 6))
                        .Replace("?YPivCima2?", Formatacoes.TrataValorDecimal(yPivCima2, 6))

                        .Replace("?XPivCima3?", Formatacoes.TrataValorDecimal(xPivCima3, 6))
                        .Replace("?YPivCima3?", Formatacoes.TrataValorDecimal(yPivCima3, 6))
                        .Replace("?IPivCima3?", Formatacoes.TrataValorDecimal(iPivCima3, 6))
                        .Replace("?JPivCima3?", Formatacoes.TrataValorDecimal(jPivCima3, 6))
                        .Replace("?RaioPivCima3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                        .Replace("?XPivCima4?", Formatacoes.TrataValorDecimal(xPivCima4, 6))
                        .Replace("?YPivCima4?", Formatacoes.TrataValorDecimal(yPivCima4, 6))

                        // Faz o trinco
                        .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                        .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                        .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                        .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                        .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }
                else if (codigoArquivo == "PIVBLINDEX3010TRILATERAL")
                {
                    #region PIVBLINDEX3010TRILATERAL

                    int altura1335_3539 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.Altura1335_3539);

                    if (altura1335_3539 == 0)
                        throw new Exception("O campo altura da 1335/3539 não foi informado." + mensagemErro);

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    decimal raioPiv = 9;
                    decimal recuoPivo = 10;
                    decimal aberturaPivo = 23; // Distância do ponto inicial ao ponto final da marcação do pivô
                    decimal distBordaYCentroPivo = (decimal)32.5;

                    // Faz o pivô de baixo
                    decimal xPivBaixo1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                    decimal yPivBaixo1 = recuoPivo + (decimal)descontoLap;

                    decimal xPivBaixo2 = xPivBaixo1 + (decimal)2.5;
                    decimal yPivBaixo2 = yPivBaixo1 + (decimal)23.5;

                    decimal xPivBaixo3 = xPivBaixo2 + 18;
                    decimal yPivBaixo3 = yPivBaixo2;
                    decimal iPivBaixo3 = (largura / 2) + (decimal)descontoLap;
                    decimal jPivBaixo3 = (decimal)descontoLap + distBordaYCentroPivo;

                    decimal xPivBaixo4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                    decimal yPivBaixo4 = yPivBaixo1;

                    // Faz o pivô de cima
                    decimal xPivCima1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                    decimal yPivCima1 = (decimal)altura - recuoPivo + (decimal)descontoLap;

                    decimal xPivCima2 = xPivCima1 + (decimal)2.5;
                    decimal yPivCima2 = yPivCima1 - (decimal)23.5;

                    decimal xPivCima3 = xPivCima2 + 18;
                    decimal yPivCima3 = yPivCima2;
                    decimal iPivCima3 = (largura / 2) + (decimal)descontoLap; ;
                    decimal jPivCima3 = (decimal)altura + (decimal)descontoLap - distBordaYCentroPivo;

                    decimal xPivCima4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                    decimal yPivCima4 = yPivCima1;

                    // Faz o trinco
                    decimal xTrinco = largura - (config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco) + (decimal)descontoLap;
                    decimal yTrinco = altura1335_3539 + config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco = xTrinco;
                    decimal jTrinco = yTrinco - config.Trinco.RaioTrinco;

                    conteudoArquivo = conteudoArquivo

                        // Faz o pivotante de baixo
                        .Replace("?XPivBaixo1?", Formatacoes.TrataValorDecimal(xPivBaixo1, 6))
                        .Replace("?YPivBaixo1?", Formatacoes.TrataValorDecimal(yPivBaixo1, 6))

                        .Replace("?XPivBaixo2?", Formatacoes.TrataValorDecimal(xPivBaixo2, 6))
                        .Replace("?YPivBaixo2?", Formatacoes.TrataValorDecimal(yPivBaixo2, 6))

                        .Replace("?XPivBaixo3?", Formatacoes.TrataValorDecimal(xPivBaixo3, 6))
                        .Replace("?YPivBaixo3?", Formatacoes.TrataValorDecimal(yPivBaixo3, 6))
                        .Replace("?IPivBaixo3?", Formatacoes.TrataValorDecimal(iPivBaixo3, 6))
                        .Replace("?JPivBaixo3?", Formatacoes.TrataValorDecimal(jPivBaixo3, 6))
                        .Replace("?RaioPivBaixo3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                        .Replace("?XPivBaixo4?", Formatacoes.TrataValorDecimal(xPivBaixo4, 6))
                        .Replace("?YPivBaixo4?", Formatacoes.TrataValorDecimal(yPivBaixo4, 6))

                        // Faz o pivotante de cima
                        .Replace("?XPivCima1?", Formatacoes.TrataValorDecimal(xPivCima1, 6))
                        .Replace("?YPivCima1?", Formatacoes.TrataValorDecimal(yPivCima1, 6))

                        .Replace("?XPivCima2?", Formatacoes.TrataValorDecimal(xPivCima2, 6))
                        .Replace("?YPivCima2?", Formatacoes.TrataValorDecimal(yPivCima2, 6))

                        .Replace("?XPivCima3?", Formatacoes.TrataValorDecimal(xPivCima3, 6))
                        .Replace("?YPivCima3?", Formatacoes.TrataValorDecimal(yPivCima3, 6))
                        .Replace("?IPivCima3?", Formatacoes.TrataValorDecimal(iPivCima3, 6))
                        .Replace("?JPivCima3?", Formatacoes.TrataValorDecimal(jPivCima3, 6))
                        .Replace("?RaioPivCima3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                        .Replace("?XPivCima4?", Formatacoes.TrataValorDecimal(xPivCima4, 6))
                        .Replace("?YPivCima4?", Formatacoes.TrataValorDecimal(yPivCima4, 6))

                        // Faz o trinco
                        .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                        .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                        .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                        .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                        .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }
                else if (codigoArquivo == "PIVCONTRABLINDEX3010")
                {
                    #region PIVCONTRABLINDEX3010

                    decimal raioPiv = 9;
                    decimal recuoPivo = 10;
                    decimal aberturaPivo = 23; // Distância do ponto inicial ao ponto final da marcação do pivô
                    decimal distBordaYCentroPivo = (decimal)32.5;

                    // Faz o pivô de cima
                    decimal xPivCima1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                    decimal yPivCima1 = (decimal)altura - recuoPivo + (decimal)descontoLap;

                    decimal xPivCima2 = xPivCima1 + (decimal)2.5;
                    decimal yPivCima2 = yPivCima1 - (decimal)23.5;

                    decimal xPivCima3 = xPivCima2 + 18;
                    decimal yPivCima3 = yPivCima2;
                    decimal iPivCima3 = (largura / 2) + (decimal)descontoLap; ;
                    decimal jPivCima3 = (decimal)altura + (decimal)descontoLap - distBordaYCentroPivo;

                    decimal xPivCima4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                    decimal yPivCima4 = yPivCima1;

                    conteudoArquivo = conteudoArquivo

                        // Faz o pivotante de cima
                        .Replace("?XPivCima1?", Formatacoes.TrataValorDecimal(xPivCima1, 6))
                        .Replace("?YPivCima1?", Formatacoes.TrataValorDecimal(yPivCima1, 6))

                        .Replace("?XPivCima2?", Formatacoes.TrataValorDecimal(xPivCima2, 6))
                        .Replace("?YPivCima2?", Formatacoes.TrataValorDecimal(yPivCima2, 6))

                        .Replace("?XPivCima3?", Formatacoes.TrataValorDecimal(xPivCima3, 6))
                        .Replace("?YPivCima3?", Formatacoes.TrataValorDecimal(yPivCima3, 6))
                        .Replace("?IPivCima3?", Formatacoes.TrataValorDecimal(iPivCima3, 6))
                        .Replace("?JPivCima3?", Formatacoes.TrataValorDecimal(jPivCima3, 6))
                        .Replace("?RaioPivCima3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                        .Replace("?XPivCima4?", Formatacoes.TrataValorDecimal(xPivCima4, 6))
                        .Replace("?YPivCima4?", Formatacoes.TrataValorDecimal(yPivCima4, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXDUPLBLINDEX3140-3530")
                {
                    #region PTABRIRPUXDUPLBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    if (distEixoFuroPux == 0)
                        throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                    distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz os dois furos do puxador
                    decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                    decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                    decimal jFuroPux1 = yFuroPux1;

                    decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                    decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                    decimal jFuroPux2 = yFuroPux2;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador 1 e faz o furo
                        .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                        .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                        .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                        .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                        .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Posiciona o CNC no furo do puxador 2 e faz o furo
                        .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                        .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                        .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                        .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                        .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXDUPLTRIBLINDEX3140-3530")
                {
                    #region PTABRIRPUXDUPLTRIBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    if (distEixoFuroPux == 0)
                        throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                    distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz os dois furos do puxador
                    decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                    decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                    decimal jFuroPux1 = yFuroPux1;

                    decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                    decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                    decimal jFuroPux2 = yFuroPux2;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    // Faz o trinco
                    decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                    decimal jTrinco = yTrinco;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador 1 e faz o furo
                        .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                        .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                        .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                        .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                        .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Posiciona o CNC no furo do puxador 2 e faz o furo
                        .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                        .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                        .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                        .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                        .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Faz o trinco
                        .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                        .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                        .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                        .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                        .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXDUPLTRICIMABAIXOBLINDEX3140-3530")
                {
                    #region PTABRIRPUXDUPLTRICIMABAIXOBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    if (distEixoFuroPux == 0)
                        throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                    distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz os dois furos do puxador
                    decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                    decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                    decimal jFuroPux1 = yFuroPux1;

                    decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                    decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                    decimal jFuroPux2 = yFuroPux2;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    // Faz os dois trincos
                    decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco1 = (decimal)altura - config.Trinco.DistBordaYTrincoSup + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                    decimal jTrinco1 = yTrinco1;

                    decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                    decimal jTrinco2 = yTrinco2;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador 1 e faz o furo
                        .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                        .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                        .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                        .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                        .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Posiciona o CNC no furo do puxador 2 e faz o furo
                        .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                        .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                        .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                        .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                        .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Faz os trincos
                        .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                        .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                        .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                        .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                        .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                        .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                        .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                        .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                        .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                        .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXSIMPLESBLINDEX3140-3530")
                {
                    #region PTABRIRPUXSIMPLESBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz o furo do puxador
                    decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                    decimal iFuroPux = xFuroPux - raioFuroPux;
                    decimal jFuroPux = yFuroPux;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador e faz o furo
                        .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                        .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                        .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                        .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                        .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXSIMPLESTRIBLINDEX3140-3530")
                {
                    #region PTABRIRPUXSIMPLESTRIBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz o furo do puxador
                    decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                    decimal iFuroPux = xFuroPux - raioFuroPux;
                    decimal jFuroPux = yFuroPux;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    // Faz o trinco
                    decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                    decimal jTrinco = yTrinco;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador e faz o furo
                        .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                        .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                        .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                        .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                        .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Faz o trinco
                        .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                        .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                        .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                        .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                        .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }
                else if (codigoArquivo == "PTABRIRPUXSIMPLESTRICIMABAIXOBLINDEX3140-3530")
                {
                    #region PTABRIRPUXSIMPLESTRICIMABAIXOBLINDEX3140-3530

                    int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                    int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                    int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    if (alturaPuxador == 0)
                        throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                    decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                    distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                    decimal distBordaXCentroFuroMaior3140 = 70; // Distância da borda do vidro até o centro do furo maior da 3140 (esquerda-direita)
                    decimal distEntreFuroMaiorMenor3140 = (decimal)37.5; // Distância dos centros do furo maior para o furo menor da 3140
                    decimal distBordaYCentroFuroMenor3140 = (decimal)32.5; // Distância da borda do vidro até o centro do furo maior da 3140 (cima-baixo)
                    decimal recuoFuroMaior3140 = 7;

                    decimal raioFuroMenor3140 = 10;
                    decimal raioFuroMaior3140 = 25;

                    // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                    // neste caso foi necessário alterar esta distância
                    config.Trinco.DistBordaYTrincoPtAbrir = 35;

                    // Faz a fechadura 3530
                    float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                    float yFuroMaior = alturaPuxador + descontoLap;
                    float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                    float jFuroMaior = yFuroMaior;

                    float xFuroMenor1 = xFuroMaior - descontoLap;
                    float yFuroMenor1 = yFuroMaior + 42.5f;
                    float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor1 = yFuroMenor1;

                    float xFuroMenor2 = xFuroMaior - descontoLap;
                    float yFuroMenor2 = yFuroMaior - 42.5f;
                    float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                    float jFuroMenor2 = yFuroMenor2;

                    // Faz o furo do puxador
                    decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                    decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                    decimal iFuroPux = xFuroPux - raioFuroPux;
                    decimal jFuroPux = yFuroPux;

                    // Faz os furos da 3140 inferior
                    decimal xFuroMenorDirInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirInf2 = xFuroMenorDirInf1;
                    decimal yFuroMenorDirInf2 = yFuroMenorDirInf1;
                    decimal iFuroMenorDirInf2 = xFuroMenorDirInf1 - raioFuroMenor3140;
                    decimal jFuroMenorDirInf2 = yFuroMenorDirInf2;

                    decimal xFuroMenorEsqInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqInf1 = distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqInf2 = xFuroMenorEsqInf1;
                    decimal yFuroMenorEsqInf2 = yFuroMenorEsqInf1;
                    decimal iFuroMenorEsqInf2 = xFuroMenorEsqInf1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqInf2 = yFuroMenorEsqInf2;

                    decimal xFuroMaiorInf1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorInf1 = (decimal)descontoLap + recuoFuroMaior3140;

                    decimal xFuroMaiorInf2 = xFuroMaiorInf1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorInf2 = yFuroMaiorInf1;
                    decimal iFuroMaiorInf2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorInf2 = yFuroMaiorInf2 - recuoFuroMaior3140;

                    // Faz os furos da 3140 superior
                    decimal xFuroMenorDirSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 + distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorDirSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorDirSup2 = xFuroMenorDirSup1;
                    decimal yFuroMenorDirSup2 = yFuroMenorDirSup1;
                    decimal iFuroMenorDirSup2 = xFuroMenorDirSup1 - raioFuroMenor3140;
                    decimal jFuroMenorDirSup2 = yFuroMenorDirSup2;

                    decimal xFuroMenorEsqSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - distEntreFuroMaiorMenor3140 + raioFuroMenor3140;
                    decimal yFuroMenorEsqSup1 = (decimal)altura - distBordaYCentroFuroMenor3140 + (decimal)descontoLap;

                    decimal xFuroMenorEsqSup2 = xFuroMenorEsqSup1;
                    decimal yFuroMenorEsqSup2 = yFuroMenorEsqSup1;
                    decimal iFuroMenorEsqSup2 = xFuroMenorEsqSup1 - raioFuroMenor3140;
                    decimal jFuroMenorEsqSup2 = yFuroMenorEsqSup2;

                    decimal xFuroMaiorSup1 = (decimal)descontoLap + distBordaXCentroFuroMaior3140 - raioFuroMaior3140 + 1;
                    decimal yFuroMaiorSup1 = (decimal)(altura + descontoLap) - recuoFuroMaior3140;

                    decimal xFuroMaiorSup2 = xFuroMaiorSup1 + (raioFuroMaior3140 * 2) - 2;
                    decimal yFuroMaiorSup2 = yFuroMaiorSup1;
                    decimal iFuroMaiorSup2 = (decimal)descontoLap + distBordaXCentroFuroMaior3140;
                    decimal jFuroMaiorSup2 = yFuroMaiorSup2 + recuoFuroMaior3140;

                    // Faz os dois trincos
                    decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco1 = (decimal)altura - config.Trinco.DistBordaYTrincoSup + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                    decimal jTrinco1 = yTrinco1;

                    decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                    decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco + (decimal)descontoLap;
                    decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                    decimal jTrinco2 = yTrinco2;

                    conteudoArquivo = conteudoArquivo

                        // Faz a fechadura 3530
                        .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                        .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                        .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                        .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                        .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                        .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                        .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                        .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                        .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                        .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                        .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                        .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                        .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                        .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                        // Faz os furos da 3140 inferior
                        .Replace("?XFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf1, 6))
                        .Replace("?YFuroMenorDirInf1?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf1, 6))

                        .Replace("?XFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(xFuroMenorDirInf2, 6))
                        .Replace("?YFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(yFuroMenorDirInf2, 6))
                        .Replace("?IFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(iFuroMenorDirInf2, 6))
                        .Replace("?JFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(jFuroMenorDirInf2, 6))
                        .Replace("?RaioFuroMenorDirInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf1, 6))
                        .Replace("?YFuroMenorEsqInf1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf1, 6))

                        .Replace("?XFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqInf2, 6))
                        .Replace("?YFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqInf2, 6))
                        .Replace("?IFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqInf2, 6))
                        .Replace("?JFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqInf2, 6))
                        .Replace("?RaioFuroMenorEsqInf2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorInf1?", Formatacoes.TrataValorDecimal(xFuroMaiorInf1, 6))
                        .Replace("?YFuroMaiorInf1?", Formatacoes.TrataValorDecimal(yFuroMaiorInf1, 6))

                        .Replace("?XFuroMaiorInf2?", Formatacoes.TrataValorDecimal(xFuroMaiorInf2, 6))
                        .Replace("?YFuroMaiorInf2?", Formatacoes.TrataValorDecimal(yFuroMaiorInf2, 6))
                        .Replace("?IFuroMaiorInf2?", Formatacoes.TrataValorDecimal(iFuroMaiorInf2, 6))
                        .Replace("?JFuroMaiorInf2?", Formatacoes.TrataValorDecimal(jFuroMaiorInf2, 6))
                        .Replace("?RaioFuroMaiorInf2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Faz os furos da 3140 superior
                        .Replace("?XFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup1, 6))
                        .Replace("?YFuroMenorDirSup1?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup1, 6))

                        .Replace("?XFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(xFuroMenorDirSup2, 6))
                        .Replace("?YFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(yFuroMenorDirSup2, 6))
                        .Replace("?IFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(iFuroMenorDirSup2, 6))
                        .Replace("?JFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(jFuroMenorDirSup2, 6))
                        .Replace("?RaioFuroMenorDirSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup1, 6))
                        .Replace("?YFuroMenorEsqSup1?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup1, 6))

                        .Replace("?XFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(xFuroMenorEsqSup2, 6))
                        .Replace("?YFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(yFuroMenorEsqSup2, 6))
                        .Replace("?IFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(iFuroMenorEsqSup2, 6))
                        .Replace("?JFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(jFuroMenorEsqSup2, 6))
                        .Replace("?RaioFuroMenorEsqSup2?", Formatacoes.TrataValorDecimal(raioFuroMenor3140, 6))

                        .Replace("?XFuroMaiorSup1?", Formatacoes.TrataValorDecimal(xFuroMaiorSup1, 6))
                        .Replace("?YFuroMaiorSup1?", Formatacoes.TrataValorDecimal(yFuroMaiorSup1, 6))

                        .Replace("?XFuroMaiorSup2?", Formatacoes.TrataValorDecimal(xFuroMaiorSup2, 6))
                        .Replace("?YFuroMaiorSup2?", Formatacoes.TrataValorDecimal(yFuroMaiorSup2, 6))
                        .Replace("?IFuroMaiorSup2?", Formatacoes.TrataValorDecimal(iFuroMaiorSup2, 6))
                        .Replace("?JFuroMaiorSup2?", Formatacoes.TrataValorDecimal(jFuroMaiorSup2, 6))
                        .Replace("?RaioFuroMaiorSup2?", Formatacoes.TrataValorDecimal(raioFuroMaior3140, 6))

                        // Posiciona o CNC no furo do puxador e faz o furo
                        .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                        .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                        .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                        .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                        .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                        // Faz os trincos
                        .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                        .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                        .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                        .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                        .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                        .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                        .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                        .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                        .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                        .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                    #endregion
                }

            #endregion

            else if (codigoArquivo == "1302DOISCANTOSSUP")
            {
                #region 1302DOISCANTOSSUP

                // Faz o recorte superior direito
                float xRecDirSup1 = largura - (36 + descontoLap);
                float yRecDirSup1 = altura - descontoLap;

                float xRecDirSup2 = xRecDirSup1;
                float yRecDirSup2 = yRecDirSup1 - 23.5f;

                float xRecDirSup3 = xRecDirSup2 + 12.5f;
                float yRecDirSup3 = yRecDirSup2 - 12.5f;
                float iRecDirSup3 = xRecDirSup3;
                float jRecDirSup3 = yRecDirSup2;
                float raioRecDirSup = 12.5f;

                float xRecDirSup4 = largura - descontoLap;
                float yRecDirSup4 = yRecDirSup3;

                // Faz o recorte superior esquerdo
                float xRecEsqSup1 = 36 + descontoLap;
                float yRecEsqSup1 = altura - descontoLap;

                float xRecEsqSup2 = xRecEsqSup1;
                float yRecEsqSup2 = yRecEsqSup1 - 23.5f;

                float xRecEsqSup3 = xRecEsqSup2 - 12.5f;
                float yRecEsqSup3 = yRecEsqSup2 - 12.5f;
                float iRecEsqSup3 = xRecEsqSup3;
                float jRecEsqSup3 = yRecEsqSup2;
                float raioRecEsqSup = 12.5f;

                float xRecEsqSup4 = descontoLap;
                float yRecEsqSup4 = yRecEsqSup3;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDouble(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDouble(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDouble(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDouble(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDouble(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDouble(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDouble(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDouble(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup?", Formatacoes.TrataValorDouble(raioRecDirSup, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDouble(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDouble(yRecDirSup4, 6))

                    // Faz o recorte superior esquerdo
                    .Replace("?XRecEsqSup1?", Formatacoes.TrataValorDouble(xRecEsqSup1, 6))
                    .Replace("?YRecEsqSup1?", Formatacoes.TrataValorDouble(yRecEsqSup1, 6))

                    .Replace("?XRecEsqSup2?", Formatacoes.TrataValorDouble(xRecEsqSup2, 6))
                    .Replace("?YRecEsqSup2?", Formatacoes.TrataValorDouble(yRecEsqSup2, 6))

                    .Replace("?XRecEsqSup3?", Formatacoes.TrataValorDouble(xRecEsqSup3, 6))
                    .Replace("?YRecEsqSup3?", Formatacoes.TrataValorDouble(yRecEsqSup3, 6))
                    .Replace("?IRecEsqSup3?", Formatacoes.TrataValorDouble(iRecEsqSup3, 6))
                    .Replace("?JRecEsqSup3?", Formatacoes.TrataValorDouble(jRecEsqSup3, 6))
                    .Replace("?RaioRecEsqSup?", Formatacoes.TrataValorDouble(raioRecEsqSup, 6))

                    .Replace("?XRecEsqSup4?", Formatacoes.TrataValorDouble(xRecEsqSup4, 6))
                    .Replace("?YRecEsqSup4?", Formatacoes.TrataValorDouble(yRecEsqSup4, 6));

                #endregion
            }
            else if (codigoArquivo == "1302QUATROCANTOS")
            {
                #region 1302QUATROCANTOS

                // Faz o recorte inferior direito
                float xRecDirInf1 = largura - (36 + descontoLap);
                float yRecDirInf1 = descontoLap;

                float xRecDirInf2 = xRecDirInf1;
                float yRecDirInf2 = 23.5f + descontoLap;

                float xRecDirInf3 = xRecDirInf2 + 12.5f;
                float yRecDirInf3 = yRecDirInf2 + 12.5f;
                float iRecDirInf3 = xRecDirInf3;
                float jRecDirInf3 = yRecDirInf2;
                float raioRecDirInf = 12.5f;

                float xRecDirInf4 = largura - descontoLap;
                float yRecDirInf4 = yRecDirInf3;

                // Faz o recorte superior direito
                float xRecDirSup1 = largura - (36 + descontoLap);
                float yRecDirSup1 = altura - descontoLap;

                float xRecDirSup2 = xRecDirSup1;
                float yRecDirSup2 = yRecDirSup1 - 23.5f;

                float xRecDirSup3 = xRecDirSup2 + 12.5f;
                float yRecDirSup3 = yRecDirSup2 - 12.5f;
                float iRecDirSup3 = xRecDirSup3;
                float jRecDirSup3 = yRecDirSup2;
                float raioRecDirSup = 12.5f;

                float xRecDirSup4 = largura - descontoLap;
                float yRecDirSup4 = yRecDirSup3;

                // Faz o recorte inferior esquerdo
                float xRecEsqInf1 = 36 + descontoLap;
                float yRecEsqInf1 = descontoLap;

                float xRecEsqInf2 = xRecEsqInf1;
                float yRecEsqInf2 = yRecEsqInf1 + 23.5f;

                float xRecEsqInf3 = xRecEsqInf2 - 12.5f;
                float yRecEsqInf3 = yRecEsqInf2 + 12.5f;
                float iRecEsqInf3 = xRecEsqInf3;
                float jRecEsqInf3 = yRecEsqInf2;
                float raioRecEsqInf = 12.5f;

                float xRecEsqInf4 = descontoLap;
                float yRecEsqInf4 = yRecEsqInf3;

                // Faz o recorte superior esquerdo
                float xRecEsqSup1 = 36 + descontoLap;
                float yRecEsqSup1 = altura - descontoLap;

                float xRecEsqSup2 = xRecEsqSup1;
                float yRecEsqSup2 = yRecEsqSup1 - 23.5f;

                float xRecEsqSup3 = xRecEsqSup2 - 12.5f;
                float yRecEsqSup3 = yRecEsqSup2 - 12.5f;
                float iRecEsqSup3 = xRecEsqSup3;
                float jRecEsqSup3 = yRecEsqSup2;
                float raioRecEsqSup = 12.5f;

                float xRecEsqSup4 = descontoLap;
                float yRecEsqSup4 = yRecEsqSup3;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte inferior direito
                    .Replace("?XRecDirInf1?", Formatacoes.TrataValorDouble(xRecDirInf1, 6))
                    .Replace("?YRecDirInf1?", Formatacoes.TrataValorDouble(yRecDirInf1, 6))

                    .Replace("?XRecDirInf2?", Formatacoes.TrataValorDouble(xRecDirInf2, 6))
                    .Replace("?YRecDirInf2?", Formatacoes.TrataValorDouble(yRecDirInf2, 6))

                    .Replace("?XRecDirInf3?", Formatacoes.TrataValorDouble(xRecDirInf3, 6))
                    .Replace("?YRecDirInf3?", Formatacoes.TrataValorDouble(yRecDirInf3, 6))
                    .Replace("?IRecDirInf3?", Formatacoes.TrataValorDouble(iRecDirInf3, 6))
                    .Replace("?JRecDirInf3?", Formatacoes.TrataValorDouble(jRecDirInf3, 6))
                    .Replace("?RaioRecDirInf?", Formatacoes.TrataValorDouble(raioRecDirInf, 6))

                    .Replace("?XRecDirInf4?", Formatacoes.TrataValorDouble(xRecDirInf4, 6))
                    .Replace("?YRecDirInf4?", Formatacoes.TrataValorDouble(yRecDirInf4, 6))

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDouble(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDouble(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDouble(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDouble(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDouble(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDouble(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDouble(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDouble(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup?", Formatacoes.TrataValorDouble(raioRecDirSup, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDouble(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDouble(yRecDirSup4, 6))

                    // Faz o recorte inferior esquerdo
                    .Replace("?XRecEsqInf1?", Formatacoes.TrataValorDouble(xRecEsqInf1, 6))
                    .Replace("?YRecEsqInf1?", Formatacoes.TrataValorDouble(yRecEsqInf1, 6))

                    .Replace("?XRecEsqInf2?", Formatacoes.TrataValorDouble(xRecEsqInf2, 6))
                    .Replace("?YRecEsqInf2?", Formatacoes.TrataValorDouble(yRecEsqInf2, 6))

                    .Replace("?XRecEsqInf3?", Formatacoes.TrataValorDouble(xRecEsqInf3, 6))
                    .Replace("?YRecEsqInf3?", Formatacoes.TrataValorDouble(yRecEsqInf3, 6))
                    .Replace("?IRecEsqInf3?", Formatacoes.TrataValorDouble(iRecEsqInf3, 6))
                    .Replace("?JRecEsqInf3?", Formatacoes.TrataValorDouble(jRecEsqInf3, 6))
                    .Replace("?RaioRecEsqInf?", Formatacoes.TrataValorDouble(raioRecEsqInf, 6))

                    .Replace("?XRecEsqInf4?", Formatacoes.TrataValorDouble(xRecEsqInf4, 6))
                    .Replace("?YRecEsqInf4?", Formatacoes.TrataValorDouble(yRecEsqInf4, 6))

                    // Faz o recorte superior esquerdo
                    .Replace("?XRecEsqSup1?", Formatacoes.TrataValorDouble(xRecEsqSup1, 6))
                    .Replace("?YRecEsqSup1?", Formatacoes.TrataValorDouble(yRecEsqSup1, 6))

                    .Replace("?XRecEsqSup2?", Formatacoes.TrataValorDouble(xRecEsqSup2, 6))
                    .Replace("?YRecEsqSup2?", Formatacoes.TrataValorDouble(yRecEsqSup2, 6))

                    .Replace("?XRecEsqSup3?", Formatacoes.TrataValorDouble(xRecEsqSup3, 6))
                    .Replace("?YRecEsqSup3?", Formatacoes.TrataValorDouble(yRecEsqSup3, 6))
                    .Replace("?IRecEsqSup3?", Formatacoes.TrataValorDouble(iRecEsqSup3, 6))
                    .Replace("?JRecEsqSup3?", Formatacoes.TrataValorDouble(jRecEsqSup3, 6))
                    .Replace("?RaioRecEsqSup?", Formatacoes.TrataValorDouble(raioRecEsqSup, 6))

                    .Replace("?XRecEsqSup4?", Formatacoes.TrataValorDouble(xRecEsqSup4, 6))
                    .Replace("?YRecEsqSup4?", Formatacoes.TrataValorDouble(yRecEsqSup4, 6));

                #endregion
            }
            else if (codigoArquivo == "1306CANTODIREITOSUP")
            {
                #region 1306CANTODIREITOSUP

                // Faz o recorte superior direito
                float xRecDirSup1 = largura - (4 + descontoLap);
                float yRecDirSup1 = altura - (24 + descontoLap);

                float xRecDirSup2 = xRecDirSup1 - 3.22949F;
                float yRecDirSup2 = yRecDirSup1;

                float xRecDirSup3 = xRecDirSup2 - 4.791574F;
                float yRecDirSup3 = yRecDirSup2 - 3.571429F;
                float iRecDirSup3 = xRecDirSup2;
                float jRecDirSup3 = yRecDirSup2 - 5F;
                float raioRecDirSup3 = 5F;

                float xRecDirSup4 = xRecDirSup3 - 15.550365F;
                float yRecDirSup4 = yRecDirSup3 + 15.550365F;
                float iRecDirSup4 = iRecDirSup3 - 16.77051F;
                float jRecDirSup4 = yRecDirSup2;
                float raioRecDirSup4 = 12.5F;

                float xRecDirSup5 = iRecDirSup4;
                float yRecDirSup5 = yRecDirSup4 + 4.791574F;
                float iRecDirSup5 = xRecDirSup5 - 5F;
                float jRecDirSup5 = yRecDirSup5;
                float raioRecDirSup5 = 5F;

                float xRecDirSup6 = xRecDirSup5;
                float yRecDirSup6 = yRecDirSup1 + 20F;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDouble(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDouble(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDouble(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDouble(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDouble(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDouble(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDouble(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDouble(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup3?", Formatacoes.TrataValorDouble(raioRecDirSup3, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDouble(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDouble(yRecDirSup4, 6))
                    .Replace("?IRecDirSup4?", Formatacoes.TrataValorDouble(iRecDirSup4, 6))
                    .Replace("?JRecDirSup4?", Formatacoes.TrataValorDouble(jRecDirSup4, 6))
                    .Replace("?RaioRecDirSup4?", Formatacoes.TrataValorDouble(raioRecDirSup4, 6))

                    .Replace("?XRecDirSup5?", Formatacoes.TrataValorDouble(xRecDirSup5, 6))
                    .Replace("?YRecDirSup5?", Formatacoes.TrataValorDouble(yRecDirSup5, 6))
                    .Replace("?IRecDirSup5?", Formatacoes.TrataValorDouble(iRecDirSup5, 6))
                    .Replace("?JRecDirSup5?", Formatacoes.TrataValorDouble(jRecDirSup5, 6))
                    .Replace("?RaioRecDirSup5?", Formatacoes.TrataValorDouble(raioRecDirSup5, 6))

                    .Replace("?XRecDirSup6?", Formatacoes.TrataValorDouble(xRecDirSup6, 6))
                    .Replace("?YRecDirSup6?", Formatacoes.TrataValorDouble(yRecDirSup6, 6));

                #endregion
            }
            else if (codigoArquivo == "1329QUATROCANTOSESQDIR")
            {
                #region 1329QUATROCANTOSESQDIR

                int altura1329 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1329);

                if (altura1329 == 0)
                    throw new Exception("O campo altura da 1329 não foi informado." + mensagemErro);

                // Faz o recorte inferior esquerdo
                decimal xRecEsqInf1 = 9 + (decimal)descontoLap;
                decimal yRecEsqInf1 = (decimal)(altura1329 + (decimal)15.148746 + (decimal)descontoLap);

                decimal xRecEsqInf2 = xRecEsqInf1 + (decimal)14.746604;
                decimal yRecEsqInf2 = yRecEsqInf1 - (decimal)3.467301;

                decimal xRecEsqInf3 = xRecEsqInf2;
                decimal yRecEsqInf3 = yRecEsqInf2 - (decimal)23.36289;
                decimal iRecEsqInf3 = 22 + (decimal)descontoLap;
                decimal jRecEsqInf3 = altura1329 + (decimal)descontoLap;
                decimal raioRecEsqInf = 12;

                decimal xRecEsqInf4 = xRecEsqInf1;
                decimal yRecEsqInf4 = yRecEsqInf3 - (decimal)3.467301;

                // Faz o recorte superior esquerdo
                decimal xRecEsqSup1 = 9 + (decimal)descontoLap;
                decimal yRecEsqSup1 = ((decimal)altura + (decimal)descontoLap) - (altura1329 + (decimal)15.148746);

                decimal xRecEsqSup2 = xRecEsqSup1 + (decimal)14.746604;
                decimal yRecEsqSup2 = yRecEsqSup1 + (decimal)3.467301;

                decimal xRecEsqSup3 = xRecEsqSup2;
                decimal yRecEsqSup3 = yRecEsqSup2 + (decimal)23.36289;
                decimal iRecEsqSup3 = 22 + (decimal)descontoLap;
                decimal jRecEsqSup3 = ((decimal)altura + (decimal)descontoLap) - altura1329;
                decimal raioRecEsqSup = 12;

                decimal xRecEsqSup4 = xRecEsqSup1;
                decimal yRecEsqSup4 = yRecEsqSup3 + (decimal)3.467301;

                // Faz o recorte superior direito
                decimal xRecDirSup1 = largura - (9 + (decimal)descontoLap);
                decimal yRecDirSup1 = ((decimal)altura + (decimal)descontoLap) - (altura1329 + (decimal)15.148746);

                decimal xRecDirSup2 = xRecDirSup1 - (decimal)14.746604;
                decimal yRecDirSup2 = yRecDirSup1 + (decimal)3.467301;

                decimal xRecDirSup3 = xRecDirSup2;
                decimal yRecDirSup3 = yRecDirSup2 + (decimal)23.36289;
                decimal iRecDirSup3 = largura - (22 + (decimal)descontoLap);
                decimal jRecDirSup3 = ((decimal)altura + (decimal)descontoLap) - altura1329;
                decimal raioRecDirSup = 12;

                decimal xRecDirSup4 = xRecDirSup1;
                decimal yRecDirSup4 = yRecDirSup3 + (decimal)3.467301;

                // Faz o recorte inferior direito
                decimal xRecDirInf1 = largura - (9 + (decimal)descontoLap);
                decimal yRecDirInf1 = (altura1329 + (decimal)descontoLap) - (decimal)15.148746;

                decimal xRecDirInf2 = xRecDirInf1 - (decimal)14.746604;
                decimal yRecDirInf2 = yRecDirInf1 + (decimal)3.467301;

                decimal xRecDirInf3 = xRecDirInf2;
                decimal yRecDirInf3 = yRecDirInf2 + (decimal)23.36289;
                decimal iRecDirInf3 = largura - (22 + (decimal)descontoLap);
                decimal jRecDirInf3 = altura1329 + (decimal)descontoLap;
                decimal raioRecDirInf = 12;

                decimal xRecDirInf4 = xRecDirInf1;
                decimal yRecDirInf4 = yRecDirInf3 + (decimal)3.467301;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte inferior esquerdo
                    .Replace("?XRecEsqInf1?", Formatacoes.TrataValorDecimal(xRecEsqInf1, 6))
                    .Replace("?YRecEsqInf1?", Formatacoes.TrataValorDecimal(yRecEsqInf1, 6))

                    .Replace("?XRecEsqInf2?", Formatacoes.TrataValorDecimal(xRecEsqInf2, 6))
                    .Replace("?YRecEsqInf2?", Formatacoes.TrataValorDecimal(yRecEsqInf2, 6))

                    .Replace("?XRecEsqInf3?", Formatacoes.TrataValorDecimal(xRecEsqInf3, 6))
                    .Replace("?YRecEsqInf3?", Formatacoes.TrataValorDecimal(yRecEsqInf3, 6))
                    .Replace("?IRecEsqInf3?", Formatacoes.TrataValorDecimal(iRecEsqInf3, 6))
                    .Replace("?JRecEsqInf3?", Formatacoes.TrataValorDecimal(jRecEsqInf3, 6))
                    .Replace("?RaioRecEsqInf?", Formatacoes.TrataValorDecimal(raioRecEsqInf, 6))

                    .Replace("?XRecEsqInf4?", Formatacoes.TrataValorDecimal(xRecEsqInf4, 6))
                    .Replace("?YRecEsqInf4?", Formatacoes.TrataValorDecimal(yRecEsqInf4, 6))

                    // Faz o recorte superior esquerdo
                    .Replace("?XRecEsqSup1?", Formatacoes.TrataValorDecimal(xRecEsqSup1, 6))
                    .Replace("?YRecEsqSup1?", Formatacoes.TrataValorDecimal(yRecEsqSup1, 6))

                    .Replace("?XRecEsqSup2?", Formatacoes.TrataValorDecimal(xRecEsqSup2, 6))
                    .Replace("?YRecEsqSup2?", Formatacoes.TrataValorDecimal(yRecEsqSup2, 6))

                    .Replace("?XRecEsqSup3?", Formatacoes.TrataValorDecimal(xRecEsqSup3, 6))
                    .Replace("?YRecEsqSup3?", Formatacoes.TrataValorDecimal(yRecEsqSup3, 6))
                    .Replace("?IRecEsqSup3?", Formatacoes.TrataValorDecimal(iRecEsqSup3, 6))
                    .Replace("?JRecEsqSup3?", Formatacoes.TrataValorDecimal(jRecEsqSup3, 6))
                    .Replace("?RaioRecEsqSup?", Formatacoes.TrataValorDecimal(raioRecEsqSup, 6))

                    .Replace("?XRecEsqSup4?", Formatacoes.TrataValorDecimal(xRecEsqSup4, 6))
                    .Replace("?YRecEsqSup4?", Formatacoes.TrataValorDecimal(yRecEsqSup4, 6))

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDecimal(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDecimal(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDecimal(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDecimal(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDecimal(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDecimal(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDecimal(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDecimal(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup?", Formatacoes.TrataValorDecimal(raioRecDirSup, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDecimal(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDecimal(yRecDirSup4, 6))

                    // Faz o recorte inferior direito
                    .Replace("?XRecDirInf1?", Formatacoes.TrataValorDecimal(xRecDirInf1, 6))
                    .Replace("?YRecDirInf1?", Formatacoes.TrataValorDecimal(yRecDirInf1, 6))

                    .Replace("?XRecDirInf2?", Formatacoes.TrataValorDecimal(xRecDirInf2, 6))
                    .Replace("?YRecDirInf2?", Formatacoes.TrataValorDecimal(yRecDirInf2, 6))

                    .Replace("?XRecDirInf3?", Formatacoes.TrataValorDecimal(xRecDirInf3, 6))
                    .Replace("?YRecDirInf3?", Formatacoes.TrataValorDecimal(yRecDirInf3, 6))
                    .Replace("?IRecDirInf3?", Formatacoes.TrataValorDecimal(iRecDirInf3, 6))
                    .Replace("?JRecDirInf3?", Formatacoes.TrataValorDecimal(jRecDirInf3, 6))
                    .Replace("?RaioRecDirInf?", Formatacoes.TrataValorDecimal(raioRecDirInf, 6))

                    .Replace("?XRecDirInf4?", Formatacoes.TrataValorDecimal(xRecDirInf4, 6))
                    .Replace("?YRecDirInf4?", Formatacoes.TrataValorDecimal(yRecDirInf4, 6));

                #endregion
            }
            else if (codigoArquivo == "1329QUATROCANTOSESQDIREDOISCENTRAIS")
            {
                #region 1329QUATROCANTOSESQDIREDOISCENTRAIS

                int altura1329 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1329);

                if (altura1329 == 0)
                    throw new Exception("O campo altura da 1329 não foi informado." + mensagemErro);

                // FAZ O RECORTE SUPERIOR CENTRAL
                float xRecCenSup1 = ((largura / 2F) - descontoLap) - 12.399063F;
                float yRecCenSup1 = altura - (7F + descontoLap);

                float xRecCenSup2 = xRecCenSup1 + 3.1666125F;
                float yRecCenSup2 = yRecCenSup1 - 15.07F;

                float xRecCenSup3 = xRecCenSup2 + 24.465876F;
                float yRecCenSup3 = yRecCenSup2;
                float iRecCenSup3 = xRecCenSup3 - 12.232938F;
                float jRecCenSup3 = yRecCenSup2 + 2.570063F;
                float raioRecCenSup = 12.5F;

                float xRecCenSup4 = xRecCenSup3 + 3.166125F;
                float yRecCenSup4 = yRecCenSup1;

                // FAZ O RECORTE SUPERIOR ESQUERDO
                float xRecEsqSup1 = 10F + descontoLap;
                float yRecEsqSup1 = (altura - altura1329) - (12.399063F + descontoLap);

                float xRecEsqSup2 = xRecEsqSup1 + 15.070063F;
                float yRecEsqSup2 = yRecEsqSup1 + 3.166125F;

                float xRecEsqSup3 = xRecEsqSup2;
                float yRecEsqSup3 = yRecEsqSup2 + 24.465876F;
                float iRecEsqSup3 = xRecEsqSup3 - 2.570063F;
                float jRecEsqSup3 = yRecEsqSup3 - 12.232938F;
                float raioRecEsqSup = 12.5F;

                float xRecEsqSup4 = xRecEsqSup1;
                float yRecEsqSup4 = yRecEsqSup3 + 3.166125F;

                // FAZ O RECORTE SUPERIOR DIREITO
                float xRecDirSup1 = (largura - 10F) + descontoLap;
                float yRecDirSup1 = (altura - altura1329) + (15.399063F + descontoLap);

                float xRecDirSup2 = xRecDirSup1 - 15.070063F;
                float yRecDirSup2 = yRecDirSup1 - 3.166125F;

                float xRecDirSup3 = xRecDirSup2;
                float yRecDirSup3 = yRecDirSup2 - 24.465876F;
                float iRecDirSup3 = xRecDirSup3 + 2.570063F;
                float jRecDirSup3 = yRecDirSup3 + 12.232938F;
                float raioRecDirSup = 12.5F;

                float xRecDirSup4 = xRecDirSup3 + 15.070063F;
                float yRecDirSup4 = yRecDirSup3 - 3.166125F;

                // FAZ O RECORTE INFERIOR DIREITO
                float xRecDirInf1 = (largura - 10) + descontoLap;
                float yRecDirInf1 = altura1329 + (15.399063F + descontoLap);

                float xRecDirInf2 = xRecDirInf1 - 15.070063F;
                float yRecDirInf2 = yRecDirInf1 - 3.166125F;

                float xRecDirInf3 = xRecDirInf2;
                float yRecDirInf3 = yRecDirInf2 - 24.465876F;
                float iRecDirInf3 = xRecDirInf3 + 2.570063F;
                float jRecDirInf3 = yRecDirInf3 + 12.232938F;
                float raioRecDirInf = 12.5F;

                float xRecDirInf4 = xRecDirInf1;
                float yRecDirInf4 = yRecDirInf3 - 3.166125F;

                // FAZ O RECORTE INFERIOR ESQUERDO
                float xRecEsqInf1 = 10F + descontoLap;
                float yRecEsqInf1 = altura1329 - (12.399063F + descontoLap);

                float xRecEsqInf2 = xRecEsqInf1 + 15.070063F;
                float yRecEsqInf2 = yRecEsqInf1 + 3.166125F;

                float xRecEsqInf3 = xRecEsqInf2;
                float yRecEsqInf3 = yRecEsqInf2 + 24.465876F;
                float iRecEsqInf3 = xRecEsqInf3 - 2.570063F;
                float jRecEsqInf3 = yRecEsqInf3 - 12.232938F;
                float raioRecEsqInf = 12.5F;

                float xRecEsqInf4 = xRecEsqInf1;
                float yRecEsqInf4 = yRecEsqInf3 + 3.166125F;

                // FAZ O RECORTE INFERIOR CENTRAL
                float xRecCenInf1 = (largura / 2F) + (15.399063F + descontoLap);
                float yRecCenInf1 = 10 + descontoLap;

                float xRecCenInf2 = xRecCenInf1 - 3.1666125F;
                float yRecCenInf2 = yRecCenInf1 + 15.070063F;

                float xRecCenInf3 = xRecCenInf2 - 24.465876F;
                float yRecCenInf3 = yRecCenInf2;
                float iRecCenInf3 = xRecCenInf3 + 12.232938F;
                float jRecCenInf3 = yRecCenInf3 - 2.570063F;
                float raioRecCenInf = 12.5F;

                float xRecCenInf4 = xRecCenInf3 - 3.166125F;
                float yRecCenInf4 = yRecCenInf1;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte superior central
                    .Replace("?XRecCenSup1?", Formatacoes.TrataValorDouble(xRecCenSup1, 6))
                    .Replace("?YRecCenSup1?", Formatacoes.TrataValorDouble(yRecCenSup1, 6))

                    .Replace("?XRecCenSup2?", Formatacoes.TrataValorDouble(xRecCenSup2, 6))
                    .Replace("?YRecCenSup2?", Formatacoes.TrataValorDouble(yRecCenSup2, 6))

                    .Replace("?XRecCenSup3?", Formatacoes.TrataValorDouble(xRecCenSup3, 6))
                    .Replace("?YRecCenSup3?", Formatacoes.TrataValorDouble(yRecCenSup3, 6))
                    .Replace("?IRecCenSup3?", Formatacoes.TrataValorDouble(iRecCenSup3, 6))
                    .Replace("?JRecCenSup3?", Formatacoes.TrataValorDouble(jRecCenSup3, 6))
                    .Replace("?RaioRecCenSup?", Formatacoes.TrataValorDouble(raioRecCenSup, 6))

                    .Replace("?XRecCenSup4?", Formatacoes.TrataValorDouble(xRecCenSup4, 6))
                    .Replace("?YRecCenSup4?", Formatacoes.TrataValorDouble(yRecCenSup4, 6))

                    // Faz o recorte superior esquerdo
                    .Replace("?XRecEsqSup1?", Formatacoes.TrataValorDouble(xRecEsqSup1, 6))
                    .Replace("?YRecEsqSup1?", Formatacoes.TrataValorDouble(yRecEsqSup1, 6))

                    .Replace("?XRecEsqSup2?", Formatacoes.TrataValorDouble(xRecEsqSup2, 6))
                    .Replace("?YRecEsqSup2?", Formatacoes.TrataValorDouble(yRecEsqSup2, 6))

                    .Replace("?XRecEsqSup3?", Formatacoes.TrataValorDouble(xRecEsqSup3, 6))
                    .Replace("?YRecEsqSup3?", Formatacoes.TrataValorDouble(yRecEsqSup3, 6))
                    .Replace("?IRecEsqSup3?", Formatacoes.TrataValorDouble(iRecEsqSup3, 6))
                    .Replace("?JRecEsqSup3?", Formatacoes.TrataValorDouble(jRecEsqSup3, 6))
                    .Replace("?RaioRecEsqSup?", Formatacoes.TrataValorDouble(raioRecEsqSup, 6))

                    .Replace("?XRecEsqSup4?", Formatacoes.TrataValorDouble(xRecEsqSup4, 6))
                    .Replace("?YRecEsqSup4?", Formatacoes.TrataValorDouble(yRecEsqSup4, 6))

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDouble(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDouble(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDouble(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDouble(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDouble(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDouble(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDouble(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDouble(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup?", Formatacoes.TrataValorDouble(raioRecDirSup, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDouble(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDouble(yRecDirSup4, 6))

                    // Faz o recorte inferior direito
                    .Replace("?XRecDirInf1?", Formatacoes.TrataValorDouble(xRecDirInf1, 6))
                    .Replace("?YRecDirInf1?", Formatacoes.TrataValorDouble(yRecDirInf1, 6))

                    .Replace("?XRecDirInf2?", Formatacoes.TrataValorDouble(xRecDirInf2, 6))
                    .Replace("?YRecDirInf2?", Formatacoes.TrataValorDouble(yRecDirInf2, 6))

                    .Replace("?XRecDirInf3?", Formatacoes.TrataValorDouble(xRecDirInf3, 6))
                    .Replace("?YRecDirInf3?", Formatacoes.TrataValorDouble(yRecDirInf3, 6))
                    .Replace("?IRecDirInf3?", Formatacoes.TrataValorDouble(iRecDirInf3, 6))
                    .Replace("?JRecDirInf3?", Formatacoes.TrataValorDouble(jRecDirInf3, 6))
                    .Replace("?RaioRecDirInf?", Formatacoes.TrataValorDouble(raioRecDirInf, 6))

                    .Replace("?XRecDirInf4?", Formatacoes.TrataValorDouble(xRecDirInf4, 6))
                    .Replace("?YRecDirInf4?", Formatacoes.TrataValorDouble(yRecDirInf4, 6))

                    // Faz o recorte inferior esquerdo
                    .Replace("?XRecEsqInf1?", Formatacoes.TrataValorDouble(xRecEsqInf1, 6))
                    .Replace("?YRecEsqInf1?", Formatacoes.TrataValorDouble(yRecEsqInf1, 6))

                    .Replace("?XRecEsqInf2?", Formatacoes.TrataValorDouble(xRecEsqInf2, 6))
                    .Replace("?YRecEsqInf2?", Formatacoes.TrataValorDouble(yRecEsqInf2, 6))

                    .Replace("?XRecEsqInf3?", Formatacoes.TrataValorDouble(xRecEsqInf3, 6))
                    .Replace("?YRecEsqInf3?", Formatacoes.TrataValorDouble(yRecEsqInf3, 6))
                    .Replace("?IRecEsqInf3?", Formatacoes.TrataValorDouble(iRecEsqInf3, 6))
                    .Replace("?JRecEsqInf3?", Formatacoes.TrataValorDouble(jRecEsqInf3, 6))
                    .Replace("?RaioRecEsqInf?", Formatacoes.TrataValorDouble(raioRecEsqInf, 6))

                    .Replace("?XRecEsqInf4?", Formatacoes.TrataValorDouble(xRecEsqInf4, 6))
                    .Replace("?YRecEsqInf4?", Formatacoes.TrataValorDouble(yRecEsqInf4, 6))

                    // Faz o recorte inferior central
                    .Replace("?XRecCenInf1?", Formatacoes.TrataValorDouble(xRecCenInf1, 6))
                    .Replace("?YRecCenInf1?", Formatacoes.TrataValorDouble(yRecCenInf1, 6))

                    .Replace("?XRecCenInf2?", Formatacoes.TrataValorDouble(xRecCenInf2, 6))
                    .Replace("?YRecCenInf2?", Formatacoes.TrataValorDouble(yRecCenInf2, 6))

                    .Replace("?XRecCenInf3?", Formatacoes.TrataValorDouble(xRecCenInf3, 6))
                    .Replace("?YRecCenInf3?", Formatacoes.TrataValorDouble(yRecCenInf3, 6))
                    .Replace("?IRecCenInf3?", Formatacoes.TrataValorDouble(iRecCenInf3, 6))
                    .Replace("?JRecCenInf3?", Formatacoes.TrataValorDouble(jRecCenInf3, 6))
                    .Replace("?RaioRecCenInf?", Formatacoes.TrataValorDouble(raioRecCenInf, 6))

                    .Replace("?XRecCenInf4?", Formatacoes.TrataValorDouble(xRecCenInf4, 6))
                    .Replace("?YRecCenInf4?", Formatacoes.TrataValorDouble(yRecCenInf4, 6));

                #endregion
            }
            else if (codigoArquivo == "1329QUATROCANTOSESQDIREUMCENTRAL")
            {
                #region 1329QUATROCANTOSESQDIREUMCENTRAL

                int altura1329 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1329);

                if (altura1329 == 0)
                    throw new Exception("O campo altura da 1329 não foi informado." + mensagemErro);

                // Faz o recorte superior central
                float xRecCenSup1 = (largura / 2) - (12.399063F + descontoLap);
                float yRecCenSup1 = altura - (7F + descontoLap);

                float xRecCenSup2 = xRecCenSup1 + 3.1666125F;
                float yRecCenSup2 = yRecCenSup1 - 15.070063F;

                float xRecCenSup3 = xRecCenSup2 + 24.465876F;
                float yRecCenSup3 = yRecCenSup2;
                float iRecCenSup3 = (largura / 2) + descontoLap;
                float jRecCenSup3 = altura - 21F;
                float raioRecCenSup = 12.5F;

                float xRecCenSup4 = xRecCenSup3 + 3.166125F;
                float yRecCenSup4 = yRecCenSup1;

                // Faz o recorte inferior direito
                float xRecDirInf1 = largura - (7F + descontoLap);
                float yRecDirInf1 = altura1329 + 16.899063F;

                float xRecDirInf2 = xRecDirInf1 - 15.070063F;
                float yRecDirInf2 = yRecDirInf1 - 3.166125F;

                float xRecDirInf3 = xRecDirInf2;
                float yRecDirInf3 = yRecDirInf2 - 24.465876F;
                float iRecDirInf3 = largura - (19.5F + descontoLap);
                float jRecDirInf3 = altura1329 + descontoLap;
                float raioRecDirInf = 12.5F;

                float xRecDirInf4 = xRecDirInf1;
                float yRecDirInf4 = yRecDirInf3 - 3.166125F;

                // Faz o recorte superior direito
                float xRecDirSup1 = largura - (7F + descontoLap);
                float yRecDirSup1 = (altura - altura1329) + 16.899063F;

                float xRecDirSup2 = xRecDirInf2;
                float yRecDirSup2 = yRecDirSup1 - 3.166125F;

                float xRecDirSup3 = xRecDirInf2;
                float yRecDirSup3 = yRecDirSup2 - 24.465876F;
                float iRecDirSup3 = largura - (19.5F + descontoLap);
                float jRecDirSup3 = (altura + descontoLap) - altura1329;
                float raioRecDirSup = 12.5F;

                float xRecDirSup4 = xRecDirSup1;
                float yRecDirSup4 = yRecDirSup3 - 3.166125F;

                // Faz o recorte superior esquerdo
                float xRecEsqSup1 = 10F + descontoLap;
                float yRecEsqSup1 = yRecDirSup4;

                float xRecEsqSup2 = 26.570063F;
                float yRecEsqSup2 = yRecEsqSup1 + 3.166125F;

                float xRecEsqSup3 = xRecEsqSup2;
                float yRecEsqSup3 = yRecEsqSup2 + 24.465876F;
                float iRecEsqSup3 = 22.5F + descontoLap;
                float jRecEsqSup3 = (altura + descontoLap) - altura1329;
                float raioRecEsqSup = 12.5F;

                float xRecEsqSup4 = xRecEsqSup1;
                float yRecEsqSup4 = yRecEsqSup3 + 3.166125F;

                // Faz o recorte inferior esquerdo
                float xRecEsqInf1 = 10F + descontoLap;
                float yRecEsqInf1 = altura1329 - 13.899063F;

                float xRecEsqInf2 = xRecEsqSup3;
                float yRecEsqInf2 = yRecEsqInf1 + 3.166125F;

                float xRecEsqInf3 = xRecEsqSup3;
                float yRecEsqInf3 = yRecEsqInf2 + 24.465876F;
                float iRecEsqInf3 = 22.5F + descontoLap;
                float jRecEsqInf3 = altura1329 + descontoLap;
                float raioRecEsqInf = 12.5F;

                float xRecEsqInf4 = xRecEsqInf1;
                float yRecEsqInf4 = yRecEsqInf3 + 3.166125F;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte superior central
                    .Replace("?XRecCenSup1?", Formatacoes.TrataValorDouble(xRecCenSup1, 6))
                    .Replace("?YRecCenSup1?", Formatacoes.TrataValorDouble(yRecCenSup1, 6))

                    .Replace("?XRecCenSup2?", Formatacoes.TrataValorDouble(xRecCenSup2, 6))
                    .Replace("?YRecCenSup2?", Formatacoes.TrataValorDouble(yRecCenSup2, 6))

                    .Replace("?XRecCenSup3?", Formatacoes.TrataValorDouble(xRecCenSup3, 6))
                    .Replace("?YRecCenSup3?", Formatacoes.TrataValorDouble(yRecCenSup3, 6))
                    .Replace("?IRecCenSup3?", Formatacoes.TrataValorDouble(iRecCenSup3, 6))
                    .Replace("?JRecCenSup3?", Formatacoes.TrataValorDouble(jRecCenSup3, 6))
                    .Replace("?RaioRecCenSup?", Formatacoes.TrataValorDouble(raioRecCenSup, 6))

                    .Replace("?XRecCenSup4?", Formatacoes.TrataValorDouble(xRecCenSup4, 6))
                    .Replace("?YRecCenSup4?", Formatacoes.TrataValorDouble(yRecCenSup4, 6))

                    // Faz o recorte inferior direito
                    .Replace("?XRecDirInf1?", Formatacoes.TrataValorDouble(xRecDirInf1, 6))
                    .Replace("?YRecDirInf1?", Formatacoes.TrataValorDouble(yRecDirInf1, 6))

                    .Replace("?XRecDirInf2?", Formatacoes.TrataValorDouble(xRecDirInf2, 6))
                    .Replace("?YRecDirInf2?", Formatacoes.TrataValorDouble(yRecDirInf2, 6))

                    .Replace("?XRecDirInf3?", Formatacoes.TrataValorDouble(xRecDirInf3, 6))
                    .Replace("?YRecDirInf3?", Formatacoes.TrataValorDouble(yRecDirInf3, 6))
                    .Replace("?IRecDirInf3?", Formatacoes.TrataValorDouble(iRecDirInf3, 6))
                    .Replace("?JRecDirInf3?", Formatacoes.TrataValorDouble(jRecDirInf3, 6))
                    .Replace("?RaioRecDirInf?", Formatacoes.TrataValorDouble(raioRecDirInf, 6))

                    .Replace("?XRecDirInf4?", Formatacoes.TrataValorDouble(xRecDirInf4, 6))
                    .Replace("?YRecDirInf4?", Formatacoes.TrataValorDouble(yRecDirInf4, 6))

                    // Faz o recorte superior direito
                    .Replace("?XRecDirSup1?", Formatacoes.TrataValorDouble(xRecDirSup1, 6))
                    .Replace("?YRecDirSup1?", Formatacoes.TrataValorDouble(yRecDirSup1, 6))

                    .Replace("?XRecDirSup2?", Formatacoes.TrataValorDouble(xRecDirSup2, 6))
                    .Replace("?YRecDirSup2?", Formatacoes.TrataValorDouble(yRecDirSup2, 6))

                    .Replace("?XRecDirSup3?", Formatacoes.TrataValorDouble(xRecDirSup3, 6))
                    .Replace("?YRecDirSup3?", Formatacoes.TrataValorDouble(yRecDirSup3, 6))
                    .Replace("?IRecDirSup3?", Formatacoes.TrataValorDouble(iRecDirSup3, 6))
                    .Replace("?JRecDirSup3?", Formatacoes.TrataValorDouble(jRecDirSup3, 6))
                    .Replace("?RaioRecDirSup?", Formatacoes.TrataValorDouble(raioRecDirSup, 6))

                    .Replace("?XRecDirSup4?", Formatacoes.TrataValorDouble(xRecDirSup4, 6))
                    .Replace("?YRecDirSup4?", Formatacoes.TrataValorDouble(yRecDirSup4, 6))

                    // Faz o recorte superior esquerdo
                    .Replace("?XRecEsqSup1?", Formatacoes.TrataValorDouble(xRecEsqSup1, 6))
                    .Replace("?YRecEsqSup1?", Formatacoes.TrataValorDouble(yRecEsqSup1, 6))

                    .Replace("?XRecEsqSup2?", Formatacoes.TrataValorDouble(xRecEsqSup2, 6))
                    .Replace("?YRecEsqSup2?", Formatacoes.TrataValorDouble(yRecEsqSup2, 6))

                    .Replace("?XRecEsqSup3?", Formatacoes.TrataValorDouble(xRecEsqSup3, 6))
                    .Replace("?YRecEsqSup3?", Formatacoes.TrataValorDouble(yRecEsqSup3, 6))
                    .Replace("?IRecEsqSup3?", Formatacoes.TrataValorDouble(iRecEsqSup3, 6))
                    .Replace("?JRecEsqSup3?", Formatacoes.TrataValorDouble(jRecEsqSup3, 6))
                    .Replace("?RaioRecEsqSup?", Formatacoes.TrataValorDouble(raioRecEsqSup, 6))

                    .Replace("?XRecEsqSup4?", Formatacoes.TrataValorDouble(xRecEsqSup4, 6))
                    .Replace("?YRecEsqSup4?", Formatacoes.TrataValorDouble(yRecEsqSup4, 6))

                    // Faz o recorte inferior esquerdo
                    .Replace("?XRecEsqInf1?", Formatacoes.TrataValorDouble(xRecEsqInf1, 6))
                    .Replace("?YRecEsqInf1?", Formatacoes.TrataValorDouble(yRecEsqInf1, 6))

                    .Replace("?XRecEsqInf2?", Formatacoes.TrataValorDouble(xRecEsqInf2, 6))
                    .Replace("?YRecEsqInf2?", Formatacoes.TrataValorDouble(yRecEsqInf2, 6))

                    .Replace("?XRecEsqInf3?", Formatacoes.TrataValorDouble(xRecEsqInf3, 6))
                    .Replace("?YRecEsqInf3?", Formatacoes.TrataValorDouble(yRecEsqInf3, 6))
                    .Replace("?IRecEsqInf3?", Formatacoes.TrataValorDouble(iRecEsqInf3, 6))
                    .Replace("?JRecEsqInf3?", Formatacoes.TrataValorDouble(jRecEsqInf3, 6))
                    .Replace("?RaioRecEsqInf?", Formatacoes.TrataValorDouble(raioRecEsqInf, 6))

                    .Replace("?XRecEsqInf4?", Formatacoes.TrataValorDouble(xRecEsqInf4, 6))
                    .Replace("?YRecEsqInf4?", Formatacoes.TrataValorDouble(yRecEsqInf4, 6));

                #endregion
            }
            else if (codigoArquivo == "BASCULA")
            {
                #region BASCULA

                int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                // 1123 Esquerda
                decimal xTrinco1 = (decimal)descontoLap;
                decimal yTrinco1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)18.5);

                decimal distBordaXTrinco2 = (xTrinco1 + (decimal)26.867961);
                decimal distBordaYTrinco2 = yTrinco1 - (decimal)6.333456;

                decimal distBordaXTrinco3 = distBordaXTrinco2;
                decimal distBordaYTrinco3 = distBordaYTrinco2 - (decimal)24.333088;
                decimal posXCurvaTrinco3 = distBordaXTrinco3 - (decimal)2.867961;
                decimal posYCurvaTrinco3 = distBordaYTrinco3 + (decimal)12.16544;
                decimal raioCurvaTrinco3 = (decimal)12.5f;

                decimal distBordaXTrinco4 = (distBordaXTrinco3 - (decimal)26.867961);
                decimal distBordaYTrinco4 = (distBordaYTrinco3 - (decimal)6.333456);

                // 1523 Trinco
                decimal distBordaX1123Dir1 = (largura - (distBorda1523 + (decimal)18.5));
                decimal distBordaY1123Dir1 = ((decimal)altura + (decimal)descontoLap);

                decimal distBordaX1123Dir2 = distBordaX1123Dir1 + (decimal)6.333456;
                decimal distBordaY1123Dir2 = (distBordaY1123Dir1 - (decimal)25.645008);

                decimal distBordaX1123Dir3 = distBordaX1123Dir2 + (decimal)23.409736;
                decimal distBordaY1123Dir3 = distBordaY1123Dir2;
                decimal posXCurva1123Dir3 = distBordaX1123Dir3 - (decimal)11.704868;
                decimal posYCurva1123Dir3 = distBordaY1123Dir3 + (decimal)2.645008;
                decimal raioCurva1123Dir3 = (decimal)12f;

                decimal distBordaX1123Dir4 = (distBordaX1123Dir3 + (decimal)5.795132);
                decimal distBordaY1123Dir4 = (distBordaY1123Dir3 + (decimal)25.645008);

                // 1123 Direita
                decimal distBordaX1123Esq1 = ((decimal)largura + (decimal)descontoLap);
                decimal distBordaY1123Esq1 = ((((decimal)(altura / 2) + 50) - (decimal)18.5) + (decimal)descontoLap);

                decimal distBordaX1123Esq2 = (distBordaX1123Esq1 - (decimal)26.867961);
                decimal distBordaY1123Esq2 = distBordaY1123Esq1 + (decimal)6.333456;

                decimal distBordaX1123Esq3 = distBordaX1123Esq2;
                decimal distBordaY1123Esq3 = distBordaY1123Esq2 + (decimal)24.333088;
                decimal posXCurva1123Esq3 = distBordaX1123Esq3 + (decimal)2.867961;
                decimal posYCurva1123Esq3 = (distBordaY1123Esq3 - (decimal)12.166544);
                decimal raioCurva1123Esq3 = (decimal)12.5f;

                decimal distBordaX1123Esq4 = (largura + (decimal)descontoLap);
                decimal distBordaY1123Esq4 = (distBordaY1123Esq3 + (decimal)6.333456);

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte do trinco
                    .Replace("?CoordXTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?CoordYTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))

                    .Replace("?CoordXTrinco2?", Formatacoes.TrataValorDecimal(distBordaXTrinco2, 6))
                    .Replace("?CoordYTrinco2?", Formatacoes.TrataValorDecimal(distBordaYTrinco2, 6))

                    .Replace("?CoordXTrinco3?", Formatacoes.TrataValorDecimal(distBordaXTrinco3, 6))
                    .Replace("?CoordYTrinco3?", Formatacoes.TrataValorDecimal(distBordaYTrinco3, 6))
                    .Replace("?PosXTrinco3?", Formatacoes.TrataValorDecimal(posXCurvaTrinco3, 6))
                    .Replace("?PosYTrinco3?", Formatacoes.TrataValorDecimal(posYCurvaTrinco3, 6))
                    .Replace("?RaioTrinco3?", Formatacoes.TrataValorDecimal(raioCurvaTrinco3, 6))

                    .Replace("?CoordXTrinco4?", Formatacoes.TrataValorDecimal(distBordaXTrinco4, 6))
                    .Replace("?CoordYTrinco4?", Formatacoes.TrataValorDecimal(distBordaYTrinco4, 6))

                    // Faz o recorte na 1123 da direita
                    .Replace("?CoordX1123Dir1?", Formatacoes.TrataValorDecimal(distBordaX1123Dir1, 6))
                    .Replace("?CoordY1123Dir1?", Formatacoes.TrataValorDecimal(distBordaY1123Dir1, 6))

                    .Replace("?CoordX1123Dir2?", Formatacoes.TrataValorDecimal(distBordaX1123Dir2, 6))
                    .Replace("?CoordY1123Dir2?", Formatacoes.TrataValorDecimal(distBordaY1123Dir2, 6))

                    .Replace("?CoordX1123Dir3?", Formatacoes.TrataValorDecimal(distBordaX1123Dir3, 6))
                    .Replace("?CoordY1123Dir3?", Formatacoes.TrataValorDecimal(distBordaY1123Dir3, 6))
                    .Replace("?PosX1123Dir3?", Formatacoes.TrataValorDecimal(posXCurva1123Dir3, 6))
                    .Replace("?PosY1123Dir3?", Formatacoes.TrataValorDecimal(posYCurva1123Dir3, 6))
                    .Replace("?Raio1123Dir3?", Formatacoes.TrataValorDecimal(raioCurva1123Dir3, 6))

                    .Replace("?CoordX1123Dir4?", Formatacoes.TrataValorDecimal(distBordaX1123Dir4, 6))
                    .Replace("?CoordY1123Dir4?", Formatacoes.TrataValorDecimal(distBordaY1123Dir4, 6))

                    // Faz o recorte na 1123 da esquerda
                    .Replace("?CoordX1123Esq1?", Formatacoes.TrataValorDecimal(distBordaX1123Esq1, 6))
                    .Replace("?CoordY1123Esq1?", Formatacoes.TrataValorDecimal(distBordaY1123Esq1, 6))

                    .Replace("?CoordX1123Esq2?", Formatacoes.TrataValorDecimal(distBordaX1123Esq2, 6))
                    .Replace("?CoordY1123Esq2?", Formatacoes.TrataValorDecimal(distBordaY1123Esq2, 6))

                    .Replace("?CoordX1123Esq3?", Formatacoes.TrataValorDecimal(distBordaX1123Esq3, 6))
                    .Replace("?CoordY1123Esq3?", Formatacoes.TrataValorDecimal(distBordaY1123Esq3, 6))
                    .Replace("?PosX1123Esq3?", Formatacoes.TrataValorDecimal(posXCurva1123Esq3, 6))
                    .Replace("?PosY1123Esq3?", Formatacoes.TrataValorDecimal(posYCurva1123Esq3, 6))
                    .Replace("?Raio1123Esq3?", Formatacoes.TrataValorDecimal(raioCurva1123Esq3, 6))

                    .Replace("?CoordX1123Esq4?", Formatacoes.TrataValorDecimal(distBordaX1123Esq4, 6))
                    .Replace("?CoordY1123Esq4?", Formatacoes.TrataValorDecimal(distBordaY1123Esq4, 6));

                #endregion
            }
            else if (codigoArquivo == "BASCULACENTRALRECUADA")
            {
                #region BASCULACENTRALRECUADA

                decimal raioRecorte = 12;
                decimal recuo1123 = 10;

                // 1123 Esquerda
                decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                decimal yRecEsq1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)13.648746);

                decimal xRecEsq2 = xRecEsq1 + (decimal)14.746604;
                decimal yRecEsq2 = yRecEsq1 - (decimal)3.467301;

                decimal xRecEsq3 = xRecEsq2;
                decimal yRecEsq3 = yRecEsq2 - (decimal)23.36289;
                decimal iRecEsq3 = xRecEsq3 - (decimal)2.746604;
                decimal jRecEsq3 = yRecEsq3 + (decimal)11.681445;

                decimal xRecEsq4 = xRecEsq1;
                decimal yRecEsq4 = yRecEsq3 - (decimal)3.467301;

                // 1123 Direita
                decimal xRecDir1 = largura - (recuo1123 + (decimal)descontoLap);
                decimal yRecDir1 = (((decimal)(altura / 2) + 50) - (decimal)16.648746) + (decimal)descontoLap;

                decimal xRecDir2 = xRecDir1 - (decimal)14.746604;
                decimal yRecDir2 = yRecDir1 + (decimal)3.467301;

                decimal xRecDir3 = xRecDir2;
                decimal yRecDir3 = yRecDir2 + (decimal)23.36289;
                decimal iRecDir3 = xRecDir3 + (decimal)2.746604;
                decimal jRecDir3 = yRecDir3 - (decimal)11.681445;

                decimal xRecDir4 = xRecDir1;
                decimal yRecDir4 = yRecDir3 + (decimal)3.467301;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte na 1123 da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                    .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                    .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                    .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                    .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte, 6))

                    .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                    .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))

                    // Faz o recorte na 1123 da direita
                    .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                    .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                    .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                    .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                    .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                    .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                    .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                    .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                    .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecorte, 6))

                    .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                    .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6));

                #endregion
            }
            else if (codigoArquivo == "BASCULACOM1130RECUADA")
            {
                #region BASCULACOM1130RECUADA

                int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                decimal raioTrincoBasc = 12;
                decimal raioCurvaMenorRec = 4;
                decimal raioCurvaMaiorRec = (decimal)12.5;

                decimal recuo1130 = 10;
                decimal recuoTrinco = 12;

                // 1523 Trinco
                decimal xTrinco1 = (largura - (distBorda1523 + (decimal)18.5));
                decimal yTrinco1 = (decimal)altura - (recuoTrinco + (decimal)descontoLap);

                decimal xTrinco2 = xTrinco1 + (decimal)3.083431;
                decimal yTrinco2 = yTrinco1 - (decimal)13.645008;

                decimal xTrinco3 = xTrinco2 + (decimal)23.409736;
                decimal yTrinco3 = yTrinco2;
                decimal iTrinco3 = xTrinco3 - (decimal)11.704868;
                decimal jTrinco3 = yTrinco3 + (decimal)2.645008;

                decimal xTrinco4 = xTrinco3 + (decimal)3.083431;
                decimal yTrinco4 = yTrinco1;

                // 1130 Direita
                decimal xRecDir1 = largura - (recuo1130 + (decimal)descontoLap);
                decimal yRecDir1 = (((decimal)(altura / 2) + 50) - (decimal)29.5) + (decimal)descontoLap;

                decimal xRecDir2 = xRecDir1 - (decimal)0.992189;
                decimal yRecDir2 = yRecDir1;

                decimal xRecDir3CurvaMenor = xRecDir2 - (decimal)3.880682;
                decimal yRecDir3CurvaMenor = yRecDir2 - (decimal)3.030303;
                decimal iRecDir3CurvaMenor = xRecDir2;
                decimal jRecDir3CurvaMenor = yRecDir2 - 4;

                decimal xRecDir4CurvaMaior = xRecDir3CurvaMenor - (decimal)15.157432;
                decimal yRecDir4CurvaMaior = yRecDir3CurvaMenor + (decimal)15.157432;
                decimal iRecDir4CurvaMaior = xRecDir4CurvaMaior + (decimal)3.030303;
                decimal jRecDir4CurvaMaior = yRecDir4CurvaMaior - (decimal)12.127129;

                decimal xRecDir5CurvaMenor = xRecDir4CurvaMaior + (decimal)3.030303;
                decimal yRecDir5CurvaMenor = yRecDir4CurvaMaior + (decimal)3.880682;
                decimal iRecDir5CurvaMenor = xRecDir5CurvaMenor - 4;
                decimal jRecDir5CurvaMenor = yRecDir5CurvaMenor;

                decimal xRecDir6 = xRecDir5CurvaMenor;
                decimal yRecDir6 = yRecDir5CurvaMenor + (decimal)21.984378;

                decimal xRecDir7CurvaMenor = xRecDir6 - (decimal)3.030303;
                decimal yRecDir7CurvaMenor = yRecDir6 + (decimal)3.880682;
                decimal iRecDir7CurvaMenor = xRecDir7CurvaMenor - (decimal)0.969697;
                decimal jRecDir7CurvaMenor = yRecDir6;

                decimal xRecDir8CurvaMaior = xRecDir7CurvaMenor + (decimal)15.157432;
                decimal yRecDir8CurvaMaior = yRecDir7CurvaMenor + (decimal)15.157432;
                decimal iRecDir8CurvaMaior = xRecDir8CurvaMaior - (decimal)12.127129;
                decimal jRecDir8CurvaMaior = yRecDir8CurvaMaior - (decimal)3.030303;

                decimal xRecDir9CurvaMenor = xRecDir8CurvaMaior + (decimal)3.880682;
                decimal yRecDir9CurvaMenor = yRecDir8CurvaMaior - (decimal)3.030303;
                decimal iRecDir9CurvaMenor = xRecDir9CurvaMenor;
                decimal jRecDir9CurvaMenor = yRecDir9CurvaMenor + 4;

                decimal xRecDir10 = xRecDir9CurvaMenor + (decimal)0.992189;
                decimal yRecDir10 = yRecDir9CurvaMenor;

                // 1130 Esquerda
                decimal xRecEsq1 = recuo1130 + (decimal)descontoLap;
                decimal yRecEsq1 = (((decimal)(altura / 2) + 50) - (decimal)29.5) + (decimal)descontoLap;

                decimal xRecEsq2 = xRecEsq1 + (decimal)0.992189;
                decimal yRecEsq2 = yRecEsq1;

                decimal xRecEsq3CurvaMenor = xRecEsq2 + (decimal)3.880682;
                decimal yRecEsq3CurvaMenor = yRecEsq2 - (decimal)3.030303;
                decimal iRecEsq3CurvaMenor = xRecEsq2;
                decimal jRecEsq3CurvaMenor = yRecEsq2 - 4;

                decimal xRecEsq4CurvaMaior = xRecEsq3CurvaMenor + (decimal)15.157432;
                decimal yRecEsq4CurvaMaior = yRecEsq3CurvaMenor + (decimal)15.157432;
                decimal iRecEsq4CurvaMaior = xRecEsq4CurvaMaior - (decimal)3.030303;
                decimal jRecEsq4CurvaMaior = yRecEsq4CurvaMaior - (decimal)12.127129;

                decimal xRecEsq5CurvaMenor = xRecEsq4CurvaMaior - (decimal)3.030303;
                decimal yRecEsq5CurvaMenor = yRecEsq4CurvaMaior + (decimal)3.880682;
                decimal iRecEsq5CurvaMenor = xRecEsq5CurvaMenor + 4;
                decimal jRecEsq5CurvaMenor = yRecEsq5CurvaMenor;

                decimal xRecEsq6 = xRecEsq5CurvaMenor;
                decimal yRecEsq6 = yRecEsq5CurvaMenor + (decimal)21.984378;

                decimal xRecEsq7CurvaMenor = xRecEsq6 + (decimal)3.030303;
                decimal yRecEsq7CurvaMenor = yRecEsq6 + (decimal)3.880682;
                decimal iRecEsq7CurvaMenor = xRecEsq7CurvaMenor + (decimal)0.969697;
                decimal jRecEsq7CurvaMenor = yRecEsq6;

                decimal xRecEsq8CurvaMaior = xRecEsq7CurvaMenor - (decimal)15.157432;
                decimal yRecEsq8CurvaMaior = yRecEsq7CurvaMenor + (decimal)15.157432;
                decimal iRecEsq8CurvaMaior = xRecEsq8CurvaMaior + (decimal)12.127129;
                decimal jRecEsq8CurvaMaior = yRecEsq8CurvaMaior - (decimal)3.030303;

                decimal xRecEsq9CurvaMenor = xRecEsq8CurvaMaior - (decimal)3.880682;
                decimal yRecEsq9CurvaMenor = yRecEsq8CurvaMaior - (decimal)3.030303;
                decimal iRecEsq9CurvaMenor = xRecEsq9CurvaMenor;
                decimal jRecEsq9CurvaMenor = yRecEsq9CurvaMenor + 4;

                decimal xRecEsq10 = xRecEsq9CurvaMenor - (decimal)0.992189;
                decimal yRecEsq10 = yRecEsq9CurvaMenor;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte do trinco
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))

                    .Replace("?XTrinco3?", Formatacoes.TrataValorDecimal(xTrinco3, 6))
                    .Replace("?YTrinco3?", Formatacoes.TrataValorDecimal(yTrinco3, 6))
                    .Replace("?ITrinco3?", Formatacoes.TrataValorDecimal(iTrinco3, 6))
                    .Replace("?JTrinco3?", Formatacoes.TrataValorDecimal(jTrinco3, 6))
                    .Replace("?RaioTrinco3?", Formatacoes.TrataValorDecimal(raioTrincoBasc, 6))

                    .Replace("?XTrinco4?", Formatacoes.TrataValorDecimal(xTrinco4, 6))
                    .Replace("?YTrinco4?", Formatacoes.TrataValorDecimal(yTrinco4, 6))

                    // Faz o recorte na 1123 da direita
                    .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                    .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                    .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                    .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                    .Replace("?XRecDir3CurvaMenor?", Formatacoes.TrataValorDecimal(xRecDir3CurvaMenor, 6))
                    .Replace("?YRecDir3CurvaMenor?", Formatacoes.TrataValorDecimal(yRecDir3CurvaMenor, 6))
                    .Replace("?IRecDir3CurvaMenor?", Formatacoes.TrataValorDecimal(iRecDir3CurvaMenor, 6))
                    .Replace("?JRecDir3CurvaMenor?", Formatacoes.TrataValorDecimal(jRecDir3CurvaMenor, 6))
                    .Replace("?RaioRecDir3CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecDir4CurvaMaior?", Formatacoes.TrataValorDecimal(xRecDir4CurvaMaior, 6))
                    .Replace("?YRecDir4CurvaMaior?", Formatacoes.TrataValorDecimal(yRecDir4CurvaMaior, 6))
                    .Replace("?IRecDir4CurvaMaior?", Formatacoes.TrataValorDecimal(iRecDir4CurvaMaior, 6))
                    .Replace("?JRecDir4CurvaMaior?", Formatacoes.TrataValorDecimal(jRecDir4CurvaMaior, 6))
                    .Replace("?RaioRecDir4CurvaMaior?", Formatacoes.TrataValorDecimal(raioCurvaMaiorRec, 6))

                    .Replace("?XRecDir5CurvaMenor?", Formatacoes.TrataValorDecimal(xRecDir5CurvaMenor, 6))
                    .Replace("?YRecDir5CurvaMenor?", Formatacoes.TrataValorDecimal(yRecDir5CurvaMenor, 6))
                    .Replace("?IRecDir5CurvaMenor?", Formatacoes.TrataValorDecimal(iRecDir5CurvaMenor, 6))
                    .Replace("?JRecDir5CurvaMenor?", Formatacoes.TrataValorDecimal(jRecDir5CurvaMenor, 6))
                    .Replace("?RaioRecDir5CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecDir6?", Formatacoes.TrataValorDecimal(xRecDir6, 6))
                    .Replace("?YRecDir6?", Formatacoes.TrataValorDecimal(yRecDir6, 6))

                    .Replace("?XRecDir7CurvaMenor?", Formatacoes.TrataValorDecimal(xRecDir7CurvaMenor, 6))
                    .Replace("?YRecDir7CurvaMenor?", Formatacoes.TrataValorDecimal(yRecDir7CurvaMenor, 6))
                    .Replace("?IRecDir7CurvaMenor?", Formatacoes.TrataValorDecimal(iRecDir7CurvaMenor, 6))
                    .Replace("?JRecDir7CurvaMenor?", Formatacoes.TrataValorDecimal(jRecDir7CurvaMenor, 6))
                    .Replace("?RaioRecDir7CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecDir8CurvaMaior?", Formatacoes.TrataValorDecimal(xRecDir8CurvaMaior, 6))
                    .Replace("?YRecDir8CurvaMaior?", Formatacoes.TrataValorDecimal(yRecDir8CurvaMaior, 6))
                    .Replace("?IRecDir8CurvaMaior?", Formatacoes.TrataValorDecimal(iRecDir8CurvaMaior, 6))
                    .Replace("?JRecDir8CurvaMaior?", Formatacoes.TrataValorDecimal(jRecDir8CurvaMaior, 6))
                    .Replace("?RaioRecDir8CurvaMaior?", Formatacoes.TrataValorDecimal(raioCurvaMaiorRec, 6))

                    .Replace("?XRecDir9CurvaMenor?", Formatacoes.TrataValorDecimal(xRecDir9CurvaMenor, 6))
                    .Replace("?YRecDir9CurvaMenor?", Formatacoes.TrataValorDecimal(yRecDir9CurvaMenor, 6))
                    .Replace("?IRecDir9CurvaMenor?", Formatacoes.TrataValorDecimal(iRecDir9CurvaMenor, 6))
                    .Replace("?JRecDir9CurvaMenor?", Formatacoes.TrataValorDecimal(jRecDir9CurvaMenor, 6))
                    .Replace("?RaioRecDir9CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecDir10?", Formatacoes.TrataValorDecimal(xRecDir10, 10))
                    .Replace("?YRecDir10?", Formatacoes.TrataValorDecimal(yRecDir10, 10))

                    // Faz o recorte na 1123 da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3CurvaMenor?", Formatacoes.TrataValorDecimal(xRecEsq3CurvaMenor, 6))
                    .Replace("?YRecEsq3CurvaMenor?", Formatacoes.TrataValorDecimal(yRecEsq3CurvaMenor, 6))
                    .Replace("?IRecEsq3CurvaMenor?", Formatacoes.TrataValorDecimal(iRecEsq3CurvaMenor, 6))
                    .Replace("?JRecEsq3CurvaMenor?", Formatacoes.TrataValorDecimal(jRecEsq3CurvaMenor, 6))
                    .Replace("?RaioRecEsq3CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecEsq4CurvaMaior?", Formatacoes.TrataValorDecimal(xRecEsq4CurvaMaior, 6))
                    .Replace("?YRecEsq4CurvaMaior?", Formatacoes.TrataValorDecimal(yRecEsq4CurvaMaior, 6))
                    .Replace("?IRecEsq4CurvaMaior?", Formatacoes.TrataValorDecimal(iRecEsq4CurvaMaior, 6))
                    .Replace("?JRecEsq4CurvaMaior?", Formatacoes.TrataValorDecimal(jRecEsq4CurvaMaior, 6))
                    .Replace("?RaioRecEsq4CurvaMaior?", Formatacoes.TrataValorDecimal(raioCurvaMaiorRec, 6))

                    .Replace("?XRecEsq5CurvaMenor?", Formatacoes.TrataValorDecimal(xRecEsq5CurvaMenor, 6))
                    .Replace("?YRecEsq5CurvaMenor?", Formatacoes.TrataValorDecimal(yRecEsq5CurvaMenor, 6))
                    .Replace("?IRecEsq5CurvaMenor?", Formatacoes.TrataValorDecimal(iRecEsq5CurvaMenor, 6))
                    .Replace("?JRecEsq5CurvaMenor?", Formatacoes.TrataValorDecimal(jRecEsq5CurvaMenor, 6))
                    .Replace("?RaioRecEsq5CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecEsq6?", Formatacoes.TrataValorDecimal(xRecEsq6, 6))
                    .Replace("?YRecEsq6?", Formatacoes.TrataValorDecimal(yRecEsq6, 6))

                    .Replace("?XRecEsq7CurvaMenor?", Formatacoes.TrataValorDecimal(xRecEsq7CurvaMenor, 6))
                    .Replace("?YRecEsq7CurvaMenor?", Formatacoes.TrataValorDecimal(yRecEsq7CurvaMenor, 6))
                    .Replace("?IRecEsq7CurvaMenor?", Formatacoes.TrataValorDecimal(iRecEsq7CurvaMenor, 6))
                    .Replace("?JRecEsq7CurvaMenor?", Formatacoes.TrataValorDecimal(jRecEsq7CurvaMenor, 6))
                    .Replace("?RaioRecEsq7CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecEsq8CurvaMaior?", Formatacoes.TrataValorDecimal(xRecEsq8CurvaMaior, 6))
                    .Replace("?YRecEsq8CurvaMaior?", Formatacoes.TrataValorDecimal(yRecEsq8CurvaMaior, 6))
                    .Replace("?IRecEsq8CurvaMaior?", Formatacoes.TrataValorDecimal(iRecEsq8CurvaMaior, 6))
                    .Replace("?JRecEsq8CurvaMaior?", Formatacoes.TrataValorDecimal(jRecEsq8CurvaMaior, 6))
                    .Replace("?RaioRecEsq8CurvaMaior?", Formatacoes.TrataValorDecimal(raioCurvaMaiorRec, 6))

                    .Replace("?XRecEsq9CurvaMenor?", Formatacoes.TrataValorDecimal(xRecEsq9CurvaMenor, 6))
                    .Replace("?YRecEsq9CurvaMenor?", Formatacoes.TrataValorDecimal(yRecEsq9CurvaMenor, 6))
                    .Replace("?IRecEsq9CurvaMenor?", Formatacoes.TrataValorDecimal(iRecEsq9CurvaMenor, 6))
                    .Replace("?JRecEsq9CurvaMenor?", Formatacoes.TrataValorDecimal(jRecEsq9CurvaMenor, 6))
                    .Replace("?RaioRecEsq9CurvaMenor?", Formatacoes.TrataValorDecimal(raioCurvaMenorRec, 6))

                    .Replace("?XRecEsq10?", Formatacoes.TrataValorDecimal(xRecEsq10, 10))
                    .Replace("?YRecEsq10?", Formatacoes.TrataValorDecimal(yRecEsq10, 10));

                #endregion
            }
            else if (codigoArquivo == "BASCULAFIXOSUPERIOR")
            {
                #region BASCULAFIXOSUPERIOR

                int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                float raio1524 = 10;

                float x1524 = distBorda1523 + descontoLap + raio1524;
                float y1524 = 25 + descontoLap;
                float i1524 = x1524 - raio1524;
                float j1524 = y1524;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da 1524
                    .Replace("?X1524?", Formatacoes.TrataValorDouble(x1524, 6))
                    .Replace("?Y1524?", Formatacoes.TrataValorDouble(y1524, 6))
                    .Replace("?I1524?", Formatacoes.TrataValorDouble(i1524, 6))
                    .Replace("?J1524?", Formatacoes.TrataValorDouble(j1524, 6))
                    .Replace("?Raio1524?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6));

                #endregion
            }
            else if (codigoArquivo == "BASCULALATERALRECUADA")
            {
                #region BASCULALATERALRECUADA

                decimal raioRecorte = 12;
                decimal recuo1123 = 10;

                // 1123 Esquerda
                decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                decimal yRecEsq1 = (((decimal)(altura / 2) + 50) + (decimal)descontoLap + (decimal)13.648746);

                decimal xRecEsq2 = xRecEsq1 + (decimal)14.746604;
                decimal yRecEsq2 = yRecEsq1 - (decimal)3.467301;

                decimal xRecEsq3 = xRecEsq2;
                decimal yRecEsq3 = yRecEsq2 - (decimal)23.36289;
                decimal iRecEsq3 = xRecEsq3 - (decimal)2.746604;
                decimal jRecEsq3 = yRecEsq3 + (decimal)11.681445;

                decimal xRecEsq4 = xRecEsq1;
                decimal yRecEsq4 = yRecEsq3 - (decimal)3.467301;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte na 1123 da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                    .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                    .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                    .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                    .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte, 6))

                    .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                    .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6));

                #endregion
            }
            else if (codigoArquivo == "BASCULARECUADA")
            {
                #region BASCULARECUADA

                int distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                int altura1123Bascula = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1123Bascula);

                // Se a medida altura1123Bascula existir no projeto, força o usuário a informá-la
                uint idProjetoModelo = ItemProjetoDAO.Instance.GetIdProjetoModelo(pecaItemProjeto.IdItemProjeto);
                if (altura1123Bascula == 0 && MedidaProjetoModeloDAO.Instance.ExisteMedidaProjeto(idProjetoModelo, "Posição da 1123 da bascula"))
                    throw new Exception("A posição da 1123 deve ser informada." + mensagemErro);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                if (altura1123Bascula == 0)
                    altura1123Bascula = (int)(altura / 2) + 50;

                decimal raioRecorte = 12;
                decimal raioTrincoBasc = 12;
                decimal recuo1123 = 10;
                decimal recuoTrinco = 12;

                // 1123 Esquerda
                decimal xRecEsq1 = recuo1123 + (decimal)descontoLap;
                decimal yRecEsq1 = ((decimal)altura1123Bascula + (decimal)descontoLap + (decimal)13.648746);

                decimal xRecEsq2 = xRecEsq1 + (decimal)14.746604;
                decimal yRecEsq2 = yRecEsq1 - (decimal)3.467301;

                decimal xRecEsq3 = xRecEsq2;
                decimal yRecEsq3 = yRecEsq2 - (decimal)23.36289;
                decimal iRecEsq3 = xRecEsq3 - (decimal)2.746604;
                decimal jRecEsq3 = yRecEsq3 + (decimal)11.681445;

                decimal xRecEsq4 = xRecEsq1;
                decimal yRecEsq4 = yRecEsq3 - (decimal)3.467301;

                // 1523 Trinco
                decimal xTrinco1 = (largura - (distBorda1523 + (decimal)18.5));
                decimal yTrinco1 = (decimal)altura - (recuoTrinco + (decimal)descontoLap);

                decimal xTrinco2 = xTrinco1 + (decimal)3.083431;
                decimal yTrinco2 = yTrinco1 - (decimal)13.645008;

                decimal xTrinco3 = xTrinco2 + (decimal)23.409736;
                decimal yTrinco3 = yTrinco2;
                decimal iTrinco3 = xTrinco3 - (decimal)11.704868;
                decimal jTrinco3 = yTrinco3 + (decimal)2.645008;

                decimal xTrinco4 = xTrinco3 + (decimal)3.083431;
                decimal yTrinco4 = yTrinco1;

                // 1123 Direita
                decimal xRecDir1 = largura - (recuo1123 + (decimal)descontoLap);
                decimal yRecDir1 = ((decimal)altura1123Bascula - (decimal)16.648746) + (decimal)descontoLap;

                decimal xRecDir2 = xRecDir1 - (decimal)14.746604;
                decimal yRecDir2 = yRecDir1 + (decimal)3.467301;

                decimal xRecDir3 = xRecDir2;
                decimal yRecDir3 = yRecDir2 + (decimal)23.36289;
                decimal iRecDir3 = xRecDir3 + (decimal)2.746604;
                decimal jRecDir3 = yRecDir3 - (decimal)11.681445;

                decimal xRecDir4 = xRecDir1;
                decimal yRecDir4 = yRecDir3 + (decimal)3.467301;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte na 1123 da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                    .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                    .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                    .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                    .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecorte, 6))

                    .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                    .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))

                    // Faz o recorte do trinco
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))

                    .Replace("?XTrinco3?", Formatacoes.TrataValorDecimal(xTrinco3, 6))
                    .Replace("?YTrinco3?", Formatacoes.TrataValorDecimal(yTrinco3, 6))
                    .Replace("?ITrinco3?", Formatacoes.TrataValorDecimal(iTrinco3, 6))
                    .Replace("?JTrinco3?", Formatacoes.TrataValorDecimal(jTrinco3, 6))
                    .Replace("?RaioTrinco3?", Formatacoes.TrataValorDecimal(raioTrincoBasc, 6))

                    .Replace("?XTrinco4?", Formatacoes.TrataValorDecimal(xTrinco4, 6))
                    .Replace("?YTrinco4?", Formatacoes.TrataValorDecimal(yTrinco4, 6))

                    // Faz o recorte na 1123 da direita
                    .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                    .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                    .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                    .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                    .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                    .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                    .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                    .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                    .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecorte, 6))

                    .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                    .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6));

                #endregion
            }
            else if (codigoArquivo == "BOXABRIR")
            {
                #region BOXABRIR

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int altura1114 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaDobradica);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (altura1114 == 0)
                    throw new Exception("Informe a altura da 1114." + mensagemErro);

                int raioFuroFech = 10;
                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;
                int distBordaXFuroPux = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador
                int alturaDobradica = altura1114;

                float xFuroFech1 = 26.5f + raioFuroFech;
                float yFuroFech1 = (alturaDobradica - 25) + descontoLap;
                float iFuroFech1 = xFuroFech1 - raioFuroFech;
                float jFuroFech1 = yFuroFech1;

                float xFuroFech2 = 26.5f + raioFuroFech;
                float yFuroFech2 = (alturaDobradica + 25) + descontoLap;
                float iFuroFech2 = xFuroFech2 - raioFuroFech;
                float jFuroFech2 = yFuroFech2;

                float xFuroFech3 = 26.5f + raioFuroFech;
                float yFuroFech3 = (((altura - alturaDobradica) - 25) - descontoLap);
                float iFuroFech3 = xFuroFech3 - raioFuroFech;
                float jFuroFech3 = yFuroFech3;

                float xFuroFech4 = 26.5f + raioFuroFech;
                float yFuroFech4 = (((altura - alturaDobradica) + 25) - descontoLap);
                float iFuroFech4 = xFuroFech4 - raioFuroFech;
                float jFuroFech4 = yFuroFech4;

                float xFuroPux = ((largura - distBordaXFuroPux) - descontoLap) + (raioFuroPux * 2);
                float yFuroPux = alturaPuxador + descontoLap;
                float iFuroPux = xFuroPux - raioFuroPux;
                float jFuroPux = yFuroPux;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da fechadura 1 e faz o furo
                    .Replace("?XFuroFech1?", Formatacoes.TrataValorDouble(xFuroFech1, 6))
                    .Replace("?YFuroFech1?", Formatacoes.TrataValorDouble(yFuroFech1, 6))
                    .Replace("?IFuroFech1?", Formatacoes.TrataValorDouble(iFuroFech1, 6))
                    .Replace("?JFuroFech1?", Formatacoes.TrataValorDouble(jFuroFech1, 6))
                    .Replace("?RaioFuroFech1?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 2 e faz o furo
                    .Replace("?XFuroFech2?", Formatacoes.TrataValorDouble(xFuroFech2, 6))
                    .Replace("?YFuroFech2?", Formatacoes.TrataValorDouble(yFuroFech2, 6))
                    .Replace("?IFuroFech2?", Formatacoes.TrataValorDouble(iFuroFech2, 6))
                    .Replace("?JFuroFech2?", Formatacoes.TrataValorDouble(jFuroFech2, 6))
                    .Replace("?RaioFuroFech2?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 3 e faz o furo
                    .Replace("?XFuroFech3?", Formatacoes.TrataValorDouble(xFuroFech3, 6))
                    .Replace("?YFuroFech3?", Formatacoes.TrataValorDouble(yFuroFech3, 6))
                    .Replace("?IFuroFech3?", Formatacoes.TrataValorDouble(iFuroFech3, 6))
                    .Replace("?JFuroFech3?", Formatacoes.TrataValorDouble(jFuroFech3, 6))
                    .Replace("?RaioFuroFech3?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 4 e faz o furo
                    .Replace("?XFuroFech4?", Formatacoes.TrataValorDouble(xFuroFech4, 6))
                    .Replace("?YFuroFech4?", Formatacoes.TrataValorDouble(yFuroFech4, 6))
                    .Replace("?IFuroFech4?", Formatacoes.TrataValorDouble(iFuroFech4, 6))
                    .Replace("?JFuroFech4?", Formatacoes.TrataValorDouble(jFuroFech4, 6))
                    .Replace("?RaioFuroFech4?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDouble(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDouble(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDouble(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDouble(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "BOXABRIRPUXDUPL1520")
            {
                #region BOXABRIRPUXDUPL1520

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int altura1114 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaDobradica);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (altura1114 == 0)
                    throw new Exception("Informe a altura da 1114." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroFech = 10;
                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;
                decimal distBordaXFuroPux = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador
                decimal alturaDobradica = altura1114;
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz as dobradiças
                decimal xFuroFech1 = (decimal)26.5 + raioFuroFech;
                decimal yFuroFech1 = (decimal)(alturaDobradica - 25) + (decimal)descontoLap;
                decimal iFuroFech1 = xFuroFech1 - raioFuroFech;
                decimal jFuroFech1 = yFuroFech1;

                decimal xFuroFech2 = (decimal)26.5 + raioFuroFech;
                decimal yFuroFech2 = (decimal)(alturaDobradica + 25) + (decimal)descontoLap;
                decimal iFuroFech2 = xFuroFech2 - raioFuroFech;
                decimal jFuroFech2 = yFuroFech2;

                decimal xFuroFech3 = (decimal)26.5f + raioFuroFech;
                decimal yFuroFech3 = (decimal)altura - alturaDobradica - 25 - (decimal)descontoLap;
                decimal iFuroFech3 = xFuroFech3 - raioFuroFech;
                decimal jFuroFech3 = yFuroFech3;

                decimal xFuroFech4 = (decimal)26.5 + raioFuroFech;
                decimal yFuroFech4 = (decimal)altura - alturaDobradica + 25 - (decimal)descontoLap;
                decimal iFuroFech4 = xFuroFech4 - raioFuroFech;
                decimal jFuroFech4 = yFuroFech4;

                // Faz o recorte da fechadura 1520
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da fechadura 1 e faz o furo
                    .Replace("?XFuroFech1?", Formatacoes.TrataValorDecimal(xFuroFech1, 6))
                    .Replace("?YFuroFech1?", Formatacoes.TrataValorDecimal(yFuroFech1, 6))
                    .Replace("?IFuroFech1?", Formatacoes.TrataValorDecimal(iFuroFech1, 6))
                    .Replace("?JFuroFech1?", Formatacoes.TrataValorDecimal(jFuroFech1, 6))
                    .Replace("?RaioFuroFech1?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 2 e faz o furo
                    .Replace("?XFuroFech2?", Formatacoes.TrataValorDecimal(xFuroFech2, 6))
                    .Replace("?YFuroFech2?", Formatacoes.TrataValorDecimal(yFuroFech2, 6))
                    .Replace("?IFuroFech2?", Formatacoes.TrataValorDecimal(iFuroFech2, 6))
                    .Replace("?JFuroFech2?", Formatacoes.TrataValorDecimal(jFuroFech2, 6))
                    .Replace("?RaioFuroFech2?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 3 e faz o furo
                    .Replace("?XFuroFech3?", Formatacoes.TrataValorDecimal(xFuroFech3, 6))
                    .Replace("?YFuroFech3?", Formatacoes.TrataValorDecimal(yFuroFech3, 6))
                    .Replace("?IFuroFech3?", Formatacoes.TrataValorDecimal(iFuroFech3, 6))
                    .Replace("?JFuroFech3?", Formatacoes.TrataValorDecimal(jFuroFech3, 6))
                    .Replace("?RaioFuroFech3?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 4 e faz o furo
                    .Replace("?XFuroFech4?", Formatacoes.TrataValorDecimal(xFuroFech4, 6))
                    .Replace("?YFuroFech4?", Formatacoes.TrataValorDecimal(yFuroFech4, 6))
                    .Replace("?IFuroFech4?", Formatacoes.TrataValorDecimal(iFuroFech4, 6))
                    .Replace("?JFuroFech4?", Formatacoes.TrataValorDecimal(jFuroFech4, 6))
                    .Replace("?RaioFuroFech4?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Faz o recorte da fechadura 1520
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz os furos do puxador
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "BOXABRIRSEMPUX")
            {
                #region BOXABRIRSEMPUX

                var altura1114 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaDobradica);

                if (altura1114 == 0)
                    throw new Exception("Informe a altura da 1114." + mensagemErro);

                int raioFuroDobradica = 10;
                int alturaDobradica = altura1114;

                float xFuroDobradica1 = 26.5f + raioFuroDobradica;
                float yFuroDobradica1 = (alturaDobradica - 25) + descontoLap;
                float iFuroDobradica1 = xFuroDobradica1 - raioFuroDobradica;
                float jFuroDobradica1 = yFuroDobradica1;

                float xFuroDobradica2 = 26.5f + raioFuroDobradica;
                float yFuroDobradica2 = (alturaDobradica + 25) + descontoLap;
                float iFuroDobradica2 = xFuroDobradica2 - raioFuroDobradica;
                float jFuroDobradica2 = yFuroDobradica2;

                float xFuroDobradica3 = 26.5f + raioFuroDobradica;
                float yFuroDobradica3 = (((altura - alturaDobradica) - 25) - descontoLap);
                float iFuroDobradica3 = xFuroDobradica3 - raioFuroDobradica;
                float jFuroDobradica3 = yFuroDobradica3;

                float xFuroDobradica4 = 26.5f + raioFuroDobradica;
                float yFuroDobradica4 = (((altura - alturaDobradica) + 25) - descontoLap);
                float iFuroDobradica4 = xFuroDobradica4 - raioFuroDobradica;
                float jFuroDobradica4 = yFuroDobradica4;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da fechadura 1 e faz o furo
                    .Replace("?XFuroDobradica1?", Formatacoes.TrataValorDouble(xFuroDobradica1, 6))
                    .Replace("?YFuroDobradica1?", Formatacoes.TrataValorDouble(yFuroDobradica1, 6))
                    .Replace("?IFuroDobradica1?", Formatacoes.TrataValorDouble(iFuroDobradica1, 6))
                    .Replace("?JFuroDobradica1?", Formatacoes.TrataValorDouble(jFuroDobradica1, 6))
                    .Replace("?RaioFuroDobradica1?", Formatacoes.TrataValorDouble(raioFuroDobradica, 6))

                    // Posiciona o CNC no furo da fechadura 2 e faz o furo
                    .Replace("?XFuroDobradica2?", Formatacoes.TrataValorDouble(xFuroDobradica2, 6))
                    .Replace("?YFuroDobradica2?", Formatacoes.TrataValorDouble(yFuroDobradica2, 6))
                    .Replace("?IFuroDobradica2?", Formatacoes.TrataValorDouble(iFuroDobradica2, 6))
                    .Replace("?JFuroDobradica2?", Formatacoes.TrataValorDouble(jFuroDobradica2, 6))
                    .Replace("?RaioFuroDobradica2?", Formatacoes.TrataValorDouble(raioFuroDobradica, 6))

                    // Posiciona o CNC no furo da fechadura 3 e faz o furo
                    .Replace("?XFuroDobradica3?", Formatacoes.TrataValorDouble(xFuroDobradica3, 6))
                    .Replace("?YFuroDobradica3?", Formatacoes.TrataValorDouble(yFuroDobradica3, 6))
                    .Replace("?IFuroDobradica3?", Formatacoes.TrataValorDouble(iFuroDobradica3, 6))
                    .Replace("?JFuroDobradica3?", Formatacoes.TrataValorDouble(jFuroDobradica3, 6))
                    .Replace("?RaioFuroDobradica3?", Formatacoes.TrataValorDouble(raioFuroDobradica, 6))

                    // Posiciona o CNC no furo da fechadura 4 e faz o furo
                    .Replace("?XFuroDobradica4?", Formatacoes.TrataValorDouble(xFuroDobradica4, 6))
                    .Replace("?YFuroDobradica4?", Formatacoes.TrataValorDouble(yFuroDobradica4, 6))
                    .Replace("?IFuroDobradica4?", Formatacoes.TrataValorDouble(iFuroDobradica4, 6))
                    .Replace("?JFuroDobradica4?", Formatacoes.TrataValorDouble(jFuroDobradica4, 6))
                    .Replace("?RaioFuroDobradica4?", Formatacoes.TrataValorDouble(raioFuroDobradica, 6));

                #endregion
            }
            else if (codigoArquivo == "BOXWG")
            {
                #region BOXWG

                // BOX850WG
                int distBordaXFuroRoldana1 = ProducaoConfig.PosXRoldanaBoxWG > 0 ? ProducaoConfig.PosXRoldanaBoxWG : config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1
                int raioFuroRoldana1 = config.Roldana.RaioRoldana;

                int distBordaXFuroRoldana2 = distBordaXFuroRoldana1; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2
                int raioFuroRoldana2 = config.Roldana.RaioRoldana;

                int distBordaXFuroPux = 50; // Distância da direita para a esquerda do furo do puxador
                //int distBordaYFuroPux = 50; // Distância de cima para baixo do furo do puxador
                int raioFuroPux = ProducaoConfig.RaioFuroPuxadorBoxWG;
                int alturaPuxador = ProducaoConfig.AlturaPuxadorBoxWG;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap + raioFuroRoldana1, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))

                    // Comando para fazer o furo da roldana 1
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDouble(raioFuroRoldana1, 6))

                    // Posiciona o CNC no furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + raioFuroRoldana2, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))

                    // Comando para fazer o furo da roldana 2
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDouble(raioFuroRoldana2, 6))

                    // Posiciona o CNC no furo do puxador
                    .Replace("?CoordXFuroPux?", Formatacoes.TrataValorDouble((largura - distBordaXFuroPux) + descontoLap + raioFuroPux, 6))
                    .Replace("?CoordYFuroPux?", Formatacoes.TrataValorDouble(alturaPuxador + descontoLap, 6))

                    // Comando para fazer o furo do puxador
                    .Replace("?PosXFuroPux?", Formatacoes.TrataValorDouble((largura + descontoLap) - distBordaXFuroPux, 6))
                    .Replace("?PosYFuroPux?", Formatacoes.TrataValorDouble(alturaPuxador + descontoLap, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "CR1510SEMPUX1520NOTRINCO")
            {
                #region CR1510SEMPUX1520NOTRINCO

                float alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                float distBorda1520 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaTrinco);

                if (alturaFechadura == 0)
                    throw new Exception("O campo altura da fechadura não foi informado." + mensagemErro);

                distBorda1520 = distBorda1520 == 0 ? 100 : distBorda1520;
                float raioCurvaFech = 8F;

                float distBordaXFuroRoldana1 = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1
                float raioFuroRoldana1 = config.Roldana.RaioRoldana;

                float distBordaXFuroRoldana2 = config.Roldana.PosXRoldana; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2
                float raioFuroRoldana2 = config.Roldana.RaioRoldana;

                // FAZ O RECORTE DA FECHADURA 1510.
                float distBordaXIniFech = config.Fechadura1510.DistBordaFechadura1510 + (10 + descontoLap); // Distância da direita para a esquerda inicial do furo da fechadura
                float distBordaYIniFech = (alturaFechadura - 60F) + descontoLap;

                float distBordaXSegFech = distBordaXIniFech + config.Fechadura1510.DistBordaFechadura1510; // Distância da direita para a esquerda secundária do furo da fechadura
                float distBordaYSegFech = distBordaYIniFech;

                float distBordaXCurva1 = distBordaXSegFech + 5F;
                float distBordaYCurva1 = distBordaYSegFech + 5F;
                float posXCurva1 = distBordaXSegFech;
                float posYCurva1 = distBordaYCurva1;
                float raioCurva1 = 5F;

                float distBordaXTercFech = distBordaXCurva1; // Distância de cima para baixo terciária do furo da fechadura
                float distBordaYTercFech = distBordaYCurva1 + 110F;

                float distBordaXCurva2 = distBordaXTercFech - 5F;
                float distBordaYCurva2 = distBordaYTercFech + 5F;
                float posXCurva2 = posXCurva1;
                float posYCurva2 = distBordaYCurva2 - 5F;
                float raioCurva2 = 5F;

                float distBordaXFimFech = distBordaXIniFech; // Distância da esquerda para direita da fechadura
                float distBordaYFimFech = distBordaYCurva2;

                // Faz o recorte da fechadura 1520 no lugar do trinco
                float xIniFech = distBorda1520 - (33.5F + descontoLap);
                float yIniFech = 15F + descontoLap;

                float xIni2Fech = xIniFech;
                float yIni2Fech = yIniFech + 22F;

                float xCurva1Fech = xIni2Fech + 8F;
                float yCurva1Fech = yIni2Fech + 8F;
                float iCurva1Fech = xCurva1Fech;
                float jCurva1Fech = yIni2Fech;
                float raioCurva1Fech = raioCurvaFech;

                float xMeioFech = xCurva1Fech + 57F;
                float yMeioFech = yCurva1Fech;

                float xCurva2Fech = xMeioFech + 8F;
                float yCurva2Fech = yMeioFech - 8F;
                float iCurva2Fech = xCurva2Fech - 8F;
                float jCurva2Fech = yCurva2Fech;
                float raioCurva2Fech = raioCurvaFech;

                float xFimFech = xCurva2Fech;
                float yFimFech = yIniFech;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))

                    // Comando para fazer o furo da roldana 1
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDouble(raioFuroRoldana1, 6))

                    // Posiciona o CNC no furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + raioFuroRoldana2, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))

                    // Comando para fazer o furo da roldana 2
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDouble(raioFuroRoldana2, 6))

                    // Posiciona o CNC no canto direito superior da fechadura
                    .Replace("?CoordXIniFech?", Formatacoes.TrataValorDouble(distBordaXIniFech, 6))
                    .Replace("?CoordYIniFech?", Formatacoes.TrataValorDouble(distBordaYIniFech, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura
                    .Replace("?CoordXSegFech?", Formatacoes.TrataValorDouble(distBordaXSegFech, 6))
                    .Replace("?CoordYSegFech?", Formatacoes.TrataValorDouble(distBordaYSegFech, 6))

                    // Comando para fazer a primeira curva da fechadura
                    .Replace("?CoordXCurva1?", Formatacoes.TrataValorDouble(distBordaXCurva1, 6))
                    .Replace("?CoordYCurva1?", Formatacoes.TrataValorDouble(distBordaYCurva1, 6))
                    .Replace("?PosXCurva1?", Formatacoes.TrataValorDouble(posXCurva1, 6))
                    .Replace("?PosYCurva1?", Formatacoes.TrataValorDouble(posYCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDouble(raioCurva1, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura até o canto inferior esquerdo
                    .Replace("?CoordXTercFech?", Formatacoes.TrataValorDouble(distBordaXTercFech, 6))
                    .Replace("?CoordYTercFech?", Formatacoes.TrataValorDouble(distBordaYTercFech, 6))

                    // Comando para fazer a segunda curva da fechadura
                    .Replace("?CoordXCurva2?", Formatacoes.TrataValorDouble(distBordaXCurva2, 6))
                    .Replace("?CoordYCurva2?", Formatacoes.TrataValorDouble(distBordaYCurva2, 6))
                    .Replace("?PosXCurva2?", Formatacoes.TrataValorDouble(posXCurva2, 6))
                    .Replace("?PosYCurva2?", Formatacoes.TrataValorDouble(posYCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDouble(raioCurva2, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura até o canto inferior esquerdo
                    .Replace("?CoordXFimFech?", Formatacoes.TrataValorDouble(distBordaXFimFech, 6))
                    .Replace("?CoordYFimFech?", Formatacoes.TrataValorDouble(distBordaYFimFech, 6))

                    // Faz o recorte da fechadura 1520 no lugar do trinco.
                    .Replace("?XIniFech?", Formatacoes.TrataValorDouble(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDouble(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDouble(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDouble(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDouble(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDouble(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDouble(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDouble(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDouble(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDouble(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDouble(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDouble(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDouble(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDouble(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDouble(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDouble(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDouble(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDouble(yFimFech, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3530")
            {
                #region CR3530

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXFuroRoldana1 = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1
                int raioFuroRoldana1 = config.Roldana.RaioRoldana;

                int distBordaXFuroRoldana2 = config.Roldana.PosXRoldana; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2
                int raioFuroRoldana2 = config.Roldana.RaioRoldana;

                float distBordaXFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float distBordaYFuroMaior = alturaPuxador + descontoLap;
                float posXFuroMaior = distBordaXFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float posYFuroMaior = distBordaYFuroMaior;

                float distBordaXFuroMenor1 = distBordaXFuroMaior - descontoLap;
                float distBordaYFuroMenor1 = distBordaYFuroMaior + 42.5f;
                int raioFuroMenor1 = 10;
                float posXFuroMenor1 = distBordaXFuroMenor1 - raioFuroMenor1;
                float posYFuroMenor1 = distBordaYFuroMenor1;

                float distBordaXFuroMenor2 = distBordaXFuroMaior - descontoLap;
                float distBordaYFuroMenor2 = distBordaYFuroMaior - 42.5f;
                int raioFuroMenor2 = 10;
                float posXFuroMenor2 = distBordaXFuroMenor2 - raioFuroMenor2;
                float posYFuroMenor2 = distBordaYFuroMenor2;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo superior da fechadura
                    .Replace("?CoordXFuroMenor1?", Formatacoes.TrataValorDouble(distBordaXFuroMenor1, 6))
                    .Replace("?CoordYFuroMenor1?", Formatacoes.TrataValorDouble(distBordaYFuroMenor1, 6))

                    // Comando para fazer o furo superior da fechadura
                    .Replace("?PosXFuroMenor1?", Formatacoes.TrataValorDouble(posXFuroMenor1, 6))
                    .Replace("?PosYFuroMenor1?", Formatacoes.TrataValorDouble(posYFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(raioFuroMenor1, 6))

                    // Posiciona o CNC no furo central da fechadura
                    .Replace("?CoordXFuroMaior?", Formatacoes.TrataValorDouble(distBordaXFuroMaior, 6))
                    .Replace("?CoordYFuroMaior?", Formatacoes.TrataValorDouble(distBordaYFuroMaior, 6))

                    // Comando para fazer o furo central da fechadura
                    .Replace("?PosXFuroMaior?", Formatacoes.TrataValorDouble(posXFuroMaior, 6))
                    .Replace("?PosYFuroMaior?", Formatacoes.TrataValorDouble(posYFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo inferior da fechadura
                    .Replace("?CoordXFuroMenor2?", Formatacoes.TrataValorDouble(distBordaXFuroMenor2, 6))
                    .Replace("?CoordYFuroMenor2?", Formatacoes.TrataValorDouble(distBordaYFuroMenor2, 6))

                    // Comando para fazer o furo inferior da fechadura
                    .Replace("?PosXFuroMenor2?", Formatacoes.TrataValorDouble(posXFuroMenor2, 6))
                    .Replace("?PosYFuroMenor2?", Formatacoes.TrataValorDouble(posYFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(raioFuroMenor2, 6))

                    // Posiciona o CNC no furo da roldana 1
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap + raioFuroRoldana1, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))

                    // Comando para fazer o furo da roldana 1
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDecimal(raioFuroRoldana1, 6))

                    // Posiciona o CNC no furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + raioFuroRoldana2, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))

                    // Comando para fazer o furo da roldana 2
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDecimal(raioFuroRoldana2, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3530PUXDUPL")
            {
                #region CR3530PUXDUPL

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo de distância do eixo do puxador não foi informado." + mensagemErro);

                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador

                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                float xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux1 = (alturaPuxador + descontoLap) - (distEixoFuroPux / 2);
                float iFuroPux1 = xFuroPux1 - raioFuroPux;
                float jFuroPux1 = yFuroPux1;

                float xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux2 = (alturaPuxador + descontoLap) + (distEixoFuroPux / 2);
                float iFuroPux2 = xFuroPux2 - raioFuroPux;
                float jFuroPux2 = yFuroPux2;

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = largura - config.Roldana.PosXRoldana - config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 + config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDouble(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDouble(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDouble(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDouble(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDouble(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDouble(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDouble(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDouble(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3530PUXDUPLTRI")
            {
                #region CR3530PUXDUPLTRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo de distância do eixo do puxador não foi informado." + mensagemErro);

                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador

                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                float xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux1 = (alturaPuxador + descontoLap) - (distEixoFuroPux / 2);
                float iFuroPux1 = xFuroPux1 - raioFuroPux;
                float jFuroPux1 = yFuroPux1;

                float xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux2 = (alturaPuxador + descontoLap) + (distEixoFuroPux / 2);
                float iFuroPux2 = xFuroPux2 - raioFuroPux;
                float jFuroPux2 = yFuroPux2;

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = largura - config.Roldana.PosXRoldana - config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 + config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDouble(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDouble(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDouble(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDouble(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDouble(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDouble(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDouble(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDouble(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3530TRI" || codigoArquivo == "CR3530TRIJAN")
            {
                #region CR3530TRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz as duas roldanas
                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = largura - config.Roldana.PosXRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3532" || codigoArquivo == "CR3532M")
            {
                #region CR3532M

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distBordaFuro == 0)
                    throw new Exception("O campo distância da borda do furo do puxador não foi informado." + mensagemErro);

                if (distEixoPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                int raioFuroFech = espFuroPux > 0 ? espFuroPux / 2 : 8;

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = (largura - config.Roldana.PosXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                float xFuroFech1 = largura - ((distBordaFuro - raioFuroFech) - descontoLap);
                float yFuroFech1 = (alturaPuxador + ((float)distEixoPux / 2f)) + descontoLap;
                float iFuroFech1 = xFuroFech1 - raioFuroFech;
                float jFuroFech1 = yFuroFech1;

                float xFuroFech2 = largura - ((distBordaFuro - raioFuroFech) - descontoLap);
                float yFuroFech2 = (alturaPuxador - ((float)distEixoPux / 2f)) + descontoLap;
                float iFuroFech2 = xFuroFech2 - raioFuroFech;
                float jFuroFech2 = yFuroFech2;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da fechadura 1 e faz o furo
                    .Replace("?XFuroFech1?", Formatacoes.TrataValorDouble(xFuroFech1, 6))
                    .Replace("?YFuroFech1?", Formatacoes.TrataValorDouble(yFuroFech1, 6))
                    .Replace("?IFuroFech1?", Formatacoes.TrataValorDouble(iFuroFech1, 6))
                    .Replace("?JFuroFech1?", Formatacoes.TrataValorDouble(jFuroFech1, 6))
                    .Replace("?RaioFuroFech1?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 2 e faz o furo
                    .Replace("?XFuroFech2?", Formatacoes.TrataValorDouble(xFuroFech2, 6))
                    .Replace("?YFuroFech2?", Formatacoes.TrataValorDouble(yFuroFech2, 6))
                    .Replace("?IFuroFech2?", Formatacoes.TrataValorDouble(iFuroFech2, 6))
                    .Replace("?JFuroFech2?", Formatacoes.TrataValorDouble(jFuroFech2, 6))
                    .Replace("?RaioFuroFech2?", Formatacoes.TrataValorDecimal(raioFuroFech, 6));

                #endregion
            }
            else if (codigoArquivo == "CR3532MTRI")
            {
                #region CR3532MTRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distBordaFuro == 0)
                    throw new Exception("O campo distância da borda do furo do puxador não foi informado." + mensagemErro);

                if (distEixoPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                int raioFuroFech = espFuroPux > 0 ? espFuroPux / 2 : 8;

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = (largura - config.Roldana.PosXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                float xFuroFech1 = largura - ((distBordaFuro - raioFuroFech) - descontoLap);
                float yFuroFech1 = (alturaPuxador + ((float)distEixoPux / 2f)) + descontoLap;
                float iFuroFech1 = xFuroFech1 - raioFuroFech;
                float jFuroFech1 = yFuroFech1;

                float xFuroFech2 = largura - ((distBordaFuro - raioFuroFech) - descontoLap);
                float yFuroFech2 = (alturaPuxador - ((float)distEixoPux / 2f)) + descontoLap;
                float iFuroFech2 = xFuroFech2 - raioFuroFech;
                float jFuroFech2 = yFuroFech2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da fechadura 1 e faz o furo
                    .Replace("?XFuroFech1?", Formatacoes.TrataValorDouble(xFuroFech1, 6))
                    .Replace("?YFuroFech1?", Formatacoes.TrataValorDouble(yFuroFech1, 6))
                    .Replace("?IFuroFech1?", Formatacoes.TrataValorDouble(iFuroFech1, 6))
                    .Replace("?JFuroFech1?", Formatacoes.TrataValorDouble(jFuroFech1, 6))
                    .Replace("?RaioFuroFech1?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Posiciona o CNC no furo da fechadura 2 e faz o furo
                    .Replace("?XFuroFech2?", Formatacoes.TrataValorDouble(xFuroFech2, 6))
                    .Replace("?YFuroFech2?", Formatacoes.TrataValorDouble(yFuroFech2, 6))
                    .Replace("?IFuroFech2?", Formatacoes.TrataValorDouble(iFuroFech2, 6))
                    .Replace("?JFuroFech2?", Formatacoes.TrataValorDouble(jFuroFech2, 6))
                    .Replace("?RaioFuroFech2?", Formatacoes.TrataValorDecimal(raioFuroFech, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUX")
            {
                #region CRPUX

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                float raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = largura - config.Roldana.PosXRoldana - config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 + config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                float xFuroPux = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux = alturaPuxador + descontoLap;
                float iFuroPux = xFuroPux - raioFuroPux;
                float jFuroPux = yFuroPux;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDouble(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDouble(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDouble(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDouble(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDouble(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUX3ROLDANAS")
            {
                #region CRPUX3ROLDANAS

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                float raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador

                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = (largura / 2) + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                float xRoldana3 = largura - config.Roldana.PosXRoldana;
                float yRoldana3 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana3 = xRoldana3 - config.Roldana.RaioRoldana;
                float jRoldana3 = yRoldana3;

                float xFuroPux = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux = alturaPuxador + descontoLap;
                float iFuroPux = xFuroPux - raioFuroPux;
                float jFuroPux = yFuroPux;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 3 e faz o furo
                    .Replace("?XRoldana3?", Formatacoes.TrataValorDouble(xRoldana3, 6))
                    .Replace("?YRoldana3?", Formatacoes.TrataValorDouble(yRoldana3, 6))
                    .Replace("?IRoldana3?", Formatacoes.TrataValorDouble(iRoldana3, 6))
                    .Replace("?JRoldana3?", Formatacoes.TrataValorDouble(jRoldana3, 6))
                    .Replace("?RaioRoldana3?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDouble(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDouble(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDouble(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDouble(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDouble(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUXDUPL1510")
            {
                #region CRPUXDUPL1510

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                int distBordaXRoldana = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYRoldana = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                // Faz o recorte da fechadura 1510
                decimal xIniFech = (decimal)(largura - (13 + descontoLap));
                decimal yIniFech = alturaPuxador + 65;
                decimal xSegFech = xIniFech - (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal ySegFech = alturaPuxador + 65;

                decimal xCurva1 = xSegFech - 12;
                decimal yCurva1 = ySegFech - 12;
                decimal iCurva1 = xCurva1 + 12;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 12;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 - 101;

                decimal xCurva2 = xCurva1 + 12;
                decimal yCurva2 = yTercFech - 12;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 + 12;
                decimal raioCurva2 = 12;

                decimal xFimFech = xCurva2 + (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal yFimFech = yCurva2;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz os dois furos das roldanas
                float xRoldana1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = (largura - distBordaXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo

                    // Faz o furo da fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o furo da roldana 1
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUXTRI")
            {
                #region CRPUXTRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXFuroRoldana1 = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                int distBordaXFuroRoldana2 = config.Roldana.PosXRoldana; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2

                int distBordaXFuroPux = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador
                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap + config.Roldana.RaioRoldana, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))

                    // Comando para fazer o furo da roldana 1
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDouble(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + config.Roldana.RaioRoldana, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))

                    // Comando para fazer o furo da roldana 2
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDouble(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo do puxador
                    .Replace("?CoordXFuroPux?", Formatacoes.TrataValorDouble((largura - distBordaXFuroPux) + descontoLap + (raioFuroPux * 2), 6))
                    .Replace("?CoordYFuroPux?", Formatacoes.TrataValorDouble((alturaPuxador + descontoLap), 6))

                    // Comando para fazer o furo do puxador
                    .Replace("?PosXFuroPux?", Formatacoes.TrataValorDouble((largura + descontoLap) - distBordaXFuroPux, 6))
                    .Replace("?PosYFuroPux?", Formatacoes.TrataValorDouble((alturaPuxador + descontoLap), 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do trinco
                    .Replace("?CoordXTrinco?", Formatacoes.TrataValorDouble((largura - config.Trinco.DistBordaXTrinco) + descontoLap + config.Trinco.RaioTrinco, 6))
                    .Replace("?CoordYTrinco?", Formatacoes.TrataValorDouble(descontoLap + config.Trinco.DistBordaYTrincoCorrer, 6))

                    // Comando para fazer o furo do trinco
                    .Replace("?PosXTrinco?", Formatacoes.TrataValorDouble((largura - config.Trinco.DistBordaXTrinco) + descontoLap, 6))
                    .Replace("?PosYTrinco?", Formatacoes.TrataValorDouble(descontoLap + config.Trinco.DistBordaYTrincoCorrer, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUXTRI3ROLDANAS")
            {
                #region CRPUXTRI3ROLDANAS

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                float raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador

                // Faz as 3 roldanas
                float xRoldana1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                float xRoldana2 = (largura / 2) + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                float xRoldana3 = largura - config.Roldana.PosXRoldana;
                float yRoldana3 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana3 = xRoldana3 - config.Roldana.RaioRoldana;
                float jRoldana3 = yRoldana3;

                // Faz o furo do puxador
                float xFuroPux = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux = alturaPuxador + descontoLap;
                float iFuroPux = xFuroPux - raioFuroPux;
                float jFuroPux = yFuroPux;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo da roldana 1 e faz o furo
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 2 e faz o furo
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo da roldana 3 e faz o furo
                    .Replace("?XRoldana3?", Formatacoes.TrataValorDouble(xRoldana3, 6))
                    .Replace("?YRoldana3?", Formatacoes.TrataValorDouble(yRoldana3, 6))
                    .Replace("?IRoldana3?", Formatacoes.TrataValorDouble(iRoldana3, 6))
                    .Replace("?JRoldana3?", Formatacoes.TrataValorDouble(jRoldana3, 6))
                    .Replace("?RaioRoldana3?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDouble(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDouble(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDouble(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDouble(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDouble(raioFuroPux, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRPUXTRITRILHOEMBUT")
            {
                #region CRPUXTRITRILHOEMBUT

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXFuroRoldana1 = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                int distBordaXFuroRoldana2 = config.Roldana.PosXRoldana; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2

                int distBordaXFuroPux = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador
                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da roldana 1
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap + config.Roldana.RaioRoldana, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + config.Roldana.RaioRoldana, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo do puxador
                    .Replace("?CoordXFuroPux?", Formatacoes.TrataValorDouble((largura - distBordaXFuroPux) + descontoLap + (raioFuroPux * 2), 6))
                    .Replace("?CoordYFuroPux?", Formatacoes.TrataValorDouble((alturaPuxador + descontoLap), 6))
                    .Replace("?PosXFuroPux?", Formatacoes.TrataValorDouble((largura + descontoLap) - distBordaXFuroPux, 6))
                    .Replace("?PosYFuroPux?", Formatacoes.TrataValorDouble((alturaPuxador + descontoLap), 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o trinco
                    .Replace("?CoordXTrinco?", Formatacoes.TrataValorDouble((largura - config.Trinco.DistBordaXTrinco) + descontoLap + config.Trinco.RaioTrinco, 6))
                    .Replace("?CoordYTrinco?", Formatacoes.TrataValorDouble(descontoLap + config.Trinco.DistBordaYTrincoTrilhoEmbut, 6))
                    .Replace("?PosXTrinco?", Formatacoes.TrataValorDouble((largura - config.Trinco.DistBordaXTrinco) + descontoLap, 6))
                    .Replace("?PosYTrinco?", Formatacoes.TrataValorDouble(descontoLap + config.Trinco.DistBordaYTrincoTrilhoEmbut, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRROLDANADUPLA1510COMTRINCO")
            {
                #region CRROLDANADUPLA1510COMTRINCO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXRoldana = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYRoldana = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                // Faz o recorte da fechadura 1510
                decimal xIniFech = (decimal)(18 + descontoLap);
                decimal yIniFech = alturaPuxador - 65;
                decimal xSegFech = xIniFech + (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal ySegFech = yIniFech;

                decimal xCurva1 = xSegFech + 12;
                decimal yCurva1 = ySegFech + 12;
                decimal iCurva1 = xCurva1 - 12;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 12;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 + 101;

                decimal xCurva2 = xCurva1 - 12;
                decimal yCurva2 = yTercFech + 12;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 - 12;
                decimal raioCurva2 = 12;

                decimal xFimFech = xCurva2 - (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal yFimFech = yCurva2;

                // Faz o trinco
                decimal xTrinco = config.Trinco.DistBordaXTrinco + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Faz o furo da roldana da esquerda 1
                float xRoldanaEsq1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldanaEsq1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq1 = xRoldanaEsq1 - config.Roldana.RaioRoldana;
                float jRoldanaEsq1 = yRoldanaEsq1;

                // Faz o furo da roldana da esquerda 2
                float xRoldanaEsq2 = xRoldanaEsq1 + 100;
                float yRoldanaEsq2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq2 = xRoldanaEsq2 - config.Roldana.RaioRoldana;
                float jRoldanaEsq2 = yRoldanaEsq2;

                // Faz o furo da roldana da direita 1
                float xRoldanaDir1 = largura - distBordaXRoldana;
                float yRoldanaDir1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir1 = xRoldanaDir1 - config.Roldana.RaioRoldana;
                float jRoldanaDir1 = yRoldanaDir1;

                // Faz o furo da roldana da direita 2
                float xRoldanaDir2 = xRoldanaDir1 - 100;
                float yRoldanaDir2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir2 = xRoldanaDir2 - config.Roldana.RaioRoldana;
                float jRoldanaDir2 = yRoldanaDir2;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    // Faz o furo da roldana da esquerda 1
                    .Replace("?XRoldanaEsq1?", Formatacoes.TrataValorDouble(xRoldanaEsq1, 6))
                    .Replace("?YRoldanaEsq1?", Formatacoes.TrataValorDouble(yRoldanaEsq1, 6))
                    .Replace("?IRoldanaEsq1?", Formatacoes.TrataValorDouble(iRoldanaEsq1, 6))
                    .Replace("?JRoldanaEsq1?", Formatacoes.TrataValorDouble(jRoldanaEsq1, 6))
                    .Replace("?RaioRoldanaEsq1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da esquerda 2
                    .Replace("?XRoldanaEsq2?", Formatacoes.TrataValorDouble(xRoldanaEsq2, 6))
                    .Replace("?YRoldanaEsq2?", Formatacoes.TrataValorDouble(yRoldanaEsq2, 6))
                    .Replace("?IRoldanaEsq2?", Formatacoes.TrataValorDouble(iRoldanaEsq2, 6))
                    .Replace("?JRoldanaEsq2?", Formatacoes.TrataValorDouble(jRoldanaEsq2, 6))
                    .Replace("?RaioRoldanaEsq2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 1
                    .Replace("?XRoldanaDir1?", Formatacoes.TrataValorDouble(xRoldanaDir1, 6))
                    .Replace("?YRoldanaDir1?", Formatacoes.TrataValorDouble(yRoldanaDir1, 6))
                    .Replace("?IRoldanaDir1?", Formatacoes.TrataValorDouble(iRoldanaDir1, 6))
                    .Replace("?JRoldanaDir1?", Formatacoes.TrataValorDouble(jRoldanaDir1, 6))
                    .Replace("?RaioRoldanaDir1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 2
                    .Replace("?XRoldanaDir2?", Formatacoes.TrataValorDouble(xRoldanaDir2, 6))
                    .Replace("?YRoldanaDir2?", Formatacoes.TrataValorDouble(yRoldanaDir2, 6))
                    .Replace("?IRoldanaDir2?", Formatacoes.TrataValorDouble(iRoldanaDir2, 6))
                    .Replace("?JRoldanaDir2?", Formatacoes.TrataValorDouble(jRoldanaDir2, 6))
                    .Replace("?RaioRoldanaDir2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6));

                #endregion
            }
            else if (codigoArquivo == "CRROLDANADUPLA1510SEMTRINCO")
            {
                #region CRROLDANADUPLA1510SEMTRINCO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXRoldana = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYRoldana = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                // Faz o recorte da fechadura 1510
                decimal xIniFech = (decimal)(18 + descontoLap);
                decimal yIniFech = alturaPuxador - 65;
                decimal xSegFech = xIniFech + (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal ySegFech = yIniFech;

                decimal xCurva1 = xSegFech + 12;
                decimal yCurva1 = ySegFech + 12;
                decimal iCurva1 = xCurva1 - 12;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 12;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 + 101;

                decimal xCurva2 = xCurva1 - 12;
                decimal yCurva2 = yTercFech + 12;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 - 12;
                decimal raioCurva2 = 12;

                decimal xFimFech = xCurva2 - (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal yFimFech = yCurva2;

                // Faz o trinco
                decimal xTrinco = config.Trinco.DistBordaXTrinco + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Faz o furo da roldana da esquerda 1
                float xRoldanaEsq1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldanaEsq1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq1 = xRoldanaEsq1 - config.Roldana.RaioRoldana;
                float jRoldanaEsq1 = yRoldanaEsq1;

                // Faz o furo da roldana da esquerda 2
                float xRoldanaEsq2 = xRoldanaEsq1 + 100;
                float yRoldanaEsq2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq2 = xRoldanaEsq2 - config.Roldana.RaioRoldana;
                float jRoldanaEsq2 = yRoldanaEsq2;

                // Faz o furo da roldana da direita 1
                float xRoldanaDir1 = largura - distBordaXRoldana;
                float yRoldanaDir1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir1 = xRoldanaDir1 - config.Roldana.RaioRoldana;
                float jRoldanaDir1 = yRoldanaDir1;

                // Faz o furo da roldana da direita 2
                float xRoldanaDir2 = xRoldanaDir1 - 100;
                float yRoldanaDir2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir2 = xRoldanaDir2 - config.Roldana.RaioRoldana;
                float jRoldanaDir2 = yRoldanaDir2;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    // Faz o furo da roldana da esquerda 1
                    .Replace("?XRoldanaEsq1?", Formatacoes.TrataValorDouble(xRoldanaEsq1, 6))
                    .Replace("?YRoldanaEsq1?", Formatacoes.TrataValorDouble(yRoldanaEsq1, 6))
                    .Replace("?IRoldanaEsq1?", Formatacoes.TrataValorDouble(iRoldanaEsq1, 6))
                    .Replace("?JRoldanaEsq1?", Formatacoes.TrataValorDouble(jRoldanaEsq1, 6))
                    .Replace("?RaioRoldanaEsq1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da esquerda 2
                    .Replace("?XRoldanaEsq2?", Formatacoes.TrataValorDouble(xRoldanaEsq2, 6))
                    .Replace("?YRoldanaEsq2?", Formatacoes.TrataValorDouble(yRoldanaEsq2, 6))
                    .Replace("?IRoldanaEsq2?", Formatacoes.TrataValorDouble(iRoldanaEsq2, 6))
                    .Replace("?JRoldanaEsq2?", Formatacoes.TrataValorDouble(jRoldanaEsq2, 6))
                    .Replace("?RaioRoldanaEsq2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 1
                    .Replace("?XRoldanaDir1?", Formatacoes.TrataValorDouble(xRoldanaDir1, 6))
                    .Replace("?YRoldanaDir1?", Formatacoes.TrataValorDouble(yRoldanaDir1, 6))
                    .Replace("?IRoldanaDir1?", Formatacoes.TrataValorDouble(iRoldanaDir1, 6))
                    .Replace("?JRoldanaDir1?", Formatacoes.TrataValorDouble(jRoldanaDir1, 6))
                    .Replace("?RaioRoldanaDir1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 2
                    .Replace("?XRoldanaDir2?", Formatacoes.TrataValorDouble(xRoldanaDir2, 6))
                    .Replace("?YRoldanaDir2?", Formatacoes.TrataValorDouble(yRoldanaDir2, 6))
                    .Replace("?IRoldanaDir2?", Formatacoes.TrataValorDouble(iRoldanaDir2, 6))
                    .Replace("?JRoldanaDir2?", Formatacoes.TrataValorDouble(jRoldanaDir2, 6))
                    .Replace("?RaioRoldanaDir2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6));

                #endregion
            }
            else if (codigoArquivo == "CRROLDANADUPLA3530")
            {
                #region CRROLDANADUPLA3530

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                // Faz o furo da roldana da esquerda 1
                float xRoldanaEsq1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldanaEsq1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq1 = xRoldanaEsq1 - config.Roldana.RaioRoldana;
                float jRoldanaEsq1 = yRoldanaEsq1;

                // Faz o furo da roldana da esquerda 2
                float xRoldanaEsq2 = xRoldanaEsq1 + 100;
                float yRoldanaEsq2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq2 = xRoldanaEsq2 - config.Roldana.RaioRoldana;
                float jRoldanaEsq2 = yRoldanaEsq2;

                // Faz o furo da roldana da direita 1
                float xRoldanaDir1 = largura - config.Roldana.PosXRoldana;
                float yRoldanaDir1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir1 = xRoldanaDir1 - config.Roldana.RaioRoldana;
                float jRoldanaDir1 = yRoldanaDir1;

                // Faz o furo da roldana da direita 2
                float xRoldanaDir2 = xRoldanaDir1 - 100;
                float yRoldanaDir2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir2 = xRoldanaDir2 - config.Roldana.RaioRoldana;
                float jRoldanaDir2 = yRoldanaDir2;

                // Faz os furos da 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da roldana da esquerda 1
                    .Replace("?XRoldanaEsq1?", Formatacoes.TrataValorDouble(xRoldanaEsq1, 6))
                    .Replace("?YRoldanaEsq1?", Formatacoes.TrataValorDouble(yRoldanaEsq1, 6))
                    .Replace("?IRoldanaEsq1?", Formatacoes.TrataValorDouble(iRoldanaEsq1, 6))
                    .Replace("?JRoldanaEsq1?", Formatacoes.TrataValorDouble(jRoldanaEsq1, 6))
                    .Replace("?RaioRoldanaEsq1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da esquerda 2
                    .Replace("?XRoldanaEsq2?", Formatacoes.TrataValorDouble(xRoldanaEsq2, 6))
                    .Replace("?YRoldanaEsq2?", Formatacoes.TrataValorDouble(yRoldanaEsq2, 6))
                    .Replace("?IRoldanaEsq2?", Formatacoes.TrataValorDouble(iRoldanaEsq2, 6))
                    .Replace("?JRoldanaEsq2?", Formatacoes.TrataValorDouble(jRoldanaEsq2, 6))
                    .Replace("?RaioRoldanaEsq2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 1
                    .Replace("?XRoldanaDir1?", Formatacoes.TrataValorDouble(xRoldanaDir1, 6))
                    .Replace("?YRoldanaDir1?", Formatacoes.TrataValorDouble(yRoldanaDir1, 6))
                    .Replace("?IRoldanaDir1?", Formatacoes.TrataValorDouble(iRoldanaDir1, 6))
                    .Replace("?JRoldanaDir1?", Formatacoes.TrataValorDouble(jRoldanaDir1, 6))
                    .Replace("?RaioRoldanaDir1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 2
                    .Replace("?XRoldanaDir2?", Formatacoes.TrataValorDouble(xRoldanaDir2, 6))
                    .Replace("?YRoldanaDir2?", Formatacoes.TrataValorDouble(yRoldanaDir2, 6))
                    .Replace("?IRoldanaDir2?", Formatacoes.TrataValorDouble(iRoldanaDir2, 6))
                    .Replace("?JRoldanaDir2?", Formatacoes.TrataValorDouble(jRoldanaDir2, 6))
                    .Replace("?RaioRoldanaDir2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6));

                #endregion
            }
            else if (codigoArquivo == "CRROLDANADUPLA3530PUXDUPL")
            {
                #region CRROLDANADUPLA3530PUXDUPL

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo de distância do eixo do puxador não foi informado." + mensagemErro);

                int raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 6;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo da roldana da esquerda 1
                float xRoldanaEsq1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldanaEsq1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq1 = xRoldanaEsq1 - config.Roldana.RaioRoldana;
                float jRoldanaEsq1 = yRoldanaEsq1;

                // Faz o furo da roldana da esquerda 2
                float xRoldanaEsq2 = xRoldanaEsq1 + 100;
                float yRoldanaEsq2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq2 = xRoldanaEsq2 - config.Roldana.RaioRoldana;
                float jRoldanaEsq2 = yRoldanaEsq2;

                // Faz o furo da roldana da direita 1
                float xRoldanaDir1 = largura - config.Roldana.PosXRoldana;
                float yRoldanaDir1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir1 = xRoldanaDir1 - config.Roldana.RaioRoldana;
                float jRoldanaDir1 = yRoldanaDir1;

                // Faz o furo da roldana da direita 2
                float xRoldanaDir2 = xRoldanaDir1 - 100;
                float yRoldanaDir2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir2 = xRoldanaDir2 - config.Roldana.RaioRoldana;
                float jRoldanaDir2 = yRoldanaDir2;

                // Faz os furos da 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz os furos do puxador duplo
                float xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux1 = (alturaPuxador + descontoLap) - (distEixoFuroPux / 2);
                float iFuroPux1 = xFuroPux1 - raioFuroPux;
                float jFuroPux1 = yFuroPux1;

                float xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux2 = (alturaPuxador + descontoLap) + (distEixoFuroPux / 2);
                float iFuroPux2 = xFuroPux2 - raioFuroPux;
                float jFuroPux2 = yFuroPux2;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da roldana da esquerda 1
                    .Replace("?XRoldanaEsq1?", Formatacoes.TrataValorDouble(xRoldanaEsq1, 6))
                    .Replace("?YRoldanaEsq1?", Formatacoes.TrataValorDouble(yRoldanaEsq1, 6))
                    .Replace("?IRoldanaEsq1?", Formatacoes.TrataValorDouble(iRoldanaEsq1, 6))
                    .Replace("?JRoldanaEsq1?", Formatacoes.TrataValorDouble(jRoldanaEsq1, 6))
                    .Replace("?RaioRoldanaEsq1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da esquerda 2
                    .Replace("?XRoldanaEsq2?", Formatacoes.TrataValorDouble(xRoldanaEsq2, 6))
                    .Replace("?YRoldanaEsq2?", Formatacoes.TrataValorDouble(yRoldanaEsq2, 6))
                    .Replace("?IRoldanaEsq2?", Formatacoes.TrataValorDouble(iRoldanaEsq2, 6))
                    .Replace("?JRoldanaEsq2?", Formatacoes.TrataValorDouble(jRoldanaEsq2, 6))
                    .Replace("?RaioRoldanaEsq2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 1
                    .Replace("?XRoldanaDir1?", Formatacoes.TrataValorDouble(xRoldanaDir1, 6))
                    .Replace("?YRoldanaDir1?", Formatacoes.TrataValorDouble(yRoldanaDir1, 6))
                    .Replace("?IRoldanaDir1?", Formatacoes.TrataValorDouble(iRoldanaDir1, 6))
                    .Replace("?JRoldanaDir1?", Formatacoes.TrataValorDouble(jRoldanaDir1, 6))
                    .Replace("?RaioRoldanaDir1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 2
                    .Replace("?XRoldanaDir2?", Formatacoes.TrataValorDouble(xRoldanaDir2, 6))
                    .Replace("?YRoldanaDir2?", Formatacoes.TrataValorDouble(yRoldanaDir2, 6))
                    .Replace("?IRoldanaDir2?", Formatacoes.TrataValorDouble(iRoldanaDir2, 6))
                    .Replace("?JRoldanaDir2?", Formatacoes.TrataValorDouble(jRoldanaDir2, 6))
                    .Replace("?RaioRoldanaDir2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDouble(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDouble(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDouble(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDouble(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDouble(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDouble(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDouble(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDouble(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "CRROLDANADUPLA3530TRI")
            {
                #region CRROLDANADUPLA3530TRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                // Faz o furo da roldana da esquerda 1
                float xRoldanaEsq1 = config.Roldana.PosXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldanaEsq1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq1 = xRoldanaEsq1 - config.Roldana.RaioRoldana;
                float jRoldanaEsq1 = yRoldanaEsq1;

                // Faz o furo da roldana da esquerda 2
                float xRoldanaEsq2 = xRoldanaEsq1 + 100;
                float yRoldanaEsq2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaEsq2 = xRoldanaEsq2 - config.Roldana.RaioRoldana;
                float jRoldanaEsq2 = yRoldanaEsq2;

                // Faz o furo da roldana da direita 1
                float xRoldanaDir1 = largura - config.Roldana.PosXRoldana;
                float yRoldanaDir1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir1 = xRoldanaDir1 - config.Roldana.RaioRoldana;
                float jRoldanaDir1 = yRoldanaDir1;

                // Faz o furo da roldana da direita 2
                float xRoldanaDir2 = xRoldanaDir1 - 100;
                float yRoldanaDir2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldanaDir2 = xRoldanaDir2 - config.Roldana.RaioRoldana;
                float jRoldanaDir2 = yRoldanaDir2;

                // Faz os furos da 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo
                    // Faz o furo da roldana da esquerda 1
                    .Replace("?XRoldanaEsq1?", Formatacoes.TrataValorDouble(xRoldanaEsq1, 6))
                    .Replace("?YRoldanaEsq1?", Formatacoes.TrataValorDouble(yRoldanaEsq1, 6))
                    .Replace("?IRoldanaEsq1?", Formatacoes.TrataValorDouble(iRoldanaEsq1, 6))
                    .Replace("?JRoldanaEsq1?", Formatacoes.TrataValorDouble(jRoldanaEsq1, 6))
                    .Replace("?RaioRoldanaEsq1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da esquerda 2
                    .Replace("?XRoldanaEsq2?", Formatacoes.TrataValorDouble(xRoldanaEsq2, 6))
                    .Replace("?YRoldanaEsq2?", Formatacoes.TrataValorDouble(yRoldanaEsq2, 6))
                    .Replace("?IRoldanaEsq2?", Formatacoes.TrataValorDouble(iRoldanaEsq2, 6))
                    .Replace("?JRoldanaEsq2?", Formatacoes.TrataValorDouble(jRoldanaEsq2, 6))
                    .Replace("?RaioRoldanaEsq2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 1
                    .Replace("?XRoldanaDir1?", Formatacoes.TrataValorDouble(xRoldanaDir1, 6))
                    .Replace("?YRoldanaDir1?", Formatacoes.TrataValorDouble(yRoldanaDir1, 6))
                    .Replace("?IRoldanaDir1?", Formatacoes.TrataValorDouble(iRoldanaDir1, 6))
                    .Replace("?JRoldanaDir1?", Formatacoes.TrataValorDouble(jRoldanaDir1, 6))
                    .Replace("?RaioRoldanaDir1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana da direita 2
                    .Replace("?XRoldanaDir2?", Formatacoes.TrataValorDouble(xRoldanaDir2, 6))
                    .Replace("?YRoldanaDir2?", Formatacoes.TrataValorDouble(yRoldanaDir2, 6))
                    .Replace("?IRoldanaDir2?", Formatacoes.TrataValorDouble(iRoldanaDir2, 6))
                    .Replace("?JRoldanaDir2?", Formatacoes.TrataValorDouble(jRoldanaDir2, 6))
                    .Replace("?RaioRoldanaDir2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRTRINCO")
            {
                #region CRTRINCO

                int distBordaXRoldana = config.Roldana.PosXRoldana;
                float distBordaYRoldana = config.Roldana.PosYRoldana;

                // Faz o furo da roldana 1
                float xRoldana1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                // Faz o furo da roldana 2
                float xRoldana2 = (largura - distBordaXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Monta o arquivo
                conteudoArquivo = conteudoArquivo

                    // Faz o furo da roldana 1
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "CRTRINCOTRILHOEMBUT")
            {
                #region CRTRINCOTRILHOEMBUT

                int distBordaXRoldana = config.Roldana.PosXRoldana;
                float distBordaYRoldana = config.Roldana.PosYRoldana;

                // Faz o furo da roldana 1
                float xRoldana1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                // Faz o furo da roldana 2
                float xRoldana2 = (largura - distBordaXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoTrilhoEmbut + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Monta o arquivo
                conteudoArquivo = conteudoArquivo

                    // Faz o furo da roldana 1
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "FECH1510")
            {
                #region FECH1510

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXFuroRoldana1 = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYFuroRoldana1 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1
                int raioFuroRoldana1 = config.Roldana.RaioRoldana;

                int distBordaXFuroRoldana2 = config.Roldana.PosXRoldana; // Distância da direita para a esquerda do furo da roldana 2
                float distBordaYFuroRoldana2 = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 2
                int raioFuroRoldana2 = config.Roldana.RaioRoldana;

                // Faz o recorte da fechadura 1510
                float distBordaXIniFech = largura - (13 + descontoLap);
                float distBordaYIniFech = alturaPuxador + 65;
                float distBordaXSegFech = distBordaXIniFech - config.Fechadura1510.DistBordaFechadura1510;
                float distBordaYSegFech = alturaPuxador + 65;

                float distBordaXCurva1 = distBordaXSegFech - 12;
                float distBordaYCurva1 = distBordaYSegFech - 12;
                float posXCurva1 = distBordaXCurva1 + 12;
                float posYCurva1 = distBordaYCurva1;
                float raioCurva1 = 12;

                float distBordaXTercFech = distBordaXCurva1;
                float distBordaYTercFech = distBordaYCurva1 - 101;

                float distBordaXCurva2 = distBordaXCurva1 + 12;
                float distBordaYCurva2 = distBordaYTercFech - 12;
                float posXCurva2 = distBordaXCurva2;
                float posYCurva2 = distBordaYCurva2 + 12;
                float raioCurva2 = 12;

                float distBordaXFimFech = distBordaXCurva2 + config.Fechadura1510.DistBordaFechadura1510;
                float distBordaYFimFech = distBordaYCurva2;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo
                    .Replace("?CoordXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?CoordYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))

                    // Comando para fazer o furo da roldana 1
                    .Replace("?PosXFuroRoldana1?", Formatacoes.TrataValorDouble(distBordaXFuroRoldana1 + descontoLap, 6))
                    .Replace("?PosYFuroRoldana1?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana1, 6))
                    .Replace("?RaioFuroRoldana1?", Formatacoes.TrataValorDecimal(raioFuroRoldana1, 6))

                    // Posiciona o CNC no furo da roldana 2
                    .Replace("?CoordXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap + raioFuroRoldana2, 6))
                    .Replace("?CoordYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))

                    // Comando para fazer o furo da roldana 2
                    .Replace("?PosXFuroRoldana2?", Formatacoes.TrataValorDouble((largura - distBordaXFuroRoldana2) + descontoLap, 6))
                    .Replace("?PosYFuroRoldana2?", Formatacoes.TrataValorDouble((altura + descontoLap) - distBordaYFuroRoldana2, 6))
                    .Replace("?RaioFuroRoldana2?", Formatacoes.TrataValorDecimal(raioFuroRoldana2, 6))

                    // Posiciona o CNC no canto direito superior da fechadura
                    .Replace("?CoordXIniFech?", Formatacoes.TrataValorDouble(distBordaXIniFech, 6))
                    .Replace("?CoordYIniFech?", Formatacoes.TrataValorDouble(distBordaYIniFech, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura
                    .Replace("?CoordXSegFech?", Formatacoes.TrataValorDouble(distBordaXSegFech, 6))
                    .Replace("?CoordYSegFech?", Formatacoes.TrataValorDouble(distBordaYSegFech, 6))

                    // Comando para fazer a primeira curva da fechadura
                    .Replace("?CoordXCurva1?", Formatacoes.TrataValorDouble(distBordaXCurva1, 6))
                    .Replace("?CoordYCurva1?", Formatacoes.TrataValorDouble(distBordaYCurva1, 6))
                    .Replace("?PosXCurva1?", Formatacoes.TrataValorDouble(posXCurva1, 6))
                    .Replace("?PosYCurva1?", Formatacoes.TrataValorDouble(posYCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal((decimal)raioCurva1, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura até o canto inferior esquerdo
                    .Replace("?CoordXTercFech?", Formatacoes.TrataValorDouble(distBordaXTercFech, 6))
                    .Replace("?CoordYTercFech?", Formatacoes.TrataValorDouble(distBordaYTercFech, 6))

                    // Comando para fazer a segunda curva da fechadura
                    .Replace("?CoordXCurva2?", Formatacoes.TrataValorDouble(distBordaXCurva2, 6))
                    .Replace("?CoordYCurva2?", Formatacoes.TrataValorDouble(distBordaYCurva2, 6))
                    .Replace("?PosXCurva2?", Formatacoes.TrataValorDouble(posXCurva2, 6))
                    .Replace("?PosYCurva2?", Formatacoes.TrataValorDouble(posYCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal((decimal)raioCurva2, 6))

                    // Posiciona o CNC no canto esquerdo superior da fechadura até o canto inferior esquerdo
                    .Replace("?CoordXFimFech?", Formatacoes.TrataValorDouble(distBordaXFimFech, 6))
                    .Replace("?CoordYFimFech?", Formatacoes.TrataValorDouble(distBordaYFimFech, 6));

                #endregion
            }
            else if (codigoArquivo == "FECH1510TRI")
            {
                #region FECH1510TRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXRoldana = config.Roldana.PosXRoldana; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYRoldana = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                // Faz o recorte da fechadura 1510
                decimal xIniFech = (decimal)(largura - (13 + descontoLap));
                decimal yIniFech = alturaPuxador + 65;
                decimal xSegFech = xIniFech - (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal ySegFech = alturaPuxador + 65;

                decimal xCurva1 = xSegFech - 12;
                decimal yCurva1 = ySegFech - 12;
                decimal iCurva1 = xCurva1 + 12;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 12;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 - 101;

                decimal xCurva2 = xCurva1 + 12;
                decimal yCurva2 = yTercFech - 12;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 + 12;
                decimal raioCurva2 = 12;

                decimal xFimFech = xCurva2 + (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal yFimFech = yCurva2;

                // Faz o furo da roldana 1
                float xRoldana1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                // Faz o furo da roldana 2
                float xRoldana2 = (largura - distBordaXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoCorrer + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo

                    // Faz o furo da fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o furo da roldana 1
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "FECH1510TRITRILHOEMBUT")
            {
                #region FECH1510TRITRILHOEMBUT

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int distBordaXRoldana = 50; // Distância da esquerda para a direita do furo da roldana 1
                float distBordaYRoldana = config.Roldana.PosYRoldana; // Distância de cima para baixo do furo da roldana 1

                // Faz o recorte da fechadura 1510
                decimal xIniFech = (decimal)(largura - (10 + descontoLap));
                decimal yIniFech = alturaPuxador + 65;
                decimal xSegFech = xIniFech - (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal ySegFech = alturaPuxador + 65;

                decimal xCurva1 = xSegFech - 12;
                decimal yCurva1 = ySegFech - 12;
                decimal iCurva1 = xCurva1 + 12;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 12;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 - 101;

                decimal xCurva2 = xCurva1 + 12;
                decimal yCurva2 = yTercFech - 12;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 + 12;
                decimal raioCurva2 = 12;

                decimal xFimFech = xCurva2 + (decimal)config.Fechadura1510.DistBordaFechadura1510;
                decimal yFimFech = yCurva2;

                // Faz o furo da roldana 1
                float xRoldana1 = distBordaXRoldana + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana1 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana1 = xRoldana1 - config.Roldana.RaioRoldana;
                float jRoldana1 = yRoldana1;

                // Faz o furo da roldana 2
                float xRoldana2 = (largura - distBordaXRoldana) + descontoLap + config.Roldana.RaioRoldana;
                float yRoldana2 = (altura + descontoLap) - config.Roldana.PosYRoldana;
                float iRoldana2 = xRoldana2 - config.Roldana.RaioRoldana;
                float jRoldana2 = yRoldana2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoTrilhoEmbut + (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                // Posiciona o CNC no furo da roldana 1
                conteudoArquivo = conteudoArquivo

                    // Faz o furo da fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o furo da roldana 1
                    .Replace("?XRoldana1?", Formatacoes.TrataValorDouble(xRoldana1, 6))
                    .Replace("?YRoldana1?", Formatacoes.TrataValorDouble(yRoldana1, 6))
                    .Replace("?IRoldana1?", Formatacoes.TrataValorDouble(iRoldana1, 6))
                    .Replace("?JRoldana1?", Formatacoes.TrataValorDouble(jRoldana1, 6))
                    .Replace("?RaioRoldana1?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o furo da roldana 2
                    .Replace("?XRoldana2?", Formatacoes.TrataValorDouble(xRoldana2, 6))
                    .Replace("?YRoldana2?", Formatacoes.TrataValorDouble(yRoldana2, 6))
                    .Replace("?IRoldana2?", Formatacoes.TrataValorDouble(iRoldana2, 6))
                    .Replace("?JRoldana2?", Formatacoes.TrataValorDouble(jRoldana2, 6))
                    .Replace("?RaioRoldana2?", Formatacoes.TrataValorDecimal(config.Roldana.RaioRoldana, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXO1FURO")
            {
                #region FIXO1FURO

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int alturaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuro);

                int larguraFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuro);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do furo não foi informada." + mensagemErro);

                if (alturaFuro == 0)
                    throw new Exception("A altura do furo não foi informada." + mensagemErro);

                if (larguraFuro == 0)
                    throw new Exception("A largura do furo não foi informada." + mensagemErro);

                decimal xFuro = largura + (decimal)descontoLap - larguraFuro - espFuroPux;
                decimal yFuro = alturaFuro + (decimal)descontoLap;
                decimal iFuro = xFuro + espFuroPux;
                decimal jFuro = yFuro;
                decimal raioFuro = espFuroPux / 2;

                conteudoArquivo = conteudoArquivo

                    // Faz o furo superior esquerdo
                    .Replace("?XFuro?", Formatacoes.TrataValorDecimal(xFuro, 6))
                    .Replace("?YFuro?", Formatacoes.TrataValorDecimal(yFuro, 6))
                    .Replace("?IFuro?", Formatacoes.TrataValorDecimal(iFuro, 6))
                    .Replace("?JFuro?", Formatacoes.TrataValorDecimal(jFuro, 6))
                    .Replace("?RaioFuro?", Formatacoes.TrataValorDecimal(raioFuro, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXO2FUROS")
            {
                #region FIXO2FUROS

                int espFuroInf = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroInferior);

                int espFuroSup = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroSuperior);

                int espFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int alturaFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroInferior);

                int larguraFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroInferior);

                int alturaFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroSuperior);

                int larguraFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroSuperior);

                if (espFuroInf == 0 && espFuroSup == 0 && espFuro > 0)
                {
                    espFuroInf = espFuro;
                    espFuroSup = espFuro;
                }

                if (espFuroInf == 0)
                    throw new Exception("A espessura do furo inferior não foi informada." + mensagemErro);

                if (espFuroSup == 0)
                    throw new Exception("A espessura do furo superior não foi informada." + mensagemErro);

                if (alturaFuroInferior == 0)
                    throw new Exception("A altura do furo inferior não foi informada." + mensagemErro);

                if (larguraFuroInferior == 0)
                    throw new Exception("A largura do furo inferior não foi informada." + mensagemErro);

                if (alturaFuroSuperior == 0)
                    throw new Exception("A altura do furo superior não foi informada." + mensagemErro);

                if (larguraFuroSuperior == 0)
                    throw new Exception("A largura do furo superior não foi informada." + mensagemErro);

                decimal xFuroSup = larguraFuroSuperior + (decimal)descontoLap - (espFuroSup / 2);
                decimal yFuroSup = (decimal)altura + (decimal)descontoLap - alturaFuroSuperior;
                decimal iFuroSup = xFuroSup + (espFuroSup / 2);
                decimal jFuroSup = yFuroSup;

                decimal xFuroInf = larguraFuroInferior + (decimal)descontoLap - (espFuroInf / 2);
                decimal yFuroInf = alturaFuroInferior + (decimal)descontoLap;
                decimal iFuroInf = xFuroInf + (espFuroInf / 2);
                decimal jFuroInf = yFuroInf;

                conteudoArquivo = conteudoArquivo

                    // Faz o furo superior uerdo
                    .Replace("?XFuroSup?", Formatacoes.TrataValorDecimal(xFuroSup, 6))
                    .Replace("?YFuroSup?", Formatacoes.TrataValorDecimal(yFuroSup, 6))
                    .Replace("?IFuroSup?", Formatacoes.TrataValorDecimal(iFuroSup, 6))
                    .Replace("?JFuroSup?", Formatacoes.TrataValorDecimal(jFuroSup, 6))
                    .Replace("?RaioFuroSup?", Formatacoes.TrataValorDecimal((espFuroSup / 2), 6))

                    .Replace("?XFuroInf?", Formatacoes.TrataValorDecimal(xFuroInf, 6))
                    .Replace("?YFuroInf?", Formatacoes.TrataValorDecimal(yFuroInf, 6))
                    .Replace("?IFuroInf?", Formatacoes.TrataValorDecimal(iFuroInf, 6))
                    .Replace("?JFuroInf?", Formatacoes.TrataValorDecimal(jFuroInf, 6))
                    .Replace("?RaioFuroInf?", Formatacoes.TrataValorDecimal((espFuroInf / 2), 6));

                #endregion
            }
            else if (codigoArquivo == "FIXO4FUROS")
            {
                #region FIXO4FUROS

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int alturaFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroInferior);

                int larguraFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroInferior);

                int alturaFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroSuperior);

                int larguraFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroSuperior);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do furo não foi informada." + mensagemErro);

                if (alturaFuroInferior == 0)
                    throw new Exception("A altura do furo inferior não foi informada." + mensagemErro);

                if (larguraFuroInferior == 0)
                    throw new Exception("A largura do furo inferior não foi informada." + mensagemErro);

                if (alturaFuroSuperior == 0)
                    throw new Exception("A altura do furo superior não foi informada." + mensagemErro);

                if (larguraFuroSuperior == 0)
                    throw new Exception("A largura do furo superior não foi informada." + mensagemErro);

                decimal raioFuro = espFuroPux / 2;

                decimal xFuroSupEsq = larguraFuroSuperior + (decimal)descontoLap - raioFuro;
                decimal yFuroSupEsq = (decimal)altura + (decimal)descontoLap - alturaFuroSuperior;
                decimal iFuroSupEsq = xFuroSupEsq + raioFuro;
                decimal jFuroSupEsq = yFuroSupEsq;

                decimal xFuroInfEsq = larguraFuroInferior + (decimal)descontoLap - raioFuro;
                decimal yFuroInfEsq = (decimal)alturaFuroInferior + (decimal)descontoLap;
                decimal iFuroInfEsq = xFuroInfEsq + raioFuro;
                decimal jFuroInfEsq = yFuroInfEsq;

                decimal xFuroSupDir = ((decimal)largura + (decimal)descontoLap) - larguraFuroSuperior - raioFuro;
                decimal yFuroSupDir = ((decimal)altura + (decimal)descontoLap) - alturaFuroSuperior;
                decimal iFuroSupDir = xFuroSupDir + raioFuro;
                decimal jFuroSupDir = yFuroSupDir;

                decimal xFuroInfDir = (decimal)largura + (decimal)descontoLap - larguraFuroInferior - raioFuro;
                decimal yFuroInfDir = alturaFuroInferior + (decimal)descontoLap;
                decimal iFuroInfDir = xFuroInfDir + raioFuro;
                decimal jFuroInfDir = yFuroInfDir;

                conteudoArquivo = conteudoArquivo

                    // Faz o furo superior esquerdo
                    .Replace("?XFuroSupEsq?", Formatacoes.TrataValorDecimal(xFuroSupEsq, 6))
                    .Replace("?YFuroSupEsq?", Formatacoes.TrataValorDecimal(yFuroSupEsq, 6))
                    .Replace("?IFuroSupEsq?", Formatacoes.TrataValorDecimal(iFuroSupEsq, 6))
                    .Replace("?JFuroSupEsq?", Formatacoes.TrataValorDecimal(jFuroSupEsq, 6))
                    .Replace("?RaioFuroSupEsq?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroInfEsq?", Formatacoes.TrataValorDecimal(xFuroInfEsq, 6))
                    .Replace("?YFuroInfEsq?", Formatacoes.TrataValorDecimal(yFuroInfEsq, 6))
                    .Replace("?IFuroInfEsq?", Formatacoes.TrataValorDecimal(iFuroInfEsq, 6))
                    .Replace("?JFuroInfEsq?", Formatacoes.TrataValorDecimal(jFuroInfEsq, 6))
                    .Replace("?RaioFuroInfEsq?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroSupDir?", Formatacoes.TrataValorDecimal(xFuroSupDir, 6))
                    .Replace("?YFuroSupDir?", Formatacoes.TrataValorDecimal(yFuroSupDir, 6))
                    .Replace("?IFuroSupDir?", Formatacoes.TrataValorDecimal(iFuroSupDir, 6))
                    .Replace("?JFuroSupDir?", Formatacoes.TrataValorDecimal(jFuroSupDir, 6))
                    .Replace("?RaioFuroSupDir?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroInfDir?", Formatacoes.TrataValorDecimal(xFuroInfDir, 6))
                    .Replace("?YFuroInfDir?", Formatacoes.TrataValorDecimal(yFuroInfDir, 6))
                    .Replace("?IFuroInfDir?", Formatacoes.TrataValorDecimal(iFuroInfDir, 6))
                    .Replace("?JFuroInfDir?", Formatacoes.TrataValorDecimal(jFuroInfDir, 6))
                    .Replace("?RaioFuroInfDir?", Formatacoes.TrataValorDecimal(raioFuro, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXO6FUROS")
            {
                #region FIXO6FUROS

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                int alturaFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroInferior);

                int larguraFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroInferior);

                int alturaFuroCentral = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroCentral);

                int larguraFuroCentral = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroCentral);

                int alturaFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroSuperior);

                int larguraFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroSuperior);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do furo não foi informada." + mensagemErro);

                if (alturaFuroInferior == 0)
                    throw new Exception("A altura do furo inferior não foi informada." + mensagemErro);

                if (larguraFuroInferior == 0)
                    throw new Exception("A largura do furo inferior não foi informada." + mensagemErro);

                if (alturaFuroCentral == 0)
                    throw new Exception("A altura do furo central não foi informada." + mensagemErro);

                if (larguraFuroCentral == 0)
                    throw new Exception("A largura do furo central não foi informada." + mensagemErro);

                if (alturaFuroSuperior == 0)
                    throw new Exception("A altura do furo superior não foi informada." + mensagemErro);

                if (larguraFuroSuperior == 0)
                    throw new Exception("A largura do furo superior não foi informada." + mensagemErro);

                decimal raioFuro = espFuroPux / 2;

                decimal xFuroSupEsq = larguraFuroSuperior + (decimal)descontoLap - espFuroPux;
                decimal yFuroSupEsq = (decimal)altura + (decimal)descontoLap - alturaFuroSuperior;
                decimal iFuroSupEsq = xFuroSupEsq + espFuroPux;
                decimal jFuroSupEsq = yFuroSupEsq;

                decimal xFuroCentralEsq = larguraFuroCentral + (decimal)descontoLap - espFuroPux;
                decimal yFuroCentralEsq = (decimal)altura + (decimal)descontoLap - alturaFuroCentral;
                decimal iFuroCentralEsq = xFuroCentralEsq + espFuroPux;
                decimal jFuroCentralEsq = yFuroCentralEsq;

                decimal xFuroInfEsq = larguraFuroInferior + (decimal)descontoLap - espFuroPux;
                decimal yFuroInfEsq = (decimal)altura + (decimal)descontoLap - alturaFuroInferior;
                decimal iFuroInfEsq = xFuroInfEsq + espFuroPux;
                decimal jFuroInfEsq = yFuroInfEsq;

                decimal xFuroSupDir = (decimal)largura + (decimal)descontoLap - larguraFuroSuperior - espFuroPux;
                decimal yFuroSupDir = (decimal)altura + (decimal)descontoLap - alturaFuroSuperior;
                decimal iFuroSupDir = xFuroSupDir + espFuroPux;
                decimal jFuroSupDir = yFuroSupDir;

                decimal xFuroCentralDir = (decimal)largura + (decimal)descontoLap - larguraFuroCentral - espFuroPux;
                decimal yFuroCentralDir = (decimal)altura + (decimal)descontoLap - alturaFuroCentral;
                decimal iFuroCentralDir = xFuroCentralDir + espFuroPux;
                decimal jFuroCentralDir = yFuroCentralDir;

                decimal xFuroInfDir = (decimal)largura - larguraFuroInferior + (decimal)descontoLap - espFuroPux;
                decimal yFuroInfDir = (decimal)altura + (decimal)descontoLap - alturaFuroInferior;
                decimal iFuroInfDir = xFuroInfDir + espFuroPux;
                decimal jFuroInfDir = yFuroInfDir;

                conteudoArquivo = conteudoArquivo

                    // Faz o furo superior esquerdo
                    .Replace("?XFuroSupEsq?", Formatacoes.TrataValorDecimal(xFuroSupEsq, 6))
                    .Replace("?YFuroSupEsq?", Formatacoes.TrataValorDecimal(yFuroSupEsq, 6))
                    .Replace("?IFuroSupEsq?", Formatacoes.TrataValorDecimal(iFuroSupEsq, 6))
                    .Replace("?JFuroSupEsq?", Formatacoes.TrataValorDecimal(jFuroSupEsq, 6))
                    .Replace("?RaioFuroSupEsq?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroCentralEsq?", Formatacoes.TrataValorDecimal(xFuroCentralEsq, 6))
                    .Replace("?YFuroCentralEsq?", Formatacoes.TrataValorDecimal(yFuroCentralEsq, 6))
                    .Replace("?IFuroCentralEsq?", Formatacoes.TrataValorDecimal(iFuroCentralEsq, 6))
                    .Replace("?JFuroCentralEsq?", Formatacoes.TrataValorDecimal(jFuroCentralEsq, 6))
                    .Replace("?RaioFuroCentralEsq?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroInfEsq?", Formatacoes.TrataValorDecimal(xFuroInfEsq, 6))
                    .Replace("?YFuroInfEsq?", Formatacoes.TrataValorDecimal(yFuroInfEsq, 6))
                    .Replace("?IFuroInfEsq?", Formatacoes.TrataValorDecimal(iFuroInfEsq, 6))
                    .Replace("?JFuroInfEsq?", Formatacoes.TrataValorDecimal(jFuroInfEsq, 6))
                    .Replace("?RaioFuroInfEsq?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroSupDir?", Formatacoes.TrataValorDecimal(xFuroSupDir, 6))
                    .Replace("?YFuroSupDir?", Formatacoes.TrataValorDecimal(yFuroSupDir, 6))
                    .Replace("?IFuroSupDir?", Formatacoes.TrataValorDecimal(iFuroSupDir, 6))
                    .Replace("?JFuroSupDir?", Formatacoes.TrataValorDecimal(jFuroSupDir, 6))
                    .Replace("?RaioFuroSupDir?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroCentralDir?", Formatacoes.TrataValorDecimal(xFuroCentralDir, 6))
                    .Replace("?YFuroCentralDir?", Formatacoes.TrataValorDecimal(yFuroCentralDir, 6))
                    .Replace("?IFuroCentralDir?", Formatacoes.TrataValorDecimal(iFuroCentralDir, 6))
                    .Replace("?JFuroCentralDir?", Formatacoes.TrataValorDecimal(jFuroCentralDir, 6))
                    .Replace("?RaioFuroCentralDir?", Formatacoes.TrataValorDecimal(raioFuro, 6))

                    .Replace("?XFuroInfDir?", Formatacoes.TrataValorDecimal(xFuroInfDir, 6))
                    .Replace("?YFuroInfDir?", Formatacoes.TrataValorDecimal(yFuroInfDir, 6))
                    .Replace("?IFuroInfDir?", Formatacoes.TrataValorDecimal(iFuroInfDir, 6))
                    .Replace("?JFuroInfDir?", Formatacoes.TrataValorDecimal(jFuroInfDir, 6))
                    .Replace("?RaioFuroInfDir?", Formatacoes.TrataValorDecimal(raioFuro, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXOCONTRA1520VV")
            {
                #region FIXOCONTRA1520VV

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                // Faz o recorte da contra-fechadura 1520
                decimal xIniFech = (decimal)(15 + descontoLap);
                decimal yIniFech = alturaPuxador - 34;
                decimal xSegFech = xIniFech + 21;
                decimal ySegFech = yIniFech;

                decimal xCurva1 = xSegFech + 9;
                decimal yCurva1 = ySegFech + 9;
                decimal iCurva1 = xCurva1 - 9;
                decimal jCurva1 = yCurva1;
                decimal raioCurva1 = 9;

                decimal xTercFech = xCurva1;
                decimal yTercFech = yCurva1 + 55;

                decimal xCurva2 = xTercFech - 9;
                decimal yCurva2 = yTercFech + 9;
                decimal iCurva2 = xCurva2;
                decimal jCurva2 = yCurva2 - 9;
                decimal raioCurva2 = 9;

                decimal xFimFech = xCurva2 - 21;
                decimal yFimFech = yCurva2;

                conteudoArquivo = conteudoArquivo
                    // Faz a contra-fechadura
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))
                    .Replace("?XSegFech?", Formatacoes.TrataValorDecimal(xSegFech, 6))
                    .Replace("?YSegFech?", Formatacoes.TrataValorDecimal(ySegFech, 6))

                    .Replace("?XCurva1?", Formatacoes.TrataValorDecimal(xCurva1, 6))
                    .Replace("?YCurva1?", Formatacoes.TrataValorDecimal(yCurva1, 6))
                    .Replace("?ICurva1?", Formatacoes.TrataValorDecimal(iCurva1, 6))
                    .Replace("?JCurva1?", Formatacoes.TrataValorDecimal(jCurva1, 6))
                    .Replace("?RaioCurva1?", Formatacoes.TrataValorDecimal(raioCurva1, 6))

                    .Replace("?XTercFech?", Formatacoes.TrataValorDecimal(xTercFech, 6))
                    .Replace("?YTercFech?", Formatacoes.TrataValorDecimal(yTercFech, 6))

                    .Replace("?XCurva2?", Formatacoes.TrataValorDecimal(xCurva2, 6))
                    .Replace("?YCurva2?", Formatacoes.TrataValorDecimal(yCurva2, 6))
                    .Replace("?ICurva2?", Formatacoes.TrataValorDecimal(iCurva2, 6))
                    .Replace("?JCurva2?", Formatacoes.TrataValorDecimal(jCurva2, 6))
                    .Replace("?RaioCurva2?", Formatacoes.TrataValorDecimal(raioCurva2, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXOCONTRA3210VV")
            {
                #region FIXOCONTRA3210VV

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                // Faz os furos da 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                conteudoArquivo = conteudoArquivo
                    // Posiciona o CNC no furo maior da 3530 e faz o furo
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    // Posiciona o CNC no furo menor 1 da 3530 e faz o furo
                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Posiciona o CNC no furo menor 2 da 3530 e faz o furo
                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6));

                #endregion
            }
            else if (codigoArquivo == "FIXOUMRECORTE")
            {
                #region FIXOUMRECORTE

                int alturaRecorte = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaRecorte);

                int larguraRecorte = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraRecorte);

                if (alturaRecorte == 0)
                    throw new Exception("A altura do recorte não foi informada." + mensagemErro);

                if (larguraRecorte == 0)
                    throw new Exception("A largura do recorte não foi informada." + mensagemErro);

                // Se a altura ou a largura for menor que 16 não gera o arquivo SAG, porque esta é a medida do recuo mais o raio da curva.
                if (alturaRecorte < 16 || larguraRecorte < 16)
                    return false;

                decimal recuoRecorte = 10;
                decimal raioCurva = 5;

                decimal xRecInf1 = recuoRecorte + (decimal)descontoLap;
                decimal yRecInf1 = alturaRecorte + (decimal)descontoLap;

                decimal xRecInf2 = larguraRecorte - raioCurva + (decimal)descontoLap;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + raioCurva;
                decimal yRecInf3 = yRecInf2 - raioCurva;
                decimal iRecInf3 = xRecInf3 - raioCurva;
                decimal jRecInf3 = yRecInf3;

                decimal xRecInf4 = xRecInf3;
                decimal yRecInf4 = recuoRecorte + (decimal)descontoLap;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte inferior esquerdo
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioCurva, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6));

                #endregion
            }
            else if (codigoArquivo == "PIVCONTRA")
            {
                #region PIVCONTRA

                decimal raioPiv = 12;
                decimal recuoPivo = 10;
                decimal aberturaPivo = 32; // Distância do ponto inicial ao ponto final da marcação do pivô
                decimal distBordaYCentroPivo = (decimal)24;

                // Faz o pivô de cima
                decimal xPivCima1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                decimal yPivCima1 = (decimal)altura - recuoPivo + (decimal)descontoLap;

                decimal xPivCima2 = xPivCima1 + (decimal)4;
                decimal yPivCima2 = yPivCima1 - (decimal)15;

                decimal xPivCima3 = xPivCima2 + 24;
                decimal yPivCima3 = yPivCima2;
                decimal iPivCima3 = (largura / 2) + (decimal)descontoLap;
                decimal jPivCima3 = (decimal)altura + (decimal)descontoLap - distBordaYCentroPivo;

                decimal xPivCima4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                decimal yPivCima4 = yPivCima1;

                conteudoArquivo = conteudoArquivo

                    // Faz o pivotante de cima
                    .Replace("?XPivCima1?", Formatacoes.TrataValorDecimal(xPivCima1, 6))
                    .Replace("?YPivCima1?", Formatacoes.TrataValorDecimal(yPivCima1, 6))

                    .Replace("?XPivCima2?", Formatacoes.TrataValorDecimal(xPivCima2, 6))
                    .Replace("?YPivCima2?", Formatacoes.TrataValorDecimal(yPivCima2, 6))

                    .Replace("?XPivCima3?", Formatacoes.TrataValorDecimal(xPivCima3, 6))
                    .Replace("?YPivCima3?", Formatacoes.TrataValorDecimal(yPivCima3, 6))
                    .Replace("?IPivCima3?", Formatacoes.TrataValorDecimal(iPivCima3, 6))
                    .Replace("?JPivCima3?", Formatacoes.TrataValorDecimal(jPivCima3, 6))
                    .Replace("?RaioPivCima3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                    .Replace("?XPivCima4?", Formatacoes.TrataValorDecimal(xPivCima4, 6))
                    .Replace("?YPivCima4?", Formatacoes.TrataValorDecimal(yPivCima4, 6));

                #endregion
            }
            else if (codigoArquivo == "PIVTRILATERAL")
            {
                #region PIVTRILATERAL

                int altura1335_3539 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1335_3539);

                if (altura1335_3539 == 0)
                    throw new Exception("O campo altura da 1335/3539 não foi informado." + mensagemErro);

                // Neste arquivo a distância y ao centro do furo do trinco foi corrigida em relação aos demais arquivos de mesa,
                // neste caso foi necessário alterar esta distância
                config.Trinco.DistBordaYTrincoPtAbrir = 35;

                decimal raioPiv = 12;
                decimal recuoPivo = 10;
                decimal aberturaPivo = 32; // Distância do ponto inicial ao ponto final da marcação do pivô
                decimal distBordaYCentroPivo = (decimal)24;

                // Faz o pivô de baixo
                decimal xPivBaixo1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                decimal yPivBaixo1 = recuoPivo + (decimal)descontoLap;

                decimal xPivBaixo2 = xPivBaixo1 + (decimal)4;
                decimal yPivBaixo2 = yPivBaixo1 + (decimal)15;

                decimal xPivBaixo3 = xPivBaixo2 + 24;
                decimal yPivBaixo3 = yPivBaixo2;
                decimal iPivBaixo3 = (largura / 2) + (decimal)descontoLap;
                decimal jPivBaixo3 = (decimal)descontoLap + distBordaYCentroPivo;

                decimal xPivBaixo4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                decimal yPivBaixo4 = yPivBaixo1;

                // Faz o pivô de cima
                decimal xPivCima1 = (largura / 2) + (decimal)descontoLap - (aberturaPivo / 2);
                decimal yPivCima1 = (decimal)altura - recuoPivo + (decimal)descontoLap;

                decimal xPivCima2 = xPivCima1 + (decimal)4;
                decimal yPivCima2 = yPivCima1 - (decimal)15;

                decimal xPivCima3 = xPivCima2 + 24;
                decimal yPivCima3 = yPivCima2;
                decimal iPivCima3 = (largura / 2) + (decimal)descontoLap;
                decimal jPivCima3 = (decimal)altura + (decimal)descontoLap - distBordaYCentroPivo;

                decimal xPivCima4 = (largura / 2) + (decimal)descontoLap + (aberturaPivo / 2);
                decimal yPivCima4 = yPivCima1;

                // Faz o trinco
                decimal xTrinco = largura - (config.Trinco.DistBordaYTrincoPtAbrir - config.Trinco.RaioTrinco) + (decimal)descontoLap;
                decimal yTrinco = altura1335_3539 + config.Trinco.RaioTrinco + (decimal)descontoLap;
                decimal iTrinco = xTrinco;
                decimal jTrinco = yTrinco - config.Trinco.RaioTrinco;

                conteudoArquivo = conteudoArquivo

                    // Faz o pivotante de baixo
                    .Replace("?XPivBaixo1?", Formatacoes.TrataValorDecimal(xPivBaixo1, 6))
                    .Replace("?YPivBaixo1?", Formatacoes.TrataValorDecimal(yPivBaixo1, 6))

                    .Replace("?XPivBaixo2?", Formatacoes.TrataValorDecimal(xPivBaixo2, 6))
                    .Replace("?YPivBaixo2?", Formatacoes.TrataValorDecimal(yPivBaixo2, 6))

                    .Replace("?XPivBaixo3?", Formatacoes.TrataValorDecimal(xPivBaixo3, 6))
                    .Replace("?YPivBaixo3?", Formatacoes.TrataValorDecimal(yPivBaixo3, 6))
                    .Replace("?IPivBaixo3?", Formatacoes.TrataValorDecimal(iPivBaixo3, 6))
                    .Replace("?JPivBaixo3?", Formatacoes.TrataValorDecimal(jPivBaixo3, 6))
                    .Replace("?RaioPivBaixo3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                    .Replace("?XPivBaixo4?", Formatacoes.TrataValorDecimal(xPivBaixo4, 6))
                    .Replace("?YPivBaixo4?", Formatacoes.TrataValorDecimal(yPivBaixo4, 6))

                    // Faz o pivotante de cima
                    .Replace("?XPivCima1?", Formatacoes.TrataValorDecimal(xPivCima1, 6))
                    .Replace("?YPivCima1?", Formatacoes.TrataValorDecimal(yPivCima1, 6))

                    .Replace("?XPivCima2?", Formatacoes.TrataValorDecimal(xPivCima2, 6))
                    .Replace("?YPivCima2?", Formatacoes.TrataValorDecimal(yPivCima2, 6))

                    .Replace("?XPivCima3?", Formatacoes.TrataValorDecimal(xPivCima3, 6))
                    .Replace("?YPivCima3?", Formatacoes.TrataValorDecimal(yPivCima3, 6))
                    .Replace("?IPivCima3?", Formatacoes.TrataValorDecimal(iPivCima3, 6))
                    .Replace("?JPivCima3?", Formatacoes.TrataValorDecimal(jPivCima3, 6))
                    .Replace("?RaioPivCima3?", Formatacoes.TrataValorDecimal(raioPiv, 6))

                    .Replace("?XPivCima4?", Formatacoes.TrataValorDecimal(xPivCima4, 6))
                    .Replace("?YPivCima4?", Formatacoes.TrataValorDecimal(yPivCima4, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PIVTRINCO")
            {
                #region PIVTRINCO

                decimal larguraPeca = (decimal)(largura + aumentoPeca);
                decimal alturaPeca = (decimal)(altura + aumentoPeca);

                // Pivotante inferior
                decimal xPos1Piv1 = (larguraPeca / 2) - (decimal)18.5;
                decimal yPos1Piv1 = (decimal)descontoLap;

                decimal xPos2Piv1 = xPos1Piv1 + (decimal)6.333456;
                decimal yPos2Piv1 = yPos1Piv1 + (decimal)26.867961;

                decimal xPos3Piv1 = xPos2Piv1 + (decimal)24.333088;
                decimal yPos3Piv1 = yPos2Piv1;
                decimal iPos3Piv1 = larguraPeca / 2;
                decimal jPos3Piv1 = (decimal)descontoLap + 24;
                decimal raioPiv1 = (decimal)12.5;

                decimal xPos4Piv1 = (larguraPeca / 2) + (decimal)18.5;
                decimal yPos4Piv1 = (decimal)descontoLap;

                // Pivotante superior
                decimal xPos1Piv2 = (larguraPeca / 2) - (decimal)18.5;
                decimal yPos1Piv2 = (decimal)alturaPeca - (decimal)descontoLap;

                decimal xPos2Piv2 = xPos1Piv2 + (decimal)6.333456;
                decimal yPos2Piv2 = yPos1Piv2 - (decimal)26.867961;

                decimal xPos3Piv2 = xPos2Piv2 + (decimal)24.333088;
                decimal yPos3Piv2 = yPos2Piv2;
                decimal iPos3Piv2 = larguraPeca / 2;
                decimal jPos3Piv2 = alturaPeca - ((decimal)descontoLap + 24);
                decimal raioPiv2 = (decimal)12.5;

                decimal xPos4Piv2 = (larguraPeca / 2) + (decimal)18.5;
                decimal yPos4Piv2 = alturaPeca - (decimal)descontoLap;

                // Trinco
                decimal xPos1Trinco = larguraPeca - (decimal)((config.Trinco.DistBordaXTrinco + descontoLap)) + config.Trinco.RaioTrinco;
                decimal yPos1Trinco = (decimal)(26 + descontoLap);

                decimal xPos2Trinco = xPos1Trinco;
                decimal yPos2Trinco = yPos1Trinco;
                decimal iPos2Trinco = xPos2Trinco - config.Trinco.RaioTrinco;
                decimal jPos2Trinco = yPos1Trinco;

                conteudoArquivo = conteudoArquivo

                    // Pivotante inferior
                    .Replace("?XPos1Piv1?", Formatacoes.TrataValorDecimal(xPos1Piv1, 6))
                    .Replace("?YPos1Piv1?", Formatacoes.TrataValorDecimal(yPos1Piv1, 6))

                    .Replace("?XPos2Piv1?", Formatacoes.TrataValorDecimal(xPos2Piv1, 6))
                    .Replace("?YPos2Piv1?", Formatacoes.TrataValorDecimal(yPos2Piv1, 6))

                    .Replace("?XPos3Piv1?", Formatacoes.TrataValorDecimal(xPos3Piv1, 6))
                    .Replace("?YPos3Piv1?", Formatacoes.TrataValorDecimal(yPos3Piv1, 6))
                    .Replace("?IPos3Piv1?", Formatacoes.TrataValorDecimal(iPos3Piv1, 6))
                    .Replace("?JPos3Piv1?", Formatacoes.TrataValorDecimal(jPos3Piv1, 6))
                    .Replace("?RaioPiv1?", Formatacoes.TrataValorDecimal(raioPiv1, 6))

                    .Replace("?XPos4Piv1?", Formatacoes.TrataValorDecimal(xPos4Piv1, 6))
                    .Replace("?YPos4Piv1?", Formatacoes.TrataValorDecimal(yPos4Piv1, 6))

                    // Pivotante superior
                    .Replace("?XPos1Piv2?", Formatacoes.TrataValorDecimal(xPos1Piv2, 6))
                    .Replace("?YPos1Piv2?", Formatacoes.TrataValorDecimal(yPos1Piv2, 6))

                    .Replace("?XPos2Piv2?", Formatacoes.TrataValorDecimal(xPos2Piv2, 6))
                    .Replace("?YPos2Piv2?", Formatacoes.TrataValorDecimal(yPos2Piv2, 6))

                    .Replace("?XPos3Piv2?", Formatacoes.TrataValorDecimal(xPos3Piv2, 6))
                    .Replace("?YPos3Piv2?", Formatacoes.TrataValorDecimal(yPos3Piv2, 6))
                    .Replace("?IPos3Piv2?", Formatacoes.TrataValorDecimal(iPos3Piv2, 6))
                    .Replace("?JPos3Piv2?", Formatacoes.TrataValorDecimal(jPos3Piv2, 6))
                    .Replace("?RaioPiv2?", Formatacoes.TrataValorDecimal(raioPiv2, 6))

                    .Replace("?XPos4Piv2?", Formatacoes.TrataValorDecimal(xPos4Piv2, 6))
                    .Replace("?YPos4Piv2?", Formatacoes.TrataValorDecimal(yPos4Piv2, 6))

                    // Trinco
                    .Replace("?XPos1Trinco?", Formatacoes.TrataValorDecimal(xPos1Trinco, 6))
                    .Replace("?YPos1Trinco?", Formatacoes.TrataValorDecimal(yPos1Trinco, 6))

                    .Replace("?XPos2Trinco?", Formatacoes.TrataValorDecimal(xPos2Trinco, 6))
                    .Replace("?YPos2Trinco?", Formatacoes.TrataValorDecimal(yPos2Trinco, 6))
                    .Replace("?IPos2Trinco?", Formatacoes.TrataValorDecimal(iPos2Trinco, 6))
                    .Replace("?JPos2Trinco?", Formatacoes.TrataValorDecimal(jPos2Trinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIR1520SEMPUX")
            {
                #region PTABRIR1520SEMPUX

                int alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("O campo altura da fechadura não foi informado." + mensagemErro);

                // Faz o recorte da fechadura 1520
                float xIniFech = largura - (10F + descontoLap);
                float yIniFech = alturaFechadura - 36.5F;

                float xIni2Fech = xIniFech - 22F;
                float yIni2Fech = yIniFech;

                float xCurva1Fech = xIni2Fech - 8F;
                float yCurva1Fech = yIni2Fech + 8F;
                float iCurva1Fech = xCurva1Fech + 8F;
                float jCurva1Fech = yCurva1Fech;
                float raioCurva1Fech = 8F;

                float xMeioFech = xCurva1Fech;
                float yMeioFech = yCurva1Fech + 65F;

                float xCurva2Fech = xMeioFech + 8F;
                float yCurva2Fech = yMeioFech + 8F;
                float iCurva2Fech = xCurva2Fech;
                float jCurva2Fech = yCurva2Fech - 8F;
                float raioCurva2Fech = 8;

                float xFimFech = xCurva2Fech + 22F;
                float yFimFech = yCurva2Fech;

                // Faz o recorte superior
                float xRecSup1 = 10F + descontoLap;
                float yRecSup1 = altura - (25F - descontoLap);

                float xRecSup2 = xRecSup1 + 83.992189F;
                float yRecSup2 = yRecSup1;

                float xRecSup3 = xRecSup2 + 3.880682F;
                float yRecSup3 = yRecSup2 - 3.030303F;
                float iRecSup3 = xRecSup2;
                float jRecSup3 = yRecSup2 - 4F;
                float raioRecSup3 = 4F;

                float xRecSup4 = xRecSup3 + 15.157432F;
                float yRecSup4 = yRecSup3 + 15.157432F;
                float iRecSup4 = xRecSup4 - 3.030303F;
                float jRecSup4 = jRecSup3 + 4F;
                float raioRecSup4 = 12.5F;

                float xRecSup5 = iRecSup4;
                float yRecSup5 = yRecSup4 + 3.880682F;
                float iRecSup5 = xRecSup5 + 4F;
                float jRecSup5 = yRecSup5;
                float raioRecSup5 = 4F;

                // Faz o recorte inferior
                float xRecInf1 = 10f + descontoLap;
                float yRecInf1 = 30f - descontoLap;

                float xRecInf2 = xRecInf1 + 98.992189F;
                float yRecInf2 = yRecInf1;

                float xRecInf3 = xRecInf2 + 3.880682F;
                float yRecInf3 = yRecInf2 + 3.030303F;
                float iRecInf3 = xRecInf2;
                float jRecInf3 = yRecInf2 + 4F;
                float raioRecInf3 = 4F;

                float xRecInf4 = xRecInf3 + 15.157432F;
                float yRecInf4 = yRecInf3 - 15.157432F;
                float iRecInf4 = xRecInf4 - 3.030303F;
                float jRecInf4 = jRecInf3 - 4F;
                float raioRecInf4 = 12.5F;

                float xRecInf5 = xRecInf4 - 3.030303F;
                float yRecInf5 = yRecInf4 - 3.880682F;
                float iRecInf5 = iRecInf4 + 4F;
                float jRecInf5 = jRecInf4 - 16.007811F;
                float raioRecInf5 = 4F;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte da fechadura 1520
                    .Replace("?XIniFech?", Formatacoes.TrataValorDouble(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDouble(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDouble(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDouble(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDouble(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDouble(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDouble(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDouble(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDouble(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDouble(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDouble(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDouble(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDouble(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDouble(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDouble(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDouble(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDouble(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDouble(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDouble(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDouble(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDouble(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDouble(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDouble(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDouble(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDouble(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDouble(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDouble(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDouble(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDouble(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDouble(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDouble(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDouble(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDouble(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDouble(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDouble(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDouble(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDouble(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDouble(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDouble(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDouble(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDouble(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDouble(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDouble(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDouble(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDouble(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDouble(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDouble(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDouble(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDouble(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDouble(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDouble(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDouble(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDouble(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDouble(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDouble(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDouble(raioRecInf5, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPL")
            {
                #region PTABRIRPUXDUPL

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPL1520NOTRINCO")
            {
                #region PTABRIRPUXDUPL1520NOTRINCO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;
                int distBorda1520 = 100;
                int alt1520 = 28;
                int raioCurvaFech = 8;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte da fechadura 1520 no lugar do trinco
                decimal xIniFech = largura - ((distBorda1520 - raioFuroPux) - (decimal)descontoLap);
                decimal yIniFech = alt1520 - (raioCurvaFech + (decimal)descontoLap);

                decimal xIni2Fech = xIniFech;
                decimal yIni2Fech = yIniFech + (decimal)22;

                decimal xCurva1Fech = xIni2Fech + (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech;
                decimal jCurva1Fech = yIni2Fech;
                decimal raioCurva1Fech = raioCurvaFech;

                decimal xMeioFech = xCurva1Fech + (decimal)57;
                decimal yMeioFech = yCurva1Fech;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech - (decimal)8;
                decimal iCurva2Fech = xCurva2Fech - (decimal)8;
                decimal jCurva2Fech = yCurva2Fech;
                decimal raioCurva2Fech = raioCurvaFech;

                decimal xFimFech = xCurva2Fech;
                decimal yFimFech = yCurva2Fech - (decimal)22;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1520
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPL3210")
            {
                #region PTABRIRPUXDUPL3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 100; // Distância da direita para a esquerda do furo do puxador

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPLTRI")
            {
                #region PTABRIRPUXDUPLTRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPLTRI3210")
            {
                #region PTABRIRPUXDUPLTRI3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPLTRICIMABAIXO")
            {
                #region PTABRIRPUXDUPLTRICIMABAIXO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz os dois trincos
                decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco1 = ((decimal)altura - config.Trinco.DistBordaYTrincoSup) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                decimal jTrinco1 = yTrinco1;

                decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                /* Chamado 17880. */
                if (ProducaoConfig.ConfiguracaoTrincoMirandex)
                    yTrinco2 -= 10;
                decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                decimal jTrinco2 = yTrinco2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz os trincos
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                    .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                    .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                    .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                    .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                    .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                    .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXDUPLTRICIMABAIXO3210")
            {
                #region PTABRIRPUXDUPLTRICIMABAIXO3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int distEixoFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                if (distEixoFuroPux == 0)
                    throw new Exception("O campo distância do eixo do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                distEixoFuroPux = distEixoFuroPux == 0 ? 100 : distEixoFuroPux;

                // Faz os dois furos do puxador
                decimal xFuroPux1 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux1 = (alturaPuxador + (decimal)descontoLap) - (distEixoFuroPux / 2);
                decimal iFuroPux1 = xFuroPux1 - raioFuroPux;
                decimal jFuroPux1 = yFuroPux1;

                decimal xFuroPux2 = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux2 = (alturaPuxador + (decimal)descontoLap) + (distEixoFuroPux / 2);
                decimal iFuroPux2 = xFuroPux2 - raioFuroPux;
                decimal jFuroPux2 = yFuroPux2;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz os dois trincos
                decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco1 = ((decimal)altura - config.Trinco.DistBordaYTrincoSup) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                decimal jTrinco1 = yTrinco1;

                decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - ((decimal)descontoLap + config.Trinco.RaioTrinco);
                decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                decimal jTrinco2 = yTrinco2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux1?", Formatacoes.TrataValorDecimal(xFuroPux1, 6))
                    .Replace("?YFuroPux1?", Formatacoes.TrataValorDecimal(yFuroPux1, 6))
                    .Replace("?IFuroPux1?", Formatacoes.TrataValorDecimal(iFuroPux1, 6))
                    .Replace("?JFuroPux1?", Formatacoes.TrataValorDecimal(jFuroPux1, 6))
                    .Replace("?RaioFuroPux1?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Posiciona o CNC no furo do puxador 2 e faz o furo
                    .Replace("?XFuroPux2?", Formatacoes.TrataValorDecimal(xFuroPux2, 6))
                    .Replace("?YFuroPux2?", Formatacoes.TrataValorDecimal(yFuroPux2, 6))
                    .Replace("?IFuroPux2?", Formatacoes.TrataValorDecimal(iFuroPux2, 6))
                    .Replace("?JFuroPux2?", Formatacoes.TrataValorDecimal(jFuroPux2, 6))
                    .Replace("?RaioFuroPux2?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Faz os trincos
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                    .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                    .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                    .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                    .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                    .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                    .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLES")
            {
                #region PTABRIRPUXSIMPLES

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLES1520NOTRINCO")
            {
                #region PTABRIRPUXSIMPLES1520NOTRINCO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador
                int distBorda1520 = 100;
                int alt1520 = 28;
                int raioCurvaFech = 8;

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte da fechadura 1520 no lugar do trinco
                decimal xIniFech = largura - ((distBorda1520 - raioFuroPux) - (decimal)descontoLap);
                decimal yIniFech = alt1520 - (raioCurvaFech + (decimal)descontoLap);

                decimal xIni2Fech = xIniFech;
                decimal yIni2Fech = yIniFech + (decimal)22;

                decimal xCurva1Fech = xIni2Fech + (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech;
                decimal jCurva1Fech = yIni2Fech;
                decimal raioCurva1Fech = raioCurvaFech;

                decimal xMeioFech = xCurva1Fech + (decimal)57;
                decimal yMeioFech = yCurva1Fech;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech - (decimal)8;
                decimal iCurva2Fech = xCurva2Fech - (decimal)8;
                decimal jCurva2Fech = yCurva2Fech;
                decimal raioCurva2Fech = raioCurvaFech;

                decimal xFimFech = xCurva2Fech;
                decimal yFimFech = yCurva2Fech - (decimal)22;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLES3210")
            {
                #region PTABRIRPUXSIMPLES3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLESTRI")
            {
                #region PTABRIRPUXSIMPLESTRI

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLESTRI3210")
            {
                #region PTABRIRPUXSIMPLESTRI3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz o trinco
                decimal xTrinco = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                decimal iTrinco = xTrinco - config.Trinco.RaioTrinco;
                decimal jTrinco = yTrinco;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Faz o trinco
                    .Replace("?XTrinco?", Formatacoes.TrataValorDecimal(xTrinco, 6))
                    .Replace("?YTrinco?", Formatacoes.TrataValorDecimal(yTrinco, 6))
                    .Replace("?ITrinco?", Formatacoes.TrataValorDecimal(iTrinco, 6))
                    .Replace("?JTrinco?", Formatacoes.TrataValorDecimal(jTrinco, 6))
                    .Replace("?RaioTrinco?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLESTRICIMABAIXO")
            {
                #region PTABRIRPUXSIMPLESTRICIMABAIXO

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);


                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte da fechadura 1510
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz os dois trincos
                decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco1 = ((decimal)altura - config.Trinco.DistBordaYTrincoSup) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                decimal jTrinco1 = yTrinco1;

                decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - ((decimal)descontoLap + config.Trinco.RaioTrinco);
                decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                decimal jTrinco2 = yTrinco2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1510
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz os trincos
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                    .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                    .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                    .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                    .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                    .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                    .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "PTABRIRPUXSIMPLESTRICIMABAIXO3210")
            {
                #region PTABRIRPUXSIMPLESTRICIMABAIXO3210

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte superior
                decimal xRecSup1 = (decimal)(10f + descontoLap);
                decimal yRecSup1 = (decimal)(altura - (25 - descontoLap));

                decimal xRecSup2 = xRecSup1 + (decimal)83.992189;
                decimal yRecSup2 = yRecSup1;

                decimal xRecSup3 = xRecSup2 + (decimal)3.880682;
                decimal yRecSup3 = yRecSup2 - (decimal)3.030303;
                decimal iRecSup3 = xRecSup2;
                decimal jRecSup3 = yRecSup2 - (decimal)4;
                decimal raioRecSup3 = 4;

                decimal xRecSup4 = xRecSup3 + (decimal)15.157432;
                decimal yRecSup4 = yRecSup3 + (decimal)15.157432;
                decimal iRecSup4 = xRecSup4 - (decimal)3.030303;
                decimal jRecSup4 = jRecSup3 + 4;
                decimal raioRecSup4 = (decimal)12.5;

                decimal xRecSup5 = iRecSup4;
                decimal yRecSup5 = yRecSup4 + (decimal)3.880682;
                decimal iRecSup5 = xRecSup5 + 4;
                decimal jRecSup5 = yRecSup5;
                decimal raioRecSup5 = 4;

                // Faz o recorte inferior
                decimal xRecInf1 = (decimal)(10f + descontoLap);
                decimal yRecInf1 = (decimal)(30f - descontoLap);

                decimal xRecInf2 = xRecInf1 + (decimal)98.992189;
                decimal yRecInf2 = yRecInf1;

                decimal xRecInf3 = xRecInf2 + (decimal)3.880682;
                decimal yRecInf3 = yRecInf2 + (decimal)3.030303;
                decimal iRecInf3 = xRecInf2;
                decimal jRecInf3 = yRecInf2 + (decimal)4;
                decimal raioRecInf3 = 4;

                decimal xRecInf4 = xRecInf3 + (decimal)15.157432;
                decimal yRecInf4 = yRecInf3 - (decimal)15.157432;
                decimal iRecInf4 = xRecInf4 - (decimal)3.030303;
                decimal jRecInf4 = jRecInf3 - (decimal)4;
                decimal raioRecInf4 = (decimal)12.5;

                decimal xRecInf5 = xRecInf4 - (decimal)3.030303;
                decimal yRecInf5 = yRecInf4 - (decimal)3.880682;
                decimal iRecInf5 = iRecInf4 + (decimal)4;
                decimal jRecInf5 = jRecInf4 - (decimal)16.007811;
                decimal raioRecInf5 = 4;

                // Faz a fechadura 3530
                float xFuroMaior = (largura + descontoLap) - 20; // Distância da direita para a esquerda do furo da fechadura
                float yFuroMaior = alturaPuxador + descontoLap;
                float iFuroMaior = xFuroMaior - config.Fechadura3530.RaioFuroMaior;
                float jFuroMaior = yFuroMaior;

                float xFuroMenor1 = xFuroMaior - descontoLap;
                float yFuroMenor1 = yFuroMaior + 42.5f;
                float iFuroMenor1 = xFuroMenor1 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor1 = yFuroMenor1;

                float xFuroMenor2 = xFuroMaior - descontoLap;
                float yFuroMenor2 = yFuroMaior - 42.5f;
                float iFuroMenor2 = xFuroMenor2 - config.Fechadura3530.RaioFuroMenor;
                float jFuroMenor2 = yFuroMenor2;

                // Faz os dois trincos
                decimal xTrinco1 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco1 = ((decimal)altura - config.Trinco.DistBordaYTrincoSup) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal iTrinco1 = xTrinco1 - config.Trinco.RaioTrinco;
                decimal jTrinco1 = yTrinco1;

                decimal xTrinco2 = (largura - config.Trinco.DistBordaXTrinco) + (decimal)descontoLap + config.Trinco.RaioTrinco;
                decimal yTrinco2 = config.Trinco.DistBordaYTrincoPtAbrir - (decimal)descontoLap;
                decimal iTrinco2 = xTrinco2 - config.Trinco.RaioTrinco;
                decimal jTrinco2 = yTrinco2;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte superior
                    .Replace("?XRecSup1?", Formatacoes.TrataValorDecimal(xRecSup1, 6))
                    .Replace("?YRecSup1?", Formatacoes.TrataValorDecimal(yRecSup1, 6))

                    .Replace("?XRecSup2?", Formatacoes.TrataValorDecimal(xRecSup2, 6))
                    .Replace("?YRecSup2?", Formatacoes.TrataValorDecimal(yRecSup2, 6))

                    .Replace("?XRecSup3?", Formatacoes.TrataValorDecimal(xRecSup3, 6))
                    .Replace("?YRecSup3?", Formatacoes.TrataValorDecimal(yRecSup3, 6))
                    .Replace("?IRecSup3?", Formatacoes.TrataValorDecimal(iRecSup3, 6))
                    .Replace("?JRecSup3?", Formatacoes.TrataValorDecimal(jRecSup3, 6))
                    .Replace("?RaioRecSup3?", Formatacoes.TrataValorDecimal(raioRecSup3, 6))

                    .Replace("?XRecSup4?", Formatacoes.TrataValorDecimal(xRecSup4, 6))
                    .Replace("?YRecSup4?", Formatacoes.TrataValorDecimal(yRecSup4, 6))
                    .Replace("?IRecSup4?", Formatacoes.TrataValorDecimal(iRecSup4, 6))
                    .Replace("?JRecSup4?", Formatacoes.TrataValorDecimal(jRecSup4, 6))
                    .Replace("?RaioRecSup4?", Formatacoes.TrataValorDecimal(raioRecSup4, 6))

                    .Replace("?XRecSup5?", Formatacoes.TrataValorDecimal(xRecSup5, 6))
                    .Replace("?YRecSup5?", Formatacoes.TrataValorDecimal(yRecSup5, 6))
                    .Replace("?IRecSup5?", Formatacoes.TrataValorDecimal(iRecSup5, 6))
                    .Replace("?JRecSup5?", Formatacoes.TrataValorDecimal(jRecSup5, 6))
                    .Replace("?RaioRecSup5?", Formatacoes.TrataValorDecimal(raioRecSup5, 6))

                    // Faz o recorte superior
                    .Replace("?XRecInf1?", Formatacoes.TrataValorDecimal(xRecInf1, 6))
                    .Replace("?YRecInf1?", Formatacoes.TrataValorDecimal(yRecInf1, 6))

                    .Replace("?XRecInf2?", Formatacoes.TrataValorDecimal(xRecInf2, 6))
                    .Replace("?YRecInf2?", Formatacoes.TrataValorDecimal(yRecInf2, 6))

                    .Replace("?XRecInf3?", Formatacoes.TrataValorDecimal(xRecInf3, 6))
                    .Replace("?YRecInf3?", Formatacoes.TrataValorDecimal(yRecInf3, 6))
                    .Replace("?IRecInf3?", Formatacoes.TrataValorDecimal(iRecInf3, 6))
                    .Replace("?JRecInf3?", Formatacoes.TrataValorDecimal(jRecInf3, 6))
                    .Replace("?RaioRecInf3?", Formatacoes.TrataValorDecimal(raioRecInf3, 6))

                    .Replace("?XRecInf4?", Formatacoes.TrataValorDecimal(xRecInf4, 6))
                    .Replace("?YRecInf4?", Formatacoes.TrataValorDecimal(yRecInf4, 6))
                    .Replace("?IRecInf4?", Formatacoes.TrataValorDecimal(iRecInf4, 6))
                    .Replace("?JRecInf4?", Formatacoes.TrataValorDecimal(jRecInf4, 6))
                    .Replace("?RaioRecInf4?", Formatacoes.TrataValorDecimal(raioRecInf4, 6))

                    .Replace("?XRecInf5?", Formatacoes.TrataValorDecimal(xRecInf5, 6))
                    .Replace("?YRecInf5?", Formatacoes.TrataValorDecimal(yRecInf5, 6))
                    .Replace("?IRecInf5?", Formatacoes.TrataValorDecimal(iRecInf5, 6))
                    .Replace("?JRecInf5?", Formatacoes.TrataValorDecimal(jRecInf5, 6))
                    .Replace("?RaioRecInf5?", Formatacoes.TrataValorDecimal(raioRecInf5, 6))

                    // Faz a fechadura 3530
                    .Replace("?XFuroMaior?", Formatacoes.TrataValorDouble(xFuroMaior, 6))
                    .Replace("?YFuroMaior?", Formatacoes.TrataValorDouble(yFuroMaior, 6))
                    .Replace("?IFuroMaior?", Formatacoes.TrataValorDouble(iFuroMaior, 6))
                    .Replace("?JFuroMaior?", Formatacoes.TrataValorDouble(jFuroMaior, 6))
                    .Replace("?RaioFuroMaior?", Formatacoes.TrataValorDouble(config.Fechadura3530.RaioFuroMaior, 6))

                    .Replace("?XFuroMenor1?", Formatacoes.TrataValorDouble(xFuroMenor1, 6))
                    .Replace("?YFuroMenor1?", Formatacoes.TrataValorDouble(yFuroMenor1, 6))
                    .Replace("?IFuroMenor1?", Formatacoes.TrataValorDouble(iFuroMenor1, 6))
                    .Replace("?JFuroMenor1?", Formatacoes.TrataValorDouble(jFuroMenor1, 6))
                    .Replace("?RaioFuroMenor1?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    .Replace("?XFuroMenor2?", Formatacoes.TrataValorDouble(xFuroMenor2, 6))
                    .Replace("?YFuroMenor2?", Formatacoes.TrataValorDouble(yFuroMenor2, 6))
                    .Replace("?IFuroMenor2?", Formatacoes.TrataValorDouble(iFuroMenor2, 6))
                    .Replace("?JFuroMenor2?", Formatacoes.TrataValorDouble(jFuroMenor2, 6))
                    .Replace("?RaioFuroMenor2?", Formatacoes.TrataValorDecimal(config.Fechadura3530.RaioFuroMenor, 6))

                    // Faz os trincos
                    .Replace("?XTrinco1?", Formatacoes.TrataValorDecimal(xTrinco1, 6))
                    .Replace("?YTrinco1?", Formatacoes.TrataValorDecimal(yTrinco1, 6))
                    .Replace("?ITrinco1?", Formatacoes.TrataValorDecimal(iTrinco1, 6))
                    .Replace("?JTrinco1?", Formatacoes.TrataValorDecimal(jTrinco1, 6))
                    .Replace("?RaioTrinco1?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6))

                    .Replace("?XTrinco2?", Formatacoes.TrataValorDecimal(xTrinco2, 6))
                    .Replace("?YTrinco2?", Formatacoes.TrataValorDecimal(yTrinco2, 6))
                    .Replace("?ITrinco2?", Formatacoes.TrataValorDecimal(iTrinco2, 6))
                    .Replace("?JTrinco2?", Formatacoes.TrataValorDecimal(jTrinco2, 6))
                    .Replace("?RaioTrinco2?", Formatacoes.TrataValorDecimal(config.Trinco.RaioTrinco, 6));

                #endregion
            }
            else if (codigoArquivo == "REIKIFECH")
            {
                #region REIKIFECH

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                // Faz o recorte da fechadura 1520
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                conteudoArquivo = conteudoArquivo

                    // Faz o recorte da fechadura 1520
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6));

                #endregion
            }
            else if (codigoArquivo == "REIKIPUX")
            {
                #region REIKIPUX

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                float raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador

                float xFuroPux = largura - ((distBordaFuro - raioFuroPux) - descontoLap);
                float yFuroPux = alturaPuxador + descontoLap;
                float iFuroPux = xFuroPux - raioFuroPux;
                float jFuroPux = yFuroPux;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDouble(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDouble(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDouble(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDouble(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDouble(raioFuroPux, 6));

                #endregion
            }
            else if (codigoArquivo == "REIKIPUXFECH")
            {
                #region REIKIPUXFECH

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                decimal raioFuroPux = espFuroPux > 0 ? espFuroPux / 2 : 5;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 150; // Distância da direita para a esquerda do furo do puxador

                // Faz o furo do puxador
                decimal xFuroPux = largura - ((distBordaFuro - raioFuroPux) - (decimal)descontoLap);
                decimal yFuroPux = (alturaPuxador + (decimal)descontoLap);
                decimal iFuroPux = xFuroPux - raioFuroPux;
                decimal jFuroPux = yFuroPux;

                // Faz o recorte da fechadura 1520
                decimal xIniFech = largura - ((decimal)10 + (decimal)descontoLap);
                decimal yIniFech = alturaPuxador - (decimal)34;

                decimal xIni2Fech = xIniFech - (decimal)22;
                decimal yIni2Fech = yIniFech;

                decimal xCurva1Fech = xIni2Fech - (decimal)8;
                decimal yCurva1Fech = yIni2Fech + (decimal)8;
                decimal iCurva1Fech = xCurva1Fech + (decimal)8;
                decimal jCurva1Fech = yCurva1Fech;
                decimal raioCurva1Fech = 8;

                decimal xMeioFech = xCurva1Fech;
                decimal yMeioFech = yCurva1Fech + (decimal)57;

                decimal xCurva2Fech = xMeioFech + (decimal)8;
                decimal yCurva2Fech = yMeioFech + (decimal)8;
                decimal iCurva2Fech = xCurva2Fech;
                decimal jCurva2Fech = yCurva2Fech - (decimal)8;
                decimal raioCurva2Fech = 8;

                decimal xFimFech = xCurva2Fech + (decimal)22;
                decimal yFimFech = yCurva2Fech;

                conteudoArquivo = conteudoArquivo

                    // Posiciona o CNC no furo do puxador 1 e faz o furo
                    .Replace("?XFuroPux?", Formatacoes.TrataValorDecimal(xFuroPux, 6))
                    .Replace("?YFuroPux?", Formatacoes.TrataValorDecimal(yFuroPux, 6))
                    .Replace("?IFuroPux?", Formatacoes.TrataValorDecimal(iFuroPux, 6))
                    .Replace("?JFuroPux?", Formatacoes.TrataValorDecimal(jFuroPux, 6))
                    .Replace("?RaioFuroPux?", Formatacoes.TrataValorDecimal(raioFuroPux, 6))

                    // Faz o recorte da fechadura 1520
                    .Replace("?XIniFech?", Formatacoes.TrataValorDecimal(xIniFech, 6))
                    .Replace("?YIniFech?", Formatacoes.TrataValorDecimal(yIniFech, 6))

                    .Replace("?XIni2Fech?", Formatacoes.TrataValorDecimal(xIni2Fech, 6))
                    .Replace("?YIni2Fech?", Formatacoes.TrataValorDecimal(yIni2Fech, 6))

                    .Replace("?XCurva1Fech?", Formatacoes.TrataValorDecimal(xCurva1Fech, 6))
                    .Replace("?YCurva1Fech?", Formatacoes.TrataValorDecimal(yCurva1Fech, 6))
                    .Replace("?ICurva1Fech?", Formatacoes.TrataValorDecimal(iCurva1Fech, 6))
                    .Replace("?JCurva1Fech?", Formatacoes.TrataValorDecimal(jCurva1Fech, 6))
                    .Replace("?RaioCurva1Fech?", Formatacoes.TrataValorDecimal(raioCurva1Fech, 6))

                    .Replace("?XMeioFech?", Formatacoes.TrataValorDecimal(xMeioFech, 6))
                    .Replace("?YMeioFech?", Formatacoes.TrataValorDecimal(yMeioFech, 6))

                    .Replace("?XCurva2Fech?", Formatacoes.TrataValorDecimal(xCurva2Fech, 6))
                    .Replace("?YCurva2Fech?", Formatacoes.TrataValorDecimal(yCurva2Fech, 6))
                    .Replace("?ICurva2Fech?", Formatacoes.TrataValorDecimal(iCurva2Fech, 6))
                    .Replace("?JCurva2Fech?", Formatacoes.TrataValorDecimal(jCurva2Fech, 6))
                    .Replace("?RaioCurva2Fech?", Formatacoes.TrataValorDecimal(raioCurva2Fech, 6))

                    .Replace("?XFimFech?", Formatacoes.TrataValorDecimal(xFimFech, 6))
                    .Replace("?YFimFech?", Formatacoes.TrataValorDecimal(yFimFech, 6));

                #endregion
            }
            else if (codigoArquivo == "SUPCOM1203CONJDUASPORTAS")
            {
                #region SUPCOM1203CONJDUASPORTAS

                // Calcula o recorte da direita
                decimal xRecDir1 = largura - (decimal)(15 + descontoLap);
                decimal yRecDir1 = (decimal)descontoLap + 25;

                decimal xRecDir2 = xRecDir1 - (decimal)78.992189;
                decimal yRecDir2 = yRecDir1;

                decimal xRecDir3 = xRecDir2 - (decimal)3.880682;
                decimal yRecDir3 = yRecDir2 + (decimal)3.030303;
                decimal iRecDir3 = xRecDir3 + (decimal)3.880682;
                decimal jRecDir3 = yRecDir2 + 4;
                decimal raioRecDir3 = 4;

                decimal xRecDir4 = xRecDir3 - (decimal)15.157432;
                decimal yRecDir4 = yRecDir3 - (decimal)15.157432;
                decimal iRecDir4 = xRecDir4 + (decimal)3.030303;
                decimal jRecDir4 = jRecDir3 - 4;
                decimal raioRecDir4 = (decimal)12.5;

                decimal xRecDir5 = xRecDir4 + (decimal)3.030303;
                decimal yRecDir5 = yRecDir4 - (decimal)3.880682;
                decimal iRecDir5 = iRecDir4 - 4;
                decimal jRecDir5 = jRecDir4 - (decimal)16.007811;
                decimal raioRecDir5 = 4;

                // Calcula o recorte da esquerda
                decimal xRecEsq1 = (decimal)descontoLap + 15;
                decimal yRecEsq1 = (decimal)descontoLap + 25;

                decimal xRecEsq2 = xRecEsq1 + (decimal)78.992189;
                decimal yRecEsq2 = yRecEsq1;

                decimal xRecEsq3 = xRecEsq2 + (decimal)3.880682;
                decimal yRecEsq3 = yRecEsq2 + (decimal)3.030303;
                decimal iRecEsq3 = xRecEsq2;
                decimal jRecEsq3 = yRecEsq2 + 4;
                decimal raioRecEsq3 = 4;

                decimal xRecEsq4 = xRecEsq3 + (decimal)15.157432;
                decimal yRecEsq4 = yRecEsq3 - (decimal)15.157432;
                decimal iRecEsq4 = xRecEsq4 - (decimal)3.030303;
                decimal jRecEsq4 = jRecEsq3 - 4;
                decimal raioRecEsq4 = (decimal)12.5;

                decimal xRecEsq5 = xRecEsq4 - (decimal)3.030303;
                decimal yRecEsq5 = yRecEsq4 - (decimal)3.880682;
                decimal iRecEsq5 = iRecEsq4 + 4;
                decimal jRecEsq5 = jRecEsq4 - (decimal)16.007811;
                decimal raioRecEsq5 = 4;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte da direita
                    .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                    .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                    .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                    .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                    .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                    .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                    .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                    .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                    .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecDir3, 6))

                    .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                    .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6))
                    .Replace("?IRecDir4?", Formatacoes.TrataValorDecimal(iRecDir4, 6))
                    .Replace("?JRecDir4?", Formatacoes.TrataValorDecimal(jRecDir4, 6))
                    .Replace("?RaioRecDir4?", Formatacoes.TrataValorDecimal(raioRecDir4, 6))

                    .Replace("?XRecDir5?", Formatacoes.TrataValorDecimal(xRecDir5, 6))
                    .Replace("?YRecDir5?", Formatacoes.TrataValorDecimal(yRecDir5, 6))
                    .Replace("?IRecDir5?", Formatacoes.TrataValorDecimal(iRecDir5, 6))
                    .Replace("?JRecDir5?", Formatacoes.TrataValorDecimal(jRecDir5, 6))
                    .Replace("?RaioRecDir5?", Formatacoes.TrataValorDecimal(raioRecDir5, 6))

                    // Faz o recorte da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                    .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                    .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                    .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                    .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecEsq3, 6))

                    .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                    .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))
                    .Replace("?IRecEsq4?", Formatacoes.TrataValorDecimal(iRecEsq4, 6))
                    .Replace("?JRecEsq4?", Formatacoes.TrataValorDecimal(jRecEsq4, 6))
                    .Replace("?RaioRecEsq4?", Formatacoes.TrataValorDecimal(raioRecEsq4, 6))

                    .Replace("?XRecEsq5?", Formatacoes.TrataValorDecimal(xRecEsq5, 6))
                    .Replace("?YRecEsq5?", Formatacoes.TrataValorDecimal(yRecEsq5, 6))
                    .Replace("?IRecEsq5?", Formatacoes.TrataValorDecimal(iRecEsq5, 6))
                    .Replace("?JRecEsq5?", Formatacoes.TrataValorDecimal(jRecEsq5, 6))
                    .Replace("?RaioRecEsq5?", Formatacoes.TrataValorDecimal(raioRecEsq5, 6));

                #endregion
            }
            else if (codigoArquivo == "SUPCOM1203E1402CENTROCONJDUASPORTAS")
            {
                #region SUPCOM1203E1402CENTROCONJDUASPORTAS

                // Calcula o recorte da direita
                decimal xRecDir1 = largura - (decimal)(15 + descontoLap);
                decimal yRecDir1 = (decimal)descontoLap + 25;

                decimal xRecDir2 = xRecDir1 - (decimal)78.992189;
                decimal yRecDir2 = yRecDir1;

                decimal xRecDir3 = xRecDir2 - (decimal)3.880682;
                decimal yRecDir3 = yRecDir2 + (decimal)3.030303;
                decimal iRecDir3 = xRecDir3 + (decimal)3.880682;
                decimal jRecDir3 = yRecDir2 + 4;
                decimal raioRecDir3 = 4;

                decimal xRecDir4 = xRecDir3 - (decimal)15.157432;
                decimal yRecDir4 = yRecDir3 - (decimal)15.157432;
                decimal iRecDir4 = xRecDir4 + (decimal)3.030303;
                decimal jRecDir4 = jRecDir3 - 4;
                decimal raioRecDir4 = (decimal)12.5;

                decimal xRecDir5 = xRecDir4 + (decimal)3.030303;
                decimal yRecDir5 = yRecDir4 - (decimal)3.880682;
                decimal iRecDir5 = iRecDir4 - 4;
                decimal jRecDir5 = jRecDir4 - (decimal)16.007811;
                decimal raioRecDir5 = 4;

                // Calcula o recorte da esquerda
                decimal xRecEsq1 = (decimal)descontoLap + 15;
                decimal yRecEsq1 = (decimal)descontoLap + 25;

                decimal xRecEsq2 = xRecEsq1 + (decimal)78.992189;
                decimal yRecEsq2 = yRecEsq1;

                decimal xRecEsq3 = xRecEsq2 + (decimal)3.880682;
                decimal yRecEsq3 = yRecEsq2 + (decimal)3.030303;
                decimal iRecEsq3 = xRecEsq2;
                decimal jRecEsq3 = yRecEsq2 + 4;
                decimal raioRecEsq3 = 4;

                decimal xRecEsq4 = xRecEsq3 + (decimal)15.157432;
                decimal yRecEsq4 = yRecEsq3 - (decimal)15.157432;
                decimal iRecEsq4 = xRecEsq4 - (decimal)3.030303;
                decimal jRecEsq4 = jRecEsq3 - 4;
                decimal raioRecEsq4 = (decimal)12.5;

                decimal xRecEsq5 = xRecEsq4 - (decimal)3.030303;
                decimal yRecEsq5 = yRecEsq4 - (decimal)3.880682;
                decimal iRecEsq5 = iRecEsq4 + 4;
                decimal jRecEsq5 = jRecEsq4 - (decimal)16.007811;
                decimal raioRecEsq5 = 4;

                // Calcula a 1402
                decimal xRecCentral1 = (decimal)((largura + aumentoPeca) / 2) + (decimal)15.399063;
                decimal yRecCentral1 = (decimal)descontoLap + 10;

                decimal xRecCentral2 = xRecCentral1 - (decimal)3.166125;
                decimal yRecCentral2 = yRecCentral1 + (decimal)15.070063;

                decimal xRecCentral3 = xRecCentral2 - (decimal)24.465876;
                decimal yRecCentral3 = yRecCentral2;
                decimal iRecCentral3 = (decimal)((largura + aumentoPeca) / 2);
                decimal jRecCentral3 = yRecCentral3 - (decimal)2.570063;
                decimal raioRecCentral3 = (decimal)12.5;

                decimal xRecCentral4 = xRecCentral3 - (decimal)3.166125;
                decimal yRecCentral4 = yRecCentral3 - (decimal)15.070063;

                conteudoArquivo = conteudoArquivo
                    // Faz o recorte da direita
                    .Replace("?XRecDir1?", Formatacoes.TrataValorDecimal(xRecDir1, 6))
                    .Replace("?YRecDir1?", Formatacoes.TrataValorDecimal(yRecDir1, 6))

                    .Replace("?XRecDir2?", Formatacoes.TrataValorDecimal(xRecDir2, 6))
                    .Replace("?YRecDir2?", Formatacoes.TrataValorDecimal(yRecDir2, 6))

                    .Replace("?XRecDir3?", Formatacoes.TrataValorDecimal(xRecDir3, 6))
                    .Replace("?YRecDir3?", Formatacoes.TrataValorDecimal(yRecDir3, 6))
                    .Replace("?IRecDir3?", Formatacoes.TrataValorDecimal(iRecDir3, 6))
                    .Replace("?JRecDir3?", Formatacoes.TrataValorDecimal(jRecDir3, 6))
                    .Replace("?RaioRecDir3?", Formatacoes.TrataValorDecimal(raioRecDir3, 6))

                    .Replace("?XRecDir4?", Formatacoes.TrataValorDecimal(xRecDir4, 6))
                    .Replace("?YRecDir4?", Formatacoes.TrataValorDecimal(yRecDir4, 6))
                    .Replace("?IRecDir4?", Formatacoes.TrataValorDecimal(iRecDir4, 6))
                    .Replace("?JRecDir4?", Formatacoes.TrataValorDecimal(jRecDir4, 6))
                    .Replace("?RaioRecDir4?", Formatacoes.TrataValorDecimal(raioRecDir4, 6))

                    .Replace("?XRecDir5?", Formatacoes.TrataValorDecimal(xRecDir5, 6))
                    .Replace("?YRecDir5?", Formatacoes.TrataValorDecimal(yRecDir5, 6))
                    .Replace("?IRecDir5?", Formatacoes.TrataValorDecimal(iRecDir5, 6))
                    .Replace("?JRecDir5?", Formatacoes.TrataValorDecimal(jRecDir5, 6))
                    .Replace("?RaioRecDir5?", Formatacoes.TrataValorDecimal(raioRecDir5, 6))

                    // Faz o recorte da esquerda
                    .Replace("?XRecEsq1?", Formatacoes.TrataValorDecimal(xRecEsq1, 6))
                    .Replace("?YRecEsq1?", Formatacoes.TrataValorDecimal(yRecEsq1, 6))

                    .Replace("?XRecEsq2?", Formatacoes.TrataValorDecimal(xRecEsq2, 6))
                    .Replace("?YRecEsq2?", Formatacoes.TrataValorDecimal(yRecEsq2, 6))

                    .Replace("?XRecEsq3?", Formatacoes.TrataValorDecimal(xRecEsq3, 6))
                    .Replace("?YRecEsq3?", Formatacoes.TrataValorDecimal(yRecEsq3, 6))
                    .Replace("?IRecEsq3?", Formatacoes.TrataValorDecimal(iRecEsq3, 6))
                    .Replace("?JRecEsq3?", Formatacoes.TrataValorDecimal(jRecEsq3, 6))
                    .Replace("?RaioRecEsq3?", Formatacoes.TrataValorDecimal(raioRecEsq3, 6))

                    .Replace("?XRecEsq4?", Formatacoes.TrataValorDecimal(xRecEsq4, 6))
                    .Replace("?YRecEsq4?", Formatacoes.TrataValorDecimal(yRecEsq4, 6))
                    .Replace("?IRecEsq4?", Formatacoes.TrataValorDecimal(iRecEsq4, 6))
                    .Replace("?JRecEsq4?", Formatacoes.TrataValorDecimal(jRecEsq4, 6))
                    .Replace("?RaioRecEsq4?", Formatacoes.TrataValorDecimal(raioRecEsq4, 6))

                    .Replace("?XRecEsq5?", Formatacoes.TrataValorDecimal(xRecEsq5, 6))
                    .Replace("?YRecEsq5?", Formatacoes.TrataValorDecimal(yRecEsq5, 6))
                    .Replace("?IRecEsq5?", Formatacoes.TrataValorDecimal(iRecEsq5, 6))
                    .Replace("?JRecEsq5?", Formatacoes.TrataValorDecimal(jRecEsq5, 6))
                    .Replace("?RaioRecEsq5?", Formatacoes.TrataValorDecimal(raioRecEsq5, 6))

                    // Faz o recorte central da 1402
                    .Replace("?XRecCentral1?", Formatacoes.TrataValorDecimal(xRecCentral1, 6))
                    .Replace("?YRecCentral1?", Formatacoes.TrataValorDecimal(yRecCentral1, 6))

                    .Replace("?XRecCentral2?", Formatacoes.TrataValorDecimal(xRecCentral2, 6))
                    .Replace("?YRecCentral2?", Formatacoes.TrataValorDecimal(yRecCentral2, 6))

                    .Replace("?XRecCentral3?", Formatacoes.TrataValorDecimal(xRecCentral3, 6))
                    .Replace("?YRecCentral3?", Formatacoes.TrataValorDecimal(yRecCentral3, 6))
                    .Replace("?IRecCentral3?", Formatacoes.TrataValorDecimal(iRecCentral3, 6))
                    .Replace("?JRecCentral3?", Formatacoes.TrataValorDecimal(jRecCentral3, 6))
                    .Replace("?RaioRecCentral3?", Formatacoes.TrataValorDecimal(raioRecCentral3, 6))

                    .Replace("?XRecCentral4?", Formatacoes.TrataValorDecimal(xRecCentral4, 6))
                    .Replace("?YRecCentral4?", Formatacoes.TrataValorDecimal(yRecCentral4, 6));

                #endregion
            }
            else
                return false;

            return true;

            #endregion
        }

        #endregion

        #region FOR TXT

        /// <summary>
        /// Monta o arquivo de marcação do tipo FOR TXT, com base nos arquivos fixos no código.
        /// </summary>
        private static bool GerarArquivoFORTxt(float altura, string codigoArquivo, ConfiguracoesArqMesa config, ref string conteudoArquivo, float espessura, float largura, string mensagemErro,
            PecaItemProjeto pecaItemProjeto)
        {
            #region Arquivos FOR txt

            conteudoArquivo = conteudoArquivo
                .Replace("?Altura?", altura.ToString())
                .Replace("?Largura?", largura.ToString())
                .Replace("?Espessura?", espessura.ToString());

            if (codigoArquivo == "FOR_CRPUX")
            {
                #region FOR_CRPUX

                int alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                int distBordaFuro = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                int espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("O campo altura do puxador não foi informado." + mensagemErro);

                int diamentroFuroRoldana = config.Roldana.RaioRoldana * 2;
                int diametroFuroPux = espFuroPux > 0 ? espFuroPux : 10;
                distBordaFuro = distBordaFuro > 0 ? distBordaFuro : 50; // Distância da direita para a esquerda do furo do puxador

                float xRoldana1 = config.Roldana.PosXRoldana;
                float yRoldana1 = config.Roldana.PosYRoldana;

                float xRoldana2 = config.Roldana.PosXRoldana * -1;
                float yRoldana2 = config.Roldana.PosYRoldana;

                float xFuroPux = distBordaFuro;
                float yFuroPux = alturaPuxador * -1;

                conteudoArquivo = conteudoArquivo
                    // Faz os furos das roldanas
                    .Replace("?XRoldana1?", xRoldana1.ToString())
                    .Replace("?YRoldana1?", yRoldana1.ToString())
                    .Replace("?XRoldana2?", xRoldana2.ToString())
                    .Replace("?YRoldana2?", yRoldana2.ToString())

                    // Faz o furo do puxador
                    .Replace("?XFuroPux?", xFuroPux.ToString())
                    .Replace("?YFuroPux?", yFuroPux.ToString())

                    // Preenche os diâmetros do furo do puxador e do furo da roldana
                    .Replace("?DiametroFuroPux?", diametroFuroPux.ToString())
                    .Replace("?DiametroFuroRoldana?", diamentroFuroRoldana.ToString());

                #endregion
            }
            else
                return false;

            return true;

            #endregion
        }

        #endregion

        #region DXF

        /// <summary>
        /// Monta o arquivo de marcação do tipo DXF, com base nos arquivos fixos no código.
        /// </summary>
        private bool GerarArquivoDXF(float altura, Stream arquivo, string codigoArquivo, float espessura, List<FlagArqMesa> flags, uint idProdPedEsp, float largura, string mensagemErro,
            PecaItemProjeto pecaItemProjeto, CalcEngine.Dxf.DxfProject projeto, TipoArquivoMesaCorte tipoArquivo, ref Dictionary<string, double> variaveis)
        {
            #region Arquivos DXF

            if (codigoArquivo == "DXF_BASCULA")
            {
                #region DXF_BASCULA

                var distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                var altura1123Bascula = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1123Bascula);

                // Se a medida altura1123Bascula existir no projeto, força o usuário a informá-la
                var idProjetoModelo = ItemProjetoDAO.Instance.GetIdProjetoModelo(pecaItemProjeto.IdItemProjeto);
                if (altura1123Bascula == 0 && MedidaProjetoModeloDAO.Instance.ExisteMedidaProjeto(idProjetoModelo, "Posição da 1123 da bascula"))
                    throw new Exception("A posição da 1123 deve ser informada." + mensagemErro);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                if (altura1123Bascula == 0)
                    altura1123Bascula = (int)(altura / 2) + 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("Altura1123", altura1123Bascula);
                variaveis.Add("DistBorda1230", distBorda1523);

                #endregion
            }
            else if (codigoArquivo == "DXF_BOXABRIR_ESQ" || codigoArquivo == "DXF_BOXABRIR_DIR")
            {
                #region DXF_BOXABRIR

                var alturaDobradica = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaDobradica);

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada." + mensagemErro);

                distBordaFuroPuxador = distBordaFuroPuxador > 0 ? distBordaFuroPuxador : 50;
                var raioFuroPuxador = espFuroPuxador > 0 ? (float)espFuroPuxador / 2f : 6f;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaDobradica", alturaDobradica);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("RaioFuroPuxador", raioFuroPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPuxador);

                #endregion
            }
            else if (codigoArquivo == "DXF_BOXELEGANCE_ESQ" || codigoArquivo == "DXF_BOXELEGANCE_DIR")
            {
                #region DXF_BOXELEGANCE

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada." + mensagemErro);

                distBordaFuroPuxador = distBordaFuroPuxador > 0 ? distBordaFuroPuxador : 50;
                var raioFuroPuxador = espFuroPuxador > 0 ? (float)espFuroPuxador / 2f : 6f;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("RaioFuroPuxador", raioFuroPuxador);
                variaveis.Add("DistBordaPuxador", distBordaFuroPuxador);

                #endregion
            }
            else if (codigoArquivo == "DXF_CAR1122BATEFECHA_ESQ" || codigoArquivo == "DXF_CAR1122BATEFECHA_DIR" ||
                codigoArquivo == "DXF_CAR1122BATEFECHATRI_ESQ" || codigoArquivo == "DXF_CAR1122BATEFECHATRI_DIR")
            {
                #region DXF_CAR1122BATEFECHA E DXF_CAR1122BATEFECHATRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                if (espFuroFech == 0)
                    throw new Exception("A espessura do furo da fechadura não foi informada. " + mensagemErro);

                if (distBordaFuroFech == 0)
                    throw new Exception("A distância da borda da fechadura não foi informada. " + mensagemErro);

                if (distEntreFurosFech == 0)
                    throw new Exception("A distância entre os furos da fechadura não foi informada. " + mensagemErro);

                var raioFuroFech = espFuroFech > 0 ? (float)espFuroFech / 2F : 8F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);
                variaveis.Add("DistEntreFurosFechadura", distEntreFurosFech);
                variaveis.Add("DistBordaFechadura", distBordaFuroFech);
                variaveis.Add("RaioFuroFechadura", raioFuroFech);

                #endregion
            }
            else if (codigoArquivo == "DXF_CAR1122MAOAMIGA3530_ESQ" || codigoArquivo == "DXF_CAR1122MAOAMIGA3530_DIR")
            {
                #region DXF_CAR1122MAOAMIGA3530

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var alturaMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaMaoAmiga);

                var espFuroMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroMaoAmiga);

                var distBordaMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaMaoAmiga);

                var distEixoMaoAMiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoMaoAmiga);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                if (alturaMaoAmiga == 0)
                    throw new Exception("A altura da ferragem mão amiga não foi informada. " + mensagemErro);

                if (espFuroMaoAmiga == 0)
                    throw new Exception("A espessura do furo da ferragem mão amiga não foi informada. " + mensagemErro);

                if (distBordaMaoAmiga == 0)
                    throw new Exception("A distância da borda da ferragem mão amiga não foi informada. " + mensagemErro);

                if (distEixoMaoAMiga == 0)
                    throw new Exception("A distância entre os furos da ferragem mão amiga não foi informada. " + mensagemErro);

                // DEFINIR PADRÕES
                var raioFuroMaoAmiga = espFuroMaoAmiga > 0 ? (float)espFuroMaoAmiga / 2F : 7F;
                distBordaMaoAmiga = distBordaMaoAmiga > 0 ? distBordaMaoAmiga : 18;
                distEixoMaoAMiga = distEixoMaoAMiga > 0 ? distBordaMaoAmiga : 80;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);
                variaveis.Add("AlturaMaoAmiga", alturaMaoAmiga);
                variaveis.Add("DistBordaMaoAmiga", distBordaMaoAmiga);
                variaveis.Add("EspessuraMaoAmiga", raioFuroMaoAmiga);
                variaveis.Add("DistEntreFurosMaoAmiga", distEixoMaoAMiga);

                #endregion
            }
            else if (codigoArquivo == "DXF_CAR1122MAOAMIGAESQDIR")
            {
                #region DXF_CAR1122MAOAMIGAESQDIR

                var alturaMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaMaoAmiga);

                var espFuroMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroMaoAmiga);

                var distBordaMaoAmiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaMaoAmiga);

                var distEixoMaoAMiga = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoMaoAmiga);

                if (alturaMaoAmiga == 0)
                    throw new Exception("A altura da ferragem mão amiga não foi informada. " + mensagemErro);

                if (espFuroMaoAmiga == 0)
                    throw new Exception("A espessura do furo da ferragem mão amiga não foi informada. " + mensagemErro);

                if (distBordaMaoAmiga == 0)
                    throw new Exception("A distância da borda da ferragem mão amiga não foi informada. " + mensagemErro);

                if (distEixoMaoAMiga == 0)
                    throw new Exception("A distância entre os furos da ferragem mão amiga não foi informada. " + mensagemErro);

                // DEFINIR PADRÕES
                var raioFuroMaoAmiga = espFuroMaoAmiga > 0 ? (float)espFuroMaoAmiga / 2F : 7F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaMaoAmiga", alturaMaoAmiga);
                variaveis.Add("DistBordaMaoAmiga", distBordaMaoAmiga);
                variaveis.Add("EspessuraMaoAmiga", raioFuroMaoAmiga);
                variaveis.Add("DistEntreFurosMaoAmiga", distEixoMaoAMiga);

                #endregion
            }
            else if (codigoArquivo == "DXF_CAR1122MINISEMPUX")
            {
                #region DXF_CAR1122MINISEMPUX

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);

                #endregion
            }
            else if (codigoArquivo == "DXF_CAR1122PUXSIMPLES_ESQ" || codigoArquivo == "DXF_CAR1122PUXSIMPLES_DIR" ||
                codigoArquivo == "DXF_CAR1122PUXSIMPLESTRI_ESQ" || codigoArquivo == "DXF_CAR1122PUXSIMPLESTRI_DIR")
            {
                #region DXF_CAR1122PUXSIMPLES E DXF_CAR1122PUXSIMPLESTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do puxador não foi informada. " + mensagemErro);

                if (distBordaFuroPux == 0)
                    throw new Exception("A distância da borda do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 6F;
                distBordaFuroPux = distBordaFuroPux > 0 ? distBordaFuroPux : 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "DXF_CR3530_ESQ" || codigoArquivo == "DXF_CR3530_DIR" ||
                codigoArquivo == "DXF_CR3530TRI_ESQ" || codigoArquivo == "DXF_CR3530TRI_DIR")
            {
                #region DXF_CR3530 E DXF_CR3530TRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informado. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "DXF_CR3530PUXDUPL_ESQ" || codigoArquivo == "DXF_CR3530PUXDUPL_DIR" ||
                codigoArquivo == "DXF_CR3530PUXDUPLTRI_ESQ" || codigoArquivo == "DXF_CR3530PUXDUPLTRI_DIR")
            {
                #region DXF_CR3530PUXDUPL E DXF_CR3530PUXDUPLTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                if (espFuroPuxador == 0)
                    throw new Exception("A espessura da fechadura não foi informada. " + mensagemErro);

                if (distBordaFuroPuxador == 0)
                    throw new Exception("A distância da borda da fechadura não foi informada. " + mensagemErro);

                if (distEntreFurosPuxador == 0)
                    throw new Exception("A distância entre os furos da fechadura não foi informada. " + mensagemErro);

                var raioFuroPuxador = espFuroPuxador > 0 ? (float)espFuroPuxador / 2F : 6F;
                distBordaFuroPuxador = distBordaFuroPuxador > 0 ? distBordaFuroPuxador : 100;
                distEntreFurosPuxador = distEntreFurosPuxador > 0 ? distEntreFurosPuxador : 300;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPuxador);
                variaveis.Add("DistEntreFurosPuxador", distEntreFurosPuxador);
                variaveis.Add("RaioFuroPuxador", raioFuroPuxador);

                #endregion
            }
            else if (codigoArquivo == "DXF_CR3530PUXSIMPLES_ESQ" || codigoArquivo == "DXF_CR3530PUXSIMPLES_DIR" ||
                codigoArquivo == "DXF_CR3530PUXSIMPLESTRI_ESQ" || codigoArquivo == "DXF_CR3530PUXSIMPLESTRI_DIR")
            {
                #region DXF_CR3530PUXSIMPLES E DXF_CR3530PUXSIMPLESTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do puxador não foi informada. " + mensagemErro);

                if (distBordaFuroPux == 0)
                    throw new Exception("A distância da borda do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 6F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "DXF_CR3532_ESQ" || codigoArquivo == "DXF_CR3532MTRI_ESQ" ||
                codigoArquivo == "DXF_CR3532_DIR" || codigoArquivo == "DXF_CR3532MTRI_DIR")
            {
                #region DXF_CR3532 E DXF_CR3532MTRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                if (espFuroFech == 0)
                    throw new Exception("A espessura da fechadura não foi informada. " + mensagemErro);

                if (distBordaFuroFech == 0)
                    throw new Exception("A distância da borda da fechadura não foi informada. " + mensagemErro);

                if (distEntreFurosFech == 0)
                    throw new Exception("A distância entre os furos da fechadura não foi informada. " + mensagemErro);

                var raioFuroFech = espFuroFech > 0 ? (float)espFuroFech / 2F : 8F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);
                variaveis.Add("DistBordaFechadura", distBordaFuroFech);
                variaveis.Add("DistEntreFurosFechadura", distEntreFurosFech);
                variaveis.Add("RaioFuroFechadura", raioFuroFech);

                #endregion
            }
            else if (codigoArquivo == "DXF_CRPUX_ESQ" || codigoArquivo == "DXF_CRPUX_DIR" ||
                codigoArquivo == "DXF_CRPUXTRI_ESQ" || codigoArquivo == "DXF_CRPUXTRI_DIR")
            {
                #region DXF_CRPUX E DXF_CRPUXTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do puxador não foi informada. " + mensagemErro);

                if (distBordaFuroPux == 0)
                    throw new Exception("A distância da borda do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 7F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "DXF_CRPUXTRI_ESQ" || codigoArquivo == "DXF_CRPUXTRI_DIR")
            {
                #region DXF_CRPUXTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                if (espFuroPux == 0)
                    throw new Exception("A espessura do puxador não foi informada. " + mensagemErro);

                if (distBordaFuroPux == 0)
                    throw new Exception("A distância do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 7F;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "DXF_CRROLDANADUPLA3530TRI_ESQ" || codigoArquivo == "DXF_CRROLDANADUPLA3530TRI_DIR")
            {
                #region DXF_CRROLDANADUPLA3530TRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informado. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "DXF_CRTRINCO_ESQ" || codigoArquivo == "DXF_CRTRINCO_DIR")
            {
                #region DXF_CRTRINCO

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);

                #endregion
            }
            else if (codigoArquivo == "DXF_FIXO04FUROS")
            {
                #region DXF_FIXO04FUROS

                var espFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroInferior);

                var alturaFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroInferior);

                var larguraFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroInferior);

                var espFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroSuperior);

                var alturaFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroSuperior);

                var larguraFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroSuperior);

                if (espFuroInferior == 0 && espFuroSuperior == 0)
                {
                    espFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    espFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);
                }

                if (espFuroInferior == 0)
                    throw new Exception("A espessura do furo inferior não foi informada. " + mensagemErro);

                if (alturaFuroInferior == 0)
                    throw new Exception("A altura do furo inferior não foi informada. " + mensagemErro);

                if (larguraFuroInferior == 0)
                    throw new Exception("A largura do furo inferior não foi informada. " + mensagemErro);

                if (espFuroSuperior == 0)
                    throw new Exception("A espessura do furo superior não foi informada. " + mensagemErro);

                if (alturaFuroSuperior == 0)
                    throw new Exception("A altura do furo superior não foi informada. " + mensagemErro);

                if (larguraFuroSuperior == 0)
                    throw new Exception("A largura do furo superior não foi informada. " + mensagemErro);

                var raioFuroInferior = (float)espFuroInferior / 2f;
                var raioFuroSuperior = (float)espFuroSuperior / 2f;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AltFuroEsqSup", alturaFuroSuperior);
                variaveis.Add("AltFuroDirSup", alturaFuroSuperior);
                variaveis.Add("AltFuroEsqInf", alturaFuroInferior);
                variaveis.Add("AltFuroDirInf", alturaFuroInferior);
                variaveis.Add("LargFuroEsqSup", larguraFuroSuperior);
                variaveis.Add("LargFuroDirSup", larguraFuroSuperior);
                variaveis.Add("LargFuroEsqInf", larguraFuroInferior);
                variaveis.Add("LargFuroDirInf", larguraFuroInferior);
                variaveis.Add("RaioFuroEsqSup", raioFuroSuperior);
                variaveis.Add("RaioFuroDirSup", raioFuroSuperior);
                variaveis.Add("RaioFuroEsqInf", raioFuroInferior);
                variaveis.Add("RaioFuroDirInf", raioFuroInferior);

                #endregion
            }
            else if (codigoArquivo == "DXF_FIXOCONTRA1520VV_ESQ" || codigoArquivo == "DXF_FIXOCONTRA1520VV_DIR")
            {
                #region DXF_FIXOCONTRA1520VV

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "DXF_PTABRIR1520TASEMPUXTA_ESQ" || codigoArquivo == "DXF_PTABRIR1520TASEMPUXTA_DIR")
            {
                #region DXF_PTABRIR1520TASEMPUXTA

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else
                return false;

            // Salva a marcação da peça conforme o tipo do arquivo.
            return SalvarArquivoCalcEngine(idProdPedEsp, espessura, projeto, variaveis, arquivo, (int)tipoArquivo, 0F, flags, false, false, false);

            #endregion
        }

        #endregion

        #region FML

        /// <summary>
        /// Monta o arquivo de marcação do tipo FML, com base nos arquivos fixos no código.
        /// </summary>
        private bool GerarArquivoFML(float altura, Stream arquivo, string codigoArquivo, float espessura, List<FlagArqMesa> flags, uint idProdPedEsp, float largura, string mensagemErro, PecaItemProjeto pecaItemProjeto,
            CalcEngine.Dxf.DxfProject projeto, TipoArquivoMesaCorte tipoArquivo, ref Dictionary<string, double> variaveis)
        {
            #region Arquivos FML

            if (tipoArquivo == TipoArquivoMesaCorte.FMLBasico)
            {
                #region FML_ARQUIVOBASICO

                if (Configuracoes.ProjetoConfig.FMLBasicoSalvarMaiorMedidaNoCampoAltura)
                {
                    // dimX.
                    variaveis.Add("Altura", altura > largura ? altura : largura);
                    // dimY.
                    variaveis.Add("Largura", altura > largura ? largura : altura);
                }
                else
                {
                    variaveis.Add("Altura", altura);
                    variaveis.Add("Largura", largura);
                }

                variaveis.Add("Espessura", espessura);

                #endregion
            }
            else if (codigoArquivo == "FML_BASCULA")
            {
                #region FML_BASCULA

                var distBorda1523 = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBorda1523);

                var altura1123Bascula = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.Altura1123Bascula);

                // Se a medida altura1123Bascula existir no projeto, força o usuário a informá-la
                var idProjetoModelo = ItemProjetoDAO.Instance.GetIdProjetoModelo(pecaItemProjeto.IdItemProjeto);
                if (altura1123Bascula == 0 && MedidaProjetoModeloDAO.Instance.ExisteMedidaProjeto(idProjetoModelo, "Posição da 1123 da bascula"))
                    throw new Exception("A posição da 1123 deve ser informada." + mensagemErro);

                if (distBorda1523 == 0)
                    distBorda1523 = altura > 300 ? 50 : 100;

                if (altura1123Bascula == 0)
                    altura1123Bascula = (int)(altura / 2) + 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("Altura1123", altura1123Bascula);
                variaveis.Add("DistBorda1230", distBorda1523);

                #endregion
            }
            else if (codigoArquivo == "FML_BOXABRIR_ESQ" || codigoArquivo == "FML_BOXABRIR_DIR")
            {
                #region FML_BOXABRIR

                var alturaDobradica = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaDobradica);

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada." + mensagemErro);

                distBordaFuroPuxador = distBordaFuroPuxador > 0 ? distBordaFuroPuxador : 50;
                var raioFuroPuxador = espFuroPuxador > 0 ? (float)espFuroPuxador / 2f : 6f;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaDobradica", alturaDobradica);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("RaioFuroPuxador", raioFuroPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPuxador);

                #endregion
            }
            else if (codigoArquivo == "FML_CAR1122BATEFECHA_ESQ" || codigoArquivo == "FML_CAR1122BATEFECHA_DIR" ||
                codigoArquivo == "FML_CAR1122BATEFECHATRI_ESQ" || codigoArquivo == "FML_CAR1122BATEFECHATRI_DIR")
            {
                #region FML_CAR1122BATEFECHA E FML_CAR1122BATEFECHATRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                var raioFuroFech = espFuroFech > 0 ? (float)espFuroFech / 2F : 8F;
                distBordaFuroFech = distBordaFuroFech > 0 ? distBordaFuroFech : 27;
                distEntreFurosFech = distEntreFurosFech > 0 ? distEntreFurosFech : 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);
                variaveis.Add("DistEntreFurosFechadura", distEntreFurosFech);
                variaveis.Add("DistBordaFechadura", distBordaFuroFech);
                variaveis.Add("RaioFuroFechadura", raioFuroFech);

                #endregion
            }
            else if (codigoArquivo == "FML_CAR1122PUXSIMPLES_ESQ" || codigoArquivo == "FML_CAR1122PUXSIMPLES_DIR" ||
                codigoArquivo == "FML_CAR1122PUXSIMPLESTRI_ESQ" || codigoArquivo == "FML_CAR1122PUXSIMPLESTRI_DIR")
            {
                #region FML_CAR1122PUXSIMPLES E FML_CAR1122PUXSIMPLESTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 6F;
                distBordaFuroPux = distBordaFuroPux > 0 ? distBordaFuroPux : 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "FML_CR3530_ESQ" || codigoArquivo == "FML_CR3530_DIR" ||
                codigoArquivo == "FML_CR3530TRI_ESQ" || codigoArquivo == "FML_CR3530TRI_DIR")
            {
                #region FML_CR3530 E FML_CR3530TRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informado. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "FML_CR3530PUXDUPL_ESQ" || codigoArquivo == "FML_CR3530PUXDUPL_DIR" ||
                codigoArquivo == "FML_CR3530PUXDUPLTRI_ESQ" || codigoArquivo == "FML_CR3530PUXDUPLTRI_DIR")
            {
                #region FML_CR3530PUXDUPL E FML_CR3530PUXDUPLTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaPuxador == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                var raioFuroPuxador = espFuroPuxador > 0 ? (float)espFuroPuxador / 2F : 6F;
                distBordaFuroPuxador = distBordaFuroPuxador > 0 ? distBordaFuroPuxador : 100;
                distEntreFurosPuxador = distEntreFurosPuxador > 0 ? distEntreFurosPuxador : 300;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPuxador);
                variaveis.Add("DistEntreFurosPuxador", distEntreFurosPuxador);
                variaveis.Add("RaioFuroPuxador", raioFuroPuxador);

                #endregion
            }
            else if (codigoArquivo == "FML_CR3530PUXSIMPLES_ESQ" || codigoArquivo == "FML_CR3530PUXSIMPLES_DIR" ||
                codigoArquivo == "FML_CR3530PUXSIMPLESTRI_ESQ" || codigoArquivo == "FML_CR3530PUXSIMPLESTRI_DIR")
            {
                #region FML_CR3530PUXSIMPLES E FML_CR3530PUXSIMPLESTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 6F;
                distBordaFuroPux = distBordaFuroPux > 0 ? distBordaFuroPux : 50; // Distância da direita para a esquerda do furo do puxador

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "FML_CR3532_ESQ" || codigoArquivo == "FML_CR3532MTRI_ESQ" ||
                codigoArquivo == "FML_CR3532_DIR" || codigoArquivo == "FML_CR3532MTRI_DIR")
            {
                #region FML_CR3532 E FML_CR3532MTRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                var distEntreFurosFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistEixoFuroPux);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                var raioFuroFech = espFuroFech > 0 ? (float)espFuroFech / 2F : 8F;
                distBordaFuroFech = distBordaFuroFech > 0 ? distBordaFuroFech : 27; // Distância da direita para a esquerda do furo do puxador
                distEntreFurosFech = distEntreFurosFech > 0 ? distEntreFurosFech : 50;

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);
                variaveis.Add("DistBordaFechadura", distBordaFuroFech);
                variaveis.Add("DistEntreFurosFechadura", distEntreFurosFech);
                variaveis.Add("RaioFuroFechadura", raioFuroFech);

                #endregion
            }
            else if (codigoArquivo == "FML_CRPUX_ESQ" || codigoArquivo == "FML_CRPUX_DIR" ||
                codigoArquivo == "FML_CRPUXTRI_ESQ" || codigoArquivo == "FML_CRPUXTRI_DIR")
            {
                #region FML_CRPUX E FML_CRPUXTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 7F;
                distBordaFuroPux = distBordaFuroPux > 0 ? distBordaFuroPux : 50; // Distância da direita para a esquerda do furo do puxador

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "FML_CRPUXTRI_ESQ" || codigoArquivo == "FML_CRPUXTRI_DIR")
            {
                #region FML_CRPUXTRI

                var alturaPuxador = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                var espFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                var distBordaFuroPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.DistBordaFuro);

                if (alturaPuxador == 0)
                    throw new Exception("A altura do puxador não foi informada. " + mensagemErro);

                var raioFuroPux = espFuroPux > 0 ? (float)espFuroPux / 2F : 7F;
                distBordaFuroPux = distBordaFuroPux > 0 ? distBordaFuroPux : 50; // Distância da direita para a esquerda do furo do puxador

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaPuxador", alturaPuxador);
                variaveis.Add("DistBordaFuroPuxador", distBordaFuroPux);
                variaveis.Add("RaioFuroPuxador", raioFuroPux);

                #endregion
            }
            else if (codigoArquivo == "FML_CRROLDANADUPLA3530TRI_ESQ" || codigoArquivo == "FML_CRROLDANADUPLA3530TRI_DIR")
            {
                #region FML_CRROLDANADUPLA3530TRI

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informado. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "FML_CRTRINCO_ESQ" || codigoArquivo == "FML_CRTRINCO_DIR")
            {
                #region FML_CRTRINCO

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);

                #endregion
            }
            else if (codigoArquivo == "FML_FIXO04FUROS")
            {
                #region FML_FIXO04FUROS

                var espFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.EspFuroInferior);

                var alturaFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroInferior);

                var larguraFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroInferior);

                var espFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroSuperior);

                var alturaFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaFuroSuperior);

                var larguraFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.LarguraFuroSuperior);

                if (espFuroInferior == 0 && espFuroSuperior == 0)
                {
                    espFuroInferior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);

                    espFuroSuperior = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                        (uint)GrupoMedidaProjeto.TipoMedida.EspFuroPux);
                }

                if (espFuroInferior == 0)
                    throw new Exception("A espessura do furo inferior não foi informada. " + mensagemErro);

                if (alturaFuroInferior == 0)
                    throw new Exception("A altura do furo inferior não foi informada. " + mensagemErro);

                if (larguraFuroInferior == 0)
                    throw new Exception("A largura do furo inferior não foi informada. " + mensagemErro);

                if (espFuroSuperior == 0)
                    throw new Exception("A espessura do furo superior não foi informada. " + mensagemErro);

                if (alturaFuroSuperior == 0)
                    throw new Exception("A altura do furo superior não foi informada. " + mensagemErro);

                if (larguraFuroSuperior == 0)
                    throw new Exception("A largura do furo superior não foi informada. " + mensagemErro);

                var raioFuroInferior = (float)espFuroInferior / 2f;
                var raioFuroSuperior = (float)espFuroSuperior / 2f;


                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AltFuroEsqSup", alturaFuroSuperior);
                variaveis.Add("AltFuroDirSup", alturaFuroSuperior);
                variaveis.Add("AltFuroEsqInf", alturaFuroInferior);
                variaveis.Add("AltFuroDirInf", alturaFuroInferior);
                variaveis.Add("LargFuroEsqSup", larguraFuroSuperior);
                variaveis.Add("LargFuroDirSup", larguraFuroSuperior);
                variaveis.Add("LargFuroEsqInf", larguraFuroInferior);
                variaveis.Add("LargFuroDirInf", larguraFuroInferior);
                variaveis.Add("RaioFuroEsqSup", raioFuroSuperior);
                variaveis.Add("RaioFuroDirSup", raioFuroSuperior);
                variaveis.Add("RaioFuroEsqInf", raioFuroInferior);
                variaveis.Add("RaioFuroDirInf", raioFuroInferior);

                #endregion
            }
            else if (codigoArquivo == "FML_FIXOCONTRA1520VV_ESQ" || codigoArquivo == "FML_FIXOCONTRA1520VV_DIR")
            {
                #region FML_FIXOCONTRA1520VV

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else if (codigoArquivo == "FML_PTABRIR1520TASEMPUX_ESQ" || codigoArquivo == "FML_PTABRIR1520TASEMPUX_DIR")
            {
                #region FML_PTABRIR1520SEMPUXTA

                var alturaFechadura = MedidaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, 0,
                    (uint)GrupoMedidaProjeto.TipoMedida.AlturaPuxador);

                if (alturaFechadura == 0)
                    throw new Exception("A altura da fechadura não foi informada. " + mensagemErro);

                // Você deve carregar as variáveis que o WebGlass vai fornecer
                variaveis.Add("Altura", altura);
                variaveis.Add("Largura", largura);
                variaveis.Add("AlturaFechadura", alturaFechadura);

                #endregion
            }
            else
                return false;

            // Salva a marcação da peça conforme o tipo do arquivo.
            return SalvarArquivoCalcEngine(idProdPedEsp, espessura, projeto, variaveis, arquivo, (int)tipoArquivo, 0F, flags, false, false, false);

            #endregion
        }

        #endregion

        #endregion

        #region Variáveis CalcEngine

        private static void PreencheVariaveisCalcEngine(GDASession session, int tipoArquivo, PecaItemProjeto pecaItemProjeto, float altura,
            int largura, string mensagemErro, string codigoArquivo, ConfiguracoesArqMesa config,
            Dictionary<string, double> variaveisCalcEngine, List<ArquivoCalcEngineVariavel> arquivoCeVar, float acrescimoSag)
        {
            foreach (var variavel in arquivoCeVar)
            {
                /* Chamado 24022. */
                if (variaveisCalcEngine.ContainsKey(variavel.VariavelCalcEngine) || variavel.VariavelSistema == null)
                    continue;

                #region Variáveis do sistema

                // As medidas de altura e largura da peça não existem no enumerador do grupo de medida de projeto,
                // sendo assim temos que recuperá-los antes da recuperação do id do grupo da medida de projeto.
                if (variavel.VariavelSistema.ToLower() == "altura")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, altura /*+ (acrescimoSag * 2)*/);
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "largura")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, largura /*+ (acrescimoSag * 2)*/);
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "alturabase")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, altura);
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "largurabase")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, largura);
                    continue;
                }
                // Estas medidas deve ser variáveis somente ao gerar o arquivo, pois, podem ser diferentes de empresa para empresa.
                else if (variavel.VariavelSistema.ToLower() == "dist. borda roldana")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine,
                        ((double)(variavel.ValorPadrao > 0 ? variavel.ValorPadrao : config.Roldana.PosXRoldana) + acrescimoSag));
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "esp. roldana")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, (double)(variavel.ValorPadrao > 0 ? variavel.ValorPadrao : config.Roldana.RaioRoldana));
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "altura trinco")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, ((double)(variavel.ValorPadrao > 0 ? variavel.ValorPadrao :
                        tipoArquivo == (int)TipoProjetoMesaCorte.Correr ? config.Trinco.DistBordaYTrincoCorrer :
                        tipoArquivo == (int)TipoProjetoMesaCorte.Porta ? config.Trinco.DistBordaYTrincoPtAbrir : 0)) +
                        acrescimoSag);
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "esp. trinco")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, (double)(variavel.ValorPadrao > 0 ? variavel.ValorPadrao : config.Trinco.RaioTrinco));
                    continue;
                }
                else if (variavel.VariavelSistema.ToLower() == "outros")
                {
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, (double)(variavel.ValorPadrao > 0 ? variavel.ValorPadrao : 0));
                    continue;
                }

                #endregion

                // Recupera o grupo de medida do projeto conforme o nome da variável do sistema.
                var idGrupoMedProj = GrupoMedidaProjetoDAO.Instance.ObtemValorCampo<uint?>(session, "idGrupoMedProj", "descricao=?descricao", new GDAParameter("?descricao", variavel.VariavelSistema));

                // Algumas empresas não tem ponto na descrição do grupo de medida, outras tem, caso o id não seja recuperado com a descrição contendo pontuação,
                // então temos que recuperar sem a pontuação, caso não seja recuperado então as variáveis foram definidas incorretamente no BD.
                idGrupoMedProj = idGrupoMedProj.GetValueOrDefault() == 0 ? GrupoMedidaProjetoDAO.Instance.ObtemValorCampo<uint?>(session, "idGrupoMedProj", "descricao=?descricao",
                    new GDAParameter("?descricao", variavel.VariavelSistema.Replace(".", ""))) : idGrupoMedProj;

                if (idGrupoMedProj.GetValueOrDefault() > 0)
                {
                    // Recupera o valor da medida no item projeto.
                    var medida = (double)MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, pecaItemProjeto.IdItemProjeto, 0, idGrupoMedProj.Value);

                    // Caso a medida não tenha sido informada e caso o valor padrão da mesma seja 0, lança uma exceção.
                    if (medida == 0)
                        throw new Exception("O campo " + variavel.VariavelSistema + " não foi informado." + mensagemErro);

                    /* Foi definido com o Reinaldo que não será necessário informar o acréscimo nas medidas,
                     * será feito futuramente conforme demanda, pois, provavelmente não será preciso fazer esta alteração.
                     * Caso seja necessário, nós iremos criar um campo na variável do calcengine que irá definir
                     * se o acréscimo deve ser considerado ou não. */
                    // Caso a medida esteja zerada salva o valor padrão da variável, definido no banco de dados.
                    //medida += !variavel.VariavelSistema.ToLower().Contains("esp") ? acrescimoSag : 0;

                    // Adiciona ao dicionário o nome da variável e seu respectivo valor.
                    variaveisCalcEngine.Add(variavel.VariavelCalcEngine, medida);
                }
                else
                    throw new Exception("O arquivo " + codigoArquivo + "não está cadastrado corretamente. Existem variáveis não associadas.");
            }
        }
        
        private static void PreencheVariaveisCompilador(PecaItemProjeto pecaItemProjeto, Dictionary<string, double> variaveisCalcEngine, float descontoLap, int tipoArquivo)
        {
            if (pecaItemProjeto != null && pecaItemProjeto.IdItemProjeto > 0)
            {
                var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(pecaItemProjeto.IdItemProjeto);
                var pecasItemProjeto = PecaItemProjetoDAO.Instance.GetByItemProjeto(pecaItemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                for (var i = 0; i < pecasItemProjeto.Count; i++)
                {
                    var pip = pecasItemProjeto[i];

                    variaveisCalcEngine.Add(string.Format("P{0}ALT", pip.Item.ToUpper().Replace(" ", "")), pip.Altura);
                    variaveisCalcEngine.Add(string.Format("P{0}LARG", pip.Item.ToUpper().Replace(" ", "")), pip.Largura);

                    var ppm = PecaProjetoModeloDAO.Instance.GetByCliente(null, pip.IdPecaProjMod, itemProjeto.IdCliente.GetValueOrDefault());

                    var folgaAltura =
                        itemProjeto.MedidaExata ?
                            0 : (Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Altura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Altura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Altura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Altura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Altura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Altura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Altura12MM : 0) : ppm.Altura);

                    var folgaLargura =
                        itemProjeto.MedidaExata ?
                            0 : (Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                (itemProjeto.EspessuraVidro == 3 ? ppm.Largura03MM :
                                itemProjeto.EspessuraVidro == 4 ? ppm.Largura04MM :
                                itemProjeto.EspessuraVidro == 5 ? ppm.Largura05MM :
                                itemProjeto.EspessuraVidro == 6 ? ppm.Largura06MM :
                                itemProjeto.EspessuraVidro == 8 ? ppm.Largura08MM :
                                itemProjeto.EspessuraVidro == 10 ? ppm.Largura10MM :
                                itemProjeto.EspessuraVidro == 12 ? ppm.Largura12MM : 0) : ppm.Largura);

                    variaveisCalcEngine.Add(string.Format("FOLGA{0}ALT", pip.Item.ToUpper().Replace(" ", "")), folgaAltura);
                    variaveisCalcEngine.Add(string.Format("FOLGA{0}LARG", pip.Item.ToUpper().Replace(" ", "")), folgaLargura);

                    /* Chamado 44775. */
                    if (pip.IdProd > 0)
                    {
                        var espessura = ProdutoDAO.Instance.ObtemEspessura((int)pip.IdProd);

                        if (espessura > 0)
                            variaveisCalcEngine.Add(string.Format("P{0}ESP", pip.Item.ToUpper().Replace(" ", "")), espessura);
                    }
                }
            }

            if (tipoArquivo == (int)TipoArquivoMesaCorte.SAG)
            {
                if (variaveisCalcEngine.ContainsKey("SAG_RX1"))
                    variaveisCalcEngine["SAG_RX1"] = descontoLap;
                else
                    variaveisCalcEngine.Add("SAG_RX1", descontoLap);

                if (variaveisCalcEngine.ContainsKey("SAG_RX2"))
                    variaveisCalcEngine["SAG_RX2"] = descontoLap;
                else
                    variaveisCalcEngine.Add("SAG_RX2", descontoLap);

                if (variaveisCalcEngine.ContainsKey("SAG_RY1"))
                    variaveisCalcEngine["SAG_RY1"] = descontoLap;
                else
                    variaveisCalcEngine.Add("SAG_RY1", descontoLap);

                if (variaveisCalcEngine.ContainsKey("SAG_RY2"))
                    variaveisCalcEngine["SAG_RY2"] = descontoLap;
                else
                    variaveisCalcEngine.Add("SAG_RY2", descontoLap);
            }
        }

        #endregion

        #region Obtém dados

        public int ObtemTipoArquivo(uint idArquivoMesaCorte)
        {
            return ObtemTipoArquivo(null, idArquivoMesaCorte);
        }

        /// <summary>
        /// Retorna o tipo de arquivo do arquivo de mesa passado
        /// </summary>
        /// <param name="idArquivoMesaCorte"></param>
        /// <returns></returns>
        public int ObtemTipoArquivo(GDASession sessao, uint idArquivoMesaCorte)
        {
            return ArquivoMesaCorteDAO.Instance.ObtemValorCampo<int>(sessao, "tipoArquivo", "idArquivoMesaCorte=" + idArquivoMesaCorte);
        }

        /// <summary>
        /// Retorna o tipo de arquivo do arquivo de mesa passado
        /// </summary>
        /// <param name="idArquivoMesaCorte"></param>
        /// <returns></returns>
        public int ObtemTipoProjeto(uint idArquivoMesaCorte)
        {
            return ObtemTipoProjeto(null, idArquivoMesaCorte);
        }

        /// <summary>
        /// Retorna o tipo de arquivo do arquivo de mesa passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idArquivoMesaCorte"></param>
        /// <returns></returns>
        public int ObtemTipoProjeto(GDASession session, uint idArquivoMesaCorte)
        {
            return ArquivoMesaCorteDAO.Instance.ObtemValorCampo<int?>(session, "tipoProjeto",
                "idArquivoMesaCorte=" + idArquivoMesaCorte).GetValueOrDefault();
        }

        /// <summary>
        /// Retorna o id do arquivo do CalcEngine associado ao arquivo de mesa.
        /// </summary>
        /// <param name="idArquivoMesaCorte"></param>
        /// <returns></returns>
        public uint ObtemIdArquivoCalcEngine(uint idArquivoMesaCorte)
        {
            return ObtemIdArquivoCalcEngine(null, idArquivoMesaCorte);
        }

        /// <summary>
        /// Retorna o id do arquivo do CalcEngine associado ao arquivo de mesa.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idArquivoMesaCorte"></param>
        /// <returns></returns>
        public uint ObtemIdArquivoCalcEngine(GDASession session, uint idArquivoMesaCorte)
        {
            return ArquivoMesaCorteDAO.Instance.ObtemValorCampo<uint>(session, "idArquivoCalcEngine", "idArquivoMesaCorte=" + idArquivoMesaCorte);
        }

        #endregion

        #region Pedido importado

        /// <summary>
        /// Retorna o caminho exato de onde deve ser salvo o arquivo de marcação do pedido importado.
        /// </summary>
        public string CaminhoSalvarArquivoPedidoImportado(string numeroEtiqueta, int idProdPedEsp, TipoArquivoMesaCorte tipoArquivo)
        {
            return CaminhoSalvarArquivoPedidoImportado(null, numeroEtiqueta, idProdPedEsp, tipoArquivo);
        }

        /// <summary>
        /// Retorna o caminho exato de onde deve ser salvo o arquivo de marcação do pedido importado.
        /// </summary>
        public string CaminhoSalvarArquivoPedidoImportado(GDASession session, string numeroEtiqueta, int idProdPedEsp, TipoArquivoMesaCorte tipoArquivo)
        {
            string forma;
            // Obtém o nome do arquivo de marcação, de acordo com as configurações da empresa que importou o pedido.
            var retorno = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(session, null, tipoArquivo, idProdPedEsp, numeroEtiqueta, false, out forma, false);

            // Salva o arquivo no caminho DXF configurado internamente.
            if (tipoArquivo == TipoArquivoMesaCorte.DXF)
                retorno = PCPConfig.CaminhoSalvarDxf + retorno;
            // Salva o arquivo no caminho FML configurado internamente.
            else if (tipoArquivo == TipoArquivoMesaCorte.FML || tipoArquivo == TipoArquivoMesaCorte.FMLBasico)
                retorno = PCPConfig.CaminhoSalvarFml + retorno;
            else
                retorno = string.Empty;

            return retorno;
        }

        /// <summary>
        /// Salva o arquivo de mesa do pedido importado.
        /// </summary>
        public void SalvarArquivoMesaCorte(GDASession session, uint idProdPedEsp, ExportarPedido.ArquivoMesaCorte arquivo)
        {
            // Verifica se o arquivo é válido e se o produto existe.
            if (arquivo == null || arquivo.Arquivo == null || arquivo.Arquivo.Length == 0 || !ProdutosPedidoEspelhoDAO.Instance.Exists(session, idProdPedEsp))
                return;

            // Recupera todos os números de etiqueta associados ao id do produto do pedido espelho.
            var numerosEtiqueta = ProdutoPedidoProducaoDAO.Instance.ExecuteMultipleScalar<string>(session,
                string.Format("SELECT NumEtiqueta FROM produto_pedido_producao WHERE IdProdPed={0};", idProdPedEsp)).ToArray();

            /* Caso alguma etiqueta tenha sido recuperada, caso a empresa utilize DXF ou FML e o tipo de arquivo seja 
             * DXF, FML OU FML Básico, então, salva um arquivo de marcação para cada etiqueta. */
            if (numerosEtiqueta != null && numerosEtiqueta.Length > 0 &&
                !string.IsNullOrWhiteSpace(numerosEtiqueta[0]) && !string.IsNullOrEmpty(numerosEtiqueta[0]) &&
                (PCPConfig.EmpresaGeraArquivoDxf || PCPConfig.EmpresaGeraArquivoFml || PCPConfig.EmpresaGeraArquivoSGlass) &&
                (arquivo.TipoArquivo == TipoArquivoMesaCorte.DXF ||
                arquivo.TipoArquivo == TipoArquivoMesaCorte.FML ||
                arquivo.TipoArquivo == TipoArquivoMesaCorte.FMLBasico))
            {
                foreach (var numeroEtiqueta in numerosEtiqueta)
                {
                    // Arquivo SGlass
                    if (arquivo.TipoArquivo == TipoArquivoMesaCorte.DXF && arquivo.paraSGlass)
                    {
                        var tempPath = Path.GetTempPath();
                        var programsDirectory = PCPConfig.CaminhoSalvarProgramSGlass;
                        var hardwaresDirectory = PCPConfig.CaminhoSalvarSGlassHardware;
                        string forma;
                        var nomeArquivo = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(session, null, arquivo.TipoArquivo, (int)idProdPedEsp, numeroEtiqueta, false, out forma, false);

                        // Salva o arquivo SGlass na pasta definida
                        PedidoEspelhoDAO.Instance.SalvarArquivoSglass(nomeArquivo, arquivo.Arquivo, tempPath, programsDirectory, hardwaresDirectory);
                    }
                    else
                        using (FileStream f = File.Create(CaminhoSalvarArquivoPedidoImportado(numeroEtiqueta.Replace("  ", "").Replace(" ", ""), (int)idProdPedEsp, arquivo.TipoArquivo)))
                            f.Write(arquivo.Arquivo, 0, arquivo.Arquivo.Length);
                }
            }
            // Se a condição acima não for atendida, salva a marcação na pasta padrão do sistema e com a nomenclatura padrão.
            else
                using (FileStream f = File.Create(Utils.GetArquivoMesaCorteImpPath + idProdPedEsp + ".sag"))
                    f.Write(arquivo.Arquivo, 0, arquivo.Arquivo.Length);
        }

        /// <summary>
        /// Apaga o arquivo de mesa do pedido importado.
        /// </summary>
        public void ApagarArquivoMesaCorte(uint idProdPedEsp)
        {
            ApagarArquivoMesaCorte(null, idProdPedEsp);
        }

        /// <summary>
        /// Apaga o arquivo de mesa do pedido importado.
        /// </summary>
        public void ApagarArquivoMesaCorte(GDASession session, uint idProdPedEsp)
        {
            var arquivoCompleto = string.Empty;

            // Obtém todas as etiquetas associadas ao produto de pedido informado.
            var numerosEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<string[]>(session, "NumEtiqueta",
                "IdProdPed=" + idProdPedEsp);

            /* Caso algum número de etiqueta tenha sido recuperado, caso a empresa trabalhe com DXF ou FML,
             * então, todos os arquivos (por etiqueta) são apagados. */
            if (numerosEtiqueta != null && numerosEtiqueta.Length > 0 &&
                !string.IsNullOrWhiteSpace(numerosEtiqueta[0]) && !string.IsNullOrEmpty(numerosEtiqueta[0]) &&
                (PCPConfig.EmpresaGeraArquivoDxf || PCPConfig.EmpresaGeraArquivoFml))
            {
                foreach (var numeroEtiqueta in numerosEtiqueta)
                {
                    // Recupera o caminho completo do arquivo DXF.
                    arquivoCompleto = CaminhoSalvarArquivoPedidoImportado(session, numeroEtiqueta.Replace("  ", "").Replace(" ", ""),
                        (int)idProdPedEsp, TipoArquivoMesaCorte.DXF);

                    // Caso o caminho exista então apaga o arquivo.
                    if (File.Exists(arquivoCompleto))
                        File.Delete(arquivoCompleto);

                    // Recupera o caminho completo do arquivo FML.
                    arquivoCompleto = CaminhoSalvarArquivoPedidoImportado(session, numeroEtiqueta.Replace("  ", "").Replace(" ", ""),
                        (int)idProdPedEsp, TipoArquivoMesaCorte.FML);

                    // Caso o caminho exista então apaga o arquivo.
                    if (File.Exists(arquivoCompleto))
                        File.Delete(arquivoCompleto);
                }
            }
            /* Caso a condição acima não seja atentida então apaga o arquivo padrão da pasta padrão,
             * de acordo com a identificação do produto do pedido. */
            else
            {
                // Recupera o caminho completo do arquivo SAG.
                arquivoCompleto = Utils.GetArquivoMesaCorteImpPath + idProdPedEsp + ".sag";

                // Caso o caminho exista então apaga o arquivo.
                if (File.Exists(arquivoCompleto))
                    File.Delete(arquivoCompleto);
            }
        }

        #endregion

        #region Salva arquivo CalcEngine

        /// <summary>
        /// Salva na memória o arquivo gerado a partir do arquivo DXF.
        /// </summary>
        /// <param name="idProdPedEsp"></param>
        /// <param name="espessura"></param>
        /// <param name="projeto"></param>
        /// <param name="variaveisCalcEngine"></param>
        /// <param name="arquivo"></param>
        /// <param name="tipoArquivo"></param>
        /// <param name="descontoLap"></param>
        /// <param name="flags"></param>
        /// <param name="forCadProject"></param>
        /// <param name="forSGlass"></param>
        /// <param name="forIntermac"></param>
        /// <returns></returns>
        public bool SalvarArquivoCalcEngine(uint idProdPedEsp, float espessura, CalcEngine.Dxf.DxfProject projeto,
            Dictionary<string, double> variaveisCalcEngine, Stream arquivo, int tipoArquivo, float descontoLap,
            List<FlagArqMesa> flags, bool forCadProject, bool forSGlass, bool forIntermac)
        {
            foreach (var variavelCalcEngine in variaveisCalcEngine)
            {
                // Só por garantia verifica se a variável realmente existe na configuração
                if (projeto.Variables.Any(f => f.Name == variavelCalcEngine.Key))
                {
                    var variavel = projeto.Variables.FirstOrDefault(f => f.Name == variavelCalcEngine.Key);

                    // Não permite setar valores em Functions
                    if (variavel != null && variavel is CalcEngine.Function)
                    {
                        throw new InvalidOperationException($"Está sendo feita uma tentativa inválida de atribuir valor para uma função. Função: { variavel.Name } Valor: { variavel.Value }");
                    }

                    /* Chamado 66568. */
                    if (variavel == null)
                        projeto.Variables.Add(new CalcEngine.Variable(variavelCalcEngine.Key, variavelCalcEngine.Value));
                    else
                        variavel.Value = variavelCalcEngine.Value;
                }
            }

            var tipoArquivoEnum = (TipoArquivoMesaCorte)tipoArquivo;

            #region Flags do projeto

            projeto.Flags.Clear();

            if (forSGlass)
                flags = flags.Where(f => f.Descricao.ToLower() != "waterjet").ToList();
            else
                flags = flags.Where(f => f.Descricao.ToLower() != "sglass").ToList();

            projeto.Flags.Add(new CalcEngine.Flag() { Name = tipoArquivoEnum.ToString() });
            projeto.Flags.Add(new CalcEngine.Flag() { Name = string.Format("[{0}]", System.Configuration.ConfigurationManager.AppSettings["sistema"]) });

            /* Chamado 54681. */
            // Chamado 50979
            if (tipoArquivoEnum == TipoArquivoMesaCorte.DXF && !forCadProject && flags.Any(f => f.Descricao.ToLower() == "waterjet"))
                projeto.Flags.Add(new CalcEngine.Flag() { Name = "RemoveBounds" });

            // Essa flag faz com que a peça seja exibida sempre em pé, caso ela esteja sendo visualizada no cálculo do projeto, em um pedido, orçamento, projeto ou PCP.
            if (forCadProject)
                projeto.Flags.Add(new CalcEngine.Flag("EditMode"));

            foreach (var f in flags)
            {
                if (f.TipoArquivo.HasValue && !f.TipoArquivo.Value.HasFlag(tipoArquivoEnum))
                    continue;

                if (!projeto.Flags.Any(x => x.Name == f.Descricao))
                {
                    projeto.Flags.Add(new CalcEngine.Flag() { Name = f.Descricao });
                }
            }

            // Aplica as vinculações das variáveis com o projeto.
            projeto.Bindings.ApplyBindings();

            #endregion

            bool res = false;
            var task = Task.Run(async () => res = await SalvarArquivoCalcEngine(espessura, projeto, arquivo, tipoArquivo, descontoLap, flags, forCadProject, forSGlass, forIntermac));

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any())
                {
                    throw ex.InnerExceptions.FirstOrDefault();
                }

                throw;
            }

            return res;
        }

        private async Task<bool> SalvarArquivoCalcEngine(
            float espessura,
            CalcEngine.Dxf.DxfProject projeto,
            Stream arquivo,
            int tipoArquivo,
            float descontoLap,
            List<FlagArqMesa> flags,
            bool forCadProject,
            bool forSGlass,
            bool forIntermac)
        {
            var outputDriverProvider = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IOutputDriverProvider>();

            if (forCadProject)
            {
                var driver = await outputDriverProvider.GetDriver("DXF");

                var profile = CompilationProfileFactory.CreateValidationProfile(
                           "WebGlass",
                           "WebGlass Output",
                           driver.Name,
                           String.Empty,
                           projeto.Flags.ToArray(),
                           new CompilationProfileVariable[0],
                           new[]
                           {
                               new CompilationProfileParameter("IncludeProjectVariables", true),
                               new CompilationProfileParameter("CreateQuotas", true),
                               new CompilationProfileParameter("DrawAnnotations", true),
                               new CompilationProfileParameter("CreateLimitQuotas", true),
                           });

                return await driver.CreateOutput(profile, projeto, arquivo);
            }

            switch (tipoArquivo)
            {
                case (int)TipoArquivoMesaCorte.DXF:
                    {
                        // Chamado 50979 - Geração de arquivo SGLASS
                        if (forSGlass)
                        {
                            if (PCPConfig.EmpresaGeraArquivoSGlass && flags.Any(f => f.Descricao.ToLower() == "sglass"))
                            {
                                var driver = await outputDriverProvider.GetDriver("SGLASS Drawing");

                                var profile = CompilationProfileFactory.CreateValidationProfile(
                                   "WebGlass",
                                   "WebGlass Output",
                                   driver.Name,
                                   Guid.NewGuid().ToString(),
                                   projeto.Flags.ToArray(),
                                   new CompilationProfileVariable[0],
                                   new[]
                                   {
                                       new CompilationProfileParameter("Thickness", espessura),
                                   });

                                return await driver.CreateOutput(profile, projeto, arquivo);
                            }

                            break;
                        }

                        if (forIntermac)
                        {
                            if (PCPConfig.EmpresaGeraArquivoIntermac && flags.Any(f => f.Descricao.ToLower() == "intermac"))
                            {

                                var driversInfo = await outputDriverProvider.GetDrivers();
                                var biesseDriversInfo = driversInfo.Where(f => f.Type == "Biesse");

                                // Adiciona os arquivos
                                var zip = new ZipFile(arquivo);

                                foreach (var driverInfo in biesseDriversInfo)
                                {
                                    var driver = await outputDriverProvider.GetDriver(driverInfo.Name);

                                    var profile = CompilationProfileFactory.CreateValidationProfile(
                                        "WebGlass",
                                        "WebGlass Output",
                                        driver.Name,
                                        null,
                                        projeto.Flags.ToArray(),
                                        new CompilationProfileVariable[0],
                                        new[]
                                        {
                                            new CompilationProfileParameter("Thickness", espessura),
                                        });

                                    using (var stream = new System.IO.MemoryStream())
                                    {
                                        bool gerado = false;

                                        try
                                        {
                                            gerado = await driver.CreateOutput(profile, projeto, stream);
                                        }
                                        catch (Exception ex)
                                        {
                                            zip.AddStringAsFile(ex.Message, "error.err", driver.Name);
                                            continue;
                                        }

                                        if (gerado)
                                        {
                                            stream.Position = 0;
                                            var zip2 = ZipFile.Read(stream);

                                            foreach (var file in zip2.EntryFilenames.Select(f => zip2[f]).Where(f => !f.IsDirectory))
                                            {
                                                var fileName = System.IO.Path.Combine(driver.Name, file.FileName);

                                                var fs = new MemoryStream();
                                                file.Extract(fs);
                                                fs.Position = 0;
                                                zip.AddFileStream(System.IO.Path.GetFileName(fileName), System.IO.Path.GetDirectoryName(fileName), fs);
                                            }
                                        }
                                    }
                                }

                                zip.Save();
                            }

                            break;
                        }

                        var dxfOptions = new CalcEngine.Dxf.CreateDxfDocumentOptions()
                        {
                            IncludeBounds = true,
                            // Identifica que é para limpar os atributos customizados
                            ClearCustomAttributes = true
                        };

                        projeto.SaveDxf(arquivo, dxfOptions);

                        break;
                    }
                case (int)TipoArquivoMesaCorte.FMLBasico:
                case (int)TipoArquivoMesaCorte.FML:
                    // Carrega o projeto com base no projeto DXF.

                    var forvetDriver = await outputDriverProvider.GetDriver("Forvet FML");

                    var forvetProfile = CompilationProfileFactory.CreateValidationProfile(
                       "WebGlass",
                       "WebGlass Output",
                       forvetDriver.Name,
                       Guid.NewGuid().ToString(),
                       projeto.Flags.ToArray(),
                       new CompilationProfileVariable[0],
                       new[]
                       {
                            new CompilationProfileParameter("Thickness", espessura),
                       });

                    return await forvetDriver.CreateOutput(forvetProfile, projeto, arquivo);

                case (int)TipoArquivoMesaCorte.SAG:

                    var sagDriver = await outputDriverProvider.GetDriver("SAG");

                    var sagProfile = CompilationProfileFactory.CreateValidationProfile(
                       "WebGlass",
                       "WebGlass Output",
                       sagDriver.Name,
                       Guid.NewGuid().ToString(),
                       projeto.Flags.ToArray(),
                       new CompilationProfileVariable[0],
                       new[]
                       {
                            new CompilationProfileParameter("RX1", descontoLap),
                            new CompilationProfileParameter("RY1", descontoLap),
                            new CompilationProfileParameter("RX2", descontoLap),
                            new CompilationProfileParameter("RY2", descontoLap),
                       });

                    return await sagDriver.CreateOutput(sagProfile, projeto, arquivo);

                default: return false;
            }

            return true;
        }

        #endregion

        #region Configurações

        public class ConfiguracoesArqMesa
        {
            #region Variaveis Locais

            RoldanaConfig _roldana;
            Fechadura1510Config _fechadura1510;
            Fechadura3530Config _fechadura3530;
            TrincoConfig _trinco;

            #endregion

            #region Contrutores

            public ConfiguracoesArqMesa(int largura, float espessura, float descontoLap, string codigoArquivo)
            {
                _roldana = new RoldanaConfig(largura, espessura);
                _fechadura1510 = new Fechadura1510Config();
                _fechadura3530 = new Fechadura3530Config();
                _trinco = new TrincoConfig(descontoLap, codigoArquivo);
            }

            #endregion

            #region Propiedades

            public RoldanaConfig Roldana { get { return _roldana; } }

            public Fechadura1510Config Fechadura1510 { get { return _fechadura1510; } }

            public Fechadura3530Config Fechadura3530 { get { return _fechadura3530; } }

            public TrincoConfig Trinco { get { return _trinco; } }

            #endregion

            #region Classes

            public class RoldanaConfig
            {
                #region Variaveis Locais

                // Posição horizontal da roldana.
                int _posXRoldana = 50;
                // Posição vertical da roldana.
                int _posYRoldana = 20;
                // Raio do furo da roldana.
                int _raioRoldana = 8;

                #endregion

                #region Propiedades

                // Posição horizontal da roldana.
                public int PosXRoldana
                {
                    get
                    {
                        return _posXRoldana;
                    }
                }

                // Posição vertical da roldana.
                public int PosYRoldana
                {
                    get
                    {
                        return _posYRoldana;
                    }
                }

                // Raio do furo da roldana.
                public int RaioRoldana
                {
                    get
                    {
                        return _raioRoldana;
                    }
                }

                #endregion

                #region Métodos Privados

                private void PreencheConfigRoldana(int largura, float espessura)
                {
                    _raioRoldana = ProducaoConfig.RaioRoldana(largura, espessura);
                    _posXRoldana = ProducaoConfig.PosXRoldana(largura);
                    _posYRoldana = ProducaoConfig.PosYRoldana;
                }

                #endregion

                #region Contrutor

                public RoldanaConfig(int largura, float espessura)
                {
                    PreencheConfigRoldana(largura, espessura);
                }

                #endregion
            }

            public class Fechadura1510Config
            {
                #region Propiedades

                // Profundidade da fechadura 1510.
                public float DistBordaFechadura1510
                {
                    get
                    {
                        return 15F;
                    }
                }

                #endregion
            }

            public class Fechadura3530Config
            {
                #region Propiedades

                // Raio do furo central (furo maior) da fechadura 3530.
                public float RaioFuroMaior
                {
                    get
                    {
                        return 15F;
                    }
                }

                // Raio dos furos superior e inferior (furos menores) da fechadura 3530.
                public int RaioFuroMenor
                {
                    get
                    {
                        return 10;
                    }
                }

                #endregion
            }

            public class TrincoConfig
            {
                #region Variaveis Locais

                // Distância da borda horizontal do trinco.
                int _distBordaXTrinco;
                // Distância de baixo para cima do furo do trinco para projetos de correr.
                int _distBordaYTrincoCorrer;
                // Distância de baixo para cima do furo do trinco para projetos de porta de abrir.
                int _distBordaYTrincoPtAbrir;
                // Distância de baixo para cima do furo do trinco para projetos com trilho embutido.
                int _distBordaYTrincoTrilhoEmbut = 65;
                // Distância de baixo para cima do furo do trinco.
                int _distBordaYTrincoSup;
                // Raio do furo do trinco.
                int _raioTrinco = 10;

                #endregion

                #region Contrutor

                public TrincoConfig(float descontoLap, string codigoArquivo)
                {
                    PreencheConfigTrinco(descontoLap, codigoArquivo);
                }

                #endregion

                #region Propiedades

                // Distância da borda horizontal do trinco.
                public int DistBordaXTrinco
                {
                    get
                    {
                        return _distBordaXTrinco;
                    }
                    set
                    {
                        _distBordaXTrinco = value;
                    }
                }

                // Distância de baixo para cima do furo do trinco para projetos de correr.
                public int DistBordaYTrincoCorrer
                {
                    get
                    {
                        return _distBordaYTrincoCorrer;
                    }
                }

                // Distância de baixo para cima do furo do trinco para projetos de porta de abrir.
                public int DistBordaYTrincoPtAbrir
                {
                    get
                    {
                        return _distBordaYTrincoPtAbrir;
                    }

                    set
                    {
                        _distBordaYTrincoPtAbrir = value;
                    }
                }

                // Distância de baixo para cima do furo do trinco para projetos com trilho embutido.
                public int DistBordaYTrincoTrilhoEmbut
                {
                    get
                    {
                        return _distBordaYTrincoTrilhoEmbut;
                    }
                }

                // Raio do furo do trinco.
                public int DistBordaYTrincoSup
                {
                    get
                    {
                        return _distBordaYTrincoSup;
                    }
                }

                // Raio do furo do trinco.
                public int RaioTrinco
                {
                    get
                    {
                        return _raioTrinco;
                    }
                }

                #endregion

                #region Métodos Privados

                private void PreencheConfigTrinco(float descontoLap, string codigoArquivo)
                {
                    _distBordaXTrinco = ProducaoConfig.DistBordaXTrinco;
                    _distBordaYTrincoCorrer = ProducaoConfig.DistBordaYTrincoCorrer;
                    _distBordaYTrincoPtAbrir = 30;
                    _distBordaYTrincoSup = ProducaoConfig.DistBordaYTrincoSup;

                    if (ProducaoConfig.ConfiguracaoTrincoMirandex)
                    {
                        // Arquivos que possuem trinco.
                        switch (codigoArquivo)
                        {
                            case "CR3530TRI":
                            case "CR3530PUXDUPLTRI":
                            case "CRROLDANADUPLA1510COMTRINCO":
                            case "CRROLDANADUPLA3530TRI":
                            case "FECH1510TRI":
                                _distBordaYTrincoCorrer = 30;
                                _distBordaYTrincoPtAbrir = _distBordaYTrincoCorrer;
                                break;

                            case "CR3532MTRI":
                                _distBordaYTrincoCorrer = 47;
                                _distBordaYTrincoPtAbrir = _distBordaYTrincoCorrer;
                                break;

                            case "PTABRIRPUXSIMPLESTRICIMABAIXO":
                            case "PTABRIRPUXDUPLTRI":
                            case "PTABRIRPUXSIMPLESTRI":
                            case "PTABRIRPUXDUPLTRI3210":
                            case "PTABRIRPUXDUPLTRICIMABAIXO3210":
                            case "PTABRIRPUXSIMPLESTRI3210":
                            case "PTABRIRPUXSIMPLESTRICIMABAIXO3210":
                            case "PTABRIRPUXDUPLTRICIMABAIXOBLINDEX3140-3530":
                            case "PTABRIRPUXDUPLTRIBLINDEX3140-3530":
                            case "PTABRIRPUXDUPLBLINDEX3140-3530":
                            case "PTABRIRPUXSIMPLESTRICIMABAIXOBLINDEX3140-3530":
                            case "PTABRIRPUXSIMPLESTRIBLINDEX3140-3530":
                            case "PTABRIRPUXSIMPLESBLINDEX3140-3530":
                            case "PIV3010TRIBAIXO":
                            case "PIV3010TRILATERAL":
                                _distBordaYTrincoCorrer = 30 + _raioTrinco;
                                _distBordaYTrincoPtAbrir = _distBordaYTrincoCorrer;
                                break;
                        }
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Gerar arquivo CalcEngine

        public void GerarArquivoCalcEngine(GDASession session, uint idArquivoCalcEngine, float descontoLap, int tipoArquivo, bool pcp, uint idProdPed, PecaItemProjeto pecaItemProjeto,
            float altura, int largura, ref string mensagemErro, string codigoArquivo, ConfiguracoesArqMesa config, float espessura,
            Stream arquivo, List<FlagArqMesa> flags, ref bool? retorno, bool forCadProject, bool forSGlass, bool forIntermac)
        {
            // Variável criada para carregar o arquivo de extensão .package, que contém o DXF e suas configurações.
            CalcEngine.ProjectFilesPackage pacote = null;

            // Dicionário criado para salvar o valor e o nome das medidas da peça para encaminhar para o CalcEngine.
            var variaveisCalcEngine = new Dictionary<string, double>();

            // Recupera as variáveis do arquivo do CalcEngine.
            var arquivoCeVar = ArquivoCalcEngineVariavelDAO.Instance.ObtemPeloIdArquivoCalcEngine(session, idArquivoCalcEngine, true);
            var acrescimoSag = tipoArquivo == (int)TipoArquivoMesaCorte.SAG ? descontoLap : 0;

            var criadoPeloProjeto = false;
            var idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(pecaItemProjeto.IdItemProjeto);
            var idOrcamento = ItemProjetoDAO.Instance.ObtemIdPedido(pecaItemProjeto.IdItemProjeto);
            var idPedido = ItemProjetoDAO.Instance.ObtemIdPedido(pecaItemProjeto.IdItemProjeto);
            var idPedidoEspelho = ItemProjetoDAO.Instance.ObtemIdPedidoEspelho(pecaItemProjeto.IdItemProjeto);

            if (idProjeto > 0 &&
               idOrcamento.GetValueOrDefault() == 0 &&
               idPedido.GetValueOrDefault() == 0 &&
               idPedidoEspelho.GetValueOrDefault() == 0)
                criadoPeloProjeto = true;

            var caminhoDxf = criadoPeloProjeto ? (PCPConfig.CaminhoSalvarCadProjectProjeto() + pecaItemProjeto.IdPecaItemProj + ".dxf") : (PCPConfig.CaminhoSalvarCadProject(pcp) + idProdPed + ".dxf");

            //Verifica se tem arquivo dxf salvo editado anteriormente.
            if (File.Exists(caminhoDxf))
            {
                var dxfDocument = CalcEngine.Dxf.DxfDocument.Load(caminhoDxf);
                var novoPacote = CalcEngine.Dxf.DxfProject.Import(dxfDocument);
                //novoPacote.Save("d:/novoPacote.package");

                /* Chamado 23500. */
                //Variáveis Compilador
                PreencheVariaveisCompilador(pecaItemProjeto, variaveisCalcEngine, descontoLap, tipoArquivo);

                //Variáveis Calc Engine
                PreencheVariaveisCalcEngine(session, tipoArquivo, pecaItemProjeto, altura, largura, mensagemErro, codigoArquivo, config, variaveisCalcEngine, arquivoCeVar, acrescimoSag);

                /* Chamados 54424 e 54913. */
                if (flags != null && (flags.Any(f => !string.IsNullOrWhiteSpace(f.Descricao) && f.Descricao.ToLower().Contains("rotateangle")) ||
                    flags.Any(f => !string.IsNullOrWhiteSpace(f.Descricao) && f.Descricao.ToLower().Contains("mirror"))))
                    foreach (var flag in flags.Where(f => !string.IsNullOrWhiteSpace(f.Descricao) && (f.Descricao.ToLower().Contains("rotateangle") ||
                        f.Descricao.ToLower().Contains("mirror"))).ToList())
                        flags.Remove(flag);

                // Salva a marcação da peça conforme o tipo do arquivo.
                retorno = SalvarArquivoCalcEngine(idProdPed, espessura, novoPacote, variaveisCalcEngine, arquivo, tipoArquivo, descontoLap, flags, forCadProject, forSGlass, forIntermac);

                if (retorno != null)
                    return;
            }

            var caminhoPackage = Utils.GetArquivoCalcEnginePath + codigoArquivo + ".calcpackage";

            // Gera o retorno somente se existir o arquivo .calcengine.
            if (File.Exists(caminhoPackage))
            {
                using (Stream pacoteStream = File.OpenRead(caminhoPackage))
                {
                    // Esse método deserializa os dados do pacote que estão contidos
                    // na Stream a recupera a instancia do pacote de configuração
                    pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                }
                
                // Lê a configuração do projeto.
                var projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                // Altera o ReferenceValueProvider para buscar os Valores das Constantes da ferragem cadastrada no WebGlass
                projeto.ReferenceValueProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<CalcEngine.IReferenceValueProvider>();

                // Fixa os valores padrão de referência
                foreach (var referenceValue in projeto.Variables.OfType<ReferenceValue>())
                {
                    referenceValue.DefaultValue = referenceValue.Value;
                }

                /* Chamado 23500. */
                //Variáveis Compilador
                PreencheVariaveisCompilador(pecaItemProjeto, variaveisCalcEngine, descontoLap, tipoArquivo);

                //Variáveis Calc Engine
                PreencheVariaveisCalcEngine(session, tipoArquivo, pecaItemProjeto, altura, largura, mensagemErro, codigoArquivo, config, variaveisCalcEngine, arquivoCeVar, acrescimoSag);

                // Salva a marcação da peça conforme o tipo do arquivo.
                retorno = SalvarArquivoCalcEngine(idProdPed, espessura, projeto, variaveisCalcEngine, arquivo, tipoArquivo, descontoLap, flags, forCadProject, forSGlass, forIntermac);
            }
        }

        #endregion

        public float ObterDescontoLapidacao(GDASession session, ref PecaItemProjeto pecaItemProjeto, uint? idArquivoMesaCorte,
            int idProd, List<int> idsBenefConfig, string descrBenef, int idProcesso)
        {
            var descontoLap = ImpressaoEtiquetaDAO.Instance.GetAresta(session, idProd, idArquivoMesaCorte, idsBenefConfig, descrBenef, idProcesso) / 2f;

            return descontoLap;
        }

        public object ValidarArquivoCalcEngine(string nomeArquivoCalcEngine)
        {
            var caminhoPackage = Utils.GetArquivoCalcEnginePath + nomeArquivoCalcEngine + ".calcpackage";

            // Variável criada para carregar o arquivo de extensão .package, que contém o DXF e suas configurações.
            CalcEngine.ProjectFilesPackage pacote = null;
            if (File.Exists(caminhoPackage))
            {
                using (Stream pacoteStream = File.OpenRead(caminhoPackage))
                {
                    // Esse método deserializa os dados do pacote que estão contidos
                    // na Stream a recupera a instancia do pacote de configuração
                    pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                }

                // Lê a configuração do projeto.
                var projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

                // Altera o ReferenceValueProvider para buscar os Valores das Constantes da ferragem cadastrada no WebGlass
                projeto.ReferenceValueProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<CalcEngine.IReferenceValueProvider>();

                var validator = new ProjectValidator(projeto, pacote, ValidadorCalc.Provider);
                ProjectValidatorResult result = null;

                // Executa a validação do projeto
                var resultTask = Task.Run(async () => result = await validator.Execute());

                try
                {
                    resultTask.Wait();
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Any())
                    {
                        throw ex.InnerExceptions.FirstOrDefault();
                    }

                    throw;
                }

                if (result.Success)
                {
                    return
                        new
                        {
                            Arquivo = nomeArquivoCalcEngine,
                            Sucesso = true
                        };
                }

                var textoErros = new List<object>();

                foreach (var ProfileResult in result.ProfileResults)
                {
                    if (ProfileResult.Status == ValidationProfileResultStatus.Success)
                        continue;

                    var item = ProfileResult.Items.First();
                    var diffPaneModel = (DiffPaneModel)item
                        .Attachments
                        .Where(f => f.Type == ValidationProfileResultItemAttachmentType.DiffResult &&
                            f.Content is DiffPaneModel)
                        .FirstOrDefault()?.Content;

                    var Erros = string.Empty;
                    int linhaAnterior = 0;

                    foreach (var line in diffPaneModel.Lines)
                    {
                        var erro = string.Empty;

                        switch (line.Type)
                        {
                            case ChangeType.Inserted:
                                erro += "(" + line.Position + ") + ";
                                linhaAnterior = line.Position.Value;
                                break;
                            case ChangeType.Deleted:
                                linhaAnterior++;
                                erro += "(" + linhaAnterior + ") - ";
                                break;
                            default:
                                erro += "(" + line.Position + ")  ";
                                linhaAnterior = line.Position.Value;
                                break;
                        }

                        if (line.Type != ChangeType.Unchanged)
                        {
                            Erros += erro + line.Text + "|";
                        }
                    }

                    textoErros.Add(new { Erros });
                }

                return
                    new
                    {
                        Arquivo = nomeArquivoCalcEngine,
                        Sucesso = false,
                        ErroProfile = result.ProfileResults.Select(f => f.Profile.Name + "-" + f.Message),
                        Mensagem = result.ProfileResults.Select(f => f.Items.Any() ? f.Items.First().Message : ""),
                        LinhasErro = textoErros
                    };
            }

            return
                new
                {
                    Arquivo = nomeArquivoCalcEngine,
                    Sucesso = false,
                    ErroProfile = new[] { "Arquivo não encontrado" },
                    LinhasErro = ""
                };
        }

        public bool ValidarCadastroCalcEngine(Stream stream)
        {
            CalcEngine.ProjectFilesPackage pacote = null;

            // Esse método deserializa os dados do pacote que estão contidos
            // na Stream a recupera a instancia do pacote de configuração
            pacote = CalcEngine.ProjectFilesPackage.LoadPackage(stream);

            // Lê a configuração do projeto.
            var projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);

            // Altera o ReferenceValueProvider para buscar os Valores das Constantes da ferragem cadastrada no WebGlass
            projeto.ReferenceValueProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<CalcEngine.IReferenceValueProvider>();

            var validator = new ProjectValidator(projeto, pacote, ValidadorCalc.Provider);

            // Executa a validação do projeto
            var result = validator.Execute();

            Task.WaitAny(result);

            if (result.IsFaulted)
            {
                throw result.Exception;
            }

            return result.Result.Success;
        }
    }

    public static class ValidadorCalc
    {
        public static OutputDriverProvider Provider = new OutputDriverProvider(new IOutputDriver[]
           {
               new CalcEngine.Dxf.DxfOutputDriver(),
               new CalcEngine.Dxf.RawDxfOutputDriver(),
               //new CalcEngine.Biesse.BiesseOutputDriver(GetBSolidConfigurationDirectory())
           });
    }
}
