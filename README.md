# Introduction 

A messaging middleware using HTTP as transport and redis as storage. Build with ASP.Net Core

[![Build status](https://ci.appveyor.com/api/projects/status/n9588oe8iag3v509/branch/master?svg=true
)](https://ci.appveyor.com/project/aguacongas/aguacongas-redisqueue)
[![Latest Code coveragre](https://aguacongas.github.io/Aguacongas.RedisQueue/latest/badge_linecoverage.svg)](https://aguacongas.github.io/Aguacongas.RedisQueue/latest)


|Template|Core lib|Web lib|
|:------:|:------:|:------:|
|[![][Aguacongas.RedisQueue-badge]][Aguacongas.RedisQueue-nuget]|[![][Aguacongas.RedisQueue.Core-badge]][Aguacongas.RedisQueue.Core-nuget]|[![][Aguacongas.RedisQueue.Web-badge]][Aguacongas.RedisQueue.Web-nuget]|
|[![][Aguacongas.RedisQueue-downloadbadge]][Aguacongas.RedisQueue-nuget]|[![][Aguacongas.RedisQueue.Core-downloadbadge]][Aguacongas.RedisQueue.Core-nuget]|[![][Aguacongas.RedisQueue.Web-downloadbadge]][Aguacongas.RedisQueue.Web-nuget]|

[Aguacongas.RedisQueue-badge]: https://img.shields.io/nuget/v/Aguacongas.RedisQueue.svg
[Aguacongas.RedisQueue-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.RedisQueue.svg
[Aguacongas.RedisQueue-nuget]: https://www.nuget.org/packages/Aguacongas.RedisQueue/

[Aguacongas.RedisQueue.Core-badge]: https://img.shields.io/nuget/v/Aguacongas.RedisQueue.Core.svg
[Aguacongas.RedisQueue.Core-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.RedisQueue.Core.svg
[Aguacongas.RedisQueue.Core-nuget]: https://www.nuget.org/packages/Aguacongas.RedisQueue.Core/

[Aguacongas.RedisQueue.Web-badge]: https://img.shields.io/nuget/v/Aguacongas.RedisQueue.Web.svg
[Aguacongas.RedisQueue.Web-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.RedisQueue.Web.svg
[Aguacongas.RedisQueue.Web-nuget]: https://www.nuget.org/packages/Aguacongas.RedisQueue.Web/

## Install

install the template `Aguacongas.RedisQueue` with

``` batch

dotnet new -i Aguacongas.RedisQueue

```

And create a project with in a folder of your choice:

``` batch

dotnet new redisqueue

```

Then configure it with authorization that meet your requirement.

## Build

To build the project from source code run `build.cmd` or `build.ps1`