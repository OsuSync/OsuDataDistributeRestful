var _property_mapping_dict={};

//pp
_property_mapping_dict["rtpp_aim"]="RealTimeAimPP";
_property_mapping_dict["rtpp_speed"]="RealTimeSpeedPP";
_property_mapping_dict["rtpp_acc"]="RealTimeAccuracyPP";
_property_mapping_dict["rtpp"]="RealTimePP";

_property_mapping_dict["fcpp_aim"]="FullComboAimPP";
_property_mapping_dict["fcpp_speed"]="FullComboSpeedPP";
_property_mapping_dict["fcpp_acc"]="FullComboAccuracyPP";
_property_mapping_dict["fcpp"]="FullComboPP";

_property_mapping_dict["maxpp_aim"]="MaxAimPP";
_property_mapping_dict["maxpp_speed"]="MaxSpeedPP";
_property_mapping_dict["maxpp_acc"]="MaxAccuracyPP";
_property_mapping_dict["maxpp"]="MaxTimePP";


//hit count
_property_mapping_dict["n300g"]="CountGeki";
_property_mapping_dict["n300"]="Count300";
_property_mapping_dict["n200"]="CountKatu";
_property_mapping_dict["n150"]="Count100";
_property_mapping_dict["n100"]="Count100";
_property_mapping_dict["n50"]="Count50";
_property_mapping_dict["nmiss"]="CountMiss";
_property_mapping_dict["ngeki"]="CountGeki";
_property_mapping_dict["nkatu"]="CountKatu";

_property_mapping_dict["rtmaxcombo"]="RealTimeMaxCombo";
_property_mapping_dict["fullcombo"]="FullCombo";
_property_mapping_dict["maxcombo"]="PlayerMaxCombo";
_property_mapping_dict["combo"]="Combo";

class Formatter
{
	constructor(format)
	{
		var pattern=/\$\{(.+?)\}/g;
		
		this.format=format;
		this.args=[];
		
		for(let prop in _property_mapping_dict)
			this.format=this.format.replace(new RegExp(prop,"g"),_property_mapping_dict[prop]);
		
		while(true)
		{
			let arg=pattern.exec(this.format);
			if(arg==null)break;
			this.args.push(arg[1]);
		}
	}
	
	Format(tuple,digital)
	{
		let formatted=this.format;
			
		for(let i=0;i<this.args.length;++i)
		{
			let expr=this.args[i];
			for(let prop in tuple)
				expr=expr.replace(new RegExp(prop,"g"),tuple[prop]);

			let value=eval(expr).toFixed(digital);
			formatted=formatted.replace("${"+this.args[i]+"}",value);
		}
		
		return formatted;
	}
}