using System;
using System.Linq;
using OmniTag.Models;
using OmniTagDB;

namespace OmniTagWPF.Utility
{
    public static class SettingExtensions
    {
        public static Setting GetSettingOrDefault(this OmniTagContext context, string settingName,
            string settingDefaultValue)
        {
            var setting = context.Settings.SingleOrDefault(s => s.Name == settingName);
            if (setting == null)
            {
                setting = new Setting
                {
                    Name = settingName,
                    Value = settingDefaultValue,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
            }
            return setting;
        }

        public static Setting GetSettingOrDefaultAndSave(this OmniTagContext context, string settingName,
            string settingDefaultValue)
        {
            var setting = context.Settings.SingleOrDefault(s => s.Name == settingName);
            if (setting == null)
            {
                setting = new Setting
                {
                    Name = settingName,
                    Value = settingDefaultValue,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
                context.Settings.Add(setting);
                context.SaveChanges();
            }
            return setting;
        }
    }
}
