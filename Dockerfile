ARG GAME_CI_UNITY_EDITOR_IMAGE

FROM $GAME_CI_UNITY_EDITOR_IMAGE
#FROM unityci/editor:ubuntu-2020.3.48f1-linux-il2cpp-3

RUN apt-get update && \
    apt-get install -y blender
