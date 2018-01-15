using Glass.Data.Model;
using GDA;
using System;

namespace Glass.Data.DAL
{
    public class RodoviarioMDFeDAO :BaseDAO<RodoviarioMDFe, RodoviarioMDFeDAO>
    {
        #region Busca padrão

        private string Sql(int idManifestoEletronico, bool selecionar)
        {
            var campos = selecionar ? "rm.*" : "COUNT(*)";

            var sql = "SELECT " + campos + @"
                FROM rodoviario_mdfe rm
                WHERE 1";

            if (idManifestoEletronico > 0)
            {
                sql += " AND rm.IdManifestoEletronico=" + idManifestoEletronico;
            }

            return sql;
        }

        public RodoviarioMDFe ObterRodoviarioPeloManifesto(int idManifestoEletronico)
        {
            return objPersistence.LoadOneData(Sql(idManifestoEletronico, true));
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(GDASession session, RodoviarioMDFe objInsert)
        {
            var idRodoviario = (int)base.Insert(session, objInsert);

            if (objInsert.CondutorVeiculo == null || objInsert.CondutorVeiculo.Count == 0)
                throw new Exception("É necessário informar ao menos um condutor.");

            // Salva CIOT
            if (objInsert.CiotRodoviario != null)
            {
                foreach (var ci in objInsert.CiotRodoviario)
                {
                    ci.IdRodoviario = idRodoviario;
                    CiotRodoviarioMDFeDAO.Instance.Insert(session, ci);
                }
            }

            // Salva Pedagio
            if (objInsert.PedagioRodoviario != null)
            {
                foreach (var pe in objInsert.PedagioRodoviario)
                {
                    pe.IdRodoviario = idRodoviario;
                    PedagioRodoviarioMDFeDAO.Instance.Insert(session, pe);
                }
            }

            // Salva Condutor
            if (objInsert.CondutorVeiculo != null)
            {
                foreach (var co in objInsert.CondutorVeiculo)
                {
                    co.IdRodoviario = idRodoviario;
                    CondutorVeiculoMDFeDAO.Instance.Insert(session, co);
                }
            }

            // Salva Veiculo Reboque
            if (objInsert.VeiculoRodoviario != null)
            {
                foreach (var vr in objInsert.VeiculoRodoviario)
                {
                    vr.IdRodoviario = idRodoviario;
                    VeiculoRodoviarioMDFeDAO.Instance.Insert(session, vr);
                }
            }

            // Salva Lacre
            if (objInsert.LacreRodoviario != null)
            {
                foreach (var la in objInsert.LacreRodoviario)
                {
                    la.IdRodoviario = idRodoviario;
                    LacreRodoviarioMDFeDAO.Instance.Insert(session, la);
                }
            }

            return (uint)idRodoviario;
        }

        public override int Update(GDASession session, RodoviarioMDFe objUpdate)
        {
            var idRodoviario = objUpdate.IdRodoviario;
            var resultado = base.Update(session, objUpdate);

            if (objUpdate.CondutorVeiculo == null || objUpdate.CondutorVeiculo.Count == 0)
                throw new Exception("É necessário informar ao menos um condutor.");

            // Salva CIOT
            CiotRodoviarioMDFeDAO.Instance.DeletarPorIdRodoviario(session, idRodoviario);
            if (objUpdate.CiotRodoviario != null)
            {
                foreach (var ci in objUpdate.CiotRodoviario)
                {
                    ci.IdRodoviario = idRodoviario;
                    CiotRodoviarioMDFeDAO.Instance.Insert(session, ci);
                }
            }

            // Salva Pedagio
            PedagioRodoviarioMDFeDAO.Instance.DeletarPorIdRodoviario(session, idRodoviario);
            if (objUpdate.PedagioRodoviario != null)
            {
                foreach (var pe in objUpdate.PedagioRodoviario)
                {
                    pe.IdRodoviario = idRodoviario;
                    PedagioRodoviarioMDFeDAO.Instance.Insert(session, pe);
                }
            }

            // Salva Condutor
            CondutorVeiculoMDFeDAO.Instance.DeletarPorIdRodoviario(session, idRodoviario);
            if (objUpdate.CondutorVeiculo != null)
            {
                foreach (var co in objUpdate.CondutorVeiculo)
                {
                    co.IdRodoviario = idRodoviario;
                    CondutorVeiculoMDFeDAO.Instance.Insert(session, co);
                }
            }

            // Salva Veiculo Reboque
            VeiculoRodoviarioMDFeDAO.Instance.DeletarPorIdRodoviario(session, idRodoviario);
            if (objUpdate.VeiculoRodoviario != null)
            {
                foreach (var vr in objUpdate.VeiculoRodoviario)
                {
                    vr.IdRodoviario = idRodoviario;
                    VeiculoRodoviarioMDFeDAO.Instance.Insert(session, vr);
                }
            }

            // Salva Lacre
            LacreRodoviarioMDFeDAO.Instance.DeletarPorIdRodoviario(session, idRodoviario);
            if (objUpdate.LacreRodoviario != null)
            {
                foreach (var la in objUpdate.LacreRodoviario)
                {
                    la.IdRodoviario = idRodoviario;
                    LacreRodoviarioMDFeDAO.Instance.Insert(session, la);
                }
            }

            return resultado;
        }

        #endregion
    }
}
