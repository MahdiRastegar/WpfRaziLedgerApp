using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class wpfrazydbContext : DbContext
    {
        public wpfrazydbContext()
        {
        }

        public wpfrazydbContext(DbContextOptions<wpfrazydbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AcDocumentDetail> AcDocumentDetails { get; set; }
        public virtual DbSet<AcDocumentHeader> AcDocumentHeaders { get; set; }
        public virtual DbSet<Agroup> Agroups { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<ChEvent> ChEvents { get; set; }
        public virtual DbSet<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual DbSet<CheckRecieveEvent> CheckRecieveEvents { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CodeSetting> CodeSettings { get; set; }
        public virtual DbSet<CodingReceiptType> CodingReceiptTypes { get; set; }
        public virtual DbSet<CodingTypesTransfer> CodingTypesTransfers { get; set; }
        public virtual DbSet<Col> Cols { get; set; }
        public virtual DbSet<Commodity> Commodities { get; set; }
        public virtual DbSet<CommodityPricingPanel> CommodityPricingPanels { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<GroupCommodity> GroupCommodities { get; set; }
        public virtual DbSet<GroupStorage> GroupStorages { get; set; }
        public virtual DbSet<Moein> Moeins { get; set; }
        public virtual DbSet<MoneyType666> MoneyType666s { get; set; }
        public virtual DbSet<NpstorageDetail> NpstorageDetails { get; set; }
        public virtual DbSet<NpstorageHeader> NpstorageHeaders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<PaymentMoneyDetail> PaymentMoneyDetails { get; set; }
        public virtual DbSet<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PreInvoiceDetail> PreInvoiceDetails { get; set; }
        public virtual DbSet<PreInvoiceHeader> PreInvoiceHeaders { get; set; }
        public virtual DbSet<Preferential> Preferentials { get; set; }
        public virtual DbSet<PriceGroup> PriceGroups { get; set; }
        public virtual DbSet<ProductBuyDetail> ProductBuyDetails { get; set; }
        public virtual DbSet<ProductBuyHeader> ProductBuyHeaders { get; set; }
        public virtual DbSet<ProductSellDetail> ProductSellDetails { get; set; }
        public virtual DbSet<ProductSellHeader> ProductSellHeaders { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<RecieveMoneyDetail> RecieveMoneyDetails { get; set; }
        public virtual DbSet<RecieveMoneyHeader> RecieveMoneyHeaders { get; set; }
        public virtual DbSet<RibbonItem> RibbonItems { get; set; }
        public virtual DbSet<Storage> Storages { get; set; }
        public virtual DbSet<StorageReceiptDetail> StorageReceiptDetails { get; set; }
        public virtual DbSet<StorageReceiptHeader> StorageReceiptHeaders { get; set; }
        public virtual DbSet<StorageRotationDetail> StorageRotationDetails { get; set; }
        public virtual DbSet<StorageRotationHeader> StorageRotationHeaders { get; set; }
        public virtual DbSet<StorageTransferDetail> StorageTransferDetails { get; set; }
        public virtual DbSet<StorageTransferHeader> StorageTransferHeaders { get; set; }
        public virtual DbSet<TGroup> TGroups { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<UserApp> UserApps { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<Version> Versions { get; set; }

        public override int SaveChanges()
        {
            SetCurrentPeriodId();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetCurrentPeriodId();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetCurrentPeriodId()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var property = entry.Entity.GetType().GetProperty("FkPeriodId");
                if (property != null && property.PropertyType == typeof(Nullable<Guid>))
                {
                    property.SetValue(entry.Entity, MainWindow.StatusOptions.Period.Id);
                }
            }
        }
        public static string cs = "";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=wpfrazydb;Trusted_Connection=False;User Id=sa;Password=123456;");
                var str = "";

                if (cs == "")
                    try
                    {
                        str = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cs.txt"));

                        cs = str;
                    }

                    catch
                    { }

                if (cs != "")
                    optionsBuilder.UseSqlServer(cs);
                else
                    optionsBuilder.UseSqlServer("Server=.;Database=wpfrazydb;Trusted_Connection=False;User Id=sa;Password=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Arabic_CI_AS");

            modelBuilder.Entity<AcDocumentDetail>(entity =>
            {
                entity.ToTable("AcDocument_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Creditor).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Debtor).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkAcDocHeaderId).HasColumnName("fk_AcDoc_HeaderId");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.HasOne(d => d.FkAcDocHeader)
                    .WithMany(p => p.AcDocumentDetails)
                    .HasForeignKey(d => d.FkAcDocHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AcDocument_Detail_AcDocument_Header");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.AcDocumentDetails)
                    .HasForeignKey(d => d.FkMoeinId)
                    .HasConstraintName("FK_AcDocument_Detail_Moein");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.AcDocumentDetails)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .HasConstraintName("FK_AcDocument_Detail_Preferential");
            });

            modelBuilder.Entity<AcDocumentHeader>(entity =>
            {
                entity.ToTable("AcDocument_Header");

                entity.HasIndex(e => e.NoDoument, "UQ_AcDocument_Header_NoDoument")
                    .IsUnique();

                entity.HasIndex(e => e.Serial, "UQ_AcDocument_Header_Serial")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkDocumentTypeId).HasColumnName("fk_DocumentTypeId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.HasOne(d => d.FkDocumentType)
                    .WithMany(p => p.AcDocumentHeaders)
                    .HasForeignKey(d => d.FkDocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AcDocument_Header_DocumentType");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.AcDocumentHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_AcDocument_Header_Period");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<Agroup>(entity =>
            {
                entity.ToTable("AGroup");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Bank");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ChEvent>(entity =>
            {
                entity.ToTable("ChEvent");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CheckPaymentEvent>(entity =>
            {
                entity.ToTable("CheckPaymentEvent");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EventDate).HasColumnType("datetime");

                entity.Property(e => e.FkAcId).HasColumnName("fk_AcId");

                entity.Property(e => e.FkChEventId).HasColumnName("fk_ChEventId");

                entity.Property(e => e.FkDetaiId).HasColumnName("fk_DetaiId");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.Indexer).ValueGeneratedOnAdd();

                entity.HasOne(d => d.FkAc)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkAcId)
                    .HasConstraintName("FK_CheckPaymentEvents_AcDocument_Header");

                entity.HasOne(d => d.FkChEvent)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkChEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckPaymentEvents_ChEvent");

                entity.HasOne(d => d.FkDetai)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkDetaiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckPaymentEvents_PaymentMoney_Detail");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkMoeinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckPaymentEvents_Moein");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_CheckPaymentEvent_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.CheckPaymentEvents)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckPaymentEvents_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<CheckRecieveEvent>(entity =>
            {
                entity.ToTable("CheckRecieveEvent");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EventDate).HasColumnType("datetime");

                entity.Property(e => e.FkAcId).HasColumnName("fk_AcId");

                entity.Property(e => e.FkChEventId).HasColumnName("fk_ChEventId");

                entity.Property(e => e.FkDetaiId).HasColumnName("fk_DetaiId");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.Indexer).ValueGeneratedOnAdd();

                entity.HasOne(d => d.FkAc)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkAcId)
                    .HasConstraintName("FK_CheckRecieveEvents_AcDocument_Header");

                entity.HasOne(d => d.FkChEvent)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkChEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckRecieveEvents_ChEvent");

                entity.HasOne(d => d.FkDetai)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkDetaiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckRecieveEvents_RecieveMoney_Detail");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkMoeinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckRecieveEvents_Moein");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_CheckRecieveEvent_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.CheckRecieveEvents)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckRecieveEvents_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkProvinceId).HasColumnName("fk_ProvinceId");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.FkProvince)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.FkProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_City_Province");
            });

            modelBuilder.Entity<CodeSetting>(entity =>
            {
                entity.ToTable("CodeSetting");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IdValue).HasColumnName("Id_Value");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CodingReceiptType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<CodingTypesTransfer>(entity =>
            {
                entity.ToTable("CodingTypesTransfer");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Col>(entity =>
            {
                entity.ToTable("Col");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkGroupId).HasColumnName("fk_GroupId");

                entity.HasOne(d => d.FkGroup)
                    .WithMany(p => p.Cols)
                    .HasForeignKey(d => d.FkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Col_AGroup");
            });

            modelBuilder.Entity<Commodity>(entity =>
            {
                entity.ToTable("Commodity");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkGroupId).HasColumnName("fk_GroupId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkUnitId).HasColumnName("fk_UnitId");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.FkGroup)
                    .WithMany(p => p.Commodities)
                    .HasForeignKey(d => d.FkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commodity_GroupCommodity");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.Commodities)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_Commodity_Period");

                entity.HasOne(d => d.FkUnit)
                    .WithMany(p => p.Commodities)
                    .HasForeignKey(d => d.FkUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commodity_Unit");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<CommodityPricingPanel>(entity =>
            {
                entity.ToTable("CommodityPricingPanel");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPriceGroupId).HasColumnName("fk_PriceGroupId");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.CommodityPricingPanels)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommodityPricingPanel_Commodity");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.CommodityPricingPanels)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_CommodityPricingPanel_Period");

                entity.HasOne(d => d.FkPriceGroup)
                    .WithMany(p => p.CommodityPricingPanels)
                    .HasForeignKey(d => d.FkPriceGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommodityPricingPanel_PriceGroup");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<CustomerGroup>(entity =>
            {
                entity.ToTable("CustomerGroup");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkGroupId).HasColumnName("fk_GroupId");

                entity.HasOne(d => d.FkGroup)
                    .WithMany(p => p.CustomerGroups)
                    .HasForeignKey(d => d.FkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerGroup_PriceGroup");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<GroupCommodity>(entity =>
            {
                entity.ToTable("GroupCommodity");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<GroupStorage>(entity =>
            {
                entity.ToTable("GroupStorage");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Moein>(entity =>
            {
                entity.ToTable("Moein");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkColId).HasColumnName("fk_ColId");

                entity.HasOne(d => d.FkCol)
                    .WithMany(p => p.Moeins)
                    .HasForeignKey(d => d.FkColId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Moein_Col");
            });

            modelBuilder.Entity<MoneyType666>(entity =>
            {
                entity.ToTable("MoneyType666");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.RequiredBank).HasColumnName("required_Bank");

                entity.Property(e => e.RequiredDate).HasColumnName("required_Date");

                entity.Property(e => e.RequiredNumber).HasColumnName("required_Number");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.MoneyType666s)
                    .HasForeignKey(d => d.FkMoeinId)
                    .HasConstraintName("FK_MoneyType_Moein");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.MoneyType666s)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .HasConstraintName("FK_MoneyType_Preferential");
            });

            modelBuilder.Entity<NpstorageDetail>(entity =>
            {
                entity.ToTable("NPStorage_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.NpstorageDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NPStorage_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.NpstorageDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NPStorage_Detail_NPStorageHeader");
            });

            modelBuilder.Entity<NpstorageHeader>(entity =>
            {
                entity.ToTable("NPStorageHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Detail_OrderHeader");
            });

            modelBuilder.Entity<OrderHeader>(entity =>
            {
                entity.ToTable("OrderHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.OrderHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_OrderHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.OrderHeaders)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderHeader_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<PaymentMoneyDetail>(entity =>
            {
                entity.ToTable("PaymentMoney_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkBank).HasColumnName("fkBank");

                entity.Property(e => e.FkHeaderId).HasColumnName("fkHeaderId");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkBankNavigation)
                    .WithMany(p => p.PaymentMoneyDetails)
                    .HasForeignKey(d => d.FkBank)
                    .HasConstraintName("FK_PaymentMoney_Detail_Bank");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.PaymentMoneyDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMoney_Detail_PaymentMoneyHeader");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.PaymentMoneyDetails)
                    .HasForeignKey(d => d.FkMoeinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMoney_Detail_Moein");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.PaymentMoneyDetails)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMoney_Detail_Preferential");
            });

            modelBuilder.Entity<PaymentMoneyHeader>(entity =>
            {
                entity.ToTable("PaymentMoneyHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkAcDocument).HasColumnName("fkAcDocument");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.HasOne(d => d.FkAcDocumentNavigation)
                    .WithMany(p => p.PaymentMoneyHeaders)
                    .HasForeignKey(d => d.FkAcDocument)
                    .HasConstraintName("FK_PaymentMoneyHeader_AcDocument");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.PaymentMoneyHeaders)
                    .HasForeignKey(d => d.FkMoeinId)
                    .HasConstraintName("FK_PaymentMoneyHeader_Moein");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.PaymentMoneyHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_PaymentMoneyHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.PaymentMoneyHeaders)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .HasConstraintName("FK_PaymentMoneyHeader_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.ToTable("Period");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkRibbonItemId).HasColumnName("fkRibbonItemId");

                entity.Property(e => e.FkUserGroupId).HasColumnName("fkUserGroupId");

                entity.HasOne(d => d.FkRibbonItem)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.FkRibbonItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_RibbonItem");

                entity.HasOne(d => d.FkUserGroup)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.FkUserGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_UserGroup");
            });

            modelBuilder.Entity<PreInvoiceDetail>(entity =>
            {
                entity.ToTable("PreInvoice_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.PreInvoiceDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PreInvoice_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.PreInvoiceDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PreInvoice_Detail_PreInvoiceHeader");
            });

            modelBuilder.Entity<PreInvoiceHeader>(entity =>
            {
                entity.ToTable("PreInvoiceHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.InvoiceDiscount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SumDiscount).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.PreInvoiceHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_PreInvoiceHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.PreInvoiceHeaders)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PreInvoiceHeader_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<Preferential>(entity =>
            {
                entity.ToTable("Preferential");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkCityId).HasColumnName("fk_CityId");

                entity.Property(e => e.FkGroupId).HasColumnName("fk_GroupId");

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.Preferentials)
                    .HasForeignKey(d => d.FkCityId)
                    .HasConstraintName("FK_Preferential_City");

                entity.HasOne(d => d.FkGroup)
                    .WithMany(p => p.Preferentials)
                    .HasForeignKey(d => d.FkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Preferential_tGroup");
            });

            modelBuilder.Entity<PriceGroup>(entity =>
            {
                entity.ToTable("PriceGroup");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ProductBuyDetail>(entity =>
            {
                entity.ToTable("ProductBuy_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.ProductBuyDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductBuy_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.ProductBuyDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductBuy_Detail_ProductBuyHeader");
            });

            modelBuilder.Entity<ProductBuyHeader>(entity =>
            {
                entity.ToTable("ProductBuyHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkAcDocument).HasColumnName("fkAcDocument");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.InvoiceDiscount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ShippingCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SumDiscount).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkAcDocumentNavigation)
                    .WithMany(p => p.ProductBuyHeaders)
                    .HasForeignKey(d => d.FkAcDocument)
                    .HasConstraintName("FK_ProductBuyHeader_AcDocument");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.ProductBuyHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_ProductBuyHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.ProductBuyHeaders)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductBuyHeader_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<ProductSellDetail>(entity =>
            {
                entity.ToTable("ProductSell_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.ProductSellDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductSell_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.ProductSellDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductSell_Detail_ProductSellHeader");
            });

            modelBuilder.Entity<ProductSellHeader>(entity =>
            {
                entity.ToTable("ProductSellHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkAcDocument).HasColumnName("fkAcDocument");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.FkPreferentialIdDriver).HasColumnName("fk_PreferentialId_driver");

                entity.Property(e => e.FkPreferentialIdFreight).HasColumnName("fk_PreferentialId_freight");

                entity.Property(e => e.FkPreferentialIdPersonnel).HasColumnName("fk_PreferentialId_personnel");

                entity.Property(e => e.FkPreferentialIdReceiver).HasColumnName("fk_PreferentialId_receiver");

                entity.Property(e => e.InvoiceDiscount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ShippingCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SumDiscount).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkAcDocumentNavigation)
                    .WithMany(p => p.ProductSellHeaders)
                    .HasForeignKey(d => d.FkAcDocument)
                    .HasConstraintName("FK_ProductSellHeader_AcDocument");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.ProductSellHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_ProductSellHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.ProductSellHeaderFkPreferentials)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductSellHeader_Preferential");

                entity.HasOne(d => d.FkPreferentialIdDriverNavigation)
                    .WithMany(p => p.ProductSellHeaderFkPreferentialIdDriverNavigations)
                    .HasForeignKey(d => d.FkPreferentialIdDriver)
                    .HasConstraintName("FK_ProductSellHeader_Preferential1");

                entity.HasOne(d => d.FkPreferentialIdFreightNavigation)
                    .WithMany(p => p.ProductSellHeaderFkPreferentialIdFreightNavigations)
                    .HasForeignKey(d => d.FkPreferentialIdFreight)
                    .HasConstraintName("FK_ProductSellHeader_Preferential2");

                entity.HasOne(d => d.FkPreferentialIdPersonnelNavigation)
                    .WithMany(p => p.ProductSellHeaderFkPreferentialIdPersonnelNavigations)
                    .HasForeignKey(d => d.FkPreferentialIdPersonnel)
                    .HasConstraintName("FK_ProductSellHeader_Preferential3");

                entity.HasOne(d => d.FkPreferentialIdReceiverNavigation)
                    .WithMany(p => p.ProductSellHeaderFkPreferentialIdReceiverNavigations)
                    .HasForeignKey(d => d.FkPreferentialIdReceiver)
                    .HasConstraintName("FK_ProductSellHeader_Preferential4");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<RecieveMoneyDetail>(entity =>
            {
                entity.ToTable("RecieveMoney_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkBank).HasColumnName("fkBank");

                entity.Property(e => e.FkHeaderId).HasColumnName("fkHeaderId");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkBankNavigation)
                    .WithMany(p => p.RecieveMoneyDetails)
                    .HasForeignKey(d => d.FkBank)
                    .HasConstraintName("FK_RecieveMoney_Detail_Bank");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.RecieveMoneyDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecieveMoney_Detail_RecieveMoneyHeader");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.RecieveMoneyDetails)
                    .HasForeignKey(d => d.FkMoeinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecieveMoney_Detail_Moein");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.RecieveMoneyDetails)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecieveMoney_Detail_Preferential");
            });

            modelBuilder.Entity<RecieveMoneyHeader>(entity =>
            {
                entity.ToTable("RecieveMoneyHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkAcDocument).HasColumnName("fkAcDocument");

                entity.Property(e => e.FkMoeinId).HasColumnName("fk_MoeinId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkPreferentialId).HasColumnName("fk_PreferentialId");

                entity.HasOne(d => d.FkAcDocumentNavigation)
                    .WithMany(p => p.RecieveMoneyHeaders)
                    .HasForeignKey(d => d.FkAcDocument)
                    .HasConstraintName("FK_RecieveMoneyHeader_AcDocument");

                entity.HasOne(d => d.FkMoein)
                    .WithMany(p => p.RecieveMoneyHeaders)
                    .HasForeignKey(d => d.FkMoeinId)
                    .HasConstraintName("FK_RecieveMoneyHeader_Moein");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.RecieveMoneyHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_RecieveMoneyHeader_Period");

                entity.HasOne(d => d.FkPreferential)
                    .WithMany(p => p.RecieveMoneyHeaders)
                    .HasForeignKey(d => d.FkPreferentialId)
                    .HasConstraintName("FK_RecieveMoneyHeader_Preferential");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<RibbonItem>(entity =>
            {
                entity.ToTable("RibbonItem");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storage");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkGroupId).HasColumnName("fk_GroupId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.HasOne(d => d.FkGroup)
                    .WithMany(p => p.Storages)
                    .HasForeignKey(d => d.FkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Storage_GroupStorage");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.Storages)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_Storage_Period");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<StorageReceiptDetail>(entity =>
            {
                entity.ToTable("StorageReceipt_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.StorageReceiptDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageReceipt_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.StorageReceiptDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageReceipt_Detail_StorageReceiptHeader");
            });

            modelBuilder.Entity<StorageReceiptHeader>(entity =>
            {
                entity.ToTable("StorageReceiptHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkCodingReceiptTypesId).HasColumnName("fk_CodingReceiptTypesId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkStorageId).HasColumnName("fk_StorageId");

                entity.HasOne(d => d.FkCodingReceiptTypes)
                    .WithMany(p => p.StorageReceiptHeaders)
                    .HasForeignKey(d => d.FkCodingReceiptTypesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageReceiptHeader_CodingReceiptTypes");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.StorageReceiptHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_StorageReceiptHeader_Period");

                entity.HasOne(d => d.FkStorage)
                    .WithMany(p => p.StorageReceiptHeaders)
                    .HasForeignKey(d => d.FkStorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageReceiptHeader_Storage");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<StorageRotationDetail>(entity =>
            {
                entity.ToTable("StorageRotation_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.StorageRotationDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageRotation_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.StorageRotationDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageRotation_Detail_StorageRotationHeader");
            });

            modelBuilder.Entity<StorageRotationHeader>(entity =>
            {
                entity.ToTable("StorageRotationHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.StorageRotationHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_StorageRotationHeader_Period");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<StorageTransferDetail>(entity =>
            {
                entity.ToTable("StorageTransfer_Detail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkCommodityId).HasColumnName("fk_CommodityId");

                entity.Property(e => e.FkHeaderId).HasColumnName("fk_HeaderId");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.FkCommodity)
                    .WithMany(p => p.StorageTransferDetails)
                    .HasForeignKey(d => d.FkCommodityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageTransfer_Detail_Commodity");

                entity.HasOne(d => d.FkHeader)
                    .WithMany(p => p.StorageTransferDetails)
                    .HasForeignKey(d => d.FkHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageTransfer_Detail_StorageTransferHeader");
            });

            modelBuilder.Entity<StorageTransferHeader>(entity =>
            {
                entity.ToTable("StorageTransferHeader");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkCodingTypesTransferId).HasColumnName("fk_CodingTypesTransferId");

                entity.Property(e => e.FkPeriodId).HasColumnName("fkPeriodId");

                entity.Property(e => e.FkStorageId).HasColumnName("fk_StorageId");

                entity.HasOne(d => d.FkCodingTypesTransfer)
                    .WithMany(p => p.StorageTransferHeaders)
                    .HasForeignKey(d => d.FkCodingTypesTransferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageTransferHeader_CodingTypesTransfer");

                entity.HasOne(d => d.FkPeriod)
                    .WithMany(p => p.StorageTransferHeaders)
                    .HasForeignKey(d => d.FkPeriodId)
                    .HasConstraintName("FK_StorageTransferHeader_Period");

                entity.HasOne(d => d.FkStorage)
                    .WithMany(p => p.StorageTransferHeaders)
                    .HasForeignKey(d => d.FkStorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StorageTransferHeader_Storage");
                if (MainWindow.StatusOptions != null)
                    entity.HasQueryFilter(e => !e.FkPeriodId.HasValue || e.FkPeriodId == MainWindow.StatusOptions.Period.Id);
            });

            modelBuilder.Entity<TGroup>(entity =>
            {
                entity.ToTable("tGroup");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserApp>(entity =>
            {
                entity.ToTable("UserApp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FkUserGroupId).HasColumnName("fkUserGroupId");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.UserName).IsRequired();

                entity.HasOne(d => d.FkUserGroup)
                    .WithMany(p => p.UserApps)
                    .HasForeignKey(d => d.FkUserGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserApp_UserGroup");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.ToTable("UserGroup");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Version>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Version");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
