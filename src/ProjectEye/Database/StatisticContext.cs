using ProjectEye.Core.Models.Statistic;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var model = modelBuilder.Build(Database.Connection);
            IDatabaseCreator sqliteDatabaseCreator = new SqliteDatabaseCreator();
            sqliteDatabaseCreator.Create(Database, model);
        }

        //string connectionString = @"data source = {PathToSqliteDB}";
        //SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //public StatisticContext() : base(new SQLiteConnection)
        //{

        //}
    }
}