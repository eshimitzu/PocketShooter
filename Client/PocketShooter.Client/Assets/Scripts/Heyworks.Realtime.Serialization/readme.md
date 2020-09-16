


Registration: 
- Code for all and for owner for replay storage (multi target seriazliation)
- Component type id
- hierarhical stucture 

Serialization:
- delta-diff via IComparable(e.g. -2 not equal but no serialize) and Default Equaltity Comparer
- Client Received sequence number (probably should not be part of Serialization)
- Client Send on Delta (probably should not be part of Serialization)
- Rule to stop deleta send (probably should not be part of Serialization)

- Generic  (delta, huffman, other)

    // public struct FloatCompression
    // {
    //     (float min, float max, float precision) Get(string fieldName)
    //     {
    // Field<MyState>(nameof(MyState.X))
    // if (fieldName == nameof(MyState.x))
    // {
    // what about hierarchy?  MyState.OtherState.x? 
    // what about MyStruct.ThirdPartyStruct.X? 
    // What about fields of readonly properties?
    //
    // # Attributes:
    // - no need to pass compressor explicitly
    // - works in hiearchy easily
    // ## bad:
    // - one class may have only one precisions and only one (or allow to set multiple attributes describing e.g. send commands and receive remote)
    // - cannot tag other classes (e.g. Unity vector) - will still need attribute with custom compressor or custom compressor instance,
    // - or can do unsafe cast into my struct tagget (if we use third party it means it is stable and will not change)
    // - cannot be optimzied yet by generic attributes in C# 8
    //
    // # Generic compressers as other classes
    // - will need to type all classes again as if write directly.... (better is check cast like Write<MyTagget>(in thirdPartyNotTagged)) and check types match one
    //
    // # Self serialization of each struct with params into methods