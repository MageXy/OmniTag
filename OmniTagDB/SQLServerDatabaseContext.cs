using System;
using OmniTag.Models;

namespace OmniTagDB
{
    using System.Data.Entity;

    public class SQLServerDatabaseContext : OmniTagContext
    {
        public SQLServerDatabaseContext()
            : base("name=SQLServerDatabaseContext")
        {
            Database.SetInitializer<SQLServerDatabaseContext>(new CreateDatabaseIfNotExists<SQLServerDatabaseContext>());
        }
    }

    public class DropCreateDatabaseWithDefaultDataIfModelChanges : DropCreateDatabaseIfModelChanges<SQLServerDatabaseContext>
    {
        protected override void Seed(SQLServerDatabaseContext context)
        {
            base.Seed(context);

            context.Settings.Add(new Setting
            {
                Name = Setting.AutoTagVerificationThreshold,
                Value = "5",
                DateCreated = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });

            context.Settings.Add(new Setting
            {
                Name = Setting.ShowTagSearchOnStartup,
                Value = "False",
                DateCreated = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });

            context.SaveChanges();
        }
    }
}
