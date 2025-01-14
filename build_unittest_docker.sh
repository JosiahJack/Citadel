#!/usr/bin/env bash

set -ex

UNITY_VERSION=2019.4.35f1
GAME_CI_VERSION=1 # https://github.com/game-ci/docker/releases
MY_USERNAME=josiahjack

# Just Linux image for unit test runner
declare -a components=("linux-il2cpp")

for component in "${components[@]}"
do
  GAME_CI_UNITY_EDITOR_IMAGE=unityci/editor:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  IMAGE_TO_PUBLISH=${MY_USERNAME}/citadel-ci:ubuntu-${UNITY_VERSION}-${component}-${GAME_CI_VERSION}
  docker build --build-arg GAME_CI_UNITY_EDITOR_IMAGE=${GAME_CI_UNITY_EDITOR_IMAGE} . -t ${IMAGE_TO_PUBLISH}
# uncomment the following to publish the built images to docker 
# Do this whenever Unity version or host docker hub account changes
 # docker push ${IMAGE_TO_PUBLISH}
done
