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
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Autodesk {
  public class Startup {
    public Startup (IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices (IServiceCollection services) {
      services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer (options => {
          options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = Configuration["JWT:Issuer"],
              ValidAudience = Configuration["JWT:Audience"],
              IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration["JWT:IssuerSigningKey"])), //!<< Generate by node -e "console.log(require('crypto').randomBytes(256).toString('base64'));"
              RequireExpirationTime = true
          };

          options.Events = new JwtBearerEvents {
            OnMessageReceived = ctx => {
              // replace "token" with whatever your param name is
              if (ctx.Request.Method.Equals ("GET") && ctx.Request.Query.ContainsKey ("token"))
                ctx.Token = ctx.Request.Query["token"];
              return Task.CompletedTask;
            }
          };
        });

      services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment ()) {
        app.UseDeveloperExceptionPage ();
        //   } else {
        //     app.UseHsts ();
      }

      app.UseAuthentication ();

      //app.UseHttpsRedirection ();

      app.UseStaticFiles ();

      app.UseMvcWithDefaultRoute ();
    }
  }
}