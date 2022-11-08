## Purpose

A system service for dynamic update DNS record by `net6` with `Quartz.Net`. only Support [DnsPod](https://docs.dnspod.cn/api/add-domain/)、[AlibabaCloud]([阿里云 OpenAPI 开发者门户 (aliyun.com)](https://next.api.aliyun.com/document/Alidns/2015-01-09)).

## Deploy

1. Copy the folder `\bin\Debug\net6.0` to a new path and open it .
2. Configure  the `App` option in  `appsetting.json`  file.
3. In Windows system, configure the service name in  `InstallServiceByNssm.bat` file , and then double click the BAT file.

## Building
Check and configure  the `App` option in  `appsetting.json`  file, and then click the `Hua.DDNS.sln` file open the solution.

## Configuration

Example of config in `appsetting.json`
```json
{
  "ConnectionStrings": {
    "pgConnection": "Host=127.0.0.1;Port=5432;Database=Worker;Username=Worker;Password=123456;"//LogDbConnection
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "App": {
    "AppJob": {
      "Corn": "0/15 * * * * ? " //a corn expression which defined strike time and frequency.this is a util website for generate an corn expression https://cron.qqe2.com/
      
    },

    "Domain": {
      "Platform": "Ali", //platform from 'Tencent' or 'Ali'
      "Id": "Id",//get the id and key from https://dc.console.aliyun.com/ Or https://console.cloud.tencent.com/cam/capi
      "Key": "Key",
      "domain": "demo.cn",
      "subDomainArray": [ "www", "@","git"],
      "type": "A",//this is not using
      "time": "30"//this is not using
    }
}