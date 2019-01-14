using Bit.Core.Contracts;
using Bit.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDoLine.Data
{
    public class RunMigrationsAppEvent : IAppEvents
    {
        public virtual AppEnvironment AppEnvironment { get; set; }

        public virtual void OnAppStartup()
        {
            if (AppEnvironment.DebugMode == true)
            {
                using (ToDoLineDbContext dbContext = new ToDoLineDbContext())
                {
                    dbContext.Database.Migrate();
                }
            }
        }

        public virtual void OnAppEnd()
        {

        }
    }
}
