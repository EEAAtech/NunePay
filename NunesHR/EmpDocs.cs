//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NunesHR
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmpDocs
    {
        public int EDID { get; set; }
        public int EmpID { get; set; }
        public int EDocTypeID { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public bool Renewed { get; set; }
    
        public virtual EDocTypes EDocTypes { get; set; }
        public virtual Employees Employees { get; set; }
    }
}