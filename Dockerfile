FROM unityci/base

MAINTAINER ***@m***

RUN apt-get update && \
    apt-get install -y blender
