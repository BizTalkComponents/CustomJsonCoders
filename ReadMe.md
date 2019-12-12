
# Custom Json Coders
Encoding/Decoding JSON arrays in BizTalk, besides JSON objects as usual in BizTalk.
The components are based on the usual BizTalk JSON Encoder/JSON Decoder, there are some additional properties to handle the JSON array.
<br>
## Decoder:

When the incoming json is a Json array, the component encapsulates the json array within an object, the array is given a name from the property **ArrayNodeName**.<br>
The generated xml will then include the root node and one undound child which is the array, if ArrayNodeName is left empty, then the default value will be used, the default value is "<ins>data</ins>".
The decoder still can decode the usual JSON object, and if incoming json is not detected as a json array, then it will be handled exactly the same as usual BizTalk JSON decoder.<br>

## Encoder:
Allows to generate a Json array output instead of usual Json object, but it requires that array should be available as the only root node child.
Set the property **Array Output** to true to generate Json array output, otherwise it will generate the usual json output from BizTalk.
