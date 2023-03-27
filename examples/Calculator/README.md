# Calculator
This is a full calculator written in spaghetto with support for the 4 basic operations (+-*/) and order of operations (including parentheses).

## How to execute
```cs
import "test/run.spag";
// This will automatically execute one calculation, you can edit run.spag to not run the calculation at import.

calc("(3 + 3) * 3");
```

## Identified problems
> **Note**
> I primarily write these examples to test the language myself and to identify issues.
  - Enums do not exist but those are important in many applications. Whilst they can be "emulated" by constant values, its not as convienient.
  - +=/-=/++/etc. do not work on dot nodes, they only work on direct identifiers.
  - Errors are unclear, there should at **the very least** be a position in runtime errors.
  - != operator is completely missing
  - '' should either be a special type SChar or just be parsed as strings, but we should not make that cause an error.
  - Props are non-fixed type by default it seems. Not sure about that though.
  - We should have "method like properties", like `prop current => self.peek(0);` *q: should self be passed implicitly or somehow explictely?*
  - I have encountered several bugs and I fixed those. However, this proves that real world tests are important.
