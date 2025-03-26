export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;
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