export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;

        // place the initialization code here
    }

    StartTranslation() {
        document.getElementById('translation').value = '...';
        this.context.namedCommands["TranslateAsync"]();
    }

}