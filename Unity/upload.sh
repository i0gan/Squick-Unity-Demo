#!/bin/bash
rm -rf Build/application_B*
ssh root@tflash.pwnsky.com "mkdir -p /srv/pwnsky/uquick/updater/www/dlc/"
rsync -avz --delete ./Build/ root@ble.pwnsky.com:/srv/pwnsky/uquick/updater/www/dlc/
sleep 1
