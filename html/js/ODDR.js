"use strict";

class ODDR {
    //options.isHttps
    //options.errorObject
    constructor(host, port, options={}) {
        this.protocol = (options.isHttps || false) ? "https" : "http";
        this.host = host || "localhost";
        this.port = port || 10800;


        this.errorObject = options.errorObject || null;

        this._uriPrefix = `${this.protocol}://${this.host}:${this.port}/`;
    }

    combineUri(uri) {
        if (uri.charAt(0) === "/")
            uri = uri.replace("/", "");
        return `${this._uriPrefix}${uri}`;
    }

    async getApis() {
        return this.get("api");
    }

    get(uri, type = "json") {
        return new Promise((resolve, reject) => {
            let xhr = new XMLHttpRequest();

            xhr.onload = function (e) {
                if (xhr.readyState === xhr.DONE) {
                    if (xhr.status === 200) {
                        resolve(xhr.response);
                    }
                    else {
                        if(type == 'json'){
                            if(xhr.response.reason!=undefined)
                                console.error(xhr.response.reason);
                            resolve(Object.apply(this.errorObject,
                                {
                                    code: xhr.status,
                                    reason: xhr.response.reason
                                }
                            ));
                        }else{
                            resolve(xhr.response);
                        }
                    }
                }
            };

            xhr.onerror = function (e) {
                resolve(Object.apply(e,this.errorObject));
            };

            xhr.open("GET", this.combineUri(uri), true);
            xhr.responseType = type;
            xhr.send();
        });
    }
}