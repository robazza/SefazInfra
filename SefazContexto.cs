using System.Data.Entity;


namespace Sefaz.Infra.DbContexto
{
    public  class SefazContexto : DbContext
    {
        public SefazContexto()
            : base(OracleContexto.CriarConexao(), true)
        {
           
        }

        public SefazContexto(string siglasistema)
           : base(OracleContexto.CriarConexao(siglasistema), true)
        {

        }

        public int commit { get; set; }     

       //public virtual int SaveChanges<TValue>()
       // {
       //     foreach (var dbEntityEntry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
       //     {
       //         IConcorrencia entidade = dbEntityEntry.Entity as IConcorrencia;

       //         if (entidade != null)
       //         {
       //             entidade.Versao++;
       //         }
                
       //         // Percorre todas as propriedades da entidade
       //         foreach (var propriedade in dbEntityEntry.Entity.GetType().GetProperties())
       //         {
       //             // Se a propriedade for do tipo string, remove os espaços em branco 
       //             // no incício e no final da propriedade com o método Trim()
       //             if (propriedade.PropertyType == typeof(string))
       //             {
       //                 DbPropertyEntry propertyEntry = dbEntityEntry.Property(propriedade.Name);
       //                 if (propertyEntry.CurrentValue != null)
       //                     propertyEntry.CurrentValue = propertyEntry.CurrentValue.ToString().Trim();
       //             }
       //         }
       //     }

       //     return base.SaveChanges();
       // }

    }
}
