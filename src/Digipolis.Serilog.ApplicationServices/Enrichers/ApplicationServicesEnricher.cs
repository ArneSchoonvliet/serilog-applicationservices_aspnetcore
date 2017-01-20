﻿using System;
using Digipolis.ApplicationServices;
using Digipolis.Serilog.ApplicationEnrichment;
using Serilog.Core;
using Serilog.Events;

namespace Digipolis.Serilog
{
    public class ApplicationServicesEnricher : ILogEventEnricher
    {
        public ApplicationServicesEnricher(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        private readonly IApplicationContext _applicationContext;

        private LogEventProperty _applicationId;
        private LogEventProperty _applicationName;
        private LogEventProperty _applicationInstanceId;
        private LogEventProperty _applicationInstanceName;
        private LogEventProperty _applicationVersion;
        private LogEventProperty _machineName;


        private const string COMPONENTKEY = "SourceContext";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if ( _applicationId == null ) InitProperties(propertyFactory);

            logEvent.AddPropertyIfAbsent(_applicationId);
            logEvent.AddPropertyIfAbsent(_applicationName);
            logEvent.AddPropertyIfAbsent(_applicationInstanceId);
            logEvent.AddPropertyIfAbsent(_applicationInstanceName);
            logEvent.AddPropertyIfAbsent(_applicationVersion);
            logEvent.AddPropertyIfAbsent(_machineName);

            if ( logEvent.Properties.ContainsKey(COMPONENTKEY) )
            {
                var sourceContext = logEvent.Properties[COMPONENTKEY];
                var idProp = new LogEventProperty(ApplicationLoggingProperties.ApplicationComponentId, sourceContext);
                var nameProp = new LogEventProperty(ApplicationLoggingProperties.ApplicationComponentName, sourceContext);
                logEvent.AddOrUpdateProperty(idProp);
                logEvent.AddOrUpdateProperty(nameProp);
            }

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(ApplicationLoggingProperties.ProcessId, System.Diagnostics.Process.GetCurrentProcess().Id));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(ApplicationLoggingProperties.ThreadId, new ScalarValue(System.Environment.CurrentManagedThreadId)));
        }

        private void InitProperties(ILogEventPropertyFactory propertyFactory)
        {
            _applicationId = propertyFactory.CreateProperty(ApplicationLoggingProperties.ApplicationId, _applicationContext.ApplicationId ?? ApplicationLoggingProperties.NullValue);
            _applicationName = propertyFactory.CreateProperty(ApplicationLoggingProperties.ApplicationName, _applicationContext.ApplicationName ?? ApplicationLoggingProperties.NullValue);
            _applicationInstanceId = propertyFactory.CreateProperty(ApplicationLoggingProperties.ApplicationInstanceId, _applicationContext.InstanceId ?? ApplicationLoggingProperties.NullValue);
            _applicationInstanceName = propertyFactory.CreateProperty(ApplicationLoggingProperties.ApplicationInstanceName, _applicationContext.InstanceName ?? ApplicationLoggingProperties.NullValue);
            _applicationVersion = propertyFactory.CreateProperty(ApplicationLoggingProperties.ApplicationVersion, _applicationContext.ApplicationVersion ?? ApplicationLoggingProperties.NullValue);
            _machineName = propertyFactory.CreateProperty(ApplicationLoggingProperties.MachineName, System.Environment.MachineName ?? ApplicationLoggingProperties.NullValue);
        }
    }
}
