FROM node:12-alpine

RUN mkdir -p /home/node/index/node_modules && mkdir -p /home/node/index/crudOps && mkdir -p /home/node/index/settings && mkdir -p /home/node/index/storage && chown -R node:node /home/node/index

WORKDIR /home/node/index

COPY package*.json ./
COPY crudOps/ ./crudOps/
COPY storage/ ./storage/


USER node

RUN ls
RUN npm install

COPY --chown=node:node . .

EXPOSE 8080
EXPOSE 43443
Run mv Docker/Settings.json settings/Settings.json
CMD [ "node", "."]
