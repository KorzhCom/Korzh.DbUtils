
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory.Tests
{
    internal static class DbContextExtensions
    {

        public static IQueryable<Object> Set(this DbContext _context, Type t)
        {
            var contextType = _context.GetType();
            var setMethod = contextType.GetMethod("Set",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                    );
            return (IQueryable<Object>)setMethod.MakeGenericMethod(t).Invoke(_context, null);
        }

    }
}
