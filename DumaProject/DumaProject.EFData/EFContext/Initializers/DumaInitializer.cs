﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DumaProject.Core.Entities;

namespace DumaProject.EFData.EFContext.Initializers
{
    public class DumaInitializer : IDatabaseInitializer<DumaContext>
    {
        public void InitializeDatabase(DumaContext context)
        {
            bool databaseExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                databaseExists = context.Database.Exists();
            }
            if (databaseExists)
            {
                if (context.Database.CompatibleWithModel(true))
                {
                    return;
                }
                context.Database.Delete();
            }
            context.Database.Create();
            //context.Database.ExecuteSqlCommand("ALTER TABLE Users ADD CONSTRAINT EmailDataUnique UNIQUE (Email)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Users ADD CONSTRAINT NameDataUnique UNIQUE (UserName)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Roles ADD CONSTRAINT RoleNameDataUnique UNIQUE (Name)");
            try
            {
                context.SaveChanges();
                Seed(context);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw;
            }
        }

        public void Seed(DumaContext context)
        {
            //TODO: First variables.

            var member = new Member
            {
                Name = "Lesha",
                Surname = "Yurchak",
                Patronymic = "Vladimirovich",
                PassportData = "MP"
            };

            context.Members.Add(member);
            context.SaveChanges();

            var commission = new Commission {Name = "Hello", President = member};
            context.Commissions.Add(commission);

            var roleCom = new RoleCommission {Value = "Commandor"};
            context.RoleCommissions.Add(roleCom);

            context.SaveChanges();
            commission.Consist.Add(member);

            var record = new JournalRecord
            {
                EntryDate = DateTime.Now,
                ExitDate = DateTime.Now.AddDays(3),
                Member = member,
                Commission = commission,
                Role = roleCom
            };

            commission.JournalRecords.Add(record);

            context.SaveChanges();
        }
    }
}
