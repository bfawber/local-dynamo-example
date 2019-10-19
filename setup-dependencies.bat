ECHO OFF

rem Configures your aws credentials and sets up dependencies for the voice matcher locally.

aws configure
docker run -d 8000:8000 amazon/dynamodb-local