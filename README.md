# TheBlackRoom C# Libraries

A collection of utility functions, extension methods, helpers and controls for the c# programming language.


## Organizational Notes

- Libraries are implemented as shared projects to cleanly integrate into existing projects
- Libraries are split by namespace as much as possible to reduce dependancies
- Libraries generally contain subfolders of one or more of the following groups, followed by a subfolder of of the target class or type
  - **Extensions**: Static extension methods for exsiting classes in the target namespace
  - **Utility**:    Static methods for exsiting classes in the target namespace
  - **Helpers**:    Wrapper classes that provide additional functionality for a class or control
- Libraries may instead extend the namespace with one of the following groups
  - **Controls**:   Custom or extended controls for System.Windows.Forms projects
  - **Binding**:    Classes and controls that center around data binding


## TheBlackRoom.Core

Library for types and classes contained the following namespaces:
- `System`
- `System.Collections.Generic`
- `System.ComponentModel`


## TheBlackRoom.Gfx

Library for types and classes contained the following namespaces:
- `System.Drawing`


## TheBlackRoom.WinForms

Library for types and classes contained the following namespaces:
- `System.Windows.Forms`


## TheBlackRoom.Utility

Miscellaneous utility bits and pieces
