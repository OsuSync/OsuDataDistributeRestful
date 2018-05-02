"use strict";

let BeatmapEffectFlags=Object.freeze({
    None:0,
    Kiai:1,
    OmitFirstBarLine:8
});

class Beatmap{
    constructor(str){
        this.general={};
        this.editor={};
        this.metadata={};
        this.difficulty={};
        this.timingPoints=[];
        
        this._parse(str.split(/\r?\n/));
    }
    
    _parse(lines){
        let block="";
        for(let line of lines){
            line=line.trim();
            if(line.length==0)continue;
            
            if(line.charAt(0)=="["){
                block=line;
                continue;
            }else if(block=="[General]"){
                let prop=this._getProp(line);
                let lowerKey=this._at0ToLower(prop.key)
                
                this.general[lowerKey]=prop.value;
            }else if(block=="[Editor]"){
                let prop=this._getProp(line);
                let lowerKey=this._at0ToLower(prop.key)
                
                this.editor[lowerKey]=prop.value;
            }else if(block=="[Metadata]"){
                let prop=this._getProp(line);
                let lowerKey=this._at0ToLower(prop.key)
                if(lowerKey=="tags")
                    prop.value=prop.value.split(/\s/);
                this.metadata[lowerKey]=prop.value;
            }else if(block=="[Difficulty]"){
                let prop=this._getProp(line);
                let lowerKey=this._at0ToLower(prop.key)
                
                this.difficulty[lowerKey]=prop.value;
            }else if(block=="[TimingPoints]"){
                let timing=this._parseTiming(line);
                this.timingPoints.push(timing);
            }
        }
        
        this.maxBpm=Number.NEGATIVE_INFINITY;
        this.minBpm=Number.POSITIVE_INFINITY;
        for(let timing of this.timingPoints){
            if(timing.beatChange){
                this.maxBpm=Math.max(this.maxBpm,timing.bpm);
                this.minBpm=Math.min(this.minBpm,timing.bpm);
            }
        }
    }
    
    getTiming(time){
        for(let timing of this.timingPoints){
            if(timing.beatChange){
                if(time>=timing.offset)
                    return timing;
            }
        }
    }
    
    _parseTiming(str){
        let breaked=str.split(/,/);
        let timing={
            offset:0,
            timingChange:true,
            timeSignature:4,
            effectFlags:0
        };
        
        let offset=Number(breaked[0]);
        let beatLength=Number(breaked[1]);
        timing.timeSignature=Number(breaked[2]|"4");
        timing.offset=offset;
        timing.effectFlags=Number(breaked[7]|"0");
        
//        if(breaked.length>6)
//            timing.timingChange=(breaked[6]=="1"||breaked[6].trim().length==0);
        
        if(beatLength<0){
            timing.beatChange=false;
            timing.sliderSpeedPercent=-beatLength/100;
        }else{
            timing.beatChange=true;
            timing.bpm=60000.0/beatLength;
            timing.beatInterval=beatLength/1000.0;
        }
        
        return timing;
    }
    
    static get EFFECT_FLAGS(){
        return BeatmapEffectFlags;
    }
   
    _at0ToLower(str){
        let at0=str.charAt(0);
        let at0Lower=at0.toLowerCase();
        return str.replace(at0,at0Lower);
    }
    
    _getProp(line){
        let arr=line.split(":");
        for(let i=0;i<arr.length;i++)
            arr[i]=arr[i].trim();
        if(this._isNumeric(arr[1]))
            arr[1]=Number(arr[1]);
        
        return {key:arr[0],value:arr[1]};
    }
    
    _isNumeric(str){
        return /^-?\d+(\.\d+)?$/.test(str);
    }
}