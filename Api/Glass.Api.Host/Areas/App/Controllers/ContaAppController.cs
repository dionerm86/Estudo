using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Glass.Api.Host.Areas.App.Controllers
{
    /// <summary>
    /// Controller da conta do aplicativo.
    /// </summary>
    public class ContaAppController : ApiController
    {
        #region Métodos Privados

        /// <summary>
        /// Recupera o proxy da cor do vidro.
        /// </summary>
        /// <param name="corVidro"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.CorVidro corVidro)
        {
            return new
            {
                IdCorVidro = corVidro.IdCorVidro,
                Descricao = corVidro.Descricao,
                Sigla = corVidro.Sigla
            };
        }

        /// <summary>
        /// Recupera o proxy do tipo de entrega.
        /// </summary>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.TipoEntrega tipoEntrega)
        {
            return new
            {
                IdTipoEntrega = tipoEntrega.IdTipoEntrega,
                Descricao = tipoEntrega.Descricao
            };
        }
        
        /// <summary>
        /// Recupera o proxy da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.UnidadeMedida unidadeMedida)
        {
            return new
            {
                IdUnidadeMedida = unidadeMedida.IdUnidadeMedida,
                Codigo = unidadeMedida.Codigo,
                Descricao = unidadeMedida.Descricao
            };
        }

        /// <summary>
        /// Recupera o proxy para o grupo de produtos.
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.GrupoProd grupo)
        {
            return new
            {
                IdGrupoProd = grupo.IdGrupoProd,
                Descricao = grupo.Descricao,
                TipoCalculo = grupo.TipoCalculo,
                TipoGrupo = grupo.TipoGrupo
            };
        }

        /// <summary>
        /// Recupera o proxy para o subgrupo de produtos.
        /// </summary>
        /// <param name="subgrupo"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.SubgrupoProd subgrupo)
        {
            return new
            {
                IdSubgrupoProd = subgrupo.IdSubgrupoProd,
                IdGrupoProd = subgrupo.IdGrupoProd,
                Descricao = subgrupo.Descricao,
                TipoCalculo = subgrupo.TipoCalculo,
                IdLoja = (int?)null,
                TipoSubgrupo = subgrupo.TipoSubgrupo,
                IsVidroTemperado = subgrupo.IsVidroTemperado
            };
        }

        /// <summary>
        /// Recupera o proxy para a etiqueta de aplicação.
        /// </summary>
        /// <param name="aplicacao"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.EtiquetaAplicacao aplicacao)
        {
            return new
            {
                IdAplicacao = aplicacao.IdAplicacao,
                CodInterno = aplicacao.CodInterno,
                Descricao = aplicacao.Descricao,
                Situacao = aplicacao.Situacao,
                TipoPedido = aplicacao.TipoPedido
            };
        }

        /// <summary>
        /// Recupera o proxy para etiquetas de processo.
        /// </summary>
        /// <param name="processo"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.EtiquetaProcesso processo)
        {
            return new
            {
                IdProcesso = processo.IdProcesso,
                CodInterno = processo.CodInterno,
                Descricao = processo.Descricao,
                IdAplicacao = processo.IdAplicacao,
                Situacao = processo.Situacao,
                TipoPedido = processo.TipoPedido,
                TipoProcesso = processo.TipoProcesso
            };
        }

        /// <summary>
        /// Recupera o proxy dos dados do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.Produto produto)
        {
            return new
            {
                IdProd = produto.IdProd,
                IdSubgrupoProd = produto.IdSubgrupoProd,
                IdGrupoProd = produto.IdGrupoProd,
                IdCorVidro = produto.IdCorVidro,
                IdUnidadeMedida = produto.IdUnidadeMedida,
                CodInterno = produto.CodInterno,
                Situacao = produto.Situacao,
                Descricao = produto.Descricao,
                Espessura = produto.Espessura,
                Peso = produto.Peso,
                AreaMinima = produto.AreaMinima,
                AtivarAreaMinima = produto.AtivarAreaMinima,
                ItemGenerico = produto.ItemGenerico,
                Obs = produto.Obs,
                Altura = produto.Altura,
                Largura = produto.Largura,
                Redondo = produto.Redondo,
                IdProcesso = produto.IdProcesso,
                IdAplicacao = produto.IdAplicacao,
                IdProdOrig = produto.IdProdOrig,
                IdProdBase = produto.IdProdBase
            };
        }

        /// <summary>
        /// Recupera o proxy do grupo de modelos.
        /// </summary>
        /// <param name="grupoModelo"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.GrupoModelo grupoModelo)
        {
            return new
            {
                IdGrupoModelo = grupoModelo.IdGrupoModelo,
                Descricao = grupoModelo.Descricao,
                Situacao = grupoModelo.Situacao,
                BoxPadrao = grupoModelo.BoxPadrao,
                Esquadria = grupoModelo.Esquadria
            };
        }

        /// <summary>
        /// Recupera o proxy para a medida de projeto.
        /// </summary>
        /// <param name="medida"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.MedidaProjeto medida)
        {
            return new
            {
                IdMedidaProjeto = medida.IdMedidaProjeto,
                IdGrupoMedProj = medida.IdGrupoMedProj,
                Descricao = medida.Descricao,
                ValorPadrao = medida.ValorPadrao,
                ExibirMedidaExata = medida.ExibirMedidaExata,
            };
        }

        /// <summary>
        /// Recupera o proxy do modelo de projeto.
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.ProjetoModelo modelo)
        {
            return new
            {
                IdProjetoModelo = modelo.IdProjetoModelo,
                IdGrupoModelo = modelo.IdGrupoModelo,
                Codigo = modelo.Codigo,
                Descricao = modelo.Descricao,
                NomeFigura = modelo.NomeFigura,
                NomeFiguraAssociada = modelo.NomeFiguraAssociada,
                AlturaFigura = modelo.AlturaFigura,
                LarguraFigura = modelo.LarguraFigura,
                Espessura = modelo.Espessura,
                TextoOrcamento = modelo.TextoOrcamento,
                TextoOrcamentoVidro = modelo.TextoOrcamentoVidro,
                TipoMedidasInst = modelo.TipoMedidasInst,
                TipoDesenho = modelo.TipoDesenho,
                TipoCalcAluminio = modelo.TipoCalcAluminio,
                EixoPuxador = modelo.EixoPuxador,
                Situacao = modelo.Situacao,
                CorVidro = modelo.CorVidro
            };
        }

        /// <summary>
        /// Recupera o proxy da posição da peça no modelo.
        /// </summary>
        /// <param name="posicao"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.PosicaoPecaModelo posicao)
        {
            return new
            {
                IdPosicaoPecaModelo = posicao.IdPosicaoPecaModelo,
                IdProjetoModelo = posicao.IdProjetoModelo,
                CoordX = posicao.CoordX,
                CoordY = posicao.CoordY,
                Orientacao = posicao.Orientacao,
                Calc = posicao.Calc
            };
        }

        /// <summary>
        /// Recupera o proxy da medida do modelo de projeto.
        /// </summary>
        /// <param name="medida"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.MedidaProjetoModelo medida)
        {
            return new
            {
                IdMedidaProjetoModelo = medida.IdMedidaProjetoModelo,
                IdProjetoModelo = medida.IdProjetoModelo,
                IdMedidaProjeto = medida.IdMedidaProjeto
            };
        }

        /// <summary>
        /// Recupera o proxy do material do modelo de projeto.
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.MaterialProjetoModelo material)
        {
            return new
            {
                IdMaterProjMod = material.IdMaterProjMod,
                IdProjetoModelo = material.IdProjetoModelo,
                IdProdProj = material.IdProdProj,
                Qtde = material.Qtde,
                Altura = material.Altura,
                Largura = material.Largura,
                Espessuras = material.Espessuras,
                TotM = material.TotM,
                CalculoQtde = material.CalculoQtde,
                CalculoAltura = material.CalculoAltura,
            };
        }

        /// <summary>
        /// Recupera o proxy da peçao do modelo de projeto.
        /// </summary>
        /// <param name="peca"></param>
        /// <returns></returns>
        private object GetProxy(Data.Model.PecaProjetoModelo peca)
        {
            return new
            {
                IdPecaProjMod = peca.IdPecaProjMod,
                IdProjetoModelo = peca.IdProjetoModelo,
                IdAplicacao = peca.IdAplicacao,
                IdProcesso = peca.IdProcesso,
                Altura = peca.Altura,
                Largura = peca.Largura,
                Altura03MM = peca.Altura03MM,
                Largura03MM = peca.Largura03MM,
                Altura04MM = peca.Altura04MM,
                Largura04MM = peca.Largura04MM,
                Altura05MM = peca.Altura05MM,
                Largura05MM = peca.Largura05MM,
                Altura06MM = peca.Altura06MM,
                Largura06MM = peca.Largura06MM,
                Altura08MM = peca.Altura08MM,
                Largura08MM = peca.Largura08MM,
                Altura10MM = peca.Altura10MM,
                Largura10MM = peca.Largura10MM,
                Altura12MM = peca.Altura12MM,
                Largura12MM = peca.Largura12MM,
                Tipo = peca.Tipo,
                Qtde = peca.Qtde,
                Item = peca.Item,
                CalculoQtde = peca.CalculoQtde,
                CalculoAltura = peca.CalculoAltura,
                CalculoLargura = peca.CalculoLargura,
                Redondo = peca.Redondo,
                Obs = peca.Obs
            };
        }

        /// <summary>
        /// Recupera o proxy dos dados da conta do aplicativo.
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        private object GetProxy(Data.DAL.AppDAO.ContaAppDados dados)
        {
            return new
            {
                CoresVidro = dados.CoresVidro.Select(GetProxy),
                EspessurasVidro = dados.EspessurasVidro.Select(f =>
                    new { Valor = f.Valor, Descricao = f.Descricao }),
                TiposEntrega = dados.TiposEntrega.Select(GetProxy),
                UnidadesMedida = dados.UnidadesMedida.Select(GetProxy),
                GruposProduto = dados.GruposProduto.Select(GetProxy),
                SubgruposProduto = dados.SubgruposProduto.Select(GetProxy),
                EtiquetasAplicacao = dados.EtiquetasAplicacao.Select(GetProxy),
                EtiquetasProcesso = dados.EtiquetasProcesso.Select(GetProxy),
                Produtos = dados.Produtos.Select(GetProxy),
                GruposModelo = dados.GruposModelo.Select(GetProxy),
                MedidasProjeto = dados.MedidasProjeto.Select(GetProxy),
                ProjetoModelos = dados.ProjetoModelos.Select(GetProxy),
                PosicoesPecaModelo = dados.PosicoesPecaModelo.Select(GetProxy),
                MateriasProjetoModelo = dados.MateriasProjetoModelo.Select(GetProxy),
                MedidasProjetoModelo = dados.MedidasProjetoModelo.Select(GetProxy),
                PecasProjetoModelo = dados.PecasProjetoModelo.Select(GetProxy),
                Configuracoes = dados.Configuracoes.Select(GetProxy)
            };
        }

        /// <summary>
        /// Carrega os dados da configuração informada.
        /// </summary>
        /// <param name="configuracao"></param>
        /// <param name="valor">Valor da configuração.</param>
        /// <returns></returns>
        private object GetProxy(Data.DAL.AppDAO.Configuracao configuracao)
        {
            return new
            {
                IdConfig = configuracao.IdConfig,
                Tipo = configuracao.Tipo,
                Valor = configuracao.Valor
            };
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o pacote de imagens dos modelos de projeto.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ObterImagensModelos()
        {
            var diretorioModelosProjeto = System.Configuration.ConfigurationManager.AppSettings["diretorioModelosProjeto"];

            if (!System.IO.Directory.Exists(diretorioModelosProjeto))
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "O caminho do diretório de imagens não existe." });

            var zipFile = ImagesConfig.ObterImagensProjeto();

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(zipFile);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = "projetos.zip";
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");

            return httpResponseMessage;
        }

        /// <summary>
        /// Sincroniza os dados da conta.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public object Sincronizar()
        {
            var appDAO = new Data.DAL.AppDAO();

            return GetProxy(appDAO.ObterDadosConta());
        }

        #endregion
    }
}
