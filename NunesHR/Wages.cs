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
    
    public partial class Wages
    {
        public int WageID { get; set; }
        public int EmpID { get; set; }
        public System.DateTime WEF { get; set; }
        public decimal Amount { get; set; }
    
        public virtual Employees Employees { get; set; }
    }
}
