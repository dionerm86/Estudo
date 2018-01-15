using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Entidade de negocio da media do projeto modelo.
    /// </summary>
    public class MediaProjetoModelo : Glass.Api.Projeto.IMediaProjetoModelo
    {
        #region Properties

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Valor padrão.
        /// </summary>
        public decimal ValorPadrao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Cria a medida do projeto modelo.
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="valorPadrao"></param>
        public MediaProjetoModelo(string descricao, decimal valorPadrao)
        {
            Descricao = descricao;
            ValorPadrao = valorPadrao;
        }

        #endregion
    }

    /// <summary>
    /// Fluxo de negocio da media do projeto modelo.
    /// </summary>
    public class MedidaProjetoModeloFluxo : Glass.Api.Projeto.IMediaProjetoModeloFluxo
    {
        /// <summary>
        /// Recupera as medidas do projeto modelo.
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public IList<Glass.Api.Projeto.IMediaProjetoModelo> ObterMedidasProjetoModelo(uint idProjetoModelo, uint idItemProjeto)
        {
            var itemProj = idItemProjeto != 0 ? Glass.Data.DAL.ItemProjetoDAO.Instance.GetElement(idItemProjeto) : null;

            var medias = Glass.Data.DAL.MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(idProjetoModelo, false);

            foreach (var mpm in medias)
            {
                int valorMedida = Glass.Data.DAL.MedidaItemProjetoDAO.Instance.GetByItemProjeto(idItemProjeto, mpm.IdMedidaProjeto, false);

                // Caso não tenha sido cadastrado insere valor padrão.
                if (valorMedida == 0)
                    valorMedida = Glass.Data.DAL.MedidaProjetoDAO.Instance.ObtemValorPadrao(mpm.IdMedidaProjetoModelo);

                // verificar se a medida deve ser editada mesmo quando estiver para não editar.
                var isEnable = Glass.Data.DAL.MedidaProjetoDAO.Instance.ExibirCalcMedidaExata(mpm.IdMedidaProjeto);

                // Caso seja box padrão e altura de vão, setar valores padrão
                if (Glass.Data.DAL.ProjetoModeloDAO.Instance.IsBoxPadrao(idProjetoModelo) && mpm.IdMedidaProjeto == 3)
                {

                }
                else
                {
                    
                }

            }

            if (idItemProjeto == 0)
            {

            }
            else
            {

            }


            return null;
        }       

    }
}
