using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class MedidaItemProjetoDAO : BaseDAO<MedidaItemProjeto, MedidaItemProjetoDAO>
	{
        //private MedidaItemProjetoDAO() { }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="descrMedida"></param>
        /// <returns></returns>
        public int GetByItemProjeto(uint idItemProjeto, string descrMedida)
        {
            return GetByItemProjeto(null, idItemProjeto, descrMedida);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="descrMedida"></param>
        /// <returns></returns>
        public int GetByItemProjeto(GDASession sessao, uint idItemProjeto, string descrMedida)
        {
            return GetByItemProjeto(sessao, idItemProjeto, 0, 0, descrMedida, false);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <returns></returns>
        public int GetByItemProjeto(uint idItemProjeto, uint idMedidaProjeto)
        {
            return GetByItemProjeto(null, idItemProjeto, idMedidaProjeto);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <returns></returns>
        public int GetByItemProjeto(GDASession session, uint idItemProjeto, uint idMedidaProjeto)
        {
            return GetByItemProjeto(session, idItemProjeto, idMedidaProjeto, true);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="idGrupoMedProj"></param>
        /// <returns></returns>
        public int GetByItemProjeto(uint idItemProjeto, uint idMedidaProjeto, uint idGrupoMedProj)
        {
            return GetByItemProjeto(null, idItemProjeto, idMedidaProjeto, idGrupoMedProj);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="idGrupoMedProj"></param>
        /// <returns></returns>
        public int GetByItemProjeto(GDASession session, uint idItemProjeto, uint idMedidaProjeto, uint idGrupoMedProj)
        {
            return GetByItemProjeto(session, idItemProjeto, idMedidaProjeto, idGrupoMedProj, null, true);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="validarApenasUm"></param>
        /// <returns></returns>
        public int GetByItemProjeto(uint idItemProjeto, uint idMedidaProjeto, bool validarApenasUm)
        {
            return GetByItemProjeto(null, idItemProjeto, idMedidaProjeto, validarApenasUm);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="validarApenasUm"></param>
        /// <returns></returns>
        public int GetByItemProjeto(GDASession sessao, uint idItemProjeto, uint idMedidaProjeto, bool validarApenasUm)
        {
            return GetByItemProjeto(sessao, idItemProjeto, idMedidaProjeto, 0, null, validarApenasUm);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="idGrupoMedProj"></param>
        /// <param name="descrMedida"></param>
        /// <param name="validarApenasUm"></param>
        /// <returns></returns>
        private int GetByItemProjeto(uint idItemProjeto, uint idMedidaProjeto, uint idGrupoMedProj, string descrMedida, bool validarApenasUm)
        {
            return GetByItemProjeto(null, idItemProjeto, idMedidaProjeto, idGrupoMedProj, descrMedida, validarApenasUm);
        }

        /// <summary>
        /// Retorna a medida utilizada no item_projeto passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="idMedidaProjeto"></param>
        /// <param name="idGrupoMedProj"></param>
        /// <param name="descrMedida"></param>
        /// <param name="validarApenasUm"></param>
        /// <returns></returns>
        private int GetByItemProjeto(GDASession sessao, uint idItemProjeto, uint idMedidaProjeto, uint idGrupoMedProj, string descrMedida, bool validarApenasUm)
        {
            string sql = "Select mip.* From medida_item_projeto mip inner join medida_projeto m On (mip.idMedidaProjeto=m.idMedidaProjeto) Where idItemProjeto=" + idItemProjeto;
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (idMedidaProjeto > 0)
                sql += " And mip.idMedidaProjeto=" + idMedidaProjeto;

            if (idGrupoMedProj > 0)
                sql += " And mip.idMedidaProjeto In (Select idMedidaProjeto From medida_projeto Where idGrupoMedProj=" + idGrupoMedProj + ")";

            if (!String.IsNullOrEmpty(descrMedida))
            {
                sql += " And m.descricao=?descrMedida";
                lstParam.Add(new GDAParameter("?descrMedida", descrMedida));
            }

            List<MedidaItemProjeto> lst = objPersistence.LoadData(sessao, sql, lstParam.Count > 0 ? lstParam.ToArray() : null);

            if (validarApenasUm && lst.Count > 1 && idMedidaProjeto != 1 && (lst.Count == 2 ? lst[0].Valor != lst[1].Valor : true))
            {
                // Busca o pedido e o projeto para identificar pedido/projeto com possível erro
                string codProjeto = ProjetoModeloDAO.Instance.ObtemCodigo(sessao, ItemProjetoDAO.Instance.ObtemIdProjetoModelo(sessao, idItemProjeto));
                uint idPedido = ItemProjetoDAO.Instance.ObtemValorCampo<uint>(sessao, "coalesce(idPedido, idPedidoEspelho)", "idItemProjeto=" + idItemProjeto);
                string mensagemErro = " Pedido: " + idPedido + " Projeto: " + codProjeto + " Medida: " + (idMedidaProjeto > 0 ? MedidaProjetoDAO.Instance.ObtemDescricao(sessao, lst[0].IdMedidaProjeto) : "0") + ".";

                throw new Exception("Busca de medida de projeto retornou mais de um resultado." + mensagemErro);
            }

            return lst.Count > 0 ? lst[0].Valor : 0;
        }

        /// <summary>
        /// Retorna uma listagem das medidas usadas no itemProjeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public IList<MedidaItemProjeto> GetListByItemProjeto(uint idItemProjeto)
        {
            return GetListByItemProjeto(null, idItemProjeto);
        }
        
        /// <summary>
        /// Retorna uma listagem das medidas usadas no itemProjeto
        /// </summary>
        public IList<MedidaItemProjeto> GetListByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = @"Select mip.*, mp.descricao as nomeMedidaProjeto From medida_item_projeto mip 
                Left Join medida_projeto mp on (mip.idMedidaProjeto=mp.idMedidaProjeto)
                Where idItemProjeto=" + idItemProjeto;

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Exclui as medidas do item_projeto passado
        /// </summary>
        /// <param name="idItemProjeto"></param>
        public int DeleteByItemProjeto(uint idItemProjeto)
        {
            return DeleteByItemProjeto(null, idItemProjeto);
        }

        /// <summary>
        /// Exclui as medidas do item_projeto passado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        public int DeleteByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Delete From medida_item_projeto Where idItemProjeto=" + idItemProjeto;

            return objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Verifica se existe medida cadastrada neste projeto
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool ExistsMedida(uint idItemProjeto)
        {
            return ExistsMedida(null, idItemProjeto);
        }

        /// <summary>
        /// Verifica se existe medida cadastrada neste projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool ExistsMedida(GDASession sessao, uint idItemProjeto)
        {
            string sql = "Select Count(*) From medida_item_projeto Where idItemProjeto=" + idItemProjeto;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString()) > 0;
        }

        /// <summary>
        /// Verifica se todas as medidas foram inseridas
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool TodasMedidasInseridas(ItemProjeto itemProj)
        {
            int qtdMedidasIns = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(
                "Select Count(*) From medida_item_projeto Where idItemProjeto=" + itemProj.IdItemProjeto).ToString());

            int qtdMedidasProj = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(
                "Select Count(*) From medida_projeto_modelo Where idProjetoModelo=" + itemProj.IdProjetoModelo).ToString());

            return qtdMedidasIns == qtdMedidasProj;
        }

        public void InsereMedida(uint idItemProjeto, uint idMedidaProjeto, int valor)
        {
            InsereMedida(null, idItemProjeto, idMedidaProjeto, valor);
        }

        public void InsereMedida(GDASession sessao, uint idItemProjeto, uint idMedidaProjeto, int valor)
        {
            MedidaItemProjeto mip = new MedidaItemProjeto();
            mip.IdItemProjeto = idItemProjeto;
            mip.IdMedidaProjeto = idMedidaProjeto;
            mip.Valor = valor;
            base.Insert(sessao, mip);
        }
	}
}