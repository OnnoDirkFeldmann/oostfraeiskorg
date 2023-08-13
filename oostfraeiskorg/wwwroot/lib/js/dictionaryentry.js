export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;

        // place the initialization code here
    }

    PlayWbSound(soundfile) {
        var sound = new Audio(soundfile);
        sound.play();
    }

    ShowPopup(title, body) {
        //erst zeigen und dann laden sonst gehts nicht!
        $("#Detailpopup").modal("show");
        $("#Detailpopup .modal-title").html(title);
        $("#Detailpopup .modal-body").html(body);
    }

    async GetPopupInfo(id) {
        try {
            await this.context.namedCommands["SetWordID"](id);
            await this.context.namedCommands["GetPopUpBody"]();
            this.context.namedCommands["ShowPopupWithBody"]();

        } catch (err) {
            // handle error states
            dotvvm.patchState({ ErrorMessage: "The popup generation failed!" });
        }
    }

}