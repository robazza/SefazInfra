using System.Collections.Generic;

namespace Sefaz.Infra.DbContexto.Model
{
    public partial class GrupoEntidade
    {
        public GrupoEntidade()
        {
            
            this.Usuario = new HashSet<UsuarioEntidade>();
            this.Funcao = new HashSet<FuncaoEntidade>();
        }

        public short IdGrupo { get; set; }
        public short IdSistema { get; set; }
        public string CdGrupo { get; set; }
        public string DsGrupo { get; set; }
        public string FlAdmin { get; set; }
        public string SenhaBD { get; set; }
        public virtual SistemaEntidade Sistema { get; set; }
        public virtual ICollection<UsuarioEntidade> Usuario { get; set; }
        public virtual ICollection<FuncaoEntidade> Funcao { get; set; }
    }
}
