# Introduction 

A messaging middleware using HTTP as transport and redis as storage. Build with ASP.Net Core

[![Build status](https://ci.appveyor.com/api/projects/status/n9588oe8iag3v509/branch/master?svg=true
)](https://ci.appveyor.com/project/aguacongas/aguacongas-redisqueue)
[![Latest Code coveragre](https://aguacongas.github.io/Aguacongas.RedisQueue/latest/badge_linecoverage.svg)](https://aguacongas.github.io/Aguacongas.RedisQueue/latest)

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