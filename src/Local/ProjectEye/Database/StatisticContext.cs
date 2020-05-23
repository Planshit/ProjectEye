using ProjectEye.Core.Models.Options;
using ProjectEye.Core.Models.Statistic;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Database
{
    public class StatisticContext : DbContext
    {
        /// <summary>
        /// 统计数据
        /// </summary>
        public DbSet<StatisticModel> Statistics { get; set; }
        /// <summary>
        /// 番茄数据
        /// </summary>
        public DbSet<TomatoModel> Tomatos { get; set; }
        // public StatisticContext(string n)
        //: base("StatisticContext")
        // {
        //     DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        // }
        public StatisticContext()
       : base(new SQLiteConnection()
       {
           ConnectionString = "Data Source=.\\Data\\data.db"
       }, true)
        {
            DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var model = modelBuilder.Build(Database.Connection);
            new SQLiteBuilder(model).Handle();
        }
    }
}