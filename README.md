![Platforms](https://img.shields.io/badge/platform-windows%20%7C%20osx%20%7C%20linux-lightgray.svg)
[![License](http://img.shields.io/:license-mit-blue.svg)](http://opensource.org/licenses/MIT)

# BIM360 Field Attachment API Proxy

## Overview

This is a example for securing your BIM360 Field attachment accessing with a self implement  token system - [JWT](https://jwt.io/).

In this demo, we wrap the Field API calls in our own server codes implemented with ASP.Net Core Web API, in order to protect your Field ticket while sharing Field attachments to others.

## Requirements

* .net Core SDK v2.1 and later [link](https://www.microsoft.com/net/download)
* [Visual Studio 2017](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://visualstudio.microsoft.com/) with the [C# extension](https://code.visualstudio.com/docs/languages/csharp)

<a name="setup"></a>

## Setup

1. Download and install [net Core SDK](https://www.microsoft.com/net/download)

2. Download and install [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://visualstudio.microsoft.com/)

3. Download this repo anywhere you want

4. Implement your own ValidateUser function to make content access control in the [Controllers/AuthenticationController.cs](Controllers/AuthenticationController.cs), you can integrate with your own user management system here.

    ```csharp
    private bool ValidateUser (User user) {
      // Check user existence in your own user management system, not in BIM360 Field
      if (user.account == "example@example.org" && user.password == "123456")
        return true;

      return false;
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Authenticate ([FromForm] User user) {
      if (!this.ValidateUser (user))
        return Unauthorized();

      // ....
    }
    ```
5. Change JWT token lifetime here in the [Controllers/AuthenticationController.cs](Controllers/AuthenticationController.cs)

    ```csharp
    var expires = utcNow.AddHours (1); //!<<< Change this to extend lifetime of the self maintained token

    // e.g. Change to 5 days
    var expires = utcNow.AddDays (5);
    ```

6. Replace YOUR_BIM360Field_API_USER and YOUR_BIM360Field_API_USER_PASS to your Field user identity in the [Controllers/AttachmentsController.cs](Controllers/AttachmentsController.cs)

    ```Csharp
    request.AddParameter ("username", "YOUR_BIM360Field_API_USER", ParameterType.GetOrPost);
    request.AddParameter ("password", "YOUR_BIM360Field_API_USER_PASS", ParameterType.GetOrPost);
    ```

7. If you want to play with the demo web page, change user credentials here in the [wwwroot/js/index.js](wwwroot/js/index.js) (***Optional***)

    ```javascript
    $.post(
      '/api/authentication/authenticate',
      {
        account: 'example@example.org',   //!<<< User account in your own user management system, not in BIM360 Field
        password: '123456'
      }
    )
    ```

8. Build and run in Visual Studio

<a name="UseOfTheSample"></a>

## Use of the sample

1. Press F5 to run this project in debug mode of the Visual Studio

2. Open browser and navigate to http://localhost:5000 or http://localhost:5001, it will show a demo web page (***Optional***)

    <img src="wwwroot/img/Webpage.jpg" width="700" height="400">

3. Use REST API client like [Postman](https://www.getpostman.com/) and call `POST /api/authentication/authenticate` to obtain our own token

    <img src="wwwroot/img/FetchToken.jpg" width="700" height="400">

4. Get both project id and attachment id of the image you want to share to others in your BIM360 Field account.

5. Use REST API client like [Postman](https://www.getpostman.com/) and call `GET /api/attachments` to access the image attachment. The API call should like this:

```hyperlink
http://localhost:5000/api/attachments?objectId={FIELD_ATTACHMENT_ID}&projectId={FIELD_PROJECT_ID}&token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJleGFtcGxlQGV4YW1wbGUub3JnIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVmlld2VyIiwiZXhwIjoxNTM4MTE0NTI3LCJpc3MiOiJBdXRvZGVzay5BRE4iLCJhdWQiOiJCSU0zNjAgRmllbGQgQVBJIFByb3h5In0.B2cj22Cb438vachMkCh8gQQ3-PxeH-TRlAN6vPOwmZs
```

6. Obtain the image from the API `GET /api/attachments` call above

    <img src="wwwroot/img/Result.jpg" width="700" height="400">


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.

## Written by

Eason Kang <br />
Autodesk Developer Network & Forge Partner Development<br />
https://developer.autodesk.com/ <br />
https://forge.autodesk.com/blog <br />