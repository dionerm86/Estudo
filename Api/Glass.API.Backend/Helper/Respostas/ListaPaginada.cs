// <copyright file="ListaPaginada.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que contém uma resposta com uma lista paginada de itens.
    /// </summary>
    /// <typeparam name="T">O tipo de item da lista.</typeparam>
    internal class ListaPaginada<T> : IHttpActionResult
    {
        private readonly ApiController apiController;
        private readonly IEnumerable<T> lista;
        private readonly int pagina;
        private readonly int numeroRegistros;
        private readonly Lazy<long> total;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaPaginada{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="lista">A lista de itens que será retornada.</param>
        /// <param name="pagina">A página atual dos itens.</param>
        /// <param name="numeroRegistros">O número de registros por página.</param>
        /// <param name="contagemRegistros">Função que retorna o número total de registros.</param>
        public ListaPaginada(ApiController apiController, IEnumerable<T> lista, int pagina, int numeroRegistros, Func<long> contagemRegistros)
        {
            Contract.Requires(lista != null);

            this.apiController = apiController;
            this.lista = lista.ToList();
            this.pagina = pagina;
            this.numeroRegistros = numeroRegistros;

            this.total = new Lazy<long>(() => this.ObterTotalItens(contagemRegistros));
        }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<HttpResponseMessage> obterResposta = () =>
            {
                var status = this.ObterStatusResposta();
                var resposta = this.apiController.Request.CreateResponse(status, this.lista);

                this.IncluirCabecalhoLinks(resposta);

                return resposta;
            };

            return Task.Run(obterResposta, cancellationToken);
        }

        private long ObterTotalItens(Func<long> contagemRegistros)
        {
            if (this.pagina == 1 && this.lista.Count() < this.numeroRegistros)
            {
                return this.lista.Count();
            }

            return contagemRegistros != null
                ? contagemRegistros()
                : this.lista.Count();
        }

        private HttpStatusCode ObterStatusResposta()
        {
            if (!this.lista.Any())
            {
                return HttpStatusCode.NoContent;
            }

            return this.pagina == this.NumeroUltimaPagina()
                ? HttpStatusCode.OK
                : HttpStatusCode.PartialContent;
        }

        private long NumeroUltimaPagina()
        {
            var quocientePaginas = (decimal)this.total.Value / this.numeroRegistros;
            return (long)Math.Ceiling(quocientePaginas);
        }

        private void IncluirCabecalhoLinks(HttpResponseMessage resposta)
        {
            if (!this.lista.Any() || this.total.Value <= this.numeroRegistros)
            {
                return;
            }

            var uri = this.apiController.Request.RequestUri;
            var links = this.ObterLinksParaPagina(uri);

            if (links.Any())
            {
                resposta.Headers.Add("Access-Control-Expose-Headers", "Link");
                resposta.Headers.Add("Link", string.Join(", ", links));
            }
        }

        private List<string> ObterLinksParaPagina(Uri uriAtual)
        {
            string uriBase = uriAtual.AbsoluteUri.Split('?')[0];
            var query = uriAtual.ParseQueryString();
            long numeroUltimaPagina = this.NumeroUltimaPagina();

            var links = new List<string>();

            if (this.pagina > 1)
            {
                links.Add(this.ObterLink(uriBase, query, "first", 1));
                links.Add(this.ObterLink(uriBase, query, "prev", this.pagina - 1));
            }

            if (this.pagina < numeroUltimaPagina)
            {
                links.Add(this.ObterLink(uriBase, query, "next", this.pagina + 1));
                links.Add(this.ObterLink(uriBase, query, "last", numeroUltimaPagina));
            }

            return links;
        }

        private string ObterLink(string uriBase, NameValueCollection query, string rel, long pagina)
        {
            query["pagina"] = pagina.ToString();

            var queryString = new List<string>();

            foreach (var chave in query.AllKeys)
            {
                queryString.Add(string.Format("{0}={1}", chave, query[chave]));
            }

            return string.Format("<{0}?{1}>; rel=\"{2}\"", uriBase, string.Join("&", queryString), rel);
        }
    }
}
