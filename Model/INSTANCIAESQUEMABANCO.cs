//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sefaz.Infra.DbContexto.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class INSTANCIAESQUEMABANCO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INSTANCIAESQUEMABANCO()
        {
            this.SISTEMA = new HashSet<SISTEMA>();
        }
    
        public short SQESQUEMABANCO { get; set; }
        public short SQINSTANCIABANCO { get; set; }
    
        public virtual ESQUEMABANCO ESQUEMABANCO { get; set; }
        public virtual INSTANCIABANCO INSTANCIABANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SISTEMA> SISTEMA { get; set; }
    }
}
