using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoInstDAO : BaseDAO<ProducaoInst, ProducaoInstDAO>
    {
        //private ProducaoInstDAO() { }

        /// <summary>
        /// Retorna a produção referente às instalações feitas pelas equipes no período informado
        /// </summary>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public ProducaoInst[] GetProducaoInst(uint idEquipe, int tipoInstalacao, string dataIni, string dataFim)
        {
            var idsSubgrupo = SubgrupoProdDAO.Instance.GetSubgruposTemperados();

            if (String.IsNullOrEmpty(idsSubgrupo))
                idsSubgrupo = "0";

            string sql = @"
                Select Round(Sum(pi.qtdeInstalada)/Count(distinct e.idEquipe), 2) As QtdePecas,
                    Round(Sum((if(ped.TipoVenda=" + (int)Pedido.TipoVendaPedido.Garantia + @", 0, pi.qtdeInstalada) * pp.totM)/pp.qtde)/Count(Distinct e.idEquipe), 2) As TotalM2,
                    Round(Sum((if(ped.TipoVenda=" + (int)Pedido.TipoVendaPedido.Garantia + @", pi.qtdeInstalada, 0) * pp.totM)/pp.qtde)/Count(Distinct e.idEquipe), 2) As QtdeGarantia, 
                    e.Nome As NomeEquipe, (If(e.tipo=1, 'Comum', 'Temperado')) As TipoEquipe
                From produtos_instalacao pi
                    Inner Join produtos_pedido pp On (pp.idProdPed=pi.idProdPed)
                    Inner Join instalacao i On (pp.idPedido=i.idPedido)
                    Left Join pedido ped On (ped.idPedido=i.idPedido)
                    Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao)
                    Left Join equipe e On (ei.idEquipe=e.idEquipe)
                    Left Join produto p On (pp.idProd=p.idProd)
                Where p.idGrupoProd=1
                    /*And if (e.tipo=1, p.idSubgrupoProd Not In (" + idsSubgrupo + "), p.idSubgrupoProd In (" + idsSubgrupo + @") Or i.liberarTemperado=1)*/
                    And (i.Situacao=" + (int)Instalacao.SituacaoInst.Continuada + @"
                    Or i.Situacao=" + (int)Instalacao.SituacaoInst.Finalizada + @")";
            
            string criterio = String.Empty;

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (idEquipe > 0)
                sql += " And e.idEquipe=" + idEquipe;

            if (tipoInstalacao > 0)
            {
                sql += " And i.tipoInstalacao=" + tipoInstalacao;

                string descrTipoInst = String.Empty;
                foreach (GenericModel g in DataSources.Instance.GetTipoInstalacao())
                    if ((int)g.Id == tipoInstalacao)
                        descrTipoInst = g.Descr;

                criterio += "Tipo Instalação: " + descrTipoInst + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And i.dataFinal>=?dataIni";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
                lstParam.Add(new GDAParameter("?dataIniGar", DateTime.Parse(dataIni + " 00:00")));

                if (!String.IsNullOrEmpty(dataFim))
                    criterio += "Período: " + dataIni;
                else
                    criterio += "Instalações feitas a partir de: " + dataIni;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And i.dataFinal<=?dataFim";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
                lstParam.Add(new GDAParameter("?dataFimGar", DateTime.Parse(dataFim + " 23:59")));

                if (!String.IsNullOrEmpty(dataFim))
                    criterio += " a " + dataFim;
                else
                    criterio += "Instalações feitas até " + dataFim;
            }

            sql += " Group By e.idEquipe";

            var lstProducaoInst = objPersistence.LoadData(sql, lstParam.Count > 0 ? lstParam.ToArray() : null).ToList();

            if (lstProducaoInst.Count > 0)
                lstProducaoInst[0].Criterio = criterio;

            return lstProducaoInst.ToArray();
        }

        /// <summary>
        /// Retorna a produção detalhada referente às instalações feitas pelas equipes no período informado
        /// </summary>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public ProducaoInst[] GetProdInst(uint idEquipe, int tipoInstalacao, string dataIni, string dataFim)
        {
            var idsSubgrupo = SubgrupoProdDAO.Instance.GetSubgruposTemperados();

            if (String.IsNullOrEmpty(idsSubgrupo))
                idsSubgrupo = "0";

            string sql = @"
                Select Cast(pi.qtdeInstalada as decimal(12,2)) As QtdePecas,
                    (pi.qtdeInstalada * pp.totM)/pp.qtde As TotalM2, e.Nome As NomeEquipe,
                    (If(e.tipo=1, 'Comum', 'Temperado')) As TipoEquipe, i.idInstalacao As IdInstalacao,
                    Concat(p.codInterno, ' - ', p.descricao) As Produtos,
                    Concat(Truncate(pp.Altura, 0), ' x ', pp.Largura) As AlturaLargura,
                    Cast((sqlQtdeGarantia) As decimal(12,2)) As QtdeGarantia
                From produtos_instalacao pi
                    Inner Join produtos_pedido pp On (pp.idProdPed=pi.idProdPed)
                    Inner Join instalacao i On (pp.idPedido=i.idPedido)
                    Left Join pedido ped On (ped.idPedido=i.idPedido)
                    Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao)
                    Left Join equipe e On (ei.idEquipe=e.idEquipe)
                    Left Join produto p On (pp.idProd=p.idProd)
                Where p.idGrupoProd=1
                    /*And if (e.tipo=1, p.idSubgrupoProd Not In (" + idsSubgrupo + "), p.idSubgrupoProd In (" + idsSubgrupo + @") Or i.liberarTemperado=1)*/
                    And (i.Situacao=" + (int)Instalacao.SituacaoInst.Continuada + @"
                    Or i.Situacao=" + (int)Instalacao.SituacaoInst.Finalizada + @") And ped.TipoVenda<>" +
                    (int)Pedido.TipoVendaPedido.Garantia + " And pi.qtdeInstalada>0";

            string sqlGarantia = "Select Count(*) From instalacao inst Left Join equipe_instalacao ei On (inst.idInstalacao=ei.idInstalacao) " +
                "Left Join pedido pedi On(inst.idPedido=pedi.idPedido) Where ei.idEquipe=e.idEquipe " +
                "And pedi.TipoVenda=" + (int)Pedido.TipoVendaPedido.Garantia;

            string criterio = String.Empty;

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (idEquipe > 0)
                sql += " And e.idEquipe=" + idEquipe;

            if (tipoInstalacao > 0)
            {
                sql += " And i.tipoInstalacao=" + tipoInstalacao;

                string descrTipoInst = String.Empty;
                foreach (GenericModel g in DataSources.Instance.GetTipoInstalacao())
                    if ((int)g.Id == tipoInstalacao)
                        descrTipoInst = g.Descr;

                criterio += "Tipo Instalação: " + descrTipoInst + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And i.dataFinal>=?dataIni";
                sqlGarantia += " And inst.dataFinal>=?dataIniGar";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
                lstParam.Add(new GDAParameter("?dataIniGar", DateTime.Parse(dataIni + " 00:00")));

                if (!String.IsNullOrEmpty(dataFim))
                    criterio += "Período: " + dataIni;
                else
                    criterio += "Instalações feitas a partir de: " + dataIni;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And i.dataFinal<=?dataFim";
                sqlGarantia += " And inst.dataFinal<=?dataFimGar";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
                lstParam.Add(new GDAParameter("?dataFimGar", DateTime.Parse(dataFim + " 23:59")));

                if (!String.IsNullOrEmpty(dataFim))
                    criterio += " a " + dataFim;
                else
                    criterio += "Instalações feitas até " + dataFim;
            }
            
            List<ProducaoInst> lstProducaoInst = objPersistence.LoadData(sql.Replace("sqlQtdeGarantia", sqlGarantia),
                lstParam.Count > 0 ? lstParam.ToArray() : null);

            if (lstProducaoInst.Count > 0)
                lstProducaoInst[0].Criterio = criterio;

            return lstProducaoInst.ToArray();
        }
    }
}