#!/bin/bash
set -e
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../.." && pwd)"
source "$ROOT_DIR/scripts/version.sh"
RID="linux-arm64"
PUBLISH_DIR="$ROOT_DIR/publish/linux/arm64"

echo "Publishing $APP_NAME for Linux arm64..."
dotnet publish "$ROOT_DIR/PDOff/PDOff.csproj" -c Release -r "$RID" -o "$PUBLISH_DIR/app"

echo "Creating installer..."
vpk [linux] pack \
    --packId "$APP_NAME" \
    --packVersion "$VERSION" \
    --packDir "$PUBLISH_DIR/app" \
    --mainExe "$APP_NAME" \
    --outputDir "$PUBLISH_DIR/installer" \
    --runtime "$RID"

echo "Done: $PUBLISH_DIR/installer"
