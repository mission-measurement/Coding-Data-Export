//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MM.Data.Export
{
    using System;
    using System.Collections.Generic;
    
    public partial class coding_program_comments
    {
        public long id { get; set; }
        public long csid { get; set; }
        public long parentid { get; set; }
        public long programid { get; set; }
        public long gdataid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string tagwords { get; set; }
        public Nullable<long> importance { get; set; }
        public string pagenum { get; set; }
        public byte flagforreview { get; set; }
        public byte isnew { get; set; }
        public long flagged_roleid { get; set; }
        public Nullable<long> modified { get; set; }
        public long createdby { get; set; }
        public long created { get; set; }
    }
}
