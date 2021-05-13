echo 'Build Auth...'
DOCKER_REPO_SLUG=fewbox/auth PROJECTNAME=FewBox.Service.Auth PROJECTUNITTESTNAME=$PROJECTNAME.UnitTest
DOCKER_REPO_VERSION=nas
DOCKER_REPO_IP=192.168.1.38
DOCKER_REPO_PORT=6088
dotnet restore $PROJECTNAME
dotnet publish -c Release $PROJECTNAME/$PROJECTNAME.csproj -p:FileVersion=$TRAVIS_JOB_NUMBER
cp Dockerfile ./$PROJECTNAME/bin/Release/netcoreapp5.0/publish/Dockerfile
cp .dockerignore ./$PROJECTNAME/bin/Release/netcoreapp5.0/publish/.dockerignore
cd $PROJECTNAME/bin/Release/netcoreapp5.0/publish
docker build -t $DOCKER_REPO_IP:$DOCKER_REPO_PORT/$DOCKER_REPO_SLUG:$DOCKER_REPO_VERSION .
docker push $DOCKER_REPO_IP:$DOCKER_REPO_PORT/$DOCKER_REPO_SLUG:$DOCKER_REPO_VERSION
cd ../../../../../
echo 'Finished!'