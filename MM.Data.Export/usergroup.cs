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
    
    public partial class usergroup
    {
        public long groupid { get; set; }
        public long parentgroupid { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string logourl { get; set; }
        public int defaultacclvl { get; set; }
        public byte islocked { get; set; }
        public Nullable<long> startdate { get; set; }
        public Nullable<long> expiredate { get; set; }
        public long created { get; set; }
        public long createdby { get; set; }
    }
}
