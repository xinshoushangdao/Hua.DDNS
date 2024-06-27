Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
cd ./Hua.DDNS
docker build -t hua.ddns:latest -f Dockerfile ..