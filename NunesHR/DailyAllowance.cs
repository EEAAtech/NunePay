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
    
    public partial class DailyAllowance
    {
        public int EmpID { get; set; }
        public System.DateTime AllowDate { get; set; }
        public System.DateTime SaveTime { get; set; }
    
        public virtual Employees Employees { get; set; }
    }
}