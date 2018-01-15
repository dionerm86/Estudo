using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using System.Linq;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoSituacaoDAO))]
    [PersistenceClass("producao_situacao")]
    public class ProducaoSituacao
    {
        #region Propriedades

        [PersistenceProperty("Data")]
        public DateTime Data { get; set; }

        [PersistenceProperty("DataConf")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("DataLiberacao")]
        public DateTime? DataLiberacao { get; set; }

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        [PersistenceProperty("NomesSetores")]
        public string NomesSetores { get; set; }

        [PersistenceProperty("SomasSetores")]
        public string SomasSetores { get; set; }

        [PersistenceProperty("IdsSetores")]
        public string IdsSetores { get; set; }

        [PersistenceProperty("IdsFuncSetores")]
        public string IdsFuncSetores { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NomeFuncRpt
        {
            get
            {
                uint idFunc;
                return uint.TryParse(IdsFuncSetores, out idFunc) ? FuncionarioDAO.Instance.GetNome(idFunc) : String.Empty;
            }
        }

        public uint?[] IdSetor
        {
            get
            {
                string[] ids = IdsSetores.Split(',');
                uint?[] retorno = new uint?[ids.Length];

                for (int i = 0; i < retorno.Length; i++)
                {
                    uint id;
                    retorno[i] = uint.TryParse(ids[i], out id) ? (uint?)id : null;
                }

                return retorno;
            }
        }

        private decimal[] _somaSetor = null;

        public decimal[] SomaSetor
        {
            get
            {
                if (_somaSetor == null)
                {
                    string[] somas = SomasSetores.Split(',');
                    Dictionary<uint, decimal> lista = new Dictionary<uint, decimal>();

                    for (int i = 0; i < somas.Length; i++)
                    {
                        decimal soma;
                        lista.Add(IdSetor != null && IdSetor.Length > i && IdSetor[i] != null ? IdSetor[i].Value : 0, decimal.TryParse(somas[i].Replace('.', ','), out soma) ? soma : 0);
                    }

                    _somaSetor = new decimal[Utils.GetSetores.Where(f => f.ExibirSetores).ToArray().Length + 1];
                    for (int i = 0; i < _somaSetor.Length; i++)
                    {
                        uint chave = IdSetor != null && IdSetor.Length > i && IdSetor[i] != null ? IdSetor[i].Value : 0;
                        if (chave > 0)
                        {
                            int posicao = i;
                            for (int j = 0; j < Utils.GetSetores.Where(f => f.ExibirSetores).ToArray().Length; j++)
                                if (Utils.GetSetores.Where(f => f.ExibirSetores).ToArray()[j].IdSetor == chave)
                                {
                                    posicao = j;
                                    break;
                                }

                            _somaSetor[posicao] = chave != 0 ? lista[chave] : 0;
                        }
                    }
                }

                return _somaSetor;
            }
        }

        private uint?[] _idFuncSetor;

        public uint?[] IdFuncSetor
        {
            get 
            {
                if (_idFuncSetor == null)
                {
                    string[] ids = IdsFuncSetores.Split(',');
                    Dictionary<uint, uint> lista = new Dictionary<uint, uint>();

                    for (int i = 0; i < ids.Length; i++)
                    {
                        uint id;
                        lista.Add(IdSetor[i] != null ? IdSetor[i].Value : 0, uint.TryParse(ids[i], out id) ? id : 0);
                    }

                    _idFuncSetor = new uint?[Utils.GetSetores.Where(f => f.ExibirSetores).ToArray().Length + 1];
                    for (int i = 0; i < _idFuncSetor.Length; i++)
                    {
                        uint chave = i < IdSetor.Length && IdSetor[i] != null ? IdSetor[i].Value : 0;
                        if (chave > 0)
                        {
                            int posicao = i;
                            for (int j = 0; j < Utils.GetSetores.Where(f => f.ExibirSetores).ToArray().Length; j++)
                                if (Utils.GetSetores.Where(f => f.ExibirSetores).ToArray()[j].IdSetor == chave)
                                {
                                    posicao = j;
                                    break;
                                }

                            _idFuncSetor[posicao] = chave != 0 ? lista[chave] : 0;
                        }
                    }
                }

                return _idFuncSetor;
            }
        }

        #endregion
    }
}