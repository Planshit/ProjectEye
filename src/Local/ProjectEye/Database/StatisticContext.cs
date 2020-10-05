using ProjectEye.Core.Models.Statistic;
using System.Data.Entity;
using System.Data.SQLite;

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
        public DbSet<Core.Models.Statistic.TomatoModel> Tomatos { get; set; }
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