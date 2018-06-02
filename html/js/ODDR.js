"use strict";

class ODDR {
    constructor(host, port, isHttps, errorObject = null) {
        this.protocol = (isHttps || false) ? "https" : "http";
        this.host = host || "localhost";
        this.port = port || 10800;

        this.errorObject = errorObject;

        this._uriPrefix = `${this.protocol}://${this.host}:${this.port}/`;
    }

    combineUri(uri) {
        if (uri.charAt(0) === "/")
            uri = uri.replace("/", "");
        return `${this._uriPrefix}/${uri}`;
    }

    async getApis() {
        return this.get("api");
    }

    get(uri, type) {
        type = type || "json";

        return new Promise((resolve, reject) => {
            let xhr = new XMLHttpRequest();

            xhr.onload = function (e) {
                if (xhr.readyState === xhr.DONE) {
                    if (xhr.status === 200) {
                        resolve(xhr.response);
                    }
                    else {
                        resolve(this.errorObject);
                    }
                }
            };

            xhr.onerror = function (e) {
                resolve(this.errorObject);
            };

            xhr.open("GET", this.combineUri(uri), true);
            xhr.responseType = type;
            xhr.send();
        });
    }
}