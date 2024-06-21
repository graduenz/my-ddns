#!/usr/bin/env bash

set -eu -o pipefail

REPORTS_FOLDER_PATH=test-reports

dotnet test \
    --logger trx \
    --logger "console;verbosity=detailed" \
    --settings "runsettings.xml" \
    --results-directory $REPORTS_FOLDER_PATH