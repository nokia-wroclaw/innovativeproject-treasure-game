from flask import Flask, render_template, request
from werkzeug import secure_filename
import os
import time
from json import dumps

app = Flask(__name__)


global index
map_dir = 'maps'


@app.route('/upload')
def upload_file1():
    return render_template('upload.html')


@app.route('/maps', methods=['GET'])
def process():
    rv = {file: time.strftime('%D %H:%M:%S', time.gmtime(
        os.path.getmtime('maps\\'+file))) for file in os.listdir(map_dir)}
    return dumps(rv)


@app.route('/uploader', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        f = request.files['file']
        filename = ''.join(f.filename.split('.')[:-1])
        f.save(secure_filename('maps\\' + filename + str(index) + '.json'))
        return 'file uploaded successfully'


if __name__ == '__main__':
    global index
    index = len(os.listdir('maps')) + 1
    app.run(debug=True)
