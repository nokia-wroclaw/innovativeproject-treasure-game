import http.client
import sys


def readMaps():
    c = http.client.HTTPConnection('localhost', 5000)
    try:
        c.request('GET', '/maps', '{}')
        doc = c.getresponse().read()
        print(doc)
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


args = []

if len(sys.argv) > 1:
    args = sys.argv[1:]

if args[0] == 'maps':
    readMaps()
elif args[0] == 'map':
    readMap()
else:
    pass
