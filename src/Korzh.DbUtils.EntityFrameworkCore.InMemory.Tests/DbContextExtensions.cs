
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory.Tests
{
    internal static class DbContextExtensions
    {

        public static IQueryable<Object> Set(this DbContext _context, Type t)
        {
            return (IQueryable<Object>)_context.GetType().GetMethod("Set").MakeGenericMethod(t).Invoke(_context, null);
        }

    }
}
