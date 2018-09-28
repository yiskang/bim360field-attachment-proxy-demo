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

$( document ).ready(function() {
  $( 'button#token-fetcher' ).click(function() {
    $.post(
      '/api/authentication/authenticate',
      {
        account: 'example@example.org',   //!<<< User account in your own user management system, not in BIM360 Field
        password: '123456'
      }
    )
    .done(function( data ) {
      if( !data || !data['access_token'] )
        return alert( 'Failed to fetch token' );
      
        $( 'input#token' ).val( data['access_token'] );
        $( 'button#img-fetcher' ).attr('disabled', false);
    })
    .fail(function( error ) {
      alert( error );
    });
  });

  $( 'button#img-fetcher' ).click(function() {
    const token = $( 'input#token' ).val();
    const projectId = $( 'input#projectId' ).val();
    const objectId = $( 'input#objectId' ).val();

    if( !token || !projectId || !objectId )
      return alert( 'Invalid token or project id or image id');

    const param = {
      token: token,
      projectId: projectId,
      objectId: objectId
    };

    const query = $.param( param );
    $( 'img#img-holder' ).attr( 'src', '/api/attachments?' + query );
  });  
});