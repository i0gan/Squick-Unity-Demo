#! /bin/bash
echo "dev rebuild"
docker start squick
docker exec -d squick /mnt/docker/dev/stop.sh
docker exec -t squick /mnt/docker/dev/rebuild.sh
docker exec -d squick /mnt/docker/dev/start.sh