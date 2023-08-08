function ShowPopup(title, body) {
	//erst zeigen und dann laden sonst gehts nicht!
	$("#Detailpopup").modal("show");
	$("#Detailpopup .modal-title").html(title);
	$("#Detailpopup .modal-body").html(body);
}

function PlayWbSound(soundfile) {
	var sound = new Audio(soundfile);
	sound.play();
}