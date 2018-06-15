using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;
using System.Web;
using System.Linq;
using Glass.Global.UI.Web.Process.Ferragem;
using Colosoft.Business;
using Colosoft;

namespace Glass.Projeto.Negocios.Componentes
{
    public class ExportacaoImportacaoFerragem : IExportacaoImportacaoFerragem
    {
        #region Classe de Suporte

        /// <summary>
        /// Classe criada para retornar o nome das ferragens que foram ou não importadas.
        /// </summary>
        private class ResultadoImportacao
        {
            public List<string> FerragensImportadas = new List<string>();
            public List<string> FerragensNaoImportadas = new List<string>();
            public List<string> MensagensErro = new List<string>();
        }

        /// <summary>
        /// Classe utilizada para exportar/importar todos os dados de uma ferragem.
        /// </summary>
        [Serializable]
        [XmlRoot("exportarFerragem")]
        public class ExportarFerragem
        {
            #region Item

            /// <summary>
            /// Classe criada para exportar/importar os dados da ferragem.
            /// </summary>
            [Serializable]
            [XmlRoot("item")]
            public class Item
            {
                #region Construtores

                /// <summary>
                /// Construtor padrão.
                /// </summary>
                public Item() { }

                /// <summary>
                /// Construtor de inicialização.
                /// </summary>
                public Item(int idFerragem)
                {
                    #region Recuperação da ferragem

                    // Recupera o fluxo de ferragem.
                    var ferragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFerragemFluxo>();
                    // Recupera a ferragem informada por parâmetro.
                    var ferragem = ferragemFluxo.ObterFerragem(idFerragem);

                    // Não prossegue caso a ferragem não seja recuperada.
                    if (ferragem == null || ferragem.IdFerragem <= 0)
                        throw new Exception(string.Format("Não foi possível recuperar a ferragem para a exportação. ID da ferragem: {0}.", idFerragem));

                    #endregion

                    #region Declaração de variáveis

                    // Caminho da imagem da ferragem no servidor.
                    var caminhoImagemFerragem = string.Empty;
                    // Caminho do CalcPackage da ferragem.
                    var caminhoCalcPackage = string.Empty;

                    #endregion

                    #region Inicialização das propriedades

                    // A exportação envia a model de ferragem, portanto, recupera os dados da model, através da entidade de ferragem.
                    Ferragem = ferragem.DataModel;
                    // Códigos da ferragem.
                    CodigosFerragem = new List<CodigoFerragem>();
                    // Constantes da ferragem.
                    ConstantesFerragem = new List<ConstanteFerragem>();
                    // Fabricante da ferragem.
                    FabricanteFerragem = new FabricanteFerragem();

                    #endregion

                    #region Recupera os dados

                    // Verifica se a ferragem possui Códigos, se sim, os adiciona na propriedade de exportação de Códigos de ferragem.
                    if (ferragem.Codigos != null && ferragem.Codigos.Count > 0)
                        CodigosFerragem.AddRange(ferragem.Codigos.Select(f => f.DataModel).ToList());

                    // Verifica se a ferragem possui Constantes, se sim, os adiciona na propriedade de exportação de Constantes de ferragem.
                    if (ferragem.Constantes != null && ferragem.Constantes.Count > 0)
                        ConstantesFerragem.AddRange(ferragem.Constantes.Select(f => f.DataModel).ToList());

                    // Verifica se a ferragem possui Fabricante, se sim, o adiciona na propriedade de exportação de Fabricante de ferragem.
                    if (ferragem.Fabricante != null && ferragem.Fabricante.IdFabricanteFerragem > 0)
                        FabricanteFerragem = ferragem.Fabricante.DataModel;

                    // Recupera a imagem da ferragem, para exportá-la.
                    if (FerragemRepositorioImagens.Instance.PossuiImagem(ferragem.IdFerragem))
                    {
                        caminhoImagemFerragem = FerragemRepositorioImagens.Instance.ObterCaminho(ferragem.IdFerragem);

                        // Verifica se a imagem da ferragem existe.
                        if (File.Exists(caminhoImagemFerragem))
                            ImagemFerragem = Data.Helper.Utils.GetImageFromFile(caminhoImagemFerragem);
                    }

                    // Recupera o arquivo CalcPackage, para exportá-lo.
                    if (FerragemRepositorioCalcPackage.Instance.PossuiCalcPackage(ferragem.IdFerragem))
                    {
                        caminhoCalcPackage = FerragemRepositorioCalcPackage.Instance.ObterCaminho(ferragem.IdFerragem);

                        // Verifica se o arquivo CalcPackage existe.
                        if (File.Exists(caminhoCalcPackage))
                            using (FileStream f = File.OpenRead(caminhoCalcPackage))
                            {
                                ArquivoCalcPackage = new byte[f.Length];
                                // Salva os dados do CalcPackage da ferragem na propriedade de exportação do arquivo CalcPackage.
                                f.Read(ArquivoCalcPackage, 0, ArquivoCalcPackage.Length);
                                f.Flush();
                            }
                    }

                    #endregion
                }

                #endregion

                #region Propriedades

                /// <summary>
                /// Model de ferragem.
                /// </summary>
                [XmlElement("ferragem")]
                public Ferragem Ferragem;

                /// <summary>
                /// Listagem com a model dos códigos da ferragem.
                /// </summary>
                [XmlElement("codigosFerragem")]
                public List<CodigoFerragem> CodigosFerragem;

                /// <summary>
                /// Listagem com a model das constantes da ferragem.
                /// </summary>
                [XmlElement("constantesFerragem")]
                public List<ConstanteFerragem> ConstantesFerragem;

                /// <summary>
                /// Model do fabricante da ferragem.
                /// </summary>
                [XmlElement("fabricanteFerragem")]
                public FabricanteFerragem FabricanteFerragem;

                /// <summary>
                /// Imagem da ferragem.
                /// </summary>
                [XmlElement("imagemFerragem")]
                public byte[] ImagemFerragem;

                /// <summary>
                /// Arquivo CalcPackage da ferragem.
                /// </summary>
                [XmlElement("arquivoCalcPackage")]
                public byte[] ArquivoCalcPackage;

                #endregion
            }

            #endregion

            #region Itens

            private List<Item> _itens = new List<Item>();

            [XmlElement("item")]
            public List<Item> Itens
            {
                get { return _itens; }
            }

            #endregion
        }

        #endregion

        #region Métodos de serialização

        /// <summary>
        /// Método chamado ao importar uma ferragem, o arquivo recebido é tratado e a classe de exportação, com os dados da ferragem, é retornada.
        /// </summary>
        private static ExportarFerragem Deserializar(byte[] arquivo)
        {
            ExportarFerragem retorno;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportarFerragem));

            using (MemoryStream m = new MemoryStream(arquivo))
            {
                retorno = xmlSerializer.Deserialize(m) as ExportarFerragem;
            }

            return retorno;
        }

        /// <summary>
        /// Método chamado ao exportar uma ferragem, os dados da ferragem são serializados para serem salvos em um arquivo de exportação.
        /// </summary>
        private static byte[] Serializar(ExportarFerragem ferragem)
        {
            byte[] retorno;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportarFerragem));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, ferragem);
                memoryStream.Position = 0;

                using (BinaryReader r = new BinaryReader(memoryStream))
                {
                    retorno = r.ReadBytes((int)memoryStream.Length);
                }
            }

            return retorno;
        }

        #endregion

        #region Exportação

        /// <summary>
        /// Exporta as ferragens selecionadas para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        public byte[] Exportar(List<int> idsFerragem)
        {
            // Variável criada para salvar os dados de cada ferragem que será exportada.
            var exportacaoFerragem = new ExportarFerragem();

            // Verifica se foi informada alguma ferragem para a exportação.
            if (idsFerragem == null || !idsFerragem.Any(f => f > 0))
            {
                throw new Exception("Selecione pelo menos uma ferragem para exportar.");
            }

            // Percorre cada id de ferragem e cria um item de exportação, de ferragem, para cada uma delas.
            foreach (var idFerragem in idsFerragem)
            {
                exportacaoFerragem.Itens.Add(new ExportarFerragem.Item(idFerragem));
            }

            // Caso nenhum item de exportação seja criado, não prossegue com a exportação.
            if (exportacaoFerragem.Itens.Count == 0)
            {
                throw new Exception("Selecione pelo menos uma ferragem para exportar.");
            }

            // Serializa os dados e gera um arquivo compactado.
            return Arquivos.Compactar(Serializar(exportacaoFerragem));
        }

        #endregion

        #region Importação

        /// <summary>
        /// Lê o arquivo de exportação e salva as ferragens contidas nele.
        /// </summary>
        public SaveResult Importar(byte[] arquivo, bool substituirFerragemExistente)
        {
            #region Declaração de variáveis
            
            // Variável onde será salvo o resultado da importação do arquivo de exportação de ferragem.
            var resultadoImportacao = new ResultadoImportacao();
            // Variável onde será criado o texto de retorno da importação, com as ferragens que foram ou não importadas.
            var retornoImportacao = string.Empty;

            #endregion

            #region Tradução e inserção dos dados de exportação

            try
            {
                // Descompacta e deserializa os dados do arquivo de exportação.
                // Insere, no WebGlass, as ferragens informadas no arquivo de exportação.
                resultadoImportacao = SalvarItens(Deserializar(Arquivos.Descompactar(arquivo)).Itens, substituirFerragemExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível importar o arquivo. Verifique se é um arquivo válido de ferragens do WebGlass.", ex);
            }

            #endregion

            #region Retorno do resultado importação

            // Verifica se alguma ferragem não foi importadas.
            if (resultadoImportacao.FerragensNaoImportadas.Count > 0)
            {
                // Caso alguma ferragem tenha sido importadas, informa o nome delas na mensagem de erro, junto com o nome das ferragens que não foram importadas.
                if (resultadoImportacao.FerragensImportadas.Count > 0)
                {
                    retornoImportacao = $@"Algumas ferragens não foram importadas. Verifique se elas já existem.
                        \nFerragens importadas: { string.Join(", ", resultadoImportacao.FerragensImportadas) }
                        \nFerragens não importadas: { string.Join(", ", resultadoImportacao.FerragensNaoImportadas) }
                        \n{ string.Join(", ", resultadoImportacao.MensagensErro) }";
                }
                // Monta uma mensagem de erro informando quais ferragens não foram importadas.
                else
                {
                    retornoImportacao = $@"Não foi possível importar as ferragens.
                        \nFerragens não importadas: { string.Join(", ", resultadoImportacao.FerragensNaoImportadas) }
                        \n{ string.Join(", ", resultadoImportacao.MensagensErro) }";
                }

                return new SaveResult(false, retornoImportacao.GetFormatter());
            }
            // Salva uma mensagem de sucesso, informando o nome das ferragens importadas.
            else
            {
                retornoImportacao = $"Importação realizada com sucesso! Ferragens importadas: { string.Join(", ", resultadoImportacao.FerragensImportadas) }.";
                return new SaveResult(true, retornoImportacao.GetFormatter());
            }

            #endregion
        }

        #region Inserção dos itens de exportação de ferragem

        /// <summary>
        /// Método criado para a importação da ferragem. Insere as ferragens do arquivo, no WebGlass.
        /// </summary>
        private static ResultadoImportacao SalvarItens(List<ExportarFerragem.Item> itens, bool substituirFerragemExistente)
        {
            #region Declaração de variáveis
            
            var ferragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFerragemFluxo>();
            var ferragemId = new List<Tuple<int, Entidades.Ferragem>>();
            var fabricanteFerragemId = new List<Tuple<int, Entidades.FabricanteFerragem>>();
            var fabricantesFerragemInseridos = new List<Entidades.FabricanteFerragem>();
            var repositorioImagemFerragem = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Entidades.IFerragemRepositorioImagens>();
            var repositorioCalcPackageFerragem = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Entidades.IFerragemRepositorioCalcPackage>();
            var resultadoImportacao = new ResultadoImportacao();

            #endregion

            #region Recuperação dos objetos que devem ser atualizados ao invés de serem inseridos

            // Caso as ferragens devam ser substituídas, busca todas as ferragens, do arquivo de exportação, que existem no sistema de importação.
            if (substituirFerragemExistente)
            {
                // Recupera todas as ferragens, do sistema atual, com base no nome delas e no nome do fabricante associado.
                ferragemId = SourceContext.Instance.CreateQuery()
                    .Select("f.IdFerragem, f.Nome, f.IdFabricanteFerragem")
                    .From<Ferragem>("f")
                    .Where(string.Join(" OR ", itens?.Select(f => string.Format("f.Nome='{0}'", f.Ferragem.Nome))))
                    // Seleciona os IDs de ferragem do arquivo de exportação, junto com as entidades de ferragens (correspondentes) no sistema de importação.
                    .Execute()?.Select(f => new Tuple<int, Entidades.Ferragem>((itens.FirstOrDefault(g => g.Ferragem.Nome == f.GetString(1))?.Ferragem.IdFerragem).GetValueOrDefault(),
                        ferragemFluxo.ObterFerragem(f.GetInt32(0)))).Distinct().ToList();
            }

            // O fabricante deve ser sempre atualizado, caso exista no sistema de importação. Independentemente da existência da ferragem.
            if (itens.Any(f => f.FabricanteFerragem?.IdFabricanteFerragem > 0))
            {
                // No sistema onde o arquivo está sendo importado, recupera os fabricantes que possuem o mesmo nome dos fabricantes inseridos no arquivo de exportação.
                fabricanteFerragemId = SourceContext.Instance.CreateQuery()
                    .Select("ff.IdFabricanteFerragem, ff.Nome")
                    .From<FabricanteFerragem>("ff")
                    .Where(string.Join(" OR ", itens?.Select(f => string.Format("ff.Nome='{0}'", f.FabricanteFerragem.Nome))))
                    // Seleciona os IDs de fabricante de ferragem do arquivo de exportação, junto com as entidades de fabricantes de ferragens (correspondentes) no sistema de importação.
                    .Execute()?.Select(f => new Tuple<int, Entidades.FabricanteFerragem>((itens.FirstOrDefault(g => g.FabricanteFerragem.Nome == f.GetString(1))?.FabricanteFerragem.IdFabricanteFerragem).GetValueOrDefault(),
                        ferragemFluxo.ObterFabricanteFerragem(f.GetInt32(0)))).Distinct().ToList();
            }

            #endregion

            // Percorre cada item, do arquivo de exportação, e os insere no sistema.
            for (var i = 0; i < itens.Count(); i++)
            {
                try
                {
                    #region Declaração de variáveis

                    // Entidade de ferragem.
                    Entidades.Ferragem ferragem = null;
                    // Entidade de fabricante de ferragem.
                    Entidades.FabricanteFerragem fabricanteFerragem = null;

                    #endregion

                    #region Recuperação/criação da ferragem

                    // Verifica se a ferragem existe no sistema atual.
                    if (ferragemId != null && ferragemId.Any(f => f.Item1 == itens[i].Ferragem.IdFerragem))
                    {
                        ferragem = ferragemId.FirstOrDefault(f => f.Item1 == itens[i].Ferragem.IdFerragem).Item2;
                    }
                    // Cria uma nova ferragem.
                    else
                    {
                        ferragem = ferragemFluxo.CriarFerragem();
                    }

                    #region Propriedades ferragem

                    // Define/atualiza os dados da ferragem, com base no arquivo de importação.
                    ferragem.Altura = itens[i].Ferragem.Altura;
                    ferragem.EstiloAncoragem = itens[i].Ferragem.EstiloAncoragem;
                    ferragem.Largura = itens[i].Ferragem.Largura;
                    ferragem.MedidasEstaticas = itens[i].Ferragem.MedidasEstaticas;
                    ferragem.Nome = itens[i].Ferragem.Nome;
                    ferragem.PodeEspelhar = itens[i].Ferragem.PodeEspelhar;
                    ferragem.PodeRotacionar = itens[i].Ferragem.PodeRotacionar;
                    ferragem.Situacao = itens[i].Ferragem.Situacao;
                    ferragem.UUID = itens[i].Ferragem.UUID;

                    #endregion

                    #region Fabricante ferragem

                    // Verifica se o fabricante existe no sistema atual.
                    if (fabricanteFerragemId?.Any(f => f.Item1 == itens[i].FabricanteFerragem.IdFabricanteFerragem) ?? false)
                    {
                        // Recupera o fabricante. Os dados não devem ser atualizados.
                        fabricanteFerragem = fabricanteFerragemId.FirstOrDefault(f => f.Item1 == itens[i].FabricanteFerragem.IdFabricanteFerragem).Item2;
                    }
                    else if (fabricantesFerragemInseridos.Any(f => f.Nome.ToLowerInvariant() == itens[i].FabricanteFerragem.Nome.ToLowerInvariant()))
                    {
                        fabricanteFerragem = fabricantesFerragemInseridos.FirstOrDefault(f => f.Nome.ToLowerInvariant() == itens[i].FabricanteFerragem.Nome.ToLowerInvariant());
                    }
                    else
                    {
                        // Cria um novo fabricante.
                        fabricanteFerragem = new Entidades.FabricanteFerragem();
                        fabricanteFerragem.Nome = itens[i].FabricanteFerragem.Nome;
                        fabricanteFerragem.Sitio = itens[i].FabricanteFerragem.Sitio;

                        // Insere/atualiza o fabricante.
                        var retornoFabricanteFerragem = ferragemFluxo.SalvarFabricanteFerragem(fabricanteFerragem);

                        // Caso ocorra algum erro na atualização do fabricante, a ferragem não deve ser inserida.
                        if (!retornoFabricanteFerragem)
                        {
                            throw new Exception(retornoFabricanteFerragem.Message.ToString());
                        }
                        else
                        {
                            fabricantesFerragemInseridos.Add(fabricanteFerragem);
                        }
                    }

                    // Atualiza a referência do fabricante na ferragem.
                    ferragem.IdFabricanteFerragem = fabricanteFerragem.IdFabricanteFerragem;

                    #endregion

                    #region Código ferragem

                    // Caso a ferragem exista e esteja sendo atualizada, os códigos devem ser removidos e adicionados novamente.
                    ferragem.Codigos.Clear();

                    // Seta os códigos da ferragem.
                    for (var j = 0; j < itens[i].CodigosFerragem.Count(); j++)
                    {
                        var codigoFerragem = new Entidades.CodigoFerragem();
                        codigoFerragem.IdFerragem = ferragem.IdFerragem;
                        codigoFerragem.Codigo = itens[i].CodigosFerragem[j].Codigo;

                        ferragem.Codigos.Add(codigoFerragem);
                    }

                    #endregion

                    #region Constante ferragem

                    // Caso a ferragem exista e esteja sendo atualizada, as constantes devem ser removidas e adicionadas novamente.
                    ferragem.Constantes.Clear();

                    // Seta as constantes da ferragem.
                    for (var j = 0; j < itens[i].ConstantesFerragem.Count(); j++)
                    {
                        var constanteFerragem = new Entidades.ConstanteFerragem();
                        constanteFerragem.IdFerragem = ferragem.IdFerragem;
                        constanteFerragem.Nome = itens[i].ConstantesFerragem[j].Nome;
                        constanteFerragem.Valor = itens[i].ConstantesFerragem[j].Valor;

                        ferragem.Constantes.Add(constanteFerragem);
                    }

                    #endregion

                    #endregion

                    #region Inserção da imagem

                    // Verifica se a ferragem exportada possui imagem.
                    if (itens[i].ImagemFerragem != null)
                    {
                        EventHandler<Colosoft.Business.EntitySavedEventArgs> ferragemSalva = null;
                        var imagemStream = new MemoryStream(itens[i].ImagemFerragem, 0, itens[i].ImagemFerragem.Length);

                        ferragemSalva = (sender, args) =>
                        {
                            try
                            {
                                // Salva a imagem da ferragem.
                                repositorioImagemFerragem.SalvarImagem(ferragem.IdFerragem, imagemStream);
                            }
                            finally
                            {
                                ferragem.Saved -= ferragemSalva;
                            }
                        };

                        ferragem.Saved += ferragemSalva;
                    }

                    #endregion

                    #region Inserção do arquivo CalcPackage

                    if (itens[i].ArquivoCalcPackage != null)
                    {
                        CalcEngine.Dxf.DxfProject projeto = null;
                        CalcEngine.ProjectFilesPackage pacote = null;

                        using (Stream pacoteStream = new MemoryStream(itens[i].ArquivoCalcPackage, 0, itens[i].ArquivoCalcPackage.Length))
                        {
                            pacote = CalcEngine.ProjectFilesPackage.LoadPackage(pacoteStream);
                        }

                        try
                        {
                            projeto = CalcEngine.Dxf.DxfProject.LoadFromPackage(pacote);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Não foi possível carregar os dados do arquivo da ferragem de ID {0}.", itens[i].Ferragem.IdFerragem), ex);
                        }

                        EventHandler<Colosoft.Business.EntitySavedEventArgs> ferragemSalva = null;

                        // Configura o método anonimo para ser acionado quando os dados da ferragem forem salvos
                        ferragemSalva = (sender, args) =>
                        {
                            try
                            {

                                using (var stream = new MemoryStream())
                                {
                                    // Cria o pacote para onde serão salvo os dados do CalcPackage
                                    var package = CalcEngine.ProjectFilesPackage.CreatePackage(stream, false);
                                    projeto.Save(package);
                                    package.Close();

                                    stream.Position = 0;

                                    // Salva os dados no repositório das ferragens
                                    repositorioCalcPackageFerragem.SalvarCalcPackage(ferragem.IdFerragem, stream);
                                }
                            }
                            finally
                            {
                                ferragem.Saved -= ferragemSalva;
                            }
                        };

                        ferragem.Saved += ferragemSalva;
                    }

                    #endregion

                    #region Inserção da ferragem

                    // Insere/atualiza a ferragem.
                    var retornoSalvarFerragem = ferragemFluxo.SalvarFerragem(ferragem);

                    if (!retornoSalvarFerragem)
                    {
                        throw new Exception(retornoSalvarFerragem.Message.ToString());
                    }

                    #endregion

                    // Salva, na variável de retorno, o nome da ferragem importada.
                    resultadoImportacao.FerragensImportadas.Add(itens[i].Ferragem.Nome);
                }
                catch (Exception ex)
                {
                    // Salva, na variável de retorno, o nome da ferragem não importada e a mensagem de erro.
                    resultadoImportacao.FerragensNaoImportadas.Add(itens[i].Ferragem.Nome);
                    resultadoImportacao.MensagensErro.Add(ex.Message);

                    ErroDAO.Instance.InserirFromException(HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : null, ex);
                }
            }

            return resultadoImportacao;
        }

        #endregion

        #endregion
    }
}