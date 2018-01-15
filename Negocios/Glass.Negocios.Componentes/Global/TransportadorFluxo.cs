using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio de transportadores.
    /// </summary>
    public class TransportadorFluxo : Negocios.ITransportadorFluxo, Glass.Global.Negocios.Entidades.IValidadorTransportador
    {
        #region Transportador

        /// <summary>
        /// Recupera os descritores dos transportadores.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemDescritoresTransportadores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Transportador>()
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.Transportador>()
                .ToList();
        }

        /// <summary>
        /// Pesquisa os transportadores.
        /// </summary>
        /// <param name="idTransportador">Identificador do transportador.</param>
        /// <param name="nome">Nome que será usado na pesquisa.</param> 
        /// <param name="cpfCnpj">CPF/CNPJ</param>
        /// <returns></returns>
        public IList<Entidades.Transportador> PesquisarTransportadores(int? idTransportador, string nome, string cpfCnpj)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Transportador>()
                .OrderBy("Nome");

            if (idTransportador.HasValue && idTransportador.Value > 0)
                consulta.WhereClause
                    .And("IdTransportador = ?id")
                    .Add("?id", idTransportador);

            else
            {
                if (!string.IsNullOrEmpty(nome))
                    consulta.WhereClause
                        .And("(Nome LIKE ?nome OR NomeFantasia LIKE ?nome)")
                        .Add("?nome", string.Format("%{0}%", nome));


                if (!string.IsNullOrEmpty(cpfCnpj))
                    consulta.WhereClause
                        .And("REPLACE(REPLACE(REPLACE(CpfCnpj, '.', ''), '-', ''), '/', '')=?cpfCnpj")
                        .Add("?cpfCnpj", cpfCnpj);
            }

            return consulta.ToVirtualResultLazy<Entidades.Transportador>();
        }

        /// <summary>
        /// Recupera os dados do transportador.
        /// </summary>
        /// <param name="idTransportador"></param>
        /// <returns></returns>
        public Entidades.Transportador ObtemTransportador(int idTransportador)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Transportador>()
                .Where("IdTransportador=?id")
                .Add("?id", idTransportador)
                .ProcessLazyResult<Entidades.Transportador>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do transportador.
        /// </summary>
        /// <param name="transportador"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarTransportador(Entidades.Transportador transportador)
        {
            transportador.Require("transportador").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = transportador.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }
        
        /// <summary>
        /// Apaga os dados do transportador.
        /// </summary>
        /// <param name="transportador"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarTransportador(Entidades.Transportador transportador)
        {
            transportador.Require("transportador").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = transportador.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a existência do transportador
        /// </summary>
        IMessageFormattable[] Entidades.IValidadorTransportador.ValidaExistencia(Entidades.Transportador transportador)
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

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cte.ParticipanteCte>()
                    .Where("IdTransportador=?id")
                    .Add("?id", transportador.IdTransportador)
                    .Count(),
                    tratarResultado("Há CTe's associados ao mesmo."))
                    .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        /// <summary>
        /// Verifica se existencia de algum CPF/CNPJ já cadastrado para algum transportador.
        /// </summary>
        /// <param name="cpfCnpj">Valor que será verificador.</param>
        /// <returns></returns>
        public bool VerificarCpfCnpj(string cpfCnpj)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Transportador>()
                .Where("REPLACE(REPLACE(REPLACE(CpfCnpj, '.', ''), '-', ''), '/', '')=?cpfCnpj OR CpfCnpj=?cpfCnpj")
                .Add("?cpfCnpj", cpfCnpj)
                .ExistsResult();
        }

        #endregion
    }
}
