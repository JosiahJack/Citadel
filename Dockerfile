ARG GAME_CI_UNITY_EDITOR_IMAGE

FROM $GAME_CI_UNITY_EDITOR_IMAGE
#FROM unityci/editor:ubuntu-2019.4.35f1-linux-il2cpp-1

RUN apt-get update && \
    apt-get install -y blender
    apt-get install libvulkan-dev
    
