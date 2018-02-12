var html_hit_count=document.querySelector("#hit-count-lable");;
var html_pp=document.querySelector("#pp-lable");

var _url=new URL(window.location.href);
var host=_url.searchParams.get('host');
var port=_url.searchParams.get('port');

if(host==null)host="localhost";
if(port==null)port="10800";


setInterval(function(){
	var web="http://"+host+":"+port;
	
	$.get(web+"/api/pp_formated",function(pp)
	{
		html_pp.textContent=pp.Formated;
	});
	
	$.get(web+"/api/hit_count_formated",function(hit_count)
	{
		html_hit_count.textContent=hit_count.Formated;
}	);
},33);