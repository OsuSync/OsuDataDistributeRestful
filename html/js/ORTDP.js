"use strict";

class ORTDP {
    constructor(oddr){
        this._oddr=oddr;

        this.STATUS_NO_FOUND_PROCESS = 1 << 0;
        this.STATUS_UNKONWN = 1 << 1;
        this.STATUS_SELECT_SONG = 1<<2,
        this.STATUS_PLAYING = 1 << 3;
        this.STATUS_EDITING = 1 << 4;
        this.STATUS_RANK = 1 << 5;
        this.STATUS_MATCH_SETUP = 1 << 6;
        this.STATUS_LOBBY = 1 << 7;
        this.STATUS_IDLE = 1 << 8;
    }

    async getIsTourneyMode(){
        return await this._oddr.get(`api/ortdp/isTournetMode`);
    }

    async getTournetModeListenCount(){
        return await this._oddr.get(`api/ortdp/tournetModeListenerCount`);
    }

    async getBeatmapInfo(){
        let info = await this._oddr.get(`api/ortdp/beatmap/info`);
        if(info!==null){
            info.equals=function(b){
                for(let key in this){
                    if(typeof(this[key]) === "function")continue;
                    if(this[key] !== b[key])return false;
                }
                return true;
            }
        }

        return info;
    }

    async getBeatmap(){
        return await this._oddr.get(`api/ortdp/beatmap`,"text");
    }

    async getBeatmapAudio(type = "object"){
        let realtype = (type === "object" ? "blob" : type);
        let data = await this._oddr.get(`api/ortdp/beatmap/audio`,realtype);

        if(type !== "object"){
            return data;
        }else{
            let audio = document.createElement('audio');
            audio.onload = function(e) {
              window.URL.revokeObjectURL(audio.src);
            };
    
            audio.src=window.URL.createObjectURL(data);
    
            return audio;
        }
    }

    async getBeatmapBackground(type = "object"){
        let realtype = (type === "object" ? "blob" : type);
        let data = await this._oddr.get(`api/ortdp/beatmap/background`,realtype);

        if(type !== "object"){
            return data;
        }else{
            let img = document.createElement('img');
            img.onload = function (e) {
                window.URL.revokeObjectURL(img.src);
            };

            img.src = window.URL.createObjectURL(data);

            return img;
        }
    }

    async getBeatmapVideo(type = "object"){
        let realtype = (type === "object" ? "blob" : type);

        let data = await this._oddr.get(`api/ortdp/beatmap/video`,realtype);

        if(type !== "object"){
            return data;
        }else{
            let video = document.createElement('video');
            video.onload = function (e) {
                window.URL.revokeObjectURL(video.src);
            };

            video.src = window.URL.createObjectURL(data);

            return video;
        }
    }

    async getPlayingInfoAt(id = 0){
        return await this._oddr.get(`api/ortdp/playing/info/${id}`);
    }

    async getPlayingInfoList(id = 0){
        return await this._oddr.get(`api/ortdp/playing/info`);
    }

    async getPlayingModsAt(id = 0){
        return await this._oddr.get(`api/ortdp/playing/mods/${id}`);
    }

    async getPlayingModsList(id = 0){
        return await this._oddr.get(`api/ortdp/playing/mods`);
    }

    async getPlayingTime(){
        return await this._oddr.get(`api/ortdp/playing/time`);
    }

    async getGameStatusAt(id=0){
        return await this._oddr.get(`api/ortdp/gameStatus/${id}`);
    }

    async getGameStatusList(){
        return await this._oddr.get(`api/ortdp/gameStatus`);
    }

    async getGameModeAt(id=0){
        return await this._oddr.get(`api/ortdp/gameMode/${id}`);
    }

    async getGameModeList(){
        return await this._oddr.get(`api/ortdp/gameMode`);
    }

    isListening(status){
        if(status instanceof Object)
            status = status.status;

        let listen = this.STATUS_SELECT_SONG | this.STATUS_MATCH_SETUP | this.STATUS_LOBBY | this.IDLE;
        return (listen & status) == status;
    }
}