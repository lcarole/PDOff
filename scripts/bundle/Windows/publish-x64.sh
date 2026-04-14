#!/bin/bash
set -e
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../.." && pwd)"
source "$ROOT_DIR/scripts/version.sh"
RID="win-x64"
PUBLISH_DIR="$ROOT_DIR/publish/windows/x64"

echo "Publishing $APP_NAME for Windows x64..."
dotnet publish "$ROOT_DIR/PDOff/PDOff.csproj" -c Release -r "$RID" -o "$PUBLISH_DIR/app"

echo "Creating installer..."
vpk [win] pack \
    --packId "$APP_NAME" \
    --packVersion "$VERSION" \
    --packDir "$PUBLISH_DIR/app" \
    --mainExe "$APP_NAME.exe" \
    --outputDir "$PUBLISH_DIR/installer" \
    --runtime "$RID"

echo "Done: $PUBLISH_DIR/installer"
