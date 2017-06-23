using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using OmniTag.Models;

namespace OmniTagDB
{
    public abstract class OmniTagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Omni> Omnis { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected OmniTagContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            // empty
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public bool IsPortable
        {
            get
            {
                #if PORTABLE
                return true;
                #else
                return false;
                #endif
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Image

            modelBuilder.Entity<Image>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Image>()
                .Property(i => i.FileName)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .Property(i => i.ImageData)
                .IsRequired();

            #endregion

            #region Omni

            modelBuilder.Entity<Omni>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Omni>()
                .Property(o => o.Summary)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));

            modelBuilder.Entity<Omni>()
                .HasMany(o => o.Images)
                .WithRequired(i => i.Omni);

            #endregion

            #region Tag

            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute() { IsUnique = true }));

            modelBuilder.Entity<Tag>()
                .Property(t => t.IsVerified)
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .Property(t => t.ManuallyVerified)
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Omnis)
                .WithMany(o => o.Tags)
                .Map(m =>
                {
                    m.MapLeftKey("Tag_Id");
                    m.MapRightKey("Omni_Id");
                    m.ToTable("OmniTags");
                });

            #endregion

            #region Setting

            modelBuilder.Entity<Setting>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Setting>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Setting>()
                .Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
            
            #endregion
        }

        public void CancelChanges()
        {
            var changedEntities = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();
            foreach (var entry in changedEntities.Where(x => x.State == EntityState.Modified))
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }
            foreach (var entry in changedEntities.Where(x => x.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }
            foreach (var entry in changedEntities.Where(x => x.State == EntityState.Deleted))
            {
                entry.State = EntityState.Unchanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            var entities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                              e.State == EntityState.Deleted ||
                                                              e.State == EntityState.Modified ||
                                                              e.State == EntityState.Unchanged);

            foreach (var entry in entities)
            {
                entry.State = EntityState.Detached;
            }

            base.Dispose(disposing);
        }
    }
}
