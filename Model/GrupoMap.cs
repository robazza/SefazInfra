using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Sefaz.Infra.DbContexto.Model
{
    public class GrupoMap : EntityTypeConfiguration<GrupoEntidade>
    {
        public GrupoMap()
        {
            this.ToTable("GRUPO","DBASAS");

            this.HasKey(r => r.IdGrupo);

            this.Property(r => r.IdGrupo).HasColumnName("SQGRUPO").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(r => r.IdSistema).IsRequired().HasColumnName("SQSISTEMA");
            this.Property(r => r.CdGrupo).IsRequired().HasMaxLength(32).HasColumnName("CDGRUPO");
            this.Property(r => r.DsGrupo).HasMaxLength(100).HasColumnName("DSGRUPO");
            this.Property(r => r.FlAdmin).HasColumnName("FLADMINISTRATIVO");
            this.Property(r => r.SenhaBD).HasColumnName("DSSENHABANCODADOS");

            this.HasMany(r => r.Funcao);
            this.HasMany(r => r.Usuario).WithMany(u => u.Grupo);
            this.HasRequired(r => r.Sistema);

        }
    }
}
