# Setup

## Configuration

In order to run the E2E tests for this framework, you need to have an Azure Cosmos DB emulator or cloud resource active.

## Environment Variables

Grab the connection string of your database and put it into a `.runsettings` file in this directory. The file should contain this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <EnvironmentVariables>
            <DbConnectionString>CONNECTION_STRING_GOES_HERE</DbConnectionString>
        </EnvironmentVariables>
    </RunConfiguration>
</RunSettings>
```

Replace `CONNECTION_STRING_GOES_HERE` with your connection string and it will then use your database to run the tests that use the database..
