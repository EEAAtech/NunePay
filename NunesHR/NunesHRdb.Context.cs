﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class NTHRPayEntities1 : DbContext
    {
        public NTHRPayEntities1()
            : base("name=NTHRPayEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Advance> Advance { get; set; }
        public virtual DbSet<Allowance> Allowance { get; set; }
        public virtual DbSet<AllowanceTypes> AllowanceTypes { get; set; }
        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<Bonus> Bonus { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<DailyAllowance> DailyAllowance { get; set; }
        public virtual DbSet<EDocTypes> EDocTypes { get; set; }
        public virtual DbSet<EmpDocs> EmpDocs { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<EmploymentHistory> EmploymentHistory { get; set; }
        public virtual DbSet<EmpTypes> EmpTypes { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<LoanPay> LoanPay { get; set; }
        public virtual DbSet<Loans> Loans { get; set; }
        public virtual DbSet<LoanSkip> LoanSkip { get; set; }
        public virtual DbSet<Payroll> Payroll { get; set; }
        public virtual DbSet<PayrollAllowance> PayrollAllowance { get; set; }
        public virtual DbSet<PayrollRemarks> PayrollRemarks { get; set; }
        public virtual DbSet<Wages> Wages { get; set; }
    
        public virtual int FreezePayroll(Nullable<int> month, Nullable<int> year)
        {
            var monthParameter = month.HasValue ?
                new ObjectParameter("month", month) :
                new ObjectParameter("month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("year", year) :
                new ObjectParameter("year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("FreezePayroll", monthParameter, yearParameter);
        }
    
        public virtual int GenBonus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GenBonus");
        }
    
        public virtual int GenPayroll(Nullable<int> month, Nullable<int> year)
        {
            var monthParameter = month.HasValue ?
                new ObjectParameter("month", month) :
                new ObjectParameter("month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("year", year) :
                new ObjectParameter("year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GenPayroll", monthParameter, yearParameter);
        }
    
        public virtual ObjectResult<Get5yrBonus_Result> Get5yrBonus(Nullable<int> yr)
        {
            var yrParameter = yr.HasValue ?
                new ObjectParameter("yr", yr) :
                new ObjectParameter("yr", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Get5yrBonus_Result>("Get5yrBonus", yrParameter);
        }
    
        public virtual ObjectResult<GetLoanHistory_Result> GetLoanHistory(Nullable<int> empID)
        {
            var empIDParameter = empID.HasValue ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetLoanHistory_Result>("GetLoanHistory", empIDParameter);
        }
    }
}