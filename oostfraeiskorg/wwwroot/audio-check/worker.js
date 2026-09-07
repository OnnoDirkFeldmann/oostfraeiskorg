/* Audio never leaves this worker/browser. Only public detector assets are fetched. */
importScripts('/audio-check/ort.wasm.min.js');
ort.env.wasm.numThreads = 1;
ort.env.wasm.wasmPaths = '/audio-check/';
let sessionPromise;
self.onmessage = async ({ data }) => {
    try {
        sessionPromise ??= ort.InferenceSession.create('/audio-check/audioseal-detector.bin', { executionProviders: ['wasm'] });
        const session = await sessionPromise;
        const samples = data.samples, size = 80000;
        let totalScore = 0;
        const bitTotals = new Float64Array(16);
        for (let start = 0; start < samples.length; start += size) {
            const count = Math.min(size, samples.length - start);
            const block = new Float32Array(size);
            block.set(samples.subarray(start, start + count));
            const inputs = {
                audio: new ort.Tensor('float32', block, [1, 1, size]),
                valid_samples: new ort.Tensor('int64', new BigInt64Array([BigInt(count)]), [1])
            };
            const outputs = await session.run(inputs);
            totalScore += outputs.score.data[0] * count;
            for (let i = 0; i < 16; i++) bitTotals[i] += outputs.bits.data[i] * count;
            Object.values(inputs).forEach(t => t.dispose());
            Object.values(outputs).forEach(t => t.dispose());
            self.postMessage({ progress: `Wasserzeichen wird geprüft … ${Math.round((start + count) / samples.length * 100)} %` });
        }
        self.postMessage({ score: totalScore / samples.length, bits: Array.from(bitTotals, b => b / samples.length > .5 ? '1' : '0').join('') });
    } catch (error) {
        sessionPromise = null;
        self.postMessage({ error: 'Die Wasserzeichenprüfung ist fehlgeschlagen. Bitte erneut versuchen oder einen aktuellen Browser verwenden.' });
        console.error(error);
    }
};
