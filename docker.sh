# docker build -t bearplan.api:1.0.0 .

#chmod +x docker.sh

# ./docker.sh bearplan.api 3000

#!/bin/bash
processName=$1
serverPort=$2

if [[ -z $processName || -z $serverPort ]]; then
    echo "Usage: $0 <process_name> <server_port>"
    exit 1
fi

WORK_DIR=$(cd $(dirname $0) && pwd)
LOG_DIR=$WORK_DIR/logs
DB_DIR=$WORK_DIR/db

mkdir -p $LOG_DIR
mkdir -p $DB_DIR

chmod -R 777 $LOG_DIR
chmod -R 777 $DB_DIR

IMAGE_NAME="bearplan.api:1.0.0"

docker stop $processName 2>/dev/null
docker rm $processName 2>/dev/null

docker run -d \
--restart always \
--net=host \
# 让容器内能用 host.docker.internal 访问宿主机（Linux 必需）
--add-host=host.docker.internal:host-gateway \
-p $serverPort:$serverPort \
-v $WORK_DIR/publish:/publish \
-v $WORK_DIR/appsettings.Production.json:/publish/appsettings.Production.json \
-v $WORK_DIR/App_Data:/publish/App_Data \
-v $WORK_DIR/wwwroot:/publish/wwwroot \
-v $LOG_DIR:/publish/Logs \
-v $DB_DIR/BearPlan.Log.db:/publish/BearPlan.Log.db \
-e TZ=Asia/Shanghai \
-e ASPNETCORE_ENVIRONMENT=Production \
-e ASPNETCORE_URLS=http://+:$serverPort \
--name $processName \
$IMAGE_NAME

echo "✅ 容器 $processName 启动成功，端口：$serverPort"
echo "📁 日志目录：$LOG_DIR"
echo "📁 数据库目录：$DB_DIR/BearPlan.Log.db"