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
    "pgConnection": "Host=127.0.0.1;Port=5432;Database=Worker;Username=Worker;Password=123456;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "App": {
    "AppJob": {
      "Corn": "* * * * * ?" //https://cron.qqe2.com/
    }
  },
  "DDNS": {
    "Platform": 3, //1 Ali 2 Tencent 3 Namesilo
    // 主域名
    "Domain": "we965.com",
    // 子域名前缀
    "SubDomainArray": [ "git", "webutil", "dev" ],
    // 记录类型
    "type": "A",
    //间隔时间 秒
    "time": "30"
  },
  "Namesilo": {
    "ApiKey": "1111"
  },
  "Dnspod": {
    "Id": "1111",
    "Key": "1111",
    "Endpoint": "1111"
  },
  "Ali": {
    "Id": "1111",
    "Key": "1111",
    "Endpoint": "1111"
  }
}