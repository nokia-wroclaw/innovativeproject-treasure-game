from flask import Flask, request, Response
from pymongo import MongoClient
import os
from json import dumps, loads

app = Flask(__name__)
app.config.from_object(__name__)

global index
map_dir = 'maps'

client = MongoClient(
    '''mongodb+srv://webapp:XD12345@game-7sfnp.mongodb.net/test''')

db = client.business

global resp
resp = Response(response=dumps({}), status=200, mimetype="application/json")
resp.headers['Access-Control-Allow-Origin'] = '*'
resp.headers[
    'Access-Control-Allow-Headers'] = '''Content-Type, Cache-Control, X-Requested-With'''


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


@app.route('/', methods=['GET'])
def id():
    return dumps({"success": True})


@app.route('/map/<map_name>', methods=['GET'])
def get_map(map_name):
    doc = db.maps.find_one({'filename': map_name}, {'_id': False})
    return dumps(doc)


@app.route('/uploader', methods=['GET', 'POST', 'OPTIONS'])
def upload_file():
    global resp
    if request.method == "OPTIONS":
        return resp
    f = request.files['qqfile']
    parsed = loads(f.read())
    filename = ''.join(f.filename.split('.')[:-1]) + str(index) + '.json'
    parsed['filename'] = filename
    try:
        db.maps.insert_one(parsed)
        resp.set_data(dumps({"success": True}))
    except Exception:
        resp.set_data(dumps({"success": False}))
    return resp
    # if True:
    #     print("used POST method")
    #     f = request.files['file']
    #     parsed = loads(f.read())
    #     filename = ''.join(f.filename.split('.')[:-1]) + str(index) + '.json'
    #     parsed['filename'] = filename
    #     try:
    #         result = db.maps.insert_one(parsed)
    #         resp = Response("success")
    #         resp.headers['Access-Control-Allow-Origin'] = '*'
    #         return resp
    #     except Exception as e:
    #         return dumps({"success": True})
    # return dumps({"success": False})


if __name__ == '__main__':
    global index
    index = len(os.listdir('maps')) + 1
    app.run(debug=True)
