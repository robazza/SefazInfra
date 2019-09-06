
namespace Sefaz.Infra.DbContexto.Model
{
    public partial class SistemaAmbienteEntidade
    {
        public short IdAmbiente { get; set; }
        public short IdSistema { get; set; }
      
        public virtual AmbienteEntidade AMBIENTE { get; set; }
       
    }
}
