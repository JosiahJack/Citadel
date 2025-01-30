#!/usr/bin/env bash

set -ex

UNITY_VERSION=2020.3.48f1
GAME_CI_VERSION=3 # https://github.com/game-ci/docker/releases
MY_USERNAME=josiahjack

declare -a components=("linux-il2cpp" "mac-mono" "windows-mono")

for component in "${components[@]}"
do
  GAME_CI_UNITY_EDITOR_IMAGE=unityci/editor:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  IMAGE_TO_PUBLISH=${MY_USERNAME}/citadel-ci:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  docker build --build-arg GAME_CI_UNITY_EDITOR_IMAGE=${GAME_CI_UNITY_EDITOR_IMAGE} . -t ${IMAGE_TO_PUBLISH}

  # Do this whenever Unity version or host docker hub account changes
  if [ "$1" == "push_docker" ]; then
    docker push ${IMAGE_TO_PUBLISH}
  fi
done
