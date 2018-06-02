"use strict";

class RTPPD {
    constructor(oddr) {
        this._oddr = oddr;
    }

    async getPerformancePointFormatString() {
        return await this._oddr.get('api/rtppd/ppFormat');
    }

    async getHitCountFormatString() {
        return await this._oddr.get('api/rtppd/hitCountFormat');
    }

    async getPerformancePointAt(id = 0) {
        return await this._oddr.get(`api/rtppd/pp/${id}`);
    }

    async getPerformancePointList() {
        return await this._oddr.get(`api/rtppd/pp}`);
    }

    async getHitCountAt(id = 0) {
        return await this._oddr.get(`api/rtppd/hitCount/${id}`);
    }

    async getHitCountList() {
        return await this._oddr.get(`api/rtppd/hitCount`);
    }

    async getPlayingStatusAt(id = 0) {
        return await this._oddr.get(`api/rtppd/playingStatus/${id}`);
    }

    async getPlayingStatusList() {
        return await this._oddr.get(`api/rtppd/playingStatus`);
    }
}