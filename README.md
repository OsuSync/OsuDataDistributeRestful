# How to use?
1. download [zip](https://github.com/KedamaOvO/OsuDataDistributeRestful/releases) .(need [ortdp](https://github.com/KedamaOvO/OsuRTDataProvider-Release/releases) 1.4.0 or later)
2. copy to **Sync folde**r
3. add **restful** to **OutputMethods** in **config.ini**
4. add **Browser Source** to scene in **OBS**. 
5. type **{your sync folder}/html/rtpp.html** or **http://oddr.kedamaovo.net/OsuDataDistributeRestful/rtpp.html** in Browser Source

# Config.ini
| Setting Name | Default Value | Description |
| ----|----|----|
| AllowLAN | False | Whether to allow LAN users to access |
| EnableFileHttpServer | False |Whether to enable the song file server(host=localhost:10801 e.g: http://localhost:10801/rtbpm.html)|
| FileServerRootPath | ../html |if EnableSongsHttpServer=True,Must be set|
| ApiPort | 10800 | Api server port|
| FilePort | 10801 | File server port

# Screenshots
![](https://image.prntscr.com/image/09uQzDbjR2yTXqOmTbsuRw.png)

# API
* GET http://localhost:10800/api API List
* OsuDataDistributeRestful Api([ODDR.js](https://github.com/OsuSync/OsuDataDistributeRestful/blob/master/html/js/ODDR.js))
* OsuRTDataProvider Api([ORTDP.js](https://github.com/OsuSync/OsuDataDistributeRestful/blob/master/html/js/ORTDP.js))
* RealTimePPDisplayer Api([RTPPD.js](https://github.com/OsuSync/OsuDataDistributeRestful/blob/master/html/js/rtppd/RTPPD.js))