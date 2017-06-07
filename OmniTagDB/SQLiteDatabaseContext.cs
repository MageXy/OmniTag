using System;
using System.Data.Entity.Migrations.Model;
using System.IO;
using OmniTag.Models;
using SQLite.CodeFirst;

namespace OmniTagDB
{
    using System.Data.Entity;
    using System.Data.SQLite;

    public class SQLiteDatabaseContext : OmniTagContext
    {
        private const int DatabaseVersion = 1;

        public SQLiteDatabaseContext()
            : base("name=SQLiteDatabaseContext")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<SQLiteDatabaseContext>(modelBuilder));
        }
    }

    public class SqliteDropCreateDatabaseWithDefaultDataIfModelChanges : SqliteDropCreateDatabaseWhenModelChanges<SQLiteDatabaseContext>
    {
        public SqliteDropCreateDatabaseWithDefaultDataIfModelChanges(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        public SqliteDropCreateDatabaseWithDefaultDataIfModelChanges(DbModelBuilder modelBuilder, Type historyEntityType) : base(modelBuilder, historyEntityType)
        {
        }

        protected override void Seed(SQLiteDatabaseContext context)
        {
            base.Seed(context);

            context.Settings.Add(new Setting
            {
                Name = Setting.AutoTagVerificationThreshold,
                Value = "5",
                DateCreated = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });
        }
    }
}
