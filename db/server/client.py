import http.client

c = http.client.HTTPConnection('localhost', 5000)
try:
    c.request('GET', '/maps', '{}')
    doc = c.getresponse().read()
    print(doc)
except ConnectionRefusedError:
    print('Server not available')
