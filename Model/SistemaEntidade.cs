using System.Collections.Generic;

namespace Sefaz.Infra.DbContexto.Model
{
    public partial class SistemaEntidade
    {
        public SistemaEntidade()
        {

            this.Ambiente = new HashSet<SistemaAmbienteEntidade>();
            this.Dominio = new HashSet<DominioEntidade>();
           
        }

        public short IdSistema { get; set; }
        public string CodSistema { get; set; }
        public string Nome { get; set; }
       
        public virtual ICollection<SistemaAmbienteEntidade> Ambiente { get; set; }

        public virtual ICollection<DominioEntidade> Dominio { get; set; }
      
    }
}
