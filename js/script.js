var html_hit_count=document.querySelector("#hit-count-lable");;
var html_pp=document.querySelector("#pp-lable");



setInterval(function(){
	$.get("http://localhost:10800/api/pp_formated",function(pp)
	{
		html_pp.textContent=pp.Formated;
	});
	
	$.get("http://localhost:10800/api/hit_count_formated",function(hit_count)
	{
		html_hit_count.textContent=hit_count.Formated;
}	);
},33);