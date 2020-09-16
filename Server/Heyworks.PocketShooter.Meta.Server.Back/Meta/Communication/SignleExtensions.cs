using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Heyworks.PocketShooter.Meta.Services
{
    public static class SignleExtensions
    {
        public static T SingleOrError<T>(this IEnumerable<T> source, Func<T, bool> predicate, string errorMessage) 
        {
            try
            {
                return source.Single(predicate);
            }
            catch (InvalidOperationException notFound)
            {
                throw new InvalidOperationException(errorMessage, notFound);
            }            
        }
    }
}
