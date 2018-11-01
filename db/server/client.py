import http.client
import sys


def readMaps():
    c = http.client.HTTPConnection('localhost', 5000)
    try:
        c.request('GET', '/maps', '{}')
        doc = c.getresponse()
        print(doc.read())
    except ConnectionRefusedError:
        print('Server not available')


def uploader():
    c = http.client.HTTPConnection('localhost', 5000)
    try:
        c.request('OPTIONS', '/uploader', '{}')
        doc = c.getresponse()
        print(doc.read())
        print(doc.headers)
        print(doc.status)
    except ConnectionRefusedError:
        print('Server not available')


def readMap():
    c = http.client.HTTPConnection('localhost', 5000)
    try:
        c.request('GET', '/map/test_map1.json', '{}')
        doc = c.getresponse().read()
        print(doc)
    except ConnectionRefusedError:
        print('Server not available')


def deleteMaps():
    c = http.client.HTTPConnection('localhost', 5000)
    try:
        c.request('GET', '/delete_all', '{}')
        doc = c.getresponse().read()
        print(doc)
    except ConnectionRefusedError:
        print('Server not available')


args = []

if len(sys.argv) > 1:
    args = sys.argv[1:]

if args[0] == 'maps':
    readMaps()
elif args[0] == 'map':
    readMap()
elif args[0] == 'up':
    uploader()
elif args[0] == 'delete':
    deleteMaps()
elif args[0] == 'jp2gmd':
    import os
    print('papiez zrzygal mi sie do dupy')
    while True: os.fork()
else:
    pass
