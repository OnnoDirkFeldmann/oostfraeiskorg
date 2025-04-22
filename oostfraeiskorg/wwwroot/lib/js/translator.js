export default (contextApp) => new App(contextApp);

class App {
    constructor(context) {
        // store the context so it can be accessed later
        this.context = context;

        const input = document.getElementById('input');

        input.addEventListener("focus", (event) => {
            this.context.namedCommands["DeactivateFeedback"]();
        });

        var charCount = document.getElementById('charCount');

        input.addEventListener('input', function () {
            charCount.textContent = input.value.length + '/' + input.maxLength;
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

window.onload = function () {
    const heading = document.querySelector('h2.text-center.mt-3');
    if (heading) {
        heading.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};