#!/bin/bash
ssh root@tflash.pwnsky.com "mkdir -p /srv/pwnsky/uquick/updater/www/dlc/"
rsync -avz --delete ./DLC/ root@ble.pwnsky.com:/srv/pwnsky/uquick/updater/www/dlc/
sleep 1
