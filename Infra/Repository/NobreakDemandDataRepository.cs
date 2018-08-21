using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface.Repository;
using NobreakTSSharaDDDWeb.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NobreakTSSharaDDDWeb.Infra.Repository
{
    public class NobreakDemandDataRepository : EfDbRepository<NobreakDemandData>, INobreakDemandDataRepository
    {
        public NobreakDemandDataRepository(NobreakContext dbContext) : base(dbContext)
        {

        }
    }
}
