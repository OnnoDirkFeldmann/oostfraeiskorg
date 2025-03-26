import gradio as gr
import torch
from transformers import MarianMTModel, MarianTokenizer

# Load the model and tokenizer
model_path = "./translator_model"
tokenizer = MarianTokenizer.from_pretrained(model_path)
model = MarianMTModel.from_pretrained(model_path)

# Move model to GPU if available
device = "cuda" if torch.cuda.is_available() else "cpu"
model.to(device)

# Enable half precision if using CUDA
if device == "cuda":
    model.half()

# Compile model (PyTorch 2.0+)
try:
    model = torch.compile(model)
except:
    pass  # Ignore if PyTorch <2.0

print(device)

# Translate function
def translate(text):
    with torch.inference_mode():  # Disables gradient computation
        inputs = tokenizer(text, return_tensors="pt")
        inputs.to(device)
        translated = model.generate(**inputs, num_beams=3)

    # Decode output
    translated_text = tokenizer.decode(translated[0], skip_special_tokens=True)
    return translated_text

# Create Gradio interface
gr.Interface(
    fn=translate,
    inputs=gr.Textbox(label="Enter German Text"),
    outputs=gr.Textbox(label="Translated East Frisian Text"),
    title="German to East Frisian Translator",
    description="Enter a German sentence and get the East Frisian translation."
).launch()