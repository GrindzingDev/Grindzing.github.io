from flask import Flask, render_template, request, send_file, redirect, url_for
from PIL import Image
import os
import uuid

app = Flask(__name__)

UPLOAD_FOLDER = 'uploads'
OVERLAY_FOLDER = 'static/overlays'
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

def apply_overlay(image_path, overlay_path):
    base_image = Image.open(image_path).convert("RGBA")
    overlay = Image.open(overlay_path).convert("RGBA")

    # Resize overlay to match the base image size
    overlay = overlay.resize(base_image.size, Image.ANTIALIAS)

    combined = Image.alpha_composite(base_image, overlay)
    output_path = os.path.join(app.config['UPLOAD_FOLDER'], f"output_{uuid.uuid4().hex}.png")
    combined.save(output_path, "PNG")
    return output_path

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/upload', methods=['POST'])
def upload():
    if 'file' not in request.files:
        return redirect(request.url)

    file = request.files['file']
    overlay = request.form.get('overlay')

    if file and overlay:
        filename = f"{uuid.uuid4().hex}_{file.filename}"
        image_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
        file.save(image_path)

        overlay_path = os.path.join(OVERLAY_FOLDER, overlay)
        output_path = apply_overlay(image_path, overlay_path)

        return send_file(output_path, as_attachment=True)

    return redirect(url_for('index'))

if __name__ == '__main__':
    app.run(debug=True)
