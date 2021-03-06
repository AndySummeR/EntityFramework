// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Specification.Tests;
using Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests
{
    public class DataAnnotationSqlServerFixture : DataAnnotationFixtureBase<SqlServerTestStore>
    {
        public static readonly string DatabaseName = "DataAnnotations";

        private readonly string _connectionString = SqlServerTestStore.CreateConnectionString(DatabaseName);
        private readonly DbContextOptions _options;

        public DataAnnotationSqlServerFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .AddSingleton(TestModelSource.GetFactory(OnModelCreating))
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory())
                .BuildServiceProvider();

            _options = new DbContextOptionsBuilder()
                .EnableSensitiveDataLogging()
                .UseInternalServiceProvider(serviceProvider)
                .ConfigureWarnings(w =>
                {
                    w.Default(WarningBehavior.Throw);
                    w.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning);
                }).Options;
        }

        public override SqlServerTestStore CreateTestStore()
            => SqlServerTestStore.GetOrCreateShared(DatabaseName, () =>
                {
                    var options = new DbContextOptionsBuilder(_options)
                        .UseSqlServer(_connectionString, b => b.ApplyConfiguration())
                        .Options;

                    using (var context = new DataAnnotationContext(options))
                    {
                        context.Database.EnsureCreated();
                        DataAnnotationModelInitializer.Seed(context);

                        TestSqlLoggerFactory.Reset();
                    }
                });

        public override DataAnnotationContext CreateContext(SqlServerTestStore testStore)
        {
            var options = new DbContextOptionsBuilder(_options)
                .UseSqlServer(testStore.Connection, b => b.ApplyConfiguration())
                .Options;

            var context = new DataAnnotationContext(options);
            context.Database.UseTransaction(testStore.Transaction);
            return context;
        }
    }
}
