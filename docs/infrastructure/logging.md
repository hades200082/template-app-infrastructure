# Infrastructure.Logging

Logging is automatically configured locally to log the "Verbose" logging level 
to "Console" and "Debug" outputs.

## Efficient logging

All logs generated in code within this solution must use a custom Logger Message Definition (LMD). 
Search for classes named `LoggerMessageDefinitions` to see examples. 

Logging in this way is more memory efficient as it prevents the logger from needing to allocate
arrays for log message variables.

It also ensures that we are being explicit about what we log.

### Log levels usage

When creating custom LMDs the following rules should be used when determining what
level the log event should be. Note that these differ from Serilog's log levels.
Serilog will map them internally 

- `Trace` - Logs that contain the most detailed messages. These messages may contain sensitive 
    application data. These messages are disabled by default and should never be enabled in a 
    production environment. Serilog calls this log level `Verbose`.
- `Debug` - Logs that are used for interactive investigation during development.  These logs 
    should primarily contain information useful for debugging and have no long-term value.
- `Information` - Logs that track the general flow of the application. These logs should 
    have long-term value.
- `Warning` - Logs that highlight an abnormal or unexpected event in the application flow, 
    but do not otherwise cause the application execution to stop.
- `Error` - Logs that highlight when the current flow of execution is stopped due to a 
    failure. These should indicate a failure in the current activity, not an 
    application-wide failure. 
- `Critical` - Logs that describe an unrecoverable application or system crash, or a catastrophic
    failure that requires immediate attention. Serilog calls this log level `Fatal`

## Sentry.io

To log events to [Sentry.io](https://sentry.io), set an appSetting or environment varialbe 
called `SentryDsn`:

```json
{
  "Sentry": {
    "Dsn": "Your DSN from Sentry",
    "MinimumLogLevel": "Warning",
    "MinimumBreadcrumbLevel": "Information"
  }
}
```

For the "MinimumLogLevel" the options are:

- `Verbose` - Anything and everything you might want to know about a running block of code.
- `Debug` - Internal system events that aren't necessarily observable from the outside.
- `Information` - The lifeblood of operational intelligence - things happen.
- `Warning` - Service is degraded or endangered.
- `Error` - Functionality is unavailable, invariants are broken or data is lost.
- `Fatal` - If you have a pager, it goes off when one of these occurs.

For general production usage, `Warning` is recommended unless another level is needed temporarily to
gather additional information about a fault.

For Dev and UAT usage, `Information` should be sufficient unless another level is needed temporarily to
gather additional information about a fault.

### Breadcrumbs

Serilog will only send log events to Sentry that are at least the "MinimumLogLevel" in the 
Sentry configuration.

However, lower level events may be recorded as "breadcrumbs" against higher level events that 
are sent to Sentry.

These breadcrumbs can provide additional context to errors and warnings. By default, Sentry will
send `Information` events and above as breadcrumbs 

To change this, specify the `MinimumBreadcrumbLevel` property in your Sentry appSettings. 
It must be a more verbose level than `MinimumLogLevel`.

### Releases

If you want to enable tracking of different releases in Sentry you will need to configure 
your CI/CD pipeline to write the version/release/build number/ID to a file in the 
`HostingEnvironment.ContentRootPath` location when deploying your application.

The filename for the file must be `ver.txt` and it must only contain the release ID on a single line.

If this file exists, its contents will be used as the "Release" when configuring Sentry during startup.

> ***Note:** It is up to you to ensure that your release IDs are unique.*

### Trace sampling / Performance monitoring

To enable Sentry trace sampling, set the following in your Sentry appsettings.

```json
{
  "Sentry": {
    "EnableTracing": true,
    "TracesSampleRate": 1.0
  }
}
```

- `EnableTracing` - Is trace sampling enabled (default: false)
- `TracesSampleRate` - What percentage of requests do you want to sample? 0..1 (default 1.0 \[100%])

> ***Note:** Trace sampling uses "transactions" in Sentry. Your quota for transactions is different 
> *to your quota for "events" (logs)*

