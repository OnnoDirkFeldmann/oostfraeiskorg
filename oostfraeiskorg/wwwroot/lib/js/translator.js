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

        charCount.textContent = input.value.length + '/' + input.maxLength;
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

    async ToggleFeedback(show) {
        const feedbackElement = document.querySelector('.overlay-content');
        if (show) {
            feedbackElement.classList.remove('hidden');
            feedbackElement.classList.add('show');
        } else {
            feedbackElement.classList.remove('show');
            setTimeout(() => {
                feedbackElement.classList.add('hidden');
            }, 500); // Match the CSS transition duration (0.5s)
        }
    }

    async ClearInput() {
        charCount.textContent = '0/' + input.maxLength;
        await this.context.namedCommands["ClearInput"]();
    }

    async CopyOutput() {
        var output = document.getElementById('translation');
        if (output) {
            navigator.clipboard.writeText(output.value);
        }
    }

}

window.onload = function () {
    const heading = document.querySelector('.ooversetter-logo');
    if (heading) {
        heading.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};