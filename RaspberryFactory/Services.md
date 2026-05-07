# Raspberry Info
## Main Data

**Adress Local:** 192.168.2.116 

**Adress Ddns:** 93.217.246.31 http://roshodns.duckdns.org

**Port ssh:** 2222

## Services
**Mosquitto MQTT broker** 
* _mqtt listener:_ 1883
* _Web Sockets:_ 1885

**WireGuard:** 81822

**Easy-WG (Docker):** 51821

**PostgreSQL:**  5432

**NGinx:** 80, 443

## Port Naming

### SSAAEE / SSSAAAEE
1. **S** - Stack
2. **A** - Appliccation
3. **E** - Type. 
	* **e1,e3,e5,e7,e9** - Web App/UI
	* **e0,e2,e4,e6,e8** - API/Backend/Service

_Example:_

| Type		| Name							| URL						| Port		|
|-----------|-------------------------------|---------------------------|-----------|
| API		| IR.TranslateService.API		| /TranslateService/		| 200101	|
| API		| IR.SampleApp.API				| /SampleApp/API			| 101101	|
| Service	| IR.SampleApp.SampleService	| /SampleApp/SampleService/ | 101103	|
| UI/App	| IR.SampleAPp.SampleUI			| /SampleApp/				| 101100	|