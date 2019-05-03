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

        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<UserList> UserList { get; set; }
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

            modelBuilder.Entity<Item>(entity =>
            {
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
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartOfList");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.Property(e => e.ListName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserList>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ListId })
                    .HasName("PK__UserList__59B0FECC60A5ECD7");

                entity.HasOne(d => d.List)
                    .WithMany(p => p.UserList)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ListOwned");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserList)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserOwner");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Username)
                    .HasName("UQ__Users__F3DBC5729EE90806")
                    .IsUnique();

                entity.Property(e => e.Pswd)
                    .IsRequired()
                    .HasColumnName("pswd")
                    .HasColumnType("text");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text");

                entity.Property(e => e.UserRole)
                    .HasColumnName("userRole")
                    .HasColumnType("text")
                    .HasDefaultValueSql("('User')");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });
        }
    }
}
