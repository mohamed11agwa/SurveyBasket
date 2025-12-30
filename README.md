📝 Survey Management System

A RESTful Survey Management System built with ASP.NET Core Web API, enabling: Survey creation and management, Question management, Response collection, Secure authentication using JWT
--------------------------------------------------------------
🔐 Authentication & Token Strategy
This system uses Access Tokens and Refresh Tokens to ensure security and a smooth user experience.

🔑 Access Token

Short-lived token
Used to access protected API endpoints
Contains security measures such as digital signatures

🔄 Refresh Token

Long-lived token
Used to obtain a new access token without requiring the user to log in again
Improves user experience while maintaining security

-------------------------------------------------------------------------
🔁 Refresh Token Flow


1 - The client application requests an access token from the authentication server.

2 - The tokens (both access and refresh) are sent to the client application.

3 - The client application then sends the access token to the resource server.

4 - If successful, the resource server returns the protected resource (This continues as long as the access token is valid.)

5 - If the access token has expired, an invalid token error is sent to the client application.

6 - If a refresh token was acquired at authorization grant, the client application sends the refresh token to the authentication server.

7 - The authentication server returns a new set of tokens (both access and refresh).
<img width="913" height="641" alt="refresh-token-flow" src="https://github.com/user-attachments/assets/161a0673-fb31-4e64-b584-152aabb716b4" />
