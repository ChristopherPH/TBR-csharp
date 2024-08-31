# TheBlackRoom Libraries

A collection of utility functions, extension methods, helpers and controls for the c# programming language.


## Organizational Notes

- Collections are implemented as shared projects to cleanly integrate into existing projects
- Collections are split by namespace as much as possible to reduce dependancies
- Collections generally contain subfolders of one or more of the following groups, followed by a subfolder of of the target class or type
  - **Extensions**: Static extension methods for exsiting classes in the target namespace
  - **Utility**:    Static methods for exsiting classes in the target namespace
  - **Helpers**:    Wrapper classes that provide additional functionality for a class or control
- Collections may instead extend the namespace with one of the following groups
  - **Controls**:   Custom or extended controls for System.Windows.Forms projects
  - **Binding**:    Classes and controls that center around data binding


## TheBlackRoom.System

Additions to the `System` namespace


## TheBlackRoom.Gfx

Additions to the `System.Drawing` namespace


## TheBlackRoom.WinForms

Additions to the `System.Windows.Forms` namespace


## TheBlackRoom.Utility

Miscellaneous utility bits and pieces
