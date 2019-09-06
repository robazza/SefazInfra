using System.Collections.Generic;

namespace Sefaz.Infra.DbContexto.Model
{
    public partial class FuncaoEntidade
    {
        public FuncaoEntidade()
        {
            this.Grupo = new HashSet<GrupoEntidade>();
            
        }

        public short IdFuncao { get; set; }
        public short IdSistema { get; set; }
        public string CdFuncao { get; set; }
        public string Nome { get; set; }
        public virtual SistemaEntidade Sistema { get; set; }
        public virtual ICollection<GrupoEntidade> Grupo { get; set; }
    }
}
