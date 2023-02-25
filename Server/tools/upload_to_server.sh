#!/bin/bash
#ssh root@tflash.pwnsky.com "pkill squick"
rsync -avz --delete ./deploy/ root@ble.pwnsky.com:/srv/pwnsky/squick
#ssh root@tflash.pwnsky.com "tmux send -t t-flash \"cd /srv/t-flash/server && ./tflash http :80 &\" ENTER"
