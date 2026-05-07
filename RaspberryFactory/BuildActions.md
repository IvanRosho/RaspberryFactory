# Build & Delivery Actions VS -> Raspberry (Linux OS)
## Build VS
### Build Project
Full App Path Example: C:\repos\RaspberryFactory\RaspberryFactory\RaspberryDashboard\bin\Release\net8.0 **\publish\***

```
cd <App Path>
dotnet publish -c Release -r linux-arm64 --self-contained false

Compress-Archive -Path <full AppPath>\publish\* -DestinationPath dashboard.zip  
``` 


## Delivery
```
scp -P 2222 -r "<Full App Path Publish>\*" ivan@192.168.2.116:/home/ivan/<AppName>
```

## Delivery Docker
```
scp -P 2222 <AppName>.tar.gz ivan@192.168.2.116:/home/ivan/<AppName>
```

## Docker Unzip
```
unzip <FileName> -d dashboard
docker load < <AppName>.tar
```

## Docker Run
```
docker run -d \
  -p <OuterPort>:<InnerPort> \
  --name <AppName> \
  <ImageName>:latest
```