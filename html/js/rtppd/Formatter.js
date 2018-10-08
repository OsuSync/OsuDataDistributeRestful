"use strict";

class Formatter {
	constructor(format) {
		var pattern = /\$\{(([A-Z]|[a-z]|[0-9]|_|\.|,|\(|\)|\^|\+|\-|\*|\/)+?)?(@\d+)?\}/g;

		this.format = format;
		this.args = [];

		while (true) {
			let arg = pattern.exec(this.format);
			let digital = 2;
			if (arg == null) break;
			let rawExpr = arg[0].substring(2, arg[0].length - 1)
			let breakdExpr = rawExpr.split("@");
			this.args.push({
				exprStr: rawExpr,
				expr: this._CreateExprFunction(breakdExpr[0]),
				digital: Number.parseInt(breakdExpr[1])
			});
		}
	}

	Format(tuple, digital) {
		let formatted = this.format;

		for (let i = 0; i < this.args.length; ++i) {
			let expr = this.args[i].expr;
			let ret = expr(
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
				(a, b) => a % b
			);
			let value = ret.toFixed(digital);
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
			"time",
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
			`return ${expr};`
		);
	}
}