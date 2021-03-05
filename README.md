# mvp24hours-entity-t4
How to use the Mvp24Hours.Entity.SqlServer and Mono.TextTemplating library to generate templates with T4.

## How to configure?
Add the configuration below to the .csproj file

```
  <ItemGroup>
    <DotNetCliToolReference Include="dotnet-t4-project-tool" Version="2.0.5" />
    <PackageReference Include="Mono.TextTemplating" Version="2.0.5" />
    <PackageReference Include="Mvp24Hours.Entity.SqlServer" Version="1.0.0" />
    <TextTemplate Include="**\*.tt" />
    <Generated Include="**\*.Generated.cs" />
  </ItemGroup>

  <Target Name="TextTemplateTransform" BeforeTargets="BeforeBuild">
    <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet t4 %(TextTemplate.Identity) -c $(RootNameSpace).%(TextTemplate.Filename) -o %(TextTemplate.Filename).Generated.cs" />
  </Target>

  <Target Name="TextTemplateClean" AfterTargets="Clean">
    <Delete Files="@(Generated)" />
  </Target>
  
```

Create a file for the ITextTemplate interface:

```
    public interface ITextTemplate
    {
        string TransformText() => throw new NotImplementedException();
    }
	
```

Create the file that will represent the entity model or models loaded for use in ".tt", for example (ModelTemplate.cs):

```
    public partial class ModelTemplate : ITextTemplate
    {
        public readonly ResultSettings Models;
        public readonly EntitySettings Entity;

        public ModelTemplate(ResultSettings models, EntitySettings entity)
            => (Models, Entity) = (models, entity);
    }

```

Create a file with the extension ".tt" with the same name as the previous class, for example (ModelTemplate.tt):

```
<#@ template language="C#" #>
<#@ assembly name="System.Core" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Collections.Generic" #>

<#=Entity.Name#>

```

The same properties of your "ModelTemplate" class can be used in your "ModelTemplate.tt" template.

Ready! Now just run your "Program.cs" with combining the template with the models loaded from the database:

```
    var mvpSettings = new Mvp24Settings()
    {
        Database = new DatabaseSettings()
        {
            DataSource = "",
            Catalog = "",
            TrustedConnection = true
        },
        IdKeyName = true, // Use if the columns that represent your table's primary key have the name "ID"
        TableFilter = "", // Use to filter the tables you want to read
    };

    var dbEntity = new DbEntitySqlServer(mvpSettings);
    var settings = dbEntity.GetSettings();

    if (settings != null)
    {
        System.Console.WriteLine("Writing processed results in the \"Models\" folder..");

        string pathDir = ""; // What is the output directory of the processed files?

        foreach (var entity in settings.Entities)
        {
            System.Console.WriteLine($"Writing {entity.ClassName}.cs");
            ITextTemplate template = new ModelTemplate(settings, entity);
            File.WriteAllText($"{pathDir}{entity.ClassName}.cs", template.TransformText());
        }
    }
```

`When the project is compiled, a "Generated" file (ModelTemplate.Generated.cs) will be generated that corresponds to the processed template.`

## How to Configure Using a JSON File?

If you want to use a configuration file, create a ".json" file with the following structure:

```

{
  "database": {
    "datasource": "",
    "catalog": "",
    "trustedconnection": "true",
    "username": "",
    "password": ""
  },
  "generate": true,
  "schemafilter": "",
  "tablefilter": "",
  "idkeyname": false
}

```

Just inform the name of the file that is in the same directory when creating the "DbEntitySqlServer" object, like this:

```
    var dbEntity = new DbEntitySqlServer("mvp24hours-entity-settings.json");
    var settings = dbEntity.GetSettings();	
```

