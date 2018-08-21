using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NobreakTSSharaDDDWeb.Infra.Repository.Query
{
    public class DBQuery
    {
        private DbContext Context = null;

        public DBQuery()
        {
            Context = new EfDbContext();
        }

        public void CreateNobreakDemandData(NobreakDemandData data)
        {
            Context.Set<NobreakDemandData>().Add(data);
            Context.SaveChanges();
        }

        public List<NobreakDemandData> FindAll()
        {
            return Context.Set<NobreakDemandData>().AsNoTracking().OrderByDescending(x => x.CreationData).ToList();
        }

        public void DeleteNobreakDemandData(List<NobreakDemandData> listToRemove)
        {
            Context.Set<List<NobreakDemandData>>().Attach(listToRemove);
            Context.Set<List<NobreakDemandData>>().Remove(listToRemove);
            Context.SaveChanges();
        }

        public int FindNetworkFailureTime()
        {
            int value = 0;
            try
            {
                value = Context.SettingsWork.AsEnumerable().LastOrDefault().NetworkFailureTime;
            }
            catch (Exception ex)
            {

                value = 120000;
            }
            return value;
        }

        public int FindLowBatteryTime()
        {
            int value = 0;
            try
            {
                value = Context.SettingsWork.AsEnumerable().LastOrDefault().LowBatteryTime;
            }
            catch (Exception ex)
            {

                value = 120000;
            }
            return value;
        }

        public string FindEmailUser()
        {
            string value = null;
            try
            {
                value = Context.SettingsWork.AsEnumerable().LastOrDefault().UserEmail;
            }
            catch (Exception ex)
            {

                value = "";
            }
            return value;
        }

        public string FindSerialNobreak()
        {
            string value = null;
            try
            {
                value = Context.SettingsWork.AsEnumerable().LastOrDefault().Serial;
            }
            catch (Exception ex)
            {

                value = "";
            }
            return value;
        }

        public string FindCurrentLanguage()
        {
            string value = null;
            try
            {
                value = Context.SettingsWork.AsEnumerable().LastOrDefault().CurrentLanguage;
            }
            catch (Exception ex)
            {

                value = "pt-BR";
            }
            return value;
        }
    }
}
