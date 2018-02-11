var html_hit_count=document.querySelector("#hit-count-lable");;
var html_pp=document.querySelector("#pp-lable");



setInterval(function(){
	$.get("http://localhost:10800/api/pp",function(pp)
	{
		html_pp.textContent=pp.RealTimePP.toFixed(2)+"pp";
	});
	
	$.get("http://localhost:10800/api/hit_count",function(hit_count)
	{
		html_hit_count.textContent=hit_count.Count100+"x100 "+hit_count.Count50+"x50 "+hit_count.CountMiss+"xMiss";
}	);
},100);