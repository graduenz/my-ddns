#!/usr/bin/env bash

set -eu -o pipefail

if [ -z "$1" ]; then
  echo "Please provide the sonar token 👀"
  exit 0
fi

if [ -z "$2" ]; then
  echo "Please provide the project version 👀"
  exit 0
fi

echo "### Reading variables..."
SONAR_TOKEN=$1
PROJECT_VERSION=$2

echo "### Beginning sonarscanner..."

# You should start the scanner prior building your project and running your tests
.sonar/scanner/dotnet-sonarscanner begin \
    /k:"graduenz_my-ddns" \
    /o:"graduenz" \
    /d:sonar.token="$SONAR_TOKEN" \
    /v:"$PROJECT_VERSION" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.cs.opencover.reportsPaths="**/*/coverage.opencover.xml" \
    /d:sonar.cs.vstest.reportsPaths="**/*/*.trx" \
    /d:sonar.exclusions="samples/**/*.cs"

echo "### Building project..."
dotnet build
./scripts/start-tests.sh

# Now we can collect the results 👍
.sonar/scanner/dotnet-sonarscanner end /d:sonar.token="$SONAR_TOKEN"