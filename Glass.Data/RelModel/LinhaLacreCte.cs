using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LinhaLacreCteDAO))]
    public class LinhaLacreCte
    {
        public string NumLacre1 { get; set; }
        public string NumLacre2 { get; set; }
        public string NumLacre3 { get; set; }        
    }
}
