#!/bin/bash
dotnet new sln -n BookApp -o BookApp.Solution
dotnet new classlib -n BookApp.Domain -o BookApp.Solution/BookApp.Domain
dotnet new classlib -n BookApp.Application -o BookApp.Solution/BookApp.Application
dotnet new classlib -n BookApp.Infrastructure -o BookApp.Solution/BookApp.Infrastructure
dotnet new webapi -n BookApp.Api -o BookApp.Solution/BookApp.Api

dotnet sln BookApp.Solution/BookApp.sln add BookApp.Solution/BookApp.Domain/BookApp.Domain.csproj
dotnet sln BookApp.Solution/BookApp.sln add BookApp.Solution/BookApp.Application/BookApp.Application.csproj
dotnet sln BookApp.Solution/BookApp.sln add BookApp.Solution/BookApp.Infrastructure/BookApp.Infrastructure.csproj
dotnet sln BookApp.Solution/BookApp.sln add BookApp.Solution/BookApp.Api/BookApp.Api.csproj

dotnet add BookApp.Solution/BookApp.Api/BookApp.Api.csproj reference BookApp.Solution/BookApp.Application/BookApp.Application.csproj BookApp.Solution/BookApp.Infrastructure/BookApp.Infrastructure.csproj
dotnet add BookApp.Solution/BookApp.Infrastructure/BookApp.Infrastructure.csproj reference BookApp.Solution/BookApp.Application/BookApp.Application.csproj BookApp.Solution/BookApp.Domain/BookApp.Domain.csproj
dotnet add BookApp.Solution/BookApp.Application/BookApp.Application.csproj reference BookApp.Solution/BookApp.Domain/BookApp.Domain.csproj

rm BookApp.Solution/BookApp.Domain/Class1.cs
rm BookApp.Solution/BookApp.Application/Class1.cs
rm BookApp.Solution/BookApp.Infrastructure/Class1.cs
