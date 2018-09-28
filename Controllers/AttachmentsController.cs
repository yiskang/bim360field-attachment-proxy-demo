//
// Copyright (c) Autodesk, Inc. All rights reserved
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// by Eason Kang - Autodesk Developer Network (ADN)
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Autodesk.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class AttachmentsController : ControllerBase {
    private const string baseURL = "https://bim360field.autodesk.com";

    private IConfiguration Configuration { get; }

    public AttachmentsController (IConfiguration configuration) {
      this.Configuration = configuration;
    }

    private Identity FetchTicket () {
      var client = new RestClient (baseURL);
      var request = new RestRequest ("/api/login", Method.POST);
      request.AddHeader ("Cache-Control", "no-cache");
      request.AddHeader ("Content-Type", "application/x-www-form-urlencoded");

      // Replace YOUR_BIM360Field_API_USER and YOUR_BIM360Field_API_USER_PASS to your Field user identity
      request.AddParameter ("username", "YOUR_BIM360Field_API_USER", ParameterType.GetOrPost);
      request.AddParameter ("password", "YOUR_BIM360Field_API_USER_PASS", ParameterType.GetOrPost);
      IRestResponse response = client.Execute (request);

      return JsonConvert.DeserializeObject<Identity> (response.Content);
    }

    // GET api/values
    [HttpGet]
    [Authorize]
    public ActionResult Get ([FromQuery] string objectId, [FromQuery] string projectId) {
      var identity = this.FetchTicket ();

      if (identity == null || string.IsNullOrEmpty (identity.ticket)) {
        var error = new {
        type = "BIM360 Field",
        message = "Not Authored"
        };
        return StatusCode (StatusCodes.Status401Unauthorized, error);
      }

      var client = new RestClient (baseURL);
      var request = new RestRequest ("/api/binary_data", Method.POST);
      request.AddHeader ("Cache-Control", "no-cache");
      request.AddHeader ("Content-Type", "application/x-www-form-urlencoded");

      request.AddQueryParameter ("object_id", objectId);
      request.AddQueryParameter ("object_type", "Attachment");
      request.AddQueryParameter ("image_type", "original");
      request.AddQueryParameter ("page", "0");

      request.AddParameter ("ticket", identity.ticket, ParameterType.GetOrPost);
      request.AddParameter ("project_id", projectId, ParameterType.GetOrPost);
      IRestResponse response = client.Execute (request);

      if (response.StatusCode != HttpStatusCode.OK)
        return NotFound (string.Format ("Requested image `{0}` not found", objectId));

      return File (response.RawBytes, response.ContentType);
    }
  }
}