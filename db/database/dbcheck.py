from pymongo import MongoClient
import json

client = MongoClient(
    'mongodb+srv://webapp:XD12345@game-7sfnp.mongodb.net/test')

db = client.business

serverStatusResult = db.command('serverStatus')

# f = open('test_map.json', 'r')
# parsed = json.loads(f.read())

# print(parsed)

# print(serverStatusResult)
# result = db.maps.insert_one(parsed)
# result = db.maps.delete_many({})
print(db.collection_names())
cursor = db.maps.find({}, {'_id': False})
for doc in cursor:
    print(doc)
