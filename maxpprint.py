def allowed(c: str):
    return c.isalpha()

mx = {}
for s in map(lambda q: q.lower(), open('words.txt')):
    seen = set()
    for c in s:
        if c in seen or not allowed(c):
            continue
        mx.setdefault(c, 0)
        mx[c] = max(mx[c], s.count(c))
        seen.add(c)
for l, v in mx.items():
    print(l, v)