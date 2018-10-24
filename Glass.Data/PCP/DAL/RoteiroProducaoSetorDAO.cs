using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class RoteiroProducaoSetorDAO : BaseDAO<RoteiroProducaoSetor, RoteiroProducaoSetorDAO>
    {
        public void ApagarPorRoteiroProducao(GDASession sessao, int idRoteiroProducao)
        {
            objPersistence.ExecuteCommand(sessao, "delete from roteiro_producao_setor where idRoteiroProducao=" + idRoteiroProducao);
        }

        public uint[] InserirPorRoteiroProducao(int idRoteiroProducao, IEnumerable<uint> idsSetores)
        {
            ApagarPorRoteiroProducao(null, idRoteiroProducao);

            List<uint> retorno = new List<uint>();

            foreach (var id in idsSetores)
                retorno.Add(Insert(new RoteiroProducaoSetor()
                {
                    IdRoteiroProducao = idRoteiroProducao,
                    IdSetor = id
                }));

            return retorno.ToArray();
        }

        public IList<RoteiroProducaoSetor> ObtemPorRoteiroProducao(int idRoteiroProducao)
        {
            return objPersistence.LoadData("select * from roteiro_producao_setor where idRoteiroProducao=" + idRoteiroProducao).ToList();
        }
    }
}
