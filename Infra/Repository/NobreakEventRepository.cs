using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface;
using NobreakTSSharaDDDWeb.Domain.Interface.Repository;
using NobreakTSSharaDDDWeb.Infra.Data;

namespace NobreakTSSharaDDDWeb.Infra.Repository
{
    public class NobreakEventRepository : EfDbRepository<NobreakEvent>, INobreakEventRepository
    {
        public NobreakEventRepository(NobreakContext dbContext) : base(dbContext)
        {

        }

    }
}
