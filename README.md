# Sitecore.FakeDb.RainbowSerialization
A library that deserializes Rainbow (.yml) files into FakeDb items and templates for the purpose of unit/integration testing Sitecore projects.

## What are Rainbow files?
Rainbow is a serialization framework that is an alternative to Sitecore's default serialization format and file system organization.  It is most often used by Unicorn.  The Github project can be found at: https://github.com/SitecoreUnicorn/Rainbow.

## When is this library helpful?
If you are using Unicorn with Rainbow serialization in your Sitecore projects this library can be used to quickly setup tests.  With a few lines of code you can add data from your serialized files to use in testing your code.

## How do you use this library?
This library adds an extension method with overloaded options to FakeDb's Db class.  The method AddYml() accepts 1 or more filepaths to Rainbow files or directories.  
    public Sitecore.FakeDb.Db WithSerializedFiles()
    {
        var db = new Sitecore.FakeDb.Db();
        db.AddYml(true,
            @"c:\project\src\serialization\templates",
            @"c:\project\src\serialization\content"
            );
        return db;
    }
