using Colosoft;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glass.Api.Host.Areas.App.Controllers
{
    /// <summary>
    /// Representa o resultado da finalização dos dados do projeto.
    /// </summary>
    public class FinalizarProjetoResultado : Colosoft.Business.OperationResult
    {
        #region Constructores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public FinalizarProjetoResultado(bool success, IMessageFormattable message)
            : base(success, message)
        {


        }

        #endregion
    }

    /// <summary>
    /// Representa o controller dos projetos do aplicativo.
    /// </summary>
    [Authorize]
    public class ProjetoController : ApiController
    {
        #region Métodos Públicos

        /// <summary>
        /// Realiza o cálculo das medidas de projeto.
        /// </summary>
        /// <param name="idTipoEntrega">Identificador do tipo de entrega.</param>
        /// <param name="itemProjeto">Dados do item de projeto.</param>
        /// <returns></returns>
        [HttpPost]
        [Colosoft.Web.Http.MultiPostParameters]
        public Glass.Api.Projeto.ItemProjeto CalcularMedidas(int idTipoEntrega, Glass.Api.Projeto.ItemProjeto itemProjeto)
        {
            var pecasProjMod = PecaProjetoModeloDAO.Instance.GetByModelo((uint)itemProjeto.IdProjetoModelo);
            var pecasMateriaisProjeto = ItemProjetoDAO.Instance.CriarPecasMateriaisProjeto(itemProjeto, pecasProjMod, itemProjeto.Pecas, itemProjeto.Medidas, idTipoEntrega, UserInfo.GetUserInfo.IdCliente.GetValueOrDefault());
            var medidasProjetoModelo = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(null, (uint)itemProjeto.IdProjetoModelo, true);

            itemProjeto.Pecas.Clear();
            itemProjeto.Pecas.AddRange(pecasMateriaisProjeto.PecasItemProjeto.Select(f => new Glass.Api.Projeto.PecaItemProjeto(f)));

            if (itemProjeto.Pecas.All(f => !f.IdProd.HasValue || f.IdProd.Value == 0))
                throw new Exception("Não foram encontrados vidros compatíveis com a dimensão e cor informados.");

            itemProjeto.Materiais.Clear();
            itemProjeto.Materiais.AddRange(pecasMateriaisProjeto.MateriaisItemProjeto.Select(f => new Glass.Api.Projeto.MaterialItemProjeto(f)));

            itemProjeto.PosicoesPeca.Clear();
            foreach (var p in PosicaoPecaModeloDAO.Instance.GetPosicoes((uint)itemProjeto.IdProjetoModelo))
            {
                p.IdItemProjeto = itemProjeto.IdItemProjeto;
                p.Valor = UtilsProjeto.CalcExpressao(null, p.Calc, itemProjeto, itemProjeto.Pecas, medidasProjetoModelo, itemProjeto.Medidas, null);

                itemProjeto.PosicoesPeca.Add(new Projeto.PosicaoPeca(p));
            }

            foreach (var peca in itemProjeto.Pecas)
            {
                foreach (var p in PosicaoPecaIndividualDAO.Instance.GetPosicoes(peca.IdPecaProjMod, peca.Item.StrParaInt()))
                {
                    p.IdPecaItemProj = peca.IdPecaItemProj;
                    p.Valor = UtilsProjeto.CalcExpressao(null, p.Calc, itemProjeto, itemProjeto.Pecas, medidasProjetoModelo, itemProjeto.Medidas, null);

                    peca.PosicoesPeca.Add(new Projeto.PosicaoPecaIndividual(p));
                }
            }


            return itemProjeto;
        }


        /// <summary>
        /// Finaliza o projeto.
        /// </summary>
        /// <param name="projeto">Dados do projeto que será finalizado.</param>
        [HttpPost]
        public async Task<FinalizarProjetoResultado> FinalizarProjeto()
        {
            Glass.Api.Projeto.Projeto projeto = null;

            var tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath());
            var multipartProvider = new MultipartFormDataStreamProvider(tempFolder);
            // Realiza a leitura as partes postadas
            await Request.Content.ReadAsMultipartAsync(multipartProvider);

            await multipartProvider.ExecutePostProcessingAsync();

            foreach (var content in multipartProvider.Contents)
            {
                var headers = content.Headers;
                var name = headers.ContentDisposition.Name;

                if (name == "\"projeto\"")
                {
                    using (var stream = await content.ReadAsStreamAsync())
                    {
                        var formatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                        projeto = (Glass.Api.Projeto.Projeto)(await formatter.ReadFromStreamAsync(typeof(Glass.Api.Projeto.Projeto), stream, null, null));
                    }
                    break;
                }
            }

            // Recupera a relação das imagens postadas
            var imagens = multipartProvider.FileData
                .Select(fileData =>
                {
                    var nome = fileData.Headers.ContentDisposition.Name?.TrimStart('"')?.TrimEnd('"');
                    return new ImagemProjeto(nome, null, fileData.LocalFileName);
                })
                .Where(f => f.Nome.StartsWith("img"))
                .OrderBy(f => f.Nome);

            //provider.Contents.Where(f => f.n)

            try
            {
                PedidoDAO.Instance.FinalizarProjetoGerarPedidoApp(projeto, imagens);
                return new FinalizarProjetoResultado(true, null);
            }
            catch (Exception x)
            {
                return new FinalizarProjetoResultado(false, x.Message.GetFormatter());
            }
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa um imagem do projeto.
        /// </summary>
        class ImagemProjeto : IImagemProjeto
        {
            #region Variáveis Locais

            private string _nome;
            private string _descricao;
            private string _localFileName;

            #endregion

            #region Propriedades

            /// <summary>
            /// Nome da imagem.
            /// </summary>
            public string Nome { get { return _nome; } }

            /// <summary>
            /// Descrição associada.
            /// </summary>
            public string Descricao { get { return _descricao; } }

            /// <summary>
            /// Abra o arquivo.
            /// </summary>
            /// <returns></returns>
            public System.IO.Stream Abrir()
            {
                return System.IO.File.OpenRead(_localFileName);
            }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="nome"></param>
            /// <param name="descricao"></param>
            /// <param name="localFileName"></param>
            public ImagemProjeto(string nome, string descricao, string localFileName)
            {
                _nome = nome;
                _descricao = descricao;
                _localFileName = localFileName;
            }

            #endregion
        }

        #endregion
    }
}