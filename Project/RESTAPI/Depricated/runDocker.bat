@ECHO off
echo building
docker build . -t liamwoolleyrestapi
echo running
docker run  -it --name liamwoolleyrestapi -p 80:8080 -p 443:43443 liamwoolleyrestapi 
echo removing container
docker rm /liamwoolleyrestapi
echo removing image
docker rmi liamwoolleyrestapi:latest
pause