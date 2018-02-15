class Formatter
{
	constructor(format)
	{
		var pattern=/\$\{(.+?)\}/g;
		
		this.format=format;
		this.args=[];
		
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