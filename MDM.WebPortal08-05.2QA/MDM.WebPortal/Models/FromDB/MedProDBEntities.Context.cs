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
        public virtual DbSet<Edgemed_Logons> Edgemed_Logons { get; set; }
        public virtual DbSet<FvPList> FvPLists { get; set; }
        public virtual DbSet<Manager_Master> Manager_Master { get; set; }
        public virtual DbSet<Manager_Type> Manager_Type { get; set; }
        public virtual DbSet<ACCategory> ACCategories { get; set; }
        public virtual DbSet<ACPriority> ACPriorities { get; set; }
        public virtual DbSet<ActionCode> ActionCodes { get; set; }
        public virtual DbSet<ACtype> ACtypes { get; set; }
        public virtual DbSet<CodeMasterList> CodeMasterLists { get; set; }
        public virtual DbSet<DBList> DBLists { get; set; }
        public virtual DbSet<CorporateMasterList> CorporateMasterLists { get; set; }
        public virtual DbSet<MasterPOS> MasterPOS { get; set; }
        public virtual DbSet<Lev_of_Care> Lev_of_Care { get; set; }
        public virtual DbSet<MasterPOS_LevOfCare> MasterPOS_LevOfCare { get; set; }
        public virtual DbSet<MasterPOS_MPServ> MasterPOS_MPServ { get; set; }
        public virtual DbSet<MPService> MPServices { get; set; }
        public virtual DbSet<POSAddr> POSAddrs { get; set; }
        public virtual DbSet<FACInfo> FACInfoes { get; set; }
        public virtual DbSet<PHYGroup> PHYGroups { get; set; }
        public virtual DbSet<ProvidersInGrp> ProvidersInGrps { get; set; }
        public virtual DbSet<FileTypeI> FileTypeIs { get; set; }
        public virtual DbSet<Forms_sent> Forms_sent { get; set; }
        public virtual DbSet<FormsDict> FormsDicts { get; set; }
        public virtual DbSet<MasterFile> MasterFiles { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<MasterPOS_Contact> MasterPOS_Contact { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<ContactType_Contact> ContactType_Contact { get; set; }
        public virtual DbSet<Corp_Owner> Corp_Owner { get; set; }
        public virtual DbSet<CPTData> CPTDatas { get; set; }
        public virtual DbSet<ZoomDB_POSID_grp> ZoomDB_POSID_grp { get; set; }
        public virtual DbSet<Facitity_DBs> Facitity_DBs { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<InfoData> InfoDatas { get; set; }
        public virtual DbSet<POSExtraData> POSExtraDatas { get; set; }
    }
}
