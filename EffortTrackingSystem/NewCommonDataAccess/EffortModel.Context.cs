﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewCommonDataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EffortTrackingSystemEntities : DbContext
    {
        public EffortTrackingSystemEntities()
            : base("name=EffortTrackingSystemEntities")
        {
        }

       /* public EffortTrackingSystemEntities(string connectionString)
                : base(connectionString)
        {

        }*/
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Assign_Task> Assign_Task { get; set; }
        public virtual DbSet<Effort> Efforts { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Shift_Change> Shift_Change { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}