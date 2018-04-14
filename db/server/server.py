from flask import Flask, render_template, request
from pymongo import MongoClient
import os
import time
from json import dumps, loads

app = Flask(__name__)


global index
map_dir = 'maps'

client = MongoClient(
    'mongodb+srv://webapp:XD12345@game-7sfnp.mongodb.net/test')

db = client.business


@app.route('/upload')
def upload_file1():
    return render_template('upload.html')


@app.route('/maps', methods=['GET'])
def process():
    cursor = db.maps.find({}, {'_id': False})
    maps = {'maps': []}
    for doc in cursor:
        try:
            maps['maps'].append(doc['filename'])
        except Exception:
            pass
    return dumps(maps)


@app.route('/map/<map_name>', methods=['GET'])
def get_map(map_name):
    doc = db.maps.find_one({'filename': map_name}, {'_id': False})
    return dumps(doc)


@app.route('/uploader', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        f = request.files['file']
        parsed = loads(f.read())
        filename = ''.join(f.filename.split('.')[:-1]) + str(index) + '.json'
        parsed['filename'] = filename
        try:
            result = db.maps.insert_one(parsed)
            return 'File uploaded successfully'
        except Exception:
            return 'Error while uploading a file'


if __name__ == '__main__':
    global index
    index = len(os.listdir('maps')) + 1
    app.run(debug=True)
