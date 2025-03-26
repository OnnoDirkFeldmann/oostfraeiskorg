export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;

        const input = document.getElementById('input');

        input.addEventListener("focus", (event) => {
            this.context.namedCommands["DeactivateFeedback"]();
        });
    }

    async StartTranslation() {
        try {
            await this.context.namedCommands["PrepareTranslation"]();
            await this.context.namedCommands["TranslateAsync"]();
        } catch (err) {
            // handle error states
            dotvvm.patchState({ ErrorMessage: "Translation failed!" });
        }
    }

}