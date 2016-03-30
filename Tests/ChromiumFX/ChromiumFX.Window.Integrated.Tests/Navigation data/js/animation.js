function OnEnter(element,callback){
    $(element).addClass("boxanimated");
    setTimeout(callback, 2000);
}

function OnClose(callback, element){
    $(element).removeClass("boxanimated");
	setTimeout(callback, 2000);
}