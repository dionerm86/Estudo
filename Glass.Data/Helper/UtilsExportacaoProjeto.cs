using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;
using System.Web;
using Glass.Data.Exceptions;
using System.Linq;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.Helper
{
    #region Classe de Suporte

    [Serializable]
    [XmlRoot("exportarProjeto")]
    public class ExportarProjeto
    {
        #region Item

        [XmlRoot("figuraItem")]
        public class DadosFiguraItem
        {
            #region Enumeradores

            public enum TipoFiguraItem
            {
                FiguraAssociada,
                FiguraProjeto,
                FiguraItem,
                FiguraItemMarcacao
            }

            #endregion

            #region Construtores

            public DadosFiguraItem()
            {
            }

            public DadosFiguraItem(TipoFiguraItem tipoFigura, string itemFiguraAssociada, byte[] figura)
            {
                TipoFigura = tipoFigura;
                ItemFiguraAssociada = itemFiguraAssociada;
                Figura = figura;
            }

            #endregion

            [XmlAttribute("tipoFigura")]
            public TipoFiguraItem TipoFigura;

            [XmlAttribute("itemFiguraAssociada")]
            public string ItemFiguraAssociada;

            [XmlElement("figura")]
            public byte[] Figura;
        }

        [Serializable]
        [XmlRoot("item")]
        public class Item
        {
            #region Construtores

            public Item()
            {
            }

            public Item(uint idProjetoModelo, bool semFolgas)
            {
                ProjetoModelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(idProjetoModelo);

                ValidacaoPecaModelo = new List<Model.ValidacaoPecaModelo>();
                FlagArqMesa = new List<Model.FlagArqMesa>();
                FlagArqMesaPecaProjMod = new List<Model.FlagArqMesaPecaProjMod>();
                FlagsArqMesaArqCalcEngine = new List<Model.FlagArqMesaArqCalcEngine>();
                ArquivoMesaCorte = new List<Model.ArquivoMesaCorte>();
                ArquivoCalcEngine = new List<Model.ArquivoCalcEngine>();
                ArquivosCalcEngineVariavel = new List<Model.ArquivoCalcEngineVariavel>();
                ArquivoCalcPackage = new List<byte[]>();

                List<DadosFiguraItem> figura = new List<DadosFiguraItem>();
                figura.Add(new DadosFiguraItem(DadosFiguraItem.TipoFiguraItem.FiguraProjeto, null, Utils.GetImageFromFile(Utils.GetModelosProjetoPath + ProjetoModelo.NomeFigura)));
                figura.Add(new DadosFiguraItem(DadosFiguraItem.TipoFiguraItem.FiguraAssociada, null, Utils.GetImageFromFile(Utils.GetModelosProjetoPath + ProjetoModelo.NomeFiguraAssociada)));

                GrupoModelo = GrupoModeloDAO.Instance.GetElementByPrimaryKey(ProjetoModelo.IdGrupoModelo);
                MedidaProjetoModelo = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(idProjetoModelo, false).ToArray();

                Dictionary<uint, MedidaProjeto> medidasProjeto = new Dictionary<uint, MedidaProjeto>();
                foreach (MedidaProjetoModelo m in MedidaProjetoModelo)
                {
                    if (!medidasProjeto.ContainsKey(m.IdMedidaProjeto))
                        medidasProjeto.Add(m.IdMedidaProjeto, MedidaProjetoDAO.Instance.GetElementByPrimaryKey(m.IdMedidaProjeto));
                }

                MedidaProjeto = new MedidaProjeto[medidasProjeto.Count];
                medidasProjeto.Values.CopyTo(MedidaProjeto, 0);

                Dictionary<uint, GrupoMedidaProjeto> grupoMedidasProjeto = new Dictionary<uint, GrupoMedidaProjeto>();
                foreach(MedidaProjeto mp in MedidaProjeto)
                {
                    if (mp.IdGrupoMedProj > 0 && !grupoMedidasProjeto.ContainsKey((uint)mp.IdGrupoMedProj))
                    {
                        var grupoMedProj = GrupoMedidaProjetoDAO.Instance.GetElementByPrimaryKey((uint)mp.IdGrupoMedProj);
                        if (grupoMedProj.PodeEditarExcluir)
                            grupoMedidasProjeto.Add(grupoMedProj.IdGrupoMedProj, grupoMedProj);
                    }
                }

                GrupoMedidaProjeto = new Model.GrupoMedidaProjeto[grupoMedidasProjeto.Count];
                grupoMedidasProjeto.Values.CopyTo(GrupoMedidaProjeto, 0);

                PecaProjetoModelo = PecaProjetoModeloDAO.Instance.GetByModelo(idProjetoModelo).ToArray();

                List<PosicaoPecaIndividual> posicoesPecaIndiviual = new List<PosicaoPecaIndividual>();
                foreach (PecaProjetoModelo p in PecaProjetoModelo)
                {
                    if (semFolgas)
                    {
                        p.Altura = 0;
                        p.Altura06MM = 0;
                        p.Altura08MM = 0;
                        p.Altura10MM = 0;
                        p.Altura12MM = 0;
                        p.Largura = 0;
                        p.Largura06MM = 0;
                        p.Largura08MM = 0;
                        p.Largura10MM = 0;
                        p.Largura12MM = 0;
                    }

                    foreach (string item in UtilsProjeto.GetItensFromPeca(p.Item))
                    {
                        posicoesPecaIndiviual.AddRange(PosicaoPecaIndividualDAO.Instance.GetPosicoes(p.IdPecaProjMod, Glass.Conversoes.StrParaInt(item)));

                        string nomeItem = Utils.GetModelosProjetoPath + ProjetoModelo.Codigo + "§" + item + ".jpg";
                        /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                        if (!File.Exists(nomeItem))
                            nomeItem = Utils.GetModelosProjetoPath + idProjetoModelo.ToString("0##") + "_" + item + ".jpg";
                        figura.Add(new DadosFiguraItem(DadosFiguraItem.TipoFiguraItem.FiguraItem, item, Utils.GetImageFromFile(nomeItem)));
                        figura.Add(new DadosFiguraItem(DadosFiguraItem.TipoFiguraItem.FiguraItemMarcacao, item, Utils.GetImageFromFile(nomeItem + "M")));
                    }

                    //ValidacaoPecaModelo
                    var validacaoPecaModeloArray = ValidacaoPecaModeloDAO.Instance.ObtemValidacoes(((int)p.IdPecaProjMod)).ToArray();
                    foreach (var validacaoPecaModelo in validacaoPecaModeloArray)
                        if (validacaoPecaModelo.IdPecaProjMod > 0)
                            ValidacaoPecaModelo.Add(validacaoPecaModelo);

                    // Flag de arquivo de mesa associada à peça do projeto.
                    var flagArqMesaPecaProjMod = FlagArqMesaPecaProjModDAO.Instance.ObtemPorPecaProjMod((int)p.IdPecaProjMod);

                    Dictionary<int, FlagArqMesa> flagArqMesa = new Dictionary<int, FlagArqMesa>();
                    foreach (FlagArqMesaPecaProjMod f in flagArqMesaPecaProjMod)
                    {
                        if (!flagArqMesa.ContainsKey(f.IdFlagArqMesa))
                        {
                            flagArqMesa.Add(f.IdFlagArqMesa, FlagArqMesaDAO.Instance.GetElementByPrimaryKey(f.IdFlagArqMesa));
                            FlagArqMesaPecaProjMod.Add(f);
                        }
                    }

                    //ArquivoMesaCorte
                    if (p.IdArquivoMesaCorte != null)
                    {
                        // Exporta o arquivo de mesa.
                        var arquivoMesaCorte = ArquivoMesaCorteDAO.Instance.GetElement((uint)p.IdArquivoMesaCorte);
                        ArquivoMesaCorte.Add(arquivoMesaCorte);

                        // Exporta o arquivo Calc Engine.
                        var arquivoCalcEngine = ArquivoCalcEngineDAO.Instance.ObtemArquivoCalcEngine((uint)arquivoMesaCorte.IdArquivoCalcEngine);
                        ArquivoCalcEngine.Add(arquivoCalcEngine);

                        // Exporta as flags associadas ao arquivo CalcEngine.
                        var flagsArqMesaArqCalcEngine = FlagArqMesaArqCalcEngineDAO.Instance.ObtemPorArqCalcEngine(arquivoMesaCorte.IdArquivoCalcEngine);
                        FlagsArqMesaArqCalcEngine.AddRange(flagsArqMesaArqCalcEngine);

                        /* Chamado 49748. */
                        foreach (var f in flagsArqMesaArqCalcEngine)
                            if (!flagArqMesa.ContainsKey(f.IdFlagArqMesa))
                                flagArqMesa.Add(f.IdFlagArqMesa, FlagArqMesaDAO.Instance.GetElementByPrimaryKey(f.IdFlagArqMesa));
                        
                        // Exporta as variáveis associadas ao arquivo CalcEngine.
                        var arquivosCalcEngineVariavel = ArquivoCalcEngineVariavelDAO.Instance.ObtemPeloIdArquivoCalcEngine(arquivoCalcEngine.IdArquivoCalcEngine, true);
                        ArquivosCalcEngineVariavel.AddRange(arquivosCalcEngineVariavel);

                        // Exporta o arquivo físico do Calc Engine.
                        var caminhoArquivo = ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage";
                        if (File.Exists(caminhoArquivo))
                            using (FileStream f = File.OpenRead(caminhoArquivo))
                            {
                                byte[] arquivoCalcPackage = new byte[f.Length];
                                f.Read(arquivoCalcPackage, 0, arquivoCalcPackage.Length);
                                ArquivoCalcPackage.Add(arquivoCalcPackage);
                                f.Flush();
                            }
                    }

                    // Adiciona as Flags das peças e dos arquivos CalcEngine.
                    foreach (FlagArqMesa f in flagArqMesa.Select(f => f.Value))
                        FlagArqMesa.Add(f);
                }

                PosicaoPecaIndividual = posicoesPecaIndiviual.ToArray();
                PosicaoPecaModelo = PosicaoPecaModeloDAO.Instance.GetPosicoes(idProjetoModelo).ToArray();

                // Formula Expressão de Calculo
                var formulasExpressaoCalculo = new List<Model.FormulaExpressaoCalculo>();
                formulasExpressaoCalculo.AddRange(FormulaExpressaoCalculoDAO.Instance.ObtemFormulaExpressaoPorArrayPosicaoPecaModelo(PosicaoPecaModelo));
                formulasExpressaoCalculo.AddRange(FormulaExpressaoCalculoDAO.Instance.ObtemFormulaExpressaoPorArrayPosicaoPecaIndividual(PosicaoPecaIndividual));

                foreach (var formulaExpressaoCalculo in formulasExpressaoCalculo)
                    if (!FormulaExpressaoCalculo.Contains(formulaExpressaoCalculo))
                        FormulaExpressaoCalculo.ToList().Add(formulaExpressaoCalculo);

                // Material Projeto Modelo
                MaterialProjetoModelo = MaterialProjetoModeloDAO.Instance.GetByProjetoModelo(idProjetoModelo, null).ToArray();

                Dictionary<uint, ProdutoProjeto> produtosProjeto = new Dictionary<uint, ProdutoProjeto>();
                foreach (MaterialProjetoModelo m in MaterialProjetoModelo)
                {
                    if (!produtosProjeto.ContainsKey(m.IdProdProj))
                        produtosProjeto.Add(m.IdProdProj, ProdutoProjetoDAO.Instance.GetElementByPrimaryKey(m.IdProdProj));
                }

                ProdutoProjeto = new ProdutoProjeto[produtosProjeto.Count];
                produtosProjeto.Values.CopyTo(ProdutoProjeto, 0);
                FiguraItem = figura.ToArray();
            }

            #endregion

            [XmlElement("grupoModelo")]
            public GrupoModelo GrupoModelo;

            [XmlElement("projetoModelo")]
            public ProjetoModelo ProjetoModelo;

            [XmlElement("medidaProjetoModelo")]
            public MedidaProjetoModelo[] MedidaProjetoModelo;

            [XmlElement("medidaProjeto")]
            public MedidaProjeto[] MedidaProjeto;

            [XmlElement("grupoMedidaProjeto")]
            public GrupoMedidaProjeto[] GrupoMedidaProjeto;

            [XmlElement("pecaProjetoModelo")]
            public PecaProjetoModelo[] PecaProjetoModelo;

            [XmlElement("arquivomesacorte")]
            public List<ArquivoMesaCorte> ArquivoMesaCorte;

            [XmlElement("arquivocalcengine")]
            public List<ArquivoCalcEngine> ArquivoCalcEngine;

            [XmlElement("arquivoscalcenginevariavel")]
            public List<ArquivoCalcEngineVariavel> ArquivosCalcEngineVariavel;

            [XmlElement("ArquivoCalcPackage")]
            public List<byte[]> ArquivoCalcPackage;

            [XmlElement("posicaoPecaModelo")]
            public PosicaoPecaModelo[] PosicaoPecaModelo;

            [XmlElement("posicaoPecaIndividual")]
            public PosicaoPecaIndividual[] PosicaoPecaIndividual;

            [XmlElement("formulaExpressaoCalculo")]
            public FormulaExpressaoCalculo[] FormulaExpressaoCalculo;

            [XmlElement("figuraItem")]
            public DadosFiguraItem[] FiguraItem;

            [XmlElement("materialProjetoModelo")]
            public MaterialProjetoModelo[] MaterialProjetoModelo;

            [XmlElement("produtoProjeto")]
            public ProdutoProjeto[] ProdutoProjeto;

            [XmlElement("validacaoPecaModelo")]
            public List<ValidacaoPecaModelo> ValidacaoPecaModelo;

            [XmlElement("flagArqMesaPecaProjMod")]
            public List<FlagArqMesaPecaProjMod> FlagArqMesaPecaProjMod;

            [XmlElement("flagsArqMesaArqCalcEngine")]
            public List<FlagArqMesaArqCalcEngine> FlagsArqMesaArqCalcEngine;

            [XmlElement("flagArqMesa")]
            public List<FlagArqMesa> FlagArqMesa;

        }

        #endregion

        private List<Item> _itens = new List<Item>();

        [XmlElement("item")]
        public List<Item> Itens
        {
            get { return _itens; }
        }
    }

    #endregion

    public static class UtilsExportacaoProjeto
    {
        #region Métodos de serialização

        private static ExportarProjeto Deserializar(byte[] arquivo)
        {
            ExportarProjeto retorno;
            XmlSerializer s = new XmlSerializer(typeof(ExportarProjeto));

            using (MemoryStream m = new MemoryStream(arquivo))
                retorno = s.Deserialize(m) as ExportarProjeto;

            return retorno;
        }

        private static byte[] Serializar(ExportarProjeto projeto)
        {
            byte[] retorno;
            XmlSerializer s = new XmlSerializer(typeof(ExportarProjeto));

            using (MemoryStream m = new MemoryStream())
            {
                s.Serialize(m, projeto);
                m.Position = 0;
                using (BinaryReader r = new BinaryReader(m))
                    retorno = r.ReadBytes((int)m.Length);
            }

            return retorno;
        }

        #endregion

        #region Exportar

        /// <summary>
        /// Exporta os projetos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        public static byte[] Exportar(string idsProjetosModelo, bool semFolgas)
        {
            if (idsProjetosModelo.Length == 0)
                throw new Exception("Selecione pelo menos 1 projeto modelo para exportar.");

            List<uint> id = new List<uint>();
            foreach (string s in idsProjetosModelo.TrimEnd(',').Split(','))
                if (s.StrParaUint() > 0)
                    id.Add(s.StrParaUint());

            return Exportar(semFolgas, id.ToArray());
        }

        /// <summary>
        /// Exporta os projetos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        public static byte[] Exportar(bool semFolgas, params uint[] idProjetoModelo)
        {
            ExportarProjeto projeto = new ExportarProjeto();
            foreach (uint id in idProjetoModelo)
                if (ProjetoModeloDAO.Instance.IsConfiguravel(id))
                    projeto.Itens.Add(new ExportarProjeto.Item(id, semFolgas));

            if (projeto.Itens.Count == 0)
                throw new Exception("Selecione apenas projetos configuráveis.");

            return Arquivos.Compactar(Serializar(projeto));
        }

        #endregion

        #region Importar

        public static string Importar(byte[] arquivo, bool importarArquivoMesaCorte, bool importarFlag, bool importarRegraValidacao, bool importarFormulaExpressaoCalculo, bool subProjModExist)
        {
            ExportarProjeto projeto = new ExportarProjeto();
            ResultadoSalvar retorno = new ResultadoSalvar();
            string mensagemErro;

            try
            {
                projeto = Deserializar(Arquivos.Descompactar(arquivo));
                retorno = SalvarItens(projeto.Itens, importarArquivoMesaCorte, importarFlag, importarRegraValidacao, importarFormulaExpressaoCalculo, subProjModExist, out mensagemErro);
            }
            catch (Exception ex)
            {
                // Salva o erro
                throw new Exception("Não foi possível importar o arquivo. Verifique se o arquivo é um arquivo válido de projetos do WebGlass.", ex);
            }

            if (!String.IsNullOrEmpty(retorno.NaoInseridos))
            {
                if (!String.IsNullOrEmpty(retorno.Inseridos))
                {
                    throw new Exception("Alguns modelos de projeto não foram importados. Verifique se eles já existem.\n" +
                        "Modelos importados: " + retorno.Inseridos + "\n" +
                        "Modelos não importados: " + retorno.NaoInseridos + "\n" + mensagemErro);
                }
                else
                    throw new Exception("Não foi possível importar nenhum modelo de projeto.\n" +
                        "Modelos não importados: " + retorno.NaoInseridos + "\n" + mensagemErro);
            }

            return "Importação realizada com sucesso! Projetos importados: " + retorno.Inseridos;
        }

        #endregion

        #region Duplicar

        /// <summary>
        /// Duplicar os projetos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        /// <param name="idsProjetoModelo"></param>
        public static string Duplicar(uint idGrupoModelo, string finalCodigo, string idsProjetosModelo)
        {
            if (idsProjetosModelo.Length == 0)
                throw new Exception("Selecione pelo menos 1 projeto modelo para exportar.");

            List<uint> id = new List<uint>();
            foreach (string s in idsProjetosModelo.TrimEnd(',').Split(','))
                id.Add(Glass.Conversoes.StrParaUint(s));

            return Duplicar(idGrupoModelo, finalCodigo, id.ToArray());
        }

        /// <summary>
        /// Duplicar os projetos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        public static string Duplicar(uint idGrupoModelo, string finalCodigo, params uint[] idProjetoModelo)
        {
            ResultadoSalvar retorno = new ResultadoSalvar();
            List<ExportarProjeto.Item> itens = new List<ExportarProjeto.Item>();

            foreach (uint id in idProjetoModelo)
                itens.Add(new ExportarProjeto.Item(id, false));
            string mensagemErro;

            try
            {
                retorno = SalvarItens(itens, idGrupoModelo, finalCodigo, true, out mensagemErro);
            }
            catch (Exception ex)
            {
                // Salva o erro
                throw new Exception("Não foi possível duplicar os projetos.", ex);
            }

            if (!String.IsNullOrEmpty(retorno.NaoInseridos))
            {
                if (!String.IsNullOrEmpty(retorno.Inseridos))
                {
                    throw new Exception("Alguns modelos de projeto não foram importados. Verifique se eles já existem.<br />" +
                        "Modelos importados: " + retorno.Inseridos + "<br />" +
                        "Modelos não importados: " + retorno.NaoInseridos + "\n" + mensagemErro);
                }
                else
                    throw new Exception("Não foi possível importar nenhum modelo de projeto.<br />" +
                        "Modelos não importados: " + retorno.NaoInseridos + "\n" + mensagemErro);
            }

            return "Duplicação realizada com sucesso! Projetos duplicados: " + retorno.Inseridos;
        }

        #endregion

        #region Salva os itens de exportação de projeto

        #region Classes de suporte

        private class DadosProjeto
        {
            public uint idProjetoModelo = 0;
            public string codigoModelo = "";
            public Dictionary<uint, uint> grupoModelo = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> grupoMedidasProjeto = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> medidasProjeto = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> medidasProjetoModelo = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> produtosProjeto = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> materiaisProjetoModelo = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> pecasModeloProjeto = new Dictionary<uint, uint>();
            public Dictionary<int, int> arquivoMesaCorte = new Dictionary<int, int>();
            public Dictionary<int, bool> arquivoMesaCorteNovo = new Dictionary<int, bool>();
            public Dictionary<int, int> arquivoCalcEngine = new Dictionary<int, int>();
            public Dictionary<uint, uint> posicoesPecaModelo = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> posicoesPecaIndividual = new Dictionary<uint, uint>();
            public Dictionary<uint, uint> formulaExpressaoCalculo = new Dictionary<uint, uint>();
            public Dictionary<int, int> validacaoPecaModelo = new Dictionary<int, int>();
            public Dictionary<int, int> flagArqMesa = new Dictionary<int, int>();
            public List<string> imagens = new List<string>();
        }

        private class ResultadoSalvar
        {
            public string Inseridos, NaoInseridos;
        }

        #endregion

        private static ResultadoSalvar SalvarItens(List<ExportarProjeto.Item> itens, uint? idGrupoModelo, string finalCodigo, bool duplicar, out string mensagemErro)
        {
            return SalvarItens(itens, idGrupoModelo, finalCodigo, true, true, true, true, false, duplicar, out mensagemErro);
        }

        private static ResultadoSalvar SalvarItens(List<ExportarProjeto.Item> itens, bool importarArquivoMesaCorte, bool importarFlag, bool importarRegraValidacao,
            bool importarFormulaExpressaoCalculo, bool subProjModExist, out string mensagemErro)
        {
            return SalvarItens(itens, null, null, importarArquivoMesaCorte, importarFlag, importarRegraValidacao, importarFormulaExpressaoCalculo, subProjModExist, false, out mensagemErro);
        }

        private static ResultadoSalvar SalvarItens(List<ExportarProjeto.Item> itens, uint? idGrupoModelo, string finalCodigo, bool importarArquivoMesaCorte, bool importarFlag,
            bool importarRegraValidacao, bool importarFormulaExpressaoCalculo, bool subProjModExist, bool duplicar, out string mensagemErro)
        {
            var dados = new List<DadosProjeto>();
            var naoCadastrados = new List<int>();
            mensagemErro = string.Empty;

            for (var i = 0; i < itens.Count; i++)
            {
                using (var transaction = new GDATransaction())
                    try
                    {
                        transaction.BeginTransaction();

                        dados.Add(new DadosProjeto());
                        uint? busca;
                        int? busca1;

                        #region Insere o grupo, se necessário

                        if (idGrupoModelo > 0)
                            dados[i].grupoModelo.Add(itens[i].GrupoModelo.IdGrupoModelo, idGrupoModelo.Value);

                        if (!dados[i].grupoModelo.ContainsKey(itens[i].GrupoModelo.IdGrupoModelo))
                        {
                            uint atual = itens[i].GrupoModelo.IdGrupoModelo;
                            busca = GrupoModeloDAO.Instance.FindByDescricao(transaction, itens[i].GrupoModelo.IdGrupoModelo, itens[i].GrupoModelo.Descricao);
                            if (busca == null || busca == 0)
                            {
                                itens[i].GrupoModelo.IdGrupoModelo = 0;
                                uint idGrupoModeloNovo = GrupoModeloDAO.Instance.Insert(transaction, itens[i].GrupoModelo);

                                dados[i].grupoModelo.Add(atual, idGrupoModeloNovo);
                            }
                            else
                                dados[i].grupoModelo.Add(atual, busca.Value);
                        }

                        #endregion

                        #region Insere o projeto e salva seu ID

                        if (!string.IsNullOrEmpty(finalCodigo))
                            itens[i].ProjetoModelo.Codigo += finalCodigo;

                        // Caso o usuario marque a opção de Substituir Projeto Modelo Existente.
                        if (subProjModExist)
                        {
                            // Procura Projeto Modelo com o mesmo código do importado.
                            var projModOld = ProjetoModeloDAO.Instance.GetByCodigo(transaction, itens[i].ProjetoModelo.Codigo);

                            if (projModOld != null)
                            {
                                projModOld.Codigo += "_" + DateTime.Now.ToString("yyMMddHHmm");
                                projModOld.Situacao = (int)ProjetoModelo.SituacaoEnum.Inativo;
                                ProjetoModeloDAO.Instance.Update(transaction, projModOld);
                            }
                        }

                        itens[i].ProjetoModelo.IdGrupoModelo = dados[i].grupoModelo[itens[i].ProjetoModelo.IdGrupoModelo];
                        itens[i].ProjetoModelo.IdProjetoModelo = 0;
                        itens[i].ProjetoModelo.IdProjetoModelo = ProjetoModeloDAO.Instance.Insert(transaction, itens[i].ProjetoModelo);
                        itens[i].ProjetoModelo.Situacao = ProjetoModeloDAO.Instance.ObtemSituacao(transaction, itens[i].ProjetoModelo.IdProjetoModelo);
                        dados[i].idProjetoModelo = itens[i].ProjetoModelo.IdProjetoModelo;
                        dados[i].codigoModelo = itens[i].ProjetoModelo.Codigo;

                        itens[i].ProjetoModelo.NomeFigura = itens[i].ProjetoModelo.Codigo + ".jpg";
                        itens[i].ProjetoModelo.NomeFiguraAssociada = itens[i].ProjetoModelo.Codigo + "§E.jpg";
                        ProjetoModeloDAO.Instance.Update(transaction, itens[i].ProjetoModelo);

                        #endregion

                        #region Salva as figuras do modelo

                        if (itens[i].FiguraItem != null)
                            foreach (ExportarProjeto.DadosFiguraItem f in itens[i].FiguraItem)
                            {
                                if (f.Figura.Length == 0)
                                    continue;

                                string nomeArquivo = Utils.GetModelosProjetoPath;
                                nomeArquivo += dados[i].codigoModelo;
                                nomeArquivo += f.TipoFigura == ExportarProjeto.DadosFiguraItem.TipoFiguraItem.FiguraItem ||
                                    f.TipoFigura == ExportarProjeto.DadosFiguraItem.TipoFiguraItem.FiguraItemMarcacao ? "§" + f.ItemFiguraAssociada : "";
                                nomeArquivo += f.TipoFigura == ExportarProjeto.DadosFiguraItem.TipoFiguraItem.FiguraItemMarcacao ? "M" : "";
                                nomeArquivo += f.TipoFigura == ExportarProjeto.DadosFiguraItem.TipoFiguraItem.FiguraAssociada ? "§E" : "";
                                nomeArquivo += ".jpg";

                                ManipulacaoImagem.SalvarImagem(nomeArquivo, f.Figura);
                                dados[i].imagens.Add(nomeArquivo);
                            }

                        #endregion

                        #region Salva os grupos de medidas do projeto

                        // Verifica quais grupos de medidas de projeto precisam ser salvas
                        if (itens[i].GrupoMedidaProjeto != null)
                            foreach (GrupoMedidaProjeto gmp in itens[i].GrupoMedidaProjeto)
                            {
                                // Busca o grupo de medida de projeto pelo id e pela descrição
                                busca = GrupoMedidaProjetoDAO.Instance.FindByDescricao(transaction, gmp.IdGrupoMedProj, gmp.Descricao);

                                // Verifica se o grupo já existe no banco, se existir, não precisa adicionar na lista para inserir
                                uint atual = gmp.IdGrupoMedProj;
                                if (dados[i].grupoMedidasProjeto.ContainsKey(atual))
                                    continue;

                                uint idGrupoMedProj;
                                if (busca == null || busca == 0)
                                {
                                    gmp.IdGrupoMedProj = 0;
                                    idGrupoMedProj = GrupoMedidaProjetoDAO.Instance.Insert(transaction, gmp);
                                }
                                else
                                    idGrupoMedProj = busca.Value;

                                dados[i].grupoMedidasProjeto.Add(atual, idGrupoMedProj);
                            }

                        #endregion

                        #region Salva os tipos de medidas do projeto

                        // Verifica quais medidas de projeto precisam ser salvas
                        if (itens[i].MedidaProjeto != null)
                            foreach (MedidaProjeto m in itens[i].MedidaProjeto)
                            {
                                // Busca a medida de projeto pelo id e pela descricao
                                busca = MedidaProjetoDAO.Instance.FindByDescricao(transaction, m.IdMedidaProjeto, m.Descricao);

                                // Verifica se a medida já existe no banco, se existir, não precisa adiconar na lista para inserir
                                uint atual = m.IdMedidaProjeto;
                                if (dados[i].medidasProjeto.ContainsKey(atual) && busca == atual)
                                    continue;

                                uint idMedidaProjeto;
                                if (busca == null || busca == 0)
                                {
                                    m.IdMedidaProjeto = 0;
                                    // Se a MedidaProjeto tiver idGrupoMedidaProjeto e se dados[i].grupoMedidasProjeto tiver outro idGrupoMedidaProjeto para a MedidaProjeto, altera o id.
                                    // Se não, a MedidaProjeto não tem Grupo associado, ou tem um grupo padrão, que tem o mesmo idGrupoMedidaProjeto em todos os sistemas, e não deve ser alterado
                                    if (m.IdGrupoMedProj.GetValueOrDefault() > 0 && dados[i].grupoMedidasProjeto.ContainsKey(m.IdGrupoMedProj.GetValueOrDefault()))
                                        m.IdGrupoMedProj = dados[i].grupoMedidasProjeto[(uint)m.IdGrupoMedProj];

                                    idMedidaProjeto = MedidaProjetoDAO.Instance.Insert(transaction, m);
                                }
                                else
                                    idMedidaProjeto = busca.Value;

                                dados[i].medidasProjeto.Add(atual, idMedidaProjeto);
                            }

                        #endregion

                        #region Salva as medidas do projeto

                        if (itens[i].MedidaProjetoModelo != null)
                            foreach (MedidaProjetoModelo m in itens[i].MedidaProjetoModelo)
                            {
                                uint atual = m.IdMedidaProjetoModelo;
                                m.IdMedidaProjetoModelo = 0;
                                m.IdMedidaProjeto = dados[i].medidasProjeto[m.IdMedidaProjeto];
                                m.IdProjetoModelo = dados[i].idProjetoModelo;
                                uint idMedidaProjetoModelo = MedidaProjetoModeloDAO.Instance.Insert(transaction, m);

                                dados[i].medidasProjetoModelo.Add(atual, idMedidaProjetoModelo);
                            }

                        #endregion

                        #region Salva os produtos do projeto

                        if (itens[i].ProdutoProjeto != null)
                            foreach (ProdutoProjeto p in itens[i].ProdutoProjeto)
                            {
                                if (p == null)
                                    continue;

                                uint atual = p.IdProdProj;
                                if (dados[i].produtosProjeto.ContainsKey(atual))
                                    continue;

                                busca = ProdutoProjetoDAO.Instance.FindByCodInterno(transaction, p.CodInterno);
                                uint idProdProj;
                                if (busca == null || busca == 0)
                                {
                                    p.IdProdProj = 0;
                                    idProdProj = ProdutoProjetoDAO.Instance.Insert(transaction, p);
                                }
                                else
                                    idProdProj = busca.Value;

                                dados[i].produtosProjeto.Add(atual, idProdProj);
                            }

                        #endregion

                        #region Salva os materiais do projeto

                        if (itens[i].MaterialProjetoModelo != null)
                            foreach (MaterialProjetoModelo m in itens[i].MaterialProjetoModelo)
                            {
                                if (!dados[i].produtosProjeto.ContainsKey(m.IdProdProj))
                                    continue;

                                uint atual = m.IdMaterProjMod;
                                m.IdMaterProjMod = 0;
                                m.IdProjetoModelo = dados[i].idProjetoModelo;
                                m.IdProdProj = dados[i].produtosProjeto[m.IdProdProj];
                                uint idMaterProjMod = MaterialProjetoModeloDAO.Instance.Insert(transaction, m);

                                dados[i].materiaisProjetoModelo.Add(atual, idMaterProjMod);
                            }

                        #endregion

                        #region Salva os arquivos de mesa

                        if (itens[i].ArquivoMesaCorte != null && importarArquivoMesaCorte)
                        {
                            var contadorArquivoCalcEngine = -1;

                            foreach (ArquivoMesaCorte amc in itens[i].ArquivoMesaCorte)
                            {
                                contadorArquivoCalcEngine++;

                                uint atual = (uint)amc.IdArquivoCalcEngine;
                                int idArquivoMesaCorteAtual = amc.IdArquivoMesaCorte;

                                if (dados[i].arquivoMesaCorte.ContainsKey(idArquivoMesaCorteAtual))
                                    continue;

                                var arquivoCalcEngine = itens[i].ArquivoCalcEngine.FirstOrDefault(f => f.IdArquivoCalcEngine == amc.IdArquivoCalcEngine);
                                busca = ArquivoCalcEngineDAO.Instance.FindByNome(transaction, arquivoCalcEngine.IdArquivoCalcEngine, arquivoCalcEngine.Nome);

                                ArquivoMesaCorte arquivoMesaCorteAtual = null;
                                if (busca == null || busca == 0)
                                {
                                    arquivoCalcEngine.IdArquivoCalcEngine = 0;
                                    var idArquivoCalcEngine = ArquivoCalcEngineDAO.Instance.Insert(transaction, arquivoCalcEngine);
                                    amc.IdArquivoMesaCorte = 0;
                                    amc.IdArquivoCalcEngine = (int)idArquivoCalcEngine;
                                    busca = idArquivoCalcEngine;
                                    ArquivoMesaCorteDAO.Instance.Insert(transaction, amc);

                                    //Verificar ne ArquivoCalcEngineVariavel onde IdArquivoCalcEngine antigo  e salvar novos.
                                    var arquivoCalcEngineVariavel = itens[i].ArquivosCalcEngineVariavel.Where(f => f.IdArquivoCalcEngine == atual);
                                    var arquivoCalcEngineVariavelInserido = new List<uint>();

                                    foreach (var v in arquivoCalcEngineVariavel)
                                    {
                                        /* Chamado 55039. */
                                        if (!arquivoCalcEngineVariavelInserido.Contains(v.IdArquivoCalcEngineVar))
                                            arquivoCalcEngineVariavelInserido.Add(v.IdArquivoCalcEngineVar);
                                        else
                                            continue;

                                        v.IdArquivoCalcEngineVar = 0;
                                        v.IdArquivoCalcEngine = idArquivoCalcEngine;
                                        ArquivoCalcEngineVariavelDAO.Instance.Insert(transaction, v);
                                    }
                                    arquivoMesaCorteAtual = amc;

                                    using (FileStream f = File.Create(ProjetoConfig.CaminhoSalvarCalcEngine + arquivoCalcEngine.Nome + ".calcpackage"))
                                    {
                                        var arq = itens[i].ArquivoCalcPackage[contadorArquivoCalcEngine];
                                        f.Write(arq, 0, arq.Length);
                                        f.Flush();
                                    }
                                    // Define se o arquivo mesa corte foi adicionado ou se já existia no banco.
                                    // Necessário para saber se o arquivo deve ser excluida se der algum problema na importação.
                                    dados[i].arquivoMesaCorteNovo.Add(arquivoMesaCorteAtual.IdArquivoMesaCorte, true);
                                }
                                else
                                    arquivoMesaCorteAtual = ArquivoMesaCorteDAO.Instance.ObterPeloArquivoCalcEngine(transaction, (uint)busca);

                                if (busca.GetValueOrDefault() == 0)
                                    throw new Exception("Não foi possível inserir o arquivo de mesa.");

                                dados[i].arquivoCalcEngine.Add((int)atual, (int)busca);
                                dados[i].arquivoMesaCorte.Add(idArquivoMesaCorteAtual, arquivoMesaCorteAtual.IdArquivoMesaCorte);
                            }
                        }

                        #endregion

                        #region Salva as peças do projeto

                        if (itens[i].PecaProjetoModelo != null)
                            foreach (PecaProjetoModelo p in itens[i].PecaProjetoModelo)
                            {
                                uint atual = p.IdPecaProjMod;
                                p.IdPecaProjMod = 0;
                                p.IdProjetoModelo = dados[i].idProjetoModelo;

                                if (p.IdArquivoMesaCorte > 0 && dados[i].arquivoMesaCorte.ContainsKey((int)p.IdArquivoMesaCorte))
                                    p.IdArquivoMesaCorte = (uint)dados[i].arquivoMesaCorte[(int)p.IdArquivoMesaCorte];
                                else
                                    p.IdArquivoMesaCorte = null;

                                uint idPecaProjMod = PecaProjetoModeloDAO.Instance.Insert(transaction, p);

                                dados[i].pecasModeloProjeto.Add(atual, idPecaProjMod);
                            }

                        #endregion

                        #region Salva as validações das peças do projeto

                        if (itens[i].ValidacaoPecaModelo != null && importarRegraValidacao)
                            foreach (var validacao in itens[i].ValidacaoPecaModelo)
                            {
                                var atual = validacao.IdValidacaoPecaModelo;
                                validacao.IdValidacaoPecaModelo = 0;
                                validacao.IdPecaProjMod = (int)dados[i].pecasModeloProjeto[(uint)validacao.IdPecaProjMod];
                                var idValidacaoPecaModelo = ValidacaoPecaModeloDAO.Instance.Insert(transaction, validacao);

                                dados[i].validacaoPecaModelo.Add(atual, validacao.IdValidacaoPecaModelo);
                            }

                        #endregion

                        #region Salva as posições das peças

                        if (itens[i].PosicaoPecaModelo != null)
                            foreach (PosicaoPecaModelo p in itens[i].PosicaoPecaModelo)
                            {
                                uint atual = p.IdPosicaoPecaModelo;
                                p.IdPosicaoPecaModelo = 0;
                                p.IdProjetoModelo = dados[i].idProjetoModelo;
                                uint idPosicaoPecaModelo = PosicaoPecaModeloDAO.Instance.Insert(transaction, p);

                                dados[i].posicoesPecaModelo.Add(atual, idPosicaoPecaModelo);
                            }

                        #endregion

                        #region Salva as posições das peças individuais

                        if (itens[i].PosicaoPecaIndividual != null)
                            foreach (PosicaoPecaIndividual p in itens[i].PosicaoPecaIndividual)
                            {
                                if (p.IdPecaProjMod == 0)
                                    continue;

                                uint atual = p.IdPosPecaInd;
                                p.IdPosPecaInd = 0;
                                p.IdPecaProjMod = dados[i].pecasModeloProjeto[p.IdPecaProjMod];
                                uint idPosicaoPecaIndividual = PosicaoPecaIndividualDAO.Instance.Insert(transaction, p);

                                dados[i].posicoesPecaIndividual.Add(atual, idPosicaoPecaIndividual);
                            }

                        #endregion

                        #region Salva a fórmula de expressão de cálculo

                        if (itens[i].FormulaExpressaoCalculo != null && importarFormulaExpressaoCalculo)
                            foreach (var fec in itens[i].FormulaExpressaoCalculo)
                            {
                                uint atual = fec.IdFormulaExpreCalc;
                                var idFormula = FormulaExpressaoCalculoDAO.Instance.ObterIdFormulaPelaDescricao(transaction, fec.Descricao);

                                if (idFormula > 0)
                                {
                                    var expressao = FormulaExpressaoCalculoDAO.Instance.GetElementByPrimaryKey(transaction, (uint)idFormula);
                                    expressao.Expressao = fec.Expressao;

                                    // Atualiza a expressão de cálculo da fórmula.
                                    FormulaExpressaoCalculoDAO.Instance.Update(transaction, expressao);
                                }
                                else
                                {
                                    fec.IdFormulaExpreCalc = 0;
                                    idFormula = (int)FormulaExpressaoCalculoDAO.Instance.InserirPorImportacaoProjeto(transaction, fec);
                                    // Só salva em Dados se o fórmula for inserida
                                    dados[i].formulaExpressaoCalculo.Add(atual, (uint)idFormula);
                                }
                            }

                        #endregion

                        #region Salva Tipo Flag Arquivo de Mesa

                        if (itens[i].FlagArqMesa != null && importarFlag)
                            foreach (var f in itens[i].FlagArqMesa)
                            {
                                if (f == null)
                                    continue;

                                //Busca a FlagArqMesa pela descrição
                                busca1 = FlagArqMesaDAO.Instance.FindByDescricao(transaction, f.IdFlagArqMesa, f.Descricao);

                                //Verifica se a Flag já existe no banco, se existir, não precisa adiconar e pode continuar
                                int atual = f.IdFlagArqMesa;
                                if (dados[i].flagArqMesa.ContainsKey(atual))
                                    continue;

                                int idFlagArqMesa;
                                if (busca1 == null || busca1 == 0)
                                {
                                    f.IdFlagArqMesa = 0;
                                    idFlagArqMesa = (int)FlagArqMesaDAO.Instance.Insert(transaction, f);
                                }
                                else
                                    idFlagArqMesa = busca1.Value;

                                dados[i].flagArqMesa.Add(atual, idFlagArqMesa);
                            }

                        #endregion

                        #region Salva Flag Arquivo de Mesa

                        if (itens[i].FlagArqMesaPecaProjMod != null && importarFlag)
                            foreach (var fm in itens[i].FlagArqMesaPecaProjMod)
                            {
                                if (fm == null || !dados[i].flagArqMesa.ContainsKey(fm.IdFlagArqMesa))
                                    continue;

                                fm.IdFlagArqMesa = dados[i].flagArqMesa[fm.IdFlagArqMesa];
                                fm.IdPecaProjMod = (int)dados[i].pecasModeloProjeto[(uint)fm.IdPecaProjMod];
                                FlagArqMesaPecaProjModDAO.Instance.Insert(transaction, fm);
                            }

                        #endregion

                        #region Salva as Flag Arquivo de Mesa associas ao arquivo CalcEngine

                        if (itens[i].FlagsArqMesaArqCalcEngine != null && importarFlag)
                            foreach (var fm in itens[i].FlagsArqMesaArqCalcEngine)
                            {
                                if (fm == null || !dados[i].flagArqMesa.ContainsKey(fm.IdFlagArqMesa))
                                    continue;

                                fm.IdFlagArqMesa = dados[i].flagArqMesa[fm.IdFlagArqMesa];
                                fm.IdArquivoCalcEngine = dados[i].arquivoCalcEngine[fm.IdArquivoCalcEngine];
                                FlagArqMesaArqCalcEngineDAO.Instance.InsereSeNaoExistir(transaction, fm);
                            }

                        #endregion

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        // Salva o erro
                        var url = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : null;
                        ErroDAO.Instance.InserirFromException(url, new ImportarProjetoException(dados[i].codigoModelo, ex));
                        naoCadastrados.Add(i);
                        mensagemErro = ex.Message;
                    }
            }

            var retorno = new ResultadoSalvar();
            retorno.Inseridos = string.Empty;
            retorno.NaoInseridos = string.Empty;
            
            for (var i = 0; i < dados.Count; i++)
            {
                if (naoCadastrados.Contains(i))
                    retorno.NaoInseridos += string.Format(", {0}", dados[i].codigoModelo);
                else
                    retorno.Inseridos += string.Format(", {0}", dados[i].codigoModelo);
            }

            retorno.Inseridos = retorno.Inseridos.TrimEnd(' ', ',');
            retorno.NaoInseridos = retorno.NaoInseridos.TrimEnd(' ', ',');

            return retorno;
        }

        #endregion
    }
}