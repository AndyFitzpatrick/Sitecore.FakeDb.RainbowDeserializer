# Sitecore.FakeDb.RainbowSerialization
A library that deserializes Rainbow (.yml) files into FakeDb items and templates for the purpose of unit/integration testing Sitecore projects.

## What are Rainbow files?
Rainbow is a serialization framework that is an alternative to Sitecore's default serialization format and file system organization.  It is most often used by Unicorn.  The Github project can be found at: https://github.com/SitecoreUnicorn/Rainbow.

## What is Sitecore FakeDb?
FakeDb is a testing framework that mocks the Sitecore context for your unit/integration tests.  Code that integrates with Sitecore systems can be tested with relative ease.  The Github project can be found at: https://github.com/sergeyshushlyapin/Sitecore.FakeDb

## When is this library helpful?
If you are using Unicorn with Rainbow serialization in your Sitecore projects this library can be used to quickly setup tests.  With a few lines of code you can add data from your serialized files to use in testing your code.

## How do you use this library?
This library adds a couple extension methods to FakeDb's Db class.  The most important method, AddYml(), accepts 1 or more filepaths to Rainbow files or directories as can be seen in the example below:
```c#
    public Sitecore.FakeDb.Db WithSerializedFiles()
    {
        var db = new Sitecore.FakeDb.Db();
        db.AddYml(true,
            @"c:\project\src\serialization\templates",
            @"c:\project\src\serialization\content"
            );
        return db;
    }
```

AddYml() also accepts a boolean parameter to determine if the serialized items should be merged with existing database entries.  Set this to false for the fastest performance.

Once called, AddYml will try to find a file or directory for each of the file paths.  If found, it will recursively look through the file system for child serialized items.  It will then add these as items or templates to the FakeDb database.