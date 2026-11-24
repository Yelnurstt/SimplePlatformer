"""Makes every sound and both music tracks of the game.

Own work: the sounds are built from simple waves (square, triangle, noise)
in plain Python, no samples from the internet. Run from the project folder:

    python3 Tools/make_sounds.py

The WAV files go to Assets/Audio. Same file names = Unity keeps the links.
"""
import math
import random
import struct
import wave

OUT = "Assets/Audio/"
SFX_RATE = 44100
MUSIC_RATE = 32000
random.seed(12)


# ---------- basic waves ----------
def square(phase):
    return 1.0 if (phase % 1.0) < 0.5 else -1.0


def triangle(phase):
    p = phase % 1.0
    return 4.0 * p - 1.0 if p < 0.5 else 3.0 - 4.0 * p


def noise(_phase):
    return random.uniform(-1.0, 1.0)


def note_hz(name):
    """'A4' -> 440 Hz. Sharps as 'C#5'."""
    names = {"C": -9, "C#": -8, "D": -7, "D#": -6, "E": -5, "F": -4,
             "F#": -3, "G": -2, "G#": -1, "A": 0, "A#": 1, "B": 2}
    octave = int(name[-1])
    semis = names[name[:-1]] + (octave - 4) * 12
    return 440.0 * 2 ** (semis / 12.0)


def tone(buf, rate, start, length, f0, f1=None, wave_fn=square, vol=0.5,
         attack=0.005, release=0.05, vibrato=0.0):
    """Adds one sound into buf. Pitch slides from f0 to f1."""
    f1 = f0 if f1 is None else f1
    n = int(length * rate)
    s = int(start * rate)
    phase = 0.0
    for i in range(n):
        t = i / rate
        k = i / max(1, n - 1)
        f = f0 + (f1 - f0) * k
        if vibrato:
            f *= 1.0 + 0.03 * math.sin(2 * math.pi * vibrato * t)
        phase += f / rate
        env = min(1.0, t / attack) if attack > 0 else 1.0
        if t > length - release:
            env *= max(0.0, (length - t) / release)
        idx = s + i
        if idx >= len(buf):
            break
        buf[idx] += wave_fn(phase) * vol * env


def save(name, buf, rate, peak=0.85):
    top = max(1e-9, max(abs(x) for x in buf))
    scale = peak / top
    with wave.open(OUT + name, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(rate)
        w.writeframes(b"".join(struct.pack("<h", int(x * scale * 32767)) for x in buf))
    print("made", name, round(len(buf) / rate, 2), "s")


def sfx(length):
    return [0.0] * int(length * SFX_RATE)


# ---------- sound effects ----------
def make_sfx():
    R = SFX_RATE

    b = sfx(0.18)                                  # jump: short rising blip
    tone(b, R, 0, 0.18, 300, 750, square, 0.5, release=0.08)
    save("Jump.wav", b, R, 0.7)

    b = sfx(0.3)                                   # player hit: noise + low tone
    tone(b, R, 0, 0.08, 0, 0, noise, 0.6, release=0.06)
    tone(b, R, 0, 0.3, 180, 70, square, 0.5, release=0.15)
    save("Hurt.wav", b, R)

    for i, f in enumerate([240, 280, 205], start=1):   # enemy hit: 3 versions
        b = sfx(0.14)
        tone(b, R, 0, 0.05, 0, 0, noise, 0.7, release=0.04)
        tone(b, R, 0, 0.14, f, f * 0.55, triangle, 0.7, release=0.07)
        save("EnemyHit%d.wav" % i, b, R, 0.75)

    b = sfx(0.32)                                  # coin: bright two-note chime
    tone(b, R, 0.00, 0.09, note_hz("B5"), None, square, 0.35, release=0.03)
    tone(b, R, 0.08, 0.24, note_hz("E6"), None, square, 0.35, release=0.18)
    save("CoinPickup.wav", b, R, 0.6)

    b = sfx(0.13)                                  # player shot
    tone(b, R, 0, 0.13, 1000, 280, square, 0.45, release=0.07)
    save("Shoot.wav", b, R, 0.55)

    b = sfx(0.6)                                   # enemy wind-up: rising buzz, gets louder
    n = len(b)
    tone(b, R, 0, 0.6, 110, 260, square, 0.5, attack=0.01, release=0.04, vibrato=18)
    for i in range(n):
        b[i] *= 0.25 + 0.75 * (i / n)
    save("WindUp.wav", b, R, 0.8)

    b = sfx(0.7)                                   # player death: falling wobble
    tone(b, R, 0, 0.7, 620, 90, square, 0.5, release=0.25, vibrato=9)
    tone(b, R, 0, 0.12, 0, 0, noise, 0.4, release=0.1)
    save("Death.wav", b, R)

    b = sfx(0.05)                                  # UI click
    tone(b, R, 0, 0.05, 1600, 1100, triangle, 0.6, attack=0.001, release=0.04)
    save("Click.wav", b, R, 0.5)

    b = sfx(0.9)                                   # win: rising arpeggio
    for i, nm in enumerate(["C5", "E5", "G5"]):
        tone(b, R, i * 0.11, 0.12, note_hz(nm), None, square, 0.35, release=0.04)
    tone(b, R, 0.33, 0.55, note_hz("C6"), None, square, 0.35, release=0.4)
    tone(b, R, 0.33, 0.55, note_hz("G5"), None, triangle, 0.3, release=0.4)
    save("Win.wav", b, R, 0.7)

    b = sfx(1.1)                                   # lose: slow falling notes
    for i, nm in enumerate(["G4", "F#4", "F4"]):
        tone(b, R, i * 0.22, 0.22, note_hz(nm), None, triangle, 0.5, release=0.06)
    tone(b, R, 0.66, 0.44, note_hz("E4"), note_hz("D#4"), triangle, 0.5, release=0.35, vibrato=6)
    save("Lose.wav", b, R, 0.7)


# ---------- music ----------
def loop_track(name, bpm, bars, chords, melody, bass_pattern, drums, lead_wave):
    """Builds a loop. Sound that rings past the end is added to the start,
    so the end flows into the start with no click (seamless loop)."""
    R = MUSIC_RATE
    beat = 60.0 / bpm
    length = bars * 4 * beat
    buf = [0.0] * int((length + 2.0) * R)          # 2 s extra for the tails

    for bar in range(bars):
        root, third, fifth = chords[bar % len(chords)]
        t0 = bar * 4 * beat
        # soft chord pad (triangle), one per bar
        for nm in (third, fifth):
            tone(buf, R, t0, 4 * beat, note_hz(nm), None, triangle, 0.12, attack=0.08, release=0.3)
        # bass
        for step, oct_shift in bass_pattern:
            f = note_hz(root) / 2 ** oct_shift
            tone(buf, R, t0 + step * beat, beat * 0.45, f, None, triangle, 0.45, release=0.05)
        # drums: kick = fast falling tone, hat = short noise
        if drums:
            for k in range(4):
                tone(buf, R, t0 + k * beat, 0.12, 150, 45, triangle, 0.6, attack=0.001, release=0.08)
                tone(buf, R, t0 + (k + 0.5) * beat, 0.03, 0, 0, noise, 0.12, attack=0.001, release=0.02)

    # melody: (beat position, note, length in beats) for the whole loop
    for pos, nm, ln in melody:
        tone(buf, R, pos * beat, ln * beat * 0.9, note_hz(nm), None, lead_wave, 0.18,
             attack=0.01, release=min(0.15, ln * beat * 0.4))

    n = int(length * R)
    out = buf[:n]
    for i in range(n, len(buf)):                   # wrap the tail to the start
        out[i - n] += buf[i]
    save(name, out, R, 0.8)


def make_music():
    # Menu: calm, 84 BPM, Am - F - C - G, no drums
    chords = [("A3", "C4", "E4"), ("F3", "A3", "C4"), ("C3", "E3", "G3"), ("G3", "B3", "D4")]
    mel = []
    phrase = ["E5", "C5", "A4", "C5", "F5", "C5", "A4", "C5",
              "G5", "E5", "C5", "E5", "D5", "B4", "G4", "B4"]
    for bar in range(8):
        for i in range(4):
            mel.append((bar * 4 + i, phrase[(bar % 4) * 4 + i], 1))
    loop_track("MenuMusic.wav", 84, 8, chords, mel, [(0, 1), (2, 1)], False, triangle)

    # Level: faster, 140 BPM, Em - C - G - D, drums and running bass
    chords = [("E3", "G3", "B3"), ("C3", "E3", "G3"), ("G3", "B3", "D4"), ("D3", "F#3", "A3")]
    riff = [["E5", "G5", "B5", "G5", "E5", "D5", "E5", "B4"],
            ["C5", "E5", "G5", "E5", "C5", "B4", "C5", "G4"],
            ["G5", "B5", "D6", "B5", "G5", "F#5", "G5", "D5"],
            ["F#5", "A5", "D6", "A5", "F#5", "E5", "D5", "A4"]]
    mel = []
    for bar in range(8):
        notes = riff[bar % 4]
        for i, nm in enumerate(notes):
            if bar >= 4 and i % 2 == 1:
                continue                           # second half: fewer notes, more air
            mel.append((bar * 4 + i * 0.5, nm, 0.5 if bar < 4 else 1))
    bass = [(i * 0.5, 1 if i % 2 == 0 else 0) for i in range(8)]
    loop_track("LevelMusic.wav", 140, 8, chords, mel, bass, True, square)


if __name__ == "__main__":
    make_sfx()
    make_music()
