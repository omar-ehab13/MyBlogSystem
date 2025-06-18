using BlogSystem.Infrastructure.Logging.Enrichers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace BlogSystem.Infrastructure.Logging;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration configuration, IHostEnvironment environment)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails();

        // Add custom enrichers
        //loggerConfig.Enrich.With<UserIdEnricher>();
        //loggerConfig.Enrich.With<RequestPathEnricher>();

        // Environment-specific configuration
        if (environment.IsDevelopment())
        {
            ConfigureDevelopmentLogging(loggerConfig, configuration);
        }
        else if (environment.IsProduction())
        {
            ConfigureProductionLogging(loggerConfig, configuration);
        }

        // Configure Seq if available
        //var seqConfig = configuration.GetSection("SeqConfiguration");
        //if (!string.IsNullOrEmpty(seqConfig["ServerUrl"]))
        //{
        //    loggerConfig.WriteTo.Seq(
        //        serverUrl: seqConfig["ServerUrl"],
        //        apiKey: seqConfig["ApiKey"],
        //        restrictedToMinimumLevel: LogEventLevel.Information);
        //}

        // Configure Database logging
        //var dbConfig = configuration.GetSection("DatabaseLogging");
        //if (!string.IsNullOrEmpty(dbConfig["ConnectionString"]))
        //{
        //    ConfigureDatabaseLogging(loggerConfig, dbConfig);
        //}

        Log.Logger = loggerConfig.CreateLogger();
    }

    private static void ConfigureDevelopmentLogging(LoggerConfiguration config, IConfiguration configuration)
    {
        config
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj} {NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/taskflow-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {CorrelationId} {Message:lj} {NewLine}{Exception}");
    }

    private static void ConfigureProductionLogging(LoggerConfiguration config, IConfiguration configuration)
    {
        config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(formatter: new CompactJsonFormatter())
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: "/var/log/taskflow/taskflow-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                fileSizeLimitBytes: 50 * 1024 * 1024) // 50MB
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: "/var/log/taskflow/errors/taskflow-errors-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90,
                restrictedToMinimumLevel: LogEventLevel.Error);
    }

    private static void ConfigureDatabaseLogging(LoggerConfiguration config, IConfigurationSection dbConfig)
    {
        var columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Add(StandardColumn.LogEvent);

        // Add custom columns
        columnOptions.AdditionalColumns = new Collection<SqlColumn>
        {
            new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 256 },
            new SqlColumn { ColumnName = "CorrelationId", DataType = SqlDbType.NVarChar, DataLength = 256 },
            new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 2048 },
            new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = 2048 },
            new SqlColumn { ColumnName = "ClientIP", DataType = SqlDbType.NVarChar, DataLength = 45 }
        };

        config.WriteTo.MSSqlServer(
            connectionString: dbConfig["ConnectionString"],
            tableName: dbConfig["TableName"],
            columnOptions: columnOptions,
            autoCreateSqlTable: dbConfig.GetValue<bool>("AutoCreateSqlTable"),
            restrictedToMinimumLevel: LogEventLevel.Warning);
    }
}
