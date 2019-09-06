using System;
using System.Collections.Generic;

namespace Sefaz.Infra.DbContexto.Model
{
    public partial class UsuarioEntidade
    {
        public UsuarioEntidade()
        {

            this.Grupo = new HashSet<GrupoEntidade>();
        }

        public short IdUsuario { get; set; }
        public string Matricula { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string SenhaHash { get; set; }
        public Nullable<System.DateTime> DataExpiracao { get; set; }
        public string FlAtivo { get; set; }

        public virtual ICollection<GrupoEntidade> Grupo { get; set; }
    }
}
