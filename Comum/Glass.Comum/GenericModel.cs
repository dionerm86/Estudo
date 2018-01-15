namespace Glass
{
    public class GenericModel
    {
        #region Construtores

        public GenericModel(int idParam, string descrParam)
        {
            Id = (uint)idParam;
            Descr = descrParam;
        }

        public GenericModel(uint? idParam, string descrParam)
        {
            Id = idParam;
            Descr = descrParam;
        }

        #endregion

        #region Propriedades

        public uint? Id { get; set; }

        public string Descr { get; set; }

        #endregion
    }
}
