var pp;
var hit_count;

$.get("http://locathost:10800/api/pp",function(data)
{
	pp=JSON.parse(data);
});

$.get("http://locathost:10800/api/hit_count",function(data)
{
	hitcount=JSON.parse(data);
});

setInterval(function(){
	var html_pp=document.querySelector("#pp-lable");
	html_pp.innerHTML=pp.rtpp.toFixed(2)+"pp";
},33);