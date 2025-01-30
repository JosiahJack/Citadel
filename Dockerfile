ARG GAME_CI_UNITY_EDITOR_IMAGE=unityci/editor:ubuntu-2020.3.48f1-linux-il2cpp-3

FROM $GAME_CI_UNITY_EDITOR_IMAGE

RUN apt-get update && \
    apt-get install -y blender
