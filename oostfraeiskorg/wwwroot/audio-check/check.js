const fileInput = document.querySelector('#audio-file');
const button = document.querySelector('#check');
const status = document.querySelector('#status');
const result = document.querySelector('#result');
const technical = document.querySelector('#technical');
let worker;

// Parse the original bytes before audio decoding, which may discard metadata.
export function readWav(buffer) {
    const view = new DataView(buffer);
    const id = (p, n = 4) => String.fromCharCode(...new Uint8Array(buffer, p, n));
    if (buffer.byteLength < 12 || id(0) !== 'RIFF' || id(8) !== 'WAVE') throw Error('Bitte eine gültige WAV-Datei auswählen.');
    const end = view.getUint32(4, true) + 8;
    if (end !== buffer.byteLength) throw Error('Die WAV-Datei ist beschädigt oder unvollständig.');
    let format, data, metadata = null;
    for (let p = 12; p < end;) {
        if (p + 8 > end) throw Error('Die WAV-Datei enthält einen unvollständigen Datenblock.');
        const tag = id(p), size = view.getUint32(p + 4, true), start = p + 8;
        if (start + size + (size % 2) > end) throw Error('Die WAV-Datei enthält ungültige Datenlängen.');
        if (tag === 'fmt ' && size >= 16) format = { encoding: view.getUint16(start, true), channels: view.getUint16(start + 2, true), rate: view.getUint32(start + 4, true), bits: view.getUint16(start + 14, true) };
        if (tag === 'data') data = { start, size };
        if (tag === 'aigc' && size <= 65536) {
            try { metadata = JSON.parse(new TextDecoder().decode(new Uint8Array(buffer, start, size))); } catch { /* Continue independent waveform check. */ }
        }
        p = start + size + (size % 2);
    }
    if (!format || !data || format.encoding !== 1 || format.bits !== 16 || ![1, 2].includes(format.channels) || format.rate < 8000 || format.rate > 96000) throw Error('Bitte eine unkomprimierte 16-Bit-WAV-Datei verwenden (Mono oder Stereo). Ooversetter-Downloads werden unterstützt.');
    const frames = data.size / (2 * format.channels);
    if (!Number.isInteger(frames) || frames < format.rate * .5 || frames > format.rate * 120) throw Error('Die Datei muss zwischen 0,5 Sekunden und 2 Minuten lang sein.');
    const mono = new Float32Array(frames);
    for (let i = 0; i < frames; i++) {
        for (let c = 0; c < format.channels; c++) mono[i] += view.getInt16(data.start + 2 * (i * format.channels + c), true) / (32768 * format.channels);
    }
    // Match the service's linear interpolation (align_corners=false).
    const length = Math.round(frames * 16000 / format.rate), samples = new Float32Array(length);
    for (let i = 0; i < length; i++) {
        const pos = Math.max(0, Math.min(frames - 1, (i + .5) * frames / length - .5));
        const left = Math.floor(pos), mix = pos - left;
        samples[i] = mono[left] * (1 - mix) + mono[Math.min(left + 1, frames - 1)] * mix;
    }
    return { samples, metadata, duration: frames / format.rate };
}

button.addEventListener('click', async () => {
    const file = fileInput.files[0];
    result.hidden = true; technical.hidden = true;
    if (!file) { status.textContent = 'Bitte zuerst eine WAV-Datei auswählen.'; return; }
    if (file.size > 20 * 1024 * 1024) { status.textContent = 'Die Datei darf höchstens 20 MB groß sein.'; return; }
    button.disabled = true; fileInput.disabled = true;
    try {
        const audio = readWav(await file.arrayBuffer());
        status.textContent = 'Prüfsoftware wird geladen … Die Datei bleibt auf deinem Gerät.';
        worker ??= new Worker('/audio-check/worker.js');
        const found = await new Promise((resolve, reject) => {
            const timer = setTimeout(() => { worker.terminate(); worker = null; reject(Error('Die Prüfung dauert auf diesem Gerät zu lange. Bitte einen kürzeren Ausschnitt versuchen.')); }, 240000);
            worker.onerror = () => { clearTimeout(timer); worker.terminate(); worker = null; reject(Error('Die Prüfsoftware konnte nicht geladen werden. Bitte erneut versuchen oder einen aktuellen Browser verwenden.')); };
            worker.onmessage = ({ data }) => {
                if (data.progress) { status.textContent = data.progress; return; }
                clearTimeout(timer);
                if (data.error) reject(Error(data.error)); else resolve(data);
            };
            worker.postMessage({ samples: audio.samples }, [audio.samples.buffer]);
        });
        const marked = found.score >= .8;
        const claimed = audio.metadata?.schema === 'org.oostfraeisk.ai-audio-provenance' && audio.metadata?.ai_generated === true;
        result.textContent = (marked ? 'AudioSeal-Wasserzeichen erkannt.' : 'Kein eindeutiges AudioSeal-Wasserzeichen erkannt. Das beweist nicht, dass die Aufnahme menschlich erzeugt wurde.')
            + '\n' + (claimed ? 'Die Dateiinformationen kennzeichnen diese Aufnahme als KI-generiert. Diese Angabe ist nicht unabhängig bestätigt.' : 'Keine bekannte KI-Kennzeichnung in den Dateiinformationen gefunden.');
        result.hidden = false;
        document.querySelector('#technical-result').textContent = JSON.stringify({ audioseal_marked_frame_fraction: found.score, decision_threshold: .8, decoded_bits: marked ? found.bits : null, seconds_checked: audio.duration, unsigned_metadata: audio.metadata }, null, 2);
        technical.hidden = false;
        status.textContent = 'Prüfung abgeschlossen. Die gesamte Aufnahme wurde geprüft.';
    } catch (error) {
        status.textContent = error.message || 'Die Datei konnte nicht geprüft werden.';
    } finally {
        button.disabled = false; fileInput.disabled = false;
    }
});
