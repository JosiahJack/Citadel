#!/usr/bin/env bash

set -ex

UNITY_VERSION=2019.4.35f1
GAME_CI_VERSION=1 # https://github.com/game-ci/docker/releases
MY_USERNAME=josiahjack

declare -a components=("linux-il2cpp" "mac-mono" "windows-mono")

docker login -u="$1" -p="$2"
for component in "${components[@]}"
do
  GAME_CI_UNITY_EDITOR_IMAGE=unityci/editor:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  IMAGE_TO_PUBLISH=${MY_USERNAME}/citadel-ci:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  docker build --build-arg GAME_CI_UNITY_EDITOR_IMAGE=${GAME_CI_UNITY_EDITOR_IMAGE} . -t ${IMAGE_TO_PUBLISH}
# uncomment the following to publish the built images to docker hub
  docker push ${IMAGE_TO_PUBLISH}
done
