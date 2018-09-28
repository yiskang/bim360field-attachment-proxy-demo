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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Autodesk {
  [Route ("api/[controller]/[action]")]
  [ApiController]
  public class AuthenticationController : ControllerBase {
    private IConfiguration Configuration { get; }

    public AuthenticationController (IConfiguration configuration) {
      this.Configuration = configuration;
    }

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

      var claims = new [] {
        new Claim (JwtRegisteredClaimNames.NameId, user.account),
        new Claim (ClaimTypes.Role, "Viewer"),
      };

      var utcNow = DateTime.UtcNow;
      var expires = utcNow.AddHours (1); //!<<< Change this to extend lifetime of the self maintained token
      var timespan = expires.Subtract (utcNow);

      var token = new JwtSecurityToken (
        issuer: Configuration["JWT:Issuer"],
        audience : Configuration["JWT:Audience"],
        claims : claims,
        expires : expires,
        signingCredentials : new SigningCredentials (new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration["JWT:IssuerSigningKey"])),
          SecurityAlgorithms.HmacSha256)
      );

      var response = new {
        access_token = new JwtSecurityTokenHandler ().WriteToken (token),
        expires_in = timespan.TotalSeconds
      };

      return Ok (response);
    }
  }
}