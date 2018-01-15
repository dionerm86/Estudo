using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio das rotas.
    /// </summary>
    public class RotaFluxo : 
        IRotaFluxo, Entidades.IValidadorRota,
        Entidades.IValidadorRotaCliente,
        Entidades.IProvedorRota
    {
        #region Rota

        /// <summary>
        /// Pesquisa as rotas.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.RotaPesquisa> PesquisarRotas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .OrderBy("Descricao")
                .Select("IdRota, CodInterno, Descricao, Situacao, Distancia, Obs, DiasSemana, NumeroMinimoDiasEntrega")
                .ToVirtualResult<Entidades.RotaPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores das rotas.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemRotas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.Rota>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da rota.
        /// </summary>
        /// <param name="idRota"></param>
        /// <returns></returns>
        public Entidades.Rota ObtemRota(int idRota)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .Where("IdRota=?id")
                .Add("?id", idRota)
                .ProcessLazyResult<Entidades.Rota>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera os dados da rota.
        /// </summary>
        public IList<Entidades.Rota> ObterRotas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .ProcessResult<Entidades.Rota>()
                .ToList();
        }

        /// <summary>
        /// Salva os dados da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarRota(Entidades.Rota rota)
        {
            rota.Require("rota").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = rota.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarRota(Entidades.Rota rota)
        {
            rota.Require("rota").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = rota.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a atualização da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorRota.ValidaAtualizacao(Entidades.Rota rota)
        {
            if (string.IsNullOrWhiteSpace(rota.CodInterno))
                return new IMessageFormattable[] { "Informe um código para a rota.".GetFormatter() };

            if (string.IsNullOrWhiteSpace(rota.Descricao))
                return new IMessageFormattable[] { "Informe uma descrição para a rota.".GetFormatter() };

            // Verifica se já foi inserida uma rota com o código interno passado
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .Where("CodInterno=?codInterno")
                .Add("?codInterno", rota.CodInterno);

            if (rota.ExistsInStorage)
                consulta.WhereClause
                    .And("IdRota<>?idRota")
                    .Add("?idRota", rota.IdRota);

            if (consulta.ExistsResult())
                return new IMessageFormattable[]
                {
                    "O código informado já está sendo usado em outra rota.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a existencia da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorRota.ValidaExistencia(Entidades.Rota rota)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consulta = SourceContext.Instance.CreateMultiQuery();

            var adicionaConsulta = new Action<Type, string, char>((tipo, nome, genero) =>
            {
                consulta.Add(SourceContext.Instance.CreateQuery()
                    .From(new Colosoft.Query.EntityInfo(tipo.FullName))
                    .Where("IdRota=?id")
                    .Add("?id", rota.IdRota)
                    .Count(),

                    tratarResultado(String.Format(
                        "Esta rota não pode ser excluída por possuir {0} relacionad{1}s à mesma.",
                        nome, genero)));
            });

            adicionaConsulta(typeof(Data.Model.OrdemCarga), "ordens de carga", 'a');
            adicionaConsulta(typeof(Data.Model.Cte.ComplCte), "conhecimentos de transporte", 'o');
            adicionaConsulta(typeof(Data.Model.RotaCliente), "clientes", 'o');

            consulta.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region RotaCliente

        /// <summary>
        /// Pesquisa os clientes da rota.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <returns></returns>
        public IList<Entidades.RotaClientePesquisa> PesquisarClientesRota(int idRota)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RotaCliente>("rc")
                .InnerJoin<Data.Model.Cliente>("rc.IdCliente = c.IdCli", "c")
                .LeftJoin<Data.Model.Cidade>("c.IdCidade = cid.IdCidade", "cid")
                .OrderBy("rc.NumSeq")
                .Where("rc.IdRota=?id")
                .Add("?id", idRota)
                .Select(@"rc.IdRotaCliente, rc.IdRota, rc.IdCliente, rc.NumSeq,
                         ISNULL(c.Nome, c.NomeFantasia) AS Nome, c.CpfCnpj, c.Numero,
                         c.Compl, c.Bairro, cid.NomeCidade AS Cidade, cid.NomeUf AS Uf,
                         c.Endereco, c.Situacao, c.TelCont, c.Email, c.DtUltCompra,
                         c.TotalComprado")
                .ToVirtualResult<Entidades.RotaClientePesquisa>();
        }

        /// <summary>
        /// Recupera os dados da associação entre a rota e o cliente.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <param name="idCliente">Identifador do cliente.</param>
        /// <returns></returns>
        public Entidades.RotaCliente ObtemRotaCliente(int idRota, int idCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RotaCliente>()
                .Where("IdRota=?idRota AND IdCliente=?idCliente")
                .Add("?idRota", idRota)
                .Add("?idCliente", idCliente)
                .ProcessLazyResult<Entidades.RotaCliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Adiciona salva a associação do cliente com a rota.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarRotaCliente(Entidades.RotaCliente rotaCliente)
        {
            rotaCliente.Require("rotaCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = rotaCliente.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a associação da rota com o cliente.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarRotaCliente(Entidades.RotaCliente rotaCliente)
        {
            rotaCliente.Require("rotaCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = rotaCliente.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Altera a posição do cliente dentro da rota.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicao(int idRota, int idCliente, bool paraCima)
        {
            var rotaCliente = SourceContext.Instance.CreateQuery()
                .From<Data.Model.RotaCliente>()
                .Where("IdRota=?idRota AND IdCliente=?idCliente")
                .Add("?idRota", idRota)
                .Add("?idCliente", idCliente)
                .ProcessLazyResult<Entidades.RotaCliente>()
                .FirstOrDefault();

            var adjacente = SourceContext.Instance.CreateQuery()
                .From<Data.Model.RotaCliente>()
                .Where(string.Format("IdRota=?idRota AND NumSeq{0}?numSeq", (paraCima ? "<" : ">")))
                .Add("?idRota", idRota)
                .Add("?numSeq", rotaCliente.NumSeq)
                .OrderBy(string.Format("NumSeq {0}", (paraCima ? "DESC" : "ASC")))
                .Take(1)
                .ProcessLazyResult<Entidades.RotaCliente>()
                .FirstOrDefault();

            if (adjacente != null)
            {
                var numSeqAdjacente = adjacente.NumSeq;

                // Altera a posição do cliente adjacente
                adjacente.NumSeq = rotaCliente.NumSeq;
                // Altera a posição do cliente clicado
                rotaCliente.NumSeq = numSeqAdjacente;

                using (var session = SourceContext.Instance.CreateSession())
                {
                    var resultado = rotaCliente.Save(session);
                    if (!resultado)
                        return resultado;

                    resultado = adjacente.Save(session);
                    if (!resultado)
                        return resultado;

                    return session.Execute(false).ToSaveResult();
                }
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Recupera um novo número de sequencia para a associação da rota com o cliente.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        int Entidades.IValidadorRotaCliente.ObtemNumeroSequencia(Entidades.RotaCliente rotaCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RotaCliente>()
                .Where("IdRota=?idRota")
                .Add("?idRota", rotaCliente.IdRota)
                .Select("MAX(NumSeq)")
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault() + 1;
        }

        #endregion

        #region Membros de IProvedorRota

        /// <summary>
        /// Recupera a rota associada ao cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Entidades.Rota Entidades.IProvedorRota.ObtemRota(Entidades.Cliente cliente)
        {
            if (!cliente.IdRota.HasValue) return null;

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Rota>()
                .Where("IdRota=?id")
                .Add("?id", cliente.IdRota)
                .ProcessLazyResult<Entidades.Rota>()
                .FirstOrDefault();
        }

        #endregion
    }
}
