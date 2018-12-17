"use strict";

String.prototype.splice = function(idx, rem, str) {
    return this.slice(0, idx) + str + this.slice(idx + Math.abs(rem));
};

class Formatter {
	constructor(format,digital = 2) {
		var pattern = /\$\{(([A-Z]|[a-z]|[0-9]|_|\.|,|\(|\)|\^|\+|\-|\*|\/|\%|\<|\>|\=|\!|\||\&)+?)?(@\d+)?\}/g;

		this.format = format;
		this.args = [];
		this.varNames = [];

		while (true) {
			let arg = pattern.exec(this.format);
			if (arg == null) break;
			let rawExpr = arg[0].substring(2, arg[0].length - 1);
			let breakdExpr = rawExpr.split("@");
			if(breakdExpr.length>=2)
				digital = Number.parseInt(breakdExpr[1]);
			this.args.push({
				exprStr: rawExpr,
				expr: this._CreateExprFunction(this.preprocessExpr(breakdExpr[0])),
				digital: digital
			});
		}
	}

	preprocessExpr(expr){
		expr = expr.replace(/if/g,'_if');
		expr = expr.replace(/set\(\s*(\w+)\s*,/g,"set('$1',");
		return expr;
	}

	_set(name,val){
		if(window[name]==undefined){
			window[name]=0;
			this.varNames.push(name);
		}
		window[name]=val;
		return 0;
	}

	resetVariables(){
		for(let name of this.varNames){
			window[name]=0;
		}
	}

	Format(tuple) {
		let formatted = this.format;

		for (let i = 0; i < this.args.length; ++i) {
			let expr = this.args[i].expr;
			let ret = 0;
			while (true) {
				try {
					ret = expr(
						tuple.realTimePP,
						tuple.realTimeAccuracyPP,
						tuple.realTimeAimPP,
						tuple.realTimeSpeedPP,
						tuple.maxPP,
						tuple.maxAccuracyPP,
						tuple.maxAimPP,
						tuple.maxSpeedPP,
						tuple.fullComboPP,
						tuple.fullComboAccuracyPP,
						tuple.fullComboAimPP,
						tuple.fullComboSpeedPP,
						tuple.count300,
						tuple.countGeki,
						tuple.countKatu,
						tuple.count100,
						tuple.count100,
						tuple.count50,
						tuple.countKatu,
						tuple.countGeki,
						tuple.countMiss,
						tuple.currentMaxCombo,
						tuple.currentMaxCombo,
						tuple.playerMaxCombo,
						tuple.playerMaxCombo,
						tuple.fullCombo,
						tuple.combo,
						tuple.objectsCount,
						tuple.time,
						tuple.duration,
						Math.PI,
						Math.E,
						//funtion
						Math.sin,
						Math.cos,
						Math.tan,
						Math.asin,
						Math.acos,
						Math.atan,
						Math.pow,
						Math.sqrt,
						Math.abs,
						Math.max,
						Math.min,
						Math.exp,
						Math.log,
						Math.log10,
						Math.floor,
						Math.ceil,
						Math.round,
						Math.sign,
						Math.trunc,
						(a, min, max) => Math.max(Math.min(a, max), min),
						(a, b, t) => (1 - t) * a + t * b,
						(a, b) => (a || 0) + Math.random() * ((b || 1) - (a || 0)),
						() => new Date().getTime(),
						(a, b) => a % b,
						(cond, a, b) => cond ? a : b,
						(name, val) => this._set(name, val)
					);
					break;
				} catch (e) {
					const message = e.message;
					const regex = /(\w+) is not defined/;
					if (regex.test(message)) {
						let v = regex.exec(message)[1];
						window[v] = 0;
						continue;
					}
					console.error(message);
					break;
				}
			}
			let value = ret.toFixed(this.args[i].digital);
			formatted = formatted.replace("${" + this.args[i].exprStr + "}", value);
		}

		return formatted;
	}

	_CreateExprFunction(expr) {
		return new Function(
			"rtpp",
			"rtpp_acc",
			"rtpp_aim",
			"rtpp_speed",
			"maxpp",
			"maxpp_acc",
			"maxpp_aim",
			"maxpp_speed",
			"fcpp",
			"fcpp_acc",
			"fcpp_aim",
			"fcpp_speed",
			"n300",
			"n300g",
			"n200",
			"n150",
			"n100",
			"n50",
			"nkatu",
			"ngeki",
			"nmiss",
			"rtmaxcombo",
			"current_maxcombo",
			"player_maxcombo",
			"maxcombo",
			"fullcombo",
			"combo",
			"objects_count",
			"playtime",
			"duration",
			"pi",
			"e",
			//function
			"sin",
			"cos",
			"tan",
			"asin",
			"acos",
			"atan",
			"pow",
			"sqrt",
			"abs",
			"max",
			"min",
			"exp",
			"log",
			"log10",
			"floor",
			"ceil",
			"round",
			"sign",
			"truncate",
			"clamp",
			"lerp",
			"random",
			"getTime",
			"mod",
			"_if",
			"set",
			`return ${expr};`
		);
	}
}