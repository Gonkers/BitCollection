# BitCollection
Inspired by BitVector32 and BitArray, I created something of a cross between the two. 👹

> I've got a bad feeling about this.
> - Princess Leia, Obi-Wan Kenobi, Anakin Skywalker, Luke Skywalker, C-3PO, Han Solo, BB-8, Lando Calrissian

Pull requests welcome, use at your own risk, the Star Wars universe has warned you.

The hope for this was to create an efficient (small), yet well defined, JWT claim that could
store user permissions or other flags. For instance when base64 encoding 128 different flags
(bits) it can be stored in a user claim as short as 22 characters. If each resource knows that
bit 7 is the permission to create new users you can use the claim to enable and disable user
controls to without needing to make calls to the user permissions store, etc.

Feel free to fork or submit a pull request if you feel you have an improvement. If you find a
bug file an issue and I'll  try to respond in a timely manner.

# BitFlags
Given a set of key value pairs that consist of numeric values only, encode them into an arbitrary number of bits. The number of bits allocated to the key and value should be defined in the first byte.

```
          K1          V1 K2          V2
 01   02  |           |  |           | 
|--| |--| KKKK KKKK KKVV KKKK KKKK KKVV
1001 0001 0000 0000 0000 0000 0000 0000
| 9| | 1|
  |    |
  |    1 + 1 bits or 4 possible values
  9 + 1 bits or 1024 possible keys
---
```
**01** - indicates the bit length of the key, 1 to 16 bits

**02** - indicates the bit length of the value, 1 to 16 bits