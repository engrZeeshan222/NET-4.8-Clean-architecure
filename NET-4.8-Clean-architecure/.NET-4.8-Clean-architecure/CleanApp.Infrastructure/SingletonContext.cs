using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure
{
    public sealed class SingletonContext
    {
        private static readonly ApplicationDbContext _instance = new ApplicationDbContext();

        public static ApplicationDbContext Instance
        {
            get
            {
                return _instance;
            }
        }

        private SingletonContext() { } // Prevent direct instantiation
    }
}
