#!/bin/sh
for project in tests/**/*.csproj; do
    [ -f "$project" ] || break
    dotnet build -c Release $project || exit
    dotnet test -c Release -l:trx -r /src/tests/results/ $project || true
done