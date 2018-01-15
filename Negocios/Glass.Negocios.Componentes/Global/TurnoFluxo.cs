using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de turno.
    /// </summary>
    public class TurnoFluxo : ITurnoFluxo, Entidades.IValidadorTurno
    {
        /// <summary>
        /// Pesquisa os turnos.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Turno> PesquisarTurnos()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Turno>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.Turno>();
        }

        /// <summary>
        /// Recupera os descritores dos turnos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemTurnos()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Turno>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.Turno>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do turno.
        /// </summary>
        /// <param name="idTurno"></param>
        /// <returns></returns>
        public Entidades.Turno ObtemTurno(int idTurno)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Turno>()
                .Where("IdTurno=?idTurno")
                .Add("?idTurno", idTurno)
                .ProcessLazyResult<Entidades.Turno>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarTurno(Entidades.Turno turno)
        {
            turno.Require("turno").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = turno.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarTurno(Entidades.Turno turno)
        {
            turno.Require("turno").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = turno.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a atualização do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorTurno.ValidaAtualizacao(Entidades.Turno turno)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Turno>()
                    .Where("(Descricao = ?descricao OR NumSeq = ?numSeq)")
                    .Add("?descricao", turno.Descricao)
                    .Add("?numSeq", turno.NumSeq);

            if (turno.ExistsInStorage)
            {
                consulta.WhereClause
                    .And("IdTurno <> ?id")
                    .Add("?id", turno.IdTurno);
            }

            if (consulta.ExistsResult())
                return new IMessageFormattable[]
                {
                    "Já existe um turno cadastro com os mesmos dados informados.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }
    }
}
