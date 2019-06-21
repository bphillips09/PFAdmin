FROM ubuntu:latest

COPY build/                  /root/build/
COPY entrypoint.sh           /entrypoint.sh

RUN apt-get -y update
RUN apt-get -y install curl wget

WORKDIR /root/
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]