export default context => {

    return {
        sum(number1, number2) {
            return number1 + number2;
        }
    }
}

/*export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;

        // place the initialization code here
    }

    test() {

    }

    PlayWbSound(soundfile) {
        var sound = new Audio(soundfile);
        sound.play();
    }

    CreatePopup(id) {
        //GetPopupInfo(id);
        return false;
    }

    ShowPopup(title, body) {
        //erst zeigen und dann laden sonst gehts nicht!
        $("#Detailpopup").modal("show");
        $("#Detailpopup .modal-title").html(title);
        $("#Detailpopup .modal-body").html(body);
    }

    GetPopupInfo(id) {
        ShowPopup("", "");
        // call the named command in the page
        this.context.namedCommands["showPopup"](id);
        // do something after the data was refreshed
    }

}*/