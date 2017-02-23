using ReadifyBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadifyBank.Services
{
    /// <summary>
    /// Readify Bank InstanceFactory Class
    /// Generate instance of implementation of Interface. 
    /// Reduce coupling. 
    /// </summary>
    public class InstanceFactory
    {
        public Object generateInstance(Type type)
        {
            if (typeof(IAccount).Equals(type))
                return new AccountImpl();
            if (typeof(IStatementRow).Equals(type))
                return new StatementRowImpl();
            else
                return null; 
        }
    }
}
