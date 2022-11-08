## Purpose

A system service for dynamic update DNS record by `net6` with `Quartz.Net`. only Support [DnsPod](https://docs.dnspod.cn/api/add-domain/)、[AlibabaCloud]([阿里云 OpenAPI 开发者门户 (aliyun.com)](https://next.api.aliyun.com/document/Alidns/2015-01-09)).

## Building
First, you should check the `App` Option in  `appsetting.json`  file.

And then, click the `Hua.DDNS.sln` file open the solution.

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