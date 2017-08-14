# Razor Pages Minification

This sample illustrates the use of [WebMarkupMin.Core](https://www.nuget.org/packages/WebMarkupMin.Core/) to minify the markup of Razor pages prior to the Razor engine rendering the pages.

A custom Razor Template Engine is used ...

```csharp
services.AddSingleton<RazorTemplateEngine, CustomRazorTemplateEngine>();
```

... to replace the `CreateCodeDocument` method. This allows us to hack the `inputStream` for the incoming markup, pass it off to `WebMarkupMin.Core`, then shoot it back to Razor to create the `RazorCodeDocument` ...

```csharp
using (var inputStream = projectItem.Read())
{
    using (var reader = new StreamReader(inputStream))
    {
        var text = reader.ReadToEnd();

        var markupStart = text.IndexOf("<!DOCTYPE");
        var directives = text.Substring(0, markupStart);
        var markup = text.Substring(markupStart);

        text = directives + Minify(markup);

        var byteArray = Encoding.UTF8.GetBytes(text);
        var minifiedInputStream = new MemoryStream(byteArray);

        var source = RazorSourceDocument.ReadFrom(minifiedInputStream, projectItem.Path);
        var imports = GetImports(projectItem);

        return RazorCodeDocument.Create(source, imports);
    }
}
```

`WebMarkupMin.Core` isn't so good at parsing Razor in a Razor page, so see the *Index.cshtml* page below to see how to escape sections of Razor code from the minification engine. Note that the use of `<text>` tags prevents extra spaces and CR's from those Razor line transitions from making their way into the rendered markup.

The *Index.cshtml* file starts as ...

```html
@page
@model IndexModel
@inject Microsoft.AspNetCore.Hosting.IHostingEnvironment Host
<!DOCTYPE html>
<html lang="en">
<head>
    <title>Razor Pages Minification App</title>
</head>
<body>
    <h1>Razor Pages Minification App</h1>
    <p>@Host.ApplicationName @Model.DT</p>
    <figure style="text-align:center;width:400px">
        <img alt="Jack Burton" src="image.png">
        <figcaption>Jack Burton (Kurt Russell)<br><em>Big Trouble in Little China</em><br>&copy;1986 <a href="http://www.foxmovies.com/">20th Century Fox</a></figcaption>
    </figure>
    <p>
        People:
        <ol>
            <!--wmm:ignore-->@for (var i = 0; i < Model.People.Count; i++)
            {
                var person = Model.People[i];
                <text><li>@person</li></text>
            }<!--/wmm:ignore-->
        </ol>
    </p>
    <p>
        <!--wmm:ignore-->@if (Model.People.Count < 10)
        {
            <text>There are less than ten people.</text>
        }<!--/wmm:ignore-->
    </p>
</body>
</html>
```

The *Index.cshtml* file renders as ...

> \<!DOCTYPE html>\<html lang=en>\<head>\<title>Razor Pages Minification App\</title>\<body>\<h1>Razor Pages Minification App\</h1>\<p>RazorMinification 8/14/2017 4:24:29 PM\</p>\<figure style=text-align:center;width:400px>\<img alt="Jack Burton" src=image.png>\<figcaption>Jack Burton (Kurt Russell)\<br>\<em>Big Trouble in Little China\</em>\<br>&copy;1986 \<a href="http://www.foxmovies.com/">20th Century Fox\</a>\</figcaption>\</figure>\<p>People:\<ol>\<li>Susan\</li>\<li>Catalina\</li>\<li>Diego\</li>\<li>Bob\</li>\</ol>\<p>There are less than ten people.

![Rendered page](image.png)
