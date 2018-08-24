using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(RegraNaturezaOperacaoDAO))]
    [PersistenceClass("regra_natureza_operacao")]
    public class RegraNaturezaOperacao : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDREGRANATUREZAOPERACAO", PersistenceParameterType.IdentityKey)]
        public int IdRegraNaturezaOperacao { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Data.Model.Loja), "IdLoja")]
        public int? IdLoja { get; set; }

        [Log("Tipo de Cliente", "Descricao", typeof(TipoClienteDAO))]
        [PersistenceProperty("IDTIPOCLIENTE")]
        [PersistenceForeignKey(typeof(Data.Model.TipoCliente), "IdTipoCliente")]
        public int? IdTipoCliente { get; set; }

        [Log("Grupo de Produto", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        [PersistenceForeignKey(typeof(Data.Model.GrupoProd), "IdGrupoProd")]
        public int? IdGrupoProd { get; set; }

        [Log("Subgrupo de Produto", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        [PersistenceForeignKey(typeof(Data.Model.SubgrupoProd), "IdSubgrupoProd")]
        public int? IdSubgrupoProd { get; set; }

        [Log("Cor do Vidro", "Descricao", typeof(CorVidroDAO))]
        [PersistenceProperty("IDCORVIDRO")]
        public int? IdCorVidro { get; set; }

        [Log("Cor do Alumínio", "Descricao", typeof(CorAluminioDAO))]
        [PersistenceProperty("IDCORALUMINIO")]
        [PersistenceForeignKey(typeof(Data.Model.CorAluminio), "IdCorAluminio")]
        public int? IdCorAluminio { get; set; }

        [Log("Cor da Ferragem", "Descricao", typeof(CorFerragemDAO))]
        [PersistenceProperty("IDCORFERRAGEM")]
        [PersistenceForeignKey(typeof(Data.Model.CorFerragem), "IdCorFerragem")]
        public int? IdCorFerragem { get; set; }

        [Log("Espessura")]
        [PersistenceProperty("ESPESSURA")]
        public float? Espessura { get; set; }

        [Log("Natureza Operação Produção - Intraestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOPRODINTRA")]
        public int IdNaturezaOperacaoProdIntra { get; set; }

        [Log("Natureza Operação Revenda - Intraestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOREVINTRA")]
        public int IdNaturezaOperacaoRevIntra { get; set; }

        [Log("Natureza Operação Produção - Interestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOPRODINTER")]
        public int IdNaturezaOperacaoProdInter { get; set; }

        [Log("Natureza Operação Revenda - Interestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOREVINTER")]
        public int IdNaturezaOperacaoRevInter { get; set; }

        [Log("Natureza Operação Produção ST - Intraestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOPRODSTINTRA")]
        public int? IdNaturezaOperacaoProdStIntra { get; set; }

        [Log("Natureza Operação Produção ST - Intraestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOREVSTINTRA")]
        public int? IdNaturezaOperacaoRevStIntra { get; set; }

        [Log("Natureza Operação Produção ST - Interestadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOPRODSTINTER")]
        public int? IdNaturezaOperacaoProdStInter { get; set; }

        [Log("Natureza Operação Produção ST - Intrerstadual", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAOREVSTINTER")]
        public int? IdNaturezaOperacaoRevStInter { get; set; }

        [PersistenceProperty("UfDest")]
        public string UfDest { get; set; }

        #endregion

        #region Propriedades extendidas

        public string Descricao
        {
            get { return RegraNaturezaOperacaoDAO.Instance.ObtemDescricao((uint)IdRegraNaturezaOperacao); }
        }

        #endregion
    }
}
