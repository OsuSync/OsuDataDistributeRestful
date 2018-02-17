# How to use?
1. download [zip](https://github.com/KedamaOvO/OsuDataDistributeRestful/releases) .(need [ortdp](https://github.com/KedamaOvO/OsuRTDataProvider-Release/releases) 1.3.2 or later)
2. copy to **Sync folde**r
3. add **restful** to **OutputMethods** in **config.ini**
4. add **Browser Source** to scene in **OBS**. 
5. type **{your sync folder}/html/rtpp.html** or **http://github.mao-yu.net/OsuDataDistributeRestful/rtpp.html** in Browser Source

# Config.ini
| Setting Name | Default Value | Description |
| ----|----|----|
| AllowLAN | False | Whether to allow LAN users to access |
| EnableSongsHttpServer | False |Whether to enable the song file server|
| OsuSongsPath| (null) |if EnableSongsHttpServer=True,Must be set|


# API
* GET http://localhost:10800/api API List

## RaslTimePPDisplayer
* GET http://localhost:10800/api/rtppd/playing Playing Status
* GET http://localhost:10800/api/rtppd/pp PP Information
* GET http://localhost:10800/api/rtppd/hit_count Hit Count Informattion
* GET http://localhost:10800/api/rtppd/pp_formated Formatted PP String
* GET http://localhost:10800/api/rtppd/hit_count_formated Formatted Hit Count String

## OsuLiveStatusPanel
* GET /api/olsp/olsp_bg_path
* GET /api/olsp/olsp_status
* GET /api/olsp/olsp_bg_save_path
* GET /api/olsp/olsp_mod_save_path
* GET /api/olsp/olsp_ppshow_config_path
* GET /api/olsp/olsp_source
* GET /api/olsp/ar
* GET /api/olsp/cs
* GET /api/olsp/od
* GET /api/olsp/hp
* GET /api/olsp/pp
* GET /api/olsp/beatmap_setid
* GET /api/olsp/version
* GET /api/olsp/title_avaliable
* GET /api/olsp/artist_avaliable
* GET /api/olsp/beatmap_setlink
* GET /api/olsp/beatmap_link
* GET /api/olsp/beatmap_id
* GET /api/olsp/min_bpm
* GET /api/olsp/max_bpm
* GET /api/olsp/speed_stars
* GET /api/olsp/aim_stars
* GET /api/olsp/stars
* GET /api/olsp/mods
* GET /api/olsp/title
* GET /api/olsp/creator
* GET /api/olsp/max_combo
* GET /api/olsp/artist
* GET /api/olsp/circles
* GET /api/olsp/spinners

## OsuRTDataProvider
* GET /api/ortdp/tourney_mode
* GET /api/ortdp/listener_count
* GET /api/ortdp/playing_time
* GET /api/ortdp/beatmap
* GET /api/ortdp/hitcount
* GET /api/ortdp/hit_count
* GET /api/ortdp/hp
* GET /api/ortdp/acc
* GET /api/ortdp/game_mode
* GET /api/ortdp/mods
* GET /api/ortdp/score
* GET /api/ortdp/game_status
