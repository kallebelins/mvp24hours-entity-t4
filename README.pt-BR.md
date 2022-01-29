# mvp24hours-entity-t4
Como usar a biblioteca Mvp24Hours.Entity.SqlServer e Mono.TextTemplating para gerar modelos com T4.

## Ferramentas
Rode o comando abaixo para instalar a engine de template T4:

```
dotnet tool install -g dotnet-t4
```

## Como configurar?
Adicione a configuração abaixo ao arquivo .csproj:

```
<ItemGroup>
	<PackageReference Include="Mono.TextTemplating" Version="2.0.5" />
	<TextTemplate Include="**\*.tt" />
	<Generated Include="**\*.Generated.cs" />
</ItemGroup>

<Target Name="TextTemplateTransform" BeforeTargets="BeforeBuild">
	<Exec WorkingDirectory="$(ProjectDir)" Command="t4 %(TextTemplate.Identity) -c $(RootNameSpace).%(TextTemplate.Filename) -o %(TextTemplate.Filename).Generated.cs" />
</Target>

<Target Name="TextTemplateClean" AfterTargets="Clean">
	<Delete Files="@(Generated)" />
</Target>
  
```

Crie um arquivo para a interface ITextTemplate:

```
public interface ITextTemplate
{
    string TransformText() => throw new NotImplementedException();
}	
```

Crie o arquivo que representará o modelo da entidade ou modelos carregados para uso no ".tt", por exemplo (ModelTemplate.cs):

```
public partial class ModelTemplate : ITextTemplate
{
    public readonly ResultSettings Models;
    public readonly EntitySettings Entity;

    public ModelTemplate(ResultSettings models, EntitySettings entity)
        => (Models, Entity) = (models, entity);
}
```

Crie um arquivo com a extensão ".tt" com o mesmo nome da classe anterior, por exemplo (ModelTemplate.tt):

```
<#@ template language="C#" #>
<#@ assembly name="System.Core" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Collections.Generic" #>

<#=Entity.Name#>
```

As mesmas propriedades da classe "ModelTemplate" podem ser usadas no modelo "ModelTemplate.tt".

Pronto! Agora basta executar o seu "Program.cs" combinando o template com os modelos carregados do banco de dados:

```
var mvpSettings = new Mvp24Settings()
{
    Database = new DatabaseSettings()
    {
        DataSource = "",
        Catalog = "",
        TrustedConnection = true
    },
    IdKeyName = true, // Use se as colunas que represetam a chave primária da sua tabela tiverem o nome "ID"
    TableFilter = "", // Use para filtrar as tabelas que deseja ler
};

var dbEntity = new DbEntitySqlServer(mvpSettings);
var settings = dbEntity.GetSettings();

if (settings != null)
{
    System.Console.WriteLine("Writing processed results in the \"Models\" folder..");

    string pathDir = ""; // Qual é o diretório de saída dos arquivos processados?

    foreach (var entity in settings.Entities)
    {
        System.Console.WriteLine($"Writing {entity.ClassName}.cs");
        ITextTemplate template = new ModelTemplate(settings, entity);
        File.WriteAllText($"{pathDir}{entity.ClassName}.cs", template.TransformText());
    }
}
```

`Quando o projeto for compilado, será gerado um arquivo "Generated" (ModelTemplate.Generated.cs) que corresponde ao modelo processado.`

## Como configurar usando um arquivo JSON?

Se você quiser usar um arquivo de configuração, crie um arquivo ".json" com a seguinte estrutura:

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

Basta informar o nome do arquivo que está no mesmo diretório ao criar o objeto "DbEntitySqlServer", assim:

```
    var dbEntity = new DbEntitySqlServer("mvp24hours-entity-settings.json");
    var settings = dbEntity.GetSettings();	
```

