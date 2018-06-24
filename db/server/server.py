from flask import Flask, request, Response
from pymongo import MongoClient
import os
from json import dumps, loads
from datetime import datetime

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
resp.headers[
    'Access-Control-Request-Headers'] = '''Content-Type, Cache-Control, X-Requested-With'''


@app.route('/maps', methods=['GET', 'OPTIONS'])
def process():
    global resp
    if request.method == "OPTIONS":
        print(resp.headers)
        return resp
    cursor = db.maps.find({}, {'_id': False})
    maps = {'maps': []}
    for doc in cursor:
        try:
            maps['maps'].append(doc['filename'])
        except Exception:
            pass
    resp.set_data(dumps(maps))
    return resp


@app.route('/', methods=['GET'])
def id():
    return dumps({"success": True})


@app.route('/map/<map_name>', methods=['GET', 'OPTIONS'])
def get_map(map_name):
    global resp
    if request.method == "OPTIONS":
        print(resp.headers)
        return resp
    doc = db.maps.find_one({'filename': map_name}, {'_id': False})
    resp.set_data(dumps(doc))
    return resp


@app.route('/delete_all', methods=['GET', 'OPTIONS'])
def delete():
    db.maps.remove()
    resp.set_data(dumps({"success": True}))
    return resp


@app.route('/uploader', methods=['GET', 'POST', 'OPTIONS'])
def upload_file():
    global resp
    if request.method == "OPTIONS":
        return resp
    f = request.files['qqfile']
    parsed = loads(f.read())
    parsed['filename'] = f.filename
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


@app.route('/uploader_json', methods=['GET', 'POST', 'OPTIONS'])
def upload_json():
    global resp
    if request.method == "OPTIONS":
        return resp
    parsed = loads(request.get_data())
    time = datetime.strptime(parsed["createTime"], "%d/%m/%Y, %H:%M:%S")
    unique_file_name = "gameData" + str(time.day) + str(time.month) + str(
        time.year) + str(time.hour) + str(time.minute) + str(
            time.second) + ".json"
    parsed['filename'] = unique_file_name
    try:
        db.maps.insert_one(parsed)
        resp.set_data(dumps({"success": True}))
        return resp
    except Exception:
        resp.set_data(dumps({"success": False}))
        return resp


if __name__ == '__main__':
    app.run(debug=True)
