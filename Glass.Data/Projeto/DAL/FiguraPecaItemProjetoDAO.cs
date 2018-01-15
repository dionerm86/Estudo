using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FiguraPecaItemProjetoDAO : BaseDAO<FiguraPecaItemProjeto, FiguraPecaItemProjetoDAO>
    {
        //private FiguraPecaItemProjetoDAO() { }

        /// <summary>
        /// Retorna todas as figuras cadastradas na peça informada
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<FiguraPecaItemProjeto> GetFigurasByPeca(uint idPecaItemProj, int item)
        {
            string sql = @"Select fpip.*, fp.codInterno as codInternoFigura, gfp.descricao as descrGrupoFigura
                From figura_peca_item_projeto fpip
                    left join figura_projeto fp on (fpip.idFiguraProjeto=fp.idFiguraProjeto)
                    left join grupo_figura_projeto gfp on (fp.idGrupoFigProj=gfp.idGrupoFigProj)
                Where idPecaItemProj=" + idPecaItemProj + " And item=" + item;

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca figuras de uma peça para serem clonadas
        /// </summary>
        /// <param name="idPecaItemProj"></param>
        /// <returns></returns>
        public IList<FiguraPecaItemProjeto> GetForClone(GDASession sessao, uint idPecaItemProj)
        {
            string sql = "Select * From figura_peca_item_projeto Where idPecaItemProj=" + idPecaItemProj;
            return objPersistence.LoadData(sessao, sql).ToList();
        }

        public int GetCountByPeca(uint idPecaItemProj, int item)
        {
            string sql = "Select count(*) From figura_peca_item_projeto Where idPecaItemProj=" + idPecaItemProj + " And item=" + item;
            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        public bool PedidoTemPeca(uint idPedido)
        {
            string sql = @"select count(*) from figura_peca_item_projeto fgip
                inner join peca_item_projeto pip on (fgip.idPecaItemProj=pip.idPecaItemProj) 
                inner join item_projeto ip on (pip.idItemProjeto=ip.idItemProjeto)
                where ip.idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Insere/Atualiza figuras da imagem editada
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idPecaItemProj"></param>
        /// <param name="vetIdFiguraProjeto"></param>
        /// <param name="vetCoord"></param>
        public void InsereAtualizaFiguras(uint idItemProjeto, uint idPecaItemProj, int item, string[] vetIdFiguraProjeto, string[] vetCoord)
        {
            List<FiguraPecaItemProjeto> lstFigura = new List<FiguraPecaItemProjeto>();
            FiguraPecaItemProjeto posPeca;

            for (int i = 0; i < vetIdFiguraProjeto.Length; i++)
            {
                if (String.IsNullOrEmpty(vetIdFiguraProjeto[i]))
                    continue;

                string[] coord = vetCoord[i].Split(';');

                posPeca = new FiguraPecaItemProjeto();
                posPeca.IdPecaItemProj = idPecaItemProj;
                posPeca.IdFiguraProjeto = Glass.Conversoes.StrParaUint(vetIdFiguraProjeto[i]);
                posPeca.Item = item;
                posPeca.CoordX = Glass.Conversoes.StrParaInt(coord[0]);
                posPeca.CoordY = Glass.Conversoes.StrParaInt(coord[1]);
                lstFigura.Add(posPeca);
            }

            List<KeyValuePair<int, string>> alteracoes = new List<KeyValuePair<int, string>>();
            
            #region Monta a lista de alterações para o Log

            List<FiguraPecaItemProjeto> atuais = new List<FiguraPecaItemProjeto>(GetFigurasByPeca(idPecaItemProj, item));

            foreach (FiguraPecaItemProjeto atual in atuais)
            {
                FiguraPecaItemProjeto nova = lstFigura.Find(new Predicate<FiguraPecaItemProjeto>(
                    delegate(FiguraPecaItemProjeto f)
                    {
                        return f.IdFiguraProjeto == atual.IdFiguraProjeto && f.CoordX == atual.CoordX && f.CoordY == atual.CoordY;
                    }
                ));

                if (nova == null)
                {
                    alteracoes.Add(new KeyValuePair<int, string>(item, "Figura '" + FiguraProjetoDAO.Instance.ObtemCodInterno(atual.IdFiguraProjeto) + 
                        "' removida da posição (" + atual.CoordX + ", " + atual.CoordY + ")."));
                }
            }

            foreach (FiguraPecaItemProjeto nova in lstFigura)
            {
                FiguraPecaItemProjeto atual = atuais.Find(new Predicate<FiguraPecaItemProjeto>(
                    delegate(FiguraPecaItemProjeto f)
                    {
                        return f.IdFiguraProjeto == nova.IdFiguraProjeto && f.CoordX == nova.CoordX && f.CoordY == nova.CoordY;
                    }
                ));

                if (atual == null)
                {
                    alteracoes.Add(new KeyValuePair<int, string>(item, "Figura '" + FiguraProjetoDAO.Instance.ObtemCodInterno(nova.IdFiguraProjeto) + 
                        "' incluída na posição (" + nova.CoordX + ", " + nova.CoordY + ")."));
                }
            }

            #endregion

            // Exclui todas as figuras inseridas nesta peça
            objPersistence.ExecuteCommand("Delete From figura_peca_item_projeto Where idPecaItemProj=" + idPecaItemProj +
                " And item=" + item);

            // Insere os novos dados coletados
            foreach (FiguraPecaItemProjeto ppip in lstFigura)
                FiguraPecaItemProjetoDAO.Instance.Insert(ppip);

            foreach (KeyValuePair<int, string> a in alteracoes)
                LogAlteracaoDAO.Instance.LogImagemProducao(idPecaItemProj, a.Key.ToString(), a.Value);
        }

        public void DeleteByItemProjeto(uint idItemProjeto)
        {
            string sql = @"delete from figura_peca_item_projeto where idPecaItemProj in (select * from (
                select idPecaItemProj from peca_item_projeto where idItemProjeto=" + idItemProjeto + ") as temp)";

            objPersistence.ExecuteCommand(sql);
        }
    }
}
