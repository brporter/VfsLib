VfsLib
======

An implementation of a composing virtual path provider for ASP.NET.


What on earth is a 'composing' virtual path provider?
-----------------------------------------------------
Typically, a virtual path provider in ASP.NET acts as an enormous switch - it's either resolving files from a custom storage location somewhere, or its resolving files from the file system. What they do not do, mostly as an artifact of the design of the provider model, is compose the two file sources. For example, let's say you have a directory on your physical file system at ~/foo. All files within ~/foo must reside on the physical file system - you can't have virtual files within ~/foo as well.

A composing virtual path provider actually knits together the two views, so a directory structure such as:

* Foo/
  * Bar.html
  * Baz.html
* Biz/
  * Fiz.html
  * Fuz.html

The HTML files may be either virtual or physical files! 
