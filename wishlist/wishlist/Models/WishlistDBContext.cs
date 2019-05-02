using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace wishlist.Models
{
    public partial class WishlistDBContext : DbContext
    {
        public WishlistDBContext()
        {
        }

        public WishlistDBContext(DbContextOptions<WishlistDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Lists> Lists { get; set; }
        public virtual DbSet<UserLists> UserLists { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WishlistDB;Trusted_Connection=True;");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__Items__727E838BCFB68623");

                entity.Property(e => e.ItemId).ValueGeneratedNever();

                entity.Property(e => e.Bought).HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Quantity).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.List)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartOfList");
            });

            modelBuilder.Entity<Lists>(entity =>
            {
                entity.HasKey(e => e.ListId)
                    .HasName("PK__Lists__E383280501BEB558");

                entity.Property(e => e.ListId).ValueGeneratedNever();

                entity.Property(e => e.ListName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserLists>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ListId })
                    .HasName("PK__UserList__59B0FECCA6391E40");

                entity.HasOne(d => d.List)
                    .WithMany(p => p.UserLists)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ListOwned");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserOwner");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__1788CC4C0A74D3D2");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Pswd)
                    .IsRequired()
                    .HasColumnName("pswd")
                    .HasColumnType("text");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });
        }
    }
}
