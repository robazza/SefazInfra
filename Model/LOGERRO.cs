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
    
    public partial class LOGERRO
    {
        public int SQLOGERRO { get; set; }
        public short SQSISTEMA { get; set; }
        public string DSERRO { get; set; }
        public System.DateTime DTERRO { get; set; }
        public string NUERRO { get; set; }
        public string DSORIGEM { get; set; }
        public string DSLOGIN { get; set; }
        public string DCSTACKTRACE { get; set; }
    
        public virtual SISTEMA SISTEMA { get; set; }
    }
}
