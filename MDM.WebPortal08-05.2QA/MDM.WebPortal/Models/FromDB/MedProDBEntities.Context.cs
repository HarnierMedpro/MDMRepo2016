﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MDM.WebPortal.Models.FromDB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MedProDBEntities : DbContext
    {
        public MedProDBEntities()
            : base("name=MedProDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ADPMaster> ADPMasters { get; set; }
        public virtual DbSet<BI_DB_FvP_Access> BI_DB_FvP_Access { get; set; }
        public virtual DbSet<Corp_DBs> Corp_DBs { get; set; }
        public virtual DbSet<Corp_Owner> Corp_Owner { get; set; }
        public virtual DbSet<CorporateMasterList> CorporateMasterLists { get; set; }
        public virtual DbSet<CPTData> CPTDatas { get; set; }
        public virtual DbSet<DBList> DBLists { get; set; }
        public virtual DbSet<Edgemed_Logons> Edgemed_Logons { get; set; }
        public virtual DbSet<Facitity_DBs> Facitity_DBs { get; set; }
        public virtual DbSet<FvPList> FvPLists { get; set; }
        public virtual DbSet<Manager_Master> Manager_Master { get; set; }
        public virtual DbSet<Manager_Type> Manager_Type { get; set; }
        public virtual DbSet<MDM_POS_ListName> MDM_POS_ListName { get; set; }
        public virtual DbSet<MDM_POS_Name_DBPOS_grp> MDM_POS_Name_DBPOS_grp { get; set; }
        public virtual DbSet<OwnerList> OwnerLists { get; set; }
        public virtual DbSet<CollNoteTypeCatPriority> CollNoteTypeCatPriorities { get; set; }
        public virtual DbSet<ACCategory> ACCategories { get; set; }
        public virtual DbSet<ACPriority> ACPriorities { get; set; }
        public virtual DbSet<ActionCode> ActionCodes { get; set; }
        public virtual DbSet<ACtype> ACtypes { get; set; }
        public virtual DbSet<CodeMasterList> CodeMasterLists { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<MasterUserList> MasterUserLists { get; set; }
        public virtual DbSet<CorporateNameList> CorporateNameLists { get; set; }
        public virtual DbSet<ManagerDBAccessBI> ManagerDBAccessBIs { get; set; }
    }
}
