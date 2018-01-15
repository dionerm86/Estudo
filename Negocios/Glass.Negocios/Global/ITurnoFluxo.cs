using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de turnos.
    /// </summary>
    public interface ITurnoFluxo
    {
        /// <summary>
        /// Pesquisa os turnos.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Turno> PesquisarTurnos();

        /// <summary>
        /// Recupera os descritores dos turnos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemTurnos();

        /// <summary>
        /// Recupera os dados do turno.
        /// </summary>
        /// <param name="idTurno"></param>
        /// <returns></returns>
        Entidades.Turno ObtemTurno(int idTurno);

        /// <summary>
        /// Salva os dados do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarTurno(Entidades.Turno turno);

        /// <summary>
        /// Apaga os dados do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarTurno(Entidades.Turno turno);
    }
}
