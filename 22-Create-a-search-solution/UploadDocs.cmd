@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

rem Set values for your storage account
set subscription_id=98e05785-9393-44e4-a65b-9ee640a6b4da
set azure_storage_account=hongph85storage
set azure_storage_key=puckX1kQBpFCYEkL4mfzWiFfX/0G5kxH8NXdL1SdxLUSM7ro3zw1nHGQ6yUZnb04D1/RmOwjoHX9vo/jSl1B4g==


echo Creating container...
call az storage container create --account-name !azure_storage_account! --subscription !subscription_id! --name margies --public-access blob --auth-mode key --account-key !azure_storage_key! --output none

echo Uploading files...
call az storage blob upload-batch -d margies -s data --account-name !azure_storage_account! --auth-mode key --account-key !azure_storage_key!  --output none
